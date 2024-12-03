using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreenUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _soundButton;
    [SerializeField] private Sprite _soundOnSprite;
    [SerializeField] private Sprite _soundOffSprite;
    [SerializeField] private Button _musicButton;
    [SerializeField] private Sprite _musicOnSprite;
    [SerializeField] private Sprite _musicOffSprite;
    [SerializeField] private Button _vibrationButton;
    [SerializeField] private Sprite _vibrationOnSprite;
    [SerializeField] private Sprite _vibrationOffSprite;

    [Header("Sliders")]
    [SerializeField] private Slider _holdDurationSlider;
    [SerializeField] private Slider _cameraZoomSlider;

    [Header("Texts")]
    [SerializeField] private TMP_Text _holdDurationText;
    [SerializeField] private TMP_Text _cameraZoomText;

    private GameSettingsManager _settingsManager;

    private void Start()
    {
        _settingsManager = GameSettingsManager.Instance;
        _settingsManager.LoadSettings();

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
        _soundButton.image.sprite = _settingsManager.IsSoundEnabled ? _soundOnSprite : _soundOffSprite;
    }

    private void UpdateMusicButtonSprite()
    {
        _musicButton.image.sprite = _settingsManager.IsMusicEnabled ? _musicOnSprite : _musicOffSprite;
    }

    private void UpdateVibrationButtonSprite()
    {
        _vibrationButton.image.sprite = _settingsManager.IsVibrationEnabled ? _vibrationOnSprite : _vibrationOffSprite;
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

    private void OnDestroy()
    {
        _cameraZoomSlider.onValueChanged.RemoveListener(OnZoomValueChanged);
        _holdDurationSlider.onValueChanged.RemoveListener(OnHoldDurationChanged);
    }
}
