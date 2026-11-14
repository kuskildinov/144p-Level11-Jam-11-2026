using IcePiecesGames.Framework.Core.Events;
using IcePiecesGames.Framework.Core.Lifecycle;
using IcePiecesGames.Framework.Input;
using UnityEngine;

[DependsOn(typeof(PlayerRoot))]
public class RoomRoot : FrameworkRoot, IComposable, IShutdownable
{
    [Header("References")]
    [SerializeField] private TableDragController _dragController;

    private Interactable[] _interactables;

    private IEventBus _eventBus;
    private PlayerRoot _playerRoot;
    private IInputService _inputService;

    public IEventBus EventBus => _eventBus;

    public void Compose()
    {
        _playerRoot = Context.GetRoot<PlayerRoot>();
        _inputService = Context.GetService<IInputService>();
        _eventBus = Context.GetService<IEventBus>();

        _interactables = FindObjectsByType<Interactable>();
        InitializeInteractables();

        if(_dragController != null)
            _dragController.Initialize(_inputService);
    }

    private void InitializeInteractables()
    {
        if (_interactables == null || _interactables.Length <= 0)
        {
            Debug.LogWarning("Cant find interactables in room");
            return;
        }

        foreach (Interactable item in _interactables)
        {
            item.Initialize(this);
        }
    }

    public void TryEnterInspectionView()
    {
        _playerRoot.EnterInspectionMode();
        if (_dragController != null) 
            _dragController.IsActive = true;
    }

    public void TryExitInspectionView()
    {       
        _playerRoot.ExitInspectionMode();
        if (_dragController != null) 
            _dragController.IsActive = false;
    }

    public void Shutdown()
    {
        foreach (Interactable item in _interactables)
        {
            item.Dispose();
        }
    }
}
