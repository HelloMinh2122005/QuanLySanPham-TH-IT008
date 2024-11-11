﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLySanPham.Model;

namespace QuanLySanPham.ViewModel;

[QueryProperty(nameof(SanPham), "sanphamPara")]
public partial class EditSanPhamViewModel : ObservableObject
{
    [ObservableProperty]
    public SanPham sanpham;

    public EditSanPhamViewModel()
    {
        sanpham = new SanPham();
    }   

    [RelayCommand]
    async Task Save()
    {
        if (sanpham.Ten == "")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng tên sản phẩm", "OK");
            return;
        }
        if (sanpham.GiaTien == 0)
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng nhập đơn giá sản phẩm", "OK");
            return;
        }
        if (sanpham.SoLuong == 0)
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng số lượng sản phẩm", "OK");
            return;
        }
        await Shell.Current.GoToAsync($"..", new Dictionary<string, object>
        {
            {"edit", sanpham}
        });
    }

    [RelayCommand]
    async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }   
}