using IcePiecesGames.Framework.Input;
using UnityEngine;

public class BoilerInspectionController : MonoBehaviour
{
    private const string ExitAction = "Exit";

    [Header("References")]
    [SerializeField] private Boiler _boiler;

    private IInputService _input;
    private bool _isActive;

    public bool IsActive
    {
        get => _isActive;
        set => _isActive = value;
    }

    public void Initialize(IInputService inputService)
    {
        _input = inputService;
        _isActive = false;
    }

    private void Update()
    {
        if (_input == null || !_isActive)
            return;

        if (_input.WasPressedThisFrame(ExitAction))
        {
            _boiler.ExitInspection();
        }
    }

    private void OnDisable()
    {
        _isActive = false;
    }
}