using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;


namespace PetShopApp
{
    public partial class CustomerPaymentCashMemo : Form
    {

        private string connString = $@"Data Source={Environment.MachineName}\SQLEXPRESS; Initial Catalog=PetShopManagementDB; Integrated Security=True";

        public int CurrentOrderId { get; set; } // Dashboard theke pathano ID ekhane ashbe
                                                // MAMA EI LINE-TA ADD KORO
        public List<(string Name, double Price)> PurchasedItems { get; set; } = new List<(string Name, double Price)>();
        private Panel pnlHeader, pnlFooter, pnlMainContainer, pnlPromo;
        private Label lblClock, lblDate;
        private Timer timerClock;
        private Label lblUserName;
        private string selectedPaymentMethod = "";
        private string selectedDeliveryMethod = ""; // Delivery check korar jonno

        public string OrderCustomerName { get; set; } = "Guest Mama";
        public decimal OrderTotalBill { get; set; } = 0;
        

        // Ar totalBill error-er jonno local variable define kor:
        

        public CustomerPaymentCashMemo()
        {
            InitializeComponent();
            SetupProfessionalForm();

            // MAMA EIKHANE CODE KOMAYE DAO
            this.Load += (s, e) => {
                BuildHeader();
                BuildFooter();
                BuildBodyLayout(); // Load-er bhetore thakle data pabe
                StartClock();
                lblUserName.Text = OrderCustomerName.ToUpper();
            };
        }
        private void SetupProfessionalForm()
        {
            this.Text = "Pet-Universe | Premium Secure Checkout";
            this.Size = new Size(1150, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void BuildHeader()
        {
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 110, BackColor = Color.FromArgb(23, 31, 42) };
            this.Controls.Add(pnlHeader);

            // 1. Logo & Tracker
            Label lblLogo = new Label { Text = "🐾 CHECKOUT", Font = new Font("Segoe UI Black", 22, FontStyle.Bold), ForeColor = Color.FromArgb(230, 126, 34), Location = new Point(25, 15), AutoSize = true };
            Label lblTracker = new Label { Text = "Cart ➔ Delivery ➔ [Payment Interface]", Font = new Font("Segoe UI Semibold", 9), ForeColor = Color.FromArgb(46, 204, 113), Location = new Point(30, 60), AutoSize = true };

            // 2. Real-time Clock & Date
            lblClock = new Label { Text = "00:00:00 AM", Font = new Font("Segoe UI Black", 14), ForeColor = Color.White, Location = new Point(450, 20), Size = new Size(250, 30), TextAlign = ContentAlignment.MiddleCenter };
            lblDate = new Label { Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy"), Font = new Font("Segoe UI Semibold", 9), ForeColor = Color.Gold, Location = new Point(450, 50), Size = new Size(250, 30), TextAlign = ContentAlignment.MiddleCenter };

            // 3. Updated Profile Section with Name
            // Location ektu piche nisi jate naam dharte pare (820, 15)
            Panel pnlProfileWrapper = new Panel { Size = new Size(300, 70), Location = new Point(820, 15), BackColor = Color.Transparent, Cursor = Cursors.Hand };

            // Customer Name (lblUserName)
            lblUserName = new Label
            {
                Text = "MASTER MAMA",
                Font = new Font("Segoe UI Black", 11),
                ForeColor = Color.White,
                Location = new Point(10, 20),
                Size = new Size(200, 30),
                TextAlign = ContentAlignment.MiddleRight
            };

            // Rounded Profile Icon
            PictureBox pbUserIcon = new PictureBox { Size = new Size(60, 60), Location = new Point(220, 5), BackColor = Color.Transparent };
            pbUserIcon.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddEllipse(0, 0, 58, 58);
                    pbUserIcon.Region = new Region(gp);
                    e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(45, 59, 75)), 0, 0, 58, 58);
                    TextRenderer.DrawText(e.Graphics, "👤", new Font("Segoe UI", 20), new Point(7, 7), Color.White);
                }
            };

            // Click Events for entire profile area
            pnlProfileWrapper.Click += (s, e) => Action_ProfileClicked();
            lblUserName.Click += (s, e) => Action_ProfileClicked();
            pbUserIcon.Click += (s, e) => Action_ProfileClicked();

            // Wrapper e shob add kora
            pnlProfileWrapper.Controls.Add(lblUserName);
            pnlProfileWrapper.Controls.Add(pbUserIcon);

            // Header e shob add kora
            pnlHeader.Controls.AddRange(new Control[] { lblLogo, lblTracker, lblClock, lblDate, pnlProfileWrapper });
        }

        private void BuildBodyLayout()
        {
            pnlMainContainer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), BackColor = Color.Transparent };
            this.Controls.Add(pnlMainContainer);
            pnlMainContainer.BringToFront();

            FlowLayoutPanel flowBody = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, AutoScroll = true };
            pnlMainContainer.Controls.Add(flowBody);

            // --- Column 1: Memo ---
            Panel p1 = CreateCard("ORDER SUMMARY", 340, "📄");
            Panel receipt = new Panel { Size = new Size(300, 410), Location = new Point(20, 65), BackColor = Color.White };

            // MAMA EIKHANE LIST THEKE DATA NIYE LEKHA HOBE
            string allItemsText = "";
            foreach (var item in PurchasedItems)
            {
                allItemsText += $"{item.Name.PadRight(15)} .... ${item.Price}\n";
            }
            // MAMA, Receipt-er bhetore real feel anar jonno eita use koro
            Label lblContent = new Label
            {
                Text = $@"
      PET-UNIVERSE SHOP
    ---------------------
    Plot 10, Road 5, Dhaka
    Hotline: +88012345678
    ---------------------
    INVOICE: #ORD-{DateTime.Now.Ticks.ToString().Substring(10)}
    DATE: {DateTime.Now:dd/MM/yyyy HH:mm}
    CUST: {OrderCustomerName.ToUpper()}
    ---------------------
    ITEM        QTY    PRICE
    ---------------------
    {GenerateFormattedItems()}
    ---------------------
    SUBTOTAL:         ${OrderTotalBill}
    DISCOUNT:         $0.00
    ---------------------
    NET TOTAL:        ${OrderTotalBill}
    ---------------------
      PAID VIA: {selectedPaymentMethod}
    
    THANK YOU FOR SHOPPING!
    Sir, Good wishes for You cute pet! 🐾",
                Font = new Font("Consolas", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 52, 54),
                Location = new Point(10, 10),
                Size = new Size(280, 390),
                TextAlign = ContentAlignment.TopLeft
            };
            receipt.Controls.Add(lblContent);
            p1.Controls.Add(receipt);

            // --- Column 2: Delivery & "Buy More" ---
            Panel p2 = CreateCard("DELIVERY METHOD", 350, "🚚");
            string[] methods = { "🚀 Pathao Fast", "🐼 FoodPanda Go", "🚚 RedX Logistics", "🚚 Jhinku BD", "🏠 Shop Pickup" };
            int y = 70;
            foreach (var m in methods)
            {
                RadioButton rb = new RadioButton { Text = m, Font = new Font("Segoe UI Semibold", 11), Location = new Point(30, y), Size = new Size(200, 40), Cursor = Cursors.Hand };
                rb.CheckedChanged += (s, e) => { if (rb.Checked) Action_DeliveryChanged(m); };
                p2.Controls.Add(rb);
                y += 50;
            }

            // BUY MORE Banner (Inside Delivery Column for better fit)
            pnlPromo = new Panel { Size = new Size(300, 140), Location = new Point(20, 310), BackColor = Color.FromArgb(255, 248, 225) };
            pnlPromo.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, pnlPromo.ClientRectangle, Color.FromArgb(255, 167, 38), ButtonBorderStyle.Solid);

            Label lblBabu = new Label
            {
                Text = "✨ Apnar ador-er 'Babu Pet' er jonno ki r-o kichu kinte chan?",
                Font = new Font("Segoe UI Semibold", 10),
                ForeColor = Color.FromArgb(127, 44, 0),
                Location = new Point(15, 20),
                Size = new Size(280, 50),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Button btnGoBack = new Button
            {
                Text = "🛒 BUY MORE ITEMS",
                Size = new Size(200, 45),
                Location = new Point(55, 85),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Bold", 9),
                Cursor = Cursors.Hand
            };
            btnGoBack.FlatAppearance.BorderSize = 0;
            btnGoBack.Click += (s, e) => Action_BackToShop();

            pnlPromo.Controls.AddRange(new Control[] { lblBabu, btnGoBack });
            p2.Controls.Add(pnlPromo);

            // --- Column 3: Payment ---
            Panel p3 = CreateCard("SECURE PAYMENT", 360, "💳");
            string[] pays = { "bKash", "Nagad", "Rocket", "Credit Card", "Cash on Delivery" };
            int py = 70;
            foreach (var pay in pays)
            {
                Button btn = new Button
                {
                    Text = "Pay with " + pay,
                    Size = new Size(280, 50),
                    Location = new Point(40, py),
                    BackColor = Color.FromArgb(44, 62, 80),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI Bold", 10),
                    Cursor = Cursors.Hand
                };
                btn.Click += (s, e) => Action_PaymentSelected(pay, btn);
                p3.Controls.Add(btn);
                py += 65;
            }

            flowBody.Controls.AddRange(new Control[] { p1, p2, p3 });
        }

        private Panel CreateCard(string title, int width, string icon)
        {
            Panel p = new Panel { Width = width, Height = 510, BackColor = Color.White, Margin = new Padding(10, 10, 10, 10) };
            p.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, p.ClientRectangle, Color.Gainsboro, ButtonBorderStyle.Solid);

            Label lIcon = new Label { Text = icon, Font = new Font("Segoe UI", 16), Location = new Point(15, 15), AutoSize = true };
            Label lTitle = new Label { Text = title, Font = new Font("Segoe UI Black", 12), ForeColor = Color.FromArgb(31, 41, 55), Location = new Point(50, 18), AutoSize = true };

            p.Controls.AddRange(new Control[] { lIcon, lTitle });
            return p;
        }

        private void BuildFooter()
        {
            pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 80, BackColor = Color.FromArgb(31, 41, 55) };
            this.Controls.Add(pnlFooter);

            Label lblFooterNote = new Label
            {
                Text = "🔒 256-bit SSL Encrypted | All Rights Reserved © Pet-Universe 2026",
                ForeColor = Color.DarkGray,
                Font = new Font("Segoe UI", 9),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnConfirm = new Button
            {
                Text = "CONFIRM ORDER ✔",
                Size = new Size(250, 80),
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Black", 12),
                Cursor = Cursors.Hand
            };
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.Click += (s, e) => Action_ConfirmOrder();

            pnlFooter.Controls.AddRange(new Control[] { lblFooterNote, btnConfirm });
        }

        // --- ACTION METHODS ---
        

        private void Action_ProfileClicked()
        {
            MessageBox.Show("👤 CUSTOMER PROFILE\n------------------\nName: Master Mama\nRank: Gold Member", "Profile Info");
        }
        // Global variable logic er jonno
        private int currentDeliveryManId = 8; // Default ekta set thaklo

        private void Action_DeliveryChanged(string method)
        {
            selectedDeliveryMethod = method; // Database-e AgentName hishebe jabe

            // Table-er ID (AgentID) onujayi logic:
            if (method.Contains("Pathao"))
                currentDeliveryManId = 1;
            else if (method.Contains("FoodPanda"))
                currentDeliveryManId = 2;
            else if (method.Contains("RedX"))
                currentDeliveryManId = 3;
            else if (method.Contains("Jhinku"))
                currentDeliveryManId = 4;
            else if (method.Contains("Shop Pickup"))
                currentDeliveryManId = 5;
            else
                currentDeliveryManId = 0; // Kono mil na pele
        }

        private void Action_PaymentSelected(string type, Button clickedButton)
        {
            // 1. Ekta simple dummy number check (shudhu dhorar jonno)
            // Real life e eikhane ekta TextBox wala popup lage, kintu tumi simple chaiso tai:

            DialogResult result = MessageBox.Show($"Mama, tumi ki {type} diye payment korte chao?", "Confirm Payment", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                // 2. OTP Check - Eitao amra MessageBox diye dummy bhabe korbo
                // Mama, eikhane dhore neo user "123456" OTP dise (Just demo logic)

                string testOTP = "123456"; // Static OTP

                if (testOTP == "123456")
                {
                    // Shob payment button-er color age normal (dark blue) koro
                    foreach (Control ctrl in clickedButton.Parent.Controls)
                    {
                        if (ctrl is Button b)
                        {
                            b.BackColor = Color.FromArgb(44, 62, 80);
                            b.Text = b.Text.Replace(" ✔", ""); // Age select thakle oita muche dao
                        }
                    }

                    // 3. Selected button-er appearance change koro
                    clickedButton.BackColor = Color.FromArgb(46, 204, 113); // Green color
                    clickedButton.Text += " ✔"; // Pash e ekta check mark

                    selectedPaymentMethod = type; // Method ta save kora holo

                    MessageBox.Show($"{type} selected mama! Ekhon 'Confirm Order' e click koro.", "Payment Ready");
                }
            }
        }

        private void CustomerPaymentCashMemo_Load(object sender, EventArgs e)
        {

        }

        private void Action_BackToShop() => MessageBox.Show("Returning to Customer Dashboard...", "Buy More Items");

        private void Action_ConfirmOrder()
        {
            // 1. Empty Cart Check
            if (PurchasedItems == null || PurchasedItems.Count == 0)
            {
                MessageBox.Show("Mama, cart to khali! Age kisu kinen.", "Empty Cart");
                return;
            }

            // 2. Delivery Method Check (Validation)
            if (string.IsNullOrEmpty(selectedDeliveryMethod))
            {
                MessageBox.Show("Mama, age Delivery Method (Pathao/RedX/Pickup) select koro!", "Selection Missing");
                return;
            }

            // 3. Payment Method Check (Validation)
            if (string.IsNullOrEmpty(selectedPaymentMethod))
            {
                MessageBox.Show("Mama, age payment method (bKash/Nagad/Cash) select koro!", "Selection Missing");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // --- STEP 1: Insert into Orders Table ---
                    string sqlOrder = @"INSERT INTO Orders (CustomerId, TotalAmount, OrderStatus, DeliveryManId) 
                               OUTPUT INSERTED.OrderId
                               VALUES (
                                  (SELECT UserId FROM Users WHERE Username=@uname), 
                                  @total, 
                                  'Paid', 
                                  @dmanId
                               )";

                    SqlCommand cmdOrder = new SqlCommand(sqlOrder, conn, transaction);
                    cmdOrder.Parameters.AddWithValue("@uname", OrderCustomerName);
                    cmdOrder.Parameters.AddWithValue("@total", OrderTotalBill);
                    cmdOrder.Parameters.AddWithValue("@dmanId", currentDeliveryManId);
                    int generatedId = (int)cmdOrder.ExecuteScalar();

                    // --- STEP 5: MAMA ADDED: Update DeliveryPartners Table ---
                    // Eikhane Agent-er delivery count +1 hobe ar business amount add hobe
                    string sqlUpdateAgent = @"UPDATE DeliveryPartners 
                               SET DeliveryCount = DeliveryCount + 1, 
                                   BusinessEarned = BusinessEarned + @total 
                               WHERE AgentID = @dmanId";

                    SqlCommand cmdAgent = new SqlCommand(sqlUpdateAgent, conn, transaction);
                    cmdAgent.Parameters.AddWithValue("@total", OrderTotalBill);
                    cmdAgent.Parameters.AddWithValue("@dmanId", currentDeliveryManId);
                    cmdAgent.ExecuteNonQuery();



                    // --- STEP 2: Grouping items to handle multiple quantities of same item ---
                    var groupedItems = PurchasedItems
                        .GroupBy(i => i.Name)
                        .Select(g => new {
                            Name = g.Key,
                            Qty = g.Count(),
                            UnitPrice = (decimal)g.First().Price
                        });

                    foreach (var item in groupedItems)
                    {
                        // --- STEP 3: Insert into OrderDetails ---
                        string sqlDetails = "INSERT INTO OrderDetails (OrderId, ProductName, Price, Quantity) VALUES (@oid, @pname, @price, @qty)";
                        SqlCommand cmdD = new SqlCommand(sqlDetails, conn, transaction);
                        cmdD.Parameters.AddWithValue("@oid", generatedId);
                        cmdD.Parameters.AddWithValue("@pname", item.Name);
                        cmdD.Parameters.AddWithValue("@price", item.UnitPrice);
                        cmdD.Parameters.AddWithValue("@qty", item.Qty);
                        cmdD.ExecuteNonQuery();

                        // --- STEP 4: Inventory Update (Stock - Qty, SellCount + Qty) ---
                        // ISNULL(SellCount, 0) use koro jate NULL thakle error na hoy
                        string sqlStock = "UPDATE Inventory SET Quantity = Quantity - @qty, SellCount = ISNULL(SellCount, 0) + @qty WHERE Breed = @pname";
                        SqlCommand cmdS = new SqlCommand(sqlStock, conn, transaction);
                        cmdS.Parameters.AddWithValue("@qty", item.Qty);
                        cmdS.Parameters.AddWithValue("@pname", item.Name);
                        cmdS.ExecuteNonQuery();

                    }

                    // --- STEP 3: Finalize Transaction ---
                    transaction.Commit();

                    // --- STEP 4: MAMA SUCCESS MESSAGE ---
                    string message = "MAMA, THANK YOU FOR SHOPPING! 🐾\n\n" +
                                     $"Order ID: #{generatedId}\n" +
                                     $"Delivery: {selectedDeliveryMethod}\n" +
                                     "Abar ashiben kintu!";

                    MessageBox.Show(message, "Order Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // --- STEP 5: REDIRECT TO DASHBOARD ---
                    // Tor form-er naam jodi 'CustomersDashboard' hoy sheta eikhane hobe
                    CustomersDashboard dashboard = new CustomersDashboard();
                    dashboard.Show();

                    // Current checkout form ta bondho kora
                    this.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Failed! Undo everything
                    MessageBox.Show("Database Error: " + ex.Message, "Error");
                }
            }
        }
        private string GenerateFormattedItems()
        {
            if (PurchasedItems == null || PurchasedItems.Count == 0) return "No Items Found";

            string formatted = "";
            // Grouping logic: Ek-i item bar bar thakle sheta qty hishebe dekhabe
            var items = PurchasedItems
                .GroupBy(i => i.Name)
                .Select(g => new {
                    Name = g.Key.Length > 12 ? g.Key.Substring(0, 10) + ".." : g.Key,
                    Qty = g.Count(),
                    Price = (decimal)g.First().Price
                });

            foreach (var item in items)
            {
                // PadRight(12) mane 12 ta space dore align korbe
                // string.Format use korsi jate table-er moto shundor lage
                formatted += string.Format("{0,-12} {1,3}x   ${2,6:N2}\n", item.Name, item.Qty, item.Price);
            }
            return formatted;
        }

        private void StartClock()
        {
            timerClock = new Timer { Interval = 1000 };
            timerClock.Tick += (s, e) => {
                lblClock.Text = DateTime.Now.ToString("hh:mm:ss tt");
                lblDate.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
            };
            timerClock.Start();
        }
    }
}
