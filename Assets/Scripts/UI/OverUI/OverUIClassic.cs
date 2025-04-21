using UnityEngine;
using Zenject;

public class OverUIClassic : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private Canvas _settingsScreen;

    private UIServiceClassic _uiServiceClassic;

    [Inject]
    public void Construct(UIServiceClassic uiService)
    {
        _uiServiceClassic = uiService;
    }

    private void Start()
    {
        //Debug.Log("Over UI inastall");
    }

    public void OpenSettingsScreen()
    {
        _settingsScreen.gameObject.SetActive(true);
    }
}
