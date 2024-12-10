using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance { get; private set; }

    [Header("Audio Settings")]
    [SerializeField] public bool IsSoundEnabled = true;
    [SerializeField] public bool IsMusicEnabled = true;

    [Header("Vibrarion Settings")]
    [SerializeField] public bool IsVibrationEnabled = true;

    [Header("Camera Settings")]
    [SerializeField] public float _cameraZoomMin = 0.05f;
    [SerializeField] public float _cameraZoomMax = 2f;
    [SerializeField] private float _cameraZoom = 0.1f;      

    [Header("Hold Duration")]
    [SerializeField] public float _holdTimeMin = 0.15f;
    [SerializeField] public float _holdTimeMax = 0.75f;
    [SerializeField] private float _holdTime = 0.3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);        
    }

    void Start()
    {
        LoadSettings();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 300;
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

    public float CameraZoom
    {
        get => _cameraZoom;
        set
        {
            _cameraZoom = Mathf.Clamp(value, _cameraZoomMin, _cameraZoomMax);
            SaveSettings();
        }
    }

    public float HoldTime
    {
        get => _holdTime;
        set
        {
            _holdTime = Mathf.Clamp(value, _holdTimeMin, _holdTimeMax);
            SaveSettings();
        }
    }

    public void SaveSettings()
    {
        SettingsData settings = new SettingsData
        {
            IsSoundEnabled = IsSoundEnabled,
            IsMusicEnabled = IsMusicEnabled,
            IsVibrationEnabled = IsVibrationEnabled,
            CameraZoom = _cameraZoom,
            HoldTime = _holdTime
        };

        SaveManager.Instance.SaveSettings(settings);
    }

    public void LoadSettings()
    {
        SettingsData settings = SaveManager.Instance.LoadSettings();
        if (settings != null)
        {
            IsSoundEnabled = settings.IsSoundEnabled;
            IsMusicEnabled = settings.IsMusicEnabled;
            IsVibrationEnabled = settings.IsVibrationEnabled;
            _cameraZoom = settings.CameraZoom;
            _holdTime = settings.HoldTime;

            //Debug.Log("Settings Loaded Successfully");
        }
        else
        {
            Debug.LogWarning("Failed to load settings, applying default values.");
        }
    }
}
