using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace PetShopApp
{
    public partial class CreateAccount : Form
    {
      
        private TextBox txtFullName, txtUsername, txtEmail, txtPhone, txtPass, txtAddress;
        private Button btnRegister, btnTogglePass, btnBackToLogin;
        private bool isPassHidden = true;
        private Panel pnlHeader, pnlFooter;

        private void CreateAccount_Load(object sender, EventArgs e)
        {

        }

      
        string connString = $@"Data Source={Environment.MachineName}\SQLEXPRESS; Initial Catalog=PetShopManagementDB; Integrated Security=True";

        public CreateAccount()
        {
            InitializeComponent();
            SetupRegistrationForm();
            BuildRegisterUI();
        }

        private void SetupRegistrationForm()
        {
            this.Text = "PetShop | Create New Account";
            this.Size = new Size(450, 735);
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void BuildRegisterUI()
        {
            this.Controls.Clear();

      
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = Color.FromArgb(23, 31, 42) };
            Label lblLogo = new Label
            {
                Text = "🐾",
                Font = new Font("Segoe UI", 35),
                ForeColor = Color.FromArgb(230, 126, 34),
                Size = new Size(450, 60),
                Location = new Point(0, 10),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Label lblTitle = new Label
            {
                Text = "JOIN THE PET FAMILY",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(450, 25),
                Location = new Point(0, 75),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlHeader.Controls.Add(lblLogo); pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

      
            int startX = 50;
            int startY = 135;
            int gap = 62;

            CreateLabel("FULL NAME", startX, startY);
            txtFullName = CreateTextBox(startX, startY + 20);

            CreateLabel("CHOOSE USERNAME", startX, startY + gap);
            txtUsername = CreateTextBox(startX, startY + gap + 20);

            CreateLabel("EMAIL ADDRESS", startX, startY + (gap * 2));
            txtEmail = CreateTextBox(startX, startY + (gap * 2) + 20);

            CreateLabel("PHONE NUMBER (11 DIGITS)", startX, startY + (gap * 3));
            txtPhone = CreateTextBox(startX, startY + (gap * 3) + 20);

            CreateLabel("SET PASSWORD", startX, startY + (gap * 4));
            txtPass = CreateTextBox(startX, startY + (gap * 4) + 20, true);

            btnTogglePass = new Button
            {
                Text = "👁",
                Size = new Size(40, 29),
                Location = new Point(startX + 310, startY + (gap * 4) + 20),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Popup,
                Cursor = Cursors.Hand
            };
            btnTogglePass.Click += (s, e) => {
                isPassHidden = !isPassHidden;
                txtPass.PasswordChar = isPassHidden ? '●' : '\0';
                btnTogglePass.Text = isPassHidden ? "👁" : "🔒";
            };
            this.Controls.Add(btnTogglePass);

      
            CreateLabel("HOME ADDRESS", startX, startY + (gap * 5));
            txtAddress = CreateTextBox(startX, startY + (gap * 5) + 20);

      
            string placeholder = "Please enter exact delivery address";
            txtAddress.Text = placeholder;
            txtAddress.ForeColor = Color.Gray; 

      
            txtAddress.Enter += (s, e) => {
                if (txtAddress.Text == placeholder)
                {
                    txtAddress.Text = "";
                    txtAddress.ForeColor = Color.Black; 
                }
            };

            
            txtAddress.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtAddress.Text))
                {
                    txtAddress.Text = placeholder;
                    txtAddress.ForeColor = Color.Gray;
                }
            };

            
            btnRegister = new Button
            {
                Text = "CREATE ACCOUNT",
                Size = new Size(350, 50),
                Location = new Point(startX, 520),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += HandleRegistration; 
            this.Controls.Add(btnRegister);

            
            btnBackToLogin = new Button
            {
                Text = "Already have an account? Login here",
                Size = new Size(350, 30),
                Location = new Point(startX, 580),

                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnBackToLogin.Click += (s, e) => this.Close();
            this.Controls.Add(btnBackToLogin);

            
            pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.FromArgb(23, 31, 42) };
            Label lblFooter = new Label
            {
                Text = "Premium Pet Care System © 2026 | Designed with ❤️",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Italic),
                Size = new Size(450, 40),
                Location = new Point(0, 5),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlFooter.Controls.Add(lblFooter);
            this.Controls.Add(pnlFooter);
        }

        private void CreateLabel(string text, int x, int y)
        {
            Label lbl = new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.DimGray };
            this.Controls.Add(lbl);
        }

        private TextBox CreateTextBox(int x, int y, bool isPass = false)
        {
            TextBox tb = new TextBox { Width = 350, Location = new Point(x, y), Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle };
            if (isPass) { tb.Width = 310; tb.PasswordChar = '●'; }
            this.Controls.Add(tb);
            return tb;
        }

        private void HandleRegistration(object sender, EventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show("Mama, shob gula ghor thik moto puron koro!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            if (txtPass.Text.Length < 6)
            {
                MessageBox.Show("Mama, password kom-pakkhe 6 digit-er hote hobe!", "Security Error");
                return;
            }

            
            string phone = txtPhone.Text.Trim();
            if (phone.Length != 11 || !phone.StartsWith("01"))
            {
                MessageBox.Show("Mama, valid 11 digit BD phone number dao (01xx...)!", "Error");
                return;
            }

            
            string email = txtEmail.Text.Trim().ToLower();
            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Mama, email format-ta thik koro!", "Error");
                return;
            }

            
            using (SqlConnection con = new SqlConnection(connString))
            {
                try
                {
                    con.Open();
            
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@u", con);
                    checkCmd.Parameters.AddWithValue("@u", txtUsername.Text.Trim());
                    if ((int)checkCmd.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("Mama, ei Username-e account ache! Onno kisu dao.", "Taken");
                        return;
                    }

            
                    string q = "INSERT INTO Users (Username, Password, FullName, Email, Phone, Address, UserRole) VALUES (@u, @p, @n, @e, @ph, @a, 'Customer')";
                    SqlCommand cmd = new SqlCommand(q, con);

            
                    cmd.Parameters.AddWithValue("@u", txtUsername.Text.Trim());
                    cmd.Parameters.AddWithValue("@p", txtPass.Text); 
                    cmd.Parameters.AddWithValue("@n", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@ph", phone);
                    cmd.Parameters.AddWithValue("@a", txtAddress.Text.Trim());

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Shabash Mama! Account khola hoye gese।", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch (Exception ex) { MessageBox.Show("Database Error: " + ex.Message); }
            }
        }
    }
}