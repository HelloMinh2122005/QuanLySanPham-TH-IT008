using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLySanPham.View;

namespace QuanLySanPham.ViewModel;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string userName;

    public MainPageViewModel()
    {
        userName = "";
    }

    [RelayCommand]
    async Task Enter()
    {
        if (userName == "")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng nhập tên", "OK");
            return;
        }
        await Shell.Current.GoToAsync(nameof(DanhSachSP), new Dictionary<string, object>
        {
            {"userName", userName }
        });
    }

    [RelayCommand]
    async Task Create()
    {
        await Task.CompletedTask;
    }
}