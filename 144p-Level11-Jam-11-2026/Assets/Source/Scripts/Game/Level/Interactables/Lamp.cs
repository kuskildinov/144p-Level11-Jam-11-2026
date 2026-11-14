using UnityEngine;

public class Lamp : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionText;
    [SerializeField] private bool _isActive;
    [SerializeField] private GameObject _light;

    public bool IsActive { get => _isActive; set => _isActive = value; }
    public bool IsSwitchOn { get; set; } = false;
    public string InteractionText { get => _interactionText; set => _interactionText = value; }

    public void Initialize()
    {
        ToggleLight(false);
        _interactionText = "Зажечь лампу";
    }

    public void Interact(Player player)
    {
        if (IsSwitchOn)
        {
            player.ToggleLampLight(true);
            this.gameObject.SetActive(false);
        }
        else
        {
            ToggleLight(true);
            _interactionText = "Взять лампу";
        }
    }

    private void ToggleLight(bool value)
    {
        _light.gameObject.SetActive(value);
        IsSwitchOn = value;
    }
}
