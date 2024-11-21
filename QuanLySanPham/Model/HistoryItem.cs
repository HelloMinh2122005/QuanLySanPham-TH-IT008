using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLySanPham.Model;

public class HistoryItem
{
    public SanPham SanPham { get; set; } = new();
    public string Action { get; set; } = "";
}