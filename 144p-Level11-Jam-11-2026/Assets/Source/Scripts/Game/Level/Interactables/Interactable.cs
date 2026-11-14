using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour, IDisposable
{
    [Header("Interaction")]
    [SerializeField] protected string _interactionText;

    [Header("Visual")]
    [SerializeField] private bool _needOutline;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _outlineMaterial;

    private Material[] _defaultMaterials;
    private Material[] _outlineMaterials;

    private bool _isOutlined;

    public bool IsActive { get; set; } = true;
    public string InteractionText => _interactionText;

    protected virtual void Awake()
    {
        InitializeOutline();
    }

    private void InitializeOutline()
    {
        if (!_needOutline)
            return;

        if (_meshRenderer == null || _outlineMaterial == null)
            return;

        _defaultMaterials = _meshRenderer.sharedMaterials;

        _outlineMaterials =
            new Material[_defaultMaterials.Length + 1];

        for (int i = 0; i < _defaultMaterials.Length; i++)
        {
            _outlineMaterials[i] = _defaultMaterials[i];
        }

        _outlineMaterials[_outlineMaterials.Length - 1] =
            _outlineMaterial;
    }

    public void ToggleOutline(bool value)
    {
        if (!_needOutline)
            return;

        if (_meshRenderer == null ||
            _outlineMaterial == null ||
            _defaultMaterials == null)
            return;

        if (_isOutlined == value)
            return;

        _isOutlined = value;

        _meshRenderer.sharedMaterials = value
            ? _outlineMaterials
            : _defaultMaterials;
    }

    protected virtual void OnDisable()
    {
        ToggleOutline(false);
    }

    public abstract void Initialize(RoomRoot root);
    public abstract void Interact(Player player);
    public abstract void Dispose();
}
