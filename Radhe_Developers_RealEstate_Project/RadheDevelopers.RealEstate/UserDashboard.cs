using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RadheDevelopers.RealEstate
{
    public class UserDashboard : Form
    {
        Label lblTitle, lblWelcome;
        Button btnAddProperty, btnBookProperty, btnViewProperties, btnMyProperties, btnLogout;
        Panel headerPanel, bodyPanel;
        private int userId;
        private string userName;

        public UserDashboard(int userId, string userName = "User")
        {
            this.userId = userId;
            this.userName = userName;

            this.Text = "Radhe Developers - Dashboard";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.None;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // 🟦 Header
            headerPanel = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(25, 118, 210)
            };

            lblTitle = new Label()
            {
                Text = "🏠 Radhe Developers - Real Estate Dashboard",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            headerPanel.Controls.Add(lblTitle);
            Controls.Add(headerPanel);

            // 🌆 Background gradient panel
            bodyPanel = new Panel()
            {
                Dock = DockStyle.Fill
            };
            bodyPanel.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    bodyPanel.ClientRectangle,
                    Color.FromArgb(240, 248, 255),
                    Color.FromArgb(200, 220, 255),
                    45f))
                {
                    e.Graphics.FillRectangle(brush, bodyPanel.ClientRectangle);
                }
            };
            Controls.Add(bodyPanel);

            // 👋 Welcome label
            lblWelcome = new Label()
            {
                Text = $"Welcome, {userName} 👋",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(60, 40)
            };
            bodyPanel.Controls.Add(lblWelcome);

            // 📦 Buttons Grid Layout
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel()
            {
                Left = 60,
                Top = 100,
                Width = 900,
                Height = 600,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true
            };
            bodyPanel.Controls.Add(buttonPanel);

            // 🟢 Add Property
            btnAddProperty = CreateCardButton("➕ Add Property", Color.FromArgb(46, 125, 50));
            btnAddProperty.Click += (s, e) => new AddPropertyForm(userId).ShowDialog();

            // 🟠 Book Property
            btnBookProperty = CreateCardButton("📋 Book Property", Color.FromArgb(251, 140, 0));
            btnBookProperty.Click += BtnBookProperty_Click;

            // 🔵 View Properties
            btnViewProperties = CreateCardButton("🏡 View Properties", Color.FromArgb(3, 169, 244));
            btnViewProperties.Click += BtnViewProperties_Click;

            // 🧾 My Properties
            btnMyProperties = CreateCardButton("🧾 My Properties", Color.FromArgb(66, 133, 244));
            btnMyProperties.Click += BtnMyProperties_Click;

            // 🔴 Logout
            btnLogout = CreateCardButton("🚪 Logout", Color.FromArgb(211, 47, 47));
            btnLogout.Click += BtnLogout_Click;

            // Add buttons to panel
            buttonPanel.Controls.AddRange(new Control[]
            {
                btnAddProperty, btnBookProperty, btnViewProperties, btnMyProperties, btnLogout
            });

            // 👤 User Card (top right)
            Panel userCard = new Panel()
            {
                Width = 250,
                Height = 100,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(this.Width - 300, 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Label lblUser = new Label()
            {
                Text = $"👤 {userName}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 118, 210),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            userCard.Controls.Add(lblUser);
            bodyPanel.Controls.Add(userCard);
        }

        // 🔘 Helper to create animated card-style buttons
        private Button CreateCardButton(string text, Color bgColor)
        {
            Button btn = new Button()
            {
                Text = text,
                Width = 220,
                Height = 100,
                Margin = new Padding(20),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = bgColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;

            // Add hover animation
            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Light(bgColor);
            btn.MouseLeave += (s, e) => btn.BackColor = bgColor;

            // Rounded corners
            btn.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, btn.Width, btn.Height, 20, 20));

            return btn;
        }

        // 🧱 Rounded corner API
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect,
            int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        // 📋 Book Property
        private void BtnBookProperty_Click(object sender, EventArgs e)
        {
            string input = ShowInputDialog("Enter the PropertyID you want to book:", "Book Property");
            if (int.TryParse(input, out int propertyId))
            {
                new BookingForm(userId, propertyId).ShowDialog();
            }
            else if (!string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Invalid PropertyID entered.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnViewProperties_Click(object sender, EventArgs e)
        {
            new ViewPropertiesForm().ShowDialog();
        }

        private void BtnMyProperties_Click(object sender, EventArgs e)
        {
            new MyPropertiesForm(userId).ShowDialog();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure you want to logout?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                new LoginForm().Show();
                this.Close();
            }
        }

        // 🔹 Custom Input Dialog
        private string ShowInputDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 180,
                Text = caption,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                BackColor = Color.White
            };

            Label textLabel = new Label() { Left = 20, Top = 20, Text = text, Width = 340, Font = new Font("Segoe UI", 10) };
            TextBox inputBox = new TextBox() { Left = 20, Top = 60, Width = 340, Font = new Font("Segoe UI", 10) };
            Button confirmation = new Button()
            {
                Text = "OK",
                Left = 280,
                Width = 80,
                Top = 100,
                BackColor = Color.FromArgb(25, 118, 210),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };

            prompt.Controls.AddRange(new Control[] { textLabel, inputBox, confirmation });
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text : "";
        }
    }
}
