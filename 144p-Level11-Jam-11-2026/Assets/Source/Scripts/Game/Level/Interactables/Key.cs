using UnityEngine;

public class Key : Interactable
{
    [SerializeField] private RoomDoor _door;
    private RoomRoot _root;

    public override void Initialize(RoomRoot root)
    {
        _root = root;
    }

    public override void Interact(Player player)
    {
        player.OnKeyTaked();
        _door.IsOpen = true;

        this.gameObject.SetActive(false);
    }

    public override void Dispose()
    {

    }
}
