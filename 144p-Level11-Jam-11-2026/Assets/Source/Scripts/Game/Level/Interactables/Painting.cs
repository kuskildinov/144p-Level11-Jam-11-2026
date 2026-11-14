using UnityEngine;

public class Painting : MonoBehaviour, IInteractable, IPickupable
{
    [SerializeField] private string _interactionText;
    [SerializeField] private bool _isActive;
    [SerializeField] private string _itemId;
    [SerializeField] private Sprite _icon;

    public bool IsActive { get => _isActive; set => _isActive = value; }
    public string InteractionText { get => _interactionText; set => _interactionText = value; }
    public string ItemId => _itemId;
    public Sprite Icon => _icon;

    public void Interact(Player player)
    {
        player.TryTakeItem(this);
        this.gameObject.SetActive(false);
    }
}
