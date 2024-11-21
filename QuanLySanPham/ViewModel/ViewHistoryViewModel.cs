using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLySanPham.ViewModel;

[QueryProperty("SoSPThem", "SoSPThem")]
[QueryProperty("SoSPSua", "SoSPSua")]
[QueryProperty("SoSPXoa", "SoSPXoa")]
[QueryProperty("SoSPBandau", "SoSPBandau")]
[QueryProperty("DsSanPhamChinhSua", "DsSanPhamChinhSua")]
public partial class ViewHistoryViewModel : ObservableObject
{
    [ObservableProperty]
    private int soSPThem;

    [ObservableProperty]
    private int soSPSua;

    [ObservableProperty]
    private int soSPXoa;

    [ObservableProperty]
    private int soSPBandau;

    [ObservableProperty]
    private ObservableCollection<SanPham> dsSanPhamChinhSua;

    public ViewHistoryViewModel()
    {
        SoSPThem = 0;
        SoSPSua = 0;
        SoSPXoa = 0;
        SoSPBandau = 0;
        DsSanPhamChinhSua = new ObservableCollection<SanPham>();
    }


}
