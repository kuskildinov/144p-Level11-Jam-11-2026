using UnityEngine;

public class InspectionItemSettings : MonoBehaviour
{
    [Header("Preview Transform")]
    [SerializeField] private Vector3 _positionOffset = Vector3.zero;
    [SerializeField] private Vector3 _rotationOffset = Vector3.zero;
    [SerializeField] private Vector3 _scaleMultiplier = Vector3.one;

    [Header("Rotation")]
    [SerializeField] private Vector3 _rotationAxis = Vector3.up;

    public Vector3 PositionOffset => _positionOffset;
    public Quaternion RotationOffset => Quaternion.Euler(_rotationOffset);
    public Vector3 ScaleMultiplier => _scaleMultiplier;
    public Vector3 RotationAxis => _rotationAxis.sqrMagnitude > 0.0001f ? _rotationAxis.normalized : Vector3.up;
}