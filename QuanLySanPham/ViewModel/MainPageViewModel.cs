using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLySanPham.View;
using QuanLySanPham.Model;
using QuanLySanPham.Services;

namespace QuanLySanPham.ViewModel;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string dataPath = "Hiện chưa chọn tập tin nào.";

    private readonly FileService fileService = new FileService();

    [RelayCommand]
    async Task NhapFile()
    {
        DataPath = await fileService.Import();
    }

    [RelayCommand]
    async Task TaoFileMoi()
    {
        await fileService.CreateNewFile();
    }

    [RelayCommand]
    async Task BoQua()
    {
        await Shell.Current.GoToAsync(nameof(DanhSachSP), new Dictionary<string, object>
        {
            {"DsSanPham", new SanPham() }
        });
    }

    [RelayCommand]
    async Task Chon()
    {
        if (DataPath == "Hiện chưa chọn tập tin nào.")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng chọn tập tin Excel trước khi chọn", "OK");
            return;
        }
        await Shell.Current.GoToAsync($"{nameof(DanhSachSP)}?nochange=ok");
    }

    [RelayCommand]
    async Task EntryTap()
    {
        await Shell.Current.DisplayAlert("Tập tin của bạn", DataPath, "OK");
    }
}