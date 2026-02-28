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
        private Panel _buttonPanel = null!;

        // Stores each card's scalable controls for dynamic resizing
        private record CardEntry(Panel Card, Label Title, Label Desc, Panel Accent);
        private readonly List<CardEntry> _cards = new();

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
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimumSize = new Size(700, 500);

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

            // ─── Card Grid Panel ───
            _buttonPanel = new Panel
            {
                BackColor = Color.Transparent
            };
            this.Controls.Add(_buttonPanel);

            // Create 6 process buttons in a 3×2 grid
            string[,] buttons = {
                { "📖  Book Registration",    "Register new books and copies" },
                { "👤  User Registration",    "Register members and visitors" },
                { "📤  Loan Process",         "Issue books to members" },
                { "📥  Return Process",       "Accept returned books" },
                { "🔖  Reservation",          "Reserve books for members" },
                { "🔍  Inquiry",              "Search book availability" }
            };

            Color[] accentColors = {
                Color.FromArgb(52, 152, 219),
                Color.FromArgb(46, 204, 113),
                Color.FromArgb(230, 126, 34),
                Color.FromArgb(155, 89, 182),
                Color.FromArgb(231, 76, 60),
                Color.FromArgb(241, 196, 15)
            };

            for (int i = 0; i < 6; i++)
            {
                Panel card = new Panel
                {
                    BackColor = Color.White,
                    Cursor = Cursors.Hand,
                    Tag = i
                };

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
                    TextAlign = ContentAlignment.MiddleCenter
                };
                card.Controls.Add(cardTitle);

                Label cardDesc = new Label
                {
                    Text = buttons[i, 1],
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.FromArgb(100, 120, 140),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                card.Controls.Add(cardDesc);

                Panel accentBar = new Panel
                {
                    Location = new Point(0, 0),
                    BackColor = accentColors[i]
                };
                card.Controls.Add(accentBar);

                int index = i;
                card.Click      += (s, e) => OpenForm(index);
                cardTitle.Click += (s, e) => OpenForm(index);
                cardDesc.Click  += (s, e) => OpenForm(index);

                EventHandler mouseEnter = (s, e) => card.BackColor = Color.FromArgb(245, 248, 252);
                EventHandler mouseLeave = (s, e) => card.BackColor = Color.White;

                card.MouseEnter      += mouseEnter;
                card.MouseLeave      += mouseLeave;
                cardTitle.MouseEnter += mouseEnter;
                cardTitle.MouseLeave += mouseLeave;
                cardDesc.MouseEnter  += mouseEnter;
                cardDesc.MouseLeave  += mouseLeave;

                _cards.Add(new CardEntry(card, cardTitle, cardDesc, accentBar));
                _buttonPanel.Controls.Add(card);
            }

            // Initial layout + recalculate on every resize
            this.Load   += (s, e) => AdjustLayout();
            this.Resize += (s, e) => AdjustLayout();
        }

        // ─── Layout helper: scales and centres the card grid ───
        private void AdjustLayout()
        {
            const int headerHeight = 100;
            const int padding      = 40;   // margin around the grid

            int availW = this.ClientSize.Width  - padding * 2;
            int availH = this.ClientSize.Height - headerHeight - padding * 2;
            if (availW < 1 || availH < 1) return;

            // Scale relative to the base window size (900 × 620)
            float scaleX = availW / 790f;   // base grid width
            float scaleY = availH / 380f;   // base grid height
            float s      = Math.Min(scaleX, scaleY);
            s = Math.Max(0.5f, Math.Min(s, 3.0f)); // clamp

            int cardW  = (int)(250 * s);
            int cardH  = (int)(180 * s);
            int gapX   = (int)(20  * s);
            int gapY   = (int)(20  * s);

            int gridW = 3 * cardW + 2 * gapX;
            int gridH = 2 * cardH + gapY;

            int panelX = padding + Math.Max(0, (availW - gridW) / 2);
            int panelY = headerHeight + padding + Math.Max(0, (availH - gridH) / 2);

            _buttonPanel.Location = new Point(panelX, panelY);
            _buttonPanel.Size     = new Size(gridW, gridH);

            // Resize each card and its internal controls
            for (int i = 0; i < _cards.Count; i++)
            {
                int col = i % 3;
                int row = i / 3;

                var (card, title, desc, accent) = _cards[i];

                card.Location = new Point(col * (cardW + gapX), row * (cardH + gapY));
                card.Size     = new Size(cardW, cardH);

                int innerW    = cardW - (int)(20 * s);
                int titleH    = (int)(50 * s);
                int descH     = (int)(30 * s);
                int titleTop  = (int)(40 * s);
                int descTop   = (int)(cardH * 0.58f);

                title.Location = new Point((int)(10 * s), titleTop);
                title.Size     = new Size(innerW, titleH);
                title.Font     = new Font("Segoe UI", Math.Max(8f, 14f * s), FontStyle.Bold);

                desc.Location  = new Point((int)(10 * s), descTop);
                desc.Size      = new Size(innerW, descH);
                desc.Font      = new Font("Segoe UI", Math.Max(6f, 10f * s));

                accent.Size    = new Size(cardW, Math.Max(4, (int)(5 * s)));
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
