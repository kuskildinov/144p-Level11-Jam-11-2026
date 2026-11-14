
using System;
using Cinemachine;
using UnityEngine;
using IcePiecesGames.Framework.Core.Events;
using IcePiecesGames.Framework.Input;

public class PlayerInteraction : MonoBehaviour, IDisposable
{
    [Header("Settings")]
    [SerializeField] private float _interactionDistance = 2.5f;
    [SerializeField] private LayerMask _interactionMask = ~0;

    private Player _root;
    private CinemachineVirtualCamera _playerCam => _root.PlayerCam;

    private IEventBus _eventsBus;
    private IInputService _inputService;

    private Interactable _currentInteractable;

    private bool _isDisposed;

    public void Initialize(
        Player root,
        IInputService inputService,
        IEventBus eventBus)
    {
        _root = root;
        _inputService = inputService;
        _eventsBus = eventBus;

        _isDisposed = false;
    }

    private void Update()
    {
        if (_isDisposed || _root == null)
            return;

        if (!_root.IsActive)
        {
            ClearInteraction();
            return;
        }

        FindInteractable();
        InteractionPressHandler();
    }

    private void InteractionPressHandler()
    {
        if (_inputService.InteractPressed)
            OnInteract();
    }

    private void FindInteractable()
    {
        Interactable detectedInteractable = null;

        Ray ray = new Ray(
            _playerCam.transform.position,
            _playerCam.transform.forward
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            _interactionDistance,
            _interactionMask,
            QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.TryGetComponent<Interactable>(
                out Interactable interactable))
            {
                if (interactable.IsActive)
                {
                    detectedInteractable = interactable;
                }
            }
        }

        SetCurrentInteractable(detectedInteractable);
    }

    public void OnInteract()
    {
        if (_root == null || !_root.IsActive)
            return;

        Interactable interactable = _currentInteractable;

        if (interactable == null)
            return;

        // Сначала убираем подсветку и подсказку.
        ClearInteraction();

        // Затем выполняем действие.
        interactable.Interact(_root);
    }

    private void SetCurrentInteractable(Interactable interactable)
    {
        // Если смотрим на тот же объект, ничего не меняем.
        if (_currentInteractable == interactable)
            return;

        // Убираем Outline с предыдущего объекта.
        if (_currentInteractable != null)
        {
            _currentInteractable.ToggleOutline(false);
            OnInteractorLost();
        }

        // Запоминаем новый объект.
        _currentInteractable = interactable;

        // Включаем Outline нового объекта.
        if (_currentInteractable != null)
        {
            _currentInteractable.ToggleOutline(true);
            OnInteractorDetected(_currentInteractable);
        }
    }

    private void OnInteractorDetected(Interactable interactable)
    {
        _eventsBus.Publish(
            new InteractiorDetectedEvent(interactable.InteractionText)
        );
    }

    private void OnInteractorLost()
    {
        _eventsBus.Publish(new InteractorLostEvent());
    }

    private void ClearInteraction()
    {
        SetCurrentInteractable(null);
    }

    private void OnDisable()
    {
        ClearInteraction();
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;

        ClearInteraction();

        enabled = false;
    }
}