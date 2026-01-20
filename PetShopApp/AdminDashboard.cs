using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;




namespace PetShopApp
{
    public partial class AdminDashboard : Form
    {

        private string connString = $@"Data Source={Environment.MachineName}\SQLEXPRESS; Initial Catalog=PetShopManagementDB; Integrated Security=True";
        private Panel pnlSidebar, pnlHeader, pnlMain, pnlFooter;
        private Label lblWelcome, lblSubText, lblTime, lblLogo;
        private Dictionary<string, float> categorySales = new Dictionary<string, float>();
        private List<ItemStat> topItems = new List<ItemStat>();
        private FlowLayoutPanel flowReviewList;
        private Panel pnlSpotlight;

        public AdminDashboard()
        {
            InitializeComponent();
            SetupForm();

            // 🛠️ ORDER GURUTTO-PURNO (Layering Logic)
            BuildSidebar(); // 1. Sidebar agey (Full Left Height)
            BuildHeader();  // 2. Header Top-e
            BuildFooter();  // 3. Footer Bottom-e (Sidebar-er pashe spacer shoho)

            // 4. MAIN PANEL (Baki faka jaygatu automatic nibe)
            pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(30),
                AutoScroll = false
            };
            this.Controls.Add(pnlMain);
            pnlMain.BringToFront(); // Shob layer-er upore rakhar jonno

            ShowDefaultDashboardContent();
            
            // Real-time Clock
            Timer timer = new Timer { Interval = 1000 };
            timer.Tick += (s, e) => {
                if (lblTime != null) lblTime.Text = DateTime.Now.ToString("dddd, MMM dd, yyyy\nhh:mm:ss tt");
            };
            timer.Start();
        }

        private void SetupForm()
        {
            this.Text = "PetShop Premium | King's Control Panel";
            this.Size = new Size(1150, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 253);
        }

        private void BuildSidebar()
        {
            pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 280, BackColor = Color.FromArgb(31, 41, 55) };
            this.Controls.Add(pnlSidebar);

            // 🐾 Logo Circle
            PictureBox pbLogo = new PictureBox { Size = new Size(80, 80), Location = new Point(100, 20) };
            pbLogo.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(230, 126, 34)), 0, 0, 78, 78);
                TextRenderer.DrawText(e.Graphics, "🐾", new Font("Segoe UI", 28), new Point(15, 8), Color.White);
            };
            pnlSidebar.Controls.Add(pbLogo);

            // 🏷️ Logo Text Label
            lblLogo = new Label
            {
                Text = "🐾 PET SHOP",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(38, 110),
                AutoSize = true
            };
            pnlSidebar.Controls.Add(lblLogo);

            // 🔘 Sidebar Buttons (Ager motoi normal text)
            int y = 170;
            AddMenuBtn("📊   Dashboard Overview", y, OpenDashboardOverview);
            AddMenuBtn("📦   Inventory Stock", y += 70, OpenInventoryStock);
            AddMenuBtn("💰   Sales Analytics", y += 70, OpenSalesAnalytics);
            AddMenuBtn("🚚   Delivery Tracking", y += 70, OpenDeliveryTracking);
            AddMenuBtn("⭐   Customer Reviews", y += 70, OpenCustomerReviews);

            AddMenuBtn("🚪   Logout System", 620, LogoutSystem);
        }

        private void BuildHeader()
        {
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 140, BackColor = Color.FromArgb(23, 31, 42) };
            this.Controls.Add(pnlHeader);

            // Welcome Text
            lblWelcome = new Label
            {
                Text = "WELCOME BACK, MASTER!",
                Font = new Font("Segoe UI Black", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 30),
                AutoSize = true
            };

            lblSubText = new Label
            {
                Text = "👑 Your Pet Somrajjo is under your command, King!",
                Font = new Font("Segoe UI Semibold", 13),
                ForeColor = Color.FromArgb(230, 126, 34),
                Location = new Point(34, 85),
                AutoSize = true
            };

            // Clock/Date on Right
            lblTime = new Label
            {
                Text = "Loading...",
                Font = new Font("Segoe UI Semibold", 13),
                ForeColor = Color.FromArgb(0, 190, 255),
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 30, 0),
                AutoSize = false,
                Size = new Size(380, 140)
            };

            pnlHeader.Controls.AddRange(new Control[] { lblWelcome, lblSubText, lblTime });
        }

        private void BuildFooter()
        {
            pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(230, 126, 34)
            };
            this.Controls.Add(pnlFooter);

            // 🛡️ Spacer: Sidebar-er niche jate faka thake (x=280)
            Panel footerSpacer = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
                BackColor = Color.Transparent
            };
            pnlFooter.Controls.Add(footerSpacer);

            Label lblFooterText = new Label
            {
                Text = "🔥 POWERFUL PET MANAGEMENT ENGINE v2.0 LIVE | SYSTEM SECURED",
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false
            };
            pnlFooter.Controls.Add(lblFooterText);
        }

        private void AddMenuBtn(string txt, int y, EventHandler ev)
        {
            Button btn = new Button
            {
                Text = txt,
                Size = new Size(280, 65),
                Location = new Point(0, y),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 11),
                ForeColor = Color.FromArgb(224, 224, 224),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(30, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 59, 75);
            btn.Click += ev;
            pnlSidebar.Controls.Add(btn);
        }

        private void ShowDefaultDashboardContent()
        {
            pnlMain.Controls.Clear();

            // 1. Title - Eita thakbei
            Label lbl = new Label
            {
                Text = "🏰 SOMRAJJO OVERVIEW",
                Font = new Font("Segoe UI Black", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(30, 30),
                AutoSize = true
            };
            pnlMain.Controls.Add(lbl);

            // 2. Wallpaper PictureBox
            PictureBox pbWallpaper = new PictureBox
            {
                Name = "pbDashboardWallpaper",
                Size = new Size(pnlMain.Width - 60, 400), // Side theke ektu faka rakhbe
                Location = new Point(30, 90),
                SizeMode = PictureBoxSizeMode.StretchImage, // Pic jeno chapa na khay
                Image = Properties.Resources.admin_wallpaper, // 👈 Mama, tor Resource theke pic-er naam eikhane hobe
                                                              // Image = Image.FromFile("C:\\path\\to\\your\\pic.jpg"), // Alternative jodi path diye koro
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right // Window boro korle eitao boro hobe
            };

            // Jodi ekhon-o resource-e pic na thake, error jeno na dey shetar jonno check
            try
            {
                pnlMain.Controls.Add(pbWallpaper);
            }
            catch
            {
                // Pic na paile ekta gray box thakbe just
                pbWallpaper.BackColor = Color.LightGray;
                pnlMain.Controls.Add(pbWallpaper);
            }

            // 3. MAMA, Placeholder for future work (Buttons/Tiles)
            // TODO: Eikhane amra pore custom UserControl ba Summary Tiles call korbo
            /*
            DashboardStatsControl stats = new DashboardStatsControl();
            stats.Dock = DockStyle.Bottom;
            pnlMain.Controls.Add(stats);
            */
        }

        private void ResetButtonColors(object sender)
        {
            foreach (Control c in pnlSidebar.Controls) { if (c is Button b) { b.BackColor = Color.Transparent; b.ForeColor = Color.FromArgb(200, 200, 200); } }
            Button activeBtn = (Button)sender; activeBtn.BackColor = Color.FromArgb(45, 59, 75); activeBtn.ForeColor = Color.White;
        }

        // Event Handlers
        private void OpenDashboardOverview(object sender, EventArgs e)
        {
            ResetButtonColors(sender);
            
            LoadDashboardWithDB();
        }

     

        private void LoadDashboardWithDB()
        {
            pnlMain.Controls.Clear(); // 👈 'this' er bodole 'pnlMain' hobe
            pnlMain.BackColor = Color.FromArgb(15, 15, 30); // Panel-er color dark hobe
            string revenue = "$0", orders = "0", stock = "0", alerts = "0";
            DataTable dtRecentActivity = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // 1. Total Revenue
                    SqlCommand cmdRev = new SqlCommand("SELECT ISNULL(SUM(TotalAmount), 0) FROM Orders", conn);
                    revenue = "$" + Convert.ToDouble(cmdRev.ExecuteScalar()).ToString("N0");

                    // 2. Count Total Orders
                    SqlCommand cmdOrd = new SqlCommand("SELECT COUNT(*) FROM Orders", conn);
                    orders = cmdOrd.ExecuteScalar().ToString();

                    // 3. Total Inventory Quantity
                    SqlCommand cmdStock = new SqlCommand("SELECT ISNULL(SUM(Quantity), 0) FROM Inventory", conn);
                    stock = cmdStock.ExecuteScalar().ToString();

                    // 4. Low Stock Alerts
                    SqlCommand cmdAlert = new SqlCommand("SELECT COUNT(*) FROM Inventory WHERE Quantity < 5", conn);
                    alerts = cmdAlert.ExecuteScalar().ToString().PadLeft(2, '0');

                    // 5. Fetch Recent Activities (FIXED JOIN QUERY)
                    // Tor schema te Orders e CustomerId ache, tai Users er sathe join kore FullName anchi
                    string query = @"
                SELECT TOP 5 
                    o.CashMemoNo as [INV ID], 
                    u.FullName as [CUSTOMER], 
                    o.TotalAmount as [BILL], 
                    o.OrderStatus as [STATUS]
                FROM Orders o
                LEFT JOIN Users u ON o.CustomerId = u.UserId
                ORDER BY o.OrderId DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.Fill(dtRecentActivity);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mama, Database e jhamela hoise: " + ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            BuildOverviewUI(revenue, orders, stock, alerts, dtRecentActivity);
        }

        private void BuildOverviewUI(string rev, string ord, string stck, string alrt, DataTable dt)
        {
            // --- 1. HEADER SECTION ---
            Label lblTitle = new Label
            {
                Text = "👑 SOMRAJJO OVERVIEW",
                Font = new Font("Segoe UI Black", 22, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            Label lblSub = new Label
            {
                Text = "Live Data: Connected to PetShopManagementDB",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(0, 255, 255), // Cyan color for connection status
                Location = new Point(34, 60),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            pnlMain.Controls.Add(lblTitle);
            pnlMain.Controls.Add(lblSub);

            // --- 2. LIVE VIBRANT TILES ---
            AddGradientTile("TOTAL REVENUE", rev, "💰", Color.FromArgb(0, 242, 96), Color.FromArgb(5, 117, 230), 25, 90);
            AddGradientTile("TOTAL ORDERS", ord, "📦", Color.FromArgb(252, 74, 26), Color.FromArgb(247, 183, 51), 232, 90);
            AddGradientTile("PETS IN STOCK", stck, "🐾", Color.FromArgb(142, 45, 226), Color.FromArgb(74, 0, 224), 439, 90);
            AddGradientTile("LOW STOCK ALERTS", alrt, "🔥", Color.FromArgb(255, 0, 128), Color.FromArgb(120, 2, 6), 646, 90);

            // --- 3. RECENT ACTIVITY PANEL ---
            Panel pnlTableContainer = new Panel
            {
                Size = new Size(800, 290),
                Location = new Point(20, 220),
                BackColor = Color.FromArgb(25, 25, 50),
            };

            pnlTableContainer.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen p = new Pen(Color.FromArgb(50, 60, 100), 2))
                {
                    GraphicsPath path = GetRoundedRectanglePath(pnlTableContainer.ClientRectangle, 15);
                    e.Graphics.DrawPath(p, path);
                }
            };

            Label lblTableLabel = new Label
            {
                Text = "⚡ RECENT EMPIRE ACTIVITIES (LIVE)",
                Font = new Font("Segoe UI Black", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 255, 255),
                Location = new Point(20, 15),
                AutoSize = true
            };
            pnlTableContainer.Controls.Add(lblTableLabel);

            // --- 4. NEON DATAGRID ---
            DataGridView dgv = SetupNeonGrid();
            dgv.DataSource = dt; // Database theke asha table ta eikhane set hobe

            pnlTableContainer.Controls.Add(dgv);
            pnlMain.Controls.Add(pnlTableContainer);
        }

        private DataGridView SetupNeonGrid()
        {
            DataGridView dgv = new DataGridView
            {
                Location = new Point(15, 45),
                Size = new Size(760, 210),
                BackgroundColor = Color.FromArgb(25, 25, 50),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(45, 50, 80),
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowTemplate = { Height = 45 }
            };
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 40, 75);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(0, 255, 255);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(25, 25, 50);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(40, 60, 120);
            return dgv;
        }

        private void AddGradientTile(string title, string val, string icon, Color c1, Color c2, int x, int y)
        {
            Panel card = new Panel { Size = new Size(190, 110), Location = new Point(x, y), Cursor = Cursors.Hand };
            card.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                LinearGradientBrush lgb = new LinearGradientBrush(card.ClientRectangle, c1, c2, 45f);
                GraphicsPath path = GetRoundedRectanglePath(card.ClientRectangle, 15);
                e.Graphics.FillPath(lgb, path);
            };

            Label lIcon = new Label { Text = icon, Font = new Font("Segoe UI", 24), ForeColor = Color.FromArgb(180, 255, 255, 255), Location = new Point(135, 10), AutoSize = true, BackColor = Color.Transparent };
            Label lVal = new Label { Text = val, Font = new Font("Segoe UI Black", 18), ForeColor = Color.White, Location = new Point(15, 35), AutoSize = true, BackColor = Color.Transparent };
            Label lTitle = new Label { Text = title, Font = new Font("Segoe UI Bold", 8), ForeColor = Color.FromArgb(230, 255, 255, 255), Location = new Point(17, 85), AutoSize = true, BackColor = Color.Transparent };

            card.Controls.AddRange(new Control[] { lIcon, lVal, lTitle });
            pnlMain.Controls.Add(card);
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.X + rect.Width - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.X + rect.Width - d, rect.Y + rect.Height - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - d, d, d, 90, 90);
            path.CloseAllFigures();
            return path;
        }




        private void OpenInventoryStock(object sender, EventArgs e) { ResetButtonColors(sender); AdminInventoryManagement inv = new AdminInventoryManagement(); inv.Show(); this.Hide(); }
        private void OpenSalesAnalytics(object sender, EventArgs e) 
        {
            
            ResetButtonColors(sender);
            pnlMain.BackColor = Color.FromArgb(10, 10, 25);
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // 1. LEFT SIDE: Category wise total sales
                    string pieQuery = @"SELECT Category, SUM(ISNULL(SellCount, 0)) as Total 
                                        FROM Inventory 
                                        GROUP BY Category 
                                        HAVING SUM(ISNULL(SellCount, 0)) > 0";

                    SqlCommand cmd1 = new SqlCommand(pieQuery, conn);
                    using (SqlDataReader reader1 = cmd1.ExecuteReader())
                    {
                        categorySales.Clear();
                        while (reader1.Read())
                        {
                            categorySales.Add(reader1["Category"].ToString(), Convert.ToSingle(reader1["Total"]));
                        }
                    }

                    // 2. RIGHT & BOTTOM: Top 6 Items
                    string itemQuery = @"SELECT TOP 6 Category, Breed, ISNULL(SellCount, 0) as SellCount 
                                        FROM Inventory 
                                        ORDER BY ISNULL(SellCount, 0) DESC";

                    SqlCommand cmd2 = new SqlCommand(itemQuery, conn);
                    using (SqlDataReader reader2 = cmd2.ExecuteReader())
                    {
                        topItems.Clear();
                        while (reader2.Read())
                        {
                            topItems.Add(new ItemStat
                            {
                                Category = reader2["Category"].ToString(),
                                ItemName = reader2["Breed"].ToString(),
                                Sales = Convert.ToInt32(reader2["SellCount"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB Error: " + ex.Message);
            }

            BuildProDashboard();
        }

        private void BuildProDashboard()
        {
            pnlMain.Controls.Clear();

            // 1. MAIN HEADER
            Label lblHeader = new Label
            {
                Text = "👑 EMPIRE LIVE INTELLIGENCE",
                Font = new Font("Segoe UI Black", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 255, 200),
                Location = new Point(25, 20),
                AutoSize = true
            };
            pnlMain.Controls.Add(lblHeader);

            // 2. BIG DONUT PIE CHART (Left Side)
            Panel pnlBigPie = new Panel { Size = new Size(400, 200), Location = new Point(25, 70), BackColor = Color.Transparent };
            pnlBigPie.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Color[] colors = { Color.Red, Color.DodgerBlue, Color.HotPink, Color.Gold, Color.Lime, Color.Cyan };
                float totalValue = 0;
                foreach (var val in categorySales.Values) totalValue += val;

                if (totalValue == 0)
                {
                    e.Graphics.DrawString("NO SALES DATA YET", new Font("Segoe UI Bold", 10), Brushes.Gray, 50, 100);
                    return;
                }

                float startAngle = 0;
                int i = 0;
                foreach (var kvp in categorySales)
                {
                    float sweep = (kvp.Value / totalValue) * 360f;
                    using (Pen p = new Pen(Color.FromArgb(200, colors[i % colors.Length]), 40))
                        e.Graphics.DrawArc(p, 25, 30, 150, 150, startAngle, sweep);

                    DrawLegend(e.Graphics, kvp.Key, colors[i % colors.Length], 240, 25 + (i * 32));
                    startAngle += sweep;
                    i++;
                }
                e.Graphics.DrawString("LIVE", new Font("Segoe UI Black", 12), Brushes.White, 85, 95);
            };
            pnlMain.Controls.Add(pnlBigPie);

            // --- 3. RIGHT SIDE: 4 CHAMPIONS (With Pics) ---
            Label lblAwardTitle = new Label
            {
                Text = "🏆 CHAMPIONS",
                Font = new Font("Segoe UI Black", 12, FontStyle.Bold),
                ForeColor = Color.Gold,
                Location = new Point(460, 45),
                AutoSize = true
            };
            pnlMain.Controls.Add(lblAwardTitle);

            for (int k = 0; k < topItems.Count && k < 4; k++)
            {
                int r = k / 2; int c = k % 2;
                AddAwardCard(topItems[k], 460 + (c * 190), 81 + (r * 125));
            }

            // 4. BOTTOM HEADING
            Label lblBottomHeading = new Label
            {
                Text = "📊 TOP PERFORMANCE BY PRODUCT (BREED)",
                Font = new Font("Segoe UI Black", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 200, 0),
                Location = new Point(25, 320),
                AutoSize = true
            };
            pnlMain.Controls.Add(lblBottomHeading);

            // 5. MINI GRAPHS
            int xStart = 25, yStart = 365, cardW = 260, cardH = 65;
            Color[] itemColors = { Color.OrangeRed, Color.DodgerBlue, Color.HotPink, Color.Gold, Color.Lime, Color.Cyan };

            int maxSales = 0;
            foreach (var item in topItems) if (item.Sales > maxSales) maxSales = item.Sales;
            if (maxSales == 0) maxSales = 1;

            for (int j = 0; j < topItems.Count; j++)
            {
                int row = j / 3;
                int col = j % 3;
                int progressWidth = (int)((topItems[j].Sales / (float)maxSales) * 230);

                AddItemStat(topItems[j].Category + ": " + topItems[j].ItemName,
                           topItems[j].Sales,
                           progressWidth,
                           itemColors[j % itemColors.Length],
                           xStart + (col * (cardW + 10)),
                           yStart + (row * (cardH + 10)));
            }
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {

        }

        private void AddAwardCard(ItemStat item, int x, int y)
        {
            // Background ta transparent rakhle gradient ta shundor futbe
            Panel card = new Panel { Size = new Size(165, 105), Location = new Point(x, y), BackColor = Color.Transparent };

            card.Paint += (s, e) => {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // 1. Background Glassy Look (Dark Gradient)
                Rectangle rect = new Rectangle(0, 0, card.Width - 5, card.Height - 5);
                using (LinearGradientBrush lgb = new LinearGradientBrush(rect, Color.FromArgb(45, 45, 80), Color.FromArgb(20, 20, 40), 45f))
                {
                    g.FillRoundedRectangle(lgb, 0, 0, rect.Width, rect.Height, 12);
                }

                // 2. Gold Border
                using (Pen borderPen = new Pen(Color.FromArgb(120, Color.Gold), 1))
                {
                    g.DrawRoundedRectangle(borderPen, 0, 0, rect.Width, rect.Height, 12);
                }

                try
                {
                    // 3. Image Frame Logic
                    string resName = item.ItemName.Replace(" ", "_");
                    object img = Properties.Resources.ResourceManager.GetObject(resName);

                    if (img != null)
                    {
                        using (GraphicsPath picPath = new GraphicsPath())
                        {
                            picPath.AddRoundedRectangle(new Rectangle(10, 10, 80, 60), 8);
                            g.SetClip(picPath);
                            g.DrawImage((Image)img, 10, 10, 85, 65);
                            g.ResetClip();
                        }
                    }
                    else
                    {
                        g.FillRoundedRectangle(Brushes.DimGray, 10, 10, 85, 65, 8);
                    }
                }
                catch { g.FillRoundedRectangle(Brushes.Maroon, 10, 10, 85, 65, 8); }

                // 4. Gold Badge (TOP)
                g.FillRoundedRectangle(new SolidBrush(Color.FromArgb(220, Color.Gold)), 110, 15, 60, 20, 6);
                g.DrawString("CHAMP", new Font("Segoe UI Black", 7), Brushes.Black, 118, 18);

                // 5. Name and Sales
                g.DrawString(item.ItemName.ToUpper(), new Font("Segoe UI Black", 9), Brushes.White, 10, 82);

                // Neon green indicator
                g.FillEllipse(new SolidBrush(Color.FromArgb(0, 255, 150)), 12, 105, 7, 7);
                g.DrawString(item.Sales + " SOLD", new Font("Segoe UI Bold", 7), Brushes.SpringGreen, 24, 102);
            };
            pnlMain.Controls.Add(card);
        }

        private void AddItemStat(string title, int val, int barWidth, Color c, int x, int y)
        {
            Panel p = new Panel { Size = new Size(260, 65), Location = new Point(x, y), BackColor = Color.FromArgb(20, 20, 45) };
            p.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawString(title.ToUpper(), new Font("Segoe UI Bold", 8), Brushes.Silver, 15, 10);

                using (SolidBrush sb = new SolidBrush(Color.FromArgb(40, c)))
                    e.Graphics.FillRoundedRectangle(sb, 15, 32, 230, 8, 4);

                if (barWidth > 0)
                {
                    using (LinearGradientBrush lgb = new LinearGradientBrush(new Rectangle(15, 32, barWidth, 8), c, Color.White, 0f))
                        e.Graphics.FillRoundedRectangle(lgb, 15, 32, barWidth, 8, 4);
                }

                e.Graphics.DrawString(val + " Units Sold", new Font("Segoe UI", 7, FontStyle.Italic), new SolidBrush(c), 15, 45);
            };
            pnlMain.Controls.Add(p);

        }

        private void DrawLegend(Graphics g, string txt, Color c, int x, int y)
        {
            using (SolidBrush sb = new SolidBrush(c)) g.FillRoundedRectangle(sb, x, y, 10, 10, 3);
            g.DrawString(txt, new Font("Segoe UI Bold", 9), Brushes.LightGray, x + 18, y - 2);
        }


        private void OpenDeliveryTracking(object sender, EventArgs e) { ResetButtonColors(sender); MessageBox.Show("Tracking Page Coming!"); }
        // ১. কাস্টমার রিভিউ পেজ ওপেন করার মেথড
        private void OpenCustomerReviews(object sender, EventArgs e)
        {
            ResetButtonColors(sender);

            // প্যানেল ক্লিয়ার করে নতুন ফর্ম লোড করা
            pnlMain.Controls.Clear();
            pnlMain.BackColor = Color.FromArgb(10, 10, 25); // তোর পছন্দের Midnight Blue

            SetupDarkPremiumUI();
            LoadReviewDashboardData();
        }

        private void SetupDarkPremiumUI()
        {
            // --- Form Styling (Dark Theme) ---
            pnlMain.Size = new Size(870, 560);
            pnlMain.BackColor = Color.FromArgb(10, 10, 25); // Deep Midnight Blue
           

            // --- 1. Top Glass Header ---
            Panel pnlHeader = new Panel { Size = new Size(870, 45), Dock = DockStyle.Top, BackColor = Color.FromArgb(15, 15, 35) };
            Label lblTitle = new Label
            {
                Text = "Welcome back, Boss! Your customers are talking about their pets today.",
                Font = new Font("Segoe UI Black", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 210, 255), // Neon Blue
                Location = new Point(25, 0),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblTitle);
            pnlMain.Controls.Add(pnlHeader);

            // --- 2. LEFT: Social Review Feed (580px) ---
            flowReviewList = new FlowLayoutPanel
            {
                Location = new Point(15, 90),
                Size = new Size(575, 455),
                AutoScroll = true,
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(5, 5, 0, 20)
            };
            pnlMain.Controls.Add(flowReviewList);

            // --- 3. RIGHT: Fan Favorite Spotlight (255px) ---
            pnlSpotlight = new Panel
            {
                Location = new Point(600, 85),
                Size = new Size(250, 390),
                BackColor = Color.FromArgb(20, 20, 45),
                AutoScroll = false // Spotlight-e sadharonoto scroll lage na, tai false rakha bhalo
            };
            pnlSpotlight.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pnlSpotlight.Width, pnlSpotlight.Height, 30, 30));
            pnlMain.Controls.Add(pnlSpotlight);
            

            BuildDarkSpotlight();
        }

        private void BuildDarkSpotlight()
        {
            Label lblBadge = new Label { Text = "🏆 CHAMPION PET", Font = new Font("Segoe UI Black", 9), ForeColor = Color.Gold, Location = new Point(20, 25), AutoSize = true };

            PictureBox picSpot = new PictureBox
            {
                Name = "picSpot",
                Size = new Size(170, 170),
                Location = new Point(40, 65),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(30, 30, 60)
            };
            picSpot.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, picSpot.Width, picSpot.Height, 30, 30));

            Label lblName = new Label { Name = "lblTopPet", Text = "PET NAME", Font = new Font("Segoe UI Black", 15), ForeColor = Color.White, Location = new Point(10, 255), Width = 230, TextAlign = ContentAlignment.MiddleCenter };

            Label lblDesc = new Label
            {
                Text = "Highest satisfaction rating from the pet-lover community.",
                Font = new Font("Segoe UI Semibold", 9),
                ForeColor = Color.FromArgb(150, 150, 180),
                Location = new Point(20, 300),
                Size = new Size(210, 50),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Glow Effect Line
            Panel pnlGlow = new Panel { Size = new Size(120, 3), Location = new Point(65, 370), BackColor = Color.FromArgb(0, 210, 255) };

            pnlSpotlight.Controls.AddRange(new Control[] { lblBadge, picSpot, lblName, lblDesc, pnlGlow });
        }

        private void LoadReviewDashboardData()
        {
            flowReviewList.Controls.Clear();
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string topSql = "SELECT TOP 1 ItemName FROM ProductReviews GROUP BY ItemName ORDER BY AVG(CAST(Rating AS FLOAT)) DESC";
                    var topPet = new SqlCommand(topSql, conn).ExecuteScalar()?.ToString() ?? "N/A";

                    pnlMain.Controls.Find("lblTopPet", true)[0].Text = topPet.ToUpper();
                    PictureBox pb = (PictureBox)pnlMain.Controls.Find("picSpot", true)[0];
                    try { pb.Image = (Image)Properties.Resources.ResourceManager.GetObject(topPet.Replace(" ", "_")); } catch { }

                    string sql = "SELECT Username, ItemName, Comment, Rating, ReviewDate FROM ProductReviews ORDER BY ReviewDate DESC";
                    SqlDataReader r = new SqlCommand(sql, conn).ExecuteReader();
                    while (r.Read())
                    {
                        flowReviewList.Controls.Add(CreateDarkCard(
                            r["Username"].ToString(),
                            r["ItemName"].ToString(),
                            r["Comment"].ToString(),
                            Convert.ToInt32(r["Rating"]),
                            Convert.ToDateTime(r["ReviewDate"]).ToString("dd MMM")
                        ));
                    }
                }
            }
            catch { /* Add manual test samples here if DB is not ready */ }
        }

        private Panel CreateDarkCard(string user, string pet, string msg, int rating, string date)
        {
            Panel card = new Panel { Size = new Size(530, 115), BackColor = Color.FromArgb(25, 25, 50), Margin = new Padding(0, 0, 0, 15) };
            card.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, card.Width, card.Height, 20, 20));

            PictureBox pic = new PictureBox { Size = new Size(65, 65), Location = new Point(15, 25), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.FromArgb(40, 40, 70) };
            try { pic.Image = (Image)Properties.Resources.ResourceManager.GetObject(pet.Replace(" ", "_")); } catch { }

            Label lblU = new Label { Text = user, Font = new Font("Segoe UI Black", 11), Location = new Point(95, 20), AutoSize = true, ForeColor = Color.White };
            Label lblP = new Label { Text = "Product: " + pet, Font = new Font("Segoe UI Semibold", 8), ForeColor = Color.FromArgb(0, 210, 255), Location = new Point(97, 42), AutoSize = true };
            Label lblS = new Label { Text = new string('★', rating), Font = new Font("Segoe UI", 12), ForeColor = Color.Gold, Location = new Point(95, 58), AutoSize = true };
            Label lblM = new Label { Text = msg, Font = new Font("Segoe UI Semibold", 9, FontStyle.Italic), Location = new Point(97, 85), Size = new Size(380, 25), ForeColor = Color.FromArgb(180, 180, 200) };
            Label lblD = new Label { Text = date, Font = new Font("Segoe UI", 7, FontStyle.Bold), ForeColor = Color.FromArgb(80, 80, 110), Location = new Point(475, 15), AutoSize = true };

            card.Controls.AddRange(new Control[] { pic, lblU, lblP, lblS, lblM, lblD });
            return card;
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);


        private void LogoutSystem(object sender, EventArgs e) { if (MessageBox.Show("Logout, King?", "Exit Empire", MessageBoxButtons.YesNo) == DialogResult.Yes) Application.Exit(); }
    }


    public class ItemStat
    {
        public string Category { get; set; }
        public string ItemName { get; set; }
        public int Sales { get; set; }
    }

    public static class GraphicsExtensions
    {
        // Rounded Rectangle Fill korar jonno
        public static void FillRoundedRectangle(this Graphics g, Brush brush, int x, int y, int width, int height, int radius)
        {
            using (GraphicsPath path = GetRoundedPath(x, y, width, height, radius))
            {
                g.FillPath(brush, path);
            }
        }

        // Rounded Rectangle Border/Outline anar jonno (Eta missing silo)
        public static void DrawRoundedRectangle(this Graphics g, Pen pen, int x, int y, int width, int height, int radius)
        {
            using (GraphicsPath path = GetRoundedPath(x, y, width, height, radius))
            {
                g.DrawPath(pen, path);
            }
        }

        // Path ta bar bar lagbe tai common function kora hoyeche
        public static void AddRoundedRectangle(this GraphicsPath path, Rectangle rect, int radius)
        {
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
        }

        private static GraphicsPath GetRoundedPath(int x, int y, int width, int height, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(x, y, radius, radius, 180, 90);
            path.AddArc(x + width - radius, y, radius, radius, 270, 90);
            path.AddArc(x + width - radius, y + height - radius, radius, radius, 0, 90);
            path.AddArc(x, y + height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            return path;
        }
    }


}