using QuanLySanPham.ViewModel;

namespace QuanLySanPham.View;

public partial class ViewHistory : ContentPage
{
	public ViewHistory(ViewHistoryViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}