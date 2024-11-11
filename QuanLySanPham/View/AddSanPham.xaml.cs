using QuanLySanPham.ViewModel;

namespace QuanLySanPham.View;

public partial class AddSanPham : ContentPage
{
	public AddSanPham(AddSanPhamViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;	
    }
}