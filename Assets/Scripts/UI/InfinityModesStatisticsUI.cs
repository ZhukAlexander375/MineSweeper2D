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


    private void OnEnable()
    {
        SignalBus.Subscribe<LoadCompletedSignal>(UpdateStatisticsUI);
        UpdateStatisticsUI(new LoadCompletedSignal());
    }

    public void UpdateStatisticsUI(LoadCompletedSignal signal)
    {
        _openedCellsSimpleInfiniteText.text = "Opened cells: " + SimpleInfiniteStatisticController.Instance.OpenedCells.ToString();
        _openedSectorsSimpleInfiniteText.text = "Open sectors: " + SimpleInfiniteStatisticController.Instance.CompletedSectors.ToString();
        _minesSimpleInfiniteText.text = "Mines: " + SimpleInfiniteStatisticController.Instance.ExplodedMines.ToString();

        _openedCellsHardcoreText.text = "Opened cells: " + HardcoreStatisticController.Instance.OpenedCells.ToString();
        _openedSectorsHardcoreText.text = "Open sectors: " + HardcoreStatisticController.Instance.CompletedSectors.ToString();
        _minesHardcoreText.text = "Mines: " + HardcoreStatisticController.Instance.ExplodedMines.ToString();

        _openedCellsTimeTrialText.text = "Opened cells: " + TimeTrialStatisticController.Instance.OpenedCells.ToString();
        _openedSectorsTimeTrialText.text = "Open sectors: " + TimeTrialStatisticController.Instance.CompletedSectors.ToString();
        _minesTimeTrialText.text = "Mines: " + TimeTrialStatisticController.Instance.ExplodedMines.ToString();
    }

    private void OnDisable()
    {
       SignalBus.Unsubscribe<LoadCompletedSignal>(UpdateStatisticsUI);
    }
}

