using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace RadheDevelopers.RealEstate
{
    public class UpdatePropertyForm : Form
    {
        TextBox txtId, txtTitle, txtLocation, txtPrice, txtImage;
        Button btnLoad, btnUpdate, btnBrowse;
        PictureBox picPreview;

        public UpdatePropertyForm()
        {
            this.Text = "Update Property - Radhe Developers";
            this.Width = 620;
            this.Height = 440;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            InitializeComponents();
        }

        void InitializeComponents()
        {
            var lblId = new Label() { Text = "Property ID:", Left = 30, Top = 20, Width = 120 };
            txtId = new TextBox() { Left = 160, Top = 18, Width = 120 };

            btnLoad = new Button() { Text = "Load", Left = 300, Top = 16, Width = 80 };
            btnLoad.Click += BtnLoad_Click;

            var lblTitle = new Label() { Text = "Title:", Left = 30, Top = 70, Width = 120 };
            txtTitle = new TextBox() { Left = 160, Top = 68, Width = 420 };

            var lblLocation = new Label() { Text = "Location:", Left = 30, Top = 120, Width = 120 };
            txtLocation = new TextBox() { Left = 160, Top = 118, Width = 420 };

            var lblPrice = new Label() { Text = "Price:", Left = 30, Top = 170, Width = 120 };
            txtPrice = new TextBox() { Left = 160, Top = 168, Width = 200 };

            var lblImage = new Label() { Text = "Image Path:", Left = 30, Top = 220, Width = 120 };
            txtImage = new TextBox() { Left = 160, Top = 218, Width = 300 };

            btnBrowse = new Button() { Text = "Browse", Left = 470, Top = 216, Width = 110 };
            btnBrowse.Click += BtnBrowse_Click;

            picPreview = new PictureBox()
            {
                Left = 160,
                Top = 260,
                Width = 200,
                Height = 120,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            btnUpdate = new Button() { Text = "Update Property", Left = 160, Top = 400 - 40, Width = 160, Height = 36 };
            btnUpdate.Click += BtnUpdate_Click;

            this.Controls.AddRange(new Control[] {
                lblId, txtId, btnLoad,
                lblTitle, txtTitle,
                lblLocation, txtLocation,
                lblPrice, txtPrice,
                lblImage, txtImage, btnBrowse,
                picPreview, btnUpdate
            });
        }

        // Load property details by ID
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Enter Property ID to load.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string sql = "SELECT * FROM Properties WHERE Id=@id";
                DataTable dt = DataAccess.ExecuteSelect(sql, new SqlParameter("@id", txtId.Text.Trim()));

                if (dt.Rows.Count > 0)
                {
                    txtTitle.Text = dt.Rows[0]["Title"].ToString();
                    txtLocation.Text = dt.Rows[0]["Location"].ToString();
                    txtPrice.Text = dt.Rows[0]["Price"].ToString();
                    txtImage.Text = dt.Rows[0]["ImagePath"].ToString();

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(txtImage.Text) && System.IO.File.Exists(txtImage.Text))
                            picPreview.Image = Image.FromFile(txtImage.Text);
                        else
                            picPreview.Image = null;
                    }
                    catch { picPreview.Image = null; }

                    MessageBox.Show("Property loaded successfully!", "Loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Property not found. Check the ID.", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtImage.Text = ofd.FileName;
                try { picPreview.Image = Image.FromFile(ofd.FileName); }
                catch { picPreview.Image = null; }
            }
        }

        // Update property in DB
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Please load a property first (enter ID and click Load).", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTitle.Text) ||
                string.IsNullOrWhiteSpace(txtLocation.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Title, Location and Price are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string sql = "UPDATE Properties SET Title=@t, Location=@l, Price=@p, ImagePath=@i WHERE Id=@id";
                int rows = DataAccess.ExecuteNonQuery(sql,
                    new SqlParameter("@t", txtTitle.Text.Trim()),
                    new SqlParameter("@l", txtLocation.Text.Trim()),
                    new SqlParameter("@p", txtPrice.Text.Trim()),
                    new SqlParameter("@i", txtImage.Text.Trim()),
                    new SqlParameter("@id", txtId.Text.Trim()));

                if (rows > 0)
                    MessageBox.Show("Property updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Update failed. Possibly invalid ID.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
