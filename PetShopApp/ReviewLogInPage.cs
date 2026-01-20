using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PetShopApp
{
    public partial class ReviewLogInPage : Form
    {

        
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnLogin;
        private Button btnTogglePass;
        private Button btnForgotPass;
        private bool isPassHidden = true;
        private Label lblUserTitle, lblPassTitle;
        private Panel card;
        private Panel pnlHeader;
        private Panel pnlFooter;

      
        string connString = $@"Data Source={Environment.MachineName}\SQLEXPRESS; Initial Catalog=PetShopManagementDB; Integrated Security=True";

        public ReviewLogInPage()
        {
            InitializeComponent();
            SetupReviewFormLayout();
            BuildMyUI();
        }
        private void SetupReviewFormLayout()
        {
            this.Text = "PetShop | Customer Secure Login";
            this.Size = new Size(420, 650);
            this.MinimumSize = new Size(420, 650);
            this.MaximumSize = new Size(420, 650);
            this.BackColor = Color.FromArgb(236, 240, 241);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void BuildMyUI()
        {
           
            this.Controls.Clear();

            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 120;
            pnlHeader.BackColor = Color.FromArgb(23, 31, 42);

            Label lblLogo = new Label();
            lblLogo.Text = "🐾";
            lblLogo.Font = new Font("Segoe UI", 35);
            lblLogo.ForeColor = Color.FromArgb(230, 126, 34);
            lblLogo.Size = new Size(420, 60);
            lblLogo.Location = new Point(0, 15);
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;

            Label lblTitle = new Label();
            lblTitle.Text = "REVIEWER PORTAL";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Size = new Size(420, 30);
            lblTitle.Location = new Point(0, 75);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            pnlHeader.Controls.Add(lblLogo);
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);



           
            card = new Panel();
            card.Size = new Size(420, 460);
            card.Location = new Point(0, 120);
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.None;
            this.Controls.Add(card);


            lblUserTitle = new Label();
            lblUserTitle.Text = "USERNAME";
            lblUserTitle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblUserTitle.ForeColor = Color.DimGray;
            lblUserTitle.Location = new Point(70, 40);
            lblUserTitle.AutoSize = true;

            txtUser = new TextBox();
            txtUser.Width = 280;
            txtUser.Location = new Point(70, 65);
            txtUser.Font = new Font("Segoe UI", 12);
            txtUser.BorderStyle = BorderStyle.FixedSingle;

           
            lblPassTitle = new Label();
            lblPassTitle.Text = "PASSWORD";
            lblPassTitle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblPassTitle.ForeColor = Color.DimGray;
            lblPassTitle.Location = new Point(70, 120);
            lblPassTitle.AutoSize = true;

            txtPass = new TextBox();
            txtPass.Width = 235;
            txtPass.Location = new Point(70, 145);
            txtPass.Font = new Font("Segoe UI", 12);
            txtPass.PasswordChar = '●';
            txtPass.BorderStyle = BorderStyle.FixedSingle;

          
            btnTogglePass = new Button();
            btnTogglePass.Text = "👁";
            btnTogglePass.Size = new Size(40, 29);
            btnTogglePass.Location = new Point(305, 145);
            btnTogglePass.BackColor = Color.White;
            btnTogglePass.FlatStyle = FlatStyle.Popup;
            btnTogglePass.Cursor = Cursors.Hand;
            btnTogglePass.FlatAppearance.BorderSize = 0;
            btnTogglePass.Click += new EventHandler(BtnTogglePass_Click);

            
            btnLogin = new Button();
            btnLogin.Text = "LOGIN";
            btnLogin.Size = new Size(280, 50);
            btnLogin.Location = new Point(70, 220);
            btnLogin.BackColor = Color.FromArgb(230, 126, 34);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += new EventHandler(CustomerLogin_Click);

            
            btnForgotPass = new Button();
            btnForgotPass.Text = "Forgot Password?";
            btnForgotPass.Size = new Size(280, 25);
            btnForgotPass.Location = new Point(70, 280);
            btnForgotPass.FlatStyle = FlatStyle.Flat;
            btnForgotPass.ForeColor = Color.FromArgb(52, 152, 219);
            btnForgotPass.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnForgotPass.Cursor = Cursors.Hand;
            btnForgotPass.TextAlign = ContentAlignment.MiddleRight;
            btnForgotPass.FlatAppearance.BorderSize = 0;
            btnForgotPass.FlatAppearance.MouseOverBackColor = Color.White;
            btnForgotPass.Click += new EventHandler(BtnForgotPass_Click);

            Label lblReg = new Label();
            lblReg.Text = "Don't have an account? Create one";
            lblReg.Size = new Size(280, 24);
            lblReg.Location = new Point(70, 340);
            lblReg.TextAlign = ContentAlignment.MiddleCenter;
            lblReg.ForeColor = Color.Gray;
            lblReg.Font = new Font("Segoe UI", 9, FontStyle.Underline);
            lblReg.Cursor = Cursors.Hand;
            lblReg.Click += new EventHandler(lblReg_Click);


            card.Controls.Add(lblUserTitle); card.Controls.Add(txtUser);
            card.Controls.Add(lblPassTitle); card.Controls.Add(txtPass);
            card.Controls.Add(btnTogglePass); card.Controls.Add(btnLogin);
            card.Controls.Add(btnForgotPass); card.Controls.Add(lblReg);
            this.AcceptButton = btnLogin;

            pnlFooter = new Panel();
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Height = 70;
            pnlFooter.BackColor = Color.FromArgb(44, 62, 80);

            Panel orangeLine = new Panel();
            orangeLine.Dock = DockStyle.Top;
            orangeLine.Height = 4;
            orangeLine.BackColor = Color.FromArgb(230, 126, 34);

            Label lblFooterText = new Label();
            lblFooterText.Text = "PREMIUM PET CARE SYSTEM\nBuild 2026 • Secure Access";
            lblFooterText.ForeColor = Color.FromArgb(189, 195, 199);
            lblFooterText.Font = new Font("Segoe UI", 9);
            lblFooterText.Dock = DockStyle.Fill;
            lblFooterText.TextAlign = ContentAlignment.MiddleCenter;
            lblFooterText.BackColor = Color.Transparent;

            pnlFooter.Controls.Add(lblFooterText);
            pnlFooter.Controls.Add(orangeLine);
            this.Controls.Add(pnlFooter);

            pnlFooter.BringToFront();
            txtUser.Focus();
        }


        private void BtnTogglePass_Click(object sender, EventArgs e)
        {
            isPassHidden = !isPassHidden;
            txtPass.PasswordChar = isPassHidden ? '●' : '\0';
            btnTogglePass.Text = isPassHidden ? "👁" : "🔒";
        }

        private void BtnForgotPass_Click(object sender, EventArgs e)
        {
          
            RecoverPassword recoveryPage = new RecoverPassword();

            recoveryPage.ShowDialog();
        }
        private void lblReg_Click(object sender, EventArgs e)
        {
            CreateAccount regForm = new CreateAccount();

            
            regForm.Show();

            
        }

        private void ReviewLogInPage_Load(object sender, EventArgs e)
        {

        }

        private void CustomerLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show("Mamah, username ar pass faka rakha jabe na!", "Input Missing");
                return;
            }

            using (SqlConnection con = new SqlConnection(connString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT Username FROM Users WHERE Username=@user AND Password=@pass AND UserRole='Customer'";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@user", txtUser.Text.Trim());
                    cmd.Parameters.AddWithValue("@pass", txtPass.Text.Trim());

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        string loggedUser = dr["Username"].ToString();

                        MessageBox.Show($"Welcome {loggedUser}! \nPress Enter to proceed to Pet Universe.", "Login Success");

                        GiveReview dashboard = new GiveReview(loggedUser);
                        dashboard.Show();

                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("ID/Password thik nai!");
                    }
                }
                catch (Exception ex) { MessageBox.Show("DB Error: " + ex.Message); }
            }
        }
    }
}
