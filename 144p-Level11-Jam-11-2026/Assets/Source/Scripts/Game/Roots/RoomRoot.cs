using IcePiecesGames.Framework.Audio;
using IcePiecesGames.Framework.Core.Events;
using IcePiecesGames.Framework.Core.Lifecycle;
using IcePiecesGames.Framework.Input;
using UnityEngine;

[DependsOn(typeof(PlayerRoot))]
[DependsOn(typeof(LevelRoot))]
public class RoomRoot : FrameworkRoot, IComposable, IShutdownable
{
    [Header("References")]
    [SerializeField] private TableDragController _dragController;
    [SerializeField] private CodeLockController _codeLockController;
    [SerializeField] private BoardDragController _boardDragController;
    [SerializeField] private BoilerInspectionController _boilerController;
    [SerializeField] private EaselDragController _easelController;
    [Header("Sounds")]
    [SerializeField] private AudioClip _paperSound;
    [SerializeField] private AudioClip _switchOnLampSound;
    [SerializeField] private AudioClip _takeLampSound;
    [SerializeField] private AudioClip _carpetSound;
    [SerializeField] private AudioClip _keySound;
    [SerializeField] private AudioClip _doorOpenSound;
    [SerializeField] private AudioClip _turnPhotoSound;
    [SerializeField] private AudioClip _lockButtonSound;
    [SerializeField] private AudioClip _lockOpendSound;

    private Interactable[] _interactables;

    private IEventBus _eventBus;
    private PlayerRoot _playerRoot;
    private LevelRoot _levelRoot;
    private IInputService _inputService;
    private IAudioService _audioService;

    public IEventBus EventBus => _eventBus;

    public void Compose()
    {
        _playerRoot = Context.GetRoot<PlayerRoot>();
        _levelRoot = Context.GetRoot<LevelRoot>();

        _inputService = Context.GetService<IInputService>();
        _eventBus = Context.GetService<IEventBus>();
        _audioService = Context.GetService<IAudioService>();

        _interactables = FindObjectsByType<Interactable>();
        InitializeInteractables();

        if(_dragController != null)
            _dragController.Initialize(_inputService);

        if (_codeLockController != null)
            _codeLockController.Initialize(_inputService);

        if (_boardDragController != null)
            _boardDragController.Initialize(_inputService);

        if (_boilerController != null)
            _boilerController.Initialize(_inputService);

        if (_easelController != null)
            _easelController.Initialize(_inputService);
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

        if (_codeLockController != null)
            _codeLockController.IsActive = true;

        if (_boardDragController != null)
            _boardDragController.IsActive = true;

        if (_boilerController != null)
            _boilerController.IsActive = true;

        if (_easelController != null)
            _easelController.IsActive = true;
    }

    public void TryExitInspectionView()
    {       
        _playerRoot.ExitInspectionMode();
        if (_dragController != null) 
            _dragController.IsActive = false;

        if (_codeLockController != null)
            _codeLockController.IsActive = false;

        if (_boardDragController != null)
            _boardDragController.IsActive = false;

        if (_boilerController != null)
            _boilerController.IsActive = false;

        if (_easelController != null)
            _easelController.IsActive = false;
    }

    public void TrySwitchOnLight()
    {
        Debug.Log("¬ключить свет");
    }

    public void LoadSceneByName(string name)
    {
        _levelRoot.LoadSceneByName(name);
    }

    #region >>> SCENE MANAGER

    public void TryLoadNextLevel()
    {
        _levelRoot.TryLoadNextLevel();

        PlayNextLocationSound();
    }

    #endregion
    #region >>> SOUNDS

    public void PlayPaperSound()
    {
        if(_paperSound != null)
            _audioService.PlaySfx(_paperSound);
    }

    public void PlaySwitchOnLampSound()
    {
        if (_switchOnLampSound != null)
            _audioService.PlaySfx(_switchOnLampSound);
    }

    public void PlayTakeLampSound()
    {
        if (_takeLampSound != null)
            _audioService.PlaySfx(_takeLampSound);
    }

    public void PlayCarpetSound()
    {
        if (_carpetSound != null)
            _audioService.PlaySfx(_carpetSound);
    }

    public void PlayKeySound()
    {
        if (_keySound != null)
            _audioService.PlaySfx(_keySound);
    }

    public void PlayNextLocationSound()
    {
        if (_doorOpenSound != null)
            _audioService.PlaySfx(_doorOpenSound);
    }

    public void PlayTurnPhotoSound()
    {
        if (_turnPhotoSound != null)
            _audioService.PlaySfx(_turnPhotoSound);
    }

    public void PlayLockButtonSound()
    {
        if (_lockButtonSound != null)
            _audioService.PlaySfx(_lockButtonSound);
    }

    public void PlayLockOpendSound()
    {
        if (_lockOpendSound != null)
            _audioService.PlaySfx(_lockOpendSound);
    }

    #endregion

    public void Shutdown()
    {
        foreach (Interactable item in _interactables)
        {
            item.Dispose();
        }
    }
}
