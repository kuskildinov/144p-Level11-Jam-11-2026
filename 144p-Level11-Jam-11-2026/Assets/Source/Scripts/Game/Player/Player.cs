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
    [SerializeField] private ShovleOnHand _shovle;

    private PlayerRoot _root;
       
    private IInputService _inputService;
    private IEventBus _eventBus;

    private bool _isActive;
    private bool _isLightOn;
    private bool _isShovelOnHands;

    private CharacterController _controller;

    public bool IsActive => _isActive;
    public bool IsShovleOnHand => _isShovelOnHands;
    public CinemachineVirtualCamera PlayerCam => _playerCam;
    public CharacterController Controller => _controller;
    public ShovleOnHand Shovle => _shovle;

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
        _isLightOn = value;
        _lampLight.gameObject.SetActive(value);
        if(_shovle != null && _shovle.gameObject.activeInHierarchy)
        {
            _shovle.gameObject.SetActive(!value);
            _isShovelOnHands = !value;
        }
    }

    #endregion
    #region >>> SHOVLE

    public void ToggleShovleVisibility(bool value)
    {
        if (_shovle == null)
            return;

        _isShovelOnHands = value;
        _shovle.gameObject.SetActive(value);
        if (_lampLight != null && _lampLight.gameObject.activeInHierarchy)
        {
            _lampLight.gameObject.SetActive(!value);
            _isLightOn = !value;
        }
    }

    public void TryTakeCoal()
    {
        _shovle.AddCoal();
    }

    public void RemoveCoal()
    {
        _shovle.RemoveCoal();
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

    public void PlayStepSound(bool isMove)
    {
        _root.PlayStepSound(isMove);
    }

    public void Dispose()
    {
       _interaction.Dispose();
    }
}
