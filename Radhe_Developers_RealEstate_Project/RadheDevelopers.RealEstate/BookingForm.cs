using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RadheDevelopers.RealEstate
{
    public class BookingForm : Form
    {
        int userId, propertyId;
        Label lblInfo;
        Button btnBook;

        public BookingForm(int uId, int pId)
        {
            userId = uId;
            propertyId = pId;

            this.Text = "Book Property";
            this.Width = 420;
            this.Height = 220;

            InitializeComponents();
        }

        void InitializeComponents()
        {
            lblInfo = new Label()
            {
                Left = 20,
                Top = 20,
                Width = 360,
                Height = 60,
                Text = $"You are about to book property ID: {propertyId}"
            };

            btnBook = new Button()
            {
                Text = "Confirm Booking",
                Left = 130,
                Top = 90,
                Width = 150
            };
            btnBook.Click += BtnBook_Click;

            Controls.AddRange(new Control[] { lblInfo, btnBook });
        }

        private void BtnBook_Click(object sender, EventArgs e)
        {
            try
            {
                // ✅ Step 1: Check if property is already booked
                string checkSql = "SELECT IsBooked FROM Properties WHERE PropertyID = @id";
                object result = DataAccess.ExecuteScalar(checkSql, new SqlParameter("@id", propertyId));

                if (result != null && Convert.ToBoolean(result))
                {
                    MessageBox.Show("❌ This property is already booked!", "Booking Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ✅ Step 2: Insert booking record
                string insertSql = "INSERT INTO Bookings (UserID, PropertyID, BookingDate) VALUES (@u, @p, @d)";
                int r = DataAccess.ExecuteNonQuery(insertSql,
                    new SqlParameter("@u", userId),
                    new SqlParameter("@p", propertyId),
                    new SqlParameter("@d", DateTime.Now));

                if (r > 0)
                {
                    // ✅ Step 3: Mark property as booked
                    string updateSql = "UPDATE Properties SET IsBooked = 1 WHERE PropertyID = @id";
                    DataAccess.ExecuteNonQuery(updateSql, new SqlParameter("@id", propertyId));

                    MessageBox.Show("✅ Booking confirmed successfully!");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Booking failed. Please try again.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error booking property: " + ex.Message);
            }
        }
    }
}
