
using UnityEngine;

public class CodeLockButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CodeLock _codeLock;

    [Header("Button Settings")]
    [SerializeField, Range(0, 4)] private int _digitIndex;
    [SerializeField] private bool _increase = true;

    public void Press()
    {
        if (_codeLock == null || _codeLock.IsUnlocked)
            return;

        int direction = _increase ? 1 : -1;

        _codeLock.ChangeDigit(_digitIndex, direction);
    }
}