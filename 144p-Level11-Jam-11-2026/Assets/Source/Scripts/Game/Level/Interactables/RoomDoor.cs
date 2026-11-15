using UnityEngine;

public class RoomDoor : Interactable
{
    [SerializeField] private string _doorCloseMessage = "Заперто";
    [SerializeField] private bool _needComment = false;

    private bool _isPlayed;

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

            if(_needComment && !_isPlayed)
            {
                _isPlayed = true;
                player.PlayVoiceByIndex(1);
            }
        }       
    }

    public override void Dispose()
    {

    }
}
