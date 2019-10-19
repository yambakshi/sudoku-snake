using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SudokuSnake
{
    public partial class MyTrackbar : UserControl
    {
        #region Data
        public PictureBox bar, backbar, caret;
        private bool drag, dragbar;
        public new event EventHandler Scroll;
        public int BarHeight = 20;
        #endregion

        #region Ctor
        public MyTrackbar()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
            this.Size = new Size(100, 30);
            caret = new PictureBox();
            caret.BackColor = Color.Blue;
            caret.Size = new Size(20, 20);
            caret.Location = new Point(this.Width - caret.Width, this.Height / 2 - caret.Height / 2);

            bar = new PictureBox();
            bar.BackColor = Color.Red;

            backbar = new PictureBox();
            backbar.BackColor = Color.Green;
        }

        private void MyTrackbar_Load(object sender, EventArgs e)
        {
            caret.Location = new Point(this.Width - caret.Width, this.Height / 2 - caret.Height / 2);
            caret.MouseDown += new MouseEventHandler(caret_MouseDown);
            caret.MouseUp += new MouseEventHandler(caret_MouseUp);
            caret.MouseMove += new MouseEventHandler(caret_MouseMove);
            this.Controls.Add(caret);
            
            bar.Size = new Size(this.Width, BarHeight);
            bar.Location = new Point(0, this.Height / 2 - bar.Height / 2);
            bar.MouseMove += new MouseEventHandler(bar_MouseMove);
            bar.MouseDown += new MouseEventHandler(bar_MouseDown);
            bar.MouseUp += new MouseEventHandler(bar_MouseUp);
            this.Controls.Add(bar);

            backbar.Size = new Size(this.Width, BarHeight);
            backbar.Location = new Point(0, this.Height / 2 - bar.Height / 2);
            backbar.MouseDown += new MouseEventHandler(backbar_MouseDown);
            backbar.MouseUp += new MouseEventHandler(backbar_MouseUp);
            backbar.MouseMove += new MouseEventHandler(backbar_MouseMove);
            this.Controls.Add(backbar);
        }
        #endregion

        #region Backbar Events
        void backbar_MouseMove(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Cursor = Cursors.Hand;
            if (e.X >= 0 && e.X <= this.Width)
            {
                if (dragbar)
                {
                    if (caret.Location.X > 0 && caret.Location.X < this.Width - caret.Width)
                    {
                        caret.Location = new Point(e.X - caret.Width / 2, this.Height / 2 - caret.Height / 2);
                        bar.Size = new Size(e.X, BarHeight);
                        Scroll(caret, new EventArgs());
                    }
                    else
                    {
                        if (caret.Location.X <= 0)
                        {
                            if (e.X > caret.Width / 2)
                            {
                                caret.Location = new Point((caret.Location.X + e.X) - caret.Width / 2, this.Height / 2 - caret.Height / 2);
                                Scroll(caret, new EventArgs());
                            }
                        }
                        else
                        {
                            if (e.X < backbar.Width - caret.Width / 2)
                            {
                                caret.Location = new Point(e.X - caret.Width / 2, this.Height / 2 - caret.Height / 2);
                                Scroll(caret, new EventArgs());
                            }
                        }
                    }
                }
            }
            else
            {
                if (e.X <= 0)
                {
                    caret.Location = new Point(0, this.Height / 2 - caret.Height / 2);
                    Scroll(caret, new EventArgs());
                }
                else
                {
                    caret.Location = new Point(this.Width - caret.Width, this.Height / 2 - caret.Height / 2);
                    Scroll(caret, new EventArgs());
                }
            }
        }

        void backbar_MouseUp(object sender, MouseEventArgs e)
        {
            dragbar = false;
        }

        void backbar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.X > caret.Width / 2 && e.X < this.Width - caret.Width / 2)
                    caret.Location = new Point(e.X - caret.Width / 2, this.Height / 2 - caret.Height / 2);

                else
                {
                    if (e.X >= this.Width - caret.Width / 2)
                        caret.Location = new Point(this.Width - caret.Width, this.Height / 2 - caret.Height / 2);
                    else
                        caret.Location = new Point(0, this.Height / 2 - caret.Height / 2);
                }
                bar.Size = new Size(caret.Location.X, BarHeight);
                Scroll(caret, new EventArgs());
                dragbar = true;
            }
        }
        #endregion

        #region Bar Events
        void bar_MouseMove(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Cursor = Cursors.Hand;
            if (e.X >= 0 && e.X <= this.Width)
            {
                if (dragbar)
                {
                    if (caret.Location.X > 0 && caret.Location.X < this.Width - caret.Width)
                    {
                        caret.Location = new Point(e.X - caret.Width / 2, this.Height / 2 - caret.Height / 2);
                        bar.Size = new Size(e.X, BarHeight);
                        Scroll(caret, new EventArgs());
                    }
                    else
                    {
                        if (caret.Location.X <= 0)
                        {
                            if (e.X > caret.Width / 2)
                            {
                                caret.Location = new Point((caret.Location.X + e.X) - caret.Width / 2, this.Height / 2 - caret.Height / 2);
                                Scroll(caret, new EventArgs());
                            }
                        }
                        else
                        {
                            if (e.X < bar.Width - caret.Width / 2)
                            {
                                caret.Location = new Point(e.X - caret.Width / 2, this.Height / 2 - caret.Height / 2);
                                Scroll(caret, new EventArgs());
                            }
                        }

                    }
                }
            }
            else
            {
                if (e.X <= 0)
                {
                    caret.Location = new Point(0, this.Height / 2 - caret.Height / 2);
                    Scroll(caret, new EventArgs());
                }
                else
                {
                    caret.Location = new Point(this.Width - caret.Width, this.Height / 2 - caret.Height / 2);
                    Scroll(caret, new EventArgs());
                }
            }
        }

        void bar_MouseUp(object sender, MouseEventArgs e)
        {
            dragbar = false;
        }

        void bar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.X > caret.Width / 2 && e.X < this.Width - caret.Width / 2)
                    caret.Location = new Point(e.X - caret.Width / 2, this.Height / 2 - caret.Height / 2);
                else
                {
                    if (e.X >= this.Width - caret.Width / 2)
                        caret.Location = new Point(this.Width - caret.Width, this.Height / 2 - caret.Height / 2);
                    else
                        caret.Location = new Point(0, this.Height / 2 - caret.Height / 2);
                }
                bar.Size = new Size(e.X, BarHeight);
                Scroll(caret, new EventArgs());
                dragbar = true;
            }
        } 
        #endregion

        #region Caret Events
        void caret_MouseUp(object sender, MouseEventArgs e)
        {
            drag = false;
        }

        void caret_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                if (caret.Location.X + e.X > caret.Width / 2 && caret.Location.X + e.X < this.Width - caret.Width / 2)
                    caret.Location = new Point((caret.Location.X + e.X) - caret.Width / 2, this.Height / 2 - caret.Height / 2);
                else
                {
                    if (caret.Location.X + e.X >= this.Width - caret.Width / 2)
                        caret.Location = new Point(this.Width - caret.Width, this.Height / 2 - caret.Height / 2);
                    else
                        caret.Location = new Point(0, this.Height / 2 - caret.Height / 2);
                }
                drag = true;
            }
        }

        void caret_MouseMove(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Cursor = Cursors.Hand;
            if (drag)
            {
                if (caret.Location.X > 0 && caret.Location.X < this.Width - caret.Width)
                {
                    caret.Location = new Point((caret.Location.X + e.X) - caret.Width / 2, this.Height / 2 - caret.Height / 2);
                    bar.Size = new Size(caret.Location.X, BarHeight);
                    Scroll(caret, new EventArgs());
                }
                else
                {
                    if (caret.Location.X <= 0)
                    {
                        if (e.X > caret.Width / 2)
                            caret.Location = new Point((caret.Location.X + e.X) - caret.Width / 2, this.Height / 2 - caret.Height / 2);
                        else
                            caret.Location = new Point(0, this.Height / 2 - caret.Height / 2);
                        Scroll(caret, new EventArgs());
                    }
                    else
                    {
                        if (e.X < caret.Width / 2)
                            caret.Location = new Point((caret.Location.X + e.X) - caret.Width / 2, this.Height / 2 - caret.Height / 2);
                        else
                            caret.Location = new Point(this.Width - caret.Width, this.Height / 2 - caret.Height / 2);
                        Scroll(caret, new EventArgs());
                    }
                }
            }
        }
        #endregion

        #region Properties
        public float SoundScale
        {
            get { return 1f / (float)(this.Width - caret.Width); }
        }
        public int CaretLocationX
        {
            set 
            { 
                caret.Location = new Point(value, this.Height / 2 - caret.Height / 2);
                bar.Size = new Size(caret.Location.X, BarHeight);
                try
                {
                    Scroll(caret, new EventArgs());
                }
                catch { }
            }
            get { return caret.Location.X; }
        }
        #endregion
    }
}
