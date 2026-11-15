using Cinemachine;
using IcePiecesGames.Framework.Input;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 3.5f;
    [SerializeField] private float _gravity = -9.81f;
    [Header("Look")]
    [SerializeField] private float _mouseSensitivity = 0.12f;
    [SerializeField] private float _maxLookAngle = 80f;

    private Player _root;
    private CharacterController _controller => _root.Controller;
    private CinemachineVirtualCamera _playerCamera => _root.PlayerCam;

    private IInputService _inputService;

    private float _verticalVelocity;
    private float _cameraPitch;

    public void Initialize(Player root, IInputService inputService)
    {
        _root = root;
        _inputService = inputService;
    }

    private void Update()
    {
        if (!_root.IsActive)
            return;

        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        Vector2 input = _inputService.Move;       
        Vector3 direction = _root.transform.right * input.x + _root.transform.forward * input.y;

        direction = Vector3.ClampMagnitude(direction, 1f);

        if (_controller.isGrounded && _verticalVelocity < 0f)
            _verticalVelocity = -2f;

        _verticalVelocity += _gravity * Time.deltaTime;
        Vector3 velocity = direction * _moveSpeed + Vector3.up * _verticalVelocity;
        _controller.Move(velocity * Time.deltaTime);

        bool isMoving =
       direction.sqrMagnitude > 0.01f &&
       _controller.isGrounded;

        _root.PlayStepSound(isMoving);
    }

    private void HandleLook()
    {
        Vector2 delta = _inputService.Look * _mouseSensitivity;
        _root.transform.Rotate(Vector3.up * delta.x);
        _cameraPitch = Mathf.Clamp(_cameraPitch - delta.y, -_maxLookAngle, _maxLookAngle);

        _playerCamera.transform.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
    }
}
