using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace QuanLySanPham.ViewModel;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    public string username = "";
    [ObservableProperty]
    public string password = "";

    [RelayCommand]
    async Task Continue()
    {
        if (Username == "")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng nhập tên đăng nhập", "OK");
            return;
        }
        if (Password == "")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng nhập mật khẩu", "OK");
            return;
        }
        if (Username != "Phan Dinh Minh" || Password != "minh2005")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Tên đăng nhập hoặc mật khẩu không đúng", "OK");
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
