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
        if (DataPath != "Hiện chưa chọn tập tin nào.")
        {
            bool ac = await Shell.Current.DisplayAlert("Thông báo", "Bạn có chắc muốn nhập hóa đơn mới và hủy hóa đơn hiện tại?", "Có", "Không");
            if (ac)
            {
                DataPath = await fileService.Import();
            }
            return;
        }
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
        if (DataPath != "Hiện chưa chọn tập tin nào.")
        {
            bool ac = await Shell.Current.DisplayAlert("Thông báo", "Bạn có chắc muốn nhập hóa đơn mới và hủy hóa đơn hiện tại?", "Có", "Không");
            if (ac)
            {
                DataPath = "Hiện có 1 hóa đơn đang được xử lý.";
                await Shell.Current.GoToAsync(nameof(DanhSachSP), new Dictionary<string, object>
                {
                    {"DsSanPham", new SanPham() }
                });
            }
            return;
        }
        DataPath = "Hiện có 1 hóa đơn đang được xử lý.";
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
            await Shell.Current.DisplayAlert("Thông báo", "Hiện tại chưa có hóa đơn nào được xử lý", "OK");
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