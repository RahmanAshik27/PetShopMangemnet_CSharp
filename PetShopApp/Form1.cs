using System;
using System.Data.SqlClient; // 1. SQL Namespace add korlam
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PetShopApp
{
    public partial class Form1 : Form
    {
        // 2. Connection String - Dynamic PC Name detect korbe
        string connString = $@"Data Source={Environment.MachineName}\SQLEXPRESS; Initial Catalog=PetShopManagementDB; Integrated Security=True";

        public Form1()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Controls.Clear();
            InitializePetShopUI();
            this.CenterToScreen();

            // Database connection check korar jonno (Optional testing)
            TestConnection();
        }

        private void TestConnection()
        {
            using (SqlConnection con = new SqlConnection(connString))
            {
                try
                {
                    con.Open();
                    // Connection thik thakle kichu korar dorkar nai, error khaile catch-e jabe
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Connection Error: " + ex.Message);
                }
            }
        }

        private void InitializePetShopUI()
        {
            this.Text = "Pet Shop Management System - v1.0";
            this.Size = new Size(1100, 750);
            this.MinimumSize = new Size(1000, 700);

            // --- 1. Header Section ---
            Panel header = new Panel { Dock = DockStyle.Top, Height = 130, BackColor = Color.White };
            this.Controls.Add(header);

            Label lblWelcome = new Label
            {
                Text = "🐾 Welcome to Our Premium Pet Shop",
                Font = new Font("Segoe UI", 30, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 126, 34),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            header.Controls.Add(lblWelcome);

            // --- 2. Sidebar Section ---
            Panel sidebar = new Panel { Dock = DockStyle.Left, Width = 260, BackColor = Color.FromArgb(44, 62, 80) };
            this.Controls.Add(sidebar);

            // --- 3. Main Content Area ---
            Panel mainPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            mainPanel.Paint += (s, pe) =>
            {
                Image img = Properties.Resources.projectWallpaper;
                if (img != null)
                {
                    int x = (mainPanel.Width - img.Width) / 2 + 125;
                    int y = (mainPanel.Height - img.Height) / 2;
                    pe.Graphics.DrawImage(img, x, y, img.Width, img.Height);
                }
            };
            this.Controls.Add(mainPanel);

            // --- 4. Sidebar Elements ---
            PictureBox profilePic = new PictureBox { Size = new Size(110, 110), Location = new Point(75, 20), BackColor = Color.Transparent, SizeMode = PictureBoxSizeMode.StretchImage };
            profilePic.Paint += ProfilePic_Paint;
            sidebar.Controls.Add(profilePic);

            Label lblLogo = new Label { Text = "🐾 PET SHOP", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, Location = new Point(38, 140), AutoSize = true };
            sidebar.Controls.Add(lblLogo);

            // Navigation Buttons
            int startY = 260;
            AddModernBtn(sidebar, "Admin Login", startY, AdminLogin_Click);
            AddModernBtn(sidebar, "Customer Login", startY + 65, CustomerLogin_Click);
            AddModernBtn(sidebar, "Review Page", startY + 130, ReviewPage_Click);
            AddModernBtn(sidebar, "Delivery Login", startY + 195, DeliveryLogin_Click);

            // Footer Section
            Panel footer = new Panel { Dock = DockStyle.Bottom, Height = 80, BackColor = Color.FromArgb(44, 62, 80) };
            Label lblFooter = new Label { Text = "🐾 \"Pets are not our whole life...\" | Premium Pet Care © 2026", Font = new Font("Segoe UI", 16, FontStyle.Bold | FontStyle.Italic), ForeColor = Color.White, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            footer.Controls.Add(lblFooter);
            this.Controls.Add(footer);

            // Layers Management
            footer.BringToFront();
            sidebar.BringToFront();
            header.BringToFront();
        }

        private void AddModernBtn(Panel p, string txt, int y, EventHandler clickHandler)
        {
            Button b = new Button { Text = txt, Size = new Size(220, 50), Location = new Point(20, y), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(255, 159, 67), ForeColor = Color.White, Font = new Font("Segoe UI", 11, FontStyle.Bold), Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0;
            b.Click += clickHandler;
            p.Controls.Add(b);
        }

        private void ProfilePic_Paint(object sender, PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Image img = Properties.Resources.petLogo;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(2, 2, 105, 105);
                if (img != null) { g.SetClip(path); g.DrawImage(img, 2, 2, 105, 105); g.ResetClip(); }
                using (Pen pen = new Pen(Color.White, 3)) { g.DrawEllipse(pen, 2, 2, 105, 105); }
            }
        }

        // --- BUTTON ACTIONS ---

        private void AdminLogin_Click(object sender, EventArgs e)
        {
            // Login Form open hobe jeta Database check korbe
            LoginForm login = new LoginForm();
            login.Show();
        }

        private void CustomerLogin_Click(object sender, EventArgs e)
        {
            // Registration/Customer Login Form open hobe
            CustomerLoginPage custLogin = new CustomerLoginPage();
            custLogin.Show();
        }

        private void ReviewPage_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Pet Reviews Loading from Database...");
        }

        private void DeliveryLogin_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Opening Delivery Portal...");
        }
    }
}