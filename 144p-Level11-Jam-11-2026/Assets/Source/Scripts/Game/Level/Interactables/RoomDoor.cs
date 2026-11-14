using UnityEngine;

public class RoomDoor : Interactable
{
    [SerializeField] private string _doorCloseMessage = "Заперто";
    private RoomRoot _root;

    public bool IsOpen { get; set; }
   
    public override void Initialize(RoomRoot root)
    {
        _root = root;
    }

    public override void Interact(Player player)
    {
        if(IsOpen)
        {
            _root.TryLoadNextLevel();
        }
        else
        {
            player.ShowInteractionMessage(_doorCloseMessage);
        }       
    }

    public override void Dispose()
    {

    }
}
