using UnityEngine;

public class Key : Interactable
{
    private RoomRoot _root;

    public override void Initialize(RoomRoot root)
    {
        _root = root;
    }

    public override void Interact(Player player)
    {
        player.OnKeyTaked();
        this.gameObject.SetActive(false);
    }

    public override void Dispose()
    {

    }
}
