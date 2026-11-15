using IcePiecesGames.Framework.Core.Lifecycle;
using IcePiecesGames.Framework.Core.SceneManagement;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuRoot : FrameworkRoot, IComposable, IRunnable, IShutdownable
{
    [SerializeField] private ScreenFade _fade;
    [SerializeField] private string _nextLevelSceneName;

    private SceneTransitionService _sceneTransitionService;

    public void Compose()
    {
        _fade.Initialize();
        _fade.SetAlpha(1f);
        _sceneTransitionService = Context.GetService<SceneTransitionService>();
    }

    public void Run()
    {
        _fade.FadeOut(3f);
    }

    public void Shutdown()
    {
        
    }

    public void TryLoadNextLevel()
    {
        _fade.FadeIn(3, () =>
        {
            LoadNextLevel(_nextLevelSceneName);
        });
    }

    private async Task LoadNextLevel(string name)
    {
        var result = await _sceneTransitionService.LoadSceneAsync(name);
        if (!result.Success)
        {
            Debug.LogException(result.Error);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
