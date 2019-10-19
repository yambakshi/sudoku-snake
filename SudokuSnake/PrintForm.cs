using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SudokuSnake
{
    public partial class PrintForm : Form
    {
        #region Data
        public Bitmap toPrint;
        private PictureBox Confirm;
        private RadioButton BW, Colors;
        private TextBox width, height, dedication;
        #endregion

        #region Ctor
        public PrintForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(300, 300);
            this.BackColor = Color.Black;
            this.Icon = new Icon(Application.StartupPath + "\\icon.ico");
        }

        private void PrintForm_Load(object sender, EventArgs e)
        {
            width = new TextBox();
            width.Name = "Image Width";
            width.Text = "Image Width";
            width.TextAlign = HorizontalAlignment.Center;
            width.ForeColor = Color.Gray;
            width.Location = new Point(this.Width / 2 - width.Width / 2, 20);
            width.TextChanged += new EventHandler(width_TextChanged);
            width.GotFocus += new EventHandler(width_GotFocus);
            width.LostFocus += new EventHandler(width_LostFocus);
            width.Click += new EventHandler(width_Click);
            width.KeyDown += new KeyEventHandler(width_KeyDown);
            this.Controls.Add(width);

            height = new TextBox();
            height.Name = "Image Height";
            height.Enabled = false;
            height.Text = "Image Height";
            height.Location = new Point(this.Width / 2 - height.Width / 2, width.Location.Y + width.Height + 5);
            height.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(height);

            BW = new RadioButton();
            BW.ForeColor = Color.White;
            BW.Text = "Black and White";
            BW.Location = new Point(this.Width / 2 - BW.Width / 2, height.Location.Y + height.Height + 5);
            BW.KeyDown += new KeyEventHandler(_KeyDown);
            this.Controls.Add(BW);

            Colors = new RadioButton();
            Colors.Text = "Colors";
            Colors.ForeColor = Color.White;
            Colors.Checked = true;
            Colors.Location = new Point(this.Width / 2 - Colors.Width / 2, BW.Location.Y + BW.Height + 5);
            Colors.KeyDown += new KeyEventHandler(_KeyDown);
            this.Controls.Add(Colors);

            dedication = new TextBox();
            dedication.Name = "Dedication";
            dedication.Text = "Dedication";
            dedication.ForeColor = Color.Gray;
            dedication.MaxLength = 40;
            dedication.Width = 200;
            dedication.TextAlign = HorizontalAlignment.Center;
            dedication.Location = new Point(this.Width / 2 - dedication.Width / 2, Colors.Location.Y + Colors.Height + 5);
            dedication.GotFocus += new EventHandler(width_GotFocus);
            dedication.LostFocus += new EventHandler(width_LostFocus);
            dedication.Click += new EventHandler(width_Click);
            dedication.TextChanged += new EventHandler(width_TextChanged);
            dedication.KeyDown += new KeyEventHandler(dedication_KeyDown);
            this.Controls.Add(dedication);

            Confirm = new PictureBox();
            Bitmap bmp = new Bitmap(Application.StartupPath + "\\print.png");
            float h = 30;
            float bmpH = bmp.Height;
            float ratio = bmpH / bmp.Width;
            float w = h / ratio;
            Confirm.Size = new Size((int)w, (int)h);
            Confirm.Image = bmp;
            Confirm.SizeMode = PictureBoxSizeMode.StretchImage;
            Confirm.Location = new Point(this.Width / 2 - Confirm.Width / 2, 210);
            Confirm.MouseMove += new MouseEventHandler(S.MouseMove);
            Confirm.Click += new EventHandler(Confirm_Click);
            Confirm.MouseUp += new MouseEventHandler(Confirm_MouseUp);
            Confirm.MouseDown += new MouseEventHandler(Confirm_MouseDown);
            this.Controls.Add(Confirm);

            this.Click += new EventHandler(PrintForm_Click);
        }
        #endregion

        #region Events
        void Confirm_MouseDown(object sender, MouseEventArgs e)
        {
            float h = 28;
            float bmpW = Confirm.Image.Height;
            float ratio = bmpW / Confirm.Image.Width;
            float w = h / ratio;
            Confirm.Size = new Size((int)w, (int)h);
            Confirm.Location = new Point(this.Width / 2 - Confirm.Width / 2, 210);
        }
        void Confirm_MouseUp(object sender, MouseEventArgs e)
        {
            float h = 30;
            float bmpW = Confirm.Image.Height;
            float ratio = bmpW / Confirm.Image.Width;
            float w = h / ratio;
            Confirm.Size = new Size((int)w, (int)h);
            Confirm.Location = new Point(this.Width / 2 - Confirm.Width / 2, 210);
        }
        void _KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { Confirm_Click(Confirm, new EventArgs()); e.SuppressKeyPress = true; }
        }
        void dedication_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { Confirm_Click(Confirm, new EventArgs()); e.SuppressKeyPress = true; }
        } 
        void PrintForm_Click(object sender, EventArgs e)
        {
            width.Text = width.Name;
            height.Text = height.Name;
            dedication.Text = dedication.Name;

            width.ForeColor = Color.Gray;
            width.SelectAll();

            dedication.ForeColor = Color.Gray;
            dedication.SelectAll();
        } 
        #endregion

        #region TextBox Events
        private void width_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { Confirm_Click(Confirm, new EventArgs()); e.SuppressKeyPress = true; }
            else if (
                (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) &&
                (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9) &&
                e.KeyCode != Keys.Back &&
                e.KeyCode != Keys.Delete &&
                e.KeyCode != Keys.Left && e.KeyCode != Keys.Right)
                e.SuppressKeyPress = true;
        }

        private void width_TextChanged(object sender, EventArgs e)
        {
            if(((TextBox)sender).Name=="Image Width")
                height.Text = width.Text;
            if (((TextBox)sender).Focused)
                ((TextBox)sender).ForeColor = Color.Black;
        }

        private void width_LostFocus(object sender, EventArgs e)
        {
            ((TextBox)sender).LostFocus += new EventHandler(width_LostFocus);
            ((TextBox)sender).ForeColor = Color.Gray;
            ((TextBox)sender).Text = ((TextBox)sender).Name;
        }

        private void width_GotFocus(object sender, EventArgs e)
        {
            ((TextBox)sender).LostFocus -= new EventHandler(width_LostFocus);
        }

        private void width_Click(object sender, EventArgs e)
        {
            ((TextBox)sender).Clear();
        }  
        #endregion

        #region Print
        void Confirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.Parse(width.Text) < 801)
                {
                    S.yes.Play();
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += new PrintPageEventHandler(Print_Page);

                    PrintPreviewDialog dlg = new PrintPreviewDialog();
                    dlg.Document = pd;
                    dlg.ShowDialog();
                }
                else
                {
                    S.mis.Play();
                    width.Text = "Too Big!";
                    width.ForeColor = Color.Red;
                }
            }
            catch
            {
                S.mis.Play();
                width.Text = "Enter Width!";
                width.ForeColor = Color.Red;
            }
        }
        void Print_Page(object sender, PrintPageEventArgs e)
        {
            Bitmap toPrintBW;
            if (dedication.ForeColor != Color.Gray)
                e.Graphics.DrawString(
                            dedication.Text,
                            S.GetFontForTextBoxHeight(40, dedication.Font),
                            new SolidBrush(Color.Black),
                            new PointF(new PrintDocument().DefaultPageSettings.PaperSize.Width / 2 - e.Graphics.MeasureString(dedication.Text, S.GetFontForTextBoxHeight(40, dedication.Font)).Width / 2, 50));
            if (BW.Checked)
            {
                toPrintBW = BlackAndWhite(toPrint, new Rectangle(0, 0, toPrint.Width, toPrint.Height));
                e.Graphics.DrawImage(
                    toPrintBW,
                    new PrintDocument().DefaultPageSettings.PaperSize.Width / 2 - int.Parse(width.Text) / 2,
                    100,
                    int.Parse(width.Text),
                    int.Parse(height.Text));
            }
            else
            {
                toPrintBW = BlackAndWhite(toPrint, new Rectangle(0, 0, toPrint.Width, toPrint.Height));
                e.Graphics.DrawImage(
                    toPrint,
                    new PrintDocument().DefaultPageSettings.PaperSize.Width / 2 - int.Parse(width.Text) / 2,
                    100,
                    int.Parse(width.Text),
                    int.Parse(height.Text));
            }
        } 
        #endregion

        #region Black & White
        private static Bitmap BlackAndWhite(Bitmap image, Rectangle rectangle)
        {
            Bitmap blackAndWhite = new System.Drawing.Bitmap(image.Width, image.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = System.Drawing.Graphics.FromImage(blackAndWhite))
                graphics.DrawImage(image, new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            // for every pixel in the rectangle region
            for (Int32 xx = rectangle.X; xx < rectangle.X + rectangle.Width && xx < image.Width; xx++)
            {
                for (Int32 yy = rectangle.Y; yy < rectangle.Y + rectangle.Height && yy < image.Height; yy++)
                {
                    // average the red, green and blue of the pixel to get a gray value
                    Color pixel = blackAndWhite.GetPixel(xx, yy);
                    Int32 avg = (pixel.R + pixel.G + pixel.B) / 3;

                    blackAndWhite.SetPixel(xx, yy, Color.FromArgb(255, avg, avg, avg));
                }
            }

            return blackAndWhite;
        } 
        #endregion
    }
}
