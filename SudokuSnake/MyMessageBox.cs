using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SudokuSnake
{
    public partial class MyMessageBox : Form
    {
        public MyMessageBox(Bitmap b,String name)
        {
            InitializeComponent();
            this.Size = new Size(400, 300);
            this.BackgroundImage = b;
            this.Text = name;
            this.BackgroundImageLayout = ImageLayout.Stretch;            
            this.Icon = new Icon(Application.StartupPath + "\\icon.ico");
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyDown += new KeyEventHandler(MyMessageBox_KeyDown);
            this.Click += new EventHandler(MyMessageBox_Click);
        }

        void MyMessageBox_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        void MyMessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            this.Hide();
        }
    }
}
