using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace RadheDevelopers.RealEstate
{
    public class RegisterForm : Form
    {
        TextBox txtName, txtEmail, txtPassword;
        Button btnRegister;

        public RegisterForm()
        {
            Text = "Register New User";
            Width = 360;
            Height = 280;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            InitializeComponents();
        }

        void InitializeComponents()
        {
            Label lblName = new Label() { Text = "Name", Left = 40, Top = 40, Width = 100 };
            txtName = new TextBox() { Left = 140, Top = 40, Width = 150 };

            Label lblEmail = new Label() { Text = "Email", Left = 40, Top = 80, Width = 100 };
            txtEmail = new TextBox() { Left = 140, Top = 80, Width = 150 };

            Label lblPassword = new Label() { Text = "Password", Left = 40, Top = 120, Width = 100 };
            txtPassword = new TextBox() { Left = 140, Top = 120, Width = 150, UseSystemPasswordChar = true };

            btnRegister = new Button()
            {
                Text = "Register",
                Left = 120,
                Top = 170,
                Width = 100,
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;

            Controls.AddRange(new Control[] { lblName, txtName, lblEmail, txtEmail, lblPassword, txtPassword, btnRegister });
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "" || txtEmail.Text.Trim() == "" || txtPassword.Text.Trim() == "")
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            string sql = "INSERT INTO Users (Name, Email, Password) VALUES (@n,@e,@p)";
            int r = DataAccess.ExecuteNonQuery(sql,
                new SqlParameter("@n", txtName.Text.Trim()),
                new SqlParameter("@e", txtEmail.Text.Trim()),
                new SqlParameter("@p", txtPassword.Text.Trim()));

            if (r > 0)
            {
                MessageBox.Show("Registration successful!");
                Close();
            }
            else
            {
                MessageBox.Show("Registration failed.");
            }
        }
    }
}
