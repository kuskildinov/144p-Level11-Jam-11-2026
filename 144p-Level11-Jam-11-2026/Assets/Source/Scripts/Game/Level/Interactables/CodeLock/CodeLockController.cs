
using IcePiecesGames.Framework.Input;
using UnityEngine;

public class CodeLockController : MonoBehaviour
{
    private const string PointerAction = "PointerPosition";
    private const string ClickAction = "Drag";
    private const string ExitAction = "Exit";

    [Header("References")]
    [SerializeField] private CodeLock _codeLock;
    [SerializeField] private Camera _inspectionCamera;

    [Header("Interaction Settings")]
    [SerializeField] private LayerMask _buttonLayer;
    [SerializeField] private float _rayDistance = 20f;

    private IInputService _input;
    private bool _isActive;

    public bool IsActive
    {
        get => _isActive;
        set => _isActive = value;
    }

    public void Initialize(IInputService inputService)
    {
        _input = inputService;
        _isActive = false;
    }

    private void Update()
    {
        if (_input == null || !_isActive)
            return;

        if (_input.WasPressedThisFrame(ExitAction))
        {
            ExitInspection();
            return;
        }

        if (_input.WasPressedThisFrame(ClickAction))
            TryPressButton();
    }

    private void TryPressButton()
    {
        Camera camera = Camera.main;

        if (camera == null)
            return;

        Vector2 pointerPosition =
            _input.ReadValue<Vector2>(PointerAction);

        Ray ray = camera.ScreenPointToRay(pointerPosition);

        if (!Physics.Raycast(
                ray,
                out RaycastHit hit,
                _rayDistance,
                _buttonLayer,
                QueryTriggerInteraction.Ignore))
        {
            return;
        }

        if (hit.collider.TryGetComponent(
                out CodeLockButton button))
        {
            button.Press();
        }
    }

    private void ExitInspection()
    {
        _isActive = false;
       _codeLock.ExitInspection();

        Debug.Log("CodeLock: Exit inspection");
    }

    private void OnDisable()
    {
        _isActive = false;
    }
}