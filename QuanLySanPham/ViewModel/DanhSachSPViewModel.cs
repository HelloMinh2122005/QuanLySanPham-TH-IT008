using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLySanPham.Model;
using QuanLySanPham.View;
using System.Collections.ObjectModel;
using Aspose.Cells;
using static iText.StyledXmlParser.Jsoup.Select.NodeFilter;
using System.IO;


namespace QuanLySanPham.ViewModel;

public partial class DanhSachSPViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<SanPham> dsSanPham;

    [ObservableProperty]
    string title;

    [ObservableProperty]
    private int stt;

    [ObservableProperty]
    float thanhTien;

    private SanPham selectedSanPham;
    public float GiaTientmp;
    public string UserName;
    private ObservableCollection<SanPham> DsSanPhamThem;
    private ObservableCollection<SanPham> DsSanPhamSua;
    private ObservableCollection<SanPham> DsSanPhamXoa;

    public DanhSachSPViewModel()
    {
        dsSanPham = new ObservableCollection<SanPham>();
        Stt = 0;
        title = "Chào ";
        UserName = "";
        thanhTien = 0;
        GiaTientmp = 0;
        selectedSanPham = new SanPham();
        DsSanPhamThem = new ObservableCollection<SanPham>();
        DsSanPhamSua = new ObservableCollection<SanPham>();
        DsSanPhamXoa = new ObservableCollection<SanPham>();
    }

    private void AddSPifEmpty()
    {
        DsSanPham.Add(new SanPham
        { MaSanPham = "SP01", Ten = "Sản phẩm 1", SoLuong = 1, GiaTien = 1000 }
        );
        DsSanPham.Add(new SanPham
        { MaSanPham = "SP02", Ten = "Sản phẩm 2", SoLuong = 2, GiaTien = 2000 }
        );
        DsSanPham.Add(new SanPham
        { MaSanPham = "SP03", Ten = "Sản phẩm 3", SoLuong = 3, GiaTien = 3000 }
        );
        DsSanPham.Add(new SanPham
        { MaSanPham = "SP04", Ten = "Sản phẩm 4", SoLuong = 4, GiaTien = 4000 }
        );
        DsSanPham.Add(new SanPham
        { MaSanPham = "SP05", Ten = "Sản phẩm 5", SoLuong = 5, GiaTien = 5000 }
        );
    }

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("nochange"))
        {
            return;
        }
        if (query.ContainsKey("DsSanPham") && query.ContainsKey("UserName"))
        {
            UserName = query["UserName"].ToString() ?? "";
            DsSanPham = query["DsSanPham"] as ObservableCollection<SanPham> ?? new ObservableCollection<SanPham>();
            ThanhTien = 0;
            if (DsSanPham.Count == 0)
                AddSPifEmpty();
            foreach (var item in DsSanPham)
                ThanhTien += item.TongTien;
            query.Remove("DsSanPham");
            return;
        }
        if (query.ContainsKey("add"))
        {
            Stt++;
            var newSP = query["add"] as SanPham ?? new SanPham();
            DsSanPham.Add(newSP);
            DsSanPhamThem.Add(newSP);
            ThanhTien += newSP.TongTien;
            query.Remove("add");
            return;
        }
        if (query.ContainsKey("edit"))
        {
            var editedSP = query["edit"] as SanPham ?? new SanPham();

            var existingSP = DsSanPham.FirstOrDefault(sp => sp.MaSanPham == editedSP.MaSanPham);
            if (existingSP != null)
            {
                ThanhTien -= existingSP.TongTien;
                ThanhTien += editedSP.TongTien;

                var index = DsSanPham.IndexOf(existingSP);
                DsSanPham[index] = editedSP;
            }

            query.Remove("edit");
            selectedSanPham = null;
            return;
        }
        if (query.ContainsKey("DSRecover"))
        {
            var dsToRecover = query["DSRecover"] as ObservableCollection<HistoryItem>; //
            if (dsToRecover != null)
            {
                foreach (var historyItem in dsToRecover)
                {
                    var sanPham = historyItem.SanPham;
                    var action = historyItem.Action;

                    switch (action)
                    {
                        case "Thêm":
                            var productToRemove = DsSanPham.FirstOrDefault(sp => sp.MaSanPham == sanPham.MaSanPham);
                            if (productToRemove != null)
                            {
                                DsSanPham.Remove(productToRemove);
                                ThanhTien -= productToRemove.TongTien;
                            }

                            var productInThem = DsSanPhamThem.FirstOrDefault(sp => sp.MaSanPham == sanPham.MaSanPham);
                            if (productInThem != null)
                            {
                                DsSanPhamThem.Remove(productInThem);
                            }
                            break;

                        case "Sửa":
                            var existingSP = DsSanPham.FirstOrDefault(sp => sp.MaSanPham == sanPham.MaSanPham);
                            if (existingSP != null)
                            {
                                ThanhTien -= existingSP.TongTien;
                                DsSanPham.Remove(existingSP);
                                DsSanPham.Add(sanPham);
                                ThanhTien += sanPham.TongTien;
                            }
                            break;

                        case "Xóa":
                            DsSanPham.Add(sanPham);
                            ThanhTien += sanPham.TongTien;
                            break;
                    }
                }
            }
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
        if (selectedSanPham == null || string.IsNullOrWhiteSpace(selectedSanPham.MaSanPham))
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng chọn sản phẩm để xóa", "OK");
            return;
        }
        try
        {
            ThanhTien -= selectedSanPham.TongTien;
            DsSanPhamXoa.Add(selectedSanPham);
            DsSanPham.Remove(selectedSanPham);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Lỗi", ex.Message, "OK");
        }
        selectedSanPham = null;
    }

    [RelayCommand]
    public async Task Edit()
    {
        if (selectedSanPham == null || selectedSanPham.MaSanPham == "")
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng chọn sản phẩm để sửa", "OK");
            return;
        }

        SanPham sanPhamCopy = new SanPham 
        { 
            MaSanPham = selectedSanPham.MaSanPham,
            Ten = selectedSanPham.Ten,
            SoLuong = selectedSanPham.SoLuong,
            GiaTien = selectedSanPham.GiaTien
        };

        DsSanPhamSua.Add(new SanPham
        {
            MaSanPham = selectedSanPham.MaSanPham,
            Ten = selectedSanPham.Ten,
            SoLuong = selectedSanPham.SoLuong,
            GiaTien = selectedSanPham.GiaTien
        });

        await Shell.Current.GoToAsync(nameof(EditSanPham), new Dictionary<string, object>
        {
            {"sanphamPara", sanPhamCopy }
        });

        GiaTientmp = selectedSanPham.TongTien;
    }

    [RelayCommand]
    public void Select(SanPham selectedSP)
    {
        selectedSanPham = selectedSP;
    }

    // ...

    [RelayCommand]
    public async Task Export()
    {
        var finalRes = new FinalRes
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
            worksheet.Cells[0, 2].PutValue(finalRes.TenNguoiTao);
            worksheet.Cells[1, 1].PutValue("Ngày tạo:");
            worksheet.Cells[1, 2].PutValue(finalRes.NgayTao.ToString("dd/MM/yyyy"));
            worksheet.Cells[2, 1].PutValue("Tổng giá trị hóa đơn:");
            worksheet.Cells[2, 2].PutValue(finalRes.TongTien.ToString());
            worksheet.Cells[2, 3].PutValue("Đơn vị: VNĐ");

            worksheet.Cells[4, 0].PutValue("STT");
            worksheet.Cells[4, 1].PutValue("Mã sản phẩm");
            worksheet.Cells[4, 2].PutValue("Tên sản phẩm");
            worksheet.Cells[4, 3].PutValue("Số lượng");
            worksheet.Cells[4, 4].PutValue("Giá tiền");
            worksheet.Cells[4, 5].PutValue("Tổng tiền");

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
            var baseFileName = "finalRes";
            var extension = ".pdf";
            var fileName = baseFileName + extension;
            bool fileSaved = false;
            int fileIndex = 0;

            while (!fileSaved)
            {
                var fileSaverResult = await FileSaver.Default.SaveAsync(fileName, stream);

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
    }

    [RelayCommand]
    async Task ViewHis() 
    {
        await Shell.Current.GoToAsync(nameof(ViewHistory), new Dictionary<string, object>
        {
            {"DSSPXoa", DsSanPhamXoa },
            {"DSSPThem", DsSanPhamThem },
            {"DSSPSua", DsSanPhamSua }
        });
    }
}