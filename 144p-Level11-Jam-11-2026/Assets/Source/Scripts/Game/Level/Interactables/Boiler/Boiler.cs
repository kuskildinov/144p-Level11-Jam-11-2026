using UnityEngine;

public class Boiler : Interactable
{
    [Header("Cameras")]
    [SerializeField] private GameObject _boilerCamera;

    [Header("Controller")]
    [SerializeField] private BoilerInspectionController _controller;

    [Header("comments")]
    [SerializeField] private bool _needComment = false;
    [SerializeField] private int _commentIndex;
    private bool _isPlayed;

    private RoomRoot _root;
    private bool _isInspecting;

    public bool IsInspecting => _isInspecting;

    public override void Initialize(RoomRoot root)
    {
        _root = root;
        _isInspecting = false;

        if (_boilerCamera != null)
            _boilerCamera.SetActive(false);

        if (_controller != null)
            _controller.IsActive = false;
    }

    public override void Interact(Player player)
    {
        if (!IsActive || _isInspecting)
            return;

        EnterInspection();

        if (_needComment && !_isPlayed)
        {
            _isPlayed = true;
            player.PlayVoiceByIndex(_commentIndex);
        }
    }

    private void EnterInspection()
    {
        _isInspecting = true;
        IsActive = false;

        _root.TryEnterInspectionView();

        if (_boilerCamera != null)
            _boilerCamera.SetActive(true);

        if (_controller != null)
            _controller.IsActive = true;
    }

    public void ExitInspection()
    {
        if (!_isInspecting)
            return;

        _isInspecting = false;
        IsActive = true;

        if (_controller != null)
            _controller.IsActive = false;

        if (_boilerCamera != null)
            _boilerCamera.SetActive(false);

        _root.TryExitInspectionView();
    }

    public override void Dispose()
    {
        if (_isInspecting)
            ExitInspection();

        if (_controller != null)
            _controller.IsActive = false;
    }
}
