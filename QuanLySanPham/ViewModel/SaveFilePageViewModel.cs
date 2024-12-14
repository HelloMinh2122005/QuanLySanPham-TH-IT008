using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLySanPham.Model;
using QuanLySanPham.Services;
using System.Collections.ObjectModel;

namespace QuanLySanPham.ViewModel;

public partial class SaveFilePageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    string tenKhachHang = "";

    [ObservableProperty]
    float giamGia = 0;

    [ObservableProperty]
    float tongTien = 0;

    HoaDon hd = new();
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("HoaDon"))
        {
            hd = (HoaDon)query["HoaDon"];
            TongTien = hd.TongTien;
        }
    }

    partial void OnGiamGiaChanged(float value)
    {
        Task.Run(async () =>
        {
            await Task.Delay(100); // Wait for 0.1 second
            TongTien = hd.TongTien * (100 - GiamGia) / 100;
        });
    }

    [RelayCommand]
    async Task Export()
    {
        hd.TenKhachHang = TenKhachHang;
        hd.TongTienSauGiam = TongTien;
        hd.GiamGia = GiamGia;

        var fileHelper = new FileService();
        await fileHelper.Export(hd);
    }

    [RelayCommand]
    async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
