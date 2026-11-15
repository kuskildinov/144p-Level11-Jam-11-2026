using UnityEngine;

public class Shovle : Interactable
{
    [Header("optional")]
    [SerializeField] private Lamp _lamp;

    public override void Initialize(RoomRoot root)
    {
       
    }

    public override void Interact(Player player)
    {
        player.ToggleShovleVisibility(true);
        this.gameObject.SetActive(false);

        if (_lamp != null)
            _lamp.gameObject.SetActive(true);
    }

    public override void Dispose()
    {

    }
}
