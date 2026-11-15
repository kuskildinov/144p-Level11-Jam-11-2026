using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CodeLock : Interactable
{
    private const string AnimatorOpenParam = "Open";

    [SerializeField] private GameObject _lockCamera;
    [Header("Code")]
    [SerializeField] private string _correctCode = "81429";

    [Header("Sections")]
    [SerializeField] private DigitSection[] _sections = new DigitSection[5];

    [Header("Animation")]
    [SerializeField] private float _scrollDistance = 65f;
    [SerializeField] private float _scrollDuration = 0.18f;
    [SerializeField] private Ease _scrollEase = Ease.OutCubic;

    [Header("Success")]
    [SerializeField] private RoomDoor _door;
    [SerializeField] private Animator _animator;

    private readonly int[] _digits = new int[5];

    private RoomRoot _root;
    private bool _isUnlocked;
    private bool _isInspecting;

    public bool IsUnlocked => _isUnlocked;

    public string CurrentCode
    {
        get
        {
            string result = "";

            foreach (int digit in _digits)
                result += digit;

            return result;
        }
    }

    public override void Initialize(RoomRoot root)
    {
        _root = root;

        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        for (int i = 0; i < _sections.Length; i++)
        {
            int index = i;
            DigitSection section = _sections[i];

            section.Value = section.StartDigit;
            _digits[i] = section.Value;

            section.CurrentPosition =
                section.CurrentText.rectTransform.anchoredPosition;

            section.NextPosition =
                section.NextText.rectTransform.anchoredPosition;

            section.CurrentText.text = section.Value.ToString();
            section.NextText.text = "";

            section.NextText.gameObject.SetActive(false);

            section.UpButton.onClick.AddListener(
                () => ChangeDigit(index, 1)
            );

            section.DownButton.onClick.AddListener(
                () => ChangeDigit(index, -1)
            );
        }
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

        _lockCamera.gameObject.SetActive(true);
    }

    public void ExitInspection()
    {
        if (!_isInspecting)
            return;

        _isInspecting = false;
        IsActive = true;
        _root.TryExitInspectionView();

        _lockCamera.gameObject.SetActive(false);
    }

    private bool ValidateSetup()
    {
        if (_sections == null || _sections.Length != 5 ||
            string.IsNullOrEmpty(_correctCode) ||
            _correctCode.Length != 5)
        {
            Debug.LogError("CodeLock: requires 5 digits.", this);
            return false;
        }

        foreach (char c in _correctCode)
        {
            if (c < '0' || c > '9')
            {
                Debug.LogError("CodeLock: invalid code.", this);
                return false;
            }
        }

        foreach (DigitSection section in _sections)
        {
            if (section == null ||
                section.UpButton == null ||
                section.DownButton == null ||
                section.CurrentText == null ||
                section.NextText == null)
            {
                Debug.LogError("CodeLock: missing UI reference.", this);
                return false;
            }
        }

        return true;
    }

    public void ChangeDigit(int index, int direction)
    {
        if (_isUnlocked)
            return;

        DigitSection section = _sections[index];

        // Останавливаем предыдущую анимацию.
        section.Animation?.Kill();

        // Возвращаем визуальные элементы в исходное положение.
        ResetDigitVisual(section);

        _root.PlayLockButtonSound();

        int oldValue = section.Value;
        int newValue = (oldValue + direction + 10) % 10;

        section.Value = newValue;
        _digits[index] = newValue;

        RectTransform currentRect =
            section.CurrentText.rectTransform;

        RectTransform nextRect =
            section.NextText.rectTransform;

        // Верхняя кнопка: цифры идут снизу вверх.
        float offset = direction > 0
            ? _scrollDistance
            : -_scrollDistance;

        section.CurrentText.text = oldValue.ToString();
        section.NextText.text = newValue.ToString();

        currentRect.anchoredPosition = section.CurrentPosition;

        nextRect.anchoredPosition =
            section.NextPosition + Vector2.up * offset;

        section.NextText.gameObject.SetActive(true);

        section.Animation = DOTween.Sequence()
            .SetUpdate(true);

        section.Animation.Join(
            currentRect.DOAnchorPos(
                section.CurrentPosition + Vector2.down * offset,
                _scrollDuration
            ).SetEase(_scrollEase)
        );

        section.Animation.Join(
            nextRect.DOAnchorPos(
                section.NextPosition,
                _scrollDuration
            ).SetEase(_scrollEase)
        );

        section.Animation.OnComplete(() =>
        {
            ResetDigitVisual(section);
            section.CurrentText.text = section.Value.ToString();

            CheckCode();
        });
    }

    private void ResetDigitVisual(DigitSection section)
    {
        section.CurrentText.rectTransform.anchoredPosition =
            section.CurrentPosition;

        section.NextText.rectTransform.anchoredPosition =
            section.NextPosition;

        section.CurrentText.text = section.Value.ToString();
        section.NextText.gameObject.SetActive(false);
    }

    private void CheckCode()
    {
        if (_isUnlocked || CurrentCode != _correctCode)
            return;

        _isUnlocked = true;

        foreach (DigitSection section in _sections)
        {
            section.Animation?.Kill();

            ResetDigitVisual(section);

            section.UpButton.interactable = false;
            section.DownButton.interactable = false;
        }

        OnCodeCorrect();
    }

    private void OnCodeCorrect()
    {
        Debug.Log("CodeLock: Correct code!");

        _door.IsOpen = true;
        _animator.SetBool(AnimatorOpenParam, true);
        _root.PlayLockOpendSound();
    }

    public void ResetLock()
    {
        _isUnlocked = false;

        for (int i = 0; i < _sections.Length; i++)
        {
            DigitSection section = _sections[i];

            section.Animation?.Kill();

            section.Value = section.StartDigit;
            _digits[i] = section.Value;

            ResetDigitVisual(section);

            section.CurrentText.color = Color.white;
            section.CurrentText.rectTransform.localScale =
                Vector3.one;

            section.UpButton.interactable = true;
            section.DownButton.interactable = true;
        }
    }
    
    public override void Dispose()
    {       
        if (_sections == null)
            return;

        foreach (DigitSection section in _sections)
        {
            if (section == null)
                continue;

            section.Animation?.Kill();

            if (section.UpButton != null)
                section.UpButton.onClick.RemoveAllListeners();

            if (section.DownButton != null)
                section.DownButton.onClick.RemoveAllListeners();
        }
    }
}

[Serializable]
public class DigitSection
{
    public Button UpButton;
    public Button DownButton;

    public Text CurrentText;
    public Text NextText;

    [Range(0, 9)]
    public int StartDigit;

    [NonSerialized] public Sequence Animation;
    [NonSerialized] public int Value;
    [NonSerialized] public Vector2 CurrentPosition;
    [NonSerialized] public Vector2 NextPosition;
}