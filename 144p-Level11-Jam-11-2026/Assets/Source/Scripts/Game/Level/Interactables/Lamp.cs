using UnityEngine;

public class Lamp : Interactable
{    
    [SerializeField] private GameObject _light;
       
    public bool IsSwitchOn { get; set; } = false;  

    public override void Initialize(RoomRoot root)
    {
        ToggleLight(false);
        _interactionText = "Зажечь лампу";
    }

    public override void Interact(Player player)
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

    public override void Dispose()
    {

    }
}
