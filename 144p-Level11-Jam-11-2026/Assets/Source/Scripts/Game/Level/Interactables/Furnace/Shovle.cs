using UnityEngine;

public class Shovle : Interactable
{
    [Header("optional")]
    [SerializeField] private Lamp _lamp;

    private RoomRoot _root;

    public override void Initialize(RoomRoot root)
    {
        _root = root;
    }

    public override void Interact(Player player)
    {
        player.ToggleShovleVisibility(true);
        this.gameObject.SetActive(false);

        _root.PlayTakeShovleSound();

        if (_lamp != null)
            _lamp.gameObject.SetActive(true);
    }

    public override void Dispose()
    {

    }
}
