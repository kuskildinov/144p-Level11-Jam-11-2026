
using System;
using DG.Tweening;
using UnityEngine;

public class BoardItem : MonoBehaviour
{
    [Header("Identification")]
    [SerializeField] private string _itemId;

    [Header("Visual")]
    [SerializeField] private Transform _visual;
    [SerializeField] private MeshRenderer[] _renderers;
    [SerializeField] private Material _outlineMaterial;

    [Header("Rotation")]
    [SerializeField] private float _rotationDuration = 0.5f;
    [SerializeField] private Ease _rotationEase = Ease.InOutCubic;

    private Material[][] _originalMaterials;
    private Material[][] _highlightMaterials;

    private Quaternion _frontRotation;
    private Tween _rotationTween;

    private bool _isFlipped;
    private bool _isHighlighted;

    public string ItemId => _itemId;

    public bool IsRotating =>
        _rotationTween != null &&
        _rotationTween.IsActive() &&
        _rotationTween.IsPlaying();

    private RoomRoot _root;

    public void Initialize(RoomRoot root)
    {
        _root = root;

        if (_visual == null)
            _visual = transform;

        _frontRotation = _visual.localRotation;

        if (_renderers == null || _renderers.Length == 0)
            _renderers = GetComponentsInChildren<MeshRenderer>(true);

        _originalMaterials = new Material[_renderers.Length][];
        _highlightMaterials = new Material[_renderers.Length][];

        for (int i = 0; i < _renderers.Length; i++)
        {
            MeshRenderer renderer = _renderers[i];

            if (renderer == null)
                continue;

            Material[] originals = renderer.sharedMaterials;
            _originalMaterials[i] = originals;

            if (_outlineMaterial == null)
                continue;

            Material[] highlighted =
                new Material[originals.Length + 1];

            Array.Copy(originals, highlighted, originals.Length);

            highlighted[highlighted.Length - 1] =
                _outlineMaterial;

            _highlightMaterials[i] = highlighted;
        }
    }

    public void SetHighlight(bool value)
    {
        if (_isHighlighted == value)
            return;

        _isHighlighted = value;

        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i] == null ||
                _originalMaterials[i] == null)
                continue;

            if (value && _highlightMaterials[i] != null)
                _renderers[i].sharedMaterials = _highlightMaterials[i];
            else
                _renderers[i].sharedMaterials = _originalMaterials[i];
        }
    }

    public void Flip()
    {
        if (IsRotating)
            return;

        _root.PlayTurnPhotoSound();

        _isFlipped = !_isFlipped;

        Quaternion targetRotation = _isFlipped
            ? _frontRotation * Quaternion.Euler(0f, 180f, 0f)
            : _frontRotation;

        _rotationTween = _visual
            .DOLocalRotateQuaternion(targetRotation, _rotationDuration)
            .SetEase(_rotationEase);
    }

    private void OnDisable()
    {
        _rotationTween?.Kill();
        _rotationTween = null;

        SetHighlight(false);
    }

    private void OnDestroy()
    {
        _rotationTween?.Kill();

        if (_renderers == null || _originalMaterials == null)
            return;

        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i] != null &&
                _originalMaterials[i] != null)
            {
                _renderers[i].sharedMaterials =
                    _originalMaterials[i];
            }
        }
    }
}