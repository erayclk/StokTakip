using System;
using System.Windows.Forms;
using stkTakip.Models;
using stkTakip.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace stkTakip.Forms
{
    public partial class ProductForm : Form
    {
        private readonly ApplicationDbContext _context;
        private BindingSource bindingSource = new BindingSource();

        public ProductForm(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private void ProductForm_Load(object sender, EventArgs e)
        {
            LoadCategories();
            LoadProducts();
        }

        private void LoadCategories()
        {
            cmbCategory.DataSource = _context.Categories.ToList();
            cmbCategory.DisplayMember = "CategoryName";
            cmbCategory.ValueMember = "CategoryId";
        }

        private void LoadProducts()
        {
            var products = _context.Products
                .Include(p => p.Category)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.SKU,
                    p.Barcode,
                    CategoryName = p.Category.CategoryName,
                    p.UnitPrice,
                    p.MinimumStockLevel,
                    p.IsActive
                })
                .ToList();

            bindingSource.DataSource = products;
            dgvProducts.DataSource = bindingSource;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            var product = new Product
            {
                ProductName = txtProductName.Text,
                SKU = txtSKU.Text,
                Barcode = txtBarcode.Text,
                CategoryId = (int)cmbCategory.SelectedValue,
                UnitPrice = decimal.Parse(txtUnitPrice.Text),
                MinimumStockLevel = int.Parse(txtMinStock.Text),
                IsActive = chkActive.Checked,
                CreatedDate = DateTime.Now
            };

            _context.Products.Add(product);
            _context.SaveChanges();
            LoadProducts();
            ClearInputs();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null) return;
            if (!ValidateInput()) return;

            var productId = (int)dgvProducts.CurrentRow.Cells["ProductId"].Value;
            var product = _context.Products.Find(productId);

            if (product != null)
            {
                product.ProductName = txtProductName.Text;
                product.SKU = txtSKU.Text;
                product.Barcode = txtBarcode.Text;
                product.CategoryId = (int)cmbCategory.SelectedValue;
                product.UnitPrice = decimal.Parse(txtUnitPrice.Text);
                product.MinimumStockLevel = int.Parse(txtMinStock.Text);
                product.IsActive = chkActive.Checked;
                product.ModifiedDate = DateTime.Now;

                _context.SaveChanges();
                LoadProducts();
                ClearInputs();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null) return;

            if (MessageBox.Show("Ürünü silmek istediğinizden emin misiniz?", "Onay", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var productId = (int)dgvProducts.CurrentRow.Cells["ProductId"].Value;
                var product = _context.Products.Find(productId);

                if (product != null)
                {
                    _context.Products.Remove(product);
                    _context.SaveChanges();
                    LoadProducts();
                    ClearInputs();
                }
            }
        }

        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow != null)
            {
                var productId = (int)dgvProducts.CurrentRow.Cells["ProductId"].Value;
                var product = _context.Products.Find(productId);

                if (product != null)
                {
                    txtProductName.Text = product.ProductName;
                    txtSKU.Text = product.SKU;
                    txtBarcode.Text = product.Barcode;
                    cmbCategory.SelectedValue = product.CategoryId;
                    txtUnitPrice.Text = product.UnitPrice.ToString();
                    txtMinStock.Text = product.MinimumStockLevel.ToString();
                    chkActive.Checked = product.IsActive;
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Ürün adı boş olamaz!");
                return false;
            }

            if (cmbCategory.SelectedValue == null)
            {
                MessageBox.Show("Lütfen bir kategori seçiniz!");
                return false;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out _))
            {
                MessageBox.Show("Geçerli bir fiyat giriniz!");
                return false;
            }

            if (!int.TryParse(txtMinStock.Text, out _))
            {
                MessageBox.Show("Geçerli bir minimum stok seviyesi giriniz!");
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            txtProductName.Clear();
            txtSKU.Clear();
            txtBarcode.Clear();
            txtUnitPrice.Clear();
            txtMinStock.Clear();
            chkActive.Checked = true;
            cmbCategory.SelectedIndex = -1;
        }
    }
}
