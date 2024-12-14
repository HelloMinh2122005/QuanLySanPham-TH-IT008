using QuanLySanPham.ViewModel;

namespace QuanLySanPham.View;

public partial class SaveFilePage : ContentPage
{
	public SaveFilePage(SaveFilePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}