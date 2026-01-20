using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace PetShopApp
{
    public partial class GiveReview : Form
    {

        private Panel pnlSidebar, pnlHeader;
        private FlowLayoutPanel flowMainFeed;
        private TextBox txtSearch;
        private Label lblClock;
        private Timer timerClock;
        private string currentCategory = "Overview";
        

        private string connString = $@"Data Source={Environment.MachineName}\SQLEXPRESS; Initial Catalog=PetShopManagementDB; Integrated Security=True";

        private string currentUser; // Global variable for this form
        

        // Constructor-e username parameter add kora hoyeche
        public GiveReview(string username)
        {
            InitializeComponent();
            this.currentUser = username; // Login theke asha username save holo

            // Ekhon tor dashboard er baki kaj koro
            SetupDashboard();
            BuildSidebar();
            BuildHeader(); // Header e ekhon currentUser er name show korbe
            BuildFeedArea();
            StartClock();
            RenderItems("Overview");
        }
        private void SetupDashboard()
        {
            this.Text = "Pet-Universe | Customer Dashboard";
            this.Size = new Size(1200, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 243, 247);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }
        private void BuildHeader()
        {
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 160, BackColor = Color.FromArgb(18, 26, 37) };
            this.Controls.Add(pnlHeader);

            // 🐾 1. LOGO SECTION
            PictureBox picLogo = new PictureBox
            {
                Size = new Size(85, 85),
                Location = new Point(30, 30),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            try { picLogo.Image = (Image)Properties.Resources.ResourceManager.GetObject("pet_logo"); }
            catch { picLogo.BackColor = Color.FromArgb(46, 204, 113); }

            // 2. MAIN TITLE
            // Status Panel e currentUser use kora:
            Label lblUserStatus = new Label
            {
                Text = currentUser, // Eikhane login kora user-er nam auto bosbe
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(45, 10),
                AutoSize = true
            };

            // 📜 3. JOSS PET QUOTE
            Label lblQuote = new Label
            {
                Text = "\"Until one has loved an animal, a part of one's soul remains unawakened.\"",
                Font = new Font("Segoe UI Semibold", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(149, 165, 166),
                Location = new Point(140, 70),
                AutoSize = true
            };

            // 🔍 4. SEARCH LABEL
            Label lblSearchHint = new Label
            {
                Text = "FIND YOUR PET :",
                Font = new Font("Segoe UI Black", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Location = new Point(140, 115),
                AutoSize = true
            };

            // 🐾 2. MAIN TITLE (Add this block)
            Label lblWelcome = new Label
            {
                Text = "PET UNIVERSE EXPLORER",
                Font = new Font("Segoe UI Black", 22, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(135, 25),
                AutoSize = true
            };

            // 📦 5. SEARCH BAR (Clean & Bold)
            Panel pnlSearchBorder = new Panel
            {
                Size = new Size(420, 42),
                Location = new Point(310, 108),
                BackColor = Color.FromArgb(46, 204, 113),
                Padding = new Padding(1)
            };

            Panel pnlSearchInner = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(35, 48, 64) };
            pnlSearchBorder.Controls.Add(pnlSearchInner);

            Label lblSearchIcon = new Label { Text = "🔍", Font = new Font("Segoe UI", 12), ForeColor = Color.FromArgb(46, 204, 113), Location = new Point(8, 8), AutoSize = true };

            txtSearch = new TextBox
            {
                Text = "",
                Width = 360,
                Location = new Point(40, 10), // Icon er pashe x=40 thake start hobe
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(35, 48, 64),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            txtSearch.TextChanged += (s, e) => RenderItems(currentCategory, txtSearch.Text);
            pnlSearchInner.Controls.AddRange(new Control[] { lblSearchIcon, txtSearch });

            // 🕒 6. MULTI-LINE CLOCK (Time + Date + Day)
            lblClock = new Label
            {
                Text = "🕒 Loading...",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(880, 20),
                Size = new Size(280, 60),
                TextAlign = ContentAlignment.MiddleRight
            };

            // ✨ 7. STATUS PANEL (Clock-er thik niche design kora)
            Panel pnlStatus = new Panel
            {
                Size = new Size(220, 50),
                Location = new Point(940, 85),
                BackColor = Color.FromArgb(35, 48, 64),
            };

            // Design Accent Line
            Panel pnlAccent = new Panel { Dock = DockStyle.Bottom, Height = 3, BackColor = Color.FromArgb(46, 204, 113) };
            pnlStatus.Controls.Add(pnlAccent);

            Label lblUserIcon = new Label { Text = "👤", Font = new Font("Segoe UI", 12), ForeColor = Color.FromArgb(46, 204, 113), Location = new Point(10, 10), AutoSize = true };
            lblUserStatus = new Label { Text = currentUser, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.White, Location = new Point(45, 10), AutoSize = true };
            Label lblOnline = new Label { Text = "• Online Now", Font = new Font("Segoe UI", 8, FontStyle.Italic), ForeColor = Color.FromArgb(46, 204, 113), Location = new Point(45, 28), AutoSize = true };

            pnlStatus.Controls.AddRange(new Control[] { lblUserIcon, lblUserStatus, lblOnline });

            // Header-e shob add kora
            pnlHeader.Controls.AddRange(new Control[] { picLogo, lblWelcome, lblQuote, lblSearchHint, pnlSearchBorder, lblClock, pnlStatus });
            // Method-er ekdom sheshe eita thakbe
            pnlHeader.Controls.AddRange(new Control[] { picLogo, lblWelcome, lblQuote, lblSearchHint, pnlSearchBorder, lblClock, pnlStatus });
        }
        private void BuildSidebar()
        {
            pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 230, BackColor = Color.FromArgb(24, 33, 45) };
            this.Controls.Add(pnlSidebar);
            string[] navItems = { "Overview", "Cat", "Dog", "Bird", "Rabbit", "Food", "Accessories" };
            int yPos = 100;
            foreach (var item in navItems)
            {
                Button btn = new Button { Text = item, Size = new Size(230, 50), Location = new Point(0, yPos), FlatStyle = FlatStyle.Flat, ForeColor = Color.Gainsboro, Font = new Font("Segoe UI", 15, FontStyle.Bold) };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, e) => { currentCategory = item; RenderItems(item); };
                pnlSidebar.Controls.Add(btn);
                yPos += 55;
            }

            PictureBox pbLogo = new PictureBox { Size = new Size(80, 80), Location = new Point(90, 10) };
            pbLogo.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(230, 126, 34)), 0, 0, 78, 78);
                TextRenderer.DrawText(e.Graphics, "🐾", new Font("Segoe UI", 28), new Point(5, 8), Color.White);
            };
            pnlSidebar.Controls.Add(pbLogo);

        
        }

        private void BuildFeedArea()
        {
            flowMainFeed = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(25), FlowDirection = FlowDirection.TopDown, WrapContents = false, BackColor = Color.Transparent };
            this.Controls.Add(flowMainFeed);
            flowMainFeed.BringToFront();
        }

        private void RenderItems(string category, string search = "")
        {
            flowMainFeed.Controls.Clear();
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT Breed, Category, Price FROM Inventory WHERE (Category = @cat OR @cat = 'Overview') AND (Breed LIKE @search)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@cat", category);
                    cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                    conn.Open();
                    SqlDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        var item = new PetItem { Name = r["Breed"].ToString(), Category = r["Category"].ToString(), Price = r["Price"].ToString() };
                        flowMainFeed.Controls.Add(CreateProductCard(item));
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Data Error: " + ex.Message); }
        }

        private Panel CreateProductCard(PetItem item)
        {
            // Card size ektu boro kora hoyeche safety-r jonno
            Panel card = new Panel { Size = new Size(920, 240), BackColor = Color.White, Margin = new Padding(0, 0, 0, 25) };

            // Picture Box
            PictureBox pic = new PictureBox { Size = new Size(180, 180), Location = new Point(20, 20), BackColor = Color.FromArgb(245, 245, 245), SizeMode = PictureBoxSizeMode.StretchImage };
            try
            {
                object resImg = Properties.Resources.ResourceManager.GetObject(item.Name.Replace(" ", "_"));
                if (resImg != null) pic.Image = (Image)resImg;
            }
            catch { }

            // Name: Location ektu bariye 220 er bodle 230 kora hoyeche jate image er niche na pore
            Label name = new Label
            {
                Text = item.Name.ToUpper(),
                Font = new Font("Segoe UI", 18, FontStyle.Bold), // Font size ektu boro kora
                Location = new Point(230, 25),
                AutoSize = true, // AutoSize true thakle katar kotha na
                MaximumSize = new Size(300, 0) // Beshi boro hole nichey nambe, katbe na
            };

            Label price = new Label
            {
                Text = "Price: $" + item.Price,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Location = new Point(232, 65),
                AutoSize = true
            };

            // ⭐ Stars (Rating) - Location ektu nichey namano
            int localSelectedRating = 5;
            Label[] stars = new Label[5];
            for (int i = 0; i < 5; i++)
            {
                int starIndex = i + 1;
                stars[i] = new Label { Text = "★", Font = new Font("Segoe UI", 22), ForeColor = Color.Gold, Location = new Point(230 + (i * 35), 105), AutoSize = true, Cursor = Cursors.Hand };
                stars[i].Click += (s, e) =>
                {
                    localSelectedRating = starIndex;
                    for (int j = 0; j < 5; j++) stars[j].ForeColor = (j < localSelectedRating) ? Color.Gold : Color.LightGray;
                };
                card.Controls.Add(stars[i]);
            }

            // Reviews & Other controls... (Ager motoi thakbe)
            // 📜 Previous Reviews Label (Bold, Orange & Boro kora hoyeche)
            Label lblHistory = new Label
            {
                Text = "📜 PREVIOUS REVIEWS:",
                Font = new Font("Segoe UI Black", 11, FontStyle.Bold), // Size 11 & Black style for extra bold
                ForeColor = Color.DarkOrange, // Stylish Orange color
                Location = new Point(580, 18),
                AutoSize = true
            };
            ListBox lstHistory = new ListBox { Size = new Size(310, 140), Location = new Point(580, 45), BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(248, 249, 250), Font = new Font("Segoe UI", 9) };
            LoadReviewsFromDB(item.Name, lstHistory);

            TextBox txtComment = new TextBox { Size = new Size(320, 25), Location = new Point(230, 170), Font = new Font("Segoe UI", 10) };
            Button btnSubmit = new Button { Text = "POST REVIEW", Size = new Size(120, 30), Location = new Point(430, 200), BackColor = Color.FromArgb(18, 26, 37), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnSubmit.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(txtComment.Text))
                {
                    SaveReviewToDB(item.Name, txtComment.Text, localSelectedRating);
                    LoadReviewsFromDB(item.Name, lstHistory);
                    txtComment.Clear();
                }
            };

            card.Controls.AddRange(new Control[] { pic, name, price, lblHistory, lstHistory, txtComment, btnSubmit });
            return card;
        }

        private void SaveReviewToDB(string itemName, string comment, int rating)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string sql = "INSERT INTO ProductReviews (Username, ItemName, Comment, Rating, ReviewDate) VALUES (@u, @i, @c, @r, GETDATE())";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@u", currentUser);
                    cmd.Parameters.AddWithValue("@i", itemName);
                    cmd.Parameters.AddWithValue("@c", comment);
                    cmd.Parameters.AddWithValue("@r", rating);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { MessageBox.Show("Database Error: " + ex.Message); }
        }

        private void LoadReviewsFromDB(string itemName, ListBox lb)
        {
            lb.Items.Clear();
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string sql = "SELECT TOP 10 Username, Comment, Rating FROM ProductReviews WHERE ItemName = @i ORDER BY ReviewDate DESC";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@i", itemName);
                    conn.Open();
                    SqlDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        string stars = new string('★', Convert.ToInt32(r["Rating"]));
                        lb.Items.Add($"{stars} {r["Username"]}: {r["Comment"]}");
                    }
                }
            }
            catch { lb.Items.Add("No reviews yet."); }
        }

        private void StartClock()
        {
            timerClock = new Timer { Interval = 1000 };
            timerClock.Tick += (s, e) =>
            {
                lblClock.Text = "🕒 " + DateTime.Now.ToString("hh:mm:ss tt") + "\n" +
                                 DateTime.Now.ToString("dddd, dd MMM yyyy");
            };
            timerClock.Start();
        }
    }

    public class PetItem
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Price { get; set; }
    }
}
