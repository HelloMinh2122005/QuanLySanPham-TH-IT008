using QuanLySanPham.ViewModel;

namespace QuanLySanPham;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}