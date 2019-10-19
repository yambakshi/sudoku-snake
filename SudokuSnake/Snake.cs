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
    class Snake
    {
        #region Data
        private SudokuGrid sudokuG;
        private SolverGrid solverG;

        private List<int> indices;
        private List<Color> ColorList;
        public Timer snake;
        private int SnakeSpeed = 100, SnakeStartLength = 3,SnakeStartIndex,StartI,prev,last;
        private Color SnakeColor = Color.Green;
        private bool Start;
        public bool EnableKeys;
        private bool leftright = false,
                     updown = true,
                     direction = false;

        private Table table;
        private int size;
        public TextBox currT;
        private int[,] input;
        private int[,] sudoku;
        private int[,] amounts;
        public Color prevColor,remember;
        #endregion

        #region Ctor
        public Snake(SudokuGrid s)
        {
            S.snake.Play();
            this.sudokuG = s;
            this.table = s.table;
            this.size = s.sudoku.size;
            this.currT = s.currT;
            this.remember = s.remember;
            this.amounts = s.amounts;
            this.input = s.Input;
            this.sudoku = s.sudoku.getSudoku();

            indices = new List<int>();
            ColorList = new List<Color>();

            if (table.panel.Controls.IndexOf(s.currT) / size == size - 1)
                SnakeStartIndex = 0;
            else
                SnakeStartIndex = (table.panel.Controls.IndexOf(currT) / size) * size + size;

            StartLength();
            Start = true;
            StartI = 0;
            prevColor = s.remember;
            S.HideCaret(currT);

            Settings();
            snake = new Timer();
            snake.Interval = SnakeSpeed;
            snake.Tick += new EventHandler(SnakeTimer_Tick);
            snake.Start();
        }

        public Snake(SolverGrid s)
        {
            S.snake.Play();
            this.solverG = s;
            this.table = s.table;
            this.size = s.size;
            this.currT = s.currT;
            this.remember = s.remember;

            indices = new List<int>();
            ColorList = new List<Color>();

            if (table.panel.Controls.IndexOf(s.currT) / size == size - 1)
                SnakeStartIndex = 0;
            else
                SnakeStartIndex = (table.panel.Controls.IndexOf(currT) / size) * size + size;

            StartLength();
            StartI = 0;
            Start = true;
            prevColor = s.remember;
            S.HideCaret(currT);

            Settings();
            snake = new Timer();
            snake.Interval = SnakeSpeed;
            snake.Tick += new EventHandler(SnakeTimer_Tick);
            snake.Start();
        } 
        #endregion

        #region Start Length
        private void StartLength()
        {
            for (int i = 0; i < SnakeStartLength; i++)
                indices.Add((SnakeStartIndex + SnakeStartLength - 1) - i);
        } 
        #endregion

        #region Timer
        void SnakeTimer_Tick(object sender, EventArgs e)
        {
            if (Start)
            {
                ColorList.Add(table.panel.Controls[indices[indices.Count-1-StartI]].BackColor);
                table.panel.Controls[indices[indices.Count - 1 - StartI]].BackColor = SnakeColor;
                StartI++;
                if (StartI >= indices.Count)
                {
                    Start = false;
                    EnableKeys = true;
                    ColorList.Reverse();
                }
            }
            else
             {
                prev = indices[0];
                //Right
                if (leftright == false && direction == false)
                {
                    if ((indices[0] + 1) % size == 0)
                        indices[0] = indices[0] - size + 1;
                    else
                        indices[0]++;
                }
                //Left
                else if (leftright == false && direction == true)
                {
                    if ((indices[0] - 1) < 0 || indices[0] % size == 0)
                        indices[0] = indices[0] + size - 1;
                    else
                        indices[0]--;
                }
                //Down
                else if (updown == false && direction == true)
                {
                    if (indices[0] + size >= table.panel.Controls.Count)
                        indices[0] = indices[0] - size * (size - 1);
                    else
                        indices[0] += size;
                }
                //Up
                else if (updown == false && direction == false)
                {
                    if (indices[0] < size)
                        indices[0] = indices[0] + size * (size - 1);
                    else
                        indices[0] -= size;
                }       
                
                //Strike
                if (table.panel.Controls[indices[0]].BackColor == SnakeColor)
                {
                    indices[0] = prev;
                    Strike();
                }
                else
                {
                    ColorList.Reverse();
                    ColorList.Add(table.panel.Controls[indices[0]].BackColor);
                    ColorList.Reverse();

                    table.panel.Controls[indices[0]].BackColor = SnakeColor;
                    UpdateIndices(prev);
                }
                
                //Food Encounter
                if (table.panel.Controls.IndexOf(currT) == indices[0])
                {
                    S.eat.Play();
                    indices.Add(last);
                    ColorList[0] = remember;
                    ColorList.Add(table.panel.Controls[last].BackColor);
                    NewFood();
                    if (sudokuG != null)
                    {
                        ((SudokuForm)sudokuG.table.panel.Parent).US(true, 10);
                        remember = sudokuG.remember;
                    }
                    else
                    {
                        ((SolverForm)solverG.table.panel.Parent).US(true, 10);
                        remember = solverG.remember;
                    }
                }
            }
        } 
        #endregion

        #region Func
        private void UpdateIndices(int prev)
        {
            for (int i = 1; i <=indices.Count-1; i++)
            {
                last = indices[i];
                indices[i] = prev;
                prev = last;
                table.panel.Controls[indices[i]].BackColor = SnakeColor;
            }
            table.panel.Controls[last].BackColor = ColorList[ColorList.Count - 1];
            ColorList.RemoveAt(ColorList.Count - 1);
        }
        private void NewFood()
        {
            int rnd = new Random().Next(0, table.panel.Controls.Count);
            bool again = false;
            while (again == false)
            {
                again = true;
                if (rnd != indices[0])
                {
                    for (int i = 0; i < indices.Count; i++)
                    {
                        if (rnd == indices[i])
                        {
                            again = false;
                            rnd = new Random().Next(0, table.panel.Controls.Count);
                            break;
                        }
                    }
                }
                else
                {
                    again = false;
                    rnd = new Random().Next(0, table.panel.Controls.Count);
                }
            }
            if (sudokuG != null)
            {
                if (currT.Text == String.Empty)
                {
                    currT.Text = sudoku[table.panel.Controls.IndexOf(currT) / size, table.panel.Controls.IndexOf(currT) % size].ToString();
                    currT.ReadOnly = true;
                    input[table.panel.Controls.IndexOf(currT) / size, table.panel.Controls.IndexOf(currT) % size] = int.Parse(currT.Text);
                }
                sudokuG.t_LostFocus(currT, new EventArgs());
                currT = (TextBox)table.panel.Controls[rnd];
                sudokuG.t_GotFocus(currT, new EventArgs());
            }
            else
            {
                solverG.t_LostFocus(currT, new EventArgs());
                currT = (TextBox)table.panel.Controls[rnd];
                solverG.t_GotFocus(currT, new EventArgs());
            }
        }
        #endregion

        #region Settings
        private void Settings()
        {
            for (int i = 0; i < table.panel.Controls.Count; i++)
            {
                table.panel.Controls[i].KeyDown += new KeyEventHandler(Snake_KeyDown);
                table.panel.Controls[i].KeyUp += new KeyEventHandler(Snake_KeyUp);
                table.panel.Controls[i].MouseClick += new MouseEventHandler(Snake_MouseClick);
            }
        }
        #endregion

        #region Events
        private void Snake_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    
                    if (EnableKeys)
                    {
                        if (updown)
                        {
                            direction = true;
                            updown = false;
                            leftright = true;
                        }
                    }
                    break;
                case Keys.Up:
                    if (EnableKeys)
                    {
                        if (updown)
                        {
                            direction = false;
                            updown = false;
                            leftright = true;
                        }
                    }
                    break;
                case Keys.Left:
                    if (EnableKeys)
                    {
                        if (leftright == true)
                        {
                            direction = true;
                            leftright = false;
                            updown = true;
                        }
                    }
                    break;
                case Keys.Right:
                    if (EnableKeys)
                    {
                        if (leftright == true)
                        {
                            direction = false;
                            leftright = false;
                            updown = true;
                        }
                    }
                    break;
                default:
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        void Snake_KeyUp(object sender, KeyEventArgs e)
        {
            if (Start==false)
            {
                if (e.KeyCode == Keys.Space)
                {
                    e.SuppressKeyPress = true;
                    if (sudokuG != null)
                        sudokuG.unsnake.Invoke();
                    else
                        solverG.unsnake.Invoke();
                }
            }
        }

        private void Snake_MouseClick(object sender, MouseEventArgs e)
        {
            S.HideCaret((TextBox)sender);
        }
        #endregion

        #region Dtor
        public void Kill()
        {
            snake.Stop();
            for (int i = indices.Count-1; i >=0; i--)
            {
                Thread.Sleep(20);
                if(sudokuG!=null)
                    ((SudokuForm)sudokuG.table.panel.Parent).US(false, 10);
                else
                    ((SolverForm)solverG.table.panel.Parent).US(false, 10);
                table.panel.Controls[indices[i]].BackColor = ColorList[i];
                Application.DoEvents();
            }
            for (int i = 0; i < table.panel.Controls.Count; i++)
            {
                table.panel.Controls[i].KeyDown -= new KeyEventHandler(Snake_KeyDown);
                table.panel.Controls[i].KeyUp -= new KeyEventHandler(Snake_KeyUp);
                table.panel.Controls[i].MouseClick -= new MouseEventHandler(Snake_MouseClick);

                if (sudokuG != null)
                    if (amounts[i / size, i % size] == 0)
                    ((TextBox)table.panel.Controls[i]).ReadOnly = false;
            }
        }
        private void Strike()
        {
            snake.Stop();

            for (int i = 0; i < table.panel.Controls.Count; i++)
            {
                table.panel.Controls[i].KeyDown -= new KeyEventHandler(Snake_KeyDown);
                table.panel.Controls[i].KeyUp -= new KeyEventHandler(Snake_KeyUp);
                table.panel.Controls[i].MouseClick -= new MouseEventHandler(Snake_MouseClick);

                if(sudokuG!=null)
                    if (amounts[i / size, i % size] == 0)
                        ((TextBox)table.panel.Controls[i]).ReadOnly = false;                    

            }
            if (sudokuG != null)
                ((SudokuForm)sudokuG.table.panel.Parent).striked();
            else
                ((SolverForm)solverG.table.panel.Parent).striked();
        }
        #endregion
    }
}
