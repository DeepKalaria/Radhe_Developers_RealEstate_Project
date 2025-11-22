using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace RadheDevelopers.RealEstate
{
    public class LoginForm : Form
    {
        private TextBox txtEmail, txtPassword;
        private Button btnLogin, btnRegister;
        private CheckBox chkAdmin; // ✅ Added

        public LoginForm()
        {
            this.Text = "Radhe Developers - Login";
            this.Width = 380;
            this.Height = 300;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Label lblEmail = new Label()
            {
                Text = "Email / Username",
                Left = 40,
                Top = 40,
                Width = 140
            };
            txtEmail = new TextBox()
            {
                Left = 180,
                Top = 40,
                Width = 140
            };

            Label lblPassword = new Label()
            {
                Text = "Password",
                Left = 40,
                Top = 80,
                Width = 100
            };
            txtPassword = new TextBox()
            {
                Left = 180,
                Top = 80,
                Width = 140,
                UseSystemPasswordChar = true
            };

            chkAdmin = new CheckBox()
            {
                Text = "Login as Admin",
                Left = 40,
                Top = 120,
                Width = 200
            };

            btnLogin = new Button()
            {
                Text = "Login",
                Left = 70,
                Top = 170,
                Width = 100,
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            btnRegister = new Button()
            {
                Text = "Register",
                Left = 200,
                Top = 170,
                Width = 100,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;

            Controls.AddRange(new Control[]
            {
                lblEmail, txtEmail,
                lblPassword, txtPassword,
                chkAdmin,
                btnLogin, btnRegister
            });
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            using (RegisterForm register = new RegisterForm())
            {
                register.ShowDialog();
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string emailOrUsername = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(emailOrUsername) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both Email/Username and Password", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (chkAdmin.Checked)
                {
                    //  Admin login check
                    string adminQuery = "SELECT AdminID, Username FROM Admin WHERE Username=@Username AND Password=@Password";
                    SqlParameter[] adminParams = {
                        new SqlParameter("@Username", emailOrUsername),
                        new SqlParameter("@Password", password)
                    };

                    DataTable dtAdmin = DataAccess.GetTable(adminQuery, adminParams);
                    if (dtAdmin.Rows.Count > 0)
                    {
                        int adminId = Convert.ToInt32(dtAdmin.Rows[0]["AdminID"]);
                        string username = dtAdmin.Rows[0]["Username"].ToString();

                        MessageBox.Show($"Welcome Admin, {username}!", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        new AdminDashboard(adminId).Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Invalid admin credentials.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // ✅ Normal user login
                    string userQuery = "SELECT UserID, Name, Role FROM Users WHERE Email=@Email AND Password=@Password";
                    SqlParameter[] userParams = {
                        new SqlParameter("@Email", emailOrUsername),
                        new SqlParameter("@Password", password)
                    };

                    DataTable dtUser = DataAccess.GetTable(userQuery, userParams);
                    if (dtUser.Rows.Count > 0)
                    {
                        int userId = Convert.ToInt32(dtUser.Rows[0]["UserID"]);
                        string name = dtUser.Rows[0]["Name"].ToString();
                        string role = dtUser.Rows[0]["Role"].ToString();

                        MessageBox.Show($"Welcome, {name}!", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        new UserDashboard(userId, name).Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Invalid credentials. Try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database Error: {sqlEx.Message}", "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
