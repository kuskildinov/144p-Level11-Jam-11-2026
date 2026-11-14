using UnityEngine;
using Cinemachine;
using System;
using IcePiecesGames.Framework.Core.Events;
using IcePiecesGames.Framework.Input;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour, IDisposable
{
    [Header("Components")]
    [SerializeField] private PlayerMovment _movment;
    [SerializeField] private PlayerInteraction _interaction;
    [Header("Other")]
    [SerializeField] private CinemachineVirtualCamera _playerCam;
    [SerializeField] private GameObject _lampLight;

    private PlayerRoot _root;
       
    private IInputService _inputService;
    private IEventBus _eventBus;

    private bool _isActive;
    private CharacterController _controller;

    public bool IsActive => _isActive;
    public CinemachineVirtualCamera PlayerCam => _playerCam;
    public CharacterController Controller => _controller;

    public void Initialize(PlayerRoot root, IInputService inputService, IEventBus eventBus)
    {
        _root = root;       
        _inputService = inputService;
        _eventBus = eventBus;

        _controller = GetComponent<CharacterController>();

        _movment.Initialize(this, _inputService);
        _interaction.Initialize(this, _inputService, _eventBus);
    }

    public void ToggleActivation(bool value)
    {
        _isActive = value;
    }

    #region >>> LAMP LIGHT

    public void ToggleLampLight(bool value)
    {
        _lampLight.gameObject.SetActive(value);
    }

    #endregion
    #region >>> INVENTORY

    public void TryTakeItem(IPickupable item)
    {
        _root.OnItemPickedUp(item);
    }

    public void OnKeyTaked()
    {
        _root.OnKeyTaked();
    }

    #endregion
    #region >>> INTERACTOR MSG

    public void ShowInteractionMessage(string msg)
    {
        _root.ShowInteractionMessage(msg);
    }

    #endregion

    public void Dispose()
    {
       _interaction.Dispose();
    }
}
