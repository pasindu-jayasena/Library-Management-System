using SarasaviLibrary.DataAccess;
using SarasaviLibrary.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SarasaviLibrary.Forms
{
    /// <summary>
    /// Book Registration Form — Process 5.
    /// Allows the librarian to enter details of new books and their copies.
    /// Maximum 10 copies per book. Auto-generates BookNumber (X9999) and CopyNumber (X99991).
    /// </summary>
    public class BookRegistrationForm : Form
    {
        // ─── Controls ───
        private ComboBox cmbClassification = null!;
        private TextBox txtTitle = null!;
        private TextBox txtAuthor = null!;
        private TextBox txtISBN = null!;
        private TextBox txtPublisher = null!;
        private NumericUpDown nudCopies = null!;
        private CheckBox chkReference = null!;
        private Label lblBookNumber = null!;
        private DataGridView dgvBooks = null!;
        private DataGridView dgvCopies = null!;
        private Button btnRegister = null!;
        private Button btnClear = null!;

        public BookRegistrationForm()
        {
            InitializeComponent();
            LoadBooks();
        }

        private void InitializeComponent()
        {
            // ─── Form Settings ───
            this.Text = "📖 Book Registration";
            this.Size = new Size(950, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // ─── Header ───
            Panel header = CreateHeader("Book Registration", "Register new books and copies to the catalogue");
            this.Controls.Add(header);

            // ─── Input Section ───
            GroupBox grpInput = new GroupBox
            {
                Text = "Book Details",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 80),
                Size = new Size(440, 350)
            };
            this.Controls.Add(grpInput);

            int y = 30;
            int inputX = 120;
            int inputWidth = 300;

            // Classification
            grpInput.Controls.Add(CreateLabel("Classification:", 10, y));
            cmbClassification = new ComboBox
            {
                Location = new Point(inputX, y),
                Size = new Size(80, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            for (char c = 'A'; c <= 'Z'; c++)
                cmbClassification.Items.Add(c);
            cmbClassification.SelectedIndex = 0;
            cmbClassification.SelectedIndexChanged += (s, e) => UpdateBookNumber();
            grpInput.Controls.Add(cmbClassification);

            // Auto-generated Book Number
            lblBookNumber = new Label
            {
                Location = new Point(220, y),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                Text = ""
            };
            grpInput.Controls.Add(lblBookNumber);
            UpdateBookNumber();

            y += 40;

            // Title
            grpInput.Controls.Add(CreateLabel("Title:", 10, y));
            txtTitle = CreateTextBox(inputX, y, inputWidth);
            grpInput.Controls.Add(txtTitle);
            y += 40;

            // Author
            grpInput.Controls.Add(CreateLabel("Author:", 10, y));
            txtAuthor = CreateTextBox(inputX, y, inputWidth);
            grpInput.Controls.Add(txtAuthor);
            y += 40;

            // ISBN
            grpInput.Controls.Add(CreateLabel("ISBN:", 10, y));
            txtISBN = CreateTextBox(inputX, y, inputWidth);
            grpInput.Controls.Add(txtISBN);
            y += 40;

            // Publisher
            grpInput.Controls.Add(CreateLabel("Publisher:", 10, y));
            txtPublisher = CreateTextBox(inputX, y, inputWidth);
            grpInput.Controls.Add(txtPublisher);
            y += 40;

            // Number of Copies
            grpInput.Controls.Add(CreateLabel("No. of Copies:", 10, y));
            nudCopies = new NumericUpDown
            {
                Location = new Point(inputX, y),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 10),
                Minimum = 1,
                Maximum = 10,
                Value = 1
            };
            grpInput.Controls.Add(nudCopies);
            y += 40;

            // Reference Only checkbox
            chkReference = new CheckBox
            {
                Text = "  Reference Only (not borrowable)",
                Location = new Point(inputX, y),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 9)
            };
            grpInput.Controls.Add(chkReference);
            y += 40;

            // Buttons
            btnRegister = new Button
            {
                Text = "Register Book",
                Location = new Point(inputX, y),
                Size = new Size(140, 38),
                BackColor = Color.FromArgb(52, 152, 219),
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
                Location = new Point(inputX + 150, y),
                Size = new Size(100, 38),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.Click += (s, e) => ClearForm();
            grpInput.Controls.Add(btnClear);

            // ─── Books Grid ───
            GroupBox grpBooks = new GroupBox
            {
                Text = "Registered Books",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(480, 80),
                Size = new Size(445, 200)
            };
            this.Controls.Add(grpBooks);

            dgvBooks = CreateDataGridView(10, 25, 425, 165);
            dgvBooks.Columns.Add("BookNumber", "Book No.");
            dgvBooks.Columns.Add("Classification", "Class");
            dgvBooks.Columns.Add("Title", "Title");
            dgvBooks.Columns.Add("Author", "Author");
            dgvBooks.Columns.Add("Publisher", "Publisher");
            dgvBooks.Columns["Title"]!.Width = 150;
            dgvBooks.SelectionChanged += DgvBooks_SelectionChanged;
            grpBooks.Controls.Add(dgvBooks);

            // ─── Copies Grid ───
            GroupBox grpCopies = new GroupBox
            {
                Text = "Copies of Selected Book",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(480, 290),
                Size = new Size(445, 140)
            };
            this.Controls.Add(grpCopies);

            dgvCopies = CreateDataGridView(10, 25, 425, 105);
            dgvCopies.Columns.Add("CopyNumber", "Copy No.");
            dgvCopies.Columns.Add("Status", "Status");
            dgvCopies.Columns.Add("IsBorrowable", "Borrowable");
            grpCopies.Controls.Add(dgvCopies);
        }

        // ─── Event Handlers ───

        private void UpdateBookNumber()
        {
            if (cmbClassification.SelectedItem != null)
            {
                char classification = (char)cmbClassification.SelectedItem;
                string nextNumber = BookRepository.GetNextBookNumber(classification);
                lblBookNumber.Text = $"Book No: {nextNumber}";
            }
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter the book title.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            char classification = (char)cmbClassification.SelectedItem!;
            string bookNumber = BookRepository.GetNextBookNumber(classification);
            int copyCount = (int)nudCopies.Value;
            bool isReference = chkReference.Checked;

            // Check if adding these copies would exceed 10
            // (For a new book, it's always fine since max is 10 and we check)
            if (copyCount > 10)
            {
                MessageBox.Show("Maximum 10 copies allowed per book.",
                    "Limit Exceeded", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create the book
            Book newBook = new Book
            {
                BookNumber = bookNumber,
                Classification = classification,
                Title = txtTitle.Text.Trim(),
                Author = txtAuthor.Text.Trim(),
                ISBN = txtISBN.Text.Trim(),
                Publisher = txtPublisher.Text.Trim()
            };

            BookRepository.AddBook(newBook);

            // Create copies
            for (int i = 1; i <= copyCount; i++)
            {
                string copyDigit = (i == 10) ? "0" : i.ToString();
                Copy newCopy = new Copy
                {
                    CopyNumber = bookNumber + copyDigit,
                    BookNumber = bookNumber,
                    IsBorrowable = !isReference,
                    Status = isReference ? "Reference" : "Available"
                };
                BookRepository.AddCopy(newCopy);
            }

            MessageBox.Show($"Book '{newBook.Title}' registered successfully!\n" +
                            $"Book Number: {bookNumber}\n" +
                            $"Copies added: {copyCount}",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ClearForm();
            LoadBooks();
            UpdateBookNumber();
        }

        private void DgvBooks_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvBooks.CurrentRow != null)
            {
                string bookNumber = dgvBooks.CurrentRow.Cells["BookNumber"].Value?.ToString() ?? "";
                LoadCopies(bookNumber);
            }
        }

        // ─── Data Loading ───

        private void LoadBooks()
        {
            dgvBooks.Rows.Clear();
            List<Book> books = BookRepository.GetAllBooks();
            foreach (var book in books)
            {
                dgvBooks.Rows.Add(book.BookNumber, book.Classification, book.Title,
                    book.Author, book.Publisher);
            }
        }

        private void LoadCopies(string bookNumber)
        {
            dgvCopies.Rows.Clear();
            List<Copy> copies = BookRepository.GetCopiesByBookNumber(bookNumber);
            foreach (var copy in copies)
            {
                dgvCopies.Rows.Add(copy.CopyNumber, copy.Status,
                    copy.IsBorrowable ? "Yes" : "No");
            }
        }

        private void ClearForm()
        {
            txtTitle.Clear();
            txtAuthor.Clear();
            txtISBN.Clear();
            txtPublisher.Clear();
            nudCopies.Value = 1;
            chkReference.Checked = false;
            cmbClassification.SelectedIndex = 0;
        }

        // ─── UI Helper Methods ───

        private static Panel CreateHeader(string title, string subtitle)
        {
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(26, 54, 93)
            };

            header.Controls.Add(new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 5),
                AutoSize = true
            });

            header.Controls.Add(new Label
            {
                Text = subtitle,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(180, 200, 220),
                Location = new Point(22, 32),
                AutoSize = true
            });

            return header;
        }

        private static Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y + 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };
        }

        private static TextBox CreateTextBox(int x, int y, int width)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 28),
                Font = new Font("Segoe UI", 10)
            };
        }

        private static DataGridView CreateDataGridView(int x, int y, int width, int height)
        {
            return new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
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
        }
    }
}
