using System;
using System.Windows.Forms;
using stkTakip.Models;
using stkTakip.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace stkTakip.Forms
{
    public partial class StockForm : Form
    {
        private readonly ApplicationDbContext _context;
        private BindingSource bindingSource = new BindingSource();

        public StockForm()
        {
            InitializeComponent();
            _context = Program.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        private void StockForm_Load(object sender, EventArgs e)
        {
            LoadProducts();
            LoadStockMovements();
        }

        private void LoadProducts()
        {
            cmbProduct.DataSource = _context.Products
                .Where(p => p.IsActive)
                .Select(p => new { p.ProductId, DisplayName = $"{p.ProductName} ({p.SKU})" })
                .ToList();
            cmbProduct.DisplayMember = "DisplayName";
            cmbProduct.ValueMember = "ProductId";
        }

        private void LoadStockMovements()
        {
            var stocks = _context.Stocks
                .Include(s => s.Product)
                .Select(s => new
                {
                    s.StockId,
                    ProductName = s.Product.ProductName,
                    s.Product.SKU,
                    s.Quantity,
                    s.Location,
                    s.BatchNumber,
                    s.ExpiryDate,
                    s.LastUpdated
                })
                .OrderByDescending(s => s.LastUpdated)
                .ToList();

            bindingSource.DataSource = stocks;
            dgvStock.DataSource = bindingSource;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            var stock = new Stock
            {
                ProductId = (int)cmbProduct.SelectedValue,
                Quantity = int.Parse(txtQuantity.Text),
                Location = txtLocation.Text,
                BatchNumber = txtBatchNumber.Text,
                ExpiryDate = dtpExpiryDate.Checked ? dtpExpiryDate.Value : (DateTime?)null,
                LastUpdated = DateTime.Now
            };

            _context.Stocks.Add(stock);
            _context.SaveChanges();
            LoadStockMovements();
            ClearInputs();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvStock.CurrentRow == null) return;
            if (!ValidateInput()) return;

            var stockId = (int)dgvStock.CurrentRow.Cells["StockId"].Value;
            var stock = _context.Stocks.Find(stockId);

            if (stock != null)
            {
                stock.ProductId = (int)cmbProduct.SelectedValue;
                stock.Quantity = int.Parse(txtQuantity.Text);
                stock.Location = txtLocation.Text;
                stock.BatchNumber = txtBatchNumber.Text;
                stock.ExpiryDate = dtpExpiryDate.Checked ? dtpExpiryDate.Value : (DateTime?)null;
                stock.LastUpdated = DateTime.Now;

                _context.SaveChanges();
                LoadStockMovements();
                ClearInputs();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStock.CurrentRow == null) return;

            if (MessageBox.Show("Stok hareketini silmek istediğinizden emin misiniz?", "Onay", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var stockId = (int)dgvStock.CurrentRow.Cells["StockId"].Value;
                var stock = _context.Stocks.Find(stockId);

                if (stock != null)
                {
                    _context.Stocks.Remove(stock);
                    _context.SaveChanges();
                    LoadStockMovements();
                    ClearInputs();
                }
            }
        }

        private void dgvStock_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStock.CurrentRow != null)
            {
                var stockId = (int)dgvStock.CurrentRow.Cells["StockId"].Value;
                var stock = _context.Stocks.Find(stockId);

                if (stock != null)
                {
                    cmbProduct.SelectedValue = stock.ProductId;
                    txtQuantity.Text = stock.Quantity.ToString();
                    txtLocation.Text = stock.Location;
                    txtBatchNumber.Text = stock.BatchNumber;
                    if (stock.ExpiryDate.HasValue)
                    {
                        dtpExpiryDate.Checked = true;
                        dtpExpiryDate.Value = stock.ExpiryDate.Value;
                    }
                    else
                    {
                        dtpExpiryDate.Checked = false;
                    }
                }
            }
        }

        private bool ValidateInput()
        {
            if (cmbProduct.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen bir ürün seçin!");
                return false;
            }

            if (!int.TryParse(txtQuantity.Text, out _))
            {
                MessageBox.Show("Geçerli bir miktar giriniz!");
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            cmbProduct.SelectedIndex = -1;
            txtQuantity.Clear();
            txtLocation.Clear();
            txtBatchNumber.Clear();
            dtpExpiryDate.Checked = false;
        }
    }
}
