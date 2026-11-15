using IcePiecesGames.Framework.Core.Events;
using UnityEngine;

public class Board : Interactable
{
    [Header("Cameras")]
    [SerializeField] private GameObject _boardCamera;

    [Header("Board Items")]
    [SerializeField] private BoardItem[] _items;

    [Header("Controller")]
    [SerializeField] private BoardDragController _dragController;

    private RoomRoot _root;
    private IEventBus _eventBus;

    private readonly SubscriptionBag _subscriptionBag =
        new SubscriptionBag();

    private bool _isInspecting;

    public bool IsInspecting => _isInspecting;

    public override void Initialize(RoomRoot root)
    {
        _root = root;
        _eventBus = root.EventBus;
               
        _isInspecting = false;

        _boardCamera.SetActive(false);

        if (_dragController != null)
            _dragController.IsActive = false;

        

        InitializeItems();
        _subscriptionBag.Clear();
        _subscriptionBag.Subscribe<ItemPickUpEvent>(_eventBus,OnItemPickedUp);
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

        _boardCamera.SetActive(true);

        if (_dragController != null)
            _dragController.IsActive = true;
    }

    public void ExitInspection()
    {
        if (!_isInspecting)
            return;

        if (_dragController != null)
            _dragController.IsActive = false;

        _isInspecting = false;
        IsActive = true;

        _root.TryExitInspectionView();

        _boardCamera.SetActive(false);
    }

    private void OnItemPickedUp(ItemPickUpEvent eventData)
    {
        if (eventData.PickupItem == null)
            return;

        string itemId = eventData.PickupItem.ItemId;

        _root.PlayPaperSound();
        foreach (BoardItem item in _items)
        {
            if (item == null || item.ItemId != itemId)
                continue;

            if (!item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(true);
                IsActive = true;
            }

            return;
        }
    }

    private void InitializeItems()
    {
        foreach (BoardItem item in _items)
        {
            if (item == null || item == _items[0])
                continue;

            item.Initialize(_root);
            item.gameObject.SetActive(false);
        }
    }

    public override void Dispose()
    {
        if (_isInspecting)
            ExitInspection();

        _subscriptionBag.Clear();

        if (_dragController != null)
            _dragController.IsActive = false;
    }
}