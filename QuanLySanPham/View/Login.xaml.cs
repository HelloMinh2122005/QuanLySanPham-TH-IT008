using QuanLySanPham.ViewModel;

namespace QuanLySanPham.View;

public partial class Login : ContentPage
{
	public Login(LoginViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}