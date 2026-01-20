using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace PetShopApp
{
    public partial class RecoverPassword : Form
    {
        // UI Controls
        private TextBox txtUser, txtOldPass, txtMathAns, txtNewPass1, txtNewPass2;
        private Button btnVerify, btnUpdate, btnToggleOld, btnToggleNew;
        private Label lblMathChallenge;
        private Panel card, pnlHeader, pnlFooter, pnlMath, pnlIntegration, pnlReset;

        private int mathResult;
        private bool isOldHidden = true, isNewHidden = true;

        private void RecoverPassword_Load(object sender, EventArgs e)
        {

        }

        // SQL Connection String
        string connString = $@"Data Source={Environment.MachineName}\SQLEXPRESS; Initial Catalog=PetShopManagementDB; Integrated Security=True";

        public RecoverPassword()
        {
            InitializeComponent();
            SetupLayout();
            BuildUI();
            GenerateMath();
            this.AcceptButton = btnVerify;
        }

        private void SetupLayout()
        {
            this.Text = "PetShop | Recovery Specialist";
            this.Size = new Size(420, 680);
            this.BackColor = Color.FromArgb(236, 240, 241);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void BuildUI()
        {
            this.Controls.Clear();

            // --- 1. Header Section ---
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = Color.FromArgb(44, 62, 80) };
            Label lblLogo = new Label { Text = "🐾", Font = new Font("Segoe UI", 35), ForeColor = Color.FromArgb(230, 126, 34), Size = new Size(420, 60), Location = new Point(0, 15), TextAlign = ContentAlignment.MiddleCenter };
            Label lblTitle = new Label { Text = "RECOVERY PORTAL", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White, Size = new Size(420, 30), Location = new Point(0, 75), TextAlign = ContentAlignment.MiddleCenter };
            pnlHeader.Controls.Add(lblLogo); pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            // --- 2. Input Card ---
            card = new Panel { Size = new Size(420, 460), Location = new Point(0, 120), BackColor = Color.White };
            this.Controls.Add(card);

            CreateLabel("ACCOUNT USERNAME", 70, 30, card);
            txtUser = CreateTextBox(70, 52, 280, card);

            CreateLabel("LAST REMEMBERED PASSWORD", 70, 100, card);
            txtOldPass = CreateTextBox(70, 122, 235, card);
            txtOldPass.PasswordChar = '●';
            btnToggleOld = CreateToggleButton(305, 122, (s, e) => {
                isOldHidden = !isOldHidden;
                txtOldPass.PasswordChar = isOldHidden ? '●' : '\0';
                btnToggleOld.Text = isOldHidden ? "👁" : "🔒";
            }, card);

            // Math Challenge
            pnlMath = new Panel { Location = new Point(70, 180), Size = new Size(280, 70), BackColor = Color.FromArgb(250, 250, 250), BorderStyle = BorderStyle.FixedSingle };
            lblMathChallenge = new Label { Text = "Loading...", Location = new Point(10, 10), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.DimGray, AutoSize = true };
            txtMathAns = new TextBox { Location = new Point(12, 32), Width = 100, Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle };
            pnlMath.Controls.Add(lblMathChallenge); pnlMath.Controls.Add(txtMathAns);
            card.Controls.Add(pnlMath);

            btnVerify = new Button { Text = "VERIFY IDENTITY", Size = new Size(280, 50), Location = new Point(70, 270), BackColor = Color.FromArgb(230, 126, 34), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
            btnVerify.FlatAppearance.BorderSize = 0;
            btnVerify.Click += HandleVerification;
            card.Controls.Add(btnVerify);

            SetupIntegrationAndReset();

            // --- 3. Footer Section ---
            pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 70, BackColor = Color.FromArgb(44, 62, 80) };
            Panel orangeLine = new Panel { Dock = DockStyle.Top, Height = 4, BackColor = Color.FromArgb(230, 126, 34) };
            Label lblFooter = new Label { Text = "PET SHOP MANAGEMENT SYSTEM\nSecure Password Recovery", ForeColor = Color.FromArgb(189, 195, 199), Font = new Font("Segoe UI", 9), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            pnlFooter.Controls.Add(lblFooter); pnlFooter.Controls.Add(orangeLine);
            this.Controls.Add(pnlFooter);
        }

        private void SetupIntegrationAndReset()
        {
            pnlIntegration = new Panel { Location = new Point(70, 260), Size = new Size(280, 120), BackColor = Color.FromArgb(255, 253, 232), Visible = false, BorderStyle = BorderStyle.FixedSingle };
            Label lblInt = new Label { Text = "FINAL BOSS CHECK:\n∫(3x² + 2x)dx = x³ + x² + C ?", Font = new Font("Segoe UI Semibold", 9), Location = new Point(5, 15), Size = new Size(270, 45), TextAlign = ContentAlignment.MiddleCenter };
            Button btnYes = new Button { Text = "YES", Location = new Point(30, 70), Size = new Size(100, 35), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            Button btnNo = new Button { Text = "NO", Location = new Point(150, 70), Size = new Size(100, 35), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnYes.Click += (s, e) => TransitionToReset(); btnNo.Click += (s, e) => TransitionToReset();
            pnlIntegration.Controls.Add(lblInt); pnlIntegration.Controls.Add(btnYes); pnlIntegration.Controls.Add(btnNo);
            card.Controls.Add(pnlIntegration);

            pnlReset = new Panel { Location = new Point(70, 260), Size = new Size(280, 200), Visible = false };
            CreateLabel("NEW PASSWORD", 0, 0, pnlReset);
            txtNewPass1 = CreateTextBox(0, 22, 235, pnlReset); txtNewPass1.PasswordChar = '●';
            btnToggleNew = CreateToggleButton(240, 22, (s, e) => {
                isNewHidden = !isNewHidden;
                txtNewPass1.PasswordChar = isNewHidden ? '●' : '\0';
                btnToggleNew.Text = isNewHidden ? "👁" : "🔒";
            }, pnlReset);
            CreateLabel("CONFIRM PASSWORD", 0, 65, pnlReset);
            txtNewPass2 = CreateTextBox(0, 87, 280, pnlReset); txtNewPass2.PasswordChar = '●';
            btnUpdate = new Button { Text = "UPDATE NOW", Size = new Size(280, 45), Location = new Point(0, 140), BackColor = Color.FromArgb(44, 62, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnUpdate.Click += HandleFinalUpdate;
            pnlReset.Controls.Add(txtNewPass1); pnlReset.Controls.Add(btnToggleNew); pnlReset.Controls.Add(txtNewPass2); pnlReset.Controls.Add(btnUpdate);
            card.Controls.Add(pnlReset);
        }

        private void GenerateMath() { Random rnd = new Random(); int a = rnd.Next(2, 11), b = rnd.Next(2, 11); mathResult = a + b; lblMathChallenge.Text = $"LOGIC CHECK: {a} + {b} = ?"; }

        private void HandleVerification(object sender, EventArgs e)
        {
            if (txtMathAns.Text != mathResult.ToString()) { MessageBox.Show("Math Error Mama!"); GenerateMath(); return; }
            using (SqlConnection con = new SqlConnection(connString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@u AND Password=@p", con);
                    cmd.Parameters.AddWithValue("@u", txtUser.Text.Trim()); cmd.Parameters.AddWithValue("@p", txtOldPass.Text.Trim());
                    if ((int)cmd.ExecuteScalar() > 0) { btnVerify.Visible = false; pnlIntegration.Visible = true; pnlIntegration.BringToFront(); }
                    else { MessageBox.Show("Account info milteche na!"); }
                }
                catch (Exception ex) { MessageBox.Show("Database Error: " + ex.Message); }
            }
        }

        private void TransitionToReset() { MessageBox.Show("MAMAH JUST KIDDING! 😂\nEibar notun password dao."); pnlIntegration.Visible = false; pnlReset.Visible = true; pnlReset.BringToFront(); this.AcceptButton = btnUpdate; }

        private void HandleFinalUpdate(object sender, EventArgs e)
        {
            if (txtNewPass1.Text != txtNewPass2.Text || string.IsNullOrEmpty(txtNewPass1.Text)) { MessageBox.Show("Password match hoy nai!"); return; }
            using (SqlConnection con = new SqlConnection(connString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE Users SET Password=@p WHERE Username=@u", con);
                    cmd.Parameters.AddWithValue("@p", txtNewPass1.Text); cmd.Parameters.AddWithValue("@u", txtUser.Text.Trim());
                    cmd.ExecuteNonQuery(); MessageBox.Show("Mission Success! Password change hoyeche."); this.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void CreateLabel(string t, int x, int y, Panel p) { p.Controls.Add(new Label { Text = t, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.DimGray }); }
        private TextBox CreateTextBox(int x, int y, int w, Panel p) { TextBox t = new TextBox { Width = w, Location = new Point(x, y), Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle }; p.Controls.Add(t); return t; }
        private Button CreateToggleButton(int x, int y, EventHandler ev, Panel p) { Button b = new Button { Text = "👁", Size = new Size(40, 29), Location = new Point(x, y), FlatStyle = FlatStyle.Popup, BackColor = Color.White }; b.Click += ev; p.Controls.Add(b); return b; }
    }
}