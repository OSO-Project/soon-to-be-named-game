using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance;

    [SerializeField] private GameData gameData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        gameData = new GameData();
    }

    #region Save/Load Actions
    public event Action<GameData> OnLoadData;
    public event Action<GameData> OnSaveData;
    #endregion

    #region Encounter Actions
    public event Action OnEncounterStart;
    public event Action OnEncounterEnd;

    public event Action<float> OnEarthquakeEncounterStart;
    public event Action OnEarthquakeEncounterEnd;
    #endregion

    //Group of methods for managing saving and loading
    #region Saving and loading
    public void LoadData()
    {
        if (File.Exists(Application.dataPath + "/../save.xml"))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            FileStream stream = new FileStream(Application.dataPath + "/../save.xml", FileMode.Open);
            GameData tmp = serializer.Deserialize(stream) as GameData;
            /*if (tmp != null)
            {
                _gameData = tmp;
                _loadingData.sceneToLoad = _gameData.SceneToLoad;
                _loadingData.stateToLoad = _gameData.StateToLoad;
            }*/
            stream.Close();
            //Debug.Log(_gameData.PlayerPosition);
            OnLoadData?.Invoke(tmp);
            //_cEventChannel.CheckPointRestore(true);
        }
    }
    public void SaveData()
    {
        OnSaveData?.Invoke(gameData);
        XmlSerializer serializer = new XmlSerializer(typeof(GameData));
        FileStream stream = new FileStream(Application.dataPath + "/../save.xml", FileMode.Create);

        /*_gameData.SceneToLoad = _loadingData.sceneToLoad;
        _gameData.StateToLoad = _loadingData.stateToLoad;

        if (_gameData.GameDifficulty == GameDifficulty.Easy)
        {
            _gameData.PlayerHealth = 2;
        }
        else if (_gameData.GameDifficulty == GameDifficulty.Normal)
        {
            _gameData.PlayerHealth = 1;
        }*/

        serializer.Serialize(stream, gameData);
        stream.Close();
    }
    #endregion

    #region Progression
    public void HandleUnlockItem(Unlockables item)
    {
        gameData.UnlockItem(item);
    }
    #endregion

    //Group of methods for managing encounters
    #region Encounters
    // General methods
    public void HandleEncounterStart()
    {
        OnEncounterStart?.Invoke();
    }
    public void HandleEncounterEnd()
    {
        OnEncounterEnd?.Invoke();
    }

    // Earthquake
    public void HandleEarthquakeEncounterStart(float duration)
    {
        OnEarthquakeEncounterStart?.Invoke(duration);
    }
    public void HandleEarthquakeEncounterEnd()
    {
        OnEarthquakeEncounterEnd?.Invoke();
    }
    #endregion
}
