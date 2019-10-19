using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace SudokuSnake
{
    public partial class SolverForm : Form
    {
        #region Data
        private SolverGrid solver;
        public int size, ButHeight = 120;
        private PictureBox 
            Solve,
            Clear,
            ClearAll,
            Print,
            Cam;

        public ActivateSnake AS;
        public DeactivateSnake DS;
        public UpdateScore US;
        public Strike striked;
        public PictureBox Pause;
        private Label Score;

        private Bitmap bmp;
        private int i = 0;
        private Timer CamTimer;
        private bool reverse = false, first = true, first_reverse = true;

        private PictureBox Fucked;
        private Timer fucked;
        private int iFucked = 0;

        private MyTrackbar mtb;
        public dlgScroll SolverScroll;

        private PrintForm p;
        #endregion

        #region Ctor
        public SolverForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Icon = new Icon(Application.StartupPath + "\\icon.ico");
        }

        private void SudokuSolver_Load(object sender, EventArgs e)
        {
            //CamTimer
            CamTimer = new Timer();
            CamTimer.Interval = 30;
            CamTimer.Tick += new EventHandler(CamTimer_Tick);

            //Grid
            solver=new SolverGrid(size,10,10,500,500,this);
            this.Controls.Add(solver.table.panel);

            //Bitmap
            bmp = new Bitmap(solver.table.panel.Width, solver.table.panel.Height);
            solver.table.panel.DrawToBitmap(bmp, new Rectangle(0, 0, solver.table.panel.Width, solver.table.panel.Height));

            //Cam Effect
            Cam = new PictureBox();
            Cam.Location = new Point(solver.table.panel.Location.X, solver.table.panel.Location.Y);
            Cam.SizeMode = PictureBoxSizeMode.StretchImage;
            Cam.Size = new Size(solver.table.panel.Width, solver.table.panel.Height);
            Cam.Image = new Bitmap(Application.StartupPath + "\\camEffect\\1.png");
            Cam.BackgroundImage = bmp;
            this.Controls.Add(Cam);
            Cam.BringToFront();
            solver.table.panel.SendToBack();

            //Solve Button
            int x = solver.table.panel.Location.X + solver.table.panel.Width;
            Solve = new PictureBox();
            Solve.BackColor = Color.Transparent;
            Solve.Image = new Bitmap(Application.StartupPath + "\\solve2.png");
            Solve.SizeMode = PictureBoxSizeMode.StretchImage;
            Solve.Size = new Size(ButHeight, ButHeight);
            Solve.Location = new Point(x + (this.Width - x) / 2 - Solve.Width / 2, solver.table.panel.Location.Y);
            Solve.MouseMove += new MouseEventHandler(S.MouseMove);
            Solve.Click += new EventHandler(Solve_Click);
            Solve.MouseDown += new MouseEventHandler(Solve_MouseDown);
            Solve.MouseUp += new MouseEventHandler(Solve_MouseUp);
            this.Controls.Add(Solve);

            //Clear Button
            Clear = new PictureBox();
            Clear.Image = new Bitmap(Application.StartupPath + "\\clear.png");
            Clear.BackColor = Color.Transparent;
            Clear.SizeMode = PictureBoxSizeMode.StretchImage;
            Clear.Size = new Size(ButHeight, ButHeight);
            Clear.MouseMove += new MouseEventHandler(S.MouseMove);
            Clear.Location = new Point(x + (this.Width - x) / 2 - Clear.Width / 2, Solve.Location.Y + Solve.Height + 10);
            Clear.Click += new EventHandler(clear_Click);
            Clear.MouseDown += new MouseEventHandler(clear_MouseDown);
            Clear.MouseUp += new MouseEventHandler(clear_MouseUp);
            this.Controls.Add(Clear);

            //Generate Button
            ClearAll = new PictureBox();
            ClearAll.Image = new Bitmap(Application.StartupPath + "\\clearall.png");
            ClearAll.BackColor = Color.Transparent;
            ClearAll.SizeMode = PictureBoxSizeMode.StretchImage;
            ClearAll.Size = new Size(ButHeight, ButHeight);
            ClearAll.Location = new Point(x + (this.Width - x) / 2 - ClearAll.Width / 2, Clear.Location.Y + Clear.Height + 10);
            ClearAll.MouseMove += new MouseEventHandler(S.MouseMove);
            ClearAll.MouseUp += new MouseEventHandler(clearall_MouseUp);
            ClearAll.Click += new EventHandler(clearall_Click);
            ClearAll.MouseDown += new MouseEventHandler(clearall_MouseDown);
            this.Controls.Add(ClearAll);

            //Print Button
            Print = new PictureBox();
            Print.Image = new Bitmap(Application.StartupPath + "\\print2.png");
            Print.BackColor = Color.Transparent;
            Print.SizeMode = PictureBoxSizeMode.StretchImage;
            Print.Size = new Size(ButHeight, ButHeight);
            Print.Location = new Point(x + (this.Width - x) / 2 - Print.Width / 2, ClearAll.Location.Y + ClearAll.Height + 10);
            Print.MouseMove += new MouseEventHandler(S.MouseMove);
            Print.Click += new EventHandler(print_Click);
            Print.MouseDown += new MouseEventHandler(print_MouseDown);
            Print.MouseUp += new MouseEventHandler(print_MouseUp);
            this.Controls.Add(Print);

            //TrackBar
            mtb = new MyTrackbar();
            mtb.Size = S.SliderSize;
            mtb.BackColor = Color.Transparent;
            mtb.caret.Image = new Bitmap(Application.StartupPath + "\\caret.png");
            mtb.caret.BackColor = Color.Transparent;
            mtb.bar.Image = new Bitmap(Application.StartupPath + "\\slider.png");
            mtb.bar.BackColor = Color.Transparent;
            mtb.backbar.Image = new Bitmap(Application.StartupPath + "\\backslider.png");
            mtb.backbar.BackColor = Color.Transparent;
            mtb.Location = new Point(this.Width / 2 - mtb.Width / 2, solver.table.panel.Location.Y + solver.table.panel.Height + 10);
            mtb.Scroll += new EventHandler(mtb_Scroll);
            this.Controls.Add(mtb);
            mtb.CaretLocationX = S.caretX;
            SolverScroll = new dlgScroll(mtb_changeCaret);

            AS = new ActivateSnake(MakeSnake);
            DS = new DeactivateSnake(KillSnake);
            US = new UpdateScore(RaiseScore);
            striked = new Strike(strike);

            this.BackgroundImage = new Bitmap(Application.StartupPath + "\\parquet.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Shown += new EventHandler(SudokuSolver_Shown);
            this.FormClosing+=new FormClosingEventHandler(SudokuSolver_FormClosing);
        }
        #endregion

        #region Slider
        void mtb_changeCaret(int caretX)
        {
            mtb.CaretLocationX = caretX;
        }
        void mtb_Scroll(object sender, EventArgs e)
        {
            if (Form.ActiveForm == this)
                SudokuSnake.Menu.MenuScroll(mtb.CaretLocationX);
        }
        #endregion

        #region Snake
        private void KillSnake()
        {
            Solve.Enabled=true;
            Clear.Enabled=true;
            ClearAll.Enabled=true;
            Print.Enabled = true;
            this.Controls.Remove(Score);
            this.Controls.Remove(Pause);
        }
        private void RaiseScore(bool add, int update)
        {
            if (Score.InvokeRequired)
            {
                UpdateScore del = new UpdateScore(RaiseScore);
                Score.Invoke(del, new object[] { add, update });
            }
            else
            {
                if (add)
                {
                    int total = int.Parse(Score.Text) + update;
                    Score.Text = total.ToString();
                }
                else if (int.Parse(Score.Text) > 0)
                {
                    int total = int.Parse(Score.Text) - update;
                    Score.Text = total.ToString();
                }
            }
        }
        private void MakeSnake()
        {
            Solve.Enabled = false;
            Clear.Enabled = false;
            ClearAll.Enabled = false;
            Print.Enabled = false;

            Score = new Label();
            Score.Text = "0";
            Score.Height = 50;
            Score.Font = S.GetFontForTextBoxHeight(50, Score.Font);
            Score.BackColor = Color.Transparent;
            Score.Location = new Point(10, 510);

            Pause = new PictureBox();
            Pause.BackColor = Color.Transparent;
            Pause.Image = new Bitmap(Application.StartupPath + "\\Pause.png");
            Pause.SizeMode = PictureBoxSizeMode.StretchImage;
            Pause.Size = new Size(30, 40);
            Pause.Location = new Point(Score.Location.X + Score.Width,
                solver.table.panel.Location.Y + solver.table.panel.Height +
                (this.Height - (solver.table.panel.Location.Y + solver.table.panel.Height)) / 2 -
                Pause.Height + 3);
            Pause.MouseMove += new MouseEventHandler(S.MouseMove);
            Pause.Click += new EventHandler(Pause_Click);

            this.Controls.Add(Score);
            this.Controls.Add(Pause);
        }
        private void strike()
        {
            Bitmap copy = new Bitmap(800, 600);
            this.DrawToBitmap(copy, new Rectangle(0, 0, 800, 600));

            Fucked = new PictureBox();
            Fucked.Location = new Point(-5, -27);
            Fucked.BackgroundImage = copy;
            Fucked.Size = new Size(800, 600);
            Fucked.Click += new EventHandler(Fucked_Click);
            this.Controls.Add(Fucked);
            Fucked.BringToFront();

            fucked = new Timer();
            fucked.Interval = 5;
            fucked.Tick += new EventHandler(fucked_Tick);
            fucked.Start();
        }

        void fucked_Tick(object sender, EventArgs e)
        {
            iFucked++;
            if (iFucked < 8)
            {
                Fucked.Image = new Bitmap(Application.StartupPath + "\\FUCKED\\fucked" + iFucked + ".png");
                Application.DoEvents();
            }
            else
            {
                iFucked = 0;
                S.punch.Play();
                fucked.Stop();
            }
        }

        void Fucked_Click(object sender, EventArgs e)
        {
            this.Controls.Remove(((PictureBox)sender));
            this.Controls.Remove(Pause);
            this.Controls.Remove(Score);
            solver.UnSnake();
        }
        private void Pause_Click(object sender, EventArgs e)
        {
            if (solver.snake.snake.Enabled==false)
            {
                Pause.Image = new Bitmap(Application.StartupPath + "\\Pause.png");
                solver.snake.EnableKeys = true;
                solver.snake.snake.Start();

            }
            else
            {
                Pause.Image = new Bitmap(Application.StartupPath + "\\Play.png");
                solver.snake.EnableKeys = false;
                solver.snake.snake.Stop();
            }
        }
        #endregion

        #region Events
        void CamTimer_Tick(object sender, EventArgs e)
        {
            if (i == 0)
                Thread.Sleep(200);
            if (reverse == false)
            {
                if (first)
                {
                    first = false;
                    S.swoosh.Play();
                }
                i++;
                if (i <= 9)
                {
                    Cam.Image = new Bitmap(Application.StartupPath + "\\camEffect\\" + i + ".png");
                    Application.DoEvents();
                }
                else
                {
                    CamTimer.Stop();
                    Cam.Image = null;
                    Application.DoEvents();
                    reverse = true;
                    i = 10;
                    this.Controls.Remove(Cam);

                    ClearAll.Enabled = true;
                    Solve.Enabled = true;
                    Clear.Enabled = true;
                    Print.Enabled = true;
                }

            }
            else
            {
                if (first_reverse)
                {
                    first_reverse = false;
                    S.swoosh_reverse.Play();
                }
                i--;
                if (i > 0)
                {
                    Cam.Image = new Bitmap(Application.StartupPath + "\\camEffect\\" + i + ".png");
                    Application.DoEvents();
                }
                else
                {
                    reverse = false;
                    i = 0;
                    solver.Solve();
                    solver.table.panel.DrawToBitmap(bmp, new Rectangle(0, 0, 500, 500));
                    Cam.BackgroundImage = bmp;
                }
            }
        }

        void SudokuSolver_Shown(object sender, EventArgs e)
        {
            CamTimer.Start();
        }

        void SudokuSolver_FormClosing(object sender, FormClosingEventArgs e)
        {
            first = true;
            if (p != null)
                if (p.Visible)
                    p.Hide();
            this.Hide();
            if (solver.snake != null && solver.snake.snake.Enabled)
                solver.snake.snake.Stop();
        }
        #endregion

        #region Button Events
        #region Solve Events
        private void Solve_Click(object sender, EventArgs e)
        {
            S.but.Play();
            first = true;
            first_reverse = true;
            ClearAll.Enabled = false;
            Solve.Enabled = false;
            Clear.Enabled = false;
            Print.Enabled = false;

            this.Controls.Add(Cam);
            Cam.BringToFront();
            solver.table.panel.SendToBack();
            solver.table.panel.DrawToBitmap(bmp, new Rectangle(0, 0, 500, 500));
            Thread.Sleep(150);

            CamTimer.Start();
        }
        void Solve_MouseUp(object sender, MouseEventArgs e)
        {
            Solve.Image = new Bitmap(Application.StartupPath + "\\solve2.png");
        }
        void Solve_MouseDown(object sender, MouseEventArgs e)
        {
            Solve.Image = new Bitmap(Application.StartupPath + "\\solvedown.png");
        }
        #endregion

        #region Clear Events
        void clear_Click(object sender, EventArgs e)
        {
            S.but.Play();
            S.noTypeSound = true;
            solver.Clear();
            S.noTypeSound = false;
        }

        void clear_MouseUp(object sender, MouseEventArgs e)
        {
            Clear.Image = new Bitmap(Application.StartupPath + "\\clear.png");
        }
        void clear_MouseDown(object sender, MouseEventArgs e)
        {
            Clear.Image = new Bitmap(Application.StartupPath + "\\cleardown.png");
        }
        #endregion

        #region ClearAll Events
        void clearall_Click(object sender, EventArgs e)
        {
            S.but.Play();
            S.noTypeSound = true;
            solver.ClearAll();
            S.noTypeSound = false;
        }
        void clearall_MouseUp(object sender, MouseEventArgs e)
        {
            ClearAll.Image = new Bitmap(Application.StartupPath + "\\clearall.png");
        }
        void clearall_MouseDown(object sender, MouseEventArgs e)
        {
            ClearAll.Image = new Bitmap(Application.StartupPath + "\\clearalldown.png");
        }
        #endregion

        #region Print Events
        void print_Click(object sender, EventArgs e)
        {
            S.but.Play();
            if (p == null || p.Visible == false)
            {
                p = new PrintForm();                
                p.toPrint = solver.copy;
                p.Show();
            }
        }
        void print_MouseUp(object sender, MouseEventArgs e)
        {
            Print.Image = new Bitmap(Application.StartupPath + "\\print2.png");
        }
        void print_MouseDown(object sender, MouseEventArgs e)
        {
            Print.Image = new Bitmap(Application.StartupPath + "\\print2down.png");
        }
        #endregion
        #endregion
    }
}
