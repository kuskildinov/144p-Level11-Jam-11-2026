
using IcePiecesGames.Framework.Input;
using UnityEngine;

public class BoardDragController : MonoBehaviour
{
    private const string PointerAction = "PointerPosition";
    private const string DragAction = "Drag";
    private const string RotateAction = "Interact";
    private const string ExitAction = "Exit";

    public enum BoardPlane
    {
        XY,
        XZ
    }

    [Header("References")]
    [SerializeField] private Board _board;
    [SerializeField] private Transform _boardSurface;
    [SerializeField] private Camera _inspectionCamera;

    [Header("Interaction")]
    [SerializeField] private LayerMask _itemLayer;
    [SerializeField] private float _rayDistance = 20f;
    [SerializeField] private bool _debug = false;

    [Header("Board Bounds")]
    [SerializeField] private BoardPlane _boardPlane = BoardPlane.XY;
    [SerializeField] private Vector2 _boardSize = new Vector2(2f, 1f);

    private IInputService _input;

    private bool _isActive;
    private BoardItem _hoveredItem;
    private BoardItem _draggedItem;

    private Vector3 _dragOffset;
    private Plane _dragPlane;
    private float _objectLocalDepth;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value)
                return;

            _isActive = value;

            if (!_isActive)
            {
                StopDrag();
                SetHoveredItem(null);
            }
        }
    }

    public void Initialize(IInputService inputService)
    {
        _input = inputService;
        IsActive = false;

        StopDrag();
        SetHoveredItem(null);
    }

    private void Update()
    {
        if (_input == null || !_isActive)
            return;

        if (_input.WasPressedThisFrame(ExitAction))
        {
            _board.ExitInspection();
            return;
        }

        UpdateHover();

        if (_input.WasPressedThisFrame(RotateAction))
            TryRotateItem();

        if (_input.WasPressedThisFrame(DragAction))
            TryStartDrag();

        if (_input.IsPressed(DragAction))
            Drag();

        if (_input.WasReleasedThisFrame(DragAction))
            StopDrag();
    }

    private Camera GetCamera()
    {
        return _inspectionCamera != null
            ? _inspectionCamera
            : Camera.main;
    }

    private Ray GetPointerRay()
    {
        Vector2 pointerPosition =
            _input.ReadValue<Vector2>(PointerAction);

        return GetCamera().ScreenPointToRay(pointerPosition);
    }

    private BoardItem GetItemUnderPointer()
    {
        Camera camera = GetCamera();

        if (camera == null)
            return null;

        Ray ray = GetPointerRay();

        if (!Physics.Raycast(
            ray,
            out RaycastHit hit,
            _rayDistance,
            _itemLayer,
            QueryTriggerInteraction.Ignore))
        {
            return null;
        }

        return hit.collider.GetComponentInParent<BoardItem>();
    }

    private void UpdateHover()
    {
        if (_draggedItem != null)
        {
            SetHoveredItem(_draggedItem);
            return;
        }

        SetHoveredItem(GetItemUnderPointer());
    }

    private void SetHoveredItem(BoardItem item)
    {
        if (_hoveredItem == item)
            return;

        if (_hoveredItem != null)
            _hoveredItem.SetHighlight(false);

        _hoveredItem = item;

        if (_hoveredItem != null)
        {
            _hoveredItem.SetHighlight(true);

            if (_debug)
                Debug.Log("Board hover: " + _hoveredItem.name);
        }
    }

    private void TryRotateItem()
    {
        if (_draggedItem != null || _hoveredItem == null)
            return;

        if (_debug)
            Debug.Log("Board flip: " + _hoveredItem.name);

        _hoveredItem.Flip();
    }

    private void TryStartDrag()
    {
        BoardItem item = GetItemUnderPointer();

        if (item == null || item.IsRotating)
            return;

        _draggedItem = item;

        Vector3 localPosition =
            _boardSurface.InverseTransformPoint(
                _draggedItem.transform.position
            );

        Vector3 normal;
        Vector3 planePoint;

        if (_boardPlane == BoardPlane.XY)
        {
            _objectLocalDepth = localPosition.z;
            normal = _boardSurface.forward;

            planePoint = _boardSurface.TransformPoint(
                new Vector3(0f, 0f, _objectLocalDepth)
            );
        }
        else
        {
            _objectLocalDepth = localPosition.y;
            normal = _boardSurface.up;

            planePoint = _boardSurface.TransformPoint(
                new Vector3(0f, _objectLocalDepth, 0f)
            );
        }

        _dragPlane = new Plane(normal, planePoint);

        Ray ray = GetPointerRay();

        if (!_dragPlane.Raycast(ray, out float distance))
        {
            StopDrag();
            return;
        }

        Vector3 hitPoint = ray.GetPoint(distance);

        _dragOffset =
            _draggedItem.transform.position - hitPoint;

        SetHoveredItem(_draggedItem);
    }

    private void Drag()
    {
        if (_draggedItem == null)
            return;

        Ray ray = GetPointerRay();

        if (!_dragPlane.Raycast(ray, out float distance))
            return;

        Vector3 targetPosition =
            ray.GetPoint(distance) + _dragOffset;

        Vector3 localPosition =
            _boardSurface.InverseTransformPoint(targetPosition);

        if (_boardPlane == BoardPlane.XY)
        {
            localPosition.x = Mathf.Clamp(
                localPosition.x,
                -_boardSize.x * 0.5f,
                _boardSize.x * 0.5f
            );

            localPosition.y = Mathf.Clamp(
                localPosition.y,
                -_boardSize.y * 0.5f,
                _boardSize.y * 0.5f
            );

            localPosition.z = _objectLocalDepth;
        }
        else
        {
            localPosition.x = Mathf.Clamp(
                localPosition.x,
                -_boardSize.x * 0.5f,
                _boardSize.x * 0.5f
            );

            localPosition.z = Mathf.Clamp(
                localPosition.z,
                -_boardSize.y * 0.5f,
                _boardSize.y * 0.5f
            );

            localPosition.y = _objectLocalDepth;
        }

        _draggedItem.transform.position =
            _boardSurface.TransformPoint(localPosition);
    }

    private void StopDrag()
    {
        _draggedItem = null;
        _dragOffset = Vector3.zero;
    }

    private void OnDisable()
    {
        IsActive = false;
        StopDrag();
        SetHoveredItem(null);
    }
}
