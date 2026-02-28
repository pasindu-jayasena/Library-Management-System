using SarasaviLibrary.DataAccess;
using SarasaviLibrary.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SarasaviLibrary.Forms
{
    /// <summary>
    /// User Registration Form — Process 6.
    /// Registers new Members (can borrow) and Visitors (reference only).
    /// Captures: UserNumber (auto), Name, Sex, NIC, Address, UserType.
    /// </summary>
    public class UserRegistrationForm : Form
    {
        private TextBox txtName = null!;
        private RadioButton rbMale = null!;
        private RadioButton rbFemale = null!;
        private TextBox txtNIC = null!;
        private TextBox txtAddress = null!;
        private ComboBox cmbUserType = null!;
        private Label lblUserNumber = null!;
        private DataGridView dgvUsers = null!;
        private Button btnRegister = null!;
        private Button btnClear = null!;

        public UserRegistrationForm()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void InitializeComponent()
        {
            this.Text = "👤 User Registration";
            this.Size = new Size(850, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;

            // ─── Header ───
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(26, 54, 93)
            };
            header.Controls.Add(new Label
            {
                Text = "User Registration",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 5),
                AutoSize = true
            });
            header.Controls.Add(new Label
            {
                Text = "Register members and visitors",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(180, 200, 220),
                Location = new Point(22, 32),
                AutoSize = true
            });
            this.Controls.Add(header);

            // ─── Content Layout ───
            TableLayoutPanel tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2, RowCount = 1,
                Padding = new Padding(10),
                BackColor = Color.Transparent
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.Controls.Add(tbl);
            tbl.BringToFront();

            // ─── Input Group ───
            GroupBox grpInput = new GroupBox
            {
                Text = "User Details",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 5, 0)
            };
            tbl.Controls.Add(grpInput, 0, 0);

            int y = 30;
            int inputX = 120;
            int inputWidth = 260;

            // User Number (auto-generated display)
            grpInput.Controls.Add(MakeLabel("User No:", 10, y));
            lblUserNumber = new Label
            {
                Location = new Point(inputX, y),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113)
            };
            UpdateUserNumber();
            grpInput.Controls.Add(lblUserNumber);
            y += 40;

            // Name
            grpInput.Controls.Add(MakeLabel("Name:", 10, y));
            txtName = new TextBox
            {
                Location = new Point(inputX, y),
                Size = new Size(inputWidth, 28),
                Font = new Font("Segoe UI", 10)
            };
            grpInput.Controls.Add(txtName);
            y += 40;

            // Sex
            grpInput.Controls.Add(MakeLabel("Sex:", 10, y));
            rbMale = new RadioButton
            {
                Text = "Male",
                Location = new Point(inputX, y),
                Font = new Font("Segoe UI", 9),
                Checked = true,
                AutoSize = true
            };
            rbFemale = new RadioButton
            {
                Text = "Female",
                Location = new Point(inputX + 80, y),
                Font = new Font("Segoe UI", 9),
                AutoSize = true
            };
            grpInput.Controls.Add(rbMale);
            grpInput.Controls.Add(rbFemale);
            y += 35;

            // NIC
            grpInput.Controls.Add(MakeLabel("NIC:", 10, y));
            txtNIC = new TextBox
            {
                Location = new Point(inputX, y),
                Size = new Size(inputWidth, 28),
                Font = new Font("Segoe UI", 10)
            };
            grpInput.Controls.Add(txtNIC);
            y += 40;

            // Address
            grpInput.Controls.Add(MakeLabel("Address:", 10, y));
            txtAddress = new TextBox
            {
                Location = new Point(inputX, y),
                Size = new Size(inputWidth, 60),
                Font = new Font("Segoe UI", 10),
                Multiline = true
            };
            grpInput.Controls.Add(txtAddress);
            y += 70;

            // User Type
            grpInput.Controls.Add(MakeLabel("User Type:", 10, y));
            cmbUserType = new ComboBox
            {
                Location = new Point(inputX, y),
                Size = new Size(150, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            cmbUserType.Items.AddRange(new object[] { "Member", "Visitor" });
            cmbUserType.SelectedIndex = 0;
            grpInput.Controls.Add(cmbUserType);
            y += 45;

            // Buttons
            btnRegister = new Button
            {
                Text = "Register User",
                Location = new Point(inputX, y),
                Size = new Size(130, 38),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;
            grpInput.Controls.Add(btnRegister);

            btnClear = new Button
            {
                Text = "Clear",
                Location = new Point(inputX + 140, y),
                Size = new Size(90, 38),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.Click += (s, e) => ClearForm();
            grpInput.Controls.Add(btnClear);

            // ─── Users Grid ───
            GroupBox grpUsers = new GroupBox
            {
                Text = "Registered Users",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                Margin = new Padding(5, 0, 0, 0)
            };
            tbl.Controls.Add(grpUsers, 1, 0);

            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9)
            };
            dgvUsers.Columns.Add("UserNumber", "User No.");
            dgvUsers.Columns.Add("Name", "Name");
            dgvUsers.Columns.Add("Sex", "Sex");
            dgvUsers.Columns.Add("NIC", "NIC");
            dgvUsers.Columns.Add("UserType", "Type");
            dgvUsers.Columns["Name"]!.Width = 120;
            grpUsers.Controls.Add(dgvUsers);
        }

        private void UpdateUserNumber()
        {
            lblUserNumber.Text = UserRepository.GetNextUserNumber();
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter the user's name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNIC.Text))
            {
                MessageBox.Show("Please enter the NIC number.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNIC.Focus();
                return;
            }

            User newUser = new User
            {
                UserNumber = UserRepository.GetNextUserNumber(),
                Name = txtName.Text.Trim(),
                Sex = rbMale.Checked ? "M" : "F",
                NIC = txtNIC.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                UserType = cmbUserType.SelectedItem!.ToString()!
            };

            UserRepository.AddUser(newUser);

            MessageBox.Show($"User registered successfully!\n" +
                            $"User Number: {newUser.UserNumber}\n" +
                            $"Name: {newUser.Name}\n" +
                            $"Type: {newUser.UserType}",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ClearForm();
            LoadUsers();
            UpdateUserNumber();
        }

        private void LoadUsers()
        {
            dgvUsers.Rows.Clear();
            List<User> users = UserRepository.GetAllUsers();
            foreach (var user in users)
            {
                dgvUsers.Rows.Add(user.UserNumber, user.Name, user.Sex,
                    user.NIC, user.UserType);
            }
        }

        private void ClearForm()
        {
            txtName.Clear();
            txtNIC.Clear();
            txtAddress.Clear();
            rbMale.Checked = true;
            cmbUserType.SelectedIndex = 0;
        }

        private static Label MakeLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y + 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };
        }
    }
}
