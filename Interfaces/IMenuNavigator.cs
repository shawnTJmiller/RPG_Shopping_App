using GurpsShoppingApp.Interfaces;

namespace GurpsShoppingApp.Interfaces;

public interface IMenuNavigator
{
    void NavigateTo(IMenuScreen screen);

    void Exit();
}
