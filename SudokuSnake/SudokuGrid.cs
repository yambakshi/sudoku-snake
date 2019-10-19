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
    delegate void UnSnake();
    delegate void Generate();
    delegate void Settings();
    class SudokuGrid
    {
        #region Data
        private Form s;
        private EventHandler LostFocus,GotFocus;
        public Generate gen;
        public Settings sett;
        public UnSnake unsnake;
        public Snake snake;
        public Table table;
        public Sudoku sudoku;
        private int sudokuSize, sudokuLevel;

        public Bitmap toPrint;

        private bool ClickedSolve;
                
        public int[,] Input;
        public int[,] amounts;

        public Color remember;
        private Timer timer;
        private float r, g, b;
        private float R, G, B;
        public TextBox currT;
        #endregion

        #region Ctor
        public SudokuGrid(int sudokuSize,int sudokuLevel,int x,int y,int width,int height,Form s)
        {
            this.s = s;
            this.sudokuSize = sudokuSize;
            this.sudokuLevel = sudokuLevel;
            table = new Table(sudokuSize, new Point(x, y), new Size(width, height));
            timer = new Timer();
            timer.Interval = 5;
            timer.Tick += new EventHandler(timer_Tick);

            sett = new Settings(Settings);
            gen = new Generate(Generate);
            unsnake = new UnSnake(UnSnake);
            
            Generate();
            Settings();
        }
        #endregion

        #region Fill Grid
        private void FillGrid()
        {
            for (int i = 0; i < sudoku.size; i++)
            {
                for (int j = 0; j < sudoku.size; j++)
                {
                    if (amounts[i, j] != 0)
                    {
                        table.panel.Controls[i * sudoku.size + j].BackColor = S.perm;
                        ((TextBox)table.panel.Controls[i * sudoku.size + j]).ReadOnly = true;
                        table.panel.Controls[i * sudoku.size + j].Text = sudoku.getSudoku()[i, j].ToString();
                    }
                    else
                    {
                        ((TextBox)table.panel.Controls[i * sudoku.size + j]).ReadOnly = false;
                        table.panel.Controls[i * sudoku.size + j].Text = "";
                        table.panel.Controls[i * sudoku.size + j].BackColor = Color.White;
                    }
                }
            }
        }
        #endregion

        #region Button Events
        public void Generate()
        {
            sudoku = new Sudoku(sudokuSize, sudokuLevel);
            Input = new int[sudoku.size, sudoku.size];
            amounts = sudoku.getAmounts();
            for (int x = 0; x < sudoku.size; x++)
                for (int y = 0; y < sudoku.size; y++)
                    Input[x, y] = amounts[x, y];

            S.noTypeSound = true;
            FillGrid();
            S.noTypeSound = false;

            toPrint = new Bitmap(table.panel.Width, table.panel.Height);
            table.panel.DrawToBitmap(toPrint, new Rectangle(0, 0, table.panel.Width, table.panel.Height));

            if (currT != null)
                remember = currT.BackColor;
            currT = (TextBox)table.panel.Controls[new Random().Next(0, table.panel.Controls.Count)];
            currT.Focus();
            ClickedSolve = false;
        }

        public void Clear()
        {
            LostFocus.Invoke(currT, new EventArgs());
            for (int i = 0; i < sudoku.size; i++)
            {
                for (int j = 0; j < sudoku.size; j++)
                {
                    if (amounts[i, j] == 0)
                    {
                        table.panel.Controls[i * sudoku.size + j].Text = String.Empty;
                        table.panel.Controls[i * sudoku.size + j].BackColor = Color.White;
                    }
                    else
                        table.panel.Controls[i * sudoku.size + j].BackColor= S.perm;
                }
            }
            currT.Focus();
            GotFocus.Invoke(currT, new EventArgs());
        }

        public void Solve()
        {
            ClickedSolve = true;
            LostFocus.Invoke(currT,new EventArgs());
            for (int i = 0; i < sudoku.size; i++)
            {
                for (int j = 0; j < sudoku.size; j++)
                {
                    if (amounts[i, j] == 0)
                    {
                        table.panel.Controls[i * sudoku.size + j].Text = sudoku.getSudoku()[i, j].ToString();
                        table.panel.Controls[i * sudoku.size + j].BackColor = Color.White;
                    }
                    else
                        table.panel.Controls[i * sudoku.size + j].BackColor = S.perm;
                }
            }
            currT.Focus();
            GotFocus.Invoke(currT,new EventArgs());
        }

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
            ((SudokuForm)s).AS();
            
            currT.Focus();
            snake=new Snake(this);
        }

        public void UnSnake()
        {
            snake.Kill();
            ((SudokuForm)s).DS();

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

        #region TextBox Events
        private void t_TextChanged(object sender, EventArgs e)
        {
            if(S.noTypeSound==false)
                S.type.Play();
            int a = table.panel.Controls.IndexOf((TextBox)sender);
            if (((TextBox)sender).Text == String.Empty)
            {
                Input[a / sudoku.size, a % sudoku.size] = 0;
                ScanAllInput(a / sudoku.size, a % sudoku.size);
                remember = Color.White;
            }
            else
            {
                try
                {
                    Input[a / sudoku.size, a % sudoku.size] = int.Parse(((TextBox)sender).Text);
                    if (Input[a / sudoku.size, a % sudoku.size] == 0)
                    {
                        ((TextBox)sender).Text = String.Empty;
                        S.fucku.Show();
                    }
                    else
                    {
                        ScanAllInput(a / sudoku.size, a % sudoku.size);
                        ScanInput(a / sudoku.size, a % sudoku.size);
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
            if (FullSolvedSudoku() && ClickedSolve == false)
            {
                ClickedSolve = true;
                ((SudokuForm)s).won.Invoke();
            }
        }

        private bool FullSolvedSudoku()
        {
            for (int i = 0; i < table.panel.Controls.Count; i++)
                if (table.panel.Controls[i].Text == "")
                    return false;
            for (int i = 0; i < table.panel.Controls.Count; i++)
                if (table.panel.Controls[i].BackColor == S.error)
                    return false;
            return true;
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
            createRGB(remember,((TextBox)sender).BackColor);
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
                if ((table.panel.Controls.IndexOf(((TextBox)sender)) + 1) % sudoku.size == 0)
                    table.panel.Controls[(table.panel.Controls.IndexOf(((TextBox)sender)) + 1) - sudoku.size].Focus();
                else
                    table.panel.Controls[table.panel.Controls.IndexOf(((TextBox)sender)) + 1].Focus();
            }
            else if (e.KeyCode == Keys.Left)
            {
                S.walk.Play();
                if (table.panel.Controls.IndexOf((TextBox)sender) % sudoku.size == 0)
                    table.panel.Controls[table.panel.Controls.IndexOf((TextBox)sender) + sudoku.size - 1].Focus();
                else
                    table.panel.Controls[table.panel.Controls.IndexOf((TextBox)sender) - 1].Focus();
            }
            else if (e.KeyCode == Keys.Down)
            {
                S.walk.Play();
                if (table.panel.Controls.IndexOf((TextBox)sender) + sudoku.size >= Math.Pow(sudoku.size, 2))
                    table.panel.Controls[table.panel.Controls.IndexOf((TextBox)sender) - sudoku.size * (sudoku.size - 1)].Focus();
                else
                    table.panel.Controls[table.panel.Controls.IndexOf((TextBox)sender) + sudoku.size].Focus();
            }
            else if (e.KeyCode == Keys.Up)
            {
                S.walk.Play();
                if (table.panel.Controls.IndexOf((TextBox)sender) < sudoku.size)
                    table.panel.Controls[table.panel.Controls.IndexOf((TextBox)sender) + sudoku.size * (sudoku.size - 1)].Focus();
                else
                    table.panel.Controls[table.panel.Controls.IndexOf((TextBox)sender) - sudoku.size].Focus();
            }
            else if(
                (e.KeyCode<Keys.D0 || e.KeyCode>Keys.D9)&&
                (e.KeyCode<Keys.NumPad0 || e.KeyCode>Keys.NumPad9) && 
                e.KeyCode!=Keys.Back &&
                e.KeyCode!=Keys.Delete)
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

        public void createRGB(Color origC, Color newC)
        {
            r = (newC.R - origC.R) / 50f;
            g = (newC.G - origC.G) / 50f;
            b = (newC.B - origC.B) / 50f;
        }

        public void timer_Tick(object sender, EventArgs e)
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
                        R = 0;
                        G = 0;
                        B = 0;
                    }
                }
            }
        }
        #endregion

        #region Mistakes Checker
        private void ScanAllInput(int currX,int currY)
        {
            bool z;
            for (int i = 0; i < Input.GetLength(0); i++)
            {
                for (int j = 0; j < Input.GetLength(1); j++)
                {
                    if (Input[i, j] != 0)
                    {
                        z = false;
                        if (Input[i, j] > sudoku.size)
                            z = true;
                        //check row
                        for (int x = 0; x < sudoku.size; x++)
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
                            for (int x = 0; x < sudoku.size; x++)
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
                            for (int x = (i / (int)(Math.Sqrt(sudoku.size))) * (int)Math.Sqrt(sudoku.size); x < (i / (int)(Math.Sqrt(sudoku.size))) * (int)Math.Sqrt(sudoku.size) + (int)Math.Sqrt(sudoku.size); x++)
                            {
                                for (int y = (j / (int)(Math.Sqrt(sudoku.size))) * (int)Math.Sqrt(sudoku.size); y < (j / (int)(Math.Sqrt(sudoku.size))) * (int)Math.Sqrt(sudoku.size) + (int)Math.Sqrt(sudoku.size); y++)
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
                            if (amounts[i, j] == 0)
                            {
                                if (i == currX && j == currY)
                                    remember = Color.White;
                                else
                                    table.panel.Controls[i * sudoku.size + j].BackColor = Color.White;
                            }
                            else
                                table.panel.Controls[i * sudoku.size + j].BackColor = S.perm;
                        }
                    }
                }
            }
        }
        private void ScanInput(int i, int j)
        {
            bool z = false;
            if (Input[i, j] > sudoku.size)
                z = true;
            //check row
            for (int x = 0; x < sudoku.size; x++)
            {
                if (Input[i, x] == Input[i, j] && x != j)
                {
                    z = true;
                    table.panel.Controls[i * sudoku.size+ x].BackColor = S.error;
                }
            }

            //check column
            for (int x = 0; x < sudoku.size; x++)
            {
                if (Input[x, j] == Input[i, j] && x != i)
                {
                    z = true;
                    table.panel.Controls[x * sudoku.size +j].BackColor = S.error;
                }
            }

            //check cube
            for (int x = (i / (int)(Math.Sqrt(sudoku.size))) * (int)Math.Sqrt(sudoku.size); x < (i / (int)(Math.Sqrt(sudoku.size))) * (int)Math.Sqrt(sudoku.size) + (int)Math.Sqrt(sudoku.size); x++)
            {
                for (int y = (j / (int)(Math.Sqrt(sudoku.size))) * (int)Math.Sqrt(sudoku.size); y < (j / (int)(Math.Sqrt(sudoku.size))) * (int)Math.Sqrt(sudoku.size) + (int)Math.Sqrt(sudoku.size); y++)
                {
                    if (Input[x, y] == Input[i, j] && x != i && y != j)
                    {
                        z = true;
                        table.panel.Controls[x * sudoku.size + y].BackColor = S.error;
                    }
                }
            }
            if (z)
                remember = S.error;
        }
        #endregion

        #region TextBox Settings
        private void Settings()
        {
            LostFocus = new EventHandler(t_LostFocus);
            GotFocus = new EventHandler(t_GotFocus);
            for (int i = 0; i < table.panel.Controls.Count; i++)
            {
                ((TextBox)table.panel.Controls[i]).GotFocus += GotFocus;
                ((TextBox)table.panel.Controls[i]).LostFocus += LostFocus;
                ((TextBox)table.panel.Controls[i]).MouseClick += new MouseEventHandler(t_MouseClick);
                ((TextBox)table.panel.Controls[i]).KeyDown += new KeyEventHandler(t_KeyDown);
                ((TextBox)table.panel.Controls[i]).KeyUp += new KeyEventHandler(t_KeyUp);
                ((TextBox)table.panel.Controls[i]).TextChanged += new EventHandler(t_TextChanged);
            }
        }
        #endregion
    }
}
