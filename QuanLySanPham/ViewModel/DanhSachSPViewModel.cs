using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLySanPham.Model;
using QuanLySanPham.View;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace QuanLySanPham.ViewModel;

public partial class DanhSachSPViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<SanPham> dsSanPham;

    private SanPham selectedSanPham;

    [ObservableProperty]
    private int stt;

    [ObservableProperty]
    float thanhTien;

    public float GiaTientmp;

    public DanhSachSPViewModel()
    {
        dsSanPham = new ObservableCollection<SanPham>();
        Stt = 0;
        thanhTien = 0;
        GiaTientmp = 0;
        selectedSanPham = new SanPham();
    }

    void IQueryAttributable.ApplyQueryAttributes(System.Collections.Generic.IDictionary<string, object> query)
    {
        if (query.ContainsKey("add"))
        {
            Stt++;
            var newSP = query["add"] as SanPham;
            DsSanPham.Add(newSP);
            ThanhTien += newSP.TongTien;
            query.Remove("add");
        }
        if (query.ContainsKey("edit"))
        {
            var editSP = query["edit"] as SanPham;
            var index = DsSanPham.IndexOf(selectedSanPham);
            DsSanPham[index] = editSP;
            ThanhTien -= GiaTientmp;
            GiaTientmp = 0;
            ThanhTien += editSP.TongTien;
            query.Remove("edit");
        }
    }

    [RelayCommand]
    async Task Add()
    {
        await Shell.Current.GoToAsync(nameof(AddSanPham));
    }

    [RelayCommand]
    async Task Del()
    {
        if (selectedSanPham == null || selectedSanPham.MaSanPham == "")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng chọn sản phẩm để xóa", "OK");
            return;
        }
        ThanhTien -= selectedSanPham.TongTien;
        DsSanPham.Remove(selectedSanPham);
    }

    [RelayCommand]
    public async Task Edit()
    {
        if (selectedSanPham.MaSanPham == "")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng chọn sản phẩm để sửa", "OK");
            return;
        }
        await Shell.Current.GoToAsync(nameof(EditSanPham), new Dictionary<string, object>
        {
            {"sanphamPara", selectedSanPham }
        });
        
        GiaTientmp = selectedSanPham.TongTien;
    }

    [RelayCommand]
    public async Task Export()
    {
        await Task.CompletedTask;
    }

    [RelayCommand]
    public void Select(SanPham selectedSP)
    {
        selectedSanPham = selectedSP;
    }
}