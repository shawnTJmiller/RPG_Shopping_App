using GurpsShoppingApp.Interfaces;
using GurpsShoppingApp.Models;

namespace GurpsShoppingApp.Screens;

public abstract class MenuScreenBase : IMenuScreen
{
    private int _selectedIndex;

    protected abstract IReadOnlyList<MenuOption> Options { get; }

    protected virtual string Title => string.Empty;

    public void Show(IMenuNavigator navigator)
    {
        while (true)
        {
            Console.Clear();
            Render();

            var key = Console.ReadKey(true);
            if (HandleInput(key, navigator))
            {
                return;
            }
        }
    }

    private void Render()
    {
        if (!string.IsNullOrWhiteSpace(Title))
        {
            Console.WriteLine(Title);
            Console.WriteLine();
        }

        for (var i = 0; i < Options.Count; i++)
        {
            var prefix = i == _selectedIndex ? "*" : " ";
            Console.WriteLine($"{prefix} {i + 1}. {Options[i].Label}");
        }
    }

    protected virtual bool HandleInput(ConsoleKeyInfo key, IMenuNavigator navigator)
    {
        if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.RightArrow)
        {
            MoveSelection(1);
            return false;
        }

        if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.LeftArrow)
        {
            MoveSelection(-1);
            return false;
        }

        if (TryParseSelection(key, out var selectedIndex))
        {
            ExecuteSelection(selectedIndex, navigator);
            return true;
        }

        if (key.Key == ConsoleKey.Enter)
        {
            ExecuteSelection(_selectedIndex, navigator);
            return true;
        }

        return false;
    }

    private void MoveSelection(int delta)
    {
        var nextIndex = _selectedIndex + delta;

        if (nextIndex < 0)
        {
            nextIndex = Options.Count - 1;
        }
        else if (nextIndex >= Options.Count)
        {
            nextIndex = 0;
        }

        _selectedIndex = nextIndex;
    }

    private static bool TryParseSelection(ConsoleKeyInfo key, out int selectedIndex)
    {
        selectedIndex = -1;

        if (char.IsDigit(key.KeyChar))
        {
            var parsed = key.KeyChar - '0';
            if (parsed > 0)
            {
                selectedIndex = parsed - 1;
                return true;
            }
        }

        return false;
    }

    private void ExecuteSelection(int selectedIndex, IMenuNavigator navigator)
    {
        if (selectedIndex < 0 || selectedIndex >= Options.Count)
        {
            return;
        }

        Options[selectedIndex].OnSelect(navigator);
    }
}
