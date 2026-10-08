using GurpsShoppingApp.Interfaces;

namespace GurpsShoppingApp.Models;

public sealed class MenuOption
{
    public MenuOption(string label, Action<IMenuNavigator> onSelect)
    {
        Label = label;
        OnSelect = onSelect;
    }

    public string Label { get; }

    public Action<IMenuNavigator> OnSelect { get; }
}
