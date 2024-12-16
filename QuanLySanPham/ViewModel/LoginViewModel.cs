using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace QuanLySanPham.ViewModel;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string username = "";
    [ObservableProperty]
    private string password = "";

    [ObservableProperty]
    private string sdt = "";
    [ObservableProperty]
    private DateTime dob = DateTime.Now;

    [ObservableProperty]
    bool isInvisible = true;
    [ObservableProperty]
    bool isNotInvisible = false;

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

    [RelayCommand]
    void Forgot()
    {
        IsInvisible = false;
        IsNotInvisible = true;
    }

    [RelayCommand]
    void Cancel()
    {
        IsInvisible = true;
        IsNotInvisible = false;
    }

    [RelayCommand]
    async Task Continue2()
    {
        if (Sdt == "")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng nhập số điện thoại", "OK");
            return;
        }
        if (Sdt != "0865832440")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Sđt hoặc ngày sinh sai", "OK");
            return;
        }

        int day = Dob.Day;
        int month = Dob.Month;
        int year = Dob.Year;
        if (day != 21 || month != 2 || year != 2005)
        { 
            await Shell.Current.DisplayAlert("Thông báo", "Sđt hoặc ngày sinh sai", "OK");
            return;
        }

        await Shell.Current.DisplayAlert("Thông báo", "Tên đăng nhập của bạn là: Phan Dinh Minh\nMật khẩu của bạn là: minh2005", "OK");

        IsInvisible = true;
        IsNotInvisible = false;
    }
}
