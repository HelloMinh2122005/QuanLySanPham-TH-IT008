using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLySanPham.View;
using QuanLySanPham.Model;
using Aspose.Cells;
using System.Collections.ObjectModel;
using QuanLySanPham.Services;

namespace QuanLySanPham.ViewModel;

public partial class MainPageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private string userName = "";

    [ObservableProperty]
    private string hello = "Chào ";

    [ObservableProperty]
    private string dataPath = "Hiện chưa chọn tập tin nào.";

    private readonly FileService fileService = new FileService();

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("UserName"))
        {
            Hello = "Chào ";
            UserName = query["UserName"].ToString() ?? "";
            Hello += UserName;
        }
    }

    [RelayCommand]
    async Task NhapFile()
    {
        DataPath = await fileService.Import(DataPath, UserName);
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
                    {"DsSanPham", new SanPham() },
                    {"UserName", UserName }
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