using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool IsCameraInteracting => IsMoving || IsZooming;
    public bool IsMoving { get; private set; }
    public bool IsZooming { get; private set; }

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float deadZoneThrashhold = 100f;

    private Camera mainCamera;
    private Vector3 touchStartPos;
    private ThemeConfig _currentAppliedTheme;
    private Vector3 previousPosition;
    private Vector3 cameraCurrentPosition;    
    private float zoomStartTime;
    private float zoomEndTime;

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
                IsMoving = false;
                touchStartPos = touch.position;
                cameraCurrentPosition = transform.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                var delta = (Vector3)touch.position - touchStartPos;

                if (!IsMoving && delta.magnitude > deadZoneThrashhold)
                {
                    IsMoving = true;
                }

                if (IsMoving)
                {
                    touchStartPos = touch.position;
                    MoveCamera(delta);
                }
                
            }

            else if (touch.phase == TouchPhase.Ended)
            {
                IsMoving = false;
            }
        }

        if (Input.touchCount == 2)
        {
            IsZooming = true;

            var touch0 = Input.GetTouch(0);
            var touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                var currentDistance = Vector2.Distance(touch0.position, touch1.position);
                var previousDistance = Vector2.Distance(touch0.position - touch0.deltaPosition, touch1.position - touch1.deltaPosition);
                var deltaDistance = currentDistance - previousDistance;

                ZoomCamera(deltaDistance * GameSettingsManager.Instance.CameraZoom);
            }
        }
        else
        {
            IsZooming = false;
        }
    }
    
    public bool IsCameraMoving()
    {
        return transform.position == cameraCurrentPosition;
    }


    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            var delta = Input.mousePosition - previousPosition;

            if (!IsMoving && delta.magnitude > deadZoneThrashhold)
            {
                IsMoving = true;
            }

            if (IsMoving)
            {
                previousPosition = Input.mousePosition;
                MoveCamera(delta);
            }           
        }

        if (Input.GetMouseButtonUp(0))
        {
            IsMoving = false;
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            IsZooming = true;
            zoomStartTime = Time.time;
            ZoomCamera(Input.mouseScrollDelta.y * GameSettingsManager.Instance.CameraZoom);
            zoomEndTime = Time.time;
        }
        else
        {
            IsZooming = zoomEndTime + 0.1f > Time.time;
        }
    }

    private void MoveCamera(Vector3 delta)
    {
        float zoomFactor = mainCamera.orthographicSize / maxZoom;
        var move = new Vector3(-delta.x * moveSpeed * zoomFactor, -delta.y * moveSpeed * zoomFactor, 0);
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
