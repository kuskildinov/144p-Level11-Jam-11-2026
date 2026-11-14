using UnityEngine;

public class RoomDoor : Interactable
{   
    public bool IsOpen { get; set; }

   
    public override void Initialize(RoomRoot root)
    {
       
    }

    public override void Interact(Player player)
    {
        if(IsOpen)
        {
            Debug.Log("Выходим");
        }
        else
        {
            Debug.Log("Дверь закрыта");
        }       
    }

    public override void Dispose()
    {

    }
}
