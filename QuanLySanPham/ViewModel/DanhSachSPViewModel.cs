using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLySanPham.Model;
using QuanLySanPham.View;
using System.Collections.ObjectModel;

namespace QuanLySanPham.ViewModel;

public partial class DanhSachSPViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<SanPham> dsSanPham;

    [ObservableProperty]
    float thanhTien;

    private SanPham selectedSanPham;
    public float GiaTientmp;
    private ObservableCollection<SanPham> DsSanPhamThem;
    private ObservableCollection<SanPham> DsSanPhamSua;
    private ObservableCollection<SanPham> DsSanPhamXoa;

    public DanhSachSPViewModel()
    {
        dsSanPham = new ObservableCollection<SanPham>();
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

    async void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("nochange"))
        {
            return;
        }
        if (query.ContainsKey("DsSanPham"))
        {
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
            var newSP = query["add"] as SanPham ?? new SanPham();
            var existingSP = DsSanPham.FirstOrDefault(sp => sp.MaSanPham == newSP.MaSanPham);
            if (existingSP != null)
            {
                if (existingSP != null)
                {
                    bool update = await Shell.Current.DisplayAlert("Thông báo", "Sản phẩm này đã tồn tại trong danh sách, bạn có muốn cập nhật lại thông tin không?", "Có", "Không");
                    if (update)
                    {
                        ThanhTien -= existingSP.TongTien;
                        ThanhTien += newSP.TongTien;

                        DsSanPhamThem.Add(newSP);
                        var index = DsSanPham.IndexOf(existingSP);
                        DsSanPham[index] = newSP;
                    } 
                    query.Remove("add");
                    return;
                }

            }
            DsSanPham.Insert(0, newSP);
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
            query.Remove("DSRecover");
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
                                DsSanPham.Insert(0, sanPham);
                                ThanhTien += sanPham.TongTien;
                            }
                            break;

                        case "Xóa":
                            DsSanPham.Insert(0, sanPham);
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

    [RelayCommand]
    public async Task Export()
    {
        if (DsSanPham.Count == 0)
        {
            await Shell.Current.DisplayAlert("Thông báo", "Không có sản phẩm nào để xuất file", "OK");
            return;
        }

        HoaDon hd = new HoaDon
        {
            DsSanPham = DsSanPham,
            TongTien = ThanhTien,
        };
        
        await Shell.Current.GoToAsync($"{nameof(SaveFilePage)}", new Dictionary<string, object>
        {
            {"HoaDon", hd }
        });
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

    [RelayCommand]
    async Task Sort()
    {
        if (DsSanPham.Count == 0)
        {
            await Shell.Current.DisplayAlert("Thông báo", "Không có sản phẩm nào để sắp xếp", "OK");
            return;
        }
        DsSanPham = new ObservableCollection<SanPham>(DsSanPham.OrderByDescending(sp => sp.TongTien));
        await Shell.Current.DisplayAlert("Thông báo", "Sản phẩm đã được sắp xếp theo giá giảm dần", "OK");
    }
}