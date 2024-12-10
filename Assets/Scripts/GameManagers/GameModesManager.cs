
using UnityEngine;

public class GameModesManager : MonoBehaviour
{
    public static GameModesManager Instance { get; private set; }

    [SerializeField] public bool IsNewInfiniteGame;
    [SerializeField] public bool IsDownloadedInfiniteGame;
    [SerializeField] public bool IsNewHardcoreGame;
    [SerializeField] public bool IsDownloadHardcoreGame;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadModes();
    }

    public void SaveGameModes()
    {
        GameModesData modesData = new GameModesData
        {
            IsNewInfiniteGame = IsNewInfiniteGame,
            IsDownloadedInfiniteGame = IsDownloadedInfiniteGame,
            IsNewHardcoreGame = IsNewHardcoreGame,
            IsDownloadedHardcoreGame = IsDownloadedInfiniteGame
        };

        Debug.Log($"{IsNewInfiniteGame}, {IsDownloadedInfiniteGame}");

        SaveManager.Instance.SaveGameModes(modesData);
    }

    private void LoadModes()
    {
        GameModesData modesData = SaveManager.Instance.LoadGameModes();

        if (modesData != null)
        {
            IsNewInfiniteGame = modesData.IsNewInfiniteGame;
            IsDownloadedInfiniteGame = modesData.IsDownloadedInfiniteGame;
            IsNewHardcoreGame = modesData.IsNewHardcoreGame;
            IsDownloadHardcoreGame = modesData.IsDownloadedHardcoreGame;
        

            Debug.Log($"{IsNewInfiniteGame}, {IsDownloadedInfiniteGame}");
        }

        else
        {
            Debug.LogWarning("Failed to load modes info, applying default values.");
        }
    }

    private void OnApplicationQuit()
    {
        SaveGameModes();
    }
}
