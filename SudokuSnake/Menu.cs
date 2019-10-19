using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;
using Timer = System.Windows.Forms.Timer;
using MediaPlayer = System.Windows.Media.MediaPlayer;

namespace SudokuSnake
{
    public delegate void dlgScroll(int caretX);
    public partial class Menu : Form
    {
        #region Data
        public MyTrackbar MenuTb;
        public static dlgScroll MenuScroll;

        private EventHandler GenDel, SolveDel;
        private Timer timer;
        private Bitmap sudoku1, sudoku2;
        private Panel panel;
        private PictureBox 
            t,            
            leftA,
            rightA,
            tsolve,
            leftAS,
            rightAS;

        private int imageSlide = 3, solverImageSlider = 3;
        private PictureBox b, bsolve;
        private PictureBox easy, medium, hard;
        private Bitmap
            easyB, easyBfocus,
            mediumB, mediumBfocus,
            hardB, hardBfocus;
        private PictureBox generate, solve;
        private Bitmap
            one,one_focus,
            two,two_focus,
            three,three_focus,
            four,four_focus,
            five,five_focus;

        private SudokuForm sudokuForm;
        private SolverForm solverForm;
        private int level=1;

        private int x = 0, y = 0, move = 0;
        private bool start = true, bul = true;
        #endregion

        #region Ctor
        public Menu()
        {
            Thread splash = new Thread(new ThreadStart(Splash));
            splash.Start();
            Thread.Sleep(4000);
            InitializeComponent();
            splash.Abort();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 450;
            this.Height = 470;
            this.Icon = new Icon(Application.StartupPath + "\\icon.ico");
            S.m.Open(new Uri(Application.StartupPath + "\\Sounds\\05 Treefingers.wav"));
            S.m.MediaEnded += new EventHandler(S.m_MediaEnded);
            S.m.Play();

            MenuTb = new MyTrackbar();
            MenuTb.Size = S.SliderSize;
            // By the default set the volume to 0
            uint CurrVol = 0;
            // At this point, CurrVol gets assigned the volume
            S.waveOutGetVolume(IntPtr.Zero, out CurrVol);
            // Calculate the volume
            ushort CalcVol = (ushort)(CurrVol & 0x0000ffff);
            // Get the volume on a scale of 1 to 10 (to fit the trackbar)
            MenuTb.CaretLocationX = CalcVol / (ushort.MaxValue / MenuTb.Width);
        }
        private void Splash()
        {
            Application.Run(new SplashScreen());
        } 
        private void Menu_Load(object sender, EventArgs e)
        {
            S.m.Volume = 1.0;
            sudoku1 = new Bitmap(Application.StartupPath + "\\sudoku1.png");
            sudoku2 = new Bitmap(Application.StartupPath + "\\sudoku2.png");

            //SizeSlider Bitmaps
            one = new Bitmap(Application.StartupPath + "\\Size\\1.png");
            one_focus = new Bitmap(Application.StartupPath + "\\Size\\1focus.png");
            two = new Bitmap(Application.StartupPath + "\\Size\\2.png");
            two_focus = new Bitmap(Application.StartupPath + "\\Size\\2focus.png");
            three = new Bitmap(Application.StartupPath + "\\Size\\3.png");
            three_focus = new Bitmap(Application.StartupPath + "\\Size\\3focus.png");
            four = new Bitmap(Application.StartupPath + "\\Size\\4.png");
            four_focus = new Bitmap(Application.StartupPath + "\\Size\\4focus.png");
            five = new Bitmap(Application.StartupPath + "\\Size\\5.png");
            five_focus = new Bitmap(Application.StartupPath + "\\Size\\5focus.png");

            //Levels Bitmaps
            easyB = new Bitmap(Application.StartupPath + "\\Levels\\easy.png");
            easyBfocus = new Bitmap(Application.StartupPath + "\\Levels\\easyFocus.png");
            mediumB = new Bitmap(Application.StartupPath + "\\Levels\\medium.png");
            mediumBfocus = new Bitmap(Application.StartupPath + "\\Levels\\mediumFocus.png");
            hardB = new Bitmap(Application.StartupPath + "\\Levels\\hard.png");
            hardBfocus = new Bitmap(Application.StartupPath + "\\Levels\\hardFocus.png");

            //Timer Settings
            timer = new Timer();
            timer.Interval = 10;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            //Panel Settings
            panel = new Panel();
            panel.Size = new Size(220, this.Height);
            panel.Location = new Point(this.Width / 2 - panel.Width / 2, 0);
            panel.BackColor = Color.Black;
            this.Controls.Add(panel);

            //Fades
            PictureBox FadeLeft = new PictureBox();
            Bitmap bmp = new Bitmap(Application.StartupPath + "\\fadetoleft.png");
            FadeLeft.Image = bmp;
            FadeLeft.SizeMode = PictureBoxSizeMode.StretchImage;
            FadeLeft.Location = new Point(0, 0);
            FadeLeft.BackColor = Color.Transparent;
            FadeLeft.Size = new Size((this.Width - panel.Width) / 2, this.Height);
            this.Controls.Add(FadeLeft);
            PictureBox FadeRight = new PictureBox();
            bmp = new Bitmap(Application.StartupPath + "\\fadetoright.png");
            FadeRight.Image = bmp;
            FadeRight.SizeMode = PictureBoxSizeMode.StretchImage;
            FadeRight.Location = new Point(panel.Location.X + panel.Width, 0);
            FadeRight.BackColor = Color.Transparent;
            FadeRight.Size = new Size((this.Width - panel.Width) / 2, this.Height);
            this.Controls.Add(FadeRight);

            //Generate Headline
            generate = new PictureBox();
            bmp = new Bitmap(Application.StartupPath + "\\SudokuGen.png");
            float h = 70;
            float bmpH = bmp.Height;
            float ratio = bmpH / bmp.Width;
            float w = h / ratio;
            generate.Image = bmp;
            generate.SizeMode = PictureBoxSizeMode.StretchImage;
            generate.Size = new Size((int)w, (int)h);
            generate.Location = new Point(panel.Width / 2 - generate.Width / 2, 10);
            panel.Controls.Add(generate);
            
            //Generate PictureBox
            t = new PictureBox();
            t.Size = new Size(38, 38);
            t.SizeMode = PictureBoxSizeMode.StretchImage;
            t.Location = new Point(panel.Width / 2 - t.Width / 2, generate.Location.Y + generate.Height);
            t.Image = new Bitmap(Application.StartupPath + "\\Size\\"+imageSlide+"focus.png");
            t.PreviewKeyDown += new PreviewKeyDownEventHandler(PreviewKeyDown);
            t.LostFocus+=new EventHandler(LostFocus);
            panel.Controls.Add(t);

            //Left Arrow
            leftA = new PictureBox();
            leftA.Size = new Size(20, 30);
            leftA.Location=new Point(t.Location.X-leftA.Width,t.Location.Y+5);
            leftA.BackColor = Color.Transparent;
            leftA.SizeMode = PictureBoxSizeMode.StretchImage;
            leftA.Image = new Bitmap(Application.StartupPath + "\\Size\\leftA.png");
            leftA.Click += new EventHandler(leftA_Click);
            leftA.MouseMove += new MouseEventHandler(leftA_MouseMove);
            leftA.MouseLeave+=new EventHandler(leftA_MouseLeave);
            panel.Controls.Add(leftA);

            //Right Arrow
            rightA = new PictureBox();
            rightA.Size = new Size(20, 30);
            rightA.Location = new Point(t.Location.X + t.Width, t.Location.Y + 5);
            rightA.BackColor = Color.Transparent;
            rightA.SizeMode = PictureBoxSizeMode.StretchImage;
            rightA.Image = new Bitmap(Application.StartupPath + "\\Size\\rightA.png");
            rightA.Click += new EventHandler(rightA_Click);
            rightA.MouseMove += new MouseEventHandler(rightA_MouseMove);
            rightA.MouseLeave += new EventHandler(rightA_MouseLeave);
            panel.Controls.Add(rightA);

            //Generate Button
            b = new PictureBox();
            bmp = new Bitmap(Application.StartupPath + "\\gen.png");
            GenDel = new EventHandler(b_Click);
            h = 30;
            bmpH = bmp.Height;
            ratio = bmpH / bmp.Width;
            w = h / ratio;
            b.Name = t.Name;
            b.Image = bmp;
            b.SizeMode = PictureBoxSizeMode.StretchImage;
            b.Size = new Size((int)w, (int)h);
            b.Location = new Point(panel.Width / 2 - b.Width / 2, t.Location.Y + t.Height + 10);
            b.Click += GenDel;
            b.MouseDown += new MouseEventHandler(b_MouseDown);
            b.MouseUp += new MouseEventHandler(b_MouseUp);
            b.MouseMove += new MouseEventHandler(S.MouseMove);
            panel.Controls.Add(b);

            //Levels
            easy = new PictureBox();
            medium = new PictureBox();
            hard = new PictureBox();       
     
            easy.Size = new Size(50, 16);
            medium.Size = new Size(85, 16);
            hard.Size = new Size(58, 16);

            easy.Location = new Point(panel.Width / 2 - easy.Width / 2, b.Location.Y + b.Height + 10);
            medium.Location = new Point(panel.Width / 2 - medium.Width / 2, easy.Location.Y + easy.Height+5);
            hard.Location = new Point(panel.Width / 2 - hard.Width / 2, medium.Location.Y + medium.Height+5);

            easy.SizeMode = PictureBoxSizeMode.StretchImage;
            medium.SizeMode = PictureBoxSizeMode.StretchImage;
            hard.SizeMode = PictureBoxSizeMode.StretchImage;

            easy.Image = easyBfocus;
            medium.Image = mediumB;
            hard.Image = hardB;

            easy.Click += new EventHandler(easy_Click);
            medium.Click += new EventHandler(medium_Click);
            hard.Click+=new EventHandler(hard_Click);

            easy.MouseMove += new MouseEventHandler(S.MouseMove);
            medium.MouseMove += new MouseEventHandler(S.MouseMove);
            hard.MouseMove += new MouseEventHandler(S.MouseMove);

            panel.Controls.Add(easy);
            panel.Controls.Add(medium);
            panel.Controls.Add(hard);

            //Solver Headline
            solve = new PictureBox();
            bmp = new Bitmap(Application.StartupPath + "\\SudokuSolver.png");
            h = 70;
            bmpH = bmp.Height;
            ratio = bmpH / bmp.Width;
            w = h / ratio;
            solve.Image = bmp;
            solve.SizeMode = PictureBoxSizeMode.StretchImage;
            solve.Size = new Size((int)w, (int)h);
            solve.Location = new Point(panel.Width / 2 - solve.Width / 2, hard.Location.Y + hard.Height + 10);
            panel.Controls.Add(solve);

            //SolveDel PictureBox
            tsolve = new PictureBox();
            tsolve.Size = new Size(38, 38);
            tsolve.SizeMode = PictureBoxSizeMode.StretchImage;
            tsolve.Location = new Point(panel.Width / 2 - tsolve.Width / 2, solve.Location.Y + solve.Height);
            tsolve.Image = new Bitmap(Application.StartupPath + "\\Size\\" + solverImageSlider + ".png");
            panel.Controls.Add(tsolve);

            //Left Arrow
            leftAS = new PictureBox();
            leftAS.Size = new Size(20, 30);
            leftAS.Location = new Point(tsolve.Location.X - leftAS.Width, tsolve.Location.Y+5);
            leftAS.BackColor = Color.Transparent;
            leftAS.SizeMode = PictureBoxSizeMode.StretchImage;
            leftAS.Image = new Bitmap(Application.StartupPath + "\\Size\\leftA.png");
            leftAS.Click += new EventHandler(leftAS_Click);
            leftAS.MouseMove += new MouseEventHandler(leftA_MouseMove);
            leftAS.MouseLeave += new EventHandler(leftA_MouseLeave);
            panel.Controls.Add(leftAS);

            //Right Arrow
            rightAS = new PictureBox();
            rightAS.Size = new Size(20, 30);
            rightAS.Location = new Point(tsolve.Location.X + tsolve.Width, tsolve.Location.Y+5);
            rightAS.BackColor = Color.Transparent;
            rightAS.SizeMode = PictureBoxSizeMode.StretchImage;
            rightAS.Image = new Bitmap(Application.StartupPath + "\\Size\\rightA.png");
            rightAS.Click += new EventHandler(rightAS_Click);
            rightAS.MouseMove += new MouseEventHandler(rightA_MouseMove);
            rightAS.MouseLeave += new EventHandler(rightA_MouseLeave);
            panel.Controls.Add(rightAS);

            //Solve Button
            bsolve = new PictureBox();
            bmp = new Bitmap(Application.StartupPath + "\\solve.png");
            SolveDel = new EventHandler(bsolve_Click);
            h = 30;
            bmpH = bmp.Height;
            ratio = bmpH / bmp.Width;
            w = h / ratio;
            bsolve.Image = bmp;
            bsolve.SizeMode = PictureBoxSizeMode.StretchImage;
            bsolve.Size = new Size((int)w, (int)h);
            bsolve.Location = new Point(panel.Width / 2 - bsolve.Width / 2, tsolve.Location.Y + tsolve.Height + 10);
            bsolve.MouseDown += new MouseEventHandler(bsolve_MouseDown);
            bsolve.MouseUp += new MouseEventHandler(bsolve_MouseUp);
            bsolve.Click += SolveDel;
            bsolve.MouseMove += new MouseEventHandler(S.MouseMove);
            panel.Controls.Add(bsolve);

            //TrackBar
            MenuTb.BackColor = Color.Transparent;
            MenuTb.caret.Image = new Bitmap(Application.StartupPath + "\\caret.png");
            MenuTb.caret.BackColor = Color.Transparent;
            MenuTb.bar.Image = new Bitmap(Application.StartupPath + "\\slider.png");
            MenuTb.bar.BackColor = Color.Transparent;
            MenuTb.backbar.Image = new Bitmap(Application.StartupPath + "\\backslider.png");
            MenuTb.backbar.BackColor = Color.Transparent;
            MenuTb.Location = new Point(panel.Width / 2 -MenuTb.Width / 2, bsolve.Location.Y + bsolve.Height + 10);            
            MenuTb.Scroll +=new EventHandler(MenuTb_Scroll);
            panel.Controls.Add(MenuTb);
            S.caretX = MenuTb.CaretLocationX;
            MenuScroll += new dlgScroll(change_caret);

            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Paint += new PaintEventHandler(Menu_Paint);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
            t.Focus();
        }
        #endregion

        #region Size Slider
        void PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (sender == t)
            {
                if (e.KeyCode == Keys.Left)
                    leftA_Click(t, new EventArgs());

                else if (e.KeyCode == Keys.Right)
                    rightA_Click(t, new EventArgs());

                else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    if (imageSlide == 1 && solverImageSlider == 1)
                    {
                        t.Image = one;
                        tsolve.Image = one_focus;
                    }
                    else if (imageSlide == 3 && solverImageSlider == 1)
                    {
                        t.Image = three;
                        tsolve.Image = one_focus;
                    }
                    else if (imageSlide == 5 && solverImageSlider == 1)
                    {
                        t.Image = five;
                        tsolve.Image = one_focus;
                    }

                    else if (imageSlide == 1 && solverImageSlider == 3)
                    {
                        t.Image = one;
                        tsolve.Image = three_focus;
                    }
                    else if (imageSlide == 3 && solverImageSlider == 3)
                    {
                        t.Image = three;
                        tsolve.Image = three_focus;
                    }
                    else if (imageSlide == 5 && solverImageSlider == 3)
                    {
                        t.Image = five;
                        tsolve.Image = three_focus;
                    }

                    else if (imageSlide == 1 && solverImageSlider == 5)
                    {
                        t.Image = one;
                        tsolve.Image = five_focus;
                    }
                    else if (imageSlide == 3 && solverImageSlider == 5)
                    {
                        t.Image = three;
                        tsolve.Image = five_focus;
                    }
                    else if (imageSlide == 5 && solverImageSlider == 5)
                    {
                        t.Image = five;
                        tsolve.Image = five_focus;
                    }
                    t.LostFocus -= new EventHandler(LostFocus);
                    t.PreviewKeyDown-=new PreviewKeyDownEventHandler(PreviewKeyDown);
                    tsolve.LostFocus+=new EventHandler(LostFocus);
                    tsolve.PreviewKeyDown+=new PreviewKeyDownEventHandler(PreviewKeyDown);
                    tsolve.Focus();
                }

                else if (e.KeyCode == Keys.Enter)
                    b_Click(b, new EventArgs());
            }
            else if (tsolve == sender)
            {
                if (e.KeyCode == Keys.Left)
                    leftAS_Click(tsolve, new EventArgs());

                else if (e.KeyCode == Keys.Right)
                    rightAS_Click(tsolve, new EventArgs());

                else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    if (solverImageSlider == 1 && imageSlide == 1)
                    {
                        tsolve.Image = one;
                        t.Image = one_focus;
                    }
                    else if (solverImageSlider == 3 && imageSlide == 1)
                    {
                        tsolve.Image = three;
                        t.Image = one_focus;
                    }
                    else if (solverImageSlider == 5 && imageSlide == 1)
                    {
                        tsolve.Image = five;
                        t.Image = one_focus;
                    }

                    else if (solverImageSlider == 1 && imageSlide == 3)
                    {
                        tsolve.Image = one;
                        t.Image = three_focus;
                    }
                    else if (solverImageSlider == 3 && imageSlide == 3)
                    {
                        tsolve.Image = three;
                        t.Image = three_focus;
                    }
                    else if (solverImageSlider == 5 && imageSlide == 3)
                    {
                        tsolve.Image = five;
                        t.Image = three_focus;
                    }

                    else if (solverImageSlider == 1 && imageSlide == 5)
                    {
                        tsolve.Image = one;
                        t.Image = five_focus;
                    }
                    else if (solverImageSlider == 3 && imageSlide == 5)
                    {
                        tsolve.Image = three;
                        t.Image = five_focus;
                    }
                    else if (solverImageSlider == 5 && imageSlide == 5)
                    {
                        tsolve.Image = five;
                        t.Image = five_focus;
                    }
                    tsolve.LostFocus -= new EventHandler(LostFocus);
                    tsolve.PreviewKeyDown -= new PreviewKeyDownEventHandler(PreviewKeyDown);
                    t.LostFocus += new EventHandler(LostFocus);
                    t.PreviewKeyDown += new PreviewKeyDownEventHandler(PreviewKeyDown);
                    t.Focus();
                }

                else if (e.KeyCode == Keys.Enter)
                    bsolve_Click(bsolve, new EventArgs());
            }
        }

        private void LostFocus(object sender, EventArgs e)
        {
            if (Form.ActiveForm == this)
                ((PictureBox)sender).Focus();
        } 
        #endregion

        #region Levels
        void easy_Click(object sender, EventArgs e)
        {
            if (easy.Image != easyBfocus)
            {
                level = 1;
                easy.Image = easyBfocus;
                medium.Image = mediumB;
                hard.Image = hardB;    
            }
        }
        void medium_Click(object sender, EventArgs e)
        {
            if (medium.Image != mediumBfocus)
            {
                level = 2;
                easy.Image = easyB;
                medium.Image = mediumBfocus;
                hard.Image = hardB;
            }
        }
        void hard_Click(object sender, EventArgs e)
        {
            if (hard.Image != hardBfocus)
            {
                level = 3;
                easy.Image = easyB;
                medium.Image = mediumB;
                hard.Image = hardBfocus;
            }
        }
        #endregion

        #region Arrows Events
        void leftA_Click(object sender, EventArgs e)
        {
            if (imageSlide >1)
            {
                if (t.Focused)
                {
                    if (imageSlide == 3)
                    {
                        imageSlide--;
                        t.Image = two_focus;
                        Application.DoEvents();
                        Thread.Sleep(30);
                        imageSlide--;
                        t.Image = one_focus;
                    }
                    else if (imageSlide == 5)
                    {
                        imageSlide--;
                        t.Image = four_focus;
                        Application.DoEvents();
                        Thread.Sleep(30);
                        imageSlide--;
                        t.Image = three_focus;
                    }
                }
                else
                {
                    if (imageSlide == 3)
                    {
                        imageSlide--;
                        t.Image = two;
                        Application.DoEvents();
                        Thread.Sleep(30);
                        imageSlide--;
                        t.Image = one;
                    }
                    else if (imageSlide == 5)
                    {
                        imageSlide--;
                        t.Image = four;
                        Application.DoEvents();
                        Thread.Sleep(30);
                        imageSlide--;
                        t.Image = three;
                    }
                }
            }
        }
        void leftAS_Click(object sender, EventArgs e)
        {
            if (solverImageSlider > 1)
            {
                if (tsolve.Focused)
                {
                    if (solverImageSlider == 3)
                    {
                        solverImageSlider--;
                        tsolve.Image = two_focus;
                        Application.DoEvents();
                        Thread.Sleep(30);
                        solverImageSlider--;
                        tsolve.Image = one_focus;
                    }
                    else if (solverImageSlider == 5)
                    {
                        solverImageSlider--;
                        tsolve.Image = four_focus;
                        Application.DoEvents();
                        Thread.Sleep(30);
                        solverImageSlider--;
                        tsolve.Image = three_focus;
                    }
                }
                else
                {
                    if (solverImageSlider == 3)
                    {
                        solverImageSlider--;
                        tsolve.Image = two;
                        Application.DoEvents();
                        Thread.Sleep(30);
                        solverImageSlider--;
                        tsolve.Image = one;
                    }
                    else if (solverImageSlider == 5)
                    {
                        solverImageSlider--;
                        tsolve.Image = four;
                        Application.DoEvents();
                        Thread.Sleep(30);
                        solverImageSlider--;
                        tsolve.Image = three;
                    }
                }
            }
        }
        void leftA_MouseMove(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Cursor = Cursors.Hand;
            ((PictureBox)sender).Image = new Bitmap(Application.StartupPath + "\\Size\\leftAO.png");
        }
        void leftA_MouseLeave(object sender, EventArgs e)
        {
            ((PictureBox)sender).Image = new Bitmap(Application.StartupPath + "\\Size\\leftA.png");
        }

        void rightA_Click(object sender, EventArgs e)
        {
            if (imageSlide <5)
            {
                if (t.Focused)
                {
                    if (imageSlide == 1)
                    {
                        imageSlide++;
                        t.Image = two_focus;
                        Application.DoEvents();
                        Thread.Sleep(30);
                        imageSlide++;
                        t.Image = three_focus;
                    }
                    else if (imageSlide == 3)
                    {
                        imageSlide++;
                        t.Image = four_focus;
                        Application.DoEvents();
                        Thread.Sleep(30);
                        imageSlide++;
                        t.Image = five_focus;
                    }
                }
                else
                {
                    if (imageSlide == 1)
                    {
                        imageSlide++;
                        t.Image = two;
                        Application.DoEvents();
                        Thread.Sleep(30);
                        imageSlide++;
                        t.Image = three;
                    }
                    else if (imageSlide == 3)
                    {
                        imageSlide++;
                        t.Image = four;
                        Application.DoEvents();
                        Thread.Sleep(30);
                        imageSlide++;
                        t.Image = five;
                    }
                }
            }
        }
        void rightAS_Click(object sender, EventArgs e)
        {
            if (solverImageSlider < 5)
            {
                if (tsolve.Focused)
                {
                    if (solverImageSlider == 1)
                    {
                        solverImageSlider++;
                        tsolve.Image = two_focus;
                        Application.DoEvents();
                        Thread.Sleep(30);
                        solverImageSlider++;
                        tsolve.Image = three_focus;
                    }
                    else if (solverImageSlider == 3)
                    {
                        solverImageSlider++;
                        tsolve.Image = four_focus;
                        Application.DoEvents();
                        Thread.Sleep(30);
                        solverImageSlider++;
                        tsolve.Image = five_focus;
                    }
                }
                else
                {
                    if (solverImageSlider == 1)
                    {
                        solverImageSlider++;
                        tsolve.Image = two;
                        Application.DoEvents();
                        Thread.Sleep(30);
                        solverImageSlider++;
                        tsolve.Image = three;
                    }
                    else if (solverImageSlider == 3)
                    {
                        solverImageSlider++;
                        tsolve.Image = four;
                        Application.DoEvents();
                        Thread.Sleep(30);
                        solverImageSlider++;
                        tsolve.Image = five;
                    }
                }
            }
        }
        void rightA_MouseMove(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Cursor = Cursors.Hand;
            ((PictureBox)sender).Image = new Bitmap(Application.StartupPath + "\\Size\\rightAO.png");
        }
        void rightA_MouseLeave(object sender, EventArgs e)
        {
            ((PictureBox)sender).Image = new Bitmap(Application.StartupPath + "\\Size\\rightA.png");
        }
        #endregion

        #region Events
        private void Menu_Paint(object sender, PaintEventArgs e)
        {
            if (start)
            {
                start = false;
                x = new Random().Next(this.Width - sudoku1.Width, 0);
                y = new Random().Next(this.Height - sudoku1.Height, 0);
                e.Graphics.DrawImage(sudoku1, new Point(x, y));
            }
            else
            {
                if (bul)
                {
                    e.Graphics.DrawImage(sudoku1, new Point(x - move, y));
                    e.Graphics.DrawImage(sudoku2, new Point((x + sudoku1.Width) - move, y));
                }
                else
                {
                    e.Graphics.DrawImage(sudoku2, new Point(x - move, y));
                    e.Graphics.DrawImage(sudoku1, new Point((x + sudoku2.Width) - move, y));
                }
                if ((x + sudoku1.Width) - move == 0 && bul == true)
                {
                    x = 0;
                    move = 0;
                    bul = false;
                }
                if ((x + sudoku2.Width) - move == 0 && bul == false)
                {
                    x = 0;
                    move = 0;
                    bul = true;
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.Refresh();
            move++;
        }
        #endregion

        #region Slider
        void change_caret(int caret)
        {
            MenuTb.CaretLocationX = caret;
        }

        void MenuTb_Scroll(object sender, EventArgs e)
        {
            S.m.Volume = MenuTb.SoundScale * MenuTb.CaretLocationX;
            S.caretX = MenuTb.CaretLocationX;
            // Calculate the volume that's being set. BTW: this is a trackbar!
            int NewVolume = ((ushort.MaxValue / MenuTb.Width) * MenuTb.CaretLocationX);
            // Set the same volume for both the left and the right channels
            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            // Set the volume
            S.waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
            if (Form.ActiveForm == this && sudokuForm!=null)
                sudokuForm.SudokuScroll(MenuTb.CaretLocationX);
            if (Form.ActiveForm == this && solverForm != null)
                solverForm.SolverScroll(MenuTb.CaretLocationX);
        }
        #endregion

        #region Button Events
        private void b_Click(object sender, EventArgs e)
        {
            int size;
            S.yes.Play();

            if (imageSlide!=3)
            {
                if (imageSlide != 1)
                    size = 16;
                else
                    size = 4;
            }
            else
                size = 9;
            sudokuForm = new SudokuForm();
            sudokuForm.sudokuLevel = level;
            sudokuForm.sudokuSize = size;
            sudokuForm.Show();
        }

        private void b_MouseDown(object sender, MouseEventArgs e)
        {
            float h = 28;
            float bmpW = b.Image.Height;
            float ratio = bmpW / b.Image.Width;
            float w = h / ratio;
            b.Size = new Size((int)w, (int)h);
            b.Location = new Point(panel.Width / 2 - b.Width / 2, t.Location.Y + t.Height + 10);
        }

        private void b_MouseUp(object sender, MouseEventArgs e)
        {
            float h = 30;
            float bmpW = b.Image.Height;
            float ratio = bmpW / b.Image.Width;
            float w = h / ratio;
            b.Size = new Size((int)w, (int)h);
            b.Location = new Point(panel.Width / 2 - b.Width / 2, t.Location.Y + t.Height + 10);
        }

        private void bsolve_Click(object sender, EventArgs e)
        {
            int size;
            S.yes.Play();
            if (solverImageSlider != 3)
            {
                if (solverImageSlider != 1)
                    size = 16;
                else
                    size = 4;
            }
            else
                size = 9;
            solverForm = new SolverForm();
            solverForm.size = size;
            solverForm.Show();
        }

        void bsolve_MouseUp(object sender, MouseEventArgs e)
        {
            float h = 30;
            float bmpW = bsolve.Image.Height;
            float ratio = bmpW / bsolve.Image.Width;
            float w = h / ratio;
            bsolve.Size = new Size((int)w, (int)h);
            bsolve.Location = new Point(panel.Width / 2 - bsolve.Width / 2, tsolve.Location.Y + tsolve.Height + 10);
        }

        void bsolve_MouseDown(object sender, MouseEventArgs e)
        {
            float h = 28;
            float bmpW = bsolve.Image.Height;
            float ratio = bmpW / bsolve.Image.Width;
            float w = h / ratio;
            bsolve.Size = new Size((int)w, (int)h);
            bsolve.Location = new Point(panel.Width / 2 - bsolve.Width / 2, tsolve.Location.Y + tsolve.Height + 10);
        }
        #endregion

        #region Blur Effect
        private static Bitmap Blur(Bitmap image, Rectangle rectangle, Int32 blurSize)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            // look at every pixel in the blur rectangle
            for (Int32 xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (Int32 yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    Int32 avgR = 0, avgG = 0, avgB = 0;
                    Int32 blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (Int32 x = xx; (x < xx + blurSize && x < image.Width); x++)
                    {
                        for (Int32 y = yy; (y < yy + blurSize && y < image.Height); y++)
                        {
                            Color pixel = blurred.GetPixel(x, y);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (Int32 x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                        for (Int32 y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                            blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }
            return blurred;
        }
        #endregion
    }
}
