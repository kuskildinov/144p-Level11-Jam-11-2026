using IcePiecesGames.Framework.Audio;
using IcePiecesGames.Framework.Core.Events;
using IcePiecesGames.Framework.Core.Lifecycle;
using IcePiecesGames.Framework.Input;
using System;
using UnityEngine;

public class PlayerRoot : FrameworkRoot, IComposable, IRunnable, IShutdownable
{
    private const string FirstPersonInputProfileName = "FirstPerson";
    private const string DefaultUIInputProfileName = "UIOnly";

    [SerializeField] private Player _player;
    [SerializeField] private PlayerInterface _interface;

    private IInputService _inputService;
    private IEventBus _eventBus;
    
    public void Compose()
    {
        _inputService = Context.GetService<IInputService>();
        _eventBus = Context.GetService<IEventBus>();

        _player.Initialize(this, _inputService, _eventBus);
    }

    public void Run()
    {
        SubscribeToEvents();
        TogglePlayerInput(true);
        ToggleInterfaceVisual(true);

        _player.ToggleActivation(true);
    }

    #region >>> PLAYER

    public void TogglePlayerActivation(bool value)
    {
        _player.ToggleActivation(value);
    }
    #endregion
    #region >>> INTERFACE

    public void TogglePlayerInput(bool value)
    {
        string newProfile = value ? FirstPersonInputProfileName : DefaultUIInputProfileName;
        _inputService.SetProfile(newProfile);
    }

    private void ToggleInterfaceVisual(bool value)
    {
        _interface.ToggleVisibility(value);
    }

    #endregion
    #region >>> EVENTS

    private void SubscribeToEvents()
    {
        _interface.SubscribeToEvents();
    }

    private void UnSubscribeToEvents()
    {
        _interface.UnsubscribeFromEvents();
    }  
   
    #endregion

    public void Shutdown()
    {
        UnSubscribeToEvents();

        _player.Dispose();
        _interface.Dispose();
    }
}

public readonly struct InteractiorDetectedEvent
{
    public readonly string InteractorText;

    public InteractiorDetectedEvent(string interactorText)
    {
        InteractorText = interactorText;
    }
}

public readonly struct InteractorLostEvent
{
   
}



