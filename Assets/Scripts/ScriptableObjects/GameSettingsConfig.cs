using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Create Game Settings")]
public class GameSettingsConfig : ScriptableObject
{
    [Header("Audio Settings")]
    public bool IsSoundEnabled = true;
    public bool IsMusicEnabled = true;

    [Header("Vibration Settings")]
    public bool IsVibrationEnabled = true;

    [Header("Input Settings")]
    public bool OnDoubleClick = true;

    [Header("Camera Settings")]
    public float CameraZoomMin = 0.05f;
    public float CameraZoomMax = 2f;
    public float CameraZoom = 0.1f;
        
    [Header("Hold Duration")]
    public float HoldTimeMin = 0.15f;
    public float HoldTimeMax = 0.75f;
    public float HoldTime = 0.3f;
}
