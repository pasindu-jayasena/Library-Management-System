using SarasaviLibrary.DataAccess;
using SarasaviLibrary.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SarasaviLibrary.Forms
{
    /// <summary>
    /// Reservation Form — Process 3.
    /// Allows members to reserve a book title when all copies are loaned out.
    /// Reservations are per Title (BookNumber), not per Copy.
    /// </summary>
    public class ReservationForm : Form
    {
        private TextBox txtUserNumber = null!;
        private Label lblUserInfo = null!;
        private TextBox txtBookSearch = null!;
        private DataGridView dgvBooks = null!;
        private DataGridView dgvReservations = null!;
        private Button btnReserve = null!;
        private Label lblBookInfo = null!;

        private string selectedBookNumber = "";

        public ReservationForm()
        {
            InitializeComponent();
            LoadReservations();
        }

        private void InitializeComponent()
        {
            this.Text = "🔖 Reservation Process";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // ─── Header ───
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(26, 54, 93)
            };
            header.Controls.Add(new Label
            {
                Text = "Reservation Process",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 5),
                AutoSize = true
            });
            header.Controls.Add(new Label
            {
                Text = "Reserve books for members",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(180, 200, 220),
                Location = new Point(22, 32),
                AutoSize = true
            });
            this.Controls.Add(header);

            // ─── Member Section ───
            GroupBox grpMember = new GroupBox
            {
                Text = "Step 1: Select Member",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 80),
                Size = new Size(420, 100)
            };
            this.Controls.Add(grpMember);

            grpMember.Controls.Add(new Label
            {
                Text = "User No:",
                Location = new Point(10, 33),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            });
            txtUserNumber = new TextBox
            {
                Location = new Point(90, 30),
                Size = new Size(140, 28),
                Font = new Font("Segoe UI", 10)
            };
            grpMember.Controls.Add(txtUserNumber);

            Button btnSearchUser = new Button
            {
                Text = "Search",
                Location = new Point(240, 29),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSearchUser.FlatAppearance.BorderSize = 0;
            btnSearchUser.Click += BtnSearchUser_Click;
            grpMember.Controls.Add(btnSearchUser);

            lblUserInfo = new Label
            {
                Location = new Point(10, 65),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 9)
            };
            grpMember.Controls.Add(lblUserInfo);

            // ─── Book Search Section ───
            GroupBox grpBook = new GroupBox
            {
                Text = "Step 2: Select Book Title",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 190),
                Size = new Size(420, 280)
            };
            this.Controls.Add(grpBook);

            grpBook.Controls.Add(new Label
            {
                Text = "Search:",
                Location = new Point(10, 33),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            });
            txtBookSearch = new TextBox
            {
                Location = new Point(70, 30),
                Size = new Size(200, 28),
                Font = new Font("Segoe UI", 10)
            };
            grpBook.Controls.Add(txtBookSearch);

            Button btnSearchBook = new Button
            {
                Text = "Search",
                Location = new Point(280, 29),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSearchBook.FlatAppearance.BorderSize = 0;
            btnSearchBook.Click += (s, e) => SearchBooks();
            grpBook.Controls.Add(btnSearchBook);

            dgvBooks = new DataGridView
            {
                Location = new Point(10, 70),
                Size = new Size(400, 140),
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
            dgvBooks.Columns.Add("BookNumber", "Book No.");
            dgvBooks.Columns.Add("Title", "Title");
            dgvBooks.Columns.Add("Author", "Author");
            dgvBooks.Columns["Title"]!.Width = 180;
            dgvBooks.SelectionChanged += DgvBooks_SelectionChanged;
            grpBook.Controls.Add(dgvBooks);

            lblBookInfo = new Label
            {
                Location = new Point(10, 215),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 9)
            };
            grpBook.Controls.Add(lblBookInfo);

            btnReserve = new Button
            {
                Text = "Reserve Book",
                Location = new Point(10, 240),
                Size = new Size(140, 32),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnReserve.FlatAppearance.BorderSize = 0;
            btnReserve.Click += BtnReserve_Click;
            grpBook.Controls.Add(btnReserve);

            // ─── Existing Reservations Grid ───
            GroupBox grpReservations = new GroupBox
            {
                Text = "Existing Reservations",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(460, 80),
                Size = new Size(410, 390)
            };
            this.Controls.Add(grpReservations);

            dgvReservations = new DataGridView
            {
                Location = new Point(10, 25),
                Size = new Size(390, 355),
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
            dgvReservations.Columns.Add("ReservationId", "Res. ID");
            dgvReservations.Columns.Add("BookNumber", "Book No.");
            dgvReservations.Columns.Add("UserNumber", "User No.");
            dgvReservations.Columns.Add("ReservedDate", "Date");
            grpReservations.Controls.Add(dgvReservations);
        }

        private void BtnSearchUser_Click(object? sender, EventArgs e)
        {
            string userNumber = txtUserNumber.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(userNumber))
            {
                MessageBox.Show("Please enter a User Number.", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            User? user = UserRepository.GetUserByNumber(userNumber);
            if (user == null)
            {
                lblUserInfo.ForeColor = Color.Red;
                lblUserInfo.Text = "User not found.";
                return;
            }

            if (!user.CanBorrow)
            {
                lblUserInfo.ForeColor = Color.Red;
                lblUserInfo.Text = "❌ Visitors cannot reserve books.";
                return;
            }

            lblUserInfo.ForeColor = Color.FromArgb(46, 204, 113);
            lblUserInfo.Text = $"✓ {user.Name} — {user.UserType}";
        }

        private void SearchBooks()
        {
            string search = txtBookSearch.Text.Trim();
            dgvBooks.Rows.Clear();

            List<Book> books;
            if (string.IsNullOrEmpty(search))
                books = BookRepository.GetAllBooks();
            else
                books = BookRepository.SearchBooks(search);

            foreach (var book in books)
            {
                dgvBooks.Rows.Add(book.BookNumber, book.Title, book.Author);
            }
        }

        private void DgvBooks_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvBooks.CurrentRow != null)
            {
                selectedBookNumber = dgvBooks.CurrentRow.Cells["BookNumber"].Value?.ToString() ?? "";
                string title = dgvBooks.CurrentRow.Cells["Title"].Value?.ToString() ?? "";
                lblBookInfo.Text = $"Selected: {title}";
                btnReserve.Enabled = true;
            }
        }

        private void BtnReserve_Click(object? sender, EventArgs e)
        {
            string userNumber = txtUserNumber.Text.Trim().ToUpper();

            // Validate user
            User? user = UserRepository.GetUserByNumber(userNumber);
            if (user == null || !user.CanBorrow)
            {
                MessageBox.Show("Please search for a valid member first.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(selectedBookNumber))
            {
                MessageBox.Show("Please select a book to reserve.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create reservation
            ReservationRepository.AddReservation(selectedBookNumber, userNumber);

            Book? book = BookRepository.GetBookByNumber(selectedBookNumber);
            MessageBox.Show(
                $"Reservation created successfully!\n\n" +
                $"Book: {book?.Title ?? selectedBookNumber}\n" +
                $"Member: {user.Name} ({userNumber})\n" +
                $"Date: {DateTime.Today:dd/MM/yyyy}",
                "Reservation Confirmed", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadReservations();
        }

        private void LoadReservations()
        {
            dgvReservations.Rows.Clear();
            List<Reservation> reservations = ReservationRepository.GetAllReservations();
            foreach (var res in reservations)
            {
                dgvReservations.Rows.Add(res.ReservationId, res.BookNumber,
                    res.UserNumber, res.ReservedDate.ToString("dd/MM/yyyy"));
            }
        }
    }
}
