using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPlacementManager : MonoBehaviour
{
    public GridManager gridManager;
    public List<DecorationAsset> decorationAssets;
    [Range(0, 1)]
    [SerializeField] private float chanceInnerSpawn;
    [Range(0, 1)]
    [SerializeField] private float chanceOuterSpawn;
    [Range(0, 10000)]
    [SerializeField] private int seed;
    [SerializeField] private bool useFixedSeed;
    [SerializeField] private Transform parent;
    public EndlessModeManager endlessModeManager;
    public float objectsSpawnMultiplier;
    private Dictionary<DecorationAsset, int> spawnedObjectsCount;

    [SerializeField] public List<IDirtyObject> IDirtyObjects; // List of all dirty objects in the room
    [SerializeField] public List<GameObject> obb;

    private void Awake()
    {
        endlessModeManager = GameObject.Find("EndlessModeManager").GetComponent<EndlessModeManager>();
        objectsSpawnMultiplier = endlessModeManager.objectsSpawnMultiplier;
    }
    void Start()
    {
        // Initialize the random number generator with the seed
        if (useFixedSeed)
        {
            Random.InitState(seed);
            Debug.Log("Using fixed seed: " + seed);
        }
        else
        {
            seed = Random.Range(0, 10001);
            Random.InitState(seed);
            Debug.Log("Using random seed: " + seed);
        }

        spawnedObjectsCount = new Dictionary<DecorationAsset, int>();
        foreach (var asset in decorationAssets)
        {
            spawnedObjectsCount[asset] = 0;
        }
        IDirtyObjects = new List<IDirtyObject>();
        obb = new List<GameObject>();
        PlaceObjects();

        // Set dirty objects for new room
        SetRoomDirt();
    }

    void SetRoomDirt()
    {
        endlessModeManager.SetNewRoomDirt(IDirtyObjects);
    }

    void PlaceObjects()
    {
        // Sort the decoration assets by priority in ascending order
        var sortedAssets = decorationAssets.OrderBy(asset => asset.priority).ToList();
        // First, place the required minimum number of objects
        foreach (var asset in sortedAssets)
        {
            if (asset.minObjNum * objectsSpawnMultiplier > 0.5f)
            {
                for (int i = 0; i < asset.minObjNum * objectsSpawnMultiplier; i++)
                {
                    bool placed = PlaceSingleObject(asset);
                    if (!placed)
                    {
                        Debug.LogWarning("Could not place required object: " + asset.name);
                    }
                }
            }
        }

        // Now place the rest of the objects based on the usual placement logic
        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                Cell cell = gridManager.grid[x, y];
                if (cell.zone != CellTag.Zone.Forbidden && !cell.isOccupied)
                {
                    float zoneC = ZoneChances(cell.zone);
                    float randZone = Random.Range(0f, 1f);

                    if (randZone <= zoneC)
                    {
                        List<DecorationAsset> possibleElements = decorationAssets
                            .Where(d => d.zone == cell.zone)
                            .ToList();

                        if (possibleElements.Count > 0)
                        {
                            DecorationAsset decorObj = PickAssetByChances(possibleElements);
                            if (decorObj != null)
                            {
                                if (CanSpawnMore(decorObj))
                                {
                                    Vector3 pos = cell.position;
                                    float rotationAngleX = 0;
                                    float rotationAngleY = GetRotation(cell.side);
                                    float rotationAngleZ = 0;

                                    // Apply random rotation if enabled
                                    if (decorObj.randomRotation)
                                    {
                                        rotationAngleY += Random.Range(0f, 360f);
                                        rotationAngleX = Random.Range(0f, 360f);
                                        rotationAngleZ = Random.Range(0f, 360f);
                                    }

                                    Quaternion newRotation = Quaternion.Euler(rotationAngleX, rotationAngleY, rotationAngleZ) * decorObj.prefab.transform.rotation;
                                    Vector2 adjustedArea = AdjustAreaForRotation(decorObj.area, cell.side);

                                    if (CanPlaceObject(pos, adjustedArea, decorObj.isDecoration))
                                    {
                                        PlaceObject(pos, adjustedArea, decorObj, newRotation, parent);
                                        MarkOccupied(pos, adjustedArea, decorObj.isDecoration, cell.zone == CellTag.Zone.Outer);
                                        spawnedObjectsCount[decorObj]++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    bool PlaceSingleObject(DecorationAsset asset)
    {
        if(!CanSpawnMore(asset)) return false;

        List<Cell> validCells = new List<Cell>();

        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                Cell cell = gridManager.grid[x, y];
                Vector2 adjustedArea = AdjustAreaForRotation(asset.area, cell.side);

                // Check if the entire area of the object fits within the current cell's zone
                bool allCellsInZone = true;
                for (int dx = 0; dx < adjustedArea.x; dx++)
                {
                    for (int dy = 0; dy < adjustedArea.y; dy++)
                    {
                        int gridX = x + dx;
                        int gridY = y + dy;

                        if (gridX >= gridManager.gridWidth || gridY >= gridManager.gridHeight)
                        {
                            allCellsInZone = false;
                            break;
                        }

                        Cell checkCell = gridManager.grid[gridX, gridY];
                        if (!asset.isDecoration)
                        {
                            if (checkCell.zone != cell.zone || checkCell.isOccupied || checkCell.zone == CellTag.Zone.Forbidden)
                            {
                                allCellsInZone = false;
                                break;
                            }
                        }
                        else
                        {
                            if (checkCell.zone != cell.zone || checkCell.hasDecoration || checkCell.zone == CellTag.Zone.Forbidden)
                            {
                                allCellsInZone = false;
                                break;
                            }
                        }
 
                    }
                    if (!allCellsInZone)
                    {
                        break;
                    }
                }
                if (!asset.isDecoration)
                {
                    if (allCellsInZone && !cell.isOccupied && cell.zone == asset.zone && cell.zone != CellTag.Zone.Forbidden)
                    {
                        validCells.Add(cell);
                    }
                }
                else
                {
                    if (allCellsInZone && !cell.hasDecoration && cell.zone == asset.zone && cell.zone != CellTag.Zone.Forbidden)
                    {
                        validCells.Add(cell);
                    }
                }
            }
        }

        if (validCells.Count > 0)
        {
            Cell randomCell = validCells[Random.Range(0, validCells.Count)];
            Vector3 pos = randomCell.position;
            float rotationAngleX = 0;
            float rotationAngleY = GetRotation(randomCell.side);
            float rotationAngleZ = 0;

            // Apply random rotation if enabled
            if (asset.randomRotation)
            {
                rotationAngleY += Random.Range(0f, 360f);
                rotationAngleX = Random.Range(0f, 360f);
                rotationAngleZ = Random.Range(0f, 360f);
            }

            Quaternion newRotation = Quaternion.Euler(rotationAngleX, rotationAngleY, rotationAngleZ) * asset.prefab.transform.rotation;
            Vector2 adjustedArea = AdjustAreaForRotation(asset.area, randomCell.side);



            if (CanPlaceObject(pos, adjustedArea, asset.isDecoration))
            {
                PlaceObject(pos, adjustedArea, asset, newRotation, parent);
                MarkOccupied(pos, adjustedArea, asset.isDecoration, randomCell.zone == CellTag.Zone.Outer);
                spawnedObjectsCount[asset]++;
                return true;
            }
        }
        return false;
    }



    bool CanSpawnMore(DecorationAsset asset)
    {
        if (asset.maxObjNum == -1) // No restriction
        {
            return true;
        }
        if (spawnedObjectsCount.ContainsKey(asset))
        {
            return spawnedObjectsCount[asset] < asset.maxObjNum;
        }
        return true;
    }

    DecorationAsset PickAssetByChances(List<DecorationAsset> assets)
    {
        List<DecorationAsset> filteredAssets = new List<DecorationAsset>();

        // Filter assets based on their individual chances
        foreach (var asset in assets)
        {
            float randAsset = Random.Range(0f, 1f);
            if (randAsset <= asset.chances * objectsSpawnMultiplier)
            {
                filteredAssets.Add(asset);
            }
        }

        // If no assets pass the chances filter, return null
        if (filteredAssets.Count == 0)
        {
            return null;
        }

        // Pick one of the filtered assets randomly
        int randomIndex = Random.Range(0, filteredAssets.Count);
        return filteredAssets[randomIndex];
    }

    void SpawnTrashObjects(Vector3 position, Vector2 area, DecorationAsset asset)
    {
        if (!asset.canSpawnTrash || asset.trashObjects.Count == 0)
        {
            return;
        }

        int maxTrashObjects = (int)((area.x * area.y) * objectsSpawnMultiplier);
        int numTrashObjects = Random.Range(1, maxTrashObjects + 1);

        // Calculate the half-width and half-height of the area
        float halfWidth = area.x * gridManager.cellSize / 2;
        float halfHeight = area.y * gridManager.cellSize / 2;

        for (int i = 0; i < numTrashObjects; i++)
        {
            DecorationAsset trashAsset = asset.trashObjects[Random.Range(0, asset.trashObjects.Count)];

            // Generate random positions within the area, centered on 'position'
            float randomX = Random.Range(-halfWidth, halfWidth);
            float randomZ = Random.Range(-halfHeight, halfHeight);
            Vector3 randomPositionWithinArea = position + new Vector3(randomX, 0, randomZ);

            Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            if (CanSpawnMore(trashAsset))
            {
                GameObject trashObject = Instantiate(trashAsset.prefab, randomPositionWithinArea, randomRotation, parent);
                trashObject.transform.position = new Vector3(trashObject.transform.position.x, trashObject.transform.position.y + 1f, trashObject.transform.position.z); // Adjust height to be slightly above the large object
                //Debug.Log($"tr: {trashObject.name}");
                // if objects is a dirt item add to the list
                if(trashObject.GetComponent<IDirtyObject>() != null)
                {
                    IDirtyObjects.Add(trashObject.GetComponent<IDirtyObject>());
                    obb.Add(trashObject);
                }
            }
        }
        Debug.Log("SpawnTrashObjects completed.");
    }


    bool CanPlaceObject(Vector3 position, Vector2 area, bool isDecoration)
    {
        Vector3 gridStartPosition = gridManager.transform.position;
        int startX = Mathf.FloorToInt((position.x - gridStartPosition.x) / gridManager.cellSize);
        int startY = Mathf.FloorToInt((position.z - gridStartPosition.z) / gridManager.cellSize);

        for (int x = 0; x < area.x; x++)
        {
            for (int y = 0; y < area.y; y++)
            {
                int gridX = startX + x;
                int gridY = startY + y;

                if (gridX >= gridManager.gridWidth || gridY >= gridManager.gridHeight || gridX < 0 || gridY < 0)
                {
                    return false;
                }

                Cell cell = gridManager.grid[gridX, gridY];
                if (!isDecoration)
                {
                    if (cell.isOccupied || cell.zone == CellTag.Zone.Forbidden)
                    {
                        return false;
                    }
                }
                else
                {
                    if (cell.hasDecoration || cell.zone == CellTag.Zone.Forbidden)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }


    void PlaceObject(Vector3 position, Vector2 area, DecorationAsset decorationObj, Quaternion rotation, Transform parent)
    {
        // Calculate the center offset based on the area and cell size
        Vector3 gridStartPosition = gridManager.transform.position;
        float offsetX = (area.x * gridManager.cellSize) / 2f - gridManager.cellSize / 2f;
        float offsetZ = (area.y * gridManager.cellSize) / 2f - gridManager.cellSize / 2f;
        Vector3 offset = new Vector3(offsetX, 0, offsetZ);
        Vector3 centeredPosition = position + offset;


        // Calculate ray lengths based on the area
        float rayLengthX = (area.x * gridManager.cellSize) / 2f; // X dimension (width)
        float rayLengthZ = (area.y * gridManager.cellSize) / 2f; // Z dimension (length)

        // Perform raycast collision detection
        if (RaycastCollisionCheck(centeredPosition, rayLengthX, rayLengthZ))
        {
            Debug.Log("Collision detected, cannot place the object: " + decorationObj.prefab.name);
            return; // Don't instantiate if there's a collision
        }

        // Instantiate the object
        GameObject decoration = Instantiate(decorationObj.prefab, centeredPosition, rotation, parent);

        // if objects is a dirt item add to the list
        if (decoration.GetComponent<IDirtyObject>() != null)
        {
            IDirtyObjects.Add(decoration.GetComponent<IDirtyObject>());
            obb.Add(decoration);
        }
        MarkOccupied(position, area, decorationObj.isDecoration);

        // Spawn trash objects if applicable
        DecorationAsset asset = decorationAssets.Find(d => d.prefab == decorationObj.prefab);
        if (asset != null && asset.canSpawnTrash)
        {
            SpawnTrashObjects(position, area, asset);
        }
    }

    bool RaycastCollisionCheck(Vector3 centeredPosition, float rayLengthX, float rayLengthZ)
    {
        // Define layer mask for wall detection
        int wallLayerMask = LayerMask.GetMask("Wall");

        // Adjust the raycast origin to be 1 unit higher than the centered position
        Vector3 raycastOrigin = centeredPosition + Vector3.up * 0.5f;

        // Ray directions: forward (Z+), backward (Z-), right (X+), left (X-)
        Vector3[] rayDirections = {
        Vector3.forward,    // Forward (positive Z)
        Vector3.back,       // Backward (negative Z)
        Vector3.right,      // Right (positive X)
        Vector3.left        // Left (negative X)
    };

        // Halve the ray lengths corresponding to each direction
        float[] rayLengths = { rayLengthZ / 2f, rayLengthZ / 2f, rayLengthX / 2f, rayLengthX / 2f };

        // Loop through each direction and cast the ray
        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 rayDirection = rayDirections[i];
            float rayLength = rayLengths[i];

            // Perform the raycast from the raised position
            if (Physics.Raycast(raycastOrigin, rayDirection, rayLength, wallLayerMask))
            {
                Debug.DrawRay(raycastOrigin, rayDirection * rayLength, Color.red, 1.0f); // Visualize ray if collision
                Debug.Log($"Ray hit detected in direction {rayDirection}");
                return true; // Collision detected
            }

            // Visualize ray in case no collision is found (for debugging)
            Debug.DrawRay(raycastOrigin, rayDirection * rayLength, Color.green, 1.0f);
        }

        return false; // No collision
    }

    //bool CollisionCheck(GameObject decoration)
    //{
    //    // Initialize a list to hold all colliders
    //    List<Collider> decorationColliders = new List<Collider>();

    //    // Check if the decoration has any children
    //    if (decoration.transform.childCount > 0)
    //    {
    //        // Get all colliders attached to the decoration object, including its own
    //        decorationColliders.AddRange(decoration.GetComponents<Collider>()); // Get colliders on the decoration itself
    //        decorationColliders.AddRange(decoration.GetComponentsInChildren<Collider>()); // Get colliders in children
    //    }
    //    else
    //    {
    //        // If no children, just get colliders from the decoration itself
    //        decorationColliders.AddRange(decoration.GetComponents<Collider>());
    //    }

    //    bool collisionDetected = false;

    //    // Check for collision for each collider in the object
    //    foreach (var decorationCollider in decorationColliders)
    //    {
    //        // Calculate the center of the collider
    //        Vector3 boxCenter = decorationCollider.bounds.center;

    //        // Use the collider's bounds directly for precise sizing
    //        Vector3 boxSize = decorationCollider.bounds.size; // Full size for OverlapBox


    //        // Check for collision with Mesh Colliders more accurately
    //        if (decorationCollider is MeshCollider)
    //        {
    //            // For Mesh Colliders, we will use the MeshCollider's sharedMesh and bounds
    //            Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize, decorationCollider.transform.rotation);

    //            foreach (var hitCollider in hitColliders)
    //            {
    //                // Check if the hit collider has the "Wall" tag and is not the decoration itself
    //                if (hitCollider.CompareTag("Wall") && hitCollider.gameObject != decoration)
    //                {
    //                    // Calculate if the collision is significant
    //                    if (IsCollisionSignificant(decorationCollider, hitCollider))
    //                    {
    //                        VisualizeCollider(decorationCollider, Color.red);
    //                        Debug.Log($"Collision detected with {hitCollider.gameObject.name}");
    //                        collisionDetected = true; // Collision detected
    //                        break;
    //                    }
    //                }
    //                // Visualize the wall collider
    //            }
    //        }
    //        else
    //        {
    //            // Use OverlapBox for other colliders (Box, Sphere, Capsule, etc.)
    //            Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize / 2f, decorationCollider.transform.rotation);

    //            foreach (var hitCollider in hitColliders)
    //            {
    //                // Check if the hit collider has the "Wall" tag and is not the decoration itself
    //                if (hitCollider.CompareTag("Wall") && hitCollider.gameObject != decoration)
    //                {
    //                    // Calculate if the collision is significant
    //                    if (IsCollisionSignificant(decorationCollider, hitCollider))
    //                    {
    //                        VisualizeCollider(decorationCollider, Color.red);
    //                        Debug.Log($"Collision detected with {hitCollider.gameObject.name}");
    //                        collisionDetected = true; // Collision detected
    //                        break;
    //                    }
    //                }
    //            }
    //        }

    //        if (collisionDetected)
    //            break;
    //    }

    //    return collisionDetected;
    //}

    //private void VisualizeCollider(Collider collider, Color color)
    //{
    //    // Get the bounds of the collider
    //    Bounds bounds = collider.bounds;

    //    // Draw the bounds of the collider
    //    Debug.DrawLine(bounds.min, new Vector3(bounds.min.x, bounds.max.y, bounds.min.z), color, 1.0f); // Left
    //    Debug.DrawLine(bounds.min, new Vector3(bounds.max.x, bounds.min.y, bounds.min.z), color, 1.0f); // Front
    //    Debug.DrawLine(bounds.min, new Vector3(bounds.min.x, bounds.min.y, bounds.max.z), color, 1.0f); // Bottom

    //    Debug.DrawLine(bounds.max, new Vector3(bounds.min.x, bounds.max.y, bounds.max.z), color, 1.0f); // Left
    //    Debug.DrawLine(bounds.max, new Vector3(bounds.max.x, bounds.min.y, bounds.max.z), color, 1.0f); // Front
    //    Debug.DrawLine(bounds.max, new Vector3(bounds.max.x, bounds.max.y, bounds.min.z), color, 1.0f); // Top

    //    Debug.DrawLine(bounds.min, bounds.max, color, 1.0f); // Max connection
    //}

    //// Method to determine if the collision is significant enough
    //private bool IsCollisionSignificant(Collider decorationCollider, Collider wallCollider)
    //{
    //    // Calculate intersection volume
    //    Bounds decorationBounds = decorationCollider.bounds;
    //    Bounds wallBounds = wallCollider.bounds;

    //    // Calculate the intersection bounds
    //    if (decorationBounds.Intersects(wallBounds))
    //    {
    //        Bounds intersection = new Bounds();
    //        intersection.SetMinMax(
    //            Vector3.Max(decorationBounds.min, wallBounds.min),
    //            Vector3.Min(decorationBounds.max, wallBounds.max)
    //        );

    //        // Calculate intersection volume
    //        Vector3 intersectionSize = intersection.size;

    //        // Define a threshold for what constitutes a significant collision
    //        float threshold = 0.1f; // 10% of decoration volume for example

    //        // Calculate the volume of the decoration
    //        float decorationVolume = decorationBounds.size.x * decorationBounds.size.y * decorationBounds.size.z;
    //        float intersectionVolume = intersectionSize.x * intersectionSize.y * intersectionSize.z;

    //        // Check if the intersection volume is a significant percentage of the decoration's volume
    //        return (intersectionVolume / decorationVolume) > threshold;
    //    }

    //    return false; // No intersection
    //}

    Vector2 AdjustAreaForRotation(Vector2 area, CellSideTag.Side side)
    {
        if (side == CellSideTag.Side.North || side == CellSideTag.Side.South)
        {
            return new Vector2(area.y, area.x); // Swap dimensions for North and South
        }
        return area; // Keep dimensions the same for East and West
    }

    void MarkOccupied(Vector3 position, Vector2 area, bool isDecoration, bool includeBuffer = false)
    {
        int startX = Mathf.FloorToInt((position.x - gridManager.transform.position.x) / gridManager.cellSize);
        int startY = Mathf.FloorToInt((position.z - gridManager.transform.position.z) / gridManager.cellSize);
        if (isDecoration)
        {
            gridManager.grid[startX, startY].hasDecoration = true;
            return;
        }
        for (int x = 0; x < area.x; x++)
        {
            for (int y = 0; y < area.y; y++)
            {
                int gridX = startX + x;
                int gridY = startY + y;

                if (gridX >= 0 && gridX < gridManager.gridWidth && gridY >= 0 && gridY < gridManager.gridHeight)
                {
                    if (!isDecoration)
                    {
                        gridManager.grid[gridX, gridY].isOccupied = true;
                        if (includeBuffer)
                        {
                            MarkBufferCells(gridX, gridY);
                        }
                    }
                }
            }
        }
    }

    void MarkBufferCells(int gridX, int gridY)
    {
        // Check the surrounding cells and mark them as forbidden
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int nx = gridX + dx;
                int ny = gridY + dy;

                if (nx >= 0 && nx < gridManager.gridWidth && ny >= 0 && ny < gridManager.gridHeight)
                {
                    if (!gridManager.grid[nx, ny].isOccupied)
                    {
                        gridManager.grid[nx, ny].zone = CellTag.Zone.Forbidden;
                    }
                }
            }
        }
    }

    float ZoneChances(CellTag.Zone zone)
    {
        switch (zone)
        {
            case CellTag.Zone.Inner:
                return chanceInnerSpawn;
            case CellTag.Zone.Outer:
                return chanceOuterSpawn;
            default:
                return 0f;
        }
    }

    float GetRotation(CellSideTag.Side side)
    {
        switch (side)
        {
            case CellSideTag.Side.North:
                return 0;
            case CellSideTag.Side.East:
                return 90;
            case CellSideTag.Side.South:
                return 180;
            case CellSideTag.Side.West:
                return 270;
            default:
                return 0;
        }
    }
}
