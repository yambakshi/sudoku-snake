using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;
using Timer = System.Windows.Forms.Timer;

namespace SudokuSnake
{
    class SolverGrid
    {
        #region Data
        private Form s;
        public Snake snake;
        public UnSnake unsnake;
        public Table table;
        public int size;
        private int[,] Input;
        public Color remember;

        private Timer timer;
        private float r, g, b;
        private float R, G, B;
        public TextBox currT;

        public Bitmap copy;

        public Settings sett;
        #endregion

        #region Ctor
        public SolverGrid(int tableSize, int x, int y, int width, int height,Form s)
        {
            this.s = s;
            this.size = tableSize;
            table = new Table(tableSize, new Point(x, y), new Size(500, 500));

            Input = new int[size, size];
            timer = new Timer();
            timer.Interval = 5;
            timer.Tick += new EventHandler(timer_Tick);

            unsnake = new UnSnake(UnSnake);

            copy = new Bitmap(table.panel.Width, table.panel.Height);

            sett = new Settings(Settings);

            Settings();
        } 
        #endregion

        #region Events
        private void timer_Tick(object sender, EventArgs e)
        {
            if (currT != null)
            {
                R -= r;
                G -= g;
                B -= b;

                if (r <= 0 && g <= 0 && b <= 0)
                {
                    if (S.focus.R + (int)R <= remember.R && S.focus.G + (int)G <= remember.G && S.focus.B + (int)B <= remember.B)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, S.focus.G + (int)G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R <= remember.R && S.focus.G + (int)G <= remember.G)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, S.focus.G + (int)G, remember.B);

                    else if (S.focus.B + (int)B <= remember.B && S.focus.G + (int)G <= remember.G)

                        currT.BackColor = Color.FromArgb(remember.R, S.focus.G + (int)G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R <= remember.R && S.focus.B + (int)B <= remember.B)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, remember.G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R < remember.R)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, remember.G, remember.B);

                    else if (S.focus.G + (int)G < remember.G)

                        currT.BackColor = Color.FromArgb(remember.R, S.focus.G + (int)G, remember.B);

                    else if (S.focus.B + (int)B < remember.B)

                        currT.BackColor = Color.FromArgb(remember.R, remember.G, S.focus.B + (int)B);

                    else
                    {
                        if(table.panel.Parent!=null)
                            table.panel.DrawToBitmap(copy, new Rectangle(0, 0, table.panel.Width, table.panel.Height));
                        R = 0;
                        G = 0;
                        B = 0;
                    }
                }
                else if (r <= 0 && g <= 0 && b > 0)
                {
                    if (S.focus.R + (int)R <= remember.R && S.focus.G + (int)G <= remember.G && S.focus.B + (int)B >= remember.B)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, S.focus.G + (int)G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R <= remember.R && S.focus.G + (int)G <= remember.G)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, S.focus.G + (int)G, remember.B);

                    else if (S.focus.B + (int)B >= remember.B && S.focus.G + (int)G <= remember.G)

                        currT.BackColor = Color.FromArgb(remember.R, S.focus.G + (int)G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R <= remember.R && S.focus.B + (int)B >= remember.B)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, remember.G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R < remember.R)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, remember.G, remember.B);

                    else if (S.focus.G + (int)G < remember.G)

                        currT.BackColor = Color.FromArgb(remember.R, S.focus.G + (int)G, remember.B);

                    else if (S.focus.B + (int)B >= remember.B)

                        currT.BackColor = Color.FromArgb(remember.R, remember.G, S.focus.B + (int)B);

                    else
                    {
                        if (table.panel.Parent != null)
                            table.panel.DrawToBitmap(copy, new Rectangle(0, 0, table.panel.Width, table.panel.Height));
                        R = 0;
                        G = 0;
                        B = 0;
                    }
                }
                else if (r <= 0 && g > 0 && b <= 0)
                {
                    if (S.focus.R + (int)R <= remember.R && S.focus.G + (int)G >= remember.G && S.focus.B + (int)B <= remember.B)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, S.focus.G + (int)G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R <= remember.R && S.focus.G + (int)G >= remember.G)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, S.focus.G + (int)G, remember.B);

                    else if (S.focus.B + (int)B <= remember.B && S.focus.G + (int)G >= remember.G)

                        currT.BackColor = Color.FromArgb(remember.R, S.focus.G + (int)G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R <= remember.R && S.focus.B + (int)B <= remember.B)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, remember.G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R < remember.R)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, remember.G, remember.B);

                    else if (S.focus.G + (int)G >= remember.G)

                        currT.BackColor = Color.FromArgb(remember.R, S.focus.G + (int)G, remember.B);

                    else if (S.focus.B + (int)B < remember.B)

                        currT.BackColor = Color.FromArgb(remember.R, remember.G, S.focus.B + (int)B);

                    else
                    {
                        if (table.panel.Parent != null)
                            table.panel.DrawToBitmap(copy, new Rectangle(0, 0, table.panel.Width, table.panel.Height));
                        R = 0;
                        G = 0;
                        B = 0;
                    }
                }
                else if (r > 0 && g <= 0 && b <= 0)
                {
                    if (S.focus.R + (int)R >= remember.R && S.focus.G + (int)G <= remember.G && S.focus.B + (int)B <= remember.B)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, S.focus.G + (int)G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R >= remember.R && S.focus.G + (int)G <= remember.G)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, S.focus.G + (int)G, remember.B);

                    else if (S.focus.B + (int)B <= remember.B && S.focus.G + (int)G <= remember.G)

                        currT.BackColor = Color.FromArgb(remember.R, S.focus.G + (int)G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R >= remember.R && S.focus.B + (int)B <= remember.B)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, remember.G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R >= remember.R)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, remember.G, remember.B);

                    else if (S.focus.G + (int)G < remember.G)

                        currT.BackColor = Color.FromArgb(remember.R, S.focus.G + (int)G, remember.B);

                    else if (S.focus.B + (int)B < remember.B)

                        currT.BackColor = Color.FromArgb(remember.R, remember.G, S.focus.B + (int)B);

                    else
                    {
                        if (table.panel.Parent != null)
                            table.panel.DrawToBitmap(copy, new Rectangle(0, 0, table.panel.Width, table.panel.Height));
                        R = 0;
                        G = 0;
                        B = 0;
                    }
                }
                else if (r <= 0 && g > 0 && b > 0)
                {
                    if (S.focus.R + (int)R <= remember.R && S.focus.G + (int)G >= remember.G && S.focus.B + (int)B >= remember.B)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, S.focus.G + (int)G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R <= remember.R && S.focus.G + (int)G >= remember.G)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, S.focus.G + (int)G, remember.B);

                    else if (S.focus.B + (int)B >= remember.B && S.focus.G + (int)G >= remember.G)

                        currT.BackColor = Color.FromArgb(remember.R, S.focus.G + (int)G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R <= remember.R && S.focus.B + (int)B >= remember.B)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, remember.G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R < remember.R)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, remember.G, remember.B);

                    else if (S.focus.G + (int)G >= remember.G)

                        currT.BackColor = Color.FromArgb(remember.R, S.focus.G + (int)G, remember.B);

                    else if (S.focus.B + (int)B >= remember.B)

                        currT.BackColor = Color.FromArgb(remember.R, remember.G, S.focus.B + (int)B);

                    else
                    {
                        if (table.panel.Parent != null)
                            table.panel.DrawToBitmap(copy, new Rectangle(0, 0, table.panel.Width, table.panel.Height));
                        R = 0;
                        G = 0;
                        B = 0;
                    }
                }
                else if (r > 0 && g <= 0 && b > 0)
                {
                    if (S.focus.R + (int)R >= remember.R && S.focus.G + (int)G <= remember.G && S.focus.B + (int)B >= remember.B)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, S.focus.G + (int)G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R >= remember.R && S.focus.G + (int)G <= remember.G)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, S.focus.G + (int)G, remember.B);

                    else if (S.focus.B + (int)B >= remember.B && S.focus.G + (int)G <= remember.G)

                        currT.BackColor = Color.FromArgb(remember.R, S.focus.G + (int)G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R >= remember.R && S.focus.B + (int)B >= remember.B)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, remember.G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R >= remember.R)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, remember.G, remember.B);

                    else if (S.focus.G + (int)G < remember.G)

                        currT.BackColor = Color.FromArgb(remember.R, S.focus.G + (int)G, remember.B);

                    else if (S.focus.B + (int)B >= remember.B)

                        currT.BackColor = Color.FromArgb(remember.R, remember.G, S.focus.B + (int)B);

                    else
                    {
                        if (table.panel.Parent != null)
                            table.panel.DrawToBitmap(copy, new Rectangle(0, 0, table.panel.Width, table.panel.Height));
                        R = 0;
                        G = 0;
                        B = 0;
                    }
                }
                else if (r > 0 && g > 0 && b <= 0)
                {
                    if (S.focus.R + (int)R >= remember.R && S.focus.G + (int)G >= remember.G && S.focus.B + (int)B <= remember.B)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, S.focus.G + (int)G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R >= remember.R && S.focus.G + (int)G >= remember.G)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, S.focus.G + (int)G, remember.B);

                    else if (S.focus.B + (int)B <= remember.B && S.focus.G + (int)G >= remember.G)

                        currT.BackColor = Color.FromArgb(remember.R, S.focus.G + (int)G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R >= remember.R && S.focus.B + (int)B <= remember.B)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, remember.G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R >= remember.R)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, remember.G, remember.B);

                    else if (S.focus.G + (int)G >= remember.G)

                        currT.BackColor = Color.FromArgb(remember.R, S.focus.G + (int)G, remember.B);

                    else if (S.focus.B + (int)B < remember.B)

                        currT.BackColor = Color.FromArgb(remember.R, remember.G, S.focus.B + (int)B);

                    else
                    {
                        if (table.panel.Parent != null)
                            table.panel.DrawToBitmap(copy, new Rectangle(0, 0, table.panel.Width, table.panel.Height));
                        R = 0;
                        G = 0;
                        B = 0;
                    }
                }
                else if (r > 0 && g > 0 && b > 0)
                {
                    if (S.focus.R + (int)R >= remember.R && S.focus.G + (int)G >= remember.G && S.focus.B + (int)B >= remember.B)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, S.focus.G + (int)G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R >= remember.R && S.focus.G + (int)G >= remember.G)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, S.focus.G + (int)G, remember.B);

                    else if (S.focus.B + (int)B >= remember.B && S.focus.G + (int)G >= remember.G)

                        currT.BackColor = Color.FromArgb(remember.R, S.focus.G + (int)G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R >= remember.R && S.focus.B + (int)B >= remember.B)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, remember.G, S.focus.B + (int)B);

                    else if (S.focus.R + (int)R >= remember.R)

                        currT.BackColor = Color.FromArgb(S.focus.R + (int)R, remember.G, remember.B);

                    else if (S.focus.G + (int)G >= remember.G)

                        currT.BackColor = Color.FromArgb(remember.R, S.focus.G + (int)G, remember.B);

                    else if (S.focus.B + (int)B >= remember.B)

                        currT.BackColor = Color.FromArgb(remember.R, remember.G, S.focus.B + (int)B);

                    else
                    {
                        if (table.panel.Parent != null)
                            table.panel.DrawToBitmap(copy, new Rectangle(0, 0, table.panel.Width, table.panel.Height));
                        R = 0;
                        G = 0;
                        B = 0;
                    }
                }
            }
        } 
        #endregion

        #region Button Events
        #region Solve Events
        public void Solve()
        {
            bool ok = true;
            for (int i = 0; i < table.panel.Controls.Count; i++)
            {
                if (table.panel.Controls[i].BackColor == S.error)
                {
                    S.mistakes.Show();
                    ok = false;
                    break;
                }
                else if(table.panel.Controls[i]==currT)
                    if (remember == S.error)
                    {
                        S.mistakes.Show();
                        ok = false;
                        break;
                    }
            }
            if (ok)
            {
                if (CanSolve(Input))
                {
                    if (Sudoku.SolveSudoku(Input))
                    {
                        FillSolution();
                        try
                        {
                            S.poc.Show();
                        }
                        catch
                        {
                            S.poc = new MyMessageBox(new Bitmap(Application.StartupPath + "\\poc.png"), "HA HA HA!");
                            S.poc.Show();
                        }
                    }
                }
                else
                {
                    try
                    {
                        S.checknumbers.Show();
                    }
                    catch
                    {
                        S.checknumbers = new MyMessageBox(new Bitmap(Application.StartupPath + "\\check.png"), "CHECK!");
                        S.checknumbers.Show();
                    }
                }
            }
        }
        private bool CanSolve(int[,] Input)
        {
            List<int> check = new List<int>();
            List<XYNum> oneOpt = new List<XYNum>();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if(Input[i,j]==0)
                    {
                        //fill list
                        for (int f = 1; f <= size; f++)
                            check.Add(f);
                        //empty row
                        for (int f = 0; f < size; f++)
                        {
                            if (Input[i,f]!=0 && f != j)
                                check.Remove(int.Parse(table.panel.Controls[i * size + f].Text));
                            if (check.Count == 0)
                                return false;                            
                        }
                        //empty column
                        for (int f = 0; f < size; f++)
                        {
                            if (Input[f, j] != 0 && f != i)
                                check.Remove(int.Parse(table.panel.Controls[f * size + j].Text));
                            if (check.Count == 0)
                                return false;                            
                        }
                        //empty cube
                        for (int f = (i / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size); f < (i / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size) + Math.Sqrt(size); f++)
                        {
                            for (int g = (j / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size); g < (j / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size) + Math.Sqrt(size); g++)
                            {
                                if (Input[f, g] != 0 && f != i && g != j)
                                    check.Remove(int.Parse(table.panel.Controls[f * size + g].Text));
                                if (check.Count == 0)
                                    return false;
                            }
                        }
                        if (check.Count>0 && check.Count <(int)Math.Sqrt(size)+1)                  
                            oneOpt.Add(new XYNum(i, j, check[0]));
                        check.Clear();
                    }
                }
            }
            //if (oneOpt.Count > 1)
            //    if (!checkOneOpt(oneOpt,Input))
            //        return false;
            return true;
        }        
        private bool checkOneOpt(List<XYNum>oneOpt,int[,]Input)
        {
            for (int i = 0; i < oneOpt.Count; i++)
            {
                for (int j = 0; j < oneOpt.Count; j++)
                {
                    if (oneOpt[i].option == oneOpt[j].option && i != j)
                    {
                        if (oneOpt[i].x == oneOpt[j].x)
                            return false;
                        else if (oneOpt[i].y == oneOpt[j].y)
                            return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region Clear Events
        public void Clear()
        {
            for (int i = 0; i < table.panel.Controls.Count; i++)
                ((TextBox)table.panel.Controls[i]).TextChanged -= new EventHandler(t_TextChanged);
            for (int i = 0; i < table.panel.Controls.Count; i++)
            {
                if (table.panel.Controls[i].BackColor != S.perm && table.panel.Controls[i] != currT)
                {
                    table.panel.Controls[i].BackColor = Color.White;
                    Input[i / size, i % size] = 0;
                    table.panel.Controls[i].Text = "";
                }
                else if (table.panel.Controls[i] == currT)
                {
                    if (remember != S.perm)
                    {
                        remember = Color.White;
                        Input[i / size, i % size] = 0;
                        currT.Text = "";
                    }
                }
            }
            createRGB(remember, S.focus);
            R = 0; G = 0; B = 0;
            for (int i = 0; i < table.panel.Controls.Count; i++)
                ((TextBox)table.panel.Controls[i]).TextChanged += new EventHandler(t_TextChanged);
            currT.Focus();
        }
        #endregion

        #region ClearAll Events
        public void ClearAll()
        {
            for (int i = 0; i < table.panel.Controls.Count; i++)
            {
                table.panel.Controls[i].Text = String.Empty;
                table.panel.Controls[i].BackColor = Color.White;
            }
            currT.Focus();
        }
        #endregion

        public void Snake()
        {
            for (int i = 0; i < table.panel.Controls.Count; i++)
            {
                ((TextBox)table.panel.Controls[i]).GotFocus -= new EventHandler(t_GotFocus);
                ((TextBox)table.panel.Controls[i]).LostFocus -= new EventHandler(t_LostFocus);
                ((TextBox)table.panel.Controls[i]).MouseClick -= new MouseEventHandler(t_MouseClick);
                ((TextBox)table.panel.Controls[i]).KeyDown -= new KeyEventHandler(t_KeyDown);
                ((TextBox)table.panel.Controls[i]).TextChanged -= new EventHandler(t_TextChanged);
                ((TextBox)table.panel.Controls[i]).KeyUp -= new KeyEventHandler(t_KeyUp);
            }
            ((SolverForm)s).AS();

            currT.Focus();
            snake = new Snake(this);
        }

        public void UnSnake()
        {
            snake.Kill();
            ((SolverForm)s).DS();

            currT = snake.currT;
            createRGB(remember, S.focus);
            R = 0; G = 0; B = 0;
            currT.Focus();
            S.HideCaret(currT);
            snake = null;

            Settings();

            timer.Start();
        }
        #endregion

        #region TextBox Settings
        private void Settings()
        {
            for (int i = 0; i < table.panel.Controls.Count; i++)
            {
                ((TextBox)table.panel.Controls[i]).GotFocus += new EventHandler(t_GotFocus);
                ((TextBox)table.panel.Controls[i]).LostFocus += new EventHandler(t_LostFocus);
                ((TextBox)table.panel.Controls[i]).MouseClick += new MouseEventHandler(t_MouseClick);
                ((TextBox)table.panel.Controls[i]).KeyDown += new KeyEventHandler(t_KeyDown);
                ((TextBox)table.panel.Controls[i]).TextChanged += new EventHandler(t_TextChanged);
                ((TextBox)table.panel.Controls[i]).KeyUp += new KeyEventHandler(t_KeyUp);
            }
        }
        #endregion        

        #region TextBox Events
        private void t_TextChanged(object sender, EventArgs e)
        {
            if (S.noTypeSound == false)
                S.type.Play();
            int a = table.panel.Controls.IndexOf((TextBox)sender);
            if (((TextBox)sender).Text == String.Empty)
            {
                Input[a / size, a % size] = 0;
                ScanAllInput(a / size, a % size);
                remember = Color.White;
            }
            else
            {
                try
                {
                    Input[a / size, a % size] = int.Parse(((TextBox)sender).Text);
                    if (Input[a / size, a % size] == 0)
                    {
                        ((TextBox)sender).Text = String.Empty;
                        S.fucku.Show();
                    }
                    else
                    {
                        ScanAllInput(a / size, a % size);
                        ScanInput(a / size, a % size);
                    }
                }
                catch
                {
                    ((TextBox)sender).Text = String.Empty;
                    MessageBox.Show("Numbers BIATCH!");
                }
            }
            createRGB(remember, S.focus);
            R = 0; G = 0; B = 0;
        }

        private void t_MouseClick(object sender, MouseEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        public void t_GotFocus(object sender, EventArgs e)
        {
            timer.Start();
            remember = ((TextBox)sender).BackColor;
            ((TextBox)sender).BackColor = S.focus;
            createRGB(remember, ((TextBox)sender).BackColor);
            currT = ((TextBox)sender);
            S.HideCaret(currT);
            ((TextBox)sender).SelectAll();
            R = 0; G = 0; B = 0;
        }

        public void t_LostFocus(object sender, EventArgs e)
        {
            timer.Stop();
            ((TextBox)sender).BackColor = remember;
        }

        private void t_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                S.walk.Play();
                if ((table.panel.Controls.IndexOf(((TextBox)sender)) + 1) % size == 0)
                    table.panel.Controls[(table.panel.Controls.IndexOf(((TextBox)sender)) + 1) - size].Focus();
                else
                    table.panel.Controls[table.panel.Controls.IndexOf(((TextBox)sender)) + 1].Focus();
            }
            else if (e.KeyCode == Keys.Left)
            {
                S.walk.Play();
                if (table.panel.Controls.IndexOf((TextBox)sender) % size == 0)
                    table.panel.Controls[table.panel.Controls.IndexOf((TextBox)sender) + size - 1].Focus();
                else
                    table.panel.Controls[table.panel.Controls.IndexOf((TextBox)sender) - 1].Focus();
            }
            else if (e.KeyCode == Keys.Down)
            {
                S.walk.Play();
                if (table.panel.Controls.IndexOf((TextBox)sender) + size >= Math.Pow(size, 2))
                    table.panel.Controls[table.panel.Controls.IndexOf((TextBox)sender) - size * (size - 1)].Focus();
                else
                    table.panel.Controls[table.panel.Controls.IndexOf((TextBox)sender) + size].Focus();
            }
            else if (e.KeyCode == Keys.Up)
            {
                S.walk.Play();
                if (table.panel.Controls.IndexOf((TextBox)sender) < size)
                    table.panel.Controls[table.panel.Controls.IndexOf((TextBox)sender) + size * (size - 1)].Focus();
                else
                    table.panel.Controls[table.panel.Controls.IndexOf((TextBox)sender) - size].Focus();
            }
            else if (
                (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) &&
                (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9) &&
                e.KeyCode != Keys.Back &&
                e.KeyCode != Keys.Delete)
                e.SuppressKeyPress = true;
        }

        private void t_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                e.SuppressKeyPress = true;
                Snake();
            }
        }
        #endregion

        #region Functions
        private void FillSolution()
        {
            for (int i = 0; i < table.panel.Controls.Count; i++)
            {
                ((TextBox)table.panel.Controls[i]).TextChanged -= new EventHandler(t_TextChanged);
                table.panel.Controls[i].Text = Input[i / size, i % size].ToString();
                ((TextBox)table.panel.Controls[i]).TextChanged += new EventHandler(t_TextChanged);
            }
        }

        private void createRGB(Color origC, Color newC)
        {
            r = (newC.R - origC.R) / 50f;
            g = (newC.G - origC.G) / 50f;
            b = (newC.B - origC.B) / 50f;
        }
        #endregion

        #region Mistakes Checker
        private void ScanAllInput(int currX, int currY)
        {
            bool z;
            for (int i = 0; i < Input.GetLength(0); i++)
            {
                for (int j = 0; j < Input.GetLength(1); j++)
                {
                    if (Input[i, j] != 0)
                    {
                        z = false;
                        if (Input[i, j] > size)
                            z = true;
                        //check row
                        for (int x = 0; x < size; x++)
                        {
                            if (Input[i, x] == Input[i, j] && x != j)
                            {
                                z = true;
                                break;
                            }
                        }

                        //check column
                        if (z == false)
                        {
                            for (int x = 0; x < size; x++)
                            {
                                if (Input[x, j] == Input[i, j] && x != i)
                                {
                                    z = true;
                                    break;
                                }
                            }
                        }

                        //check cube
                        if (z == false)
                        {
                            for (int x = (i / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size); x < (i / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size) + (int)Math.Sqrt(size); x++)
                            {
                                for (int y = (j / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size); y < (j / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size) + (int)Math.Sqrt(size); y++)
                                {
                                    if (Input[x, y] == Input[i, j] && x != i && y != j)
                                    {
                                        z = true;
                                        break;
                                    }
                                }
                                if (z) break;
                            }
                        }
                        if (z == false)
                        {
                            if (i == currX && j == currY)
                                remember = S.perm;
                            else
                                table.panel.Controls[i * size + j].BackColor = S.perm;
                        }
                    }
                }
            }
        }
        private void ScanInput(int i, int j)
        {
            bool z = false;
            if (Input[i, j] > size)
                z = true;
            //check row
            for (int x = 0; x < size; x++)
            {
                if (Input[i, x] == Input[i, j] && x != j)
                {
                    z = true;
                    table.panel.Controls[i * size + x].BackColor = S.error;
                }
            }

            //check column
            for (int x = 0; x < size; x++)
            {
                if (Input[x, j] == Input[i, j] && x != i)
                {
                    z = true;
                    table.panel.Controls[x * size + j].BackColor = S.error;
                }
            }

            //check cube
            for (int x = (i / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size); x < (i / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size) + (int)Math.Sqrt(size); x++)
            {
                for (int y = (j / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size); y < (j / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size) + (int)Math.Sqrt(size); y++)
                {
                    if (Input[x, y] == Input[i, j] && x != i && y != j)
                    {
                        z = true;
                        table.panel.Controls[x * size + y].BackColor = S.error;
                    }
                }
            }
            if (z)
                remember = S.error;
            else
                remember = S.perm;
        }
        #endregion
    }

    struct XYNum
    {
        public int x, y, option;
        public XYNum(int x, int y, int option)
        {
            this.x = x;
            this.y = y;
            this.option = option;
        }
    }
}
