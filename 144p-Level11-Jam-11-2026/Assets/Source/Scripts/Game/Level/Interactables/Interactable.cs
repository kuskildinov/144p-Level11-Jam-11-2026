using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour, IDisposable
{
    [SerializeField] protected string _interactionText;

    public bool IsActive { get; set; } = true;
    public string InteractionText => _interactionText;


    public abstract void Initialize(RoomRoot root);
    public abstract void Interact(Player player);
    public abstract void Dispose();
}
