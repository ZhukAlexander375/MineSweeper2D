using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool IsCameraInteracting { get; private set; }

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;

    private Camera mainCamera;
    private Vector3 touchStartPos;
    private ThemeConfig _currentAppliedTheme;    
    private float _cameraMoveThreshold = 5f;

    private void Start()
    {
        Application.targetFrameRate = 1000;
        mainCamera = Camera.main;
        SignalBus.Subscribe<ThemeChangeSignal>(OnThemeChanged);
        TryApplyTheme(ThemeManager.Instance.CurrentTheme);

        zoomSpeed = GameSettingsManager.Instance.CameraZoom;        
    }

    private void Update()
    {
#if !UNITY_EDITOR
        HandleTouchInput();
#else
        HandleMouseInput();
#endif
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                IsCameraInteracting = false;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                var delta = (Vector3)touch.position - touchStartPos;

                delta *= mainCamera.orthographicSize * 0.01f;       //��� �������� �������� ��� ����

                if (delta.magnitude > _cameraMoveThreshold)
                {
                    touchStartPos = touch.position;
                    MoveCamera(delta);
                    IsCameraInteracting = true;
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                IsCameraInteracting = false;
            }
        }

        if (Input.touchCount == 2)
        {
            var touch0 = Input.GetTouch(0);
            var touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                var currentDistance = Vector2.Distance(touch0.position, touch1.position);
                var previousDistance = Vector2.Distance(touch0.position - touch0.deltaPosition, touch1.position - touch1.deltaPosition);
                var deltaDistance = currentDistance - previousDistance;

                ZoomCamera(deltaDistance * GameSettingsManager.Instance.CameraZoom);
                IsCameraInteracting = true;
            }
            else if (touch0.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Ended)
            {
                IsCameraInteracting = false;
            }
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            IsCameraInteracting = false;
        }

        if (Input.GetMouseButton(0))
        {
            var delta = (Vector3)Input.mousePosition - touchStartPos;
            delta *= mainCamera.orthographicSize * 0.01f;

            if (delta.magnitude > _cameraMoveThreshold)
            {
                touchStartPos = Input.mousePosition;
                MoveCamera(delta);
                IsCameraInteracting = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            IsCameraInteracting = false;
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            ZoomCamera(Input.mouseScrollDelta.y * GameSettingsManager.Instance.CameraZoom);
            IsCameraInteracting = true;
        }
    }

    private void MoveCamera(Vector3 delta)
    {
        var move = new Vector3(-delta.x * moveSpeed, -delta.y * moveSpeed, 0);
        transform.position += move;
    }

    private void ZoomCamera(float deltaDistance)
    {
        var newSize = mainCamera.orthographicSize - deltaDistance;
        mainCamera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
    }

    private void OnThemeChanged(ThemeChangeSignal signal)
    {
        TryApplyTheme(signal.Theme);
    }

    private void TryApplyTheme(ThemeConfig theme)
    {
        if (theme == null)
        {
            return;
        }

        if (_currentAppliedTheme == theme)
        {
            return;
        }

        ApplyTheme(theme);
    }

    private void ApplyTheme(ThemeConfig theme)
    {
        if (theme == null)
        {
            return;
        }

        mainCamera.backgroundColor = theme.CameraBackground;
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<ThemeChangeSignal>(OnThemeChanged);
    }
}
