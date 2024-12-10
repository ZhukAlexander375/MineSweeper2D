using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SectorUI : MonoBehaviour
{
    [SerializeField] private Canvas _sectorCanvas;
    [SerializeField] private Image _background;
    [SerializeField] private TMP_Text _text;        /////////??????????
    [SerializeField] private Button _loupeButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _openSectorForAwardButton;
    [SerializeField] private TMP_Text _prizeCountText;

    [Header("HOW MUCH PRIZE??????")]
    [SerializeField] private int _prizeCount;

    private Sector _sector;

    private void Start()
    {
        _loupeButton.onClick.AddListener(ShowSector);
        _closeButton.onClick.AddListener(HideSector);
        _openSectorForAwardButton.onClick.AddListener(OpenSectorForAward);
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
        currentColor.a = 0.66f;
        _background.color = currentColor;

        _loupeButton.gameObject.SetActive(false);
        _closeButton.gameObject.SetActive(true);
    }

    public void HideSector()
    {
        Color currentColor = _background.color;
        currentColor.a = 0.99f;
        _background.color = currentColor;

        _closeButton.gameObject.SetActive(false);
        _loupeButton.gameObject.SetActive(true);        
    }

    private void OpenSectorForAward()
    {
        _sector.OpenSector(_prizeCount);
    }

    private void UpdatePrizeCountText()
    {
        _prizeCountText.text = _prizeCount.ToString();
    }
}
