using IcePiecesGames.Framework.Audio;
using IcePiecesGames.Framework.Core.Lifecycle;
using IcePiecesGames.Framework.Core.SceneManagement;
using System.Threading.Tasks;
using UnityEngine;

[DependsOn(typeof(PlayerRoot))]
public class LevelRoot : FrameworkRoot, IComposable, IRunnable
{
    [SerializeField] private string _nextLevelSceneName;
    [SerializeField] private ScreenFade _screenFade;
    [Header("Sounds")]
    [SerializeField] private AudioClip _backgroundSound;
    [SerializeField] private AudioClip _lightningSound;

    private SceneTransitionService _sceneTransitionService;
    private IAudioService _audioService;

    private PlayerRoot _playerRoot;

    public void Compose()
    {
        _playerRoot = Context.GetRoot<PlayerRoot>();

        _sceneTransitionService = Context.GetService<SceneTransitionService>();
        _audioService = Context.GetService<IAudioService>();

        _screenFade.Initialize();
        _screenFade.SetAlpha(1f);
    }

    public void Run()
    {
        _screenFade.FadeOut(2, () =>
        {
            _playerRoot.TogglePlayerActivation(true);
        });

        PlayBackgroundSound();
    }

    #region >>> SCENE MANAGMENT
    public void TryLoadNextLevel()
    {
        // Play open door sound

        _playerRoot.TogglePlayerActivation(false);
        _screenFade.FadeIn(3, () =>
        {
            LoadNextLevel(_nextLevelSceneName);
        });
    }

    public void LoadSceneByName(string name)
    {
        _playerRoot.TogglePlayerActivation(false);
        _screenFade.FadeIn(4, () =>
        {
            LoadNextLevel(name);
        });
    }

    private async Task LoadNextLevel(string name)
    {
        var result = await _sceneTransitionService.LoadSceneAsync(name);
        if(!result.Success)
        {
            Debug.LogException(result.Error);
        }
    }
    #endregion
    #region >>> SOUNDS

    private void PlayBackgroundSound()
    {
        if(_backgroundSound != null)
        _audioService.PlayMusic(_backgroundSound);
        if (_lightningSound != null)
            _audioService.PlaySfx(_lightningSound);
    }

    #endregion
}
