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
    string giamGia = "";

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

    partial void OnGiamGiaChanged(string value)
    {
        Task.Run(async () =>
        {
            await Task.Delay(100); // Wait for 0.1 second

            if (string.IsNullOrWhiteSpace(GiamGia))
            {
                TongTien = hd.TongTien;
            }
            else
            {
                float giamgia = float.Parse(GiamGia);
                TongTien = hd.TongTien * (100 - giamgia) / 100;
            }
        });
    }

    [RelayCommand]
    async Task Export()
    {
        if (string.IsNullOrWhiteSpace(GiamGia))
        {
            GiamGia = "0";
            TongTien = hd.TongTien;
        }
        hd.TenKhachHang = TenKhachHang;
        hd.TongTienSauGiam = TongTien;
        hd.GiamGia = float.Parse(GiamGia);

        var fileHelper = new FileService();
        await fileHelper.Export(hd);
    }

    [RelayCommand]
    async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
