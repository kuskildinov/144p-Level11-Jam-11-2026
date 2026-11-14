using UnityEngine;

public class RoomDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionText;
    [SerializeField] private bool _isActive;

    public bool IsActive { get => _isActive; set => _isActive = value; }
    public string InteractionText { get => _interactionText; set => _interactionText = value; }

    public void Initialize()
    {

    }

    public void Interact(Player player)
    {
        Debug.Log("Дверь открыта");
    }
}
