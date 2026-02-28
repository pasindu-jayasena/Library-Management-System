using SarasaviLibrary.DataAccess;
using SarasaviLibrary.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SarasaviLibrary.Forms
{
    /// <summary>
    /// Loan Process Form — Process 1.
    /// Allows the librarian to issue books to registered members.
    /// Business rules:
    ///   - Only "Members" can borrow (not Visitors).
    ///   - Maximum 5 active loans per member.
    ///   - Cannot borrow if member has overdue books.
    ///   - Reference copies cannot be borrowed.
    ///   - Loan period: 14 days (2 weeks).
    /// </summary>
    public class LoanForm : Form
    {
        private TextBox txtUserNumber = null!;
        private Label lblUserInfo = null!;
        private Label lblLoanCount = null!;
        private TextBox txtCopyNumber = null!;
        private Label lblCopyInfo = null!;
        private DataGridView dgvActiveLoans = null!;
        private Button btnSearch = null!;
        private Button btnLoan = null!;

        public LoanForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "📤 Loan Process";
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
                Text = "Loan Process",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 5),
                AutoSize = true
            });
            header.Controls.Add(new Label
            {
                Text = "Issue books to registered members",
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

            TableLayoutPanel tblLeft = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1, RowCount = 2,
                Margin = new Padding(0),
                BackColor = Color.Transparent
            };
            tblLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 45f));
            tblLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 55f));
            tbl.Controls.Add(tblLeft, 0, 0);

            // ─── Member Section ───
            GroupBox grpMember = new GroupBox
            {
                Text = "Step 1: Select Member",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 5, 5)
            };
            tblLeft.Controls.Add(grpMember, 0, 0);

            grpMember.Controls.Add(MakeLabel("User No:", 10, 30));
            txtUserNumber = new TextBox
            {
                Location = new Point(100, 28),
                Size = new Size(150, 28),
                Font = new Font("Segoe UI", 10)
            };
            grpMember.Controls.Add(txtUserNumber);

            btnSearch = new Button
            {
                Text = "Search",
                Location = new Point(260, 27),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnSearchUser_Click;
            grpMember.Controls.Add(btnSearch);

            lblUserInfo = new Label
            {
                Location = new Point(10, 70),
                Size = new Size(380, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(60, 60, 60)
            };
            grpMember.Controls.Add(lblUserInfo);

            lblLoanCount = new Label
            {
                Location = new Point(10, 95),
                Size = new Size(380, 25),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 126, 34)
            };
            grpMember.Controls.Add(lblLoanCount);

            // ─── Copy Section ───
            GroupBox grpCopy = new GroupBox
            {
                Text = "Step 2: Select Copy to Loan",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 5, 5, 0)
            };
            tblLeft.Controls.Add(grpCopy, 0, 1);

            grpCopy.Controls.Add(MakeLabel("Copy No:", 10, 30));
            txtCopyNumber = new TextBox
            {
                Location = new Point(100, 28),
                Size = new Size(150, 28),
                Font = new Font("Segoe UI", 10)
            };
            grpCopy.Controls.Add(txtCopyNumber);

            Button btnCheckCopy = new Button
            {
                Text = "Check",
                Location = new Point(260, 27),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCheckCopy.FlatAppearance.BorderSize = 0;
            btnCheckCopy.Click += BtnCheckCopy_Click;
            grpCopy.Controls.Add(btnCheckCopy);

            lblCopyInfo = new Label
            {
                Location = new Point(10, 70),
                Size = new Size(380, 40),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(60, 60, 60)
            };
            grpCopy.Controls.Add(lblCopyInfo);

            btnLoan = new Button
            {
                Text = "Confirm Loan",
                Location = new Point(100, 115),
                Size = new Size(150, 38),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnLoan.FlatAppearance.BorderSize = 0;
            btnLoan.Click += BtnLoan_Click;
            grpCopy.Controls.Add(btnLoan);

            // ─── Active Loans Grid ───
            GroupBox grpLoans = new GroupBox
            {
                Text = "Active Loans",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                Margin = new Padding(5, 0, 0, 0)
            };
            tbl.Controls.Add(grpLoans, 1, 0);

            dgvActiveLoans = new DataGridView
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
            dgvActiveLoans.Columns.Add("CopyNumber", "Copy No.");
            dgvActiveLoans.Columns.Add("UserNumber", "User No.");
            dgvActiveLoans.Columns.Add("LoanDate", "Loan Date");
            dgvActiveLoans.Columns.Add("DueDate", "Due Date");
            dgvActiveLoans.Columns.Add("Overdue", "Overdue?");
            grpLoans.Controls.Add(dgvActiveLoans);

            LoadAllActiveLoans();
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
                lblUserInfo.Text = "User not found.";
                lblUserInfo.ForeColor = Color.Red;
                lblLoanCount.Text = "";
                return;
            }

            lblUserInfo.ForeColor = Color.FromArgb(60, 60, 60);
            lblUserInfo.Text = $"Name: {user.Name}  |  Type: {user.UserType}";

            if (!user.CanBorrow)
            {
                lblLoanCount.ForeColor = Color.Red;
                lblLoanCount.Text = "❌ Visitors cannot borrow books.";
                return;
            }

            int activeCount = LoanRepository.GetActiveLoanCount(userNumber);
            bool hasOverdue = LoanRepository.HasOverdueBooks(userNumber);

            if (hasOverdue)
            {
                lblLoanCount.ForeColor = Color.Red;
                lblLoanCount.Text = $"❌ Has overdue books! ({activeCount}/5 active loans)";
            }
            else if (activeCount >= 5)
            {
                lblLoanCount.ForeColor = Color.Red;
                lblLoanCount.Text = $"❌ Maximum loans reached ({activeCount}/5)";
            }
            else
            {
                lblLoanCount.ForeColor = Color.FromArgb(46, 204, 113);
                lblLoanCount.Text = $"✓ Eligible to borrow ({activeCount}/5 active loans)";
            }

            // Load this user's active loans
            LoadUserActiveLoans(userNumber);
        }

        private void BtnCheckCopy_Click(object? sender, EventArgs e)
        {
            string copyNumber = txtCopyNumber.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(copyNumber))
            {
                MessageBox.Show("Please enter a Copy Number.", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Copy? copy = BookRepository.GetCopyByNumber(copyNumber);
            if (copy == null)
            {
                lblCopyInfo.ForeColor = Color.Red;
                lblCopyInfo.Text = "Copy not found in catalogue.";
                btnLoan.Enabled = false;
                return;
            }

            // Get the book title for display
            Book? book = BookRepository.GetBookByNumber(copy.BookNumber);
            string bookTitle = book?.Title ?? "Unknown";

            if (!copy.IsBorrowable)
            {
                lblCopyInfo.ForeColor = Color.Red;
                lblCopyInfo.Text = $"❌ '{bookTitle}' — This copy is REFERENCE only.";
                btnLoan.Enabled = false;
                return;
            }

            if (copy.Status != "Available")
            {
                lblCopyInfo.ForeColor = Color.Red;
                lblCopyInfo.Text = $"❌ '{bookTitle}' — Status: {copy.Status}";
                btnLoan.Enabled = false;
                return;
            }

            lblCopyInfo.ForeColor = Color.FromArgb(46, 204, 113);
            lblCopyInfo.Text = $"✓ '{bookTitle}' — Available for loan.";
            btnLoan.Enabled = true;
        }

        private void BtnLoan_Click(object? sender, EventArgs e)
        {
            string userNumber = txtUserNumber.Text.Trim().ToUpper();
            string copyNumber = txtCopyNumber.Text.Trim().ToUpper();

            // Re-validate user
            User? user = UserRepository.GetUserByNumber(userNumber);
            if (user == null || !user.CanBorrow)
            {
                MessageBox.Show("Invalid member. Please search the user first.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (LoanRepository.HasOverdueBooks(userNumber))
            {
                MessageBox.Show("This member has overdue books. Cannot issue new loans until overdue books are returned.",
                    "Overdue Books", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (LoanRepository.GetActiveLoanCount(userNumber) >= 5)
            {
                MessageBox.Show("This member has reached the maximum of 5 active loans.",
                    "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Re-validate copy
            Copy? copy = BookRepository.GetCopyByNumber(copyNumber);
            if (copy == null || !copy.CanBeBorrowed)
            {
                MessageBox.Show("This copy is not available for loan.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Confirm with the librarian
            DateTime dueDate = DateTime.Today.AddDays(14);
            DialogResult result = MessageBox.Show(
                $"Confirm loan?\n\n" +
                $"Copy: {copyNumber}\n" +
                $"Member: {user.Name} ({userNumber})\n" +
                $"Due Date: {dueDate:dd/MM/yyyy}",
                "Confirm Loan", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                LoanRepository.CreateLoan(copyNumber, userNumber);

                MessageBox.Show(
                    $"Loan confirmed!\n\n" +
                    $"Return by: {dueDate:dd/MM/yyyy}",
                    "Loan Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh
                btnLoan.Enabled = false;
                txtCopyNumber.Clear();
                lblCopyInfo.Text = "";
                LoadUserActiveLoans(userNumber);
                BtnSearchUser_Click(null, EventArgs.Empty); // Refresh loan count
            }
        }

        private void LoadUserActiveLoans(string userNumber)
        {
            dgvActiveLoans.Rows.Clear();
            List<Loan> loans = LoanRepository.GetActiveLoansForUser(userNumber);
            foreach (var loan in loans)
            {
                dgvActiveLoans.Rows.Add(
                    loan.CopyNumber,
                    loan.UserNumber,
                    loan.LoanDate.ToString("dd/MM/yyyy"),
                    loan.DueDate.ToString("dd/MM/yyyy"),
                    loan.IsOverdue ? "YES" : "No"
                );
            }
        }

        private void LoadAllActiveLoans()
        {
            dgvActiveLoans.Rows.Clear();
            List<Loan> loans = LoanRepository.GetAllActiveLoans();
            foreach (var loan in loans)
            {
                dgvActiveLoans.Rows.Add(
                    loan.CopyNumber,
                    loan.UserNumber,
                    loan.LoanDate.ToString("dd/MM/yyyy"),
                    loan.DueDate.ToString("dd/MM/yyyy"),
                    loan.IsOverdue ? "YES" : "No"
                );
            }
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
