using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;
    public float cellSize;
    public Color innerColor = Color.green;
    public Color outerColor = Color.blue;
    public Color forbiddenColor = Color.red;

    public string tagToCheck = "DontSpawnHere";
    public string wallTag = "Wall";
    public string floorTag = "Ground";
    public float forbiddenRadius = 4.0f;

    public Cell[,] grid;
    public GameObject cellPrefab;

    void Awake()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        grid = new Cell[gridWidth, gridHeight];

        Vector3 startPosition = transform.position; // Get the starting position from the GridManager's position

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = startPosition + new Vector3(x * cellSize, 0, y * cellSize);

                // Check for floor underneath
                bool hasFloor = Physics.Raycast(position + Vector3.up * 10, Vector3.down, out RaycastHit hit, Mathf.Infinity) && hit.collider.CompareTag(floorTag);

                if (!hasFloor)
                {
                    // Mark cell as forbidden if there is no floor
                    Debug.Log(x + " " + y + " Doesn't have floor underneath");
                    grid[x, y] = CreateCell(position, CellTag.Zone.Forbidden, CellSideTag.Side.None);
                    continue; // Skip further checks for this cell
                }

                // Determine cell side and zone
                CellSideTag.Side side = DetermineSide(x, y, position);
                CellTag.Zone zone = DetermineZone(x, y, side, false); // false because we already checked for forbidden cells

                GameObject cellObject = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                Cell cell = cellObject.GetComponent<Cell>();
                cell.position = position;
                cell.zone = zone;
                cell.side = side;

                grid[x, y] = cell;
            }
        }

        // Find all objects with the specified name or tag
        GameObject[] objectsToCheck = GameObject.FindGameObjectsWithTag(tagToCheck);
        foreach (GameObject obj in objectsToCheck)
        {
            Vector3 objPosition = obj.transform.position;
            MarkForbiddenCells(objPosition);
        }
    }

    Cell CreateCell(Vector3 position, CellTag.Zone zone, CellSideTag.Side side)
    {
        GameObject cellObject = Instantiate(cellPrefab, position, Quaternion.identity, transform);
        Cell cell = cellObject.GetComponent<Cell>();
        cell.position = position;
        cell.zone = zone;
        cell.side = side;
        return cell;
    }


    void MarkForbiddenCells(Vector3 objectPosition)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 cellPosition = grid[x, y].position;
                float distance = Vector3.Distance(cellPosition, objectPosition);

                if (distance <= forbiddenRadius)
                {
                    grid[x, y].MakeForbidden();
                    Debug.Log("Marked cell as forbidden at: " + cellPosition);
                }
            }
        }
    }

    CellTag.Zone DetermineZone(int x, int y, CellSideTag.Side side, bool isForbidden)
    {
        if (isForbidden)
        {
            return CellTag.Zone.Forbidden;
        }

        if (side != CellSideTag.Side.None)
        {
            return CellTag.Zone.Outer;
        }

        if (x == 0 || x == gridWidth - 1 || y == 0 || y == gridHeight - 1)
        {
            return CellTag.Zone.Outer;
        }

        return CellTag.Zone.Inner;
    }

    CellSideTag.Side DetermineSide(int x, int y, Vector3 position)
    {
        float raycastHeight = 1.5f; // Height at which to cast the ray
        Vector3 raycastOrigin = position + Vector3.up * raycastHeight;
        float rayLength = cellSize;

        // Check left
        if (Physics.Raycast(raycastOrigin, Vector3.left, out RaycastHit hitLeft, rayLength))
        {
            Debug.Log($"Raycast hit on the left at ({x}, {y}): {hitLeft.collider.gameObject.name}");
            if (hitLeft.collider.CompareTag(wallTag))
            {
                return CellSideTag.Side.West;
            }
        }

        // Check right
        if (Physics.Raycast(raycastOrigin, Vector3.right, out RaycastHit hitRight, rayLength))
        {
            Debug.Log($"Raycast hit on the right at ({x}, {y}): {hitRight.collider.gameObject.name}");
            if (hitRight.collider.CompareTag(wallTag))
            {
                return CellSideTag.Side.East;
            }
        }

        // Check back
        if (Physics.Raycast(raycastOrigin, Vector3.back, out RaycastHit hitBack, rayLength))
        {
            Debug.Log($"Raycast hit on the back at ({x}, {y}): {hitBack.collider.gameObject.name}");
            if (hitBack.collider.CompareTag(wallTag))
            {
                return CellSideTag.Side.South;
            }
        }

        // Check forward
        if (Physics.Raycast(raycastOrigin, Vector3.forward, out RaycastHit hitForward, rayLength))
        {
            Debug.Log($"Raycast hit on the forward at ({x}, {y}): {hitForward.collider.gameObject.name}");
            if (hitForward.collider.CompareTag(wallTag))
            {
                return CellSideTag.Side.North;
            }
        }

        return CellSideTag.Side.None;
    }


    void OnDrawGizmos()
    {
        if (grid == null)
            return;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Cell cell = grid[x, y];
                Gizmos.color = GetCellColor(cell);
                Gizmos.DrawCube(cell.position, new Vector3(cellSize, 0.05f, cellSize));
            }
        }
    }


    Color GetCellColor(Cell cell)
    {
        if (cell.isOccupied)
        {
            return Color.gray;
        }

        switch (cell.zone)
        {
            case CellTag.Zone.Inner:
                return innerColor;
            case CellTag.Zone.Outer:
                return outerColor;
            case CellTag.Zone.Forbidden:
                return forbiddenColor;
            default:
                break;
        }

        switch (cell.side)
        {
            case CellSideTag.Side.West:
                return Color.cyan;
            case CellSideTag.Side.North:
                return Color.black;
            case CellSideTag.Side.South:
                return Color.yellow;
            case CellSideTag.Side.East:
                return Color.magenta;
            default:
                return Color.white;
        }
    }
}
