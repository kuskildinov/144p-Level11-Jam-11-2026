using UnityEngine;

public class Letter : Interactable, IInspectable
{
    [Header("Inspection")]
    [SerializeField] private bool _needAction;
    [SerializeField] private GameObject _inspectionVisual;
    [SerializeField] private string _actionText;
    [Header("other")]
    [SerializeField] private RoomDoor _door;

    private ItemInspectionController _inspectionController;

    public string ActionText => _actionText;
    public bool NeedAction => _needAction;
    
    public override void Initialize(RoomRoot root)
    {
        IsActive = false;
        _inspectionController = FindAnyObjectByType<ItemInspectionController>();
    }

    public override void Interact(Player player)
    {
        if (_inspectionController == null)
        {
            Debug.LogError($"Painting {name}: Inspection is not configured.");
            return;
        }

        player.PlayVoiceByIndex(17);
        _inspectionController.Open(this, _inspectionVisual);
        _door.IsOpen = true;
    }

    public void ExecuteInspectionAction()
    {

    }

    public override void Dispose()
    {

    }
}
