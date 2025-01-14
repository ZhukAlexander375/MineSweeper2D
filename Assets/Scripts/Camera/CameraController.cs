using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool IsCameraInteracting => IsMoving || IsZooming;
    public bool IsMoving { get; private set; }
    public bool IsZooming { get; private set; }
    public bool HasFinishedInteracting { get; private set; } = false;

    [Header("Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;
    [SerializeField] private float deadZoneThrashhold;    

    private Camera mainCamera;
    private Vector3 touchStartPos;
    private ThemeConfig _currentAppliedTheme;
    private Vector3 previousMousePosition;
    private Vector3 cameraCurrentPosition;
    private bool isSwitchingToSingleTouch = false;
    private Vector3 lastTouchPosition;

    private void Start()
    {
        Application.targetFrameRate = 240;
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

    public void ResetFinishedInteracting()
    {
        if (HasFinishedInteracting)
        {
            HasFinishedInteracting = false;
        }
    }

    /// <summary>
    /// 
    /// 
    /// Õ¿◊»Õ¿ﬁ Œ“—ﬁƒ¿!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    /// 
    /// 
    /// 
    /// </summary>
    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);

            if (IsZooming)
            {
                IsZooming = false;
                isSwitchingToSingleTouch = true;
                lastTouchPosition = touch.position;
                return;
            }

            if (isSwitchingToSingleTouch)
            {
                if (touch.phase == TouchPhase.Moved && Vector3.Distance(touch.position, lastTouchPosition) > deadZoneThrashhold)
                {
                    isSwitchingToSingleTouch = false;
                    IsMoving = true;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    isSwitchingToSingleTouch = false;
                }
                return;
            }
            

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                var delta = (Vector3)touch.position - touchStartPos;

                if (delta.magnitude > deadZoneThrashhold)
                {
                    IsMoving = true;
                    MoveCamera(delta);
                    touchStartPos = touch.position;
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                IsMoving = false;
                HasFinishedInteracting = true;
            }
        }
        else if (Input.touchCount == 2)
        {
            var touch0 = Input.GetTouch(0);
            var touch1 = Input.GetTouch(1);

            var currentDistance = Vector2.Distance(touch0.position, touch1.position);
            var previousDistance = Vector2.Distance(
                touch0.position - touch0.deltaPosition,
                touch1.position - touch1.deltaPosition
            );

            var deltaDistance = currentDistance - previousDistance;

            IsZooming = true;
            isSwitchingToSingleTouch = false;
            ZoomCamera(deltaDistance * zoomSpeed);
        }
        else
        {
            IsZooming = false;
        }
    }


    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            previousMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            var delta = Input.mousePosition - previousMousePosition;

            if (delta.magnitude > deadZoneThrashhold)
            {
                IsMoving = true;
                MoveCamera(delta);
                previousMousePosition = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            IsMoving = false;
            HasFinishedInteracting = true;
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            IsZooming = true;
            ZoomCamera(Input.mouseScrollDelta.y * zoomSpeed);
        }
        else
        {
            IsZooming = false;
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
        float zoomSpeed = GameSettingsManager.Instance.CameraZoom;
        var newSize = mainCamera.orthographicSize - deltaDistance * zoomSpeed;
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
