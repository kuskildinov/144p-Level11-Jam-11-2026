public interface IInteractable
{
    public bool IsActive { get; set; }
    public string InteractionText { get; set; }
    public void Interact(Player player);
}
