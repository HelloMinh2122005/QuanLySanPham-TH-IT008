using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLySanPham.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLySanPham.ViewModel;

public partial class ViewHistoryViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<SanPham> dsSpThem = new();

    [ObservableProperty]
    private ObservableCollection<SanPham> dsSpSua = new();

    [ObservableProperty]
    private ObservableCollection<SanPham> dsSpXoa = new();

    [ObservableProperty]
    private ObservableCollection<HistoryItem> dsToShow = new();

    private ObservableCollection<HistoryItem> dsToBack = new();

    private HistoryItem selectedSanPham = new();

    void IQueryAttributable.ApplyQueryAttributes(System.Collections.Generic.IDictionary<string, object> query)
    {
        if (query.ContainsKey("DSSPThem"))
        {
            DsSpThem.Clear();
            DsSpSua.Clear();
            DsSpXoa.Clear();
            DsToShow.Clear();
            dsToBack.Clear();

            DsSpThem = (ObservableCollection<SanPham>)query["DSSPThem"];
            DsSpSua = (ObservableCollection<SanPham>)query["DSSPSua"];
            DsSpXoa = (ObservableCollection<SanPham>)query["DSSPXoa"];

            foreach (var item in DsSpThem)
                DsToShow.Insert(0, new HistoryItem { SanPham = item, Action = "Thêm" });

            foreach (var item in DsSpSua)
                DsToShow.Insert(0, new HistoryItem { SanPham = item, Action = "Sửa" });

            foreach (var item in DsSpXoa)
                DsToShow.Insert(0, new HistoryItem { SanPham = item, Action = "Xóa" });
        }
    }

    [RelayCommand]
    public void OnItemSelected(HistoryItem sp)
    {
        selectedSanPham = sp;
    }

    [RelayCommand]
    public void Recover()
    {
        if (selectedSanPham == null)
        {
            Shell.Current.DisplayAlert("Thông báo", "Chưa chọn sản phẩm nào", "OK");
            return;
        }

        switch (selectedSanPham.Action)
        {
            case "Thêm":
                dsToBack.Add(selectedSanPham);
                DsSpThem.Remove(selectedSanPham.SanPham);
                var itemsToRemove = DsToShow
                    .Where (
                        item => 
                            item.SanPham.MaSanPham == selectedSanPham.SanPham.MaSanPham 
                            && item.Action == "Sửa")
                    .ToList();
                foreach (var item in itemsToRemove)
                {
                    if (item.SanPham.MaSanPham == selectedSanPham.SanPham.MaSanPham)
                    {
                        DsSpSua.Remove(item.SanPham);
                        DsToShow.Remove(item);
                    }
                }
                DsToShow.Remove(selectedSanPham);
                break;

            case "Sửa":
                dsToBack.Add(selectedSanPham);
                DsSpSua.Remove(selectedSanPham.SanPham);
                DsToShow.Remove(selectedSanPham);
                break;
            case "Xóa":
                dsToBack.Add(selectedSanPham);
                DsSpXoa.Remove(selectedSanPham.SanPham);
                DsToShow.Remove(selectedSanPham);
                break;
        }
    }

    [RelayCommand]
    async Task Done()
    {
        await Shell.Current.GoToAsync($"..", new Dictionary<string, object>
        {
            { "DSRecover", dsToBack }
        });
    }
}
