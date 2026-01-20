using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Linq;

namespace PetShopApp
{
    public partial class CustomersDashboard : Form
    {
        private Panel pnlSidebar, pnlHeader, pnlFooter, pnlMain, pnlRightPromo;
        private FlowLayoutPanel flowProductGrid, flowCartItems;
        private TextBox txtSearch;
        private Label lblClock, lblJossSlogan, lblTotalAmount;
        private Timer timerClock;
        private double totalBill = 0;
        // Dashboard class-er upore eita declare koro
        private List<(string Name, double Price)> cartItemsList = new List<(string Name, double Price)>();

        // 🔗 Database Connection
        private string connString = $@"Data Source={Environment.MachineName}\SQLEXPRESS; Initial Catalog=PetShopManagementDB; Integrated Security=True";

        public Dictionary<string, string[]> subItems = new Dictionary<string, string[]>
        {
            { "Cat", new string[] { "Persian", "Siamese", "Maine_Coon", "Bengal", "Ragdoll" } },
            { "Dog", new string[] { "German_Shepherd", "Bulldog", "Poodle", "Labrador", "Husky" } },
            { "Rabbit", new string[] { "Dutch_Rabbit", "Holland_Lop", "Netherland_Dwarf", "Mini_Rex", "Flemish_Giant" } },
            { "Bird", new string[] { "Parrot", "Budgerigar", "Cockatiel", "Lovebird", "Canary" } },
            { "Food", new string[] { "Royal_Canin_1kg", "Royal_Canin_2kg", "Royal_Canin_5kg", "Drools_1kg", "Drools_2kg", "Drools_5kg", "Whiskas_1kg", "Whiskas_2kg", "Whiskas_5kg", "Me_O_1kg", "Me_O_2kg", "Me_O_5kg" } },
            { "Medicine", new string[] { "Vaccine", "Dewormer", "Vitamins" } },
            { "Accessories", new string[] { "Pet_Bed", "Grooming_Kit", "Toys" } }
        };

        public CustomersDashboard()
        {
            InitializeComponent();
            SetupForm();
            BuildSidebar();
            BuildHeader();
            BuildFooter();

            pnlMain = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(242, 244, 247) };
            this.Controls.Add(pnlMain);
            pnlMain.BringToFront();

            BuildRightPromoSection();

            flowProductGrid = new FlowLayoutPanel
            {
                Location = new Point(0, 0),
                Width = 730,
                Dock = DockStyle.Left,
                Padding = new Padding(15),
                BackColor = Color.Transparent,
                AutoScroll = true,
                WrapContents = true
            };
            pnlMain.Controls.Add(flowProductGrid);

            LoadDashboard();
            StartClock();
        }

        private void SetupForm()
        {
            this.Text = "Pet-Universe | Customer Edition";
            this.Size = new Size(1150, 740);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void BuildSidebar()
        {
            pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 230, BackColor = Color.FromArgb(31, 41, 55) };
            this.Controls.Add(pnlSidebar);
            Label lblLogo = new Label { Text = "🐾 PET SHOP", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, Location = new Point(35, 20), AutoSize = true };
            pnlSidebar.Controls.Add(lblLogo);

            int y = 75;
            string[] navItems = { "Dashboard", "Best Sellers", "Cat", "Dog", "Rabbit", "Bird", "Food", "Accessories", "Medicine" };
            foreach (var item in navItems)
            {
                Button btn = new Button
                {
                    Text = "  " + item,
                    Size = new Size(230, 50),
                    Location = new Point(0, y),
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.Gainsboro,
                    Font = new Font("Segoe UI Semibold", 12),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, e) => Action_MenuNavigationClicked(item);
                pnlSidebar.Controls.Add(btn);
                y += 55;
            }
        }

        private void BuildHeader()
        {
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 130, BackColor = Color.FromArgb(23, 31, 42) };
            this.Controls.Add(pnlHeader);

            Label lblWelcome = new Label
            {
                Text = "WELCOME, " + UserSession.CurrentUsername.ToUpper() + "!",
                Font = new Font("Segoe UI Black", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(25, 12),
                AutoSize = true
            };
            Label lblQuote = new Label { Text = "“Kindness to animals is the mark of a true master.”", Font = new Font("Segoe UI Semibold", 8, FontStyle.Italic), ForeColor = Color.DarkGray, Location = new Point(27, 42), AutoSize = true };

            Label lblSearchHint = new Label { Text = "Quick Search:", Font = new Font("Segoe UI Semibold", 9), ForeColor = Color.Gold, Location = new Point(25, 65), AutoSize = true };
            txtSearch = new TextBox
            {
                Location = new Point(25, 85),
                Width = 280,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(45, 59, 75),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            AutoCompleteStringCollection searchSource = new AutoCompleteStringCollection();
            foreach (var cat in subItems.Keys) searchSource.Add(cat);
            foreach (var items in subItems.Values) searchSource.AddRange(items);
            txtSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtSearch.AutoCompleteCustomSource = searchSource;

            txtSearch.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter) { Action_MenuNavigationClicked(txtSearch.Text); e.SuppressKeyPress = true; }
            };

            lblJossSlogan = new Label { Text = "⚡ QUALITY • CARE • PASSION ⚡", Font = new Font("Segoe UI Black", 11, FontStyle.Italic), ForeColor = Color.FromArgb(46, 204, 113), Location = new Point(350, 45), Size = new Size(400, 30), TextAlign = ContentAlignment.MiddleCenter };
            lblClock = new Label { Text = "🕒 Loading...", Font = new Font("Segoe UI Black", 13, FontStyle.Bold), ForeColor = Color.White, Location = new Point(350, 75), Size = new Size(400, 30), TextAlign = ContentAlignment.MiddleCenter };

            Panel pnlProfile = new Panel { Size = new Size(180, 80), Location = new Point(950, 20), Cursor = Cursors.Hand };
            Label lblName = new Label
            {
                // UserSession theke username niye seta string-e convert kore shob boro hater (Capital) kora holo
                Text = UserSession.CurrentUsername?.ToString().ToUpper(),

                // Font "Segoe UI" boro hater bold hobe. Size 20 thakle 110 width-e naam kete jete pare, tai adjust kore nish mama
                Font = new Font("Segoe UI", 18, FontStyle.Bold),

                ForeColor = Color.White,
                Location = new Point(0, 18),
                Size = new Size(110, 40),
                TextAlign = ContentAlignment.MiddleRight
            };
            PictureBox pbProfile = new PictureBox { Size = new Size(55, 55), Location = new Point(115, 10), BackColor = Color.FromArgb(45, 59, 75), SizeMode = PictureBoxSizeMode.StretchImage };

            pbProfile.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath gp = new GraphicsPath()) { gp.AddEllipse(0, 0, 53, 53); pbProfile.Region = new Region(gp); }
                TextRenderer.DrawText(e.Graphics, "👤", new Font("Segoe UI", 20), new Point(7, 7), Color.White);
            };

            pnlProfile.Controls.AddRange(new Control[] { lblName, pbProfile });
            pnlProfile.Click += (s, e) => Action_CustomerProfileClicked();
            pbProfile.Click += (s, e) => Action_CustomerProfileClicked();

            pnlHeader.Controls.AddRange(new Control[] { lblWelcome, lblQuote, lblSearchHint, txtSearch, lblJossSlogan, lblClock, pnlProfile });
        }

        private void LoadDashboard()
        {
            flowProductGrid.Controls.Clear();
            Panel pnlBanner = new Panel { Size = new Size(680, 100), BackColor = Color.FromArgb(46, 204, 113), Margin = new Padding(10) };
            Label lblBanner = new Label { Text = "🌟 DISCOVER YOUR NEW BEST FRIEND 🌟", Font = new Font("Segoe UI Black", 14), ForeColor = Color.White, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            pnlBanner.Controls.Add(lblBanner);
            flowProductGrid.Controls.Add(pnlBanner);

            foreach (var key in subItems.Keys)
            {
                LoadProductsByCategory(key, false);
            }
        }

        private void LoadProductsByCategory(string category, bool clearGrid = true)
        {
            if (clearGrid) flowProductGrid.Controls.Clear();
            if (!subItems.ContainsKey(category)) return;

            foreach (string itemName in subItems[category])
            {
                string price = GetPriceFromDB(itemName);
                flowProductGrid.Controls.Add(CreateProductCard(itemName, price));
            }
        }

        private Panel CreateProductCard(string name, string price, bool isBestSeller = false)
        {
            int stock = GetStockFromDB(name);
            Panel card = new Panel { Size = new Size(150, 250), BackColor = Color.White, Margin = new Padding(10) };

            if (isBestSeller)
            {
                Label lblBadge = new Label { Text = "🔥 TOP SELL", BackColor = Color.Gold, ForeColor = Color.Black, Font = new Font("Segoe UI", 7, FontStyle.Bold), Location = new Point(0, 0), Size = new Size(70, 18), TextAlign = ContentAlignment.MiddleCenter };
                card.Controls.Add(lblBadge);
                lblBadge.BringToFront();
            }

            PictureBox pb = new PictureBox { Size = new Size(165, 110), BackColor = Color.FromArgb(240, 240, 240), SizeMode = PictureBoxSizeMode.Zoom, Dock = DockStyle.Top };
            string resName = name.Replace(" ", "_");
            try
            {
                object imgObj = Properties.Resources.ResourceManager.GetObject(resName);
                if (imgObj != null) pb.Image = (Image)imgObj;
            }
            catch { }

            Label lblName = new Label { Text = name, Font = new Font("Segoe UI Bold", 10), Location = new Point(5, 120), AutoSize = true };
            Label lblPrice = new Label { Text = " $ " + price, Font = new Font("Segoe UI Bold", 11), ForeColor = Color.DarkGreen, Location = new Point(5, 145), AutoSize = true };
            Label lblStock = new Label
            {
                Text = stock <= 0 ? "❌ Out of Stock" : (stock < 5 ? "⚠️ Only " + stock + " left!" : "✅ In Stock: " + stock),
                Font = new Font("Segoe UI Semibold", 8),
                ForeColor = stock <= 0 ? Color.Red : (stock < 5 ? Color.OrangeRed : Color.Gray),
                Location = new Point(5, 175),
                AutoSize = true
            };

            Button btnAdd = new Button
            {
                Text = stock <= 0 ? "NOT AVAILABLE" : "ADD TO BILL",
                Size = new Size(150, 35),
                Location = new Point(7, 215),
                BackColor = stock <= 0 ? Color.LightGray : Color.FromArgb(31, 41, 55),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = stock > 0,
                Font = new Font("Segoe UI Bold", 9)
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => AddToLiveCart(name, price);

            card.Controls.AddRange(new Control[] { pb, lblName, lblPrice, lblStock, btnAdd });
            return card;
        }

        private string GetPriceFromDB(string item)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT Price FROM Inventory WHERE Breed = @name", conn);
                    cmd.Parameters.AddWithValue("@name", item);
                    conn.Open();
                    object r = cmd.ExecuteScalar();
                    return r != null ? r.ToString() : "1200";
                }
            }
            catch { return "1000"; }
        }

        private int GetStockFromDB(string item)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT Quantity FROM Inventory WHERE Breed = @name", conn);
                    cmd.Parameters.AddWithValue("@name", item);
                    conn.Open();
                    object r = cmd.ExecuteScalar();
                    return r != null ? Convert.ToInt32(r) : 5;
                }
            }
            catch { return 10; }
        }

        private void AddToLiveCart(string name, string price)
        {
            // 1. Database theke taaja stock jene neya
            int stockInHand = GetStockFromDB(name);

            // 2. Dekha je cart-e amra ekhon porjonto koyta nisi oi item
            int itemsAlreadyInCart = cartItemsList.Count(x => x.Name == name);

            // 3. 🛑 SAFETY CHECK: Stocker beshi nite gele badha dibo
            if (itemsAlreadyInCart >= stockInHand)
            {
                MessageBox.Show($"Mama, {name} stock-e ache matro {stockInHand} ta. Ar add kora jabe na!",
                                "Stock Limit reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. Baki logic ager motoi
            double p = double.Parse(price);
            totalBill += p;

            var currentItem = (Name: name, Price: p);
            cartItemsList.Add(currentItem);

            lblTotalAmount.Text = "Total: $ " + totalBill;

            // UI Panel setup
            Panel itemPanel = new Panel { Size = new Size(150, 45), Margin = new Padding(0, 2, 0, 2), BackColor = Color.WhiteSmoke };
            Label lbl = new Label { Text = name.ToUpper() + "\n " + price, Font = new Font("Consolas", 8, FontStyle.Bold), Location = new Point(5, 5), AutoSize = true };

            Button btnX = new Button { Text = "×", Size = new Size(22, 22), Location = new Point(135, 10), FlatStyle = FlatStyle.Flat, ForeColor = Color.DimGray };
            btnX.FlatAppearance.BorderSize = 0;

            btnX.Click += (s, e) => {
                flowCartItems.Controls.Remove(itemPanel);
                totalBill -= p;
                cartItemsList.Remove(currentItem);
                lblTotalAmount.Text = "Total: $ " + totalBill;
            };

            itemPanel.Controls.AddRange(new Control[] { lbl, btnX });
            flowCartItems.Controls.Add(itemPanel);
        }
        private void CustomersDashboard_Load(object sender, EventArgs e)
        {

        }

        private void BuildRightPromoSection()
        {
            pnlRightPromo = new Panel { Dock = DockStyle.Right, Width = 185, BackColor = Color.FromArgb(236, 240, 241), Padding = new Padding(10) };
            pnlMain.Controls.Add(pnlRightPromo);
            Label lblMemo = new Label { Text = "MINI CASH MEMO", Font = new Font("Segoe UI Bold", 10), Location = new Point(10, 10), AutoSize = true };
            flowCartItems = new FlowLayoutPanel { Location = new Point(5, 40), Size = new Size(175, 500), AutoScroll = true, BackColor = Color.White };
            lblTotalAmount = new Label { Text = "Total: $ 0", Font = new Font("Segoe UI Bold", 11), ForeColor = Color.DarkRed, Location = new Point(10, 550), AutoSize = true };
            pnlRightPromo.Controls.AddRange(new Control[] { lblMemo, flowCartItems, lblTotalAmount });
        }

        private void BuildFooter()
        {
            pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 55, BackColor = Color.FromArgb(230, 126, 34) };
            this.Controls.Add(pnlFooter);
            pnlFooter.BringToFront();

            Panel footerSpacer = new Panel { Dock = DockStyle.Left, Width = 230, BackColor = Color.Transparent };
            pnlFooter.Controls.Add(footerSpacer);

            Label lblFooterText = new Label { Text = "✨ Trusted by Pet Parents | Quality Guaranteed ✨", ForeColor = Color.Black, Font = new Font("Segoe UI Bold", 10), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            pnlFooter.Controls.Add(lblFooterText);

            Button btnCheckout = new Button { Text = "🛒 GENERATE BILL", BackColor = Color.Black, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), Dock = DockStyle.Right, Width = 185, FlatStyle = FlatStyle.Flat };
            btnCheckout.FlatAppearance.BorderSize = 0;
            pnlFooter.Controls.Add(btnCheckout);
            btnCheckout.Click += (s, e) => Action_GenerateBill();
        }

        private void StartClock()
        {
            timerClock = new Timer { Interval = 1000 };
            timerClock.Tick += (s, e) => lblClock.Text = "🕒 " + DateTime.Now.ToString("hh:mm:ss tt");
            timerClock.Start();
        }

        private void Action_MenuNavigationClicked(string menu)
        {
            string m = menu.Trim().ToLower();
            if (m == "dashboard") { LoadDashboard(); return; }
            if (m == "best sellers") { LoadBestSellers(); return; }
            foreach (var key in subItems.Keys)
            {
                if (key.ToLower() == m) { LoadProductsByCategory(key); return; }
            }

            flowProductGrid.Controls.Clear();
            Label lblSearchTitle = new Label { Text = "🔎 SEARCH RESULTS FOR: " + menu.ToUpper(), Font = new Font("Segoe UI Bold", 12), ForeColor = Color.DarkSlateGray, Size = new Size(700, 30), Margin = new Padding(15, 10, 0, 0) };
            flowProductGrid.Controls.Add(lblSearchTitle);

            foreach (var pair in subItems)
            {
                foreach (var val in pair.Value)
                {
                    if (val.ToLower().Contains(m))
                    {
                        string price = GetPriceFromDB(val);
                        flowProductGrid.Controls.Add(CreateProductCard(val, price));
                    }
                }
            }
        }
        private void Action_GenerateBill()
        {
            // 1. Cart khali kina check
            if (totalBill == 0)
            {
                MessageBox.Show("Mama, cart khali! Age kichu kinen.");
                return;
            }

            // 2. Payment Page-er object banano
            CustomerPaymentCashMemo paymentPage = new CustomerPaymentCashMemo();

            // 3. UserSession theke name ar total bill pathano
            string currentUser = string.IsNullOrEmpty(UserSession.CurrentUsername) ? "customer1" : UserSession.CurrentUsername;
            paymentPage.OrderCustomerName = currentUser;
            paymentPage.OrderTotalBill = (decimal)totalBill;

            // 4. MAMA EITA HOCHCHE MAIN KAJK: Temporary list-ta pura pathay deya
            // Amra database-e ekhon insert korbo na, shudhu list-ta pathabo
            paymentPage.PurchasedItems = this.cartItemsList.ToList();

            // 5. Page change kora
            paymentPage.Show();
            this.Hide();

            // 6. Reset Dashboard Cart (Next order-er jonno ready kora)
            cartItemsList.Clear();
            totalBill = 0;
        }

        private void LoadBestSellers()
        {
            flowProductGrid.Controls.Clear();
            Label lblTitle = new Label { Text = "🔥 OUR TOP 10 BEST SELLERS", Font = new Font("Segoe UI Black", 14), ForeColor = Color.DarkRed, Size = new Size(700, 40), TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(15, 0, 0, 0) };
            flowProductGrid.Controls.Add(lblTitle);

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT TOP 10 Breed, Price FROM Inventory ORDER BY SellCount DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        flowProductGrid.Controls.Add(CreateProductCard(reader["Breed"].ToString(), reader["Price"].ToString(), true));
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Problem: " + ex.Message); }
        }

        private void Action_CustomerProfileClicked()
        {
            MessageBox.Show("👤 CUSTOMER PROFILE\n------------------\nName: Master Mama\nRank: Gold Member", "Profile Info");
        }
    }
}