using System;
using System.Drawing;
using System.Windows.Forms;

namespace SarasaviLibrary.Forms
{
    /// <summary>
    /// Main dashboard form with navigation buttons to all 6 library processes.
    /// </summary>
    public class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ─── Form Settings ───
            this.Text = "Sarasavi Library Management System";
            this.Size = new Size(900, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ─── Header Panel ───
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(26, 54, 93)
            };
            this.Controls.Add(headerPanel);

            Label titleLabel = new Label
            {
                Text = "📚 Sarasavi Library",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(500, 50),
                Location = new Point(30, 10),
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(titleLabel);

            Label subtitleLabel = new Label
            {
                Text = "Library Management System",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(180, 200, 220),
                AutoSize = false,
                Size = new Size(400, 30),
                Location = new Point(32, 60),
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(subtitleLabel);

            // ─── Button Panel ───
            Panel buttonPanel = new Panel
            {
                Location = new Point(30, 130),
                Size = new Size(830, 430)
            };
            this.Controls.Add(buttonPanel);

            // Create 6 process buttons in a 3×2 grid
            string[,] buttons = {
                { "📖  Book Registration",    "Register new books and copies" },
                { "👤  User Registration",    "Register members and visitors" },
                { "📤  Loan Process",         "Issue books to members" },
                { "📥  Return Process",       "Accept returned books" },
                { "🔖  Reservation",          "Reserve books for members" },
                { "🔍  Inquiry",              "Search book availability" }
            };

            int buttonWidth = 250;
            int buttonHeight = 180;
            int gapX = 20;
            int gapY = 20;

            for (int i = 0; i < 6; i++)
            {
                int col = i % 3;
                int row = i / 3;

                Panel card = new Panel
                {
                    Location = new Point(col * (buttonWidth + gapX), row * (buttonHeight + gapY)),
                    Size = new Size(buttonWidth, buttonHeight),
                    BackColor = Color.White,
                    Cursor = Cursors.Hand,
                    Tag = i
                };

                // Rounded appearance via painting
                card.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using var pen = new Pen(Color.FromArgb(200, 210, 220), 1);
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                };

                Label cardTitle = new Label
                {
                    Text = buttons[i, 0],
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    ForeColor = Color.FromArgb(26, 54, 93),
                    AutoSize = false,
                    Size = new Size(230, 50),
                    Location = new Point(10, 40),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                card.Controls.Add(cardTitle);

                Label cardDesc = new Label
                {
                    Text = buttons[i, 1],
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.FromArgb(100, 120, 140),
                    AutoSize = false,
                    Size = new Size(230, 30),
                    Location = new Point(10, 100),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                card.Controls.Add(cardDesc);

                // Color accent bar at top of card
                Color[] accentColors = {
                    Color.FromArgb(52, 152, 219),   // Blue
                    Color.FromArgb(46, 204, 113),   // Green
                    Color.FromArgb(230, 126, 34),   // Orange
                    Color.FromArgb(155, 89, 182),   // Purple
                    Color.FromArgb(231, 76, 60),    // Red
                    Color.FromArgb(241, 196, 15)    // Yellow
                };

                Panel accentBar = new Panel
                {
                    Location = new Point(0, 0),
                    Size = new Size(buttonWidth, 5),
                    BackColor = accentColors[i]
                };
                card.Controls.Add(accentBar);

                int index = i;
                // Handle click on entire card
                card.Click += (s, e) => OpenForm(index);
                cardTitle.Click += (s, e) => OpenForm(index);
                cardDesc.Click += (s, e) => OpenForm(index);

                // Hover effects
                EventHandler mouseEnter = (s, e) => card.BackColor = Color.FromArgb(245, 248, 252);
                EventHandler mouseLeave = (s, e) => card.BackColor = Color.White;

                card.MouseEnter += mouseEnter;
                card.MouseLeave += mouseLeave;
                cardTitle.MouseEnter += mouseEnter;
                cardTitle.MouseLeave += mouseLeave;
                cardDesc.MouseEnter += mouseEnter;
                cardDesc.MouseLeave += mouseLeave;

                buttonPanel.Controls.Add(card);
            }
        }

        /// <summary>
        /// Opens the appropriate form based on the button index.
        /// </summary>
        private void OpenForm(int index)
        {
            Form form = index switch
            {
                0 => new BookRegistrationForm(),
                1 => new UserRegistrationForm(),
                2 => new LoanForm(),
                3 => new ReturnForm(),
                4 => new ReservationForm(),
                5 => new InquiryForm(),
                _ => throw new ArgumentOutOfRangeException()
            };

            form.ShowDialog();
        }
    }
}
