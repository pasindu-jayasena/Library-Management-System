using SarasaviLibrary.DataAccess;
using SarasaviLibrary.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SarasaviLibrary.Forms
{
    /// <summary>
    /// Return Process Form — Process 2 & 3 (Return + Reservation check).
    /// The librarian accepts the return, checks the status of the copy,
    /// and if the copy's title has an outstanding reservation, notifies
    /// the oldest reserver and deletes that reservation.
    /// </summary>
    public class ReturnForm : Form
    {
        private TextBox txtCopyNumber = null!;
        private Label lblReturnInfo = null!;
        private RichTextBox rtbResult = null!;
        private Button btnReturn = null!;

        public ReturnForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "📥 Return Process";
            this.Size = new Size(600, 500);
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
                Text = "Return Process",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 5),
                AutoSize = true
            });
            header.Controls.Add(new Label
            {
                Text = "Accept book returns and check reservations",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(180, 200, 220),
                Location = new Point(22, 32),
                AutoSize = true
            });
            this.Controls.Add(header);

            // ─── Return Section ───
            GroupBox grpReturn = new GroupBox
            {
                Text = "Return a Book",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 80),
                Size = new Size(545, 130)
            };
            this.Controls.Add(grpReturn);

            grpReturn.Controls.Add(new Label
            {
                Text = "Copy Number:",
                Location = new Point(10, 33),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            });

            txtCopyNumber = new TextBox
            {
                Location = new Point(130, 30),
                Size = new Size(180, 28),
                Font = new Font("Segoe UI", 10)
            };
            grpReturn.Controls.Add(txtCopyNumber);

            btnReturn = new Button
            {
                Text = "Process Return",
                Location = new Point(320, 28),
                Size = new Size(140, 32),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnReturn.FlatAppearance.BorderSize = 0;
            btnReturn.Click += BtnReturn_Click;
            grpReturn.Controls.Add(btnReturn);

            lblReturnInfo = new Label
            {
                Location = new Point(10, 80),
                Size = new Size(520, 30),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(60, 60, 60)
            };
            grpReturn.Controls.Add(lblReturnInfo);

            // ─── Result Display ───
            GroupBox grpResult = new GroupBox
            {
                Text = "Return Details",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 220),
                Size = new Size(545, 220)
            };
            this.Controls.Add(grpResult);

            rtbResult = new RichTextBox
            {
                Location = new Point(10, 25),
                Size = new Size(525, 185),
                ReadOnly = true,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            grpResult.Controls.Add(rtbResult);
        }

        private void BtnReturn_Click(object? sender, EventArgs e)
        {
            string copyNumber = txtCopyNumber.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(copyNumber))
            {
                MessageBox.Show("Please enter a Copy Number.", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if the copy exists
            Copy? copy = BookRepository.GetCopyByNumber(copyNumber);
            if (copy == null)
            {
                lblReturnInfo.ForeColor = Color.Red;
                lblReturnInfo.Text = "Copy not found in catalogue.";
                return;
            }

            // Check if the copy is currently on loan
            Loan? activeLoan = LoanRepository.GetActiveLoanForCopy(copyNumber);
            if (activeLoan == null)
            {
                lblReturnInfo.ForeColor = Color.Red;
                lblReturnInfo.Text = "This copy is not currently on loan.";
                return;
            }

            // Get book and user info for display
            Book? book = BookRepository.GetBookByNumber(copy.BookNumber);
            User? user = UserRepository.GetUserByNumber(activeLoan.UserNumber);

            // Process the return
            string? bookNumber = LoanRepository.ReturnBook(copyNumber);

            rtbResult.Clear();
            rtbResult.SelectionFont = new Font("Segoe UI", 10, FontStyle.Bold);
            rtbResult.AppendText("✓ BOOK RETURNED SUCCESSFULLY\n\n");

            rtbResult.SelectionFont = new Font("Segoe UI", 10);
            rtbResult.AppendText($"Copy Number: {copyNumber}\n");
            rtbResult.AppendText($"Book Title: {book?.Title ?? "Unknown"}\n");
            rtbResult.AppendText($"Returned by: {user?.Name ?? "Unknown"} ({activeLoan.UserNumber})\n");
            rtbResult.AppendText($"Loan Date: {activeLoan.LoanDate:dd/MM/yyyy}\n");
            rtbResult.AppendText($"Due Date: {activeLoan.DueDate:dd/MM/yyyy}\n");
            rtbResult.AppendText($"Return Date: {DateTime.Today:dd/MM/yyyy}\n");

            if (activeLoan.IsOverdue)
            {
                rtbResult.SelectionColor = Color.Red;
                rtbResult.AppendText($"\n⚠ This book was OVERDUE!\n");
            }

            // ─── Reservation Check (Process 3) ───
            if (bookNumber != null && ReservationRepository.HasReservations(bookNumber))
            {
                Reservation? oldestReservation = ReservationRepository.GetOldestReservation(bookNumber);
                if (oldestReservation != null)
                {
                    User? reserver = UserRepository.GetUserByNumber(oldestReservation.UserNumber);

                    // Put the copy on reserve
                    BookRepository.UpdateCopyStatus(copyNumber, "Reserved");

                    // Delete the fulfilled reservation
                    ReservationRepository.DeleteReservation(oldestReservation.ReservationId);

                    rtbResult.SelectionFont = new Font("Segoe UI", 10, FontStyle.Bold);
                    rtbResult.SelectionColor = Color.FromArgb(230, 126, 34);
                    rtbResult.AppendText($"\n🔖 RESERVATION NOTIFICATION\n");
                    rtbResult.SelectionFont = new Font("Segoe UI", 10);
                    rtbResult.SelectionColor = Color.Black;
                    rtbResult.AppendText($"This title has been reserved by:\n");
                    rtbResult.AppendText($"   Member: {reserver?.Name ?? "Unknown"} ({oldestReservation.UserNumber})\n");
                    rtbResult.AppendText($"   Reserved on: {oldestReservation.ReservedDate:dd/MM/yyyy}\n");
                    rtbResult.AppendText($"\nThe copy has been set aside. Please notify the member.");

                    // Also show a popup to make sure the librarian notices
                    MessageBox.Show(
                        $"⚠ RESERVATION ALERT!\n\n" +
                        $"This book title has been reserved by:\n" +
                        $"Member: {reserver?.Name ?? "Unknown"} ({oldestReservation.UserNumber})\n\n" +
                        $"The copy has been put aside.\n" +
                        $"Please notify the member to collect the book.",
                        "Reservation Notification",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            lblReturnInfo.ForeColor = Color.FromArgb(46, 204, 113);
            lblReturnInfo.Text = "Return processed successfully.";
            txtCopyNumber.Clear();
        }
    }
}
