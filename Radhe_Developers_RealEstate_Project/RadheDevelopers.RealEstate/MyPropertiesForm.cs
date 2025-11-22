using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RadheDevelopers.RealEstate
{
    public class MyPropertiesForm : Form
    {
        private int userId;
        private DataGridView dgvMyProperties;
        private PictureBox picPreview;
        private Button btnClose;

        public MyPropertiesForm(int uId)
        {
            userId = uId;

            this.Text = "My Booked Properties";
            this.Width = 1000;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            InitializeComponents();
            LoadMyProperties();
        }

        private void InitializeComponents()
        {
            dgvMyProperties = new DataGridView()
            {
                Left = 20,
                Top = 20,
                Width = 600,
                Height = 500,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvMyProperties.SelectionChanged += DgvMyProperties_SelectionChanged;

            picPreview = new PictureBox()
            {
                Left = 650,
                Top = 50,
                Width = 300,
                Height = 250,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };

            btnClose = new Button()
            {
                Text = "Close",
                Left = 780,
                Top = 500,
                Width = 120,
                Height = 40,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClose.Click += (s, e) => this.Close();

            Controls.AddRange(new Control[] { dgvMyProperties, picPreview, btnClose });
        }

        private void LoadMyProperties()
        {
            try
            {
                string sql = @"
                    SELECT 
                        p.PropertyID, 
                        p.Title, 
                        p.Location, 
                        p.Price, 
                        p.Size, 
                        p.Description, 
                        p.ImagePath,
                        b.BookingDate
                    FROM Bookings b
                    INNER JOIN Properties p ON b.PropertyID = p.PropertyID
                    WHERE b.UserID = @u";

                SqlParameter[] prms = { new SqlParameter("@u", userId) };
                DataTable dt = DataAccess.GetTable(sql, prms);

                if (!dt.Columns.Contains("Size"))
                {
                    MessageBox.Show("⚠️ Column 'Size' not found in the Properties table. Please check your database structure.",
                                    "Missing Column", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                dgvMyProperties.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading booked properties: " + ex.Message);
            }
        }

        private void DgvMyProperties_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMyProperties.SelectedRows.Count > 0)
            {
                string imagePath = dgvMyProperties.SelectedRows[0].Cells["ImagePath"].Value?.ToString();

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    try
                    {
                        // Avoid file lock by using MemoryStream
                        using (var imgStream = new MemoryStream(File.ReadAllBytes(imagePath)))
                        {
                            picPreview.Image = Image.FromStream(imgStream);
                        }
                    }
                    catch
                    {
                        picPreview.Image = null;
                    }
                }
                else
                {
                    picPreview.Image = null;
                }
            }
        }
    }
}
