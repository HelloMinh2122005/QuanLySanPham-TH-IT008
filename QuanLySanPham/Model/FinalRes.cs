
using System.Collections.ObjectModel;

namespace QuanLySanPham.Model;

public class FinalRes
{
    public string TenNguoiTao { get; set; } = "";
    public DateTime NgayTao { get; set; } = DateTime.Now;
    public float TongTien { get; set; } = 0;

    public ObservableCollection<SanPham> DsSanPham { get; set; } = [];
}
