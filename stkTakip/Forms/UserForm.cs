using System;
using System.Windows.Forms;
using stkTakip.Models;
using stkTakip.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace stkTakip.Forms
{
    public partial class UserForm : Form
    {
        private readonly ApplicationDbContext _context;
        private BindingSource bindingSource = new BindingSource();

        public UserForm()
        {
            InitializeComponent();
            _context = Program.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            LoadRoles();
            LoadUsers();
        }

        private void LoadRoles()
        {
            cmbRole.Items.AddRange(new[] { "Admin", "User" });
            cmbRole.SelectedIndex = 1; // Default to User
        }

        private void LoadUsers()
        {
            var users = _context.Users
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Role,
                    u.IsActive,
                    u.LastLogin
                })
                .ToList();

            bindingSource.DataSource = users;
            dgvUsers.DataSource = bindingSource;
            
            if (dgvUsers.Columns["PasswordHash"] != null)
                dgvUsers.Columns["PasswordHash"].Visible = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            var user = new User
            {
                Username = txtUsername.Text,
                PasswordHash = HashPassword(txtPassword.Text),
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                Email = txtEmail.Text,
                Role = cmbRole.Text,
                IsActive = chkActive.Checked,
                CreatedDate = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            LoadUsers();
            ClearInputs();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null) return;
            if (!ValidateInput(true)) return;

            var userId = (int)dgvUsers.CurrentRow.Cells["UserId"].Value;
            var user = _context.Users.Find(userId);

            if (user != null)
            {
                user.Username = txtUsername.Text;
                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                    user.PasswordHash = HashPassword(txtPassword.Text);
                user.FirstName = txtFirstName.Text;
                user.LastName = txtLastName.Text;
                user.Email = txtEmail.Text;
                user.Role = cmbRole.Text;
                user.IsActive = chkActive.Checked;

                _context.SaveChanges();
                LoadUsers();
                ClearInputs();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null) return;

            if (MessageBox.Show("Kullanıcıyı silmek istediğinizden emin misiniz?", "Onay", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var userId = (int)dgvUsers.CurrentRow.Cells["UserId"].Value;
                var user = _context.Users.Find(userId);

                if (user != null)
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                    LoadUsers();
                    ClearInputs();
                }
            }
        }

        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow != null)
            {
                var userId = (int)dgvUsers.CurrentRow.Cells["UserId"].Value;
                var user = _context.Users.Find(userId);

                if (user != null)
                {
                    txtUsername.Text = user.Username;
                    txtPassword.Text = string.Empty; // Don't show password
                    txtFirstName.Text = user.FirstName;
                    txtLastName.Text = user.LastName;
                    txtEmail.Text = user.Email;
                    cmbRole.Text = user.Role;
                    chkActive.Checked = user.IsActive;
                }
            }
        }

        private bool ValidateInput(bool isUpdate = false)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Kullanıcı adı boş olamaz!");
                return false;
            }

            if (!isUpdate && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Şifre boş olamaz!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Ad boş olamaz!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Soyad boş olamaz!");
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtEmail.Clear();
            cmbRole.SelectedIndex = 1;
            chkActive.Checked = true;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
