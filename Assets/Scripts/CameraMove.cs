using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private LayerMask camLayerMask;
    [SerializeField] private CinemachineConfiner2D confiner;

    private InputAction pointAction;
    private InputAction clickAction;
    private InputAction scrollwheelAction;

    private bool isDragging = false;
    private bool isDraggingCard = false;
    private Vector2 lastMousePosition;

    private void Awake()
    {
        pointAction = InputSystem.actions.FindAction("Point");
        clickAction = InputSystem.actions.FindAction("Click");
        scrollwheelAction = InputSystem.actions.FindAction("ScrollWheel");

        pointAction?.Enable();
        clickAction?.Enable();
        scrollwheelAction?.Enable();
    }

    private void Update()
    {
        HandleDrag();
        HandleScroll();
    }

    private void HandleDrag()
    {
        if (clickAction == null || pointAction == null)
            return;

        bool isPressed = clickAction.ReadValue<float>() > 0;

        if (isPressed && !isDragging)
        {
            isDragging = true;
            lastMousePosition = pointAction.ReadValue<Vector2>();

            // 마우스 위치를 월드 좌표로 변환
            Vector3 mouseScreenPos = new Vector3(lastMousePosition.x, lastMousePosition.y, Mathf.Abs(Camera.main.transform.position.z));
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector2 origin = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            // 2D 레이캐스트 수행
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero, 11f, camLayerMask);
            isDraggingCard = (hit.collider != null);
        }
        else if (!isPressed && isDragging)
        {
            isDragging = false;
            isDraggingCard = false;
        }

        if (isDragging && !isDraggingCard)
        {
            Vector2 currentMousePosition = pointAction.ReadValue<Vector2>();
            Vector2 delta = currentMousePosition - lastMousePosition;
            delta *= 0.01f;

            cam.transform.position -= new Vector3(delta.x, delta.y, 0);
            lastMousePosition = currentMousePosition;
        }
    }

    private void HandleScroll()
    {
        if (!QuestScrollHandler.Instance) return;
        
        if (QuestScrollHandler.Instance.IsPointerOver) return;
        
        Vector2 scrollDelta = scrollwheelAction.ReadValue<Vector2>();
        if (scrollDelta.y != 0)
        {
            float zoomAmount = scrollDelta.y * 1f;
            float newFOV = cam.Lens.FieldOfView - zoomAmount;
            cam.Lens.FieldOfView = Mathf.Clamp(newFOV, 75f, 105f);
        }
    }
}
