using UnityEngine;

public class Key : Interactable, IInspectable
{
    [SerializeField] private RoomDoor _door;
    [Header("Inspection")]
    [SerializeField] private GameObject _inspectionVisual;
    [SerializeField] private string _actionText;

    private RoomRoot _root;
    private Player _player;
    private ItemInspectionController _inspectionController;

    public string ActionText => _actionText;

    public override void Initialize(RoomRoot root)
    {
        _root = root;
        _inspectionController = FindAnyObjectByType<ItemInspectionController>();
    }

    public override void Interact(Player player)
    {
        if (!IsActive)
            return;

        if (_inspectionController == null)
        {
            Debug.LogError($"Painting {name}: Inspection is not configured.");
            return;
        }

        _player = player;
        _inspectionController.Open(this, _inspectionVisual);       
    }

    public void ExecuteInspectionAction()
    {
        _player.OnKeyTaked();
        _door.IsOpen = true;

        this.gameObject.SetActive(false);
    }

    public override void Dispose()
    {

    }

    
}
