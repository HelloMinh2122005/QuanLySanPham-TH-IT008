using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLySanPham.View;
using QuanLySanPham.Model;
using Aspose.Cells;
using System.Collections.ObjectModel;

namespace QuanLySanPham.ViewModel;

public partial class MainPageViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private string userName;

    [ObservableProperty]
    private string hello;

    public MainPageViewModel()
    {
        UserName = "";
        Hello = "Chào ";
    }

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("UserName"))
        {
            Hello = "Chào ";
            UserName = query["UserName"].ToString() ?? "";
            Hello += UserName;
        }
    }

    [RelayCommand]
    async Task NhapFile()
    {
        if (UserName == "")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng nhập tên", "OK");
            return;
        }

        var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.iOS, new[] { "com.microsoft.excel.xls", "org.openxmlformats.spreadsheetml.sheet" } },
            { DevicePlatform.Android, new[] { "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } },
            { DevicePlatform.WinUI, new[] { ".xls", ".xlsx" } },
            { DevicePlatform.MacCatalyst, new[] { "com.microsoft.excel.xls", "org.openxmlformats.spreadsheetml.sheet" } }
        });

        var file = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Chọn file Excel",
            FileTypes = customFileType
        });

        if (file != null)
        {
            try
            {
                using var stream = await file.OpenReadAsync();
                Workbook workbook = new Workbook(stream);
                Worksheet worksheet = workbook.Worksheets[0];

                ObservableCollection<SanPham> DsSanPham = new ObservableCollection<SanPham>();
                for (int i = 1; i <= worksheet.Cells.MaxDataRow; i++)
                {
                    var maSanPham = worksheet.Cells[i, 0].StringValue;
                    var tenSanPham = worksheet.Cells[i, 1].StringValue;
                    var soLuong = worksheet.Cells[i, 2].IntValue;
                    var giaTien = worksheet.Cells[i, 3].FloatValue;

                    DsSanPham.Add(new SanPham
                    {
                        MaSanPham = maSanPham,
                        Ten = tenSanPham,
                        SoLuong = soLuong,
                        GiaTien = giaTien
                    });
                }

                await Shell.Current.GoToAsync(nameof(DanhSachSP), new Dictionary<string, object>
                {
                    {"DsSanPham", DsSanPham },
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred while reading the Excel file: {ex.Message}", "OK");
            }
        }
    }

    [RelayCommand]
    async Task TaoFileMoi()
    {
        var folder = await FolderPicker.PickAsync(default);
        if (folder != null && folder.Folder != null)
        {
            var filePath = Path.Combine(folder.Folder.Path, "input.xlsx");
            try
            {
                Workbook workbook = new Workbook();
                Worksheet worksheet = workbook.Worksheets[0];

                // Set column widths for a cleaner layout
                worksheet.Cells.SetColumnWidth(0, 20);
                worksheet.Cells.SetColumnWidth(1, 20);
                worksheet.Cells.SetColumnWidth(2, 12);
                worksheet.Cells.SetColumnWidth(3, 12);

                // Table headers
                worksheet.Cells[0, 0].PutValue("Mã sản phẩm");
                worksheet.Cells[0, 1].PutValue("Tên sản phẩm");
                worksheet.Cells[0, 2].PutValue("Số lượng");
                worksheet.Cells[0, 3].PutValue("Giá tiền");

                // Apply bold style for headers
                var headerStyle = workbook.CreateStyle();
                headerStyle.Font.IsBold = true;
                headerStyle.ForegroundColor = System.Drawing.Color.LightGray;
                headerStyle.Pattern = BackgroundType.Solid;
                headerStyle.HorizontalAlignment = TextAlignmentType.Center;

                worksheet.Cells[0, 0].SetStyle(headerStyle);
                worksheet.Cells[0, 1].SetStyle(headerStyle);
                worksheet.Cells[0, 2].SetStyle(headerStyle);
                worksheet.Cells[0, 3].SetStyle(headerStyle);

                // Apply border style to the table
                var borderStyle = workbook.CreateStyle();
                borderStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                borderStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                borderStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                borderStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;

                for (int i = 0; i <= 0; i++)
                {
                    for (int j = 0; j <= 3; j++)
                    {
                        worksheet.Cells[i, j].SetStyle(borderStyle);
                    }
                }

                // Save the workbook as Excel file
                workbook.Save(filePath, SaveFormat.Xlsx);

                await Shell.Current.DisplayAlert("Thông báo", "Tạo file thành công", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred while creating the Excel file: {ex.Message}", "OK");
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng chọn thư mục để lưu file", "OK");
        }
    }

    [RelayCommand]
    async Task BoQua()
    {
        await Shell.Current.GoToAsync(nameof(DanhSachSP), new Dictionary<string, object>
                {
                    {"DsSanPham", new SanPham() },
                });
    }
}