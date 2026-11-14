using IcePiecesGames.Framework.Audio;
using IcePiecesGames.Framework.Core.Events;
using System;
using UnityEngine;

public class PlayerInterface : MonoBehaviour, IDisposable
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private InteractionInfoPanel _interactionInfo;

    private PlayerRoot _root;

    private IEventBus _eventBus;   

    public void Initialize(PlayerRoot root, IEventBus eventBus)
    {
        _root = root;
        _eventBus = eventBus;      
       
        ToggleInteractionInfo(false);
        SubscribeToEvents();
    }

    public void ToggleVisibility(bool value)
    {
        _panel.gameObject.SetActive(value);

        ToggleInteractionInfo(false);
    }

    #region >>> INTERACTION INFO

    public void ToggleInteractionInfo(bool value, string info = "")
    {
        _interactionInfo.ToggleVisibility(value, info);
    }

    #endregion      
    #region >>> EVETNS

    public void SubscribeToEvents()
    {
       
    }

    public void UnsubscribeFromEvents()
    {
      
    }
   
    #endregion

    public void Dispose()
    {

    }
}
