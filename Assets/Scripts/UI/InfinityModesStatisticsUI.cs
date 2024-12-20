using System.Collections;
using TMPro;
using UnityEngine;

public class InfinityModesStatisticsUI : MonoBehaviour
{
    [Header("Simple Infinite")]
    [SerializeField] private TMP_Text _openedCellsSimpleInfiniteText;
    [SerializeField] private TMP_Text _openedSectorsSimpleInfiniteText;
    [SerializeField] private TMP_Text _minesSimpleInfiniteText;


    [Header("Hardcore")]
    [SerializeField] private TMP_Text _openedCellsHardcoreText;
    [SerializeField] private TMP_Text _openedSectorsHardcoreText;
    [SerializeField] private TMP_Text _minesHardcoreText;

    [Header("Time")]
    [SerializeField] private TMP_Text _openedCellsTimeTrialText;
    [SerializeField] private TMP_Text _openedSectorsTimeTrialText;
    [SerializeField] private TMP_Text _minesTimeTrialText;

    private void Start()
    {
        StartCoroutine(DelayedInitialization());
        
    }

    private IEnumerator DelayedInitialization()     ///ZENJECT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    {
        yield return null;
        UpdateStatisticsUI();
    }

    private void UpdateStatisticsUI()
    {
        var simpleStats = GameManager.Instance.GetGameModeData(GameMode.SimpleInfinite) as SimpleInfiniteModeData;
        Debug.Log($"UI при загрузке загрузке, simpleStats: {simpleStats}");
        if (simpleStats != null)
        {
            Debug.Log("есть ли");
            GenerateStatsTexts(simpleStats);
        }

        var hardcoreStats = GameManager.Instance.GetGameModeData(GameMode.Hardcore) as HardcoreModeData;
        if (hardcoreStats != null)
        {
            GenerateStatsTexts(hardcoreStats);
        }

        var timeStats = GameManager.Instance.GetGameModeData(GameMode.TimeTrial) as TimeTrialModeData;
        if (hardcoreStats != null)
        {
            GenerateStatsTexts(timeStats);
        }
    }

    private void GenerateStatsTexts(IGameModeData stats)
    {
        if (stats is SimpleInfiniteModeData simpleStats)
        {
            _openedCellsSimpleInfiniteText.text = "Opened cells: " + simpleStats.GetOpenedCells().ToString();
            _openedSectorsSimpleInfiniteText.text = "Open sectors: " + simpleStats.GetCompletedSectors().ToString();
            _minesSimpleInfiniteText.text = "Mines: " + simpleStats.GetExplodedMines().ToString();
        }

        else if (stats is HardcoreModeData hardcoreStats)
        {
            _openedCellsHardcoreText.text = "Opened cells: " + hardcoreStats.GetOpenedCells().ToString();
            _openedSectorsHardcoreText.text = "Open sectors: " + hardcoreStats.GetCompletedSectors().ToString();
            _minesHardcoreText.text = "Mines: " + hardcoreStats.GetExplodedMines().ToString(); 
        }

        else if (stats is TimeTrialModeData timeTrialStats)
        {
            _openedCellsTimeTrialText.text = "Opened cells: " + timeTrialStats.GetOpenedCells().ToString();
            _openedSectorsTimeTrialText.text = "Open sectors: " + timeTrialStats.GetCompletedSectors().ToString();
            _minesTimeTrialText.text = "Mines: " + timeTrialStats.GetExplodedMines().ToString();
        }
    }    
}

