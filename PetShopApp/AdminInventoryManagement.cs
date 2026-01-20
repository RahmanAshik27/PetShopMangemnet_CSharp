using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PetShopApp
{
    public partial class AdminInventoryManagement : Form
    {
        
        private string connString = $@"Data Source={Environment.MachineName}\SQLEXPRESS; Initial Catalog=PetShopManagementDB; Integrated Security=True";

        private Panel pnlSidebar, pnlMain, pnlHeader, pnlFooter, pnlPreviewCard, pnlLiveStatus;
        private FlowLayoutPanel flowCardGallery;
        private ComboBox cmbCategory, cmbSubItem;
        private NumericUpDown numQuantity;
        private TextBox txtPrice;
        private Label lblPreviewTitle, lblEmoji, lblClock, lblStockStatus, lblRating, lblReviewText;
        private PictureBox picLivePreview;
        private Timer timer;
        private DataGridView dgvInventory;

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


        public AdminInventoryManagement()
        {
            InitializeComponent();
            SetupFinalDashboard(); 
            StartClock();
            this.DoubleBuffered = true;
        }

        private void AdminInventoryManagement_Load(object sender, EventArgs e)
        {
            
        }

        private void SetupFinalDashboard()
        {
            this.Text = "PetShop Premium | Visual Inventory Pro";
            this.Size = new Size(1150, 750);
            this.StartPosition = FormStartPosition.CenterScreen;

            
            pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 315, BackColor = Color.FromArgb(45, 55, 72), Padding = new Padding(20) };
            this.Controls.Add(pnlSidebar);

            Label lblLogo = new Label { Text = "📦 Add Product", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Top, Height = 50 };
            pnlSidebar.Controls.Add(lblLogo);

            int y = 80;
            CreateSidebarLabel("Product Category", y);
            cmbCategory = CreateSidebarCombo(y + 25);
            cmbCategory.Items.Add("-- Select --");
            foreach (var key in subItems.Keys) cmbCategory.Items.Add(key);
            cmbCategory.SelectedIndex = 0;
            cmbCategory.SelectedIndexChanged += (s, e) => { UpdateUI(); ResetPriceField(); };
            y += 80;

            CreateSidebarLabel("Variety / Breed", y);
            cmbSubItem = CreateSidebarCombo(y + 25);
            cmbSubItem.SelectedIndexChanged += (s, e) => {
                SyncGallerySelection();
                LoadProductInsight();
                ResetPriceField();
                SelectRowInTable(cmbSubItem.Text);
            };
            y += 80;

            CreateSidebarLabel("Price ($)", y);
            txtPrice = new TextBox { Location = new Point(20, y + 25), Width = 280, Font = new Font("Segoe UI", 12), Text = "Enter Price...", ForeColor = Color.Gray };
            txtPrice.Enter += (s, e) => { if (txtPrice.Text == "Enter Price...") { txtPrice.Text = ""; txtPrice.ForeColor = Color.Black; } };
            txtPrice.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtPrice.Text)) ResetPriceField(); };
            pnlSidebar.Controls.Add(txtPrice); y += 80;

            CreateSidebarLabel("Stock Quantity", y);
            numQuantity = new NumericUpDown { Location = new Point(20, y + 25), Width = 280, Font = new Font("Segoe UI", 12), Minimum = 1 };
            pnlSidebar.Controls.Add(numQuantity); y += 90;

            Button btnSave = new Button { Text = "ADD TO INVENTORY", Location = new Point(20, y), Size = new Size(280, 50), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Cursor = Cursors.Hand };
            btnSave.Click += (s, e) => ShowConfirmationPopup();
            pnlSidebar.Controls.Add(btnSave);

            pnlPreviewCard = new Panel { Size = new Size(280, 150), Location = new Point(20, y + 70), BackColor = Color.FromArgb(31, 41, 55) };
            lblEmoji = new Label { Text = "🐾", Font = new Font("Segoe UI", 45), ForeColor = Color.White, Size = new Size(280, 100), TextAlign = ContentAlignment.MiddleCenter };
            lblPreviewTitle = new Label { Text = "SELECT CATEGORY", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(230, 126, 34), Dock = DockStyle.Bottom, Height = 40, TextAlign = ContentAlignment.MiddleCenter };
            pnlPreviewCard.Controls.AddRange(new Control[] { lblEmoji, lblPreviewTitle });
            pnlSidebar.Controls.Add(pnlPreviewCard);

            
            pnlHeader = new Panel { Location = new Point(315, 0), Size = new Size(this.Width - 320, 60), BackColor = Color.FromArgb(23, 31, 42), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            this.Controls.Add(pnlHeader);
            Label lblAppName = new Label { Text = "🐾 PETSHOP PREMIUM SYSTEM", ForeColor = Color.White, Font = new Font("Segoe UI", 15, FontStyle.Bold), Location = new Point(20, 18), AutoSize = true };
            lblClock = new Label { ForeColor = Color.FromArgb(230, 126, 34), Font = new Font("Segoe UI Semibold", 11), AutoSize = true };
            pnlHeader.Controls.AddRange(new Control[] { lblAppName, lblClock });

            
            pnlMain = new Panel { Location = new Point(320, 60), Size = new Size(this.Width - 320, this.Height - 170), BackColor = Color.FromArgb(240, 242, 245), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            this.Controls.Add(pnlMain);

            flowCardGallery = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 190, AutoScroll = true, Padding = new Padding(20, 5, 10, 5) };
            pnlLiveStatus = new Panel { Dock = DockStyle.Top, Height = 150, BackColor = Color.White, Margin = new Padding(20, 0, 20, 5) };
            picLivePreview = new PictureBox { Size = new Size(130, 110), Location = new Point(20, 20), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.FromArgb(245, 245, 245), BorderStyle = BorderStyle.FixedSingle };
            lblStockStatus = new Label { Text = "Stock Status: --", Font = new Font("Segoe UI Bold", 11), Location = new Point(170, 25), AutoSize = true };
            lblRating = new Label { Text = "Rating: ⭐⭐⭐⭐⭐", Font = new Font("Segoe UI", 10), Location = new Point(170, 50), AutoSize = true, ForeColor = Color.DarkOrange };
            lblReviewText = new Label { Text = "Select an item to see customer reviews...", Font = new Font("Segoe UI", 10, FontStyle.Italic), Location = new Point(170, 75), Size = new Size(600, 50), ForeColor = Color.Gray };
            pnlLiveStatus.Controls.AddRange(new Control[] { picLivePreview, lblStockStatus, lblRating, lblReviewText });

            Panel pnlMonitorContainer = new Panel { Dock = DockStyle.Fill };
            dgvInventory = new DataGridView { Dock = DockStyle.Left, Width = 600, BackgroundColor = Color.White, RowHeadersVisible = false, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvInventory.Columns.Add("id", "ID");
            dgvInventory.Columns.Add("Category", "Category");
            dgvInventory.Columns.Add("Breed", "Breed");
            dgvInventory.Columns.Add("Price", "Price ($)");
            dgvInventory.Columns.Add("Quantity", "Quantity");
            dgvInventory.SelectionChanged += (s, e) => LoadRatingFromSelection();

            Button btnRemoveSelected = new Button { Text = "🗑️ REMOVE SELECTED\nFROM INVENTORY", Size = new Size(180, 60), Location = new Point(620, 20), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 9), Cursor = Cursors.Hand };
            btnRemoveSelected.Click += (s, e) => ActionRemoveFromInventory();

            Button btnBackDashboard = new Button { Text = "🔙 BACK TO ADMIN\nDASHBOARD", Size = new Size(180, 60), Location = new Point(620, 90), BackColor = Color.FromArgb(52, 73, 94), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 9), Cursor = Cursors.Hand };
            btnBackDashboard.Click += (s, e) => ActionBackToDashboard();

            pnlMonitorContainer.Controls.AddRange(new Control[] { btnRemoveSelected, btnBackDashboard, dgvInventory });
            pnlMain.Controls.AddRange(new Control[] { pnlMonitorContainer, new Label { Text = "📊 LIVE TABLE MONITOR", Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI Bold", 9), Padding = new Padding(20, 5, 0, 0) }, pnlLiveStatus, flowCardGallery, new Label { Text = "🖼️ SELECT FROM CATALOG", Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI Bold", 9), Padding = new Padding(20, 5, 0, 0) } });

            
            pnlFooter = new Panel
            {
                Location = new Point(315, this.Height - 100), 
                Size = new Size(this.Width - 320, 100),       
                BackColor = Color.FromArgb(15, 23, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, 
                Padding = new Padding(30, 0, 30, 0)
            };
            this.Controls.Add(pnlFooter);

            
            Label lblFooterText = new Label
            {
                Text = "🛡️ SECURE PETSHOP MANAGEMENT SYSTEM v2.0 | Powered by PetShop Premium",
                ForeColor = Color.FromArgb(150, 150, 150),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true
            };

            Label lblCopyright = new Label
            {
                Text = "© 2026 Admin Mama's Edition",
                ForeColor = Color.FromArgb(230, 126, 34),
                Font = new Font("Segoe UI Semibold", 10),
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = true
            };

            pnlFooter.Controls.AddRange(new Control[] { lblFooterText, lblCopyright });
        }

        
        private async void LoadInventoryFromDB(string category)
        {
            dgvInventory.Rows.Clear();
            if (category == "-- Select --") return;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand("SELECT id, Category, Breed, Price, Quantity FROM Inventory WHERE Category = @c", conn);
                    cmd.Parameters.AddWithValue("@c", category);
                    SqlDataReader r = await cmd.ExecuteReaderAsync();
                    while (await r.ReadAsync()) dgvInventory.Rows.Add(r[0], r[1], r[2], r[3], r[4]);
                }
            }
            catch (Exception ex) { MessageBox.Show("Load Error: " + ex.Message); }
        }

        private async Task AddToDatabase(string breed, string cat, decimal price, int qty)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(@"
                        IF EXISTS (SELECT 1 FROM Inventory WHERE Breed=@b AND Category=@c) 
                        UPDATE Inventory SET Quantity=Quantity+@q, Price=@p WHERE Breed=@b AND Category=@c 
                        ELSE INSERT INTO Inventory (Category, Breed, Price, Quantity) VALUES (@c, @b, @p, @q)", conn);
                    cmd.Parameters.AddWithValue("@b", breed);
                    cmd.Parameters.AddWithValue("@c", cat);
                    cmd.Parameters.AddWithValue("@p", price);
                    cmd.Parameters.AddWithValue("@q", qty);
                    await cmd.ExecuteNonQueryAsync();
                }
                LoadInventoryFromDB(cat);
            }
            catch (Exception ex) { MessageBox.Show("Save Error: " + ex.Message); }
        }

        
        private void UpdateUI()
        {
            if (cmbCategory.SelectedIndex <= 0) return;
            string cat = cmbCategory.Text;
            LoadInventoryFromDB(cat);
            lblEmoji.Text = cat == "Cat" ? "🐱" : cat == "Dog" ? "🐶" : cat == "Food" ? "🦴" : cat == "Medicine" ? "💊" : cat == "Accessories" ? "🎾" : "🐾";
            lblPreviewTitle.Text = "MANAGING: " + cat.ToUpper();

            cmbSubItem.Items.Clear(); cmbSubItem.Items.Add("-- Select --");
            if (subItems.ContainsKey(cat)) cmbSubItem.Items.AddRange(subItems[cat]);
            cmbSubItem.SelectedIndex = 0;

            flowCardGallery.Controls.Clear();
            foreach (string item in subItems[cat])
            {
                Panel card = new Panel { Size = new Size(130, 160), BackColor = Color.White, Margin = new Padding(0, 0, 15, 0), Name = item, Cursor = Cursors.Hand };
                PictureBox pb = new PictureBox { Size = new Size(110, 90), Location = new Point(10, 10), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.FromArgb(245, 245, 245) };
                try { pb.Image = (Image)Properties.Resources.ResourceManager.GetObject(item.Replace(" ", "")); } catch { }
                Label l = new Label { Text = item, Dock = DockStyle.Bottom, Height = 40, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI Bold", 8) };
                card.Controls.AddRange(new Control[] { pb, l });

                Action sel = () => {
                    cmbSubItem.Text = item;
                    SyncGallerySelection();
                    LoadProductInsight();
                    SelectRowInTable(item);
                };
                card.Click += (s, e) => sel(); pb.Click += (s, e) => sel(); l.Click += (s, e) => sel();
                flowCardGallery.Controls.Add(card);
            }
        }

        private async void LoadProductInsight()
        {
            try { picLivePreview.Image = (Image)Properties.Resources.ResourceManager.GetObject(cmbSubItem.Text.Replace(" ", "")); } catch { }
            if (cmbSubItem.SelectedIndex > 0)
            {
                string breedName = cmbSubItem.Text;
                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        await conn.OpenAsync();
                        SqlCommand cmd = new SqlCommand("SELECT Quantity FROM Inventory WHERE Breed=@b", conn);
                        cmd.Parameters.AddWithValue("@b", breedName);
                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                        {
                            int currentStock = Convert.ToInt32(result);
                            lblStockStatus.Text = $"Database Stock: {currentStock} Units";
                            lblStockStatus.ForeColor = currentStock > 5 ? Color.DarkGreen : Color.Red;
                            lblReviewText.Text = $"Mama, current market-e {breedName}-er demand khub beshi!";
                        }
                        else
                        {
                            lblStockStatus.Text = "Stock Status: Not Found";
                            lblStockStatus.ForeColor = Color.Gray;
                        }
                    }
                }
                catch { }
            }
        }

        private void SelectRowInTable(string breedName)
        {
            dgvInventory.ClearSelection();
            foreach (DataGridViewRow row in dgvInventory.Rows)
            {
                if (row.Cells[2].Value != null && row.Cells[2].Value.ToString() == breedName)
                {
                    row.Selected = true;
                    dgvInventory.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        
        private void StartClock() { timer = new Timer { Interval = 1000 }; timer.Tick += (s, e) => { lblClock.Text = DateTime.Now.ToString("dddd, dd MMM yyyy | hh:mm:ss tt"); lblClock.Left = pnlHeader.Width - lblClock.Width - 20; lblClock.Top = 20; }; timer.Start(); }
        private void SyncGallerySelection() { foreach (Control c in flowCardGallery.Controls) if (c is Panel p) p.BackColor = (p.Name == cmbSubItem.Text) ? Color.FromArgb(230, 126, 34) : Color.White; }
        private void ResetPriceField() { txtPrice.Text = "Enter Price..."; txtPrice.ForeColor = Color.Gray; }
        private void CreateSidebarLabel(string t, int y) { pnlSidebar.Controls.Add(new Label { Text = t, Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI Semibold", 9), ForeColor = Color.FromArgb(224, 224, 224) }); }
        private ComboBox CreateSidebarCombo(int y) { ComboBox c = new ComboBox { Location = new Point(20, y), Width = 280, Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList }; pnlSidebar.Controls.Add(c); return c; }

        private async void LoadRatingFromSelection()
        {
            if (dgvInventory.SelectedRows.Count > 0)
            {
                string breedName = dgvInventory.SelectedRows[0].Cells[2].Value.ToString();
                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        await conn.OpenAsync();
                        SqlCommand cmd = new SqlCommand("SELECT Quantity FROM Inventory WHERE Breed=@b", conn);
                        cmd.Parameters.AddWithValue("@b", breedName);
                        object q = await cmd.ExecuteScalarAsync();
                        if (q != null) lblStockStatus.Text = $"Database Stock: {q} Units";
                        try { picLivePreview.Image = (Image)Properties.Resources.ResourceManager.GetObject(breedName.Replace(" ", "")); } catch { }
                    }
                }
                catch { }
            }
        }

        private async void ActionRemoveFromInventory()
        {
            if (dgvInventory.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvInventory.SelectedRows[0].Cells[0].Value);
                if (MessageBox.Show($"Mama, ID: {id} ki delete korbi?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connString))
                        {
                            await conn.OpenAsync();
                            new SqlCommand($"DELETE FROM Inventory WHERE id={id}", conn).ExecuteNonQuery();
                        }
                        LoadInventoryFromDB(cmbCategory.Text);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
        }

        private void ShowConfirmationPopup()
        {
            decimal p;
            if (cmbCategory.SelectedIndex <= 0 || cmbSubItem.SelectedIndex <= 0 || !decimal.TryParse(txtPrice.Text, out p)) return;

            Form pop = new Form { Size = new Size(400, 500), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.None, BackColor = Color.White };
            Panel h = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(23, 31, 42) };
            h.Controls.Add(new Label { Text = "CONFIRM ADDITION", ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });

            PictureBox pic = new PictureBox { Size = new Size(180, 150), Location = new Point(110, 80), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.FromArgb(245, 245, 245) };
            try { pic.Image = (Image)Properties.Resources.ResourceManager.GetObject(cmbSubItem.Text.Replace(" ", "")); } catch { }

            Label details = new Label { Text = $"Item: {cmbSubItem.Text}\nPrice: ${txtPrice.Text}\nQty: {numQuantity.Value}", Font = new Font("Segoe UI", 12), Location = new Point(20, 250), Size = new Size(360, 100), TextAlign = ContentAlignment.MiddleCenter };

            Button btnOk = new Button { Text = "CONFIRM", Size = new Size(150, 45), Location = new Point(220, 420), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnOk.Click += async (s, e) => {
                await AddToDatabase(cmbSubItem.Text, cmbCategory.Text, p, (int)numQuantity.Value);
                pop.Close();
            };

            Button btnNo = new Button { Text = "CANCEL", Size = new Size(150, 45), Location = new Point(30, 420), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnNo.Click += (s, e) => pop.Close();

            pop.Controls.AddRange(new Control[] { h, pic, details, btnOk, btnNo });
            pop.ShowDialog();
        }

        private void ActionBackToDashboard()
        {
           
            AdminDashboard dashboard = new AdminDashboard();

           
            dashboard.Show();

           
            this.Close();
        }
    }
}