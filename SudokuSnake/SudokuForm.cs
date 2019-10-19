using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Timer = System.Windows.Forms.Timer;

namespace SudokuSnake
{
    public delegate void ActivateSnake();
    public delegate void DeactivateSnake();
    public delegate void UpdateScore(bool add,int update);
    public delegate void Win();
    public delegate void Strike();

    public partial class SudokuForm : Form
    {
        #region Data
        public int sudokuSize, sudokuLevel ,ButHeight=120;
        private SudokuGrid grid;
        private PictureBox solve, clear, generate, print,Cam;

        public Win won;
        private Timer WinTimer;
        private PictureBox win;
        private int iWin = 0;

        private int iFucked=0;
        private Timer fucked;
        private PictureBox Fucked;

        public ActivateSnake AS;
        public DeactivateSnake DS;
        public UpdateScore US;
        public Strike striked;
        public PictureBox Pause;
        private Label Score;

        private Bitmap bmp;
        private int i = 0;
        private Timer CamTimer;
        private bool reverse = false,first = true, first_reverse = true;

        private MyTrackbar mtb;
        public dlgScroll SudokuScroll;

        private PrintForm p;
        #endregion

        #region Ctor
        public SudokuForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 800;
            this.Height = 600;
            this.Icon = new Icon(Application.StartupPath + "\\icon.ico");
        }

        private void SudokuForm_Load(object sender, EventArgs e)
        {
            //CamTimer
            CamTimer = new Timer();
            CamTimer.Interval = 30;
            CamTimer.Tick += new EventHandler(CamTimer_Tick);

            //Grid
            grid = new SudokuGrid(sudokuSize,sudokuLevel, 10, 10, 500, 500,this);
            this.Controls.Add(grid.table.panel);

            //Bitmap
            bmp = new Bitmap(grid.table.panel.Width, grid.table.panel.Height);
            grid.table.panel.DrawToBitmap(bmp, new Rectangle(0, 0, grid.table.panel.Width, grid.table.panel.Height));

            //Cam Effect
            Cam = new PictureBox();
            Cam.Location = new Point(grid.table.panel.Location.X, grid.table.panel.Location.Y);
            Cam.SizeMode = PictureBoxSizeMode.StretchImage;
            Cam.Size = new Size(grid.table.panel.Width, grid.table.panel.Height);       
            Cam.Image = new Bitmap(Application.StartupPath + "\\camEffect\\1.png");
            Cam.BackgroundImage = bmp;
            this.Controls.Add(Cam);
            Cam.BringToFront();
            grid.table.panel.SendToBack();

            //Solve Button
            int x = grid.table.panel.Location.X + grid.table.panel.Width;
            solve = new PictureBox();
            solve.BackColor = Color.Transparent;
            solve.Image = new Bitmap(Application.StartupPath + "\\solve2.png");
            solve.SizeMode = PictureBoxSizeMode.StretchImage;
            solve.Size = new Size(ButHeight, ButHeight);
            solve.Location = new Point(x+(this.Width-x)/2-solve.Width/2, grid.table.panel.Location.Y);
            solve.MouseMove += new MouseEventHandler(S.MouseMove);
            solve.Click += new EventHandler(solve_Click);
            solve.MouseDown += new MouseEventHandler(solve_MouseDown);
            solve.MouseUp += new MouseEventHandler(solve_MouseUp);
            this.Controls.Add(solve);

            //Clear Button
            clear = new PictureBox();
            clear.Image = new Bitmap(Application.StartupPath + "\\clear.png");
            clear.BackColor = Color.Transparent;
            clear.SizeMode = PictureBoxSizeMode.StretchImage;
            clear.Size = new Size(ButHeight, ButHeight);
            clear.MouseMove += new MouseEventHandler(S.MouseMove);
            clear.Location = new Point(x + (this.Width - x) / 2 - clear.Width / 2, solve.Location.Y +solve.Height+10);
            clear.Click += new EventHandler(clear_Click);
            clear.MouseDown += new MouseEventHandler(clear_MouseDown);
            clear.MouseUp += new MouseEventHandler(clear_MouseUp);
            this.Controls.Add(clear);

            //Generate Button
            generate = new PictureBox();
            generate.Image = new Bitmap(Application.StartupPath + "\\gen2.png");
            generate.BackColor = Color.Transparent;
            
            generate.SizeMode = PictureBoxSizeMode.StretchImage;
            generate.Size = new Size(ButHeight, ButHeight);
            generate.Location = new Point(x + (this.Width - x) / 2 - generate.Width / 2, clear.Location.Y + clear.Height + 10);
            generate.MouseMove += new MouseEventHandler(S.MouseMove);
            generate.MouseUp += new MouseEventHandler(generate_MouseUp);
            generate.Click += new EventHandler(generate_Click);
            generate.MouseDown += new MouseEventHandler(generate_MouseDown);
            this.Controls.Add(generate);

            //Print Button
            print = new PictureBox();
            print.Image = new Bitmap(Application.StartupPath + "\\print2.png");
            print.BackColor = Color.Transparent;
            print.SizeMode = PictureBoxSizeMode.StretchImage;
            print.Size = new Size(ButHeight, ButHeight);
            print.MouseMove += new MouseEventHandler(S.MouseMove);
            print.Location = new Point(x + (this.Width - x) / 2 - print.Width / 2, generate.Location.Y + generate.Height + 10);
            print.Click += new EventHandler(print_Click);
            print.MouseDown += new MouseEventHandler(print_MouseDown);
            print.MouseUp += new MouseEventHandler(print_MouseUp);
            this.Controls.Add(print);

            AS = new ActivateSnake(MakeSnake);
            DS = new DeactivateSnake(KillSnake);
            US = new UpdateScore(RaiseScore);
            striked = new Strike(strike);
            
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
            mtb.Location = new Point(this.Width / 2 - mtb.Width / 2, grid.table.panel.Location.Y + grid.table.panel.Height + 10);
            mtb.Scroll += new EventHandler(mtb_Scroll);
            this.Controls.Add(mtb);
            mtb.CaretLocationX = S.caretX;
            SudokuScroll = new dlgScroll(mtb_changeCaret);

            won = new Win(Winner);

            this.BackgroundImage= new Bitmap(Application.StartupPath+"\\parquet.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Shown += new EventHandler(SudokuForm_Shown);
            this.FormClosing += new FormClosingEventHandler(SudokuForm_FormClosing);
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
        }
        #endregion

        #region Slider
        void mtb_changeCaret(int caretX)
        {
            mtb.CaretLocationX = caretX;
        }
        void mtb_Scroll(object sender, EventArgs e)
        {
            if(Form.ActiveForm==this)
                SudokuSnake.Menu.MenuScroll(mtb.CaretLocationX);
        }
        #endregion

        #region Snake
        private void KillSnake()
        {
            generate.Enabled = true;
            solve.Enabled = true;
            clear.Enabled = true;
            print.Enabled = true;
            this.Controls.Remove(Score);
            this.Controls.Remove(Pause);
        }
        private void RaiseScore(bool add, int update)
        {
            if (Score.InvokeRequired)
            {
                UpdateScore del = new UpdateScore(RaiseScore);
                Score.Invoke(del, new object[] { add,update });
            }
            else
            {
                if (add)
                {
                    int total = int.Parse(Score.Text) + update;
                    Score.Text = total.ToString();
                }
                else if(int.Parse(Score.Text)>0)
                {
                    int total = int.Parse(Score.Text)- update;
                    Score.Text = total.ToString();
                }
            }
        }
        private void MakeSnake()
        {
            generate.Enabled = false;
            solve.Enabled = false;
            clear.Enabled = false;
            print.Enabled = false;

            Score = new Label();
            Score.Text = "0";
            Score.Height = 50;
            Score.Font = S.GetFontForTextBoxHeight(50, Score.Font);
            Score.BackColor = Color.Transparent;
            Score.Location = new Point(10, 510);            

            Pause = new PictureBox();
            Pause.BackColor = Color.Transparent;
            Pause.Image = new Bitmap(Application.StartupPath + "\\Pause.png");
            Pause.SizeMode= PictureBoxSizeMode.StretchImage;
            Pause.Size = new Size(30, 40);
            Pause.Location = new Point(Score.Location.X + Score.Width, 
                grid.table.panel.Location.Y + grid.table.panel.Height + 
                (this.Height - (grid.table.panel.Location.Y + grid.table.panel.Height)) / 2 - 
                Pause.Height +3);
            Pause.MouseMove += new MouseEventHandler(S.MouseMove);
            Pause.Click+=new EventHandler(Pause_Click);

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
            Fucked.Click += new EventHandler(fucked_Click);
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

        void fucked_Click(object sender, EventArgs e)
        {
            this.Controls.Remove(((PictureBox)sender));
            this.Controls.Remove(Pause);
            this.Controls.Remove(Score);
            grid.sett.Invoke();
            generate_Click(generate,new EventArgs());
            grid.snake = null;
            
        }
        private void Pause_Click(object sender, EventArgs e)
        {
            if (grid.snake.snake.Enabled==false)
            {
                Pause.Image = new Bitmap(Application.StartupPath + "\\Pause.png");
                grid.snake.EnableKeys = true;
                grid.snake.snake.Start();
                
            }
            else
            {                
                Pause.Image = new Bitmap(Application.StartupPath + "\\Play.png");
                grid.snake.EnableKeys = false;
                grid.snake.snake.Stop();
            }
        }
        #endregion

        #region Winning
        private void Winner()
        {
            if (this.InvokeRequired)
            {
                Win del = new Win(Winner);
                this.Invoke(del);
            }
            else
            {
                Bitmap copy = new Bitmap(800, 600);
                this.DrawToBitmap(copy, new Rectangle(0, 0, 800, 600));
                win = new PictureBox();
                win.Location = new Point(-5, -27);
                win.BackgroundImage = copy;
                win.Size = new Size(800, 600);
                win.Click += new EventHandler(win_Click);
                this.Controls.Add(win);
                win.BringToFront();
                WinTimer = new Timer();
                WinTimer.Interval = 5;
                WinTimer.Tick += new EventHandler(WinTimer_Tick);
                WinTimer.Start();
            }
        }

        void WinTimer_Tick(object sender, EventArgs e)
        {
            iWin ++;
            if (iWin < 8)
            {
                win.Image = new Bitmap(Application.StartupPath + "\\SOLVED\\solved" + iWin + ".png");
                Application.DoEvents();
            }
            else
            {
                iWin = 0;
                S.punch.Play();
                WinTimer.Stop();
            }
        }

        void win_Click(object sender, EventArgs e)
        {
            this.Controls.Remove(((PictureBox)sender));
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

                    generate.Enabled = true;
                    solve.Enabled = true;
                    clear.Enabled = true;
                    print.Enabled = true;                    
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
                    grid.Generate();
                    grid.table.panel.DrawToBitmap(bmp, new Rectangle(0, 0, 500, 500));
                    Cam.BackgroundImage = bmp;
                }
            }
        }

        void SudokuForm_Shown(object sender, EventArgs e)
        {
            CamTimer.Start();
        }

        void SudokuForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            first = true;
            if (p != null)
                if (p.Visible)
                    p.Hide();
            this.Hide();
            if (grid.snake !=null && grid.snake.snake.Enabled)
            {
                grid.snake.snake.Stop();
            }
            
        }
        #endregion        

        #region Solve Events
        void solve_Click(object sender, EventArgs e)
        {
            S.but.Play();
            S.noTypeSound = true;
            grid.Solve();
            S.noTypeSound = false;
        }
        void solve_MouseUp(object sender, MouseEventArgs e)
        {
            solve.Image = new Bitmap(Application.StartupPath + "\\solve2.png");
        }
        void solve_MouseDown(object sender, MouseEventArgs e)
        {
            solve.Image = new Bitmap(Application.StartupPath + "\\solvedown.png");
        }
        #endregion

        #region Clear Events
        void clear_Click(object sender, EventArgs e)
        {
            S.but.Play();
            S.noTypeSound = true;
            grid.Clear();
            S.noTypeSound = false;
        }
        void clear_MouseUp(object sender, MouseEventArgs e)
        {
            clear.Image = new Bitmap(Application.StartupPath + "\\clear.png");
        }

        void clear_MouseDown(object sender, MouseEventArgs e)
        {
            clear.Image = new Bitmap(Application.StartupPath + "\\cleardown.png");
        }
        #endregion

        #region Generate Events
        void generate_Click(object sender, EventArgs e)
        {
            S.but.Play();
            first = true;
            first_reverse = true;
            generate.Enabled = false;
            solve.Enabled = false;
            clear.Enabled = false;
            print.Enabled = false;

            this.Controls.Add(Cam);
            Cam.BringToFront();
            grid.table.panel.SendToBack();
            grid.table.panel.DrawToBitmap(bmp, new Rectangle(0, 0, 500, 500));
            Thread.Sleep(150);

            CamTimer.Start();


        }
        void generate_MouseUp(object sender, MouseEventArgs e)
        {
            generate.Image = new Bitmap(Application.StartupPath + "\\gen2.png");
        }
        void generate_MouseDown(object sender, MouseEventArgs e)
        {
            generate.Image = new Bitmap(Application.StartupPath + "\\gen2down.png");
        } 
        #endregion

        #region Print Events
        void print_Click(object sender, EventArgs e)
        {
            S.but.Play();
            if (p == null || p.Visible == false)
            {
                p = new PrintForm();
                p.toPrint = grid.toPrint;
                p.Show();
            }
        }
        void print_MouseUp(object sender, MouseEventArgs e)
        {
            print.Image = new Bitmap(Application.StartupPath + "\\print2.png");
        }
        void print_MouseDown(object sender, MouseEventArgs e)
        {
            print.Image = new Bitmap(Application.StartupPath + "\\print2down.png");
        } 
        #endregion
    }
}
