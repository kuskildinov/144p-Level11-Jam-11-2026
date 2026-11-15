
using UnityEngine;

public class PickablePicture : Interactable, IPickupable, IInspectable
{
    [Header("Item Data")]
    [SerializeField] private string _itemId;

    [Header("Inspection")]
    [SerializeField] private bool _needAction = true;
    [SerializeField] private GameObject _inspectionVisual;
    [SerializeField] private string _actionText = "Положить на стол";

    [Header("comments")]
    [SerializeField] private bool _needComment = false;
    [SerializeField] private int _commentIndex;
    private bool _isPlayed;

    private Player _player;
    private ItemInspectionController _inspectionController;
    private bool _isPickedUp;
   

    public string ItemId => _itemId;
    public string ActionText => _actionText;

    public bool NeedAction => _needAction;

    public override void Initialize(RoomRoot root)
    {
        _isPickedUp = false;
        _inspectionController = FindAnyObjectByType<ItemInspectionController>();
    }

    public override void Interact(Player player)
    {
        if (!IsActive || _isPickedUp)
            return;

        if (_inspectionController == null || _inspectionVisual == null)
        {
            Debug.LogError($"Painting {name}: Inspection is not configured."
            );
            return;
        }

        _player = player;

        _inspectionController.Open(this, _inspectionVisual);

        if (_needComment && !_isPlayed)
        {
            _isPlayed = true;
            player.PlayVoiceByIndex(_commentIndex);
        }
    }

    public void ExecuteInspectionAction()
    {
        CompletePickup();
    }

    public void CompletePickup()
    {
        if (_isPickedUp || _player == null)
            return;

        _isPickedUp = true;
        IsActive = false;

        _player.TryTakeItem(this);

        gameObject.SetActive(false);
    }
   
    public override void Dispose()
    {
        _player = null;
    }
}