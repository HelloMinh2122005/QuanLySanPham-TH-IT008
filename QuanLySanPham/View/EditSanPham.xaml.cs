using QuanLySanPham.ViewModel;

namespace QuanLySanPham.View;

public partial class EditSanPham : ContentPage
{
	public EditSanPham(EditSanPhamViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}