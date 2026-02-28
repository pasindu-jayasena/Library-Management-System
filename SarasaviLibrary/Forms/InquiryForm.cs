using SarasaviLibrary.DataAccess;
using SarasaviLibrary.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SarasaviLibrary.Forms
{
    /// <summary>
    /// Inquiry Process Form — Process 4.
    /// Allows librarians and borrowers/visitors to check book availability.
    /// Search by: Book Number (accession no.), Title (partial), or Author (partial).
    /// Shows: book info, availability, total/available/loaned/reserved/reference copy counts.
    /// </summary>
    public class InquiryForm : Form
    {
        private ComboBox cmbSearchBy = null!;
        private TextBox txtSearchTerm = null!;
        private DataGridView dgvResults = null!;
        private GroupBox grpDetails = null!;
        private DataGridView dgvCopies = null!;
        private Label lblSummary = null!;

        public InquiryForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "🔍 Inquiry Process";
            this.Size = new Size(900, 650);
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
                Text = "Inquiry Process",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 5),
                AutoSize = true
            });
            header.Controls.Add(new Label
            {
                Text = "Search book availability — by book number, title, or author",
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
                ColumnCount = 1, RowCount = 3,
                Padding = new Padding(10),
                BackColor = Color.Transparent
            };
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 90f));
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 55f));
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 45f));
            this.Controls.Add(tbl);
            tbl.BringToFront();

            // ─── Search Section ───
            GroupBox grpSearch = new GroupBox
            {
                Text = "Search Books",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 5)
            };
            tbl.Controls.Add(grpSearch, 0, 0);

            grpSearch.Controls.Add(new Label
            {
                Text = "Search By:",
                Location = new Point(10, 28),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            });

            cmbSearchBy = new ComboBox
            {
                Location = new Point(90, 25),
                Size = new Size(150, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            cmbSearchBy.Items.AddRange(new object[] { "Book Number", "Title", "Author" });
            cmbSearchBy.SelectedIndex = 1;
            grpSearch.Controls.Add(cmbSearchBy);

            txtSearchTerm = new TextBox
            {
                Location = new Point(260, 25),
                Size = new Size(350, 28),
                Font = new Font("Segoe UI", 10)
            };
            grpSearch.Controls.Add(txtSearchTerm);

            Button btnSearch = new Button
            {
                Text = "Search",
                Location = new Point(620, 23),
                Size = new Size(100, 32),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.FromArgb(50, 50, 50),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnSearch_Click;
            grpSearch.Controls.Add(btnSearch);

            Button btnShowAll = new Button
            {
                Text = "Show All",
                Location = new Point(730, 23),
                Size = new Size(100, 32),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            btnShowAll.FlatAppearance.BorderSize = 0;
            btnShowAll.Click += (s, e) => LoadAllBooks();
            grpSearch.Controls.Add(btnShowAll);

            // ─── Results Grid ───
            GroupBox grpResults = new GroupBox
            {
                Text = "Search Results",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 5, 0, 5)
            };
            tbl.Controls.Add(grpResults, 0, 1);

            dgvResults = new DataGridView
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
            dgvResults.Columns.Add("BookNumber", "Book No.");
            dgvResults.Columns.Add("Classification", "Class");
            dgvResults.Columns.Add("Title", "Title");
            dgvResults.Columns.Add("Author", "Author");
            dgvResults.Columns.Add("ISBN", "ISBN");
            dgvResults.Columns.Add("TotalCopies", "Total Copies");
            dgvResults.Columns.Add("Available", "Available");
            dgvResults.Columns.Add("Loaned", "Loaned");
            dgvResults.Columns.Add("Reserved", "Reserved");
            dgvResults.Columns.Add("Reference", "Reference");
            dgvResults.Columns["Title"]!.Width = 150;
            dgvResults.SelectionChanged += DgvResults_SelectionChanged;
            grpResults.Controls.Add(dgvResults);

            // ─── Copy Details ───
            grpDetails = new GroupBox
            {
                Text = "Copy Details",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 5, 0, 0)
            };
            tbl.Controls.Add(grpDetails, 0, 2);

            lblSummary = new Label
            {
                Location = new Point(10, 25),
                Size = new Size(825, 25),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 54, 93)
            };
            grpDetails.Controls.Add(lblSummary);

            dgvCopies = new DataGridView
            {
                Location = new Point(10, 55),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
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
            dgvCopies.Columns.Add("CopyNumber", "Copy No.");
            dgvCopies.Columns.Add("Status", "Status");
            dgvCopies.Columns.Add("IsBorrowable", "Borrowable");
            grpDetails.Controls.Add(dgvCopies);
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            string searchTerm = txtSearchTerm.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Please enter a search term.", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<Book> results;
            string searchType = cmbSearchBy.SelectedItem!.ToString()!;

            if (searchType == "Book Number")
            {
                Book? book = BookRepository.GetBookByNumber(searchTerm.ToUpper());
                results = book != null ? new List<Book> { book } : new List<Book>();
            }
            else
            {
                // Both "Title" and "Author" search use the same method
                // (it searches both title and author fields)
                results = BookRepository.SearchBooks(searchTerm);
            }

            DisplayResults(results);
        }

        private void LoadAllBooks()
        {
            List<Book> allBooks = BookRepository.GetAllBooks();
            DisplayResults(allBooks);
        }

        private void DisplayResults(List<Book> books)
        {
            dgvResults.Rows.Clear();

            foreach (var book in books)
            {
                List<Copy> copies = BookRepository.GetCopiesByBookNumber(book.BookNumber);
                int totalCopies = copies.Count;
                int available = copies.Count(c => c.Status == "Available");
                int loaned = copies.Count(c => c.Status == "Loaned");
                int reserved = copies.Count(c => c.Status == "Reserved");
                int reference = copies.Count(c => !c.IsBorrowable);

                dgvResults.Rows.Add(
                    book.BookNumber,
                    book.Classification,
                    book.Title,
                    book.Author,
                    book.ISBN,
                    totalCopies,
                    available,
                    loaned,
                    reserved,
                    reference
                );
            }

            if (books.Count == 0)
            {
                MessageBox.Show("No books found matching your search.",
                    "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DgvResults_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvResults.CurrentRow != null)
            {
                string bookNumber = dgvResults.CurrentRow.Cells["BookNumber"].Value?.ToString() ?? "";
                string title = dgvResults.CurrentRow.Cells["Title"].Value?.ToString() ?? "";

                List<Copy> copies = BookRepository.GetCopiesByBookNumber(bookNumber);

                int available = copies.Count(c => c.Status == "Available");
                int total = copies.Count;

                // Update summary label
                if (available == total)
                    lblSummary.Text = $"📖 \"{title}\" — All {total} copies are available.";
                else if (available == 0)
                    lblSummary.Text = $"📖 \"{title}\" — No copies currently available ({total} total).";
                else
                    lblSummary.Text = $"📖 \"{title}\" — {available} of {total} copies available.";

                // Load copy details
                dgvCopies.Rows.Clear();
                foreach (var copy in copies)
                {
                    dgvCopies.Rows.Add(
                        copy.CopyNumber,
                        copy.Status,
                        copy.IsBorrowable ? "Yes" : "No (Reference)"
                    );
                }
            }
        }
    }
}
