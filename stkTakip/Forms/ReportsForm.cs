using System;
using System.Windows.Forms;
using stkTakip.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace stkTakip.Forms
{
    public partial class ReportsForm : Form
    {
        private readonly ApplicationDbContext _context;

        public ReportsForm()
        {
            InitializeComponent();
            _context = Program.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        private void ReportsForm_Load(object sender, EventArgs e)
        {
            LoadLowStockReport();
        }

        private void LoadLowStockReport()
        {
            var lowStockProducts = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .Join(_context.Stocks,
                    p => p.ProductId,
                    s => s.ProductId,
                    (p, s) => new
                    {
                        p.ProductName,
                        p.SKU,
                        CategoryName = p.Category.CategoryName,
                        CurrentStock = s.Quantity,
                        p.MinimumStockLevel,
                        Status = s.Quantity <= p.MinimumStockLevel ? "Kritik" : "Normal"
                    })
                .OrderBy(x => x.Status)
                .ThenBy(x => x.ProductName)
                .ToList();

            dgvLowStock.DataSource = lowStockProducts;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadLowStockReport();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV Dosyası|*.csv";
                sfd.Title = "Raporu Kaydet";
                
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var lines = new System.Collections.Generic.List<string>();
                        string[] columnNames = new string[dgvLowStock.Columns.Count];
                        
                        // Add header
                        for (int i = 0; i < dgvLowStock.Columns.Count; i++)
                        {
                            columnNames[i] = dgvLowStock.Columns[i].HeaderText;
                        }
                        lines.Add(string.Join(",", columnNames));

                        // Add rows
                        foreach (DataGridViewRow row in dgvLowStock.Rows)
                        {
                            string[] fields = new string[row.Cells.Count];
                            for (int i = 0; i < row.Cells.Count; i++)
                            {
                                fields[i] = row.Cells[i].Value?.ToString() ?? "";
                            }
                            lines.Add(string.Join(",", fields));
                        }

                        System.IO.File.WriteAllLines(sfd.FileName, lines);
                        MessageBox.Show("Rapor başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
