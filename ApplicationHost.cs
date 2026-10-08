using GurpsShoppingApp.Interfaces;
using GurpsShoppingApp.Models;
using GurpsShoppingApp.Screens;
using GurpsShoppingApp.Services;

namespace GurpsShoppingApp;

public sealed class ApplicationHost : IMenuNavigator
{
    private readonly AzureSqlMenuRepository _menuRepository = new();
    private IMenuScreen? _currentScreen;
    private bool _isRunning = true;

    public void Run()
    {
        _isRunning = true;

        var homeOptions = LoadHomeOptionsAsync().GetAwaiter().GetResult();
        NavigateTo(new HomeMenuScreen(homeOptions));

        while (_isRunning && _currentScreen is not null)
        {
            _currentScreen.Show(this);
        }
    }

    public void NavigateTo(IMenuScreen screen)
    {
        _currentScreen = screen;
    }

    public void Exit()
    {
        _isRunning = false;
    }

    private async Task<IReadOnlyList<MenuOption>> LoadHomeOptionsAsync()
    {
        var menuItems = await _menuRepository.LoadMenuAsync("HOME");
        if (menuItems.Count > 0)
        {
            return menuItems
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.Id)
                .Select(CreateMenuOption)
                .ToList();
        }

        return CreateFallbackHomeOptions();
    }

    private static MenuOption CreateMenuOption(DatabaseMenuItem item)
    {
        return new MenuOption(item.Label, navigator =>
        {
            if (item.IsExit)
            {
                navigator.Exit();
                return;
            }

            navigator.NavigateTo(new SubMenuScreen(item.TargetScreen ?? item.Label));
        });
    }

    private static IReadOnlyList<MenuOption> CreateFallbackHomeOptions()
    {
        return new List<MenuOption>
        {
            new("BUY", navigator => navigator.NavigateTo(new SubMenuScreen("BUY MENU"))),
            new("SELL", navigator => navigator.NavigateTo(new SubMenuScreen("SELL MENU"))),
            new("TRADE", navigator => navigator.NavigateTo(new SubMenuScreen("TRADE MENU"))),
            new("ADD ITEMS TO LIST", navigator => navigator.NavigateTo(new SubMenuScreen("ADD ITEMS TO LIST MENU"))),
            new("CLOSE APP", navigator => navigator.Exit())
        };
    }
}
