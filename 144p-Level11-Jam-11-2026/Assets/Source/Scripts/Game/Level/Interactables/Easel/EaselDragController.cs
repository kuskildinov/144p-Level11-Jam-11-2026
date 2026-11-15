using IcePiecesGames.Framework.Input;
using UnityEngine;

public class EaselDragController : MonoBehaviour
{
    private const string PointerAction = "PointerPosition";
    private const string DragAction = "Drag";
    private const string ExitAction = "Exit";

    [Header("References")]
    [SerializeField] private Easel _easel;
    [SerializeField] private Transform _easelSurface;
    [SerializeField] private Camera _inspectionCamera;

    [Header("Interaction")]
    [SerializeField] private LayerMask _pieceLayer;
    [SerializeField] private float _rayDistance = 20f;

    [Header("Movement")]
    [SerializeField]
    private Vector2 _surfaceSize =
        new Vector2(2f, 1.5f);

    [SerializeField] private float _snapDistance = 0.15f;

    private IInputService _input;

    private bool _isActive;
    private EaselPiece _draggedPiece;

    private Plane _dragPlane;
    private Vector3 _dragOffset;
    private float _localDepth;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value)
                return;

            _isActive = value;

            if (!_isActive)
                StopDrag(false);
        }
    }

    public void Initialize(IInputService inputService)
    {
        _input = inputService;
        _isActive = false;

        StopDrag(false);
    }

    private void Update()
    {
        if (_input == null || !_isActive)
            return;

        if (_input.WasPressedThisFrame(ExitAction))
        {
            _easel.ExitInspection();
            return;
        }

        if (_input.WasPressedThisFrame(DragAction))
            TryStartDrag();

        if (_input.IsPressed(DragAction))
            Drag();

        if (_input.WasReleasedThisFrame(DragAction))
            StopDrag(true);
    }

    private Camera GetCamera()
    {
        return _inspectionCamera != null
            ? _inspectionCamera
            : Camera.main;
    }

    private Ray GetPointerRay()
    {
        Vector2 pointer =
            _input.ReadValue<Vector2>(PointerAction);

        return GetCamera().ScreenPointToRay(pointer);
    }

    private void TryStartDrag()
    {
        Camera camera = GetCamera();

        if (camera == null || _easelSurface == null)
            return;

        Ray ray = GetPointerRay();

        if (!Physics.Raycast(
            ray,
            out RaycastHit hit,
            _rayDistance,
            _pieceLayer,
            QueryTriggerInteraction.Ignore))
        {
            return;
        }

        EaselPiece piece =
            hit.collider.GetComponentInParent<EaselPiece>();

        if (piece == null || piece.IsPlaced)
            return;

        _draggedPiece = piece;

        Vector3 localPosition =
            _easelSurface.InverseTransformPoint(
                piece.transform.position
            );

        _localDepth = localPosition.z;

        Vector3 planePoint =
            _easelSurface.TransformPoint(
                new Vector3(0f, 0f, _localDepth)
            );

        _dragPlane = new Plane(
            _easelSurface.forward,
            planePoint
        );

        if (_dragPlane.Raycast(ray, out float distance))
        {
            _dragOffset =
                piece.transform.position -
                ray.GetPoint(distance);
        }
        else
        {
            StopDrag(false);
        }
    }

    private void Drag()
    {
        if (_draggedPiece == null)
            return;

        Ray ray = GetPointerRay();

        if (!_dragPlane.Raycast(ray, out float distance))
            return;

        Vector3 target =
            ray.GetPoint(distance) + _dragOffset;

        Vector3 local =
            _easelSurface.InverseTransformPoint(target);

        local.x = Mathf.Clamp(
            local.x,
            -_surfaceSize.x * 0.5f,
            _surfaceSize.x * 0.5f
        );

        local.y = Mathf.Clamp(
            local.y,
            -_surfaceSize.y * 0.5f,
            _surfaceSize.y * 0.5f
        );

        local.z = _localDepth;

        _draggedPiece.transform.position =
            _easelSurface.TransformPoint(local);
    }

    private void StopDrag(bool checkSnap)
    {
        if (_draggedPiece == null)
            return;

        EaselPiece piece = _draggedPiece;

        _draggedPiece = null;
        _dragOffset = Vector3.zero;

        if (!checkSnap || piece.IsPlaced)
            return;

        if (piece.DistanceToCorrectPosition() <= _snapDistance)
        {
            piece.Place();
            _easel.OnPiecePlaced(piece);
        }
    }

    private void OnDisable()
    {
        _isActive = false;
        StopDrag(false);
    }
}