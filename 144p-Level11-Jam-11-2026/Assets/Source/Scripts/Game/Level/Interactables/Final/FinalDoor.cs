using UnityEngine;

public class FinalDoor : Interactable
{
    [SerializeField] private string _endingSceneName;

    private RoomRoot _root;

    public override void Dispose()
    {
       
    }

    public override void Initialize(RoomRoot root)
    {
        _root = root;
    }

    public override void Interact(Player player)
    {
        if (_endingSceneName == "Final_1")
        {
            _root.PlayFinalOneSound();
        }
        else
        {
            _root.PlayFinalTwoSound();
        }
        _root.LoadSceneByName(_endingSceneName);
    }
}
