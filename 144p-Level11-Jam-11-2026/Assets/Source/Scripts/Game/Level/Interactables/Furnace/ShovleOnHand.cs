using UnityEngine;

public class ShovleOnHand : MonoBehaviour
{
    [SerializeField] private GameObject _coal;

    public bool IsEmpty { get; set; } = true;

    public void AddCoal()
    {
        _coal.gameObject.SetActive(true);
        IsEmpty = false;
    }

    public void RemoveCoal()
    {
        _coal.gameObject.SetActive(false);
        IsEmpty = true;
    }
}
