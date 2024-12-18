using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SectorUI : MonoBehaviour
{
    [SerializeField] private Canvas _sectorCanvas;
    [SerializeField] private Image _background;
    [SerializeField] private GameObject _containerForHideObjects;
    [SerializeField] private TMP_Text _text;        /////////??????????
    [SerializeField] private Button _viewSectorButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _openSectorForAwardButton;
    [SerializeField] private Button _openSectorForAdButton;
    [SerializeField] private Button _replayLevelButton;
    [SerializeField] private TMP_Text _prizeCountText;

    [Header("HOW MUCH PRIZE??????")]
    [SerializeField] private int _prizeCount;

    private Sector _sector;

    private void Start()
    {
        _viewSectorButton.onClick.AddListener(ShowSector);
        _closeButton.onClick.AddListener(HideSector);
        _openSectorForAwardButton.onClick.AddListener(OpenSectorForAward);
        _openSectorForAdButton.onClick.AddListener(OpenSectorForAd);
        _replayLevelButton.onClick.AddListener(ReplayLevel);
        //gameObject.SetActive(false);
        UpdatePrizeCountText();
    }

    public void SetSector(Sector sector)
    {
        _sector = sector;
    }

    private void ShowSector()
    {
        Color currentColor = _background.color;
        currentColor.a = 0.5f;
        _background.color = currentColor;
        //UpdatePrizeCountText();

        _containerForHideObjects.SetActive(false);
        _closeButton.gameObject.SetActive(true);
    }

    public void HideSector()
    {
        Color currentColor = _background.color;
        currentColor.a = 0.99f;
        _background.color = currentColor;
        UpdatePrizeCountText();

        _closeButton.gameObject.SetActive(false);
        _containerForHideObjects.gameObject.SetActive(true);        
    }

    public void CompletedSector()
    {
        Color currentColor = Color.white;
        currentColor.a = 0.33f;
        _background.color = currentColor;

        _closeButton.gameObject.SetActive(false);
        _containerForHideObjects.gameObject.SetActive(false);
    }

    private void OpenSectorForAward()
    {
        _sector.OpenSector(_sector.CurrentBuyoutCost);
    }

    private void OpenSectorForAd()
    {
        _sector.OpenSector(0);
    }

    private void ReplayLevel() 
    {
        PlayerProgress.Instance.ResetSessionStatistic();
        //GameManager.Instance.IsDownloadedInfiniteGame = false;
        //GameManager.Instance.IsNewInfiniteGame = true;
        GameManager.Instance.SaveGameModes();
        SceneLoader.Instance.LoadInfiniteMinesweeperScene();
    }

    private void UpdatePrizeCountText()
    {
        _prizeCountText.text = _sector.CurrentBuyoutCost.ToString();
    }
}
