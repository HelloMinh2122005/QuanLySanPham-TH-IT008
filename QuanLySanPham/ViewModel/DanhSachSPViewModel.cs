using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLySanPham.Model;
using QuanLySanPham.View;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection.Metadata;
using iText.Layout;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using System.Text.Json;
using iText.Layout.Element;
using Aspose.Cells.Utility;
using Aspose.Cells;


namespace QuanLySanPham.ViewModel;

public partial class DanhSachSPViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<SanPham> dsSanPham;

    private SanPham selectedSanPham;

    [ObservableProperty]
    private int stt;

    [ObservableProperty]
    float thanhTien;

    public float GiaTientmp;

    public DanhSachSPViewModel()
    {
        dsSanPham = new ObservableCollection<SanPham>();
        Stt = 0;
        thanhTien = 0;
        GiaTientmp = 0;
        selectedSanPham = new SanPham();

        dsSanPham.Add(new SanPham
        {
            MaSanPham = "SP001",
            Ten = "Bút chì",
            GiaTien = 5000,
            SoLuong = 10
        });

        dsSanPham.Add(new SanPham
        {
            MaSanPham = "SP002",
            Ten = "Bút bi",
            GiaTien = 7000,
            SoLuong = 15
        });

        dsSanPham.Add(new SanPham
        {
            MaSanPham = "SP003",
            Ten = "Thước kẻ",
            GiaTien = 3000,
            SoLuong = 20
        });

        dsSanPham.Add(new SanPham
        {
            MaSanPham = "SP004",
            Ten = "Tẩy",
            GiaTien = 2000,
            SoLuong = 25
        });

        dsSanPham.Add(new SanPham
        {
            MaSanPham = "SP005",
            Ten = "Sổ tay",
            GiaTien = 15000,
            SoLuong = 5
        });

        dsSanPham.Add(new SanPham
        {
            MaSanPham = "SP006",
            Ten = "Bìa kẹp",
            GiaTien = 12000,
            SoLuong = 8
        });

        dsSanPham.Add(new SanPham
        {
            MaSanPham = "SP007",
            Ten = "Kéo",
            GiaTien = 10000,
            SoLuong = 12
        });

        dsSanPham.Add(new SanPham
        {
            MaSanPham = "SP008",
            Ten = "Bút lông",
            GiaTien = 8000,
            SoLuong = 18
        });

        dsSanPham.Add(new SanPham
        {
            MaSanPham = "SP009",
            Ten = "Giấy A4",
            GiaTien = 50000,
            SoLuong = 2
        });

        dsSanPham.Add(new SanPham
        {
            MaSanPham = "SP010",
            Ten = "Bút dạ quang",
            GiaTien = 6000,
            SoLuong = 10
        });

        ThanhTien = 860000;
    }

    void IQueryAttributable.ApplyQueryAttributes(System.Collections.Generic.IDictionary<string, object> query)
    {
        if (query.ContainsKey("add"))
        {
            Stt++;
            var newSP = query["add"] as SanPham ?? new SanPham();
            DsSanPham.Add(newSP);
            ThanhTien += newSP.TongTien;
            query.Remove("add");
        }
        if (query.ContainsKey("edit"))
        {
            var editSP = query["edit"] as SanPham ?? new SanPham();
            var index = DsSanPham.IndexOf(selectedSanPham);
            DsSanPham[index] = editSP;
            ThanhTien -= GiaTientmp;
            GiaTientmp = 0;
            ThanhTien += editSP.TongTien;
            query.Remove("edit");
        }
    }

    [RelayCommand]
    async Task Add()
    {
        await Shell.Current.GoToAsync(nameof(AddSanPham));
    }

    [RelayCommand]
    async Task Del()
    {
        if (selectedSanPham == null || selectedSanPham.MaSanPham == "")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng chọn sản phẩm để xóa", "OK");
            return;
        }
        ThanhTien -= selectedSanPham.TongTien;
        DsSanPham.Remove(selectedSanPham);
    }

    [RelayCommand]
    public async Task Edit()
    {
        if (selectedSanPham.MaSanPham == "")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng chọn sản phẩm để sửa", "OK");
            return;
        }
        await Shell.Current.GoToAsync(nameof(EditSanPham), new Dictionary<string, object>
        {
            {"sanphamPara", selectedSanPham }
        });
        
        GiaTientmp = selectedSanPham.TongTien;
    }

    [RelayCommand]
    public void Select(SanPham selectedSP)
    {
        selectedSanPham = selectedSP;
    }

    [RelayCommand]
    public async Task Export()
    {
        var finalRes = new FinalRes
        {
            TenNguoiTao = "Nguyễn Văn A",
            NgayTao = DateTime.Now,
            TongTien = ThanhTien,
            DsSanPham = DsSanPham
        };

        var folder = await FolderPicker.PickAsync(default);

        if (folder != null && folder.Folder != null)
        {
            var filePath = System.IO.Path.Combine(folder.Folder.Path, "finalRes.pdf");
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

                // Header information
                worksheet.Cells[0, 1].PutValue("Tên người tạo:");
                worksheet.Cells[0, 2].PutValue(finalRes.TenNguoiTao);
                worksheet.Cells[1, 1].PutValue("Ngày tạo:");
                worksheet.Cells[1, 2].PutValue(finalRes.NgayTao.ToString("dd/MM/yyyy"));
                worksheet.Cells[2, 1].PutValue("Tổng giá trị hóa đơn:");
                worksheet.Cells[2, 2].PutValue(finalRes.TongTien.ToString());

                // Table headers
                worksheet.Cells[4, 0].PutValue("STT");
                worksheet.Cells[4, 1].PutValue("Mã sản phẩm");
                worksheet.Cells[4, 2].PutValue("Tên sản phẩm");
                worksheet.Cells[4, 3].PutValue("Số lượng");
                worksheet.Cells[4, 4].PutValue("Giá tiền");
                worksheet.Cells[4, 5].PutValue("Tổng tiền");

                // Product details
                int rowIndex = 5;
                foreach (var sp in finalRes.DsSanPham)
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

                // Apply bold style for headers
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

                // Apply border style to the table
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

                // Apply styling for header cells at the top (creator, date, total)
                var topHeaderStyle = workbook.CreateStyle();
                topHeaderStyle.Font.IsBold = true;
                topHeaderStyle.HorizontalAlignment = TextAlignmentType.Left;

                worksheet.Cells[0, 0].SetStyle(topHeaderStyle);
                worksheet.Cells[1, 0].SetStyle(topHeaderStyle);
                worksheet.Cells[2, 0].SetStyle(topHeaderStyle);

                // Save the workbook as PDF
                workbook.Save(filePath, SaveFormat.Pdf);

                await Shell.Current.DisplayAlert("Thông báo", "Xuất file thành công", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred while creating the PDF: {ex.Message}", "OK");
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng chọn thư mục để lưu file", "OK");
        }
    }
}