using UnityEngine;

public class KeyCarpet : Interactable
{
    [Header("Visual")]
    [SerializeField] private GameObject _normalVisual;
    [SerializeField] private GameObject _opendVisual;
    [Header("Key")]
    [SerializeField] private Key _key;

    public override void Initialize(RoomRoot root)
    {
        _key.Initialize(root);
        _key.IsActive = false;
        IsActive = false;

        _normalVisual.gameObject.SetActive(true);
        _opendVisual.gameObject.SetActive(false);
    }

    public override void Interact(Player player)
    {
        OpenCarpet();
    }

    private void OpenCarpet()
    {
        _normalVisual.gameObject.SetActive(false);
        _opendVisual.gameObject.SetActive(true);
        _key.gameObject.SetActive(true);
        _key.IsActive = true;
        IsActive = false;
    }

    public override void Dispose()
    {

    }
}
