using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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

    [Header("Toggles")]
    [SerializeField] private Toggle _doubleClickToggle;

    [Header("Texts")]
    [SerializeField] private TMP_Text _holdDurationText;
    [SerializeField] private TMP_Text _cameraZoomText;

    private GameSettingsManager _settingsManager;

    [Inject]
    private void Construct(GameSettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
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

        _doubleClickToggle.isOn = _settingsManager.OnDoubleClick;

        _cameraZoomSlider.onValueChanged.AddListener(OnZoomValueChanged);
        _holdDurationSlider.onValueChanged.AddListener(OnHoldDurationChanged);

        _supportEmailButton.onClick.AddListener(OpenSupportEmailURL);
        _discordButton.onClick.AddListener(OpenDiscordURL);
        _privacyPolicyButton.onClick.AddListener(OpenPrivacyPolicyURL);

        _doubleClickToggle.onValueChanged.AddListener(OnDoubleClickToggled);

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
        _cameraZoomSlider.minValue = _settingsManager.CameraZoomMin;
        _cameraZoomSlider.maxValue = _settingsManager.CameraZoomMax;
        _cameraZoomSlider.value = _settingsManager.CameraZoom;
    }

    private void SetTimeDurationSliderValue()
    {
        _holdDurationSlider.minValue = _settingsManager.HoldTimeMin;
        _holdDurationSlider.maxValue = _settingsManager.HoldTimeMax;
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
        _settingsManager.SetCameraZoom(value);        
        UpdateCameraZoomText(value);
    }

    private void OnHoldDurationChanged(float value)
    {
        value = Mathf.Round(value * 20f) / 20f;
        _settingsManager.SetHoldTime(value);
        UpdateHoldDurationText(value);
    }

    private void OnDoubleClickToggled(bool isOn)
    {
        _settingsManager.ToogleDoubleClick();
    }

    private void UpdateCameraZoomText(float value)
    {
        _cameraZoomText.text = "Zoom speed: " + value.ToString("F2");
    }

    private void UpdateHoldDurationText(float value)
    {
        _holdDurationText.text = "Hold duration: " + value.ToString("F2") + "s";
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
