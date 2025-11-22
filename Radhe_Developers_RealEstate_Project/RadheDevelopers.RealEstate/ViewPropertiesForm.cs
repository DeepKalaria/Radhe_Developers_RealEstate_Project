using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RadheDevelopers.RealEstate
{
    public class ViewPropertiesForm : Form
    {
        private DataGridView dgvProperties;
        private PictureBox picPreview;
        private Button btnClose;

        public ViewPropertiesForm()
        {
            this.Text = "Available Properties";
            this.Width = 1000;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            InitializeComponents();
            LoadProperties();
        }

        private void InitializeComponents()
        {
            dgvProperties = new DataGridView()
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
            dgvProperties.SelectionChanged += DgvProperties_SelectionChanged;

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

            Controls.AddRange(new Control[] { dgvProperties, picPreview, btnClose });
        }

        private void LoadProperties()
        {
            try
            {
                // ✅ Load only properties that are NOT booked
                string sql = "SELECT PropertyID, Title, Location, Price, Size, Description, ImagePath, " +
                             "CASE WHEN IsBooked = 1 THEN 'Booked' ELSE 'Available' END AS Status " +
                             "FROM Properties WHERE IsBooked = 0";

                DataTable dt = DataAccess.GetTable(sql);
                dgvProperties.DataSource = dt;

                dgvProperties.Columns["PropertyID"].HeaderText = "Property ID";
                dgvProperties.Columns["Title"].HeaderText = "Title";
                dgvProperties.Columns["Location"].HeaderText = "Location";
                dgvProperties.Columns["Price"].HeaderText = "Price";
                dgvProperties.Columns["Size"].HeaderText = "Size (sqft)";
                dgvProperties.Columns["Description"].HeaderText = "Description";
                dgvProperties.Columns["Status"].HeaderText = "Status";
                dgvProperties.Columns["ImagePath"].Visible = false; // hide image path column
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading properties: " + ex.Message);
            }
        }

        private void DgvProperties_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProperties.SelectedRows.Count > 0)
            {
                string imagePath = dgvProperties.SelectedRows[0].Cells["ImagePath"].Value?.ToString();

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    try
                    {
                        // Prevent file lock by using stream copy
                        using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                        {
                            picPreview.Image = Image.FromStream(fs);
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
