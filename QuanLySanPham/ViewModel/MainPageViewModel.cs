using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLySanPham.View;
using Aspose.Cells;

namespace QuanLySanPham.ViewModel;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string userName;

    public MainPageViewModel()
    {
        userName = "";
    }

    [RelayCommand]
    async Task Enter()
    {
        if (UserName == "")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng nhập tên", "OK");
            return;
        }
        await Shell.Current.GoToAsync(nameof(DanhSachSP), new Dictionary<string, object>
        {
            {"userName", UserName }
        });
    }

    [RelayCommand]
    async Task Create()
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
                worksheet.Cells.SetColumnWidth(0, 5);
                worksheet.Cells.SetColumnWidth(1, 20);
                worksheet.Cells.SetColumnWidth(2, 20);
                worksheet.Cells.SetColumnWidth(3, 12);
                worksheet.Cells.SetColumnWidth(4, 12);
                worksheet.Cells.SetColumnWidth(5, 15);

                // Table headers
                worksheet.Cells[0, 0].PutValue("STT");
                worksheet.Cells[0, 1].PutValue("Mã sản phẩm");
                worksheet.Cells[0, 2].PutValue("Tên sản phẩm");
                worksheet.Cells[0, 3].PutValue("Số lượng");
                worksheet.Cells[0, 4].PutValue("Giá tiền");
                worksheet.Cells[0, 5].PutValue("Tổng tiền");

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
                worksheet.Cells[0, 4].SetStyle(headerStyle);
                worksheet.Cells[0, 5].SetStyle(headerStyle);

                // Apply border style to the table
                var borderStyle = workbook.CreateStyle();
                borderStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                borderStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                borderStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                borderStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;

                for (int i = 0; i <= 0; i++)
                {
                    for (int j = 0; j <= 5; j++)
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
}