using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PetShopApp
{
    // 1. MAIN FORM CLASS
    public partial class ReviewPageCheck : Form
    {
        public ReviewPageCheck()
        {
            // --- Form Settings ---
            this.Text = "PetCare Review Hub";
            this.Size = new Size(460, 570);
            this.BackColor = Color.White;

            // NONE er jaygay FixedSingle korlam jate normal cross button ashe
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Jate form ta screen er majhkhane thake
            this.StartPosition = FormStartPosition.CenterScreen;

            // Jate maximize button e click kore design baje na hoye jay
            this.MaximizeBox = false;

            CreatePetShopUI();
        }

        private void CreatePetShopUI()
        {
            this.Controls.Clear();

            // --- 1. HEADER SECTION ---
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 150, BackColor = Color.FromArgb(23, 31, 42) };


            Label lblLogo = new Label
            {
                Text = "🐾",
                Font = new Font("Segoe UI", 45),
                ForeColor = Color.FromArgb(230, 126, 34),
                Size = new Size(460, 80),
                Location = new Point(0, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblTitle = new Label
            {
                Text = "PET SHOP REVIEWS",
                Font = new Font("Segoe UI Black", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(460, 35),
                Location = new Point(0, 105),
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlHeader.Controls.Add(lblLogo);
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            // --- 2. SELECTION BUTTONS ---
            Button btnWrite = CreatePetButton("WRITE A REVIEW", "Share your valuable feedback with us!", 190, Color.FromArgb(230, 126, 34), "📝");
            btnWrite.Click += HandleWriteReview;

            Button btnView = CreatePetButton("BROWSE REVIEWS", "See what other pet parents are saying.", 315, Color.FromArgb(52, 152, 219), "🔍");
            btnView.Click += HandleViewReviews;

            this.Controls.Add(btnWrite);
            this.Controls.Add(btnView);

            // --- 3. PAW PATTERN DESIGN ---
            Panel pnlPawPattern = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = Color.Transparent };

            for (int i = 0; i < 7; i++)
            {
                Label lblSmallPaw = new Label
                {
                    Text = "🐾",
                    Font = new Font("Segoe UI", 18),
                    ForeColor = (i % 2 == 0) ? Color.FromArgb(230, 126, 34) : Color.FromArgb(52, 152, 219),
                    Size = new Size(60, 40),
                    Location = new Point(20 + (i * 60), 5),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                pnlPawPattern.Controls.Add(lblSmallPaw);
            }
            this.Controls.Add(pnlPawPattern);

            // --- 4. FOOTER ---
            Panel pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.FromArgb(23, 31, 42) };
            Label lblFooter = new Label
            {
                Text = "Premium Pet Management © 2026",
                ForeColor = Color.DarkGray,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlFooter.Controls.Add(lblFooter);
            this.Controls.Add(pnlFooter);
        }

        private void HandleWriteReview(object sender, EventArgs e)
        {


            ReviewLogInPage revLogin = new ReviewLogInPage();
            revLogin.Show();
        }

        private void HandleViewReviews(object sender, EventArgs e)
        {
         
            ShowReview reviewForm = new ShowReview();

            // Form-ta show kora holo
            reviewForm.Show();
        }

        private Button CreatePetButton(string title, string desc, int y, Color themeColor, string icon)
        {
            Button btn = new Button
            {
                Size = new Size(360, 100),
                Location = new Point(50, y),
                BackColor = Color.FromArgb(245, 245, 250),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.Gainsboro;

            btn.Paint += (s, e) => {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // FIX: Ekhon extension method ta thik moto call hobe
                g.FillRoundedRectangle(new SolidBrush(themeColor), 0, 0, 10, btn.Height, 4);

                g.DrawString(icon, new Font("Segoe UI", 20), new SolidBrush(themeColor), 25, 30);
                g.DrawString(title, new Font("Segoe UI Black", 12), Brushes.Black, 75, 25);
                g.DrawString(desc, new Font("Segoe UI Semibold", 8), Brushes.DimGray, 75, 55);
            };

            btn.MouseEnter += (s, e) => {
                btn.BackColor = Color.White;
                btn.FlatAppearance.BorderColor = themeColor;
            };
            btn.MouseLeave += (s, e) => {
                btn.BackColor = Color.FromArgb(245, 245, 250);
                btn.FlatAppearance.BorderColor = Color.Gainsboro;
            };

            return btn;
        }

        private void ReviewPageCheck_Load(object sender, EventArgs e)
        {

        }
    }


    
}