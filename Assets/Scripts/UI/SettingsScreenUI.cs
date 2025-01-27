using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreenUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _soundButton;
    [SerializeField] private Image _soundOnImage;
    [SerializeField] private Image _soundOffImage;
    [SerializeField] private Button _musicButton;
    [SerializeField] private Image _musicOnImage;
    [SerializeField] private Image _musicOffImage;
    [SerializeField] private Button _vibrationButton;
    [SerializeField] private Image _vibrationOnImage;
    [SerializeField] private Image _vibrationOffImage;    
    [SerializeField] private Button _supportEmailButton;
    [SerializeField] private Button _discordButton;
    [SerializeField] private Button _privacyPolicyButton;


    [Header("Sliders")]
    [SerializeField] private Slider _holdDurationSlider;
    [SerializeField] private Slider _cameraZoomSlider;

    [Header("Texts")]
    [SerializeField] private TMP_Text _holdDurationText;
    [SerializeField] private TMP_Text _cameraZoomText;

    [Header("Supports ")]

    private GameSettingsManager _settingsManager;


    private void Awake()
    {
        _settingsManager = GameSettingsManager.Instance;

    }
    private void Start()        
    {        
        UpdateSettingsScreen();

        _soundButton.onClick.AddListener(() =>
        {
            _settingsManager.ToggleSound();
            UpdateSoundButtonSprite();
        });

        _musicButton.onClick.AddListener(() =>
        {
            _settingsManager.ToggleMusic();
            UpdateMusicButtonSprite();
        });

        _vibrationButton.onClick.AddListener(() =>
        {
            _settingsManager.ToggleVibration();
            UpdateVibrationButtonSprite();
        });

        _cameraZoomSlider.onValueChanged.AddListener(OnZoomValueChanged);
        _holdDurationSlider.onValueChanged.AddListener(OnHoldDurationChanged);
                
        _supportEmailButton.onClick.AddListener(OpenSupportEmailURL);
        _discordButton.onClick.AddListener(OpenDiscordURL);
        _privacyPolicyButton.onClick.AddListener(OpenPrivacyPolicyURL);
    }

    public void UpdateSettingsScreen()
    {
        UpdateSoundButtonSprite();
        UpdateMusicButtonSprite();
        UpdateVibrationButtonSprite();

        SetCameraZoomSliderValue();
        SetTimeDurationSliderValue();

        UpdateCameraZoomText(_settingsManager.CameraZoom);
        UpdateHoldDurationText(_settingsManager.HoldTime);
    }

    private void SetCameraZoomSliderValue()
    {
        _cameraZoomSlider.minValue = _settingsManager._cameraZoomMin;
        _cameraZoomSlider.maxValue = _settingsManager._cameraZoomMax;
        _cameraZoomSlider.value = _settingsManager.CameraZoom;        
    }

    private void SetTimeDurationSliderValue()
    {
        _holdDurationSlider.minValue = _settingsManager._holdTimeMin;
        _holdDurationSlider.maxValue = _settingsManager._holdTimeMax;
        _holdDurationSlider.value = _settingsManager.HoldTime;
    }

    private void UpdateSoundButtonSprite()
    {
        _soundOnImage.gameObject.SetActive(_settingsManager.IsSoundEnabled);
        _soundOffImage.gameObject.SetActive(!_settingsManager.IsSoundEnabled);
    }

    private void UpdateMusicButtonSprite()
    {
        _musicOnImage.gameObject.SetActive(_settingsManager.IsMusicEnabled);
        _musicOffImage.gameObject.SetActive(!_settingsManager.IsMusicEnabled);
    }

    private void UpdateVibrationButtonSprite()
    {
        _vibrationOnImage.gameObject.SetActive(_settingsManager.IsVibrationEnabled);
        _vibrationOffImage.gameObject.SetActive(!_settingsManager.IsVibrationEnabled);
    }


    private void OnZoomValueChanged(float value)
    {
        value = Mathf.Round(value * 20f) / 20f;
        GameSettingsManager.Instance.CameraZoom = value;        
        UpdateCameraZoomText(value);
        GameSettingsManager.Instance.SaveSettings();
    }

    private void OnHoldDurationChanged(float value)
    {
        value = Mathf.Round(value * 20f) / 20f;
        GameSettingsManager.Instance.HoldTime = value;
        UpdateHoldDurationText(value);
        GameSettingsManager.Instance.SaveSettings();
    }

    private void UpdateCameraZoomText(float value)
    {
        _cameraZoomText.text = "Zoom Speed: " + value.ToString("F2");
    }

    private void UpdateHoldDurationText(float value)
    {
        _holdDurationText.text = "Hold Duration: " + value.ToString("F2") + "s";
    }

    private void OpenSupportEmailURL()
    {
        Application.OpenURL("https://extgamemo.github.io/");
    }

    private void OpenDiscordURL()
    {
        Application.OpenURL("https://discord.gg/gEf2FqrvAj");
    }

    private void OpenPrivacyPolicyURL()
    {
        Application.OpenURL("https://extgamemo.github.io/PrivacyPolicy/"); 
    }
        

    private void OnDestroy()
    {
        _cameraZoomSlider.onValueChanged.RemoveListener(OnZoomValueChanged);
        _holdDurationSlider.onValueChanged.RemoveListener(OnHoldDurationChanged);
    }
}
