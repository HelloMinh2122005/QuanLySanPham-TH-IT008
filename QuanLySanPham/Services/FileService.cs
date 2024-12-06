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
        /// <summary>
        /// Thực thi một thao tác không đồng bộ với popup chờ xử lý và hỗ trợ hủy bỏ.
        /// </summary>
        private async Task ExecuteWithPopupAsync(
            Func<CancellationToken, Task> operation,
            string successMessage,
            string cancelMessage,
            string errorMessage)
        {
            var cts = new CancellationTokenSource();
            var popup = new WaitPopup(cts.Cancel);

            // Hiển thị popup
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);

            try
            {
                // Thực thi thao tác với CancellationToken
                await operation(cts.Token);

                if (!cts.IsCancellationRequested)
                {
                    // Đóng popup khi hoàn thành thành công
                    await popup.CloseAsync();

                    if (!string.IsNullOrEmpty(successMessage))
                        await Shell.Current.DisplayAlert("Thông báo", successMessage, "OK");
                }
            }
            catch (OperationCanceledException)
            {
                // Xử lý khi người dùng hủy bỏ
                await popup.CloseAsync();

                if (!string.IsNullOrEmpty(cancelMessage))
                    await Shell.Current.DisplayAlert("Thông báo", cancelMessage, "OK");
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ khác
                await popup.CloseAsync();

                if (!string.IsNullOrEmpty(errorMessage))
                    await Shell.Current.DisplayAlert("Lỗi", $"{errorMessage}: {ex.Message}", "OK");
            }
            finally
            {
                cts.Dispose();
            }
        }

        /// <summary>
        /// Xuất dữ liệu ra file PDF với hỗ trợ hủy bỏ.
        /// </summary>
        public async Task Export(string userName, float totalAmount, ObservableCollection<SanPham> products)
        {
            Func<CancellationToken, Task> exportOperation = async (token) =>
            {
                // Khởi tạo Workbook
                Workbook workbook = new Workbook();
                Worksheet worksheet = workbook.Worksheets[0];

                // Thiết lập cột và tiêu đề
                worksheet.Cells.SetColumnWidth(0, 5);
                worksheet.Cells.SetColumnWidth(1, 20);
                worksheet.Cells.SetColumnWidth(2, 20);
                worksheet.Cells.SetColumnWidth(3, 12);
                worksheet.Cells.SetColumnWidth(4, 12);
                worksheet.Cells.SetColumnWidth(5, 15);

                worksheet.Cells[0, 1].PutValue("Tên người tạo:");
                worksheet.Cells[0, 2].PutValue(userName);
                worksheet.Cells[1, 1].PutValue("Ngày tạo:");
                worksheet.Cells[1, 2].PutValue(DateTime.Now.ToString("dd/MM/yyyy"));
                worksheet.Cells[2, 1].PutValue("Tổng giá trị hóa đơn:");
                worksheet.Cells[2, 2].PutValue(totalAmount.ToString());
                worksheet.Cells[2, 3].PutValue("Đơn vị: VNĐ");

                worksheet.Cells[4, 0].PutValue("STT");
                worksheet.Cells[4, 1].PutValue("Mã sản phẩm");
                worksheet.Cells[4, 2].PutValue("Tên sản phẩm");
                worksheet.Cells[4, 3].PutValue("Số lượng");
                worksheet.Cells[4, 4].PutValue("Giá tiền");
                worksheet.Cells[4, 5].PutValue("Tổng tiền");

                int rowIndex = 5;
                int stt = 1;
                foreach (var sp in products)
                {
                    token.ThrowIfCancellationRequested();

                    worksheet.Cells[rowIndex, 0].PutValue(stt++);
                    worksheet.Cells[rowIndex, 1].PutValue(sp.MaSanPham);
                    worksheet.Cells[rowIndex, 2].PutValue(sp.Ten);
                    worksheet.Cells[rowIndex, 3].PutValue(sp.SoLuong.ToString());
                    worksheet.Cells[rowIndex, 4].PutValue(sp.GiaTien.ToString());
                    worksheet.Cells[rowIndex, 5].PutValue(sp.TongTien.ToString());
                    rowIndex++;

                    // Giả lập xử lý
                    await Task.Delay(100, token);
                }

                // Áp dụng style cho tiêu đề
                var headerStyle = workbook.CreateStyle();
                headerStyle.Font.IsBold = true;
                headerStyle.ForegroundColor = System.Drawing.Color.LightGray;
                headerStyle.Pattern = BackgroundType.Solid;
                headerStyle.HorizontalAlignment = TextAlignmentType.Center;

                for (int j = 0; j <= 5; j++)
                {
                    worksheet.Cells[4, j].SetStyle(headerStyle);
                }

                // Áp dụng border cho dữ liệu
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

                // Áp dụng style cho header phía trên
                var topHeaderStyle = workbook.CreateStyle();
                topHeaderStyle.Font.IsBold = true;
                topHeaderStyle.HorizontalAlignment = TextAlignmentType.Left;

                worksheet.Cells[0, 1].SetStyle(topHeaderStyle);
                worksheet.Cells[1, 1].SetStyle(topHeaderStyle);
                worksheet.Cells[2, 1].SetStyle(topHeaderStyle);
                worksheet.Cells[2, 3].SetStyle(topHeaderStyle);

                // Lưu Workbook vào MemoryStream
                using var stream = new MemoryStream();
                workbook.Save(stream, SaveFormat.Pdf);
                stream.Seek(0, SeekOrigin.Begin);

                // Lưu file bằng FileSaver
                var baseFileName = "Hóa đơn";
                var extension = ".pdf";
                var fileName = baseFileName + extension;
                bool fileSaved = false;
                int fileIndex = 0;

                while (!fileSaved)
                {
                    token.ThrowIfCancellationRequested();

                    var fileSaverResult = await FileSaver.Default.SaveAsync(fileName, stream);

                    if (fileSaverResult.IsSuccessful)
                    {
                        fileSaved = true;
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
                        throw new OperationCanceledException("User canceled the file save operation.");
                    }

                    // Giả lập xử lý
                    await Task.Delay(100, token);
                }
            };

            // Thực thi thao tác xuất với popup
            await ExecuteWithPopupAsync(
                exportOperation,
                successMessage: "Xuất file thành công",
                cancelMessage: "Bạn đã hủy lưu file.",
                errorMessage: "Có lỗi xảy ra trong quá trình xuất file");
        }

        /// <summary>
        /// Nhập dữ liệu từ file Excel với hỗ trợ hủy bỏ.
        /// </summary>
        public async Task<string> Import(string dataPath, string userName)
        {
            string resultMessage = "Hiện chưa chọn tập tin nào.";

            Func<CancellationToken, Task> importOperation = async (token) =>
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "com.microsoft.excel.xls", "org.openxmlformats.spreadsheetml.sheet" } },
                    { DevicePlatform.Android, new[] { "application/vnd.ms-excel", "application/vnd.openxmlformats.spreadsheetml.sheet" } },
                    { DevicePlatform.WinUI, new[] { ".xls", ".xlsx" } },
                    { DevicePlatform.MacCatalyst, new[] { "com.microsoft.excel.xls", "org.openxmlformats.spreadsheetml.sheet" } }
                });

                // Chọn file Excel (không thể hủy bỏ)
                var file = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Chọn file Excel",
                    FileTypes = customFileType
                });

                if (file != null)
                {
                    // Vì PickAsync không thể hủy, chỉ tiếp tục nếu đã chọn file
                    dataPath = file.FullPath;
                    using var stream = await file.OpenReadAsync();
                    Workbook workbook = new Workbook(stream);
                    Worksheet worksheet = workbook.Worksheets[0];

                    ObservableCollection<SanPham> products = new ObservableCollection<SanPham>();
                    int maxDataRow = worksheet.Cells.MaxDataRow;

                    for (int i = 1; i <= maxDataRow; i++)
                    {
                        token.ThrowIfCancellationRequested();

                        var maSanPham = worksheet.Cells[i, 0].StringValue;
                        var tenSanPham = worksheet.Cells[i, 1].StringValue;
                        var soLuong = worksheet.Cells[i, 2].IntValue;
                        var giaTien = worksheet.Cells[i, 3].FloatValue;

                        products.Add(new SanPham
                        {
                            MaSanPham = maSanPham,
                            Ten = tenSanPham,
                            SoLuong = soLuong,
                            GiaTien = giaTien
                        });

                        // Giả lập xử lý
                        await Task.Delay(100, token);
                    }

                    // Điều hướng tới trang danh sách sản phẩm với dữ liệu đã nhập
                    await Shell.Current.GoToAsync(nameof(View.DanhSachSP), new Dictionary<string, object>
                    {
                        { "DsSanPham", products },
                        { "UserName", userName }
                    });

                    resultMessage = "Import thành công.";
                }
            };

            // Thực thi thao tác nhập với popup
            await ExecuteWithPopupAsync(
                importOperation,
                successMessage: "Import thành công",
                cancelMessage: "Bạn đã hủy nhập file.",
                errorMessage: "Có lỗi xảy ra trong quá trình nhập file");

            return resultMessage;
        }

        /// <summary>
        /// Tạo file Excel mới với các tiêu đề và style đã định sẵn, hỗ trợ hủy bỏ.
        /// </summary>
        public async Task CreateNewFile()
        {
            Func<CancellationToken, Task> createFileOperation = async (token) =>
            {
                // Chọn thư mục lưu file (không thể hủy bỏ)
                var folder = await FolderPicker.PickAsync(default);

                if (folder != null && folder.Folder != null)
                {
                    var filePath = Path.Combine(folder.Folder.Path, "input.xlsx");

                    Workbook workbook = new Workbook();
                    Worksheet worksheet = workbook.Worksheets[0];

                    // Thiết lập độ rộng cột cho bố cục sạch sẽ
                    worksheet.Cells.SetColumnWidth(0, 20);
                    worksheet.Cells.SetColumnWidth(1, 20);
                    worksheet.Cells.SetColumnWidth(2, 12);
                    worksheet.Cells.SetColumnWidth(3, 12);

                    // Tiêu đề bảng
                    worksheet.Cells[0, 0].PutValue("Mã sản phẩm");
                    worksheet.Cells[0, 1].PutValue("Tên sản phẩm");
                    worksheet.Cells[0, 2].PutValue("Số lượng");
                    worksheet.Cells[0, 3].PutValue("Giá tiền");

                    // Áp dụng style in đậm cho tiêu đề
                    var headerStyle = workbook.CreateStyle();
                    headerStyle.Font.IsBold = true;
                    headerStyle.ForegroundColor = System.Drawing.Color.LightGray;
                    headerStyle.Pattern = BackgroundType.Solid;
                    headerStyle.HorizontalAlignment = TextAlignmentType.Center;

                    for (int j = 0; j <= 3; j++)
                    {
                        worksheet.Cells[0, j].SetStyle(headerStyle);
                    }

                    // Áp dụng border cho tiêu đề
                    var borderStyle = workbook.CreateStyle();
                    borderStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    borderStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                    borderStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                    borderStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;

                    for (int j = 0; j <= 3; j++)
                    {
                        worksheet.Cells[0, j].SetStyle(borderStyle);
                    }

                    // Giả lập xử lý
                    await Task.Delay(500, token);

                    // Lưu Workbook dưới dạng file Excel
                    workbook.Save(filePath, SaveFormat.Xlsx);
                }
                else
                {
                    throw new OperationCanceledException("Vui lòng chọn thư mục để lưu file.");
                }
            };

            // Thực thi thao tác tạo file với popup
            await ExecuteWithPopupAsync(
                createFileOperation,
                successMessage: "Tạo file thành công",
                cancelMessage: "Bạn đã hủy tạo file.",
                errorMessage: "Có lỗi xảy ra trong quá trình tạo file");
        }
    }
}
