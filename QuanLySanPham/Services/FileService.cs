using Aspose.Cells;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Views;
using QuanLySanPham.Model;
using QuanLySanPham.View;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace QuanLySanPham.Services
{
    public partial class FileService
    {
        public async Task<string> Import(string DataPath, string UserName)
        {
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
                    DataPath = file.FullPath;
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

                    await Shell.Current.DisplayAlert("Thông báo", "Nhập file thành công", "OK");

                    await Shell.Current.GoToAsync(nameof(DanhSachSP), new Dictionary<string, object>
                    {
                        { "DsSanPham", DsSanPham },
                        { "UserName", UserName }
                    });

                    return DataPath;
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"An error occurred while reading the Excel file: {ex.Message}", "OK");
                }
            }

            return DataPath;
        }

        public async Task CreateNewFile()
        {
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

                for (int j = 0; j <= 3; j++)
                {
                    worksheet.Cells[0, j].SetStyle(headerStyle);
                }

                // Apply border style to the table headers
                var borderStyle = workbook.CreateStyle();
                borderStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                borderStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                borderStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                borderStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;

                for (int j = 0; j <= 3; j++)
                {
                    worksheet.Cells[0, j].SetStyle(borderStyle);
                }

                // Save the workbook to a MemoryStream
                using var stream = new MemoryStream();
                workbook.Save(stream, SaveFormat.Xlsx);
                stream.Seek(0, SeekOrigin.Begin);

                // Use FileSaver to save the file to the user's chosen location
                var baseFileName = "input";
                var extension = ".xlsx";
                var fileName = baseFileName + extension;
                bool fileSaved = false;
                int fileIndex = 0;

                while (!fileSaved)
                {
                    var fileSaverResultTask = FileSaver.Default.SaveAsync(fileName, stream);

                    var fileSaverResult = await fileSaverResultTask;

                    if (fileSaverResult.IsSuccessful)
                    {
                        fileSaved = true;
                        await Shell.Current.DisplayAlert("Thông báo", "Tạo file thành công", "OK");
                    }
                    else if (fileSaverResult.Exception is IOException)
                    {
                        // Handle file name conflict by appending a number
                        fileIndex++;
                        fileName = $"{baseFileName}({fileIndex}){extension}";
                        stream.Seek(0, SeekOrigin.Begin);
                    }
                    else if (fileSaverResult.Exception != null)
                    {
                        throw fileSaverResult.Exception;
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Thông báo", "Bạn đã hủy lưu file.", "OK");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred while creating the Excel file: {ex.Message}", "OK");
            }
        }


        public async Task Export(string UserName, float ThanhTien, ObservableCollection<SanPham> DsSanPham)
        {
            var cts = new CancellationTokenSource();
            var userCancelled = await AllowUserToCancelAsync(cts.Token);
            if (userCancelled)
            {
                await Shell.Current.DisplayAlert("Thông báo", "Bạn đã hủy lưu file.", "OK");
                return;
            }

            var hoaDon = new HoaDon
            {
                TenNguoiTao = UserName,
                NgayTao = DateTime.Now,
                TongTien = ThanhTien,
                DsSanPham = DsSanPham
            };

            try
            {
                Workbook workbook = new Workbook();
                Worksheet worksheet = workbook.Worksheets[0];

                // Set up worksheet columns and headers
                worksheet.Cells.SetColumnWidth(0, 5);
                worksheet.Cells.SetColumnWidth(1, 20);
                worksheet.Cells.SetColumnWidth(2, 20);
                worksheet.Cells.SetColumnWidth(3, 12);
                worksheet.Cells.SetColumnWidth(4, 12);
                worksheet.Cells.SetColumnWidth(5, 15);

                worksheet.Cells[0, 1].PutValue("Tên người tạo:");
                worksheet.Cells[0, 2].PutValue(hoaDon.TenNguoiTao);
                worksheet.Cells[1, 1].PutValue("Ngày tạo:");
                worksheet.Cells[1, 2].PutValue(hoaDon.NgayTao.ToString("dd/MM/yyyy"));
                worksheet.Cells[2, 1].PutValue("Tổng giá trị hóa đơn:");
                worksheet.Cells[2, 2].PutValue(hoaDon.TongTien.ToString());
                worksheet.Cells[2, 3].PutValue("Đơn vị: VNĐ");

                worksheet.Cells[4, 0].PutValue("STT");
                worksheet.Cells[4, 1].PutValue("Mã sản phẩm");
                worksheet.Cells[4, 2].PutValue("Tên sản phẩm");
                worksheet.Cells[4, 3].PutValue("Số lượng");
                worksheet.Cells[4, 4].PutValue("Giá tiền");
                worksheet.Cells[4, 5].PutValue("Tổng tiền");

                int rowIndex = 5;
                foreach (var sp in hoaDon.DsSanPham)
                {
                    int valStt = rowIndex - 4;
                    worksheet.Cells[rowIndex, 0].PutValue(valStt.ToString());
                    worksheet.Cells[rowIndex, 1].PutValue(sp.MaSanPham);
                    worksheet.Cells[rowIndex, 2].PutValue(sp.Ten);
                    worksheet.Cells[rowIndex, 3].PutValue(sp.SoLuong.ToString());
                    worksheet.Cells[rowIndex, 4].PutValue(sp.GiaTien.ToString());
                    worksheet.Cells[rowIndex, 5].PutValue(sp.TongTien.ToString());
                    rowIndex++;
                }

                // Apply header style
                var headerStyle = workbook.CreateStyle();
                headerStyle.Font.IsBold = true;
                headerStyle.ForegroundColor = System.Drawing.Color.LightGray;
                headerStyle.Pattern = BackgroundType.Solid;
                headerStyle.HorizontalAlignment = TextAlignmentType.Center;

                worksheet.Cells[4, 0].SetStyle(headerStyle);
                worksheet.Cells[4, 1].SetStyle(headerStyle);
                worksheet.Cells[4, 2].SetStyle(headerStyle);
                worksheet.Cells[4, 3].SetStyle(headerStyle);
                worksheet.Cells[4, 4].SetStyle(headerStyle);
                worksheet.Cells[4, 5].SetStyle(headerStyle);

                // Apply border style
                var borderStyle = workbook.CreateStyle();
                borderStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                borderStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                borderStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                borderStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;

                for (int i = 5; i < rowIndex; i++)
                {
                    for (int j = 0; j <= 5; j++)
                    {
                        worksheet.Cells[i, j].SetStyle(borderStyle);
                    }
                }

                // Apply top header style
                var topHeaderStyle = workbook.CreateStyle();
                topHeaderStyle.Font.IsBold = true;
                topHeaderStyle.HorizontalAlignment = TextAlignmentType.Left;

                worksheet.Cells[0, 1].SetStyle(topHeaderStyle);
                worksheet.Cells[1, 1].SetStyle(topHeaderStyle);
                worksheet.Cells[2, 1].SetStyle(topHeaderStyle);
                worksheet.Cells[2, 3].SetStyle(topHeaderStyle);

                // Save the workbook to a MemoryStream
                using var stream = new MemoryStream();
                workbook.Save(stream, SaveFormat.Pdf);
                stream.Seek(0, SeekOrigin.Begin);

                // Use FileSaver to save the file to the user's chosen location
                var baseFileName = "hoaDon";
                var extension = ".pdf";
                var fileName = baseFileName + extension;
                bool fileSaved = false;
                int fileIndex = 0;

                while (!fileSaved)
                {
                    var fileSaverResultTask = FileSaver.Default.SaveAsync(fileName, stream);

                    var fileSaverResult = await fileSaverResultTask;

                    if (fileSaverResult.IsSuccessful)
                    {
                        fileSaved = true;
                        await Shell.Current.DisplayAlert("Thông báo", $"Xuất file thành công", "OK");
                    }
                    else if (fileSaverResult.Exception is IOException)
                    {
                        fileIndex++;
                        fileName = $"{baseFileName}({fileIndex}){extension}";
                        stream.Seek(0, SeekOrigin.Begin);
                    }
                    else if (fileSaverResult.Exception != null)
                    {
                        throw fileSaverResult.Exception;
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Thông báo", "Bạn đã hủy lưu file.", "OK");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Lỗi", $"Có lỗi xảy ra trong quá trình xuất file: {ex.Message}", "OK");
            }
            finally
            {
                cts.Cancel(); // Cancel the alert if the file-saving process starts
            }
        }

        async Task<bool> AllowUserToCancelAsync(CancellationToken token)
        {
            bool userCancelled = false;

            var cancellationTask = Shell.Current.DisplayAlert("Thông báo", "File đang được lưu...", "", "Hủy");

            if (await Task.WhenAny(cancellationTask, Task.Delay(3000, token)) == cancellationTask)
            {
                userCancelled = await cancellationTask == false;
            }

            return userCancelled;
        }
    }
}
