using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace stkTakip.Forms
{
    public partial class MainForm : Form
    {
        private readonly IServiceProvider _serviceProvider;

        public MainForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Initialize main form components
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            // Open Products Form
            using (var productForm = _serviceProvider.GetRequiredService<ProductForm>())
            {
                productForm.ShowDialog();
            }
        }

        private void btnCategories_Click(object sender, EventArgs e)
        {
            // Open Categories Form
            using (var categoryForm = _serviceProvider.GetRequiredService<CategoryForm>())
            {
                categoryForm.ShowDialog();
            }
        }

        private void btnStock_Click(object sender, EventArgs e)
        {
            // Open Stock Form
            using (var stockForm = _serviceProvider.GetRequiredService<StockForm>())
            {
                stockForm.ShowDialog();
            }
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            // Open Reports Form
            using (var reportsForm = _serviceProvider.GetRequiredService<ReportsForm>())
            {
                reportsForm.ShowDialog();
            }
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            // Open Users Form
            using (var userForm = _serviceProvider.GetRequiredService<UserForm>())
            {
                userForm.ShowDialog();
            }
        }
    }
}
