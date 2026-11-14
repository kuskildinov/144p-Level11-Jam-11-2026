using IcePiecesGames.Framework.Audio;
using IcePiecesGames.Framework.Core.Events;
using System;
using UnityEngine;

public class PlayerInterface : MonoBehaviour, IDisposable
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private InteractionInfoPanel _interactionInfo;
    [SerializeField] private GameObject _itemIcon;

    private PlayerRoot _root;

    private IEventBus _eventBus;
    private readonly SubscriptionBag _subscriptionBag = new SubscriptionBag();

    public void Initialize(PlayerRoot root, IEventBus eventBus)
    {
        _root = root;
        _eventBus = eventBus;
        _itemIcon.gameObject.SetActive(false);

        ToggleInteractionInfo(false);
    }

    public void ToggleVisibility(bool value)
    {
        _panel.gameObject.SetActive(value);

        ToggleInteractionInfo(false);
    }

    public void OnItemTaked()
    {
        _itemIcon.gameObject.SetActive(true);
    }

    #region >>> INTERACTION INFO

    public void ToggleInteractionInfo(bool value, string info = "")
    {
        _interactionInfo.ToggleVisibility(value, info);
    }

    public void ShowInteractionMessage(string msg)
    {
        _interactionInfo.ShowMessage(msg);
    }

    #endregion  
    #region >>> EVETNS

    public void SubscribeToEvents()
    {
        _subscriptionBag.Subscribe<InteractiorDetectedEvent>(_eventBus, OnInteractorDetected);
        _subscriptionBag.Subscribe<InteractorLostEvent>(_eventBus, OnInteractorLost);
    }

    public void UnsubscribeFromEvents()
    {
        _subscriptionBag.Clear();
    }

    private void OnInteractorDetected(InteractiorDetectedEvent eventData)
    {
        ToggleInteractionInfo(true, eventData.InteractorText);
    }

    private void OnInteractorLost(InteractorLostEvent eventData)
    {
        ToggleInteractionInfo(false);
    }
   
    #endregion

    public void Dispose()
    {

    }
}
