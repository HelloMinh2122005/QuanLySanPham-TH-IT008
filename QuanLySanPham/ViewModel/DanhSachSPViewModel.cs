using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLySanPham.Model;
using QuanLySanPham.View;
using System.Collections.ObjectModel;
using Aspose.Cells;


namespace QuanLySanPham.ViewModel;

[QueryProperty("UserName", "UserName")]
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

    [ObservableProperty]
    int daThem;

    [ObservableProperty]
    int daSua;

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
        if (query.ContainsKey("DsSanPham"))
        {
            DsSanPham = query["DsSanPham"] as ObservableCollection<SanPham> ?? new ObservableCollection<SanPham>();
            ThanhTien = 0;
            DaThem = 0;
            DaSua = 0;
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
            DaThem++;
            var newSP = query["add"] as SanPham ?? new SanPham();
            DsSanPham.Add(newSP);
            DsSanPhamThem.Add(newSP);
            ThanhTien += newSP.TongTien;
            query.Remove("add");
            return;
        }
        if (query.ContainsKey("edit"))
        {
            DaSua++;
            var editSP = query["edit"] as SanPham ?? new SanPham();
            var index = DsSanPham.IndexOf(selectedSanPham);
            selectedSanPham.MaSanPham = "";
            DsSanPham[index] = editSP;
            DsSanPhamSua.Add(editSP);
            ThanhTien -= GiaTientmp;
            GiaTientmp = 0;
            ThanhTien += editSP.TongTien;
            query.Remove("edit");
            return;
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
        try
        {
            DsSanPhamXoa.Add(selectedSanPham);
            DsSanPham.Remove(selectedSanPham);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Lỗi", ex.Message, "OK");
        }
        selectedSanPham.MaSanPham = "";
    }

    [RelayCommand]
    public async Task Edit()
    {
        if (selectedSanPham == null || selectedSanPham.MaSanPham == "")
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
            TenNguoiTao = UserName,
            NgayTao = DateTime.Now,
            TongTien = ThanhTien,
            DsSanPham = DsSanPham
        };

        var folder = await FolderPicker.PickAsync(default);

        if (folder != null && folder.Folder != null)
        {
            var filePath = Path.Combine(folder.Folder.Path, "finalRes.pdf");
            try
            {
                Workbook workbook = new Workbook();
                Worksheet worksheet = workbook.Worksheets[0];

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

                var topHeaderStyle = workbook.CreateStyle();
                topHeaderStyle.Font.IsBold = true;
                topHeaderStyle.HorizontalAlignment = TextAlignmentType.Left;

                worksheet.Cells[0, 1].SetStyle(topHeaderStyle);
                worksheet.Cells[1, 1].SetStyle(topHeaderStyle);
                worksheet.Cells[2, 1].SetStyle(topHeaderStyle);
                worksheet.Cells[2, 3].SetStyle(topHeaderStyle);

                workbook.Save(filePath, SaveFormat.Pdf);

                await Shell.Current.DisplayAlert("Thông báo", "Xuất file thành công", "OK");
            }
            catch 
            {
                await Shell.Current.DisplayAlert("Lỗi", "File bạn nhập bị lỗi, có thể do không đúng định dạng hoặc không đúng kiểu dữ liệu", "OK");
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("Thông báo", "Vui lòng chọn thư mục để lưu file", "OK");
        }
    }

    [RelayCommand]
    async Task ViewHis()
    {
        await Shell.Current.GoToAsync(nameof(ViewHistory), new Dictionary<string, object>
        {
            {"DSSPXoa", DsSanPhamXoa },
            {"DSSPThem", DsSanPhamThem },
            {"DsSPSua", DsSanPhamSua }
        });
    }
}