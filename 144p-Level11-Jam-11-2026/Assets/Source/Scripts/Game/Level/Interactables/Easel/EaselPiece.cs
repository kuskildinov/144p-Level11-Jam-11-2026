using DG.Tweening;
using UnityEngine;

public class EaselPiece : MonoBehaviour
{
    [Header("Identification")]
    [SerializeField] private string _itemId;

    [Header("Snap Animation")]
    [SerializeField] private float _snapDuration = 0.25f;
    [SerializeField] private Ease _snapEase = Ease.OutCubic;

    private Vector3 _correctPosition;
    private Quaternion _correctRotation;

    private Tween _moveTween;
    private Tween _rotateTween;

    private bool _isPlaced;

    public string ItemId => _itemId;
    public bool IsPlaced => _isPlaced;
    public Vector3 CorrectPosition => _correctPosition;

    public void Initialize()
    {
        _moveTween?.Kill();
        _rotateTween?.Kill();

        // Запоминаем положение фрагмента,
        // в котором он находится в редакторе.
        _correctPosition = transform.position;
        _correctRotation = transform.rotation;

        _isPlaced = false;
    }

    public void SetScatteredPosition(Vector3 position)
    {
        if (_isPlaced)
            return;

        transform.position = position;
    }

    public float DistanceToCorrectPosition()
    {
        return Vector3.Distance(
            transform.position,
            _correctPosition
        );
    }

    public void Place()
    {
        if (_isPlaced)
            return;

        _isPlaced = true;

        _moveTween?.Kill();
        _rotateTween?.Kill();

        _moveTween = transform
            .DOMove(_correctPosition, _snapDuration)
            .SetEase(_snapEase);

        _rotateTween = transform
            .DORotateQuaternion(
                _correctRotation,
                _snapDuration
            )
            .SetEase(_snapEase);
    }

    private void OnDestroy()
    {
        _moveTween?.Kill();
        _rotateTween?.Kill();
    }
}