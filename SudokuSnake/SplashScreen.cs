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
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackgroundImage = new Bitmap(Application.StartupPath + "\\splash.png");
            this.Size = this.BackgroundImage.Size;
            this.Icon = new Icon(Application.StartupPath + "\\icon.ico");
        }
    }
}
