using IcePiecesGames.Framework.Core.Lifecycle;
using IcePiecesGames.Framework.Core.SceneManagement;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class FinalRoot : FrameworkRoot, IComposable, IRunnable
{
    [SerializeField] private ScreenFade _fade;
    [SerializeField] private FinalPanel _finalPanel;
    [SerializeField] private AudioClip _sound_1;
    [SerializeField] private string _nextSceneName;

    private SceneTransitionService _sceneTransitionService;

    public void Compose()
    {
        _sceneTransitionService = Context.GetService<SceneTransitionService>();

        _fade.Initialize();
        _fade.SetAlpha(1f);
    }

    public void Run()
    {
        // Player. _sound_1
        _fade.FadeOut(3, () =>
        {
            StartCoroutine(EndingRoutine());
        });
    }

    private IEnumerator EndingRoutine()
    {
        yield return new WaitForSecondsRealtime(9f);
        LoadSceneByName(_nextSceneName);
    }

    public void LoadSceneByName(string name)
    {
        _fade.FadeIn(3, () =>
        {
            LoadSceneAsync(name);
        });
    }

    private async Task LoadSceneAsync(string name)
    {
        var result = await _sceneTransitionService.LoadSceneAsync(name);
        if (!result.Success)
        {
            Debug.LogException(result.Error);
        }
    }
}
