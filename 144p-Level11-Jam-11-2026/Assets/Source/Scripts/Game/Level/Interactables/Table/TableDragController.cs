using IcePiecesGames.Framework.Input;
using UnityEngine;

public class TableDragController : MonoBehaviour
{
    private const string PointerAction = "PointerPosition";
    private const string DragAction = "Drag";
    private const string ExitAction = "Exit";

    [Header("References")]
    [SerializeField] private Table _table;
    [SerializeField] private Transform _tableSurface;

    [Header("Drag Settings")]
    [SerializeField] private LayerMask _draggableLayer;
    [SerializeField] private float _rayDistance = 20f;

    [Header("Table Bounds")]
    [SerializeField] private Vector2 _tableSize = new Vector2(2f, 1f);

    private IInputService _input;

    private bool _isActive;
    private Transform _draggedObject;
    private Vector3 _dragOffset;
    private Plane _dragPlane;

    private float _objectLocalHeight;

    public bool IsActive { get => _isActive; set => _isActive = value; }

    public void Initialize(IInputService inputService)
    {
        _input = inputService;

        StopDrag();
    }

    private void Update()
    {
        if (_input == null || !_isActive)
            return;

        if (_input.WasPressedThisFrame(ExitAction))
        {
            _table.ExitInspection();
            Debug.Log("Exit");
            return;
        }

        if (_input.WasPressedThisFrame(DragAction))
            TryStartDrag();

        if (_input.IsPressed(DragAction))
            Drag();

        if (_input.WasReleasedThisFrame(DragAction))
            StopDrag();
    }

    private void TryStartDrag()
    {
        Camera camera = Camera.main;
        Vector2 pointerPosition = _input.ReadValue<Vector2>(PointerAction);

        Ray ray = camera.ScreenPointToRay(pointerPosition);

        if (!Physics.Raycast(ray,out RaycastHit hit,_rayDistance,_draggableLayer, QueryTriggerInteraction.Ignore))
            return;

        _draggedObject = hit.collider.transform;
        _objectLocalHeight =_tableSurface.InverseTransformPoint(_draggedObject.position).y;
        Vector3 planePoint =_tableSurface.TransformPoint(new Vector3(0f, _objectLocalHeight, 0f));
        _dragPlane = new Plane(_tableSurface.up,planePoint);

        if (_dragPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            _dragOffset =_draggedObject.position - hitPoint;
        }
        else
        {
            StopDrag();
        }
    }

    private void Drag()
    {
        if (_draggedObject == null)
            return;
       
        Camera camera = Camera.main;
        Vector2 pointerPosition =_input.ReadValue<Vector2>(PointerAction);

        Ray ray = camera.ScreenPointToRay(pointerPosition);

        if (!_dragPlane.Raycast(ray, out float distance))
            return;

        Vector3 targetPosition = ray.GetPoint(distance) + _dragOffset;
        Vector3 localPosition =_tableSurface.InverseTransformPoint(targetPosition);

        localPosition.x = Mathf.Clamp(localPosition.x,-_tableSize.x * 0.5f,_tableSize.x * 0.5f);
        localPosition.z = Mathf.Clamp(localPosition.z,-_tableSize.y * 0.5f,_tableSize.y * 0.5f);
        localPosition.y = _objectLocalHeight;

        _draggedObject.position = _tableSurface.TransformPoint(localPosition);
    }

    private void StopDrag()
    {
        _draggedObject = null;
        _dragOffset = Vector3.zero;
    }
}