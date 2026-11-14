using UnityEngine;

public class RoomDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionText;
    [SerializeField] private bool _isActive;

    public bool IsActive { get => _isActive; set => _isActive = value; }
    public bool IsOpen { get; set; }
    public string InteractionText { get => _interactionText; set => _interactionText = value; }

    public void Initialize()
    {

    }

    public void Interact(Player player)
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
}
