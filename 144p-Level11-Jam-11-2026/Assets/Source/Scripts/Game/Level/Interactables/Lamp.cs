using UnityEngine;

public class Lamp : Interactable
{    
    [SerializeField] private GameObject _light;
    [SerializeField] private bool _isSwitchOn;
    [Header("optional")]
    [SerializeField] private Shovle _shovle;

    private RoomRoot _root;
    public bool IsSwitchOn => _isSwitchOn;

    public override void Initialize(RoomRoot root)
    {
        ToggleLight(false);

        _root = root;
        _interactionText = "Зажечь лампу";
    }

    public override void Interact(Player player)
    {
        if (_isSwitchOn)
        {
            player.ToggleLampLight(true);
            this.gameObject.SetActive(false);
            _root.PlayTakeLampSound();
            if(_shovle != null)
                _shovle.gameObject.SetActive(true);
        }
        else
        {
            ToggleLight(true);
            _root.PlaySwitchOnLampSound();
            _interactionText = "Взять лампу";
        }
    }

    private void ToggleLight(bool value)
    {
        _light.gameObject.SetActive(value);
        _isSwitchOn = value;
    }

    public override void Dispose()
    {

    }
}
