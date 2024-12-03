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

        LoadSettings();
    }

    void Start()
    {
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
        PlayerPrefs.SetInt("IsSoundEnabled", IsSoundEnabled ? 1 : 0);
        PlayerPrefs.SetInt("IsMusicEnabled", IsMusicEnabled ? 1 : 0);
        PlayerPrefs.SetInt("IsVibrationEnabled", IsVibrationEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("CameraZoom", _cameraZoom); 
        PlayerPrefs.SetFloat("HoldTime", _holdTime);

        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        IsSoundEnabled = PlayerPrefs.GetInt("IsSoundEnabled", 1) == 1;
        IsMusicEnabled = PlayerPrefs.GetInt("IsMusicEnabled", 1) == 1;
        IsVibrationEnabled = PlayerPrefs.GetInt("IsVibrationEnabled", 1) == 1;
        CameraZoom = PlayerPrefs.GetFloat("CameraZoom", 0.1f);
        HoldTime = PlayerPrefs.GetFloat("HoldTime", 0.3f);        
    }
}
