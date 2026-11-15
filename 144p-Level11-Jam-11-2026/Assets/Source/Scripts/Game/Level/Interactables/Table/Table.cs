using IcePiecesGames.Framework.Core.Events;
using UnityEngine;

public class Table : Interactable
{   
    [Header("Cameras")]
    [SerializeField] private GameObject _tableCamera;
    [Header("Draggables")]
    [SerializeField] private DraggablePainting[] _painting;
    [Header("other links")]
    [SerializeField] private KeyCarpet _keyCarpet;
        
    private RoomRoot _root;
    private bool _isInspecting;
    private IEventBus _eventBus;
    private readonly SubscriptionBag _subscriptionBag = new SubscriptionBag();

    public override void Initialize(RoomRoot root)
    {
        _root = root;
        _eventBus = _root.EventBus;
        IsActive = false;

        _tableCamera.gameObject.SetActive(false);
        ToggleAllDraggablesVisibility(false);

        SubscribeToEvents();
    }

    public override void Interact(Player player)
    {
        if (!IsActive || _isInspecting)
            return;

        EnterInspection();
    }

    private void EnterInspection()
    {        
        _isInspecting = true;
        IsActive = false;
        _root.TryEnterInspectionView();

        _tableCamera.gameObject.SetActive(true);
    }

    public void ExitInspection()
    {
        if (!_isInspecting)
            return;

        IsActive = true; 
        _isInspecting = false;
        _root.TryExitInspectionView();

        _tableCamera.gameObject.SetActive(false);
    }

    #region >>> DRAGABLES

    public void OnPaintingPickedup(IPickupable item)
    {
        foreach (DraggablePainting painting in _painting)
        {
            if (painting.ItemId == item.ItemId)
            {
                if (!IsActive)
                    IsActive = true;

                painting.gameObject.SetActive(true);
                _root.PlayPaperSound();

                if (CheckAllPaintingsOnTable())
                {
                    _keyCarpet.IsActive = true;
                }
                return;
            }
        }
    }

    private void ToggleAllDraggablesVisibility(bool value)
    {
        foreach (DraggablePainting item in _painting)
        {
            item.gameObject.SetActive(value);
        }
    }

    private bool CheckAllPaintingsOnTable()
    {
        foreach (DraggablePainting painting in _painting)
        {
            if (!painting.gameObject.activeInHierarchy)
                return false;
        }
        return true;
    }

    #endregion
    #region >>> EVENTS

    private void SubscribeToEvents()
    {
        _subscriptionBag.Subscribe<ItemPickUpEvent>(_eventBus, OnItemPickedUp);
    }

    private void UnsubscribeToEvents()
    {
        _subscriptionBag.Clear();
    }

    private void OnItemPickedUp(ItemPickUpEvent eventData)
    {
        OnPaintingPickedup(eventData.PickupItem);
    }

    #endregion

    public override void Dispose()
    {
        UnsubscribeToEvents();
    }
}