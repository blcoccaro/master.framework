namespace master.framework.interfaces
{
    public interface IDropDownListResult
    {
        string Value { get; set; }
        string DisplayText { get; set; }
        int Order { get; set; }
    }
}
