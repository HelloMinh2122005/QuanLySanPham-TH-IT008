using System.Collections.ObjectModel;

namespace QuanLySanPham.Model;

public class HoaDon
{
    public string TenKhachHang { get; set; } = "";
    public float TongTien { get; set; } = 0;
    public float TongTienSauGiam { get; set; } = 0;
    public float GiamGia { get; set; } = 0;
    public ObservableCollection<SanPham> DsSanPham { get; set; } = [];
}
