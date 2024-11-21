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
                DsToShow.Add(new HistoryItem { SanPham = item, Action = "Thêm" });

            foreach (var item in DsSpSua)
                DsToShow.Add(new HistoryItem { SanPham = item, Action = "Sửa" });

            foreach (var item in DsSpXoa)
                DsToShow.Add(new HistoryItem { SanPham = item, Action = "Xóa" });
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

        var historyItem = DsToShow.FirstOrDefault(x => x.SanPham == selectedSanPham.SanPham);
        if (historyItem == null) return;

        switch (historyItem.Action)
        {
            case "Thêm":
                DsSpThem.Remove(selectedSanPham.SanPham);
                DsToShow.Remove(historyItem);
                dsToBack.Add(historyItem);
                break;
            case "Sửa":
                DsSpSua.Remove(selectedSanPham.SanPham);
                DsToShow.Remove(historyItem);
                dsToBack.Add(historyItem);
                break;
            case "Xóa":
                DsSpXoa.Remove(selectedSanPham.SanPham);
                DsToShow.Remove(historyItem);
                dsToBack.Add(historyItem);
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
