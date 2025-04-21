using UnityEngine;
using Zenject;

public class PopupUIClassic : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private Canvas _pauseMenuScreen;
    [SerializeField] private Canvas _winLoseScreen;

    private UIServiceClassic _uiServiceClassic;

    [Inject]
    public void Construct(UIServiceClassic uiService)
    {
        _uiServiceClassic = uiService;
    }

    private void Start()
    {
        //Debug.Log("Popup UI inastall");
    }

    public void OpenPauseMenu()
    {
        _pauseMenuScreen.gameObject.SetActive(true);
    }

    public void OpenWinLoseScreen(bool isGameOver)
    {
        _winLoseScreen.gameObject.SetActive(isGameOver);

        if (isGameOver == true)
        {
            _pauseMenuScreen.gameObject.SetActive(false);
        }
    }

    public void CloseWinLoseScreen()
    {
        _winLoseScreen.gameObject.SetActive(false);
    }
}
