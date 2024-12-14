using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using QuanLySanPham.Model;
using QuanLySanPham.Services;
using QuanLySanPham.View;
using QuanLySanPham.ViewModel;

namespace QuanLySanPham
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<SanPham>();
            builder.Services.AddSingleton<HoaDon>();
            builder.Services.AddSingleton<HistoryItem>();
            builder.Services.AddSingleton<FileService>();

            builder.Services.AddSingleton<Login>();
            builder.Services.AddSingleton<LoginViewModel>();

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainPageViewModel>();

            builder.Services.AddSingleton<DanhSachSP>();
            builder.Services.AddSingleton<DanhSachSPViewModel>();

            builder.Services.AddTransient<AddSanPham>();
            builder.Services.AddTransient<AddSanPhamViewModel>();

            builder.Services.AddTransient<EditSanPham>();
            builder.Services.AddTransient<EditSanPhamViewModel>();

            builder.Services.AddTransient<ViewHistory>();
            builder.Services.AddTransient<ViewHistoryViewModel>();

            builder.Services.AddTransient<SaveFilePage>();
            builder.Services.AddTransient<SaveFilePageViewModel>();

            return builder.Build();
        }
    }
}
