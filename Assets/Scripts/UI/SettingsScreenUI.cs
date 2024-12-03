using System;
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

        _soundButton.onClick.AddListener(ToggleSound);
        _musicButton.onClick.AddListener(ToggleMusic);
        _vibrationButton.onClick.AddListener(ToggleVibration);        

        _cameraZoomSlider.onValueChanged.AddListener(OnZoomValueChanged);
        _holdDurationSlider.onValueChanged.AddListener(OnHoldDurationChanged);

        UpdateSettingsScreen();
    }

    public void UpdateSettingsScreen()
    {
        UpdateSoundButtonSprite();
        UpdateMusicButtonSprite();
        UpdateVibrationButtonSprite();
        SetCameraZoomSliderValue();
        SetTimeDurationSliderValue();
        UpdateCameraZoomText(GameSettingsManager.Instance.CameraZoom);
        UpdateHoldDurationText(GameSettingsManager.Instance.HoldTime);
    }

    private void SetCameraZoomSliderValue()
    {
        _cameraZoomSlider.minValue = GameSettingsManager.Instance._cameraZoomMin;
        _cameraZoomSlider.maxValue = GameSettingsManager.Instance._cameraZoomMax;
        _cameraZoomSlider.value = GameSettingsManager.Instance.CameraZoom;        
    }

    private void SetTimeDurationSliderValue()
    {
        _holdDurationSlider.minValue = GameSettingsManager.Instance._holdTimeMin;
        _holdDurationSlider.maxValue = GameSettingsManager.Instance._holdTimeMax;
        _holdDurationSlider.value = GameSettingsManager.Instance.HoldTime;
    }

    private void ToggleSound()
    {
        _settingsManager.IsSoundEnabled = !_settingsManager.IsSoundEnabled;
        UpdateSoundButtonSprite();

        // Вызов логики включения/выключения звука
        //AudioManager.Instance.SetSoundState(_settingsManager._isSoundEnabled);
    }

    private void ToggleMusic()
    {
        _settingsManager.IsMusicEnabled = !_settingsManager.IsMusicEnabled;
        UpdateMusicButtonSprite();
    }

    private void ToggleVibration()
    {
        _settingsManager.IsVibrationEnabled = !_settingsManager.IsVibrationEnabled;
        UpdateVibrationButtonSprite();
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
