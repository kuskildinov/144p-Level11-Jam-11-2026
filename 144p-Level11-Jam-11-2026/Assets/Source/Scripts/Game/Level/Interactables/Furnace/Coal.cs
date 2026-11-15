using UnityEngine;

public class Coal : Interactable
{
    [SerializeField] private Furnace _furnance;

    public override void Initialize(RoomRoot root)
    {
      
    }

    public override void Interact(Player player)
    {
       if(player.IsShovleOnHand)
        {
            if(player.Shovle.IsEmpty)
            {
                player.TryTakeCoal();
                _furnance.UpdateNote();
            }
            else
            {
                player.ShowInteractionMessage("Больше не взять");
            }          
        }
       else
        {
            player.ShowInteractionMessage("Нужна лопата");
        }
    }

    public override void Dispose()
    {

    }
}
