using GurpsShoppingApp.Interfaces;
using GurpsShoppingApp.Models;

namespace GurpsShoppingApp.Screens;

public sealed class HomeMenuScreen : MenuScreenBase
{
    private readonly IReadOnlyList<MenuOption> _options;

    public HomeMenuScreen(IReadOnlyList<MenuOption>? options = null)
    {
        _options = options ?? new List<MenuOption>
        {
            new("BUY", navigator => navigator.NavigateTo(new SubMenuScreen("BUY MENU"))),
            new("SELL", navigator => navigator.NavigateTo(new SubMenuScreen("SELL MENU"))),
            new("TRADE", navigator => navigator.NavigateTo(new SubMenuScreen("TRADE MENU"))),
            new("ADD ITEMS TO LIST", navigator => navigator.NavigateTo(new SubMenuScreen("ADD ITEMS TO LIST MENU"))),
            new("CLOSE APP", navigator => navigator.Exit())
        };
    }

    protected override IReadOnlyList<MenuOption> Options => _options;

    protected override string Title => "HOME MENU";
}
