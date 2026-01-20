using System;
using System.Data.SqlClient; // 1. SQL Namespace add korlam
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PetShopApp
{
    public partial class Form1 : Form
    {
        // 2. Connection String - Dynamic PC Name detect korbe
        string connString = $@"Data Source={Environment.MachineName}\SQLEXPRESS; Initial Catalog=PetShopManagementDB; Integrated Security=True";
        Panel mainPanel;
        bool showWallpaper = true; // Eita wallpaper control korbe
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        // Color Palette
        Color primaryOrange = Color.FromArgb(255, 120, 40); // Vibrant Orange
        Color darkBg = Color.FromArgb(15, 15, 30);
        Color cardBg = Color.FromArgb(25, 25, 50);
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
            this.BackColor = Color.White;

            // --- 1. Header (Docks to Top) ---
            Panel header = new Panel { Dock = DockStyle.Top, Height = 130, BackColor = Color.FromArgb(23, 31, 42) };
            Label lblWelcome = new Label { Text = "🐾 Welcome to Our Premium Pet Shop", Font = new Font("Segoe UI", 30, FontStyle.Bold), ForeColor = Color.FromArgb(230, 126, 34), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            header.Controls.Add(lblWelcome);

            // --- 2. Footer (Docks to Bottom) ---
            Panel footer = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.FromArgb(230, 126, 34) };
            Label lblFooter = new Label { Text = "🐾 \"Pets are not our whole life...\" | Premium Pet Care © 2026", Font = new Font("Segoe UI", 16, FontStyle.Bold | FontStyle.Italic), ForeColor = Color.White, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            footer.Controls.Add(lblFooter);

            // --- 3. Sidebar (Docks to Left) ---
            Panel sidebar = new Panel { Dock = DockStyle.Left, Width = 260, BackColor = Color.FromArgb(31, 41, 55) };

            // --- 4. Main Panel (Eita ekhon majhkhane thakbe) ---
            mainPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            mainPanel.Paint += MainPanel_Paint_Logic;

            // --- MAMA EI SEQUENCE TA KHAYAL KOR ---
            // Age fixed controls add korbo, shobar sheshe Fill wala panel
            this.Controls.Add(sidebar);
            this.Controls.Add(header);
            this.Controls.Add(footer);
            this.Controls.Add(mainPanel);

            // Z-Order Fix: Jate Sidebar ar Header shobar upore thake
            sidebar.BringToFront();
            header.BringToFront();
            footer.BringToFront();
            mainPanel.SendToBack(); // SendToBack dile mainPanel shubho majher faka jayga টুকু দখল korbe

            // --- Sidebar Elements (Tor puron button code) ---
            PictureBox profilePic = new PictureBox { Size = new Size(110, 110), Location = new Point(75, 30), BackColor = Color.Transparent, SizeMode = PictureBoxSizeMode.StretchImage };
            profilePic.Paint += ProfilePic_Paint;
            sidebar.Controls.Add(profilePic);

            Label lblLogo = new Label { Text = "🐾 PET SHOP", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, Location = new Point(38, 150), AutoSize = true };
            sidebar.Controls.Add(lblLogo);

            int startY = 260;
            AddModernBtn(sidebar, "Admin Access", startY, AdminLogin_Click);
            AddModernBtn(sidebar, "Member Portal", startY + 65, CustomerLogin_Click);
            AddModernBtn(sidebar, "Pet Reviews", startY + 130, ReviewPage_Click);
            AddModernBtn(sidebar, "Specialist Doctors", startY + 195, DoctorHub_Click);
        }
        private void MainPanel_Paint_Logic(object sender, PaintEventArgs pe)
        {
            // Jodi showWallpaper false hoy (mane jokhon Doctor Hub-e jabi), tokhon eita ar draw korbe na
            if (!showWallpaper) return;

            Image img = Properties.Resources.projectWallpaper;
            if (img != null)
            {
                int centerX = (mainPanel.Width - img.Width) / 2;
                int centerY = (mainPanel.Height - img.Height) / 2;

                int offsetX = 120;
                int offsetY = 45;

                int finalX = centerX + offsetX;
                int finalY = centerY + offsetY;

                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                pe.Graphics.DrawImage(img, finalX, finalY, img.Width, img.Height);
            }
        }
        private void MainPanel_Paint_WithWallpaper(object sender, PaintEventArgs pe)
        {
            Image img = Properties.Resources.projectWallpaper;
            if (img != null)
            {
                // Image-ta center-e thakbe
                int x = (mainPanel.Width - img.Width) / 2;
                int y = (mainPanel.Height - img.Height) / 2;
                pe.Graphics.DrawImage(img, x, y, img.Width, img.Height);
            }
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
            ResetWallpaper(); // Wallpaper back anbe
            LoginForm login = new LoginForm();
            login.Show();
        }

        private void CustomerLogin_Click(object sender, EventArgs e)
        {
            ResetWallpaper(); // Wallpaper back anbe
            CustomerLoginPage custLogin = new CustomerLoginPage();
            custLogin.Show();
        }

        private void ReviewPage_Click(object sender, EventArgs e)
        {
            ResetWallpaper(); // Wallpaper back anbe
            ReviewPageCheck reviews = new ReviewPageCheck();
            reviews.Show();
        }

        // Chotto helper method jate bar bar code na likhte hoy
        private void ResetWallpaper()
        {
            showWallpaper = true;
            mainPanel.Controls.Clear();
            mainPanel.BackColor = Color.White;
            mainPanel.Invalidate(); // Wallpaper drawing trigger korbe
        }

        // 1. Button click ekhon shudhu method call korbe
        private void DoctorHub_Click(object sender, EventArgs e)
        {
            LoadDoctorHub();
        }

        private void LoadDoctorHub()
        {
            showWallpaper = false;
            mainPanel.Controls.Clear();
            mainPanel.BackColor = darkBg;

            // 1. Ekta Container Panel nibo jeta Header/Footer theke ektu faka thakbe
            // Eita dile cards gulo ekdom border-e gheshe thakbe na
            mainPanel.Padding = new Padding(10);

            // 2. FlowLayoutPanel setup
            // LoadDoctorHub er bhitorer flow panel setup ta emon kor:
            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,

                // Ekhane 1st number ta Left, 2nd ta TOP (niche namanor jonno), 
                // 3rd ta Right, 4th ta Bottom.
                // Top Padding 50-60 dile card gulo header theke besh niche neme asbe.
                Padding = new Padding(280, 120, 25, 20)
            };

            // Scrollbar smooth korar jonno eita dorkar
            flow.HorizontalScroll.Visible = false;
            flow.HorizontalScroll.Enabled = false;

            mainPanel.Controls.Add(flow);

            // 3. Doctor Data
            string[] names = { "Md. Ashikur Rahman Mirza", "Mahabub Alom Apon", "Shajia Afrin",  "AB Arafat", "Arifin Hemel", "Mostafizur Rahman", "MD Shahed Bhuiyan", "Assistant Doctor" };
            string[] resNames = { "Ashik", "apon", "shajia", "arafat", "hemel", "Mostafiz", "sohid", "none" };

            // Card gulo add kora
            for (int i = 0; i < names.Length; i++)
            {
                // Chhoto size-er card gulo ekhane add hobe
                flow.Controls.Add(CreateDoctorCard(names[i], "Veterinary Specialist", resNames[i]));
            }

            // Fix: Force Layout update
            flow.PerformLayout();
        }
        private Panel CreateDoctorCard(string name, string spec, string resName)
        {
            // 1. Card Size (320x500) - 256px image dhorar jonno card boro kora holo
            Panel card = new Panel { Size = new Size(320, 500), BackColor = cardBg, Margin = new Padding(20) };
            card.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, card.Width, card.Height, 30, 30));

            // 2. PictureBox (256x256)
            PictureBox pic = new PictureBox
            {
                Size = new Size(256, 256),
                Location = new Point(32, 25), // Center calculation: (320-256)/2 = 32
                BackColor = Color.FromArgb(45, 52, 71),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            pic.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, 256, 256, 20, 20));

            // Resource Loading with High Quality
            object obj = PetShopApp.Properties.Resources.ResourceManager.GetObject(resName);
            Image finalImage = null;

            if (obj != null && obj is Image)
            {
                Image rawImg = (Image)obj;
                // 256x256 smooth bitmap create kora
                Bitmap smoothImg = new Bitmap(256, 256);
                using (Graphics g = Graphics.FromImage(smoothImg))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.DrawImage(rawImg, 0, 0, 256, 256);
                }
                pic.Image = smoothImg;
                finalImage = smoothImg;
            }
            else
            {
                Label lblFallback = new Label { Text = "👨‍⚕️", Font = new Font("Segoe UI Emoji", 80), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
                pic.Controls.Add(lblFallback);
                finalImage = SystemIcons.Information.ToBitmap();
            }

            // 3. Labels (Niche align kora holo)
            Label lblN = new Label
            {
                Text = name,
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(310, 40),
                Location = new Point(5, 300) // Image er pore gap rekhe
            };

            Label lblS = new Label
            {
                Text = spec,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(180, 180, 180),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(310, 30),
                Location = new Point(5, 345)
            };

            // 4. Appointment Button
            Button btn = new Button
            {
                Text = "BOOK APPOINTMENT",
                Size = new Size(240, 55),
                Location = new Point(40, 410),
                FlatStyle = FlatStyle.Flat,
                BackColor = primaryOrange,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btn.Width, btn.Height, 20, 20));
            btn.Click += (s, e) => ShowAppointmentForm(name, finalImage);

            // Controls Add
            card.Controls.AddRange(new Control[] { pic, lblN, lblS, btn });
            return card;
        }
        private void ShowAppointmentForm(string docName, Image docImg)
        {
            Form popup = new Form { Size = new Size(400, 500), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.None, BackColor = Color.FromArgb(30, 30, 50) };
            popup.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, 400, 500, 25, 25));

            Label title = new Label { Text = "Appointment Booking", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = primaryOrange, Location = new Point(65, 25), AutoSize = true };

            // Input Fields
            Label l1 = new Label { Text = "Pet Name:", ForeColor = Color.Silver, Location = new Point(50, 90), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            TextBox txtPet = new TextBox { Width = 300, Location = new Point(50, 115), Font = new Font("Segoe UI", 12), BackColor = Color.White };

            Label l2 = new Label { Text = "Guardian Name:", ForeColor = Color.Silver, Location = new Point(50, 170), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            TextBox txtOwner = new TextBox { Width = 300, Location = new Point(50, 195), Font = new Font("Segoe UI", 12) };

            Label l3 = new Label { Text = "Preferred Date:", ForeColor = Color.Silver, Location = new Point(50, 255), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            DateTimePicker dtp = new DateTimePicker { Location = new Point(50, 280), Width = 300, Font = new Font("Segoe UI", 10) };

            Button btnNext = new Button
            {
                Text = "CONFIRM BOOKING",
                Size = new Size(300, 50),
                Location = new Point(50, 380),
                BackColor = primaryOrange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnNext.Width, btnNext.Height, 20, 20));

            btnNext.Click += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtPet.Text)) { MessageBox.Show("Please enter Pet Name!"); return; }
                popup.Hide();
                ShowFinalReceipt(docName, docImg, txtPet.Text, txtOwner.Text, dtp.Value.ToShortDateString());
                popup.Close();
            };

            popup.Controls.AddRange(new Control[] { title, l1, txtPet, l2, txtOwner, l3, dtp, btnNext });
            popup.ShowDialog();
        }

        private void ShowFinalReceipt(string docName, Image docImg, string pet, string owner, string date)
        {
            Form receipt = new Form
            {
                Size = new Size(500, 520),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.None,
                BackColor = Color.White,
                ShowInTaskbar = false
            };
            receipt.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, 500, 520, 35, 35));

            // --- 1. Header ---
            Panel header = new Panel { Dock = DockStyle.Top, Height = 90, BackColor = Color.FromArgb(23, 31, 42) };
            Label lblLogo = new Label
            {
                Text = "🐾 PREMIUM PET CLINIC",
                Font = new Font("Segoe UI Black", 18, FontStyle.Bold),
                ForeColor = primaryOrange,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            header.Controls.Add(lblLogo);

            // --- 2. Footer ---
            Panel footer = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = primaryOrange };
            Label lblSlogan = new Label { Text = "❤ WE CARE FOR YOUR PET LIKE FAMILY", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            footer.Controls.Add(lblSlogan);

            // --- 3. Large Background Paw (Bottom Left) ---
            Label lblLogo2 = new Label
            {
                Text = "🐾",
                Font = new Font("Segoe UI", 75),
                ForeColor = Color.FromArgb(230, 126, 34), // Tor dewa Orange color
                Size = new Size(100, 80),
                Location = new Point(20, 365), // Akdom left-e footer-er upore
                BackColor = Color.Transparent
            };

            // --- 4. Doctor Section ---
            PictureBox p = new PictureBox
            {
                Image = docImg,
                Size = new Size(130, 130),
                Location = new Point(40, 110),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.FromArgb(245, 245, 245)
            };
            p.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, 130, 130, 130, 130));

            Label dName = new Label { Text = docName.ToUpper(), Font = new Font("Segoe UI Black", 10, FontStyle.Bold), Location = new Point(10, 250), Width = 190, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.FromArgb(40, 40, 40) };
            Label dSpec = new Label { Text = "VETERINARY SPECIALIST", Font = new Font("Segoe UI", 7, FontStyle.Bold), Location = new Point(10, 275), Width = 190, TextAlign = ContentAlignment.MiddleCenter, ForeColor = primaryOrange };
            Label dQuote = new Label { Text = "\"Healing Hands, Loving Hearts\"", Font = new Font("Segoe UI", 7, FontStyle.Italic), Location = new Point(10, 295), Width = 190, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.Gray };

            // --- 5. Token Info ---
            Random rnd = new Random();
            int sl = rnd.Next(1, 15);
            Label lblTitle = new Label { Text = "APPOINTMENT TOKEN", Font = new Font("Segoe UI Black", 13), ForeColor = Color.FromArgb(20, 120, 20), Location = new Point(220, 110), AutoSize = true };
            Panel line = new Panel { Height = 2, Width = 230, BackColor = Color.LightGray, Location = new Point(225, 140) };

            Label info = new Label
            {
                Text = $"TOKEN NO   : #{sl:D2}\n\n" +
                       $"PET NAME   : {pet.ToUpper()}\n\n" +
                       $"GUARDIAN   : {owner.ToUpper()}\n\n" +
                       $"DATE       : {date}\n\n" +
                       $"TIME SLOT  : 10:30 AM",
                Font = new Font("Consolas", 10, FontStyle.Bold),
                Location = new Point(225, 160),
                Size = new Size(250, 220),
                ForeColor = Color.Black
            };

            // --- 6. Print Button ---
            Button btnClose = new Button
            {
                Text = "PRINT & CLOSE",
                Size = new Size(180, 45),
                Location = new Point(265, 410),
                BackColor = Color.FromArgb(23, 31, 42),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnClose.Width, btnClose.Height, 15, 15));
            btnClose.Click += (s, e) => receipt.Close();

            // Controls Add (lblLogo2 alada vabe add holo)
            receipt.Controls.AddRange(new Control[] { header, footer, p, dName, dSpec, dQuote, lblTitle, line, info, btnClose, lblLogo2 });

            receipt.ShowDialog(this);
        }
    }
}