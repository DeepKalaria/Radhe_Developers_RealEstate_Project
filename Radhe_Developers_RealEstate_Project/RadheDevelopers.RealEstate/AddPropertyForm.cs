using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RadheDevelopers.RealEstate
{
    public class AddPropertyForm : Form
    {
        private TextBox txtTitle, txtLocation, txtPrice, txtSize, txtDescription, txtImage;
        private Button btnBrowse, btnSave;
        private PictureBox picPreview;

        private int currentUserId; // Logged-in user ID
        private static string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=RealEstateDB;Integrated Security=True;";

        public AddPropertyForm(int userId)
        {
            currentUserId = userId;

            Text = "Add New Property - Radhe Developers";
            Width = 650;
            Height = 620;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.WhiteSmoke;
            Font = new Font("Segoe UI", 10);

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            var lblTitle = new Label() { Text = "Title:", Left = 30, Top = 40, Width = 120 };
            txtTitle = new TextBox() { Left = 160, Top = 38, Width = 420 };

            var lblLocation = new Label() { Text = "Location:", Left = 30, Top = 90, Width = 120 };
            txtLocation = new TextBox() { Left = 160, Top = 88, Width = 420 };

            var lblPrice = new Label() { Text = "Price (₹):", Left = 30, Top = 140, Width = 120 };
            txtPrice = new TextBox() { Left = 160, Top = 138, Width = 420 };

            var lblSize = new Label() { Text = "Size (sq. ft):", Left = 30, Top = 190, Width = 120 };
            txtSize = new TextBox() { Left = 160, Top = 188, Width = 420 };

            var lblDescription = new Label() { Text = "Description:", Left = 30, Top = 240, Width = 120 };
            txtDescription = new TextBox()
            {
                Left = 160,
                Top = 238,
                Width = 420,
                Height = 80,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            var lblImage = new Label() { Text = "Property Image:", Left = 30, Top = 340, Width = 120 };
            txtImage = new TextBox() { Left = 160, Top = 338, Width = 280, ReadOnly = true };

            btnBrowse = new Button()
            {
                Text = "Browse...",
                Left = 450,
                Top = 336,
                Width = 130,
                BackColor = Color.FromArgb(3, 169, 244),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.Click += BtnBrowse_Click;

            picPreview = new PictureBox()
            {
                Left = 160,
                Top = 380,
                Width = 230,
                Height = 150,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.White
            };

            btnSave = new Button()
            {
                Text = "💾 Save Property",
                Left = 160,
                Top = 550,
                Width = 200,
                Height = 40,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            Controls.AddRange(new Control[]
            {
                lblTitle, txtTitle,
                lblLocation, txtLocation,
                lblPrice, txtPrice,
                lblSize, txtSize,
                lblDescription, txtDescription,
                lblImage, txtImage, btnBrowse,
                picPreview, btnSave
            });
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Select Property Image"
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtImage.Text = ofd.FileName;
                    try
                    {
                        picPreview.Image = Image.FromFile(ofd.FileName);
                    }
                    catch
                    {
                        picPreview.Image = null;
                        MessageBox.Show("Could not load image.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // ✅ Validation
            if (string.IsNullOrWhiteSpace(txtTitle.Text) ||
                string.IsNullOrWhiteSpace(txtLocation.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text) ||
                string.IsNullOrWhiteSpace(txtSize.Text))
            {
                MessageBox.Show("Please fill in all required fields (Title, Location, Price, Size).",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal priceValue))
            {
                MessageBox.Show("Please enter a valid numeric value for Price.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ Save image (optional)
            string imagePathToSave = txtImage.Text;
            try
            {
                if (!string.IsNullOrWhiteSpace(txtImage.Text) && File.Exists(txtImage.Text))
                {
                    string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                    Directory.CreateDirectory(imagesFolder);

                    string fileName = Path.GetFileName(txtImage.Text);
                    string destinationPath = Path.Combine(imagesFolder, fileName);

                    File.Copy(txtImage.Text, destinationPath, true);
                    imagePathToSave = destinationPath;
                }
            }
            catch
            {
                MessageBox.Show("Could not copy image file. Continuing without saving image.",
                    "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // ✅ Correct SQL Query including Size
            string sql = @"INSERT INTO Properties 
                           (Title, Location, Price, Size, Description, ImagePath, UserId, IsBooked)
                           VALUES (@t, @l, @p, @s, @d, @i, @u, 0)";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@t", txtTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@l", txtLocation.Text.Trim());
                        cmd.Parameters.AddWithValue("@p", priceValue);
                        cmd.Parameters.AddWithValue("@s", txtSize.Text.Trim());
                        cmd.Parameters.AddWithValue("@d", txtDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@i", imagePathToSave);
                        cmd.Parameters.AddWithValue("@u", currentUserId);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("✅ Property added successfully!",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearForm();
                        }
                        else
                        {
                            MessageBox.Show("❌ Failed to add property.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtTitle.Clear();
            txtLocation.Clear();
            txtPrice.Clear();
            txtSize.Clear();
            txtDescription.Clear();
            txtImage.Clear();
            picPreview.Image = null;
        }
    }
}
