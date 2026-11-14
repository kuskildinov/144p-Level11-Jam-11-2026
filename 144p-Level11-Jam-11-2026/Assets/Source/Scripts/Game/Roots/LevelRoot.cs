using IcePiecesGames.Framework.Core.Lifecycle;
using IcePiecesGames.Framework.Core.SceneManagement;
using System.Threading.Tasks;
using UnityEngine;

[DependsOn(typeof(PlayerRoot))]
public class LevelRoot : FrameworkRoot, IComposable, IRunnable
{
    [SerializeField] private string _nextLevelSceneName;
    [SerializeField] private ScreenFade _screenFade;

    private SceneTransitionService _sceneTransitionService;
    private PlayerRoot _playerRoot;

    public void Compose()
    {
        _playerRoot = Context.GetRoot<PlayerRoot>();

        _sceneTransitionService = Context.GetService<SceneTransitionService>();

        _screenFade.Initialize();
        _screenFade.SetAlpha(1f);
    }

    public void Run()
    {
        _screenFade.FadeOut(2, () =>
        {
            _playerRoot.TogglePlayerActivation(true);
        });
    }

    public void TryLoadNextLevel()
    {
        // Play open door sound

        _playerRoot.TogglePlayerActivation(false);
        _screenFade.FadeIn(3, () =>
        {
            LoadNextLevel();
        });
    }

    private async Task LoadNextLevel()
    {
        var result = await _sceneTransitionService.LoadSceneAsync(_nextLevelSceneName);
        if(!result.Success)
        {
            Debug.LogException(result.Error);
        }
    }
}
