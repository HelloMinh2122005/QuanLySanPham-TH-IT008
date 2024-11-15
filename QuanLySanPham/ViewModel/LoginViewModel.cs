using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace QuanLySanPham.ViewModel;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    public string username;

    public LoginViewModel()
    {
        Username = "";
    }

    [RelayCommand]
    async Task Continue()
    {
        await Shell.Current.GoToAsync(nameof(MainPage));
    }
}
