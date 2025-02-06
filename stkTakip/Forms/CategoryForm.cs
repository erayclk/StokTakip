using System;
using System.Windows.Forms;
using stkTakip.Models;
using stkTakip.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace stkTakip.Forms
{
    public partial class CategoryForm : Form
    {
        private readonly ApplicationDbContext _context;
        private BindingSource bindingSource = new BindingSource();

        public CategoryForm(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private void CategoryForm_Load(object sender, EventArgs e)
        {
            LoadCategories();
        }

        private void LoadCategories()
        {
            var categories = _context.Categories
                .Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName,
                    c.Description,
                    c.IsActive
                })
                .ToList();

            bindingSource.DataSource = categories;
            dgvCategories.DataSource = bindingSource;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            var category = new Category
            {
                CategoryName = txtCategoryName.Text,
                Description = txtDescription.Text,
                IsActive = chkActive.Checked,
                CreatedDate = DateTime.Now
            };

            _context.Categories.Add(category);
            _context.SaveChanges();
            LoadCategories();
            ClearInputs();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvCategories.CurrentRow == null) return;
            if (!ValidateInput()) return;

            var categoryId = (int)dgvCategories.CurrentRow.Cells["CategoryId"].Value;
            var category = _context.Categories.Find(categoryId);

            if (category != null)
            {
                category.CategoryName = txtCategoryName.Text;
                category.Description = txtDescription.Text;
                category.IsActive = chkActive.Checked;
                category.ModifiedDate = DateTime.Now;

                _context.SaveChanges();
                LoadCategories();
                ClearInputs();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCategories.CurrentRow == null) return;

            if (MessageBox.Show("Kategoriyi silmek istediğinizden emin misiniz?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var categoryId = (int)dgvCategories.CurrentRow.Cells["CategoryId"].Value;
                var category = _context.Categories.Find(categoryId);

                if (category != null)
                {
                    _context.Categories.Remove(category);
                    _context.SaveChanges();
                    LoadCategories();
                    ClearInputs();
                }
            }
        }

        private void dgvCategories_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategories.CurrentRow != null)
            {
                var categoryId = (int)dgvCategories.CurrentRow.Cells["CategoryId"].Value;
                var category = _context.Categories.Find(categoryId);

                if (category != null)
                {
                    txtCategoryName.Text = category.CategoryName;
                    txtDescription.Text = category.Description;
                    chkActive.Checked = category.IsActive;
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("Kategori adı boş olamaz!");
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            txtCategoryName.Clear();
            txtDescription.Clear();
            chkActive.Checked = true;
        }
    }
}
