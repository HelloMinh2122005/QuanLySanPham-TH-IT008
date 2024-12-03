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
        if (Username == "")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng nhập tên đăng nhập", "OK");
            return;
        }
        await Shell.Current.GoToAsync(nameof(MainPage), new Dictionary<string, object>
        {
            {"UserName", Username }
        });
    }


    [RelayCommand]
    async Task OnEntryCompleted()
    {
        await Continue();
    }
}
