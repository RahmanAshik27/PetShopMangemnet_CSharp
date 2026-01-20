using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace PetShopApp
{
    public partial class LoginForm : Form
    {
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnTogglePass; 
        private Button btnForgotPass; 
        private bool isPassHidden = true;
        
        private Panel pnlHeader;
      


        string connString = $@"Data Source={Environment.MachineName}\SQLEXPRESS; Initial Catalog=PetShopManagementDB; Integrated Security=True";

        public LoginForm(string userType = "Admin")
        {
            InitializeComponent(); // Prothome eita thakbe
            ConfigureForm(userType); // Form settings
            CreateLoginUI(userType); // EI METHOD CALL NA KORLE DESIGN ASHBE NA
        }

        private void ConfigureForm(string title)
        {
            this.Text = title + " Login Portal";
            this.Size = new Size(420, 600); // Size ektu baralam shob fit korar jonno
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void CreateLoginUI(string userType)
        {
            this.Controls.Clear();

            // --- 1. Header Section (Premium Logo & Title) ---
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 130;
            pnlHeader.BackColor = Color.FromArgb(23, 31, 42);

            Label lblLogo = new Label();
            lblLogo.Text = "🐾";
            lblLogo.Font = new Font("Segoe UI", 40);
            lblLogo.ForeColor = Color.FromArgb(230, 126, 34); // Orange
            lblLogo.Size = new Size(420, 70);
            lblLogo.Location = new Point(0, 10);
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;

            Label lblTitle = new Label();
            lblTitle.Text = userType.ToUpper() + " LOGIN PORTAL";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Size = new Size(420, 30);
            lblTitle.Location = new Point(0, 85);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            pnlHeader.Controls.Add(lblLogo);
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            // --- 2. Input Section ---
            int startX = 60;

            Label lblUser = new Label { Text = "Username", Location = new Point(startX, 150), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.DimGray };
            txtUser = new TextBox { Width = 300, Font = new Font("Segoe UI", 12), Location = new Point(startX, 175), BorderStyle = BorderStyle.FixedSingle };

            Label lblPass = new Label { Text = "Password", Location = new Point(startX, 230), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.DimGray };
            txtPass = new TextBox { Width = 255, Font = new Font("Segoe UI", 12), Location = new Point(startX, 255), PasswordChar = '●', BorderStyle = BorderStyle.FixedSingle };

            // --- TOGGLE PASSWORD BUTTON (👁) ---
            btnTogglePass = new Button
            {
                Text = "👁",
                Size = new Size(45, 29),
                Location = new Point(startX + 255, 255), // txtPass er thik pashe
                BackColor = Color.White,
                FlatStyle = FlatStyle.Popup,
                Cursor = Cursors.Hand
            };
            btnTogglePass.FlatAppearance.BorderSize = 0;
            btnTogglePass.Click += (s, e) => {
                isPassHidden = !isPassHidden;
                txtPass.PasswordChar = isPassHidden ? '●' : '\0';
                btnTogglePass.Text = isPassHidden ? "👁" : "🔒";
            };

            // --- 3. LOGIN BUTTON ---
            Button btnLogin = new Button
            {
                Text = "LOGIN NOW",
                Size = new Size(300, 50),
                Location = new Point(startX, 310),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += HandleLoginAction;
            this.AcceptButton = btnLogin;

            // Forgot Password (New Design)
            btnForgotPass = new Button();
            btnForgotPass.Text = "Forgot Password?";
            btnForgotPass.Size = new Size(280, 25);
            btnForgotPass.Location = new Point(75, 370);
            btnForgotPass.FlatStyle = FlatStyle.Flat;
            btnForgotPass.ForeColor = Color.FromArgb(52, 152, 219);
            btnForgotPass.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnForgotPass.Cursor = Cursors.Hand;
            btnForgotPass.TextAlign = ContentAlignment.MiddleRight;
            btnForgotPass.FlatAppearance.BorderSize = 0;
            btnForgotPass.FlatAppearance.MouseOverBackColor = Color.White;
            btnForgotPass.Click += new EventHandler(BtnForgotPass_Click);

            this.Controls.Add(lblUser); this.Controls.Add(txtUser);
            this.Controls.Add(lblPass); this.Controls.Add(txtPass);
            this.Controls.Add(btnTogglePass);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnForgotPass);

            // --- 5. Footer Section ---
            Panel pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.FromArgb(23, 31, 42) };
            Label lblFooterText = new Label { Text = "Premium Pet Care © 2026", ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Italic), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            pnlFooter.Controls.Add(lblFooterText);
            this.Controls.Add(pnlFooter);
        }

        private void BtnForgotPass_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Mamma, eikhaney amra Password Recovery system boshabo pore!", "Forgot Password");
        }

        // DATABASE LOGIN LOGIC (Tomar dewa logic-ta eikhane thakbe)
        private void HandleLoginAction(object sender, EventArgs e)
        {
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Mama, Username ar Password dorkar!", "Opps", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection(connString))
            {
                try
                {
                    con.Open();
                    // Query-te fixed kore dilam jate shudhu Admin dhukte pare
                    string query = "SELECT UserId FROM Users WHERE Username=@user AND Password=@pass AND UserRole='Admin'";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@user", user);
                    cmd.Parameters.AddWithValue("@pass", pass);

                    object result = cmd.ExecuteScalar(); // Shudhu UserId return korbe jodi thake

                    if (result != null)
                    {
                        // ✅ 1. Username ta global session-e rakha jate next page pabe
                        UserSession.CurrentUsername = user;
                        UserSession.CurrentUserID = Convert.ToInt32(result);

                        // ✅ 2. Admin Dashboard e jawa
                        AdminDashboard dashboard = new AdminDashboard();
                        dashboard.Show();
                        this.Hide();
                    }
                    else
                    {
                        // ❌ 3. Database-e role check koro 'Admin' (A boro hater kina dekhish)
                        MessageBox.Show("Oops! Admin username ba password milche na mama.", "Login Failed");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}