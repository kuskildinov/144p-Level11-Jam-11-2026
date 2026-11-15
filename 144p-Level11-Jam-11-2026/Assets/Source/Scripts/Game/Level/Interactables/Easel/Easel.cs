
using System.Collections.Generic;
using IcePiecesGames.Framework.Core.Events;
using UnityEngine;
using UnityEngine.Events;

public class Easel : Interactable
{
    [Header("Camera")]
    [SerializeField] private GameObject _easelCamera;

    [Header("Puzzle Pieces")]
    [SerializeField] private EaselPiece[] _pieces;

    [Header("Puzzle Surface")]
    [SerializeField] private Transform _easelSurface;

    [Header("Controller")]
    [SerializeField] private EaselDragController _controller;

    [Header("Scatter Settings")]
    [SerializeField]
    private Vector2 _scatterSize =
        new Vector2(2f, 1.5f);

    [SerializeField] private float _minimumPieceDistance = 0.25f;
    [SerializeField] private float _minimumDistanceFromCorrect = 0.3f;
    [SerializeField] private int _scatterAttempts = 100;

    [Header("Completion")]
    [SerializeField] private UnityEvent _onPuzzleCompleted;

    private RoomRoot _root;
    private IEventBus _eventBus;

    private readonly SubscriptionBag _subscriptionBag =
        new SubscriptionBag();

    private bool _isInspecting;
    private bool _isCompleted;

    public bool IsCompleted => _isCompleted;

    public override void Initialize(RoomRoot root)
    {
        _root = root;
        _eventBus = root.EventBus;

        _isInspecting = false;
        _isCompleted = false;

        IsActive = false;

        if (_easelCamera != null)
            _easelCamera.SetActive(false);

        if (_controller != null)
            _controller.IsActive = false;

        // 1. Запоминаем правильные позиции.
        foreach (EaselPiece piece in _pieces)
        {
            if (piece == null)
                continue;

            piece.Initialize();
        }

        // 2. Разбрасываем фрагменты.
        ScatterPieces();

        // 3. Скрываем до подбора на уровне.
        foreach (EaselPiece piece in _pieces)
        {
            if (piece != null)
                piece.gameObject.SetActive(false);
        }

        _subscriptionBag.Clear();

        _subscriptionBag.Subscribe<ItemPickUpEvent>(
            _eventBus,
            OnItemPickedUp
        );
    }

    public override void Interact(Player player)
    {
        if (!IsActive || _isInspecting)
            return;

        EnterInspection();
    }

    private void EnterInspection()
    {
        _isInspecting = true;
        IsActive = false;

        _root.TryEnterInspectionView();

        if (_easelCamera != null)
            _easelCamera.SetActive(true);

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

        if (_easelCamera != null)
            _easelCamera.SetActive(false);

        _root.TryExitInspectionView();
    }

    #region >>> PUZZLE

    private void ScatterPieces()
    {
        if (_easelSurface == null)
        {
            Debug.LogError(
                "Easel: EaselSurface is not assigned!",
                this
            );
            return;
        }

        List<Vector2> usedPositions = new List<Vector2>();

        foreach (EaselPiece piece in _pieces)
        {
            if (piece == null)
                continue;

            Vector3 correctLocal =
                _easelSurface.InverseTransformPoint(
                    piece.CorrectPosition
                );

            Vector2 correctXY = new Vector2(
                correctLocal.x,
                correctLocal.y
            );

            Vector2 chosenPosition = Vector2.zero;
            float bestScore = float.NegativeInfinity;

            for (int attempt = 0; attempt < _scatterAttempts; attempt++)
            {
                Vector2 candidate = new Vector2(
                    Random.Range(
                        -_scatterSize.x * 0.5f,
                        _scatterSize.x * 0.5f
                    ),
                    Random.Range(
                        -_scatterSize.y * 0.5f,
                        _scatterSize.y * 0.5f
                    )
                );

                float distanceFromCorrect =
                    Vector2.Distance(candidate, correctXY);

                float nearestPieceDistance =
                    float.MaxValue;

                foreach (Vector2 usedPosition in usedPositions)
                {
                    nearestPieceDistance = Mathf.Min(
                        nearestPieceDistance,
                        Vector2.Distance(
                            candidate,
                            usedPosition
                        )
                    );
                }

                bool valid =
                    distanceFromCorrect >= _minimumDistanceFromCorrect &&
                    nearestPieceDistance >= _minimumPieceDistance;

                if (valid)
                {
                    chosenPosition = candidate;
                    break;
                }

                float score = Mathf.Min(
                    distanceFromCorrect -
                    _minimumDistanceFromCorrect,
                    nearestPieceDistance -
                    _minimumPieceDistance
                );

                if (score > bestScore)
                {
                    bestScore = score;
                    chosenPosition = candidate;
                }
            }

            usedPositions.Add(chosenPosition);

            Vector3 scatteredLocal = new Vector3(
                chosenPosition.x,
                chosenPosition.y,
                correctLocal.z
            );

            piece.SetScatteredPosition(
                _easelSurface.TransformPoint(scatteredLocal)
            );
        }
    }

    public void OnPiecePlaced(EaselPiece piece)
    {
        if (_isCompleted || piece == null)
            return;

        foreach (EaselPiece current in _pieces)
        {
            if (current == null)
                continue;

            if (!current.gameObject.activeSelf || !current.IsPlaced)
                return;
        }

        _isCompleted = true;

        Debug.Log("Easel: Puzzle completed!");

        _onPuzzleCompleted?.Invoke();
    }

    #endregion

    #region >>> EVENTS

    private void OnItemPickedUp(ItemPickUpEvent eventData)
    {
        if (eventData.PickupItem == null)
            return;

        string itemId = eventData.PickupItem.ItemId;

        foreach (EaselPiece piece in _pieces)
        {
            if (piece == null || piece.ItemId != itemId)
                continue;

            if (!piece.gameObject.activeSelf)
            {
                piece.gameObject.SetActive(true);
                IsActive = true;

                Debug.Log(
                    $"Easel: Piece collected: {itemId}"
                );
            }

            return;
        }
    }

    #endregion

    public override void Dispose()
    {
        if (_isInspecting)
            ExitInspection();

        _subscriptionBag.Clear();

        if (_controller != null)
            _controller.IsActive = false;
    }
}