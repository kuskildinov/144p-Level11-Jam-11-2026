using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class EaselFallAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _fallPivot;
    [SerializeField] private Easel _easel;

    [Header("Impact")]
    [SerializeField] private float _impactAngle = -8f;
    [SerializeField] private float _impactDuration = 0.12f;

    [Header("Fall")]
    [SerializeField] private float _fallAngle = 85f;
    [SerializeField] private float _fallDuration = 0.85f;
    [SerializeField] private Ease _fallEase = Ease.InQuad;

    [Header("Landing")]
    [SerializeField] private float _landingShakeStrength = 0.12f;
    [SerializeField] private float _landingShakeDuration = 0.2f;

    [Header("Events")]
    [SerializeField] private UnityEvent _onImpact;
    [SerializeField] private UnityEvent _onLanding;
    [SerializeField] private UnityEvent _onFallCompleted;

    private Sequence _sequence;
    private Quaternion _initialRotation;

    private bool _isFalling;
    private bool _hasFallen;

    public bool IsFalling => _isFalling;
    public bool HasFallen => _hasFallen;

    private void Awake()
    {
        if (_fallPivot == null)
            _fallPivot = transform;

        _initialRotation = _fallPivot.localRotation;
    }   

    public void PlayFall()
    {
        if (_isFalling || _hasFallen)
            return;

        _isFalling = true;

        _sequence?.Kill();

        Quaternion impactRotation =
            _initialRotation *
            Quaternion.Euler(0f, 0f, _impactAngle);

        Quaternion finalRotation =
            _initialRotation *
            Quaternion.Euler(0f, 0f, _fallAngle);

        _sequence = DOTween.Sequence();

        // Резкий удар.
        _sequence.AppendCallback(() =>
        {
            _onImpact?.Invoke();
        });

        _sequence.Append(
            _fallPivot.DOLocalRotateQuaternion(
                impactRotation,
                _impactDuration
            ).SetEase(Ease.OutQuad)
        );

        // Основное падение.
        _sequence.Append(
            _fallPivot.DOLocalRotateQuaternion(
                finalRotation,
                _fallDuration
            ).SetEase(_fallEase)
        );

        // Удар о пол.
        _sequence.AppendCallback(() =>
        {
            _onLanding?.Invoke();
        });

        // Небольшая вибрация после падения.
        _sequence.Append(
            _fallPivot.DOShakePosition(
                _landingShakeDuration,
                _landingShakeStrength,
                12,
                45f,
                false,
                true
            )
        );

        _sequence.OnComplete(() =>
        {
            _isFalling = false;
            _hasFallen = true;

            _easel.ExitInspection();

            _onFallCompleted?.Invoke();
        });
    }

    private void OnDestroy()
    {
        _sequence?.Kill();
    }
}