using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RadheDevelopers.RealEstate
{
    public class AdminDashboard : Form
    {
        private DataGridView dgvProps, dgvSummary;
        private Button btnAddProp, btnRefresh, btnViewBookings;
        private Label lblTitle, lblSummary;
        private int userId;

        public AdminDashboard(int userId)
        {
            this.userId = userId;

            Text = "🏠 Radhe Developers - Admin Dashboard";
            Width = 1000;
            Height = 650;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10);

            InitializeComponents();
            LoadProperties();
            LoadPropertySummary();
        }

        private void InitializeComponents()
        {
            // Title label
            lblTitle = new Label()
            {
                Text = "📋 Property Management Dashboard",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 55, 71),
                AutoSize = true,
                Left = 25,
                Top = 15
            };

            // Gradient header panel
            Panel headerPanel = new Panel()
            {
                Left = 0,
                Top = 0,
                Width = this.Width,
                Height = 70,
                BackColor = Color.FromArgb(52, 152, 219)
            };
            headerPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    headerPanel.ClientRectangle,
                    Color.FromArgb(52, 152, 219),
                    Color.FromArgb(41, 128, 185),
                    0f
                );
                g.FillRectangle(brush, headerPanel.ClientRectangle);
                g.DrawString("🏠 Radhe Developers Admin Panel", new Font("Segoe UI", 16, FontStyle.Bold),
                    Brushes.White, new PointF(20, 18));
            };

            // Property Summary Table label
            lblSummary = new Label()
            {
                Text = "📊 Property Summary",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.DarkSlateGray,
                Left = 25,
                Top = 85,
                AutoSize = true
            };

            // Property Summary DataGridView
            dgvSummary = new DataGridView()
            {
                Left = 25,
                Top = 115,
                Width = 930,
                Height = 150,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false
            };

            // Main Properties Table
            dgvProps = new DataGridView()
            {
                Left = 25,
                Top = 290,
                Width = 930,
                Height = 250,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Buttons
            btnAddProp = CreateStyledButton("➕ Add Property", 25, 560, Color.FromArgb(46, 204, 113));
            btnAddProp.Click += BtnAddProp_Click;

            btnRefresh = CreateStyledButton("🔄 Refresh", 200, 560, Color.FromArgb(52, 152, 219));
            btnRefresh.Click += (s, e) =>
            {
                LoadProperties();
                LoadPropertySummary();
            };

            btnViewBookings = CreateStyledButton("📑 View Bookings", 350, 560, Color.FromArgb(243, 156, 18));
            btnViewBookings.Click += BtnViewBookings_Click;

            Controls.AddRange(new Control[]
            {
                headerPanel, lblSummary, dgvSummary, dgvProps,
                btnAddProp, btnRefresh, btnViewBookings
            });
        }

        private Button CreateStyledButton(string text, int left, int top, Color bg)
        {
            var btn = new Button()
            {
                Text = text,
                Left = left,
                Top = top,
                Width = 150,
                Height = 40,
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Light(bg);
            btn.MouseLeave += (s, e) => btn.BackColor = bg;
            return btn;
        }

        private void BtnAddProp_Click(object sender, EventArgs e)
        {
            try
            {
                var addForm = new AddPropertyForm(userId);
                addForm.FormClosed += (s, ev) =>
                {
                    LoadProperties();
                    LoadPropertySummary();
                };
                addForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Cannot open Add Property form:\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load properties in main table
        private void LoadProperties()
        {
            try
            {
                var dt = DataAccess.ExecuteSelect(
                    "SELECT PropertyID, Title, Location, Price, Size, Description, ImagePath, IsBooked FROM Properties ORDER BY PropertyID DESC"
                );
                dgvProps.DataSource = dt;

                if (dgvProps.Columns["PropertyID"] != null)
                    dgvProps.Columns["PropertyID"].HeaderText = "ID";
                if (dgvProps.Columns["Title"] != null)
                    dgvProps.Columns["Title"].HeaderText = "Title";
                if (dgvProps.Columns["Location"] != null)
                    dgvProps.Columns["Location"].HeaderText = "Location";
                if (dgvProps.Columns["Price"] != null)
                    dgvProps.Columns["Price"].HeaderText = "Price (₹)";
                if (dgvProps.Columns["Size"] != null)
                    dgvProps.Columns["Size"].HeaderText = "Size";
                if (dgvProps.Columns["IsBooked"] != null)
                    dgvProps.Columns["IsBooked"].HeaderText = "Booked?";
            }
            catch (Exception ex)
            {
                MessageBox.Show("⚠️ Error loading properties:\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load property summary table (who added & who booked)
        private void LoadPropertySummary()
        {
            try
            {
                var dt = DataAccess.ExecuteSelect(@"
                    SELECT 
                        p.PropertyID AS [Property ID],
                        p.Title AS [Property Title],
                        u1.Name AS [Added By],
                        ISNULL(u2.Name, '— Not Booked —') AS [Purchased By],
                        CASE WHEN p.IsBooked = 1 THEN '✔ Booked' ELSE '❌ Available' END AS [Status]
                    FROM Properties p
                    JOIN Users u1 ON p.UserId = u1.UserId
                    LEFT JOIN Bookings b ON p.PropertyID = b.PropertyID
                    LEFT JOIN Users u2 ON b.UserID = u2.UserID
                    ORDER BY p.PropertyID DESC
                ");

                dgvSummary.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("⚠️ Error loading property summary:\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // View all bookings
        private void BtnViewBookings_Click(object sender, EventArgs e)
        {
            try
            {
                var dt = DataAccess.ExecuteSelect(@"
                    SELECT 
                        b.BookingID, 
                        u.Name AS [Customer Name], 
                        p.Title AS [Property Title], 
                        b.BookingDate 
                    FROM Bookings b
                    JOIN Users u ON b.UserID = u.UserID
                    JOIN Properties p ON b.PropertyID = p.PropertyID
                    ORDER BY b.BookingDate DESC");

                Form f = new Form()
                {
                    Text = "All Bookings - Radhe Developers",
                    Width = 800,
                    Height = 450,
                    StartPosition = FormStartPosition.CenterParent,
                    BackColor = Color.White
                };

                DataGridView dgv = new DataGridView()
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    BackgroundColor = Color.White,
                    DataSource = dt
                };

                f.Controls.Add(dgv);
                f.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("⚠️ Error loading bookings:\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
