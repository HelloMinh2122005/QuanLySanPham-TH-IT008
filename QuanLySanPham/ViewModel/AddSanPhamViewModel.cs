using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLySanPham.Model;
using QuanLySanPham.View;

namespace QuanLySanPham.ViewModel;

public partial class AddSanPhamViewModel : ObservableObject
{
    [ObservableProperty]
    public string maSanPham;
    [ObservableProperty]
    public string ten;
    [ObservableProperty]
    public int soLuong;
    [ObservableProperty]
    public int giaTien;

    public AddSanPhamViewModel()
    {
        maSanPham = "";
        ten = "";
        soLuong = 0;
        giaTien = 0;
    }

    [RelayCommand]
    async Task Save()
    {
        if (MaSanPham == "")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng nhập mã sản phẩm", "OK");
            return;
        }
        if (Ten == "")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng tên sản phẩm", "OK");
            return;
        }
        if (GiaTien == 0)
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng nhập đơn giá sản phẩm", "OK");
            return;
        }
        if (SoLuong == 0)
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng số lượng sản phẩm", "OK");
            return;
        }

        await Shell.Current.GoToAsync($"..", new Dictionary<string, object>
        {
            {"add", new SanPham
                {
                    MaSanPham = MaSanPham,
                    Ten = Ten,
                    SoLuong = SoLuong,
                    GiaTien = GiaTien
                } }
        });
    }

    [RelayCommand]
    async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}