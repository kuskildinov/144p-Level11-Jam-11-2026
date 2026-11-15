public interface IInspectable
{
    bool NeedAction { get; }
    string ActionText { get; }

    void ExecuteInspectionAction();
}