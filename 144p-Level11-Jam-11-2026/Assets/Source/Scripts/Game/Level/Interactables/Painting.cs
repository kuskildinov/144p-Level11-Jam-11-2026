using UnityEngine;

public class Painting : Interactable, IPickupable
{   
    [SerializeField] private string _itemId;
    [SerializeField] private Sprite _icon;
        
    public string ItemId => _itemId;
    public Sprite Icon => _icon;

    public override void Initialize(RoomRoot root)
    {
       
    }

    public override void Interact(Player player)
    {
        player.TryTakeItem(this);
        this.gameObject.SetActive(false);
    }

    public override void Dispose()
    {

    }
}
