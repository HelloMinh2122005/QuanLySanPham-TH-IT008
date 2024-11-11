using QuanLySanPham.ViewModel;

namespace QuanLySanPham.View;

public partial class DanhSachSP : ContentPage
{
	public DanhSachSP(DanhSachSPViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}