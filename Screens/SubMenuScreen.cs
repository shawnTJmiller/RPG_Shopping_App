using GurpsShoppingApp.Models;

namespace GurpsShoppingApp.Screens;

public sealed class SubMenuScreen : MenuScreenBase
{
    private readonly IReadOnlyList<MenuOption> _options;

    public SubMenuScreen(string title, IReadOnlyList<MenuOption>? options = null)
    {
        Title = title;
        _options = options ?? new List<MenuOption>
        {
            new("BACK TO MAIN MENU", navigator => navigator.NavigateTo(new HomeMenuScreen()))
        };
    }

    protected override IReadOnlyList<MenuOption> Options => _options;

    protected override string Title { get; }
}
