using UnityEngine;
using Zenject;

public class GameSettingsManager : IInitializable
{
    public bool IsSoundEnabled { get; private set; }
    public bool IsMusicEnabled { get; private set; }
    public bool IsVibrationEnabled { get; private set; }
    public bool OnDoubleClick { get; private set; }

    public float CameraZoom { get; private set; }
    public float CameraZoomMin { get; private set; }
    public float CameraZoomMax { get; private set; }


    public float HoldTimeMin { get; private set; }
    public float HoldTimeMax { get; private set; }
    public float HoldTime { get; private set; }

    private GameSettingsConfig _defaultSettingsConfig;
    private SaveManager _saveManager;


    [Inject]
    private void Construct(GameSettingsConfig settingsConfig, SaveManager saveManager)
    {
        _defaultSettingsConfig = settingsConfig;
        _saveManager = saveManager;
    }

    public void Initialize()
    {
        ApplyDefaultSettings();  // Загружаем СО в память
        LoadSettings();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    public void ToggleSound()
    {
        IsSoundEnabled = !IsSoundEnabled;
        // Вызов логики включения/выключения звука
        //AudioManager.Instance.SetSoundState(_settingsManager._isSoundEnabled);

        SaveSettings();
    }

    public void ToggleMusic()
    {
        IsMusicEnabled = !IsMusicEnabled;
        SaveSettings();
    }

    public void ToggleVibration()
    {
        IsVibrationEnabled = !IsVibrationEnabled;
        SaveSettings();
    }

    public void SetCameraZoom(float value)
    {
        CameraZoom = Mathf.Clamp(value, CameraZoomMin, CameraZoomMax);
        SaveSettings();
    }

    public void SetHoldTime(float value)
    {
        HoldTime = Mathf.Clamp(value, HoldTimeMin, HoldTimeMax);
        SaveSettings();
    }

    public void ToogleDoubleClick()
    {
        OnDoubleClick = !OnDoubleClick;
        SaveSettings();
    }


    public void SaveSettings()
    {
        SettingsData settings = new SettingsData
        {
            IsSoundEnabled = IsSoundEnabled,
            IsMusicEnabled = IsMusicEnabled,
            IsVibrationEnabled = IsVibrationEnabled,
            OnDoubleClick = OnDoubleClick,
            CameraZoom = CameraZoom,
            CameraZoomMin = CameraZoomMin,
            CameraZoomMax = CameraZoomMax,
            HoldTime = HoldTime,
            HoldTimeMin = HoldTimeMin,
            HoldTimeMax = HoldTimeMax
        };

        _saveManager.SaveSettings(settings);
    }

    public void LoadSettings()
    {
        SettingsData settings = _saveManager.LoadSettings();

        if (settings == null || settings.CameraZoomMax == 0) // Если файл пустой или некорректный
        {
            SaveSettings(); // Сохраняем дефолтные настройки
            Debug.LogWarning("No valid settings found. Using defaults and saving them.");
            return;
        }

        IsSoundEnabled = settings.IsSoundEnabled;
        IsMusicEnabled = settings.IsMusicEnabled;
        IsVibrationEnabled = settings.IsVibrationEnabled;
        OnDoubleClick = settings.OnDoubleClick;
        CameraZoom = settings.CameraZoom;
        HoldTime = settings.HoldTime;

        CameraZoomMin = settings.CameraZoomMin > 0 ? settings.CameraZoomMin : _defaultSettingsConfig.CameraZoomMin;
        CameraZoomMax = settings.CameraZoomMax > 0 ? settings.CameraZoomMax : _defaultSettingsConfig.CameraZoomMax;

        HoldTimeMin = settings.HoldTimeMin > 0 ? settings.HoldTimeMin : _defaultSettingsConfig.HoldTimeMin;
        HoldTimeMax = settings.HoldTimeMax > 0 ? settings.HoldTimeMax : _defaultSettingsConfig.HoldTimeMax;
    }

    private void ApplyDefaultSettings()
    {
        IsSoundEnabled = _defaultSettingsConfig.IsSoundEnabled;
        IsMusicEnabled = _defaultSettingsConfig.IsMusicEnabled;
        IsVibrationEnabled = _defaultSettingsConfig.IsVibrationEnabled;
        OnDoubleClick = _defaultSettingsConfig.OnDoubleClick;

        CameraZoomMin = _defaultSettingsConfig.CameraZoomMin;
        CameraZoomMax = _defaultSettingsConfig.CameraZoomMax;
        CameraZoom = _defaultSettingsConfig.CameraZoom;

        HoldTimeMin = _defaultSettingsConfig.HoldTimeMin;
        HoldTimeMax = _defaultSettingsConfig.HoldTimeMax;
        HoldTime = _defaultSettingsConfig.HoldTime;
    }
}

