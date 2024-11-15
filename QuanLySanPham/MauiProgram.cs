﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using QuanLySanPham.Model;
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

            return builder.Build();
        }
    }
}
