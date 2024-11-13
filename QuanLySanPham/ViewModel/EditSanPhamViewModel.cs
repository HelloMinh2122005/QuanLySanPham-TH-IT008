using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLySanPham.Model;
using QuanLySanPham.View;

namespace QuanLySanPham.ViewModel;

public partial class EditSanPhamViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private SanPham sanpham;

    public EditSanPhamViewModel()
    {
        sanpham = new SanPham();
    }

    void IQueryAttributable.ApplyQueryAttributes(System.Collections.Generic.IDictionary<string, object> query)
    {
        if (query.ContainsKey("sanphamPara"))
        {
            Sanpham = (SanPham)query["sanphamPara"];
        }
    }

        [RelayCommand]
    async Task Save()
    {
        if (Sanpham.Ten == "")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng tên sản phẩm", "OK");
            return;
        }
        if (Sanpham.GiaTien == 0)
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng nhập đơn giá sản phẩm", "OK");
            return;
        }
        if (Sanpham.SoLuong == 0)
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng số lượng sản phẩm", "OK");
            return;
        }
        await Shell.Current.GoToAsync($"..", new Dictionary<string, object>
        {
            {"edit", Sanpham}
        });
    }

    [RelayCommand]
    async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }   
}