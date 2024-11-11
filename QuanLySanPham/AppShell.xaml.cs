using QuanLySanPham.View;

namespace QuanLySanPham
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(DanhSachSP), typeof(DanhSachSP));
            Routing.RegisterRoute(nameof(AddSanPham), typeof(AddSanPham));
            Routing.RegisterRoute(nameof(EditSanPham), typeof(EditSanPham));
        }
    }
}
