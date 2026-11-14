using IcePiecesGames.Framework.Input;
using UnityEngine;
using UnityEngine.UI;

public class ItemInspectionController : MonoBehaviour
{
    private const string PointerAction = "PointerPosition";
    private const string RotateAction = "Rotate";
    private const string ExitAction = "Exit";

    [Header("References")]
    [SerializeField] private PlayerRoot _playerRoot;
    [SerializeField] private GameObject _inspectionPanel;
    [SerializeField] private Transform _previewPivot;
    [SerializeField] private Camera _previewCamera;

    [Header("UI")]
    [SerializeField] private Button _actionButton;
    [SerializeField] private Text _actionButtonText;
    [SerializeField] private Button _leaveButton;

    [Header("Preview")]
    [SerializeField] private string _previewLayerName = "InspectionItem";
    [SerializeField] private float _rotationSensitivity = 0.35f;

    private IInputService _input;
    private IInspectable _currentItem;
    private GameObject _previewInstance;

    private Vector2 _previousPointerPosition;
    private bool _wasRotating;
    private bool _isInspecting;

    private Quaternion _initialRotation;
    private Vector3 _rotationAxis = Vector3.up;
    private float _rotationAngle;

    public bool IsInspecting => _isInspecting;

    public void Initialize()
    {
        if (_inspectionPanel != null)
            _inspectionPanel.SetActive(false);

        if (_actionButton != null)
            _actionButton.onClick.AddListener(ExecuteAction);

        if (_leaveButton != null)
            _leaveButton.onClick.AddListener(LeaveItem);
    }

    public void Open(IInspectable item, GameObject previewPrefab)
    {
        if (_isInspecting)
            return;

        if (_playerRoot == null || _previewPivot == null || _previewCamera == null || _inspectionPanel == null || item == null || previewPrefab == null)
        {
            Debug.LogError("ItemInspectionController: Missing references.");
            return;
        }

        _input = _playerRoot.InputService;

        if (_input == null)
        {
            Debug.LogError("ItemInspectionController: InputService is null.");
            return;
        }

        int previewLayer = LayerMask.NameToLayer(_previewLayerName);
        if (previewLayer < 0)
        {
            Debug.LogError($"Inspection layer not found: {_previewLayerName}");
            return;
        }

        _currentItem = item;

        _previewInstance = Instantiate(previewPrefab,_previewPivot);

        _previewInstance.transform.localPosition = Vector3.zero;
        _previewInstance.transform.localRotation = Quaternion.identity;

        InspectionItemSettings settings =_previewInstance.GetComponent<InspectionItemSettings>();

        if (settings != null)
        {
            _previewInstance.transform.localPosition = settings.PositionOffset;
            _previewInstance.transform.localRotation = settings.RotationOffset;
            _previewInstance.transform.localScale = Vector3.Scale(_previewInstance.transform.localScale, settings.ScaleMultiplier);

            _rotationAxis = settings.RotationAxis;
        }
        else
        {
            _rotationAxis = Vector3.up;
        }

        _initialRotation = _previewInstance.transform.localRotation;

        _rotationAngle = 0f;

        SetLayerRecursively(_previewInstance.transform, previewLayer);

        if (_actionButtonText != null)
            _actionButtonText.text = item.ActionText;

        _isInspecting = true;
        _wasRotating = false;

        _playerRoot.EnterItemInspectionMode();
        _inspectionPanel.SetActive(true);
    }

    private void Update()
    {
        if (!_isInspecting || _input == null)
            return;

        if (_input.WasPressedThisFrame(ExitAction))
        {
            LeaveItem();
            return;
        }

        HandleRotation();
    }

    private void HandleRotation()
    {
        if (_previewInstance == null)
            return;

        Vector2 pointerPosition = _input.ReadValue<Vector2>(PointerAction);
        bool isRotating = _input.IsPressed(RotateAction);

        if (!isRotating)
        {
            _wasRotating = false;
            return;
        }

        if (!_wasRotating)
        {
            _previousPointerPosition = pointerPosition;
            _wasRotating = true;
            return;
        }

        float deltaX = pointerPosition.x - _previousPointerPosition.x;
        _previousPointerPosition = pointerPosition;
        _rotationAngle -= deltaX * _rotationSensitivity;
        _previewInstance.transform.localRotation = Quaternion.AngleAxis( _rotationAngle,Vector3.up) * _initialRotation;
    }

    public void ExecuteAction()
    {
        if (!_isInspecting || _currentItem == null)
            return;

        IInspectable item = _currentItem;
        CloseInspection();
        item.ExecuteInspectionAction();
    }

    public void LeaveItem()
    {
        if (!_isInspecting)
            return;

        CloseInspection();
    }

    private void CloseInspection()
    {
        _isInspecting = false;
        _wasRotating = false;

        if (_inspectionPanel != null)
            _inspectionPanel.SetActive(false);

        if (_previewInstance != null)
        {
            Destroy(_previewInstance);
            _previewInstance = null;
        }

        _currentItem = null;
        _rotationAngle = 0f;

        _playerRoot.ExitItemInspectionMode();
    }

    private void SetLayerRecursively(Transform target, int layer)
    {
        target.gameObject.layer = layer;
        foreach (Transform child in target)
        {
            SetLayerRecursively(child, layer);
        }
    }

    private void OnDestroy()
    {
        if (_actionButton != null)
            _actionButton.onClick.RemoveListener(ExecuteAction);

        if (_leaveButton != null)
            _leaveButton.onClick.RemoveListener(LeaveItem);
    }
}