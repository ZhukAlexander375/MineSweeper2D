using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float zoomSpeed = 1.0f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;
    private Camera mainCamera;
    private Vector3 touchStartPos;

    private void Start()
    {
        Application.targetFrameRate = 1000;
        mainCamera = Camera.main;
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
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                var delta = (Vector3)touch.position - touchStartPos;
                touchStartPos = touch.position;
                MoveCamera(delta);
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

                ZoomCamera(deltaDistance * zoomSpeed);
            }
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButton(0))
        {
            var delta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
            MoveCamera(delta);
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            ZoomCamera(Input.mouseScrollDelta.y * zoomSpeed);
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
}