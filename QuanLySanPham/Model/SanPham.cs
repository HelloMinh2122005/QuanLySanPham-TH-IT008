namespace QuanLySanPham.Model;

public class SanPham
{
    public string MaSanPham { get; set; } = "";
    public string Ten { get; set; } = "";
    public int SoLuong { get; set; } = 0;
    public float GiaTien { get; set; } = 0;
    public float TongTien => SoLuong * GiaTien;
}