using TMPro;
using UnityEngine;
using Zenject;

public class InfinityModesStatisticsUI : MonoBehaviour
{
    [Header("Simple Infinite")]
    //[SerializeField] private TMP_Text _openedCellsSimpleInfiniteText;
    [SerializeField] private TMP_Text _openedSectorsSimpleInfiniteText;
    [SerializeField] private TMP_Text _minesSimpleInfiniteText;
    [SerializeField] private TMP_Text _playTimeSimpleInfiniteText;

    [Header("Hardcore")]
    //[SerializeField] private TMP_Text _openedCellsHardcoreText;
    [SerializeField] private TMP_Text _openedSectorsHardcoreText;
    [SerializeField] private TMP_Text _minesHardcoreText; 
    [SerializeField] private TMP_Text _playTimeHardcoreText;

    [Header("Time")]
    //[SerializeField] private TMP_Text _openedCellsTimeTrialText;
    [SerializeField] private TMP_Text _openedSectorsTimeTrialText;
    [SerializeField] private TMP_Text _minesTimeTrialText;
    [SerializeField] private TMP_Text _playTimeTimeTrialText;
    
    private GameManager _gameManager;

    [Inject]
    private void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }


    private void OnEnable()
    {
        SignalBus.Subscribe<LoadCompletedSignal>(UpdateStatisticsUI);
        UpdateStatisticsUI(new LoadCompletedSignal());
    }

    public void UpdateStatisticsUI(LoadCompletedSignal signal)
    {
        //_openedCellsSimpleInfiniteText.text = "Opened cells: " + SimpleInfiniteStatisticController.Instance.OpenedCells.ToString();
        _openedSectorsSimpleInfiniteText.text = "Open sectors: " + _gameManager.SimpleInfiniteStats.CompletedSectors.ToString();
        _minesSimpleInfiniteText.text = "Mines: " + _gameManager.SimpleInfiniteStats.ExplodedMines.ToString();
        _playTimeSimpleInfiniteText.text = "Time spent: " + FormatTime(_gameManager.SimpleInfiniteStats.TotalPlayTime, "SimpleInfinite");

        //_openedCellsHardcoreText.text = "Opened cells: " + HardcoreStatisticController.Instance.OpenedCells.ToString();
        _openedSectorsHardcoreText.text = "Open sectors: " + _gameManager.HardcoreStats.CompletedSectors.ToString();
        _minesHardcoreText.text = "Mines: " + _gameManager.HardcoreStats.ExplodedMines.ToString();
        _playTimeHardcoreText.text = "Time spent: " + FormatTime(_gameManager.HardcoreStats.TotalPlayTime, "Hardcore");

        //_openedCellsTimeTrialText.text = "Opened cells: " + TimeTrialStatisticController.Instance.OpenedCells.ToString();
        _openedSectorsTimeTrialText.text = "Open sectors: " + _gameManager.TimeTrialStats.CompletedSectors.ToString();
        _minesTimeTrialText.text = "Mines: " + _gameManager.TimeTrialStats.ExplodedMines.ToString();
        _playTimeTimeTrialText.text = "Time spent: " + FormatTime(_gameManager.TimeTrialStats.TotalPlayTime, "TimeTrial");
    }

    private string FormatTime(float totalSeconds, string modeName)
    {
        int hours = Mathf.FloorToInt(totalSeconds / 3600);
        int minutes = Mathf.FloorToInt((totalSeconds % 3600) / 60);
        int seconds = Mathf.FloorToInt(totalSeconds % 60);

        if (hours > 0)
        {
            return $"{hours} h. {minutes} min. {seconds} sec";
        }
        else if (minutes > 0)
        {
            return $"{minutes} min. {seconds} sec";
        }
        else
        {
            return $"{seconds} sec";
        }
    }

    private void OnDisable()
    {
       SignalBus.Unsubscribe<LoadCompletedSignal>(UpdateStatisticsUI);
    }
}

