using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace SudokuSnake
{
    class Sudoku
    {
        #region Data
        public int size, level;

        public int[,] sudoku;
        public int[,] amounts;
        private List<List<int>> nums;
        private bool bul = false;
        private Random rnd;
        #endregion

        #region Ctor
        public Sudoku(int size, int level)
        {
            this.size = size;
            this.level = level;
            FillSudoku();
        } 
        #endregion

        #region FillSudoku
        private bool FillSudoku()
        {
            sudoku = new int[(int)size, (int)size];
            rnd = new Random();
            int num, index = 0;
            nums = new List<List<int>>((int)size);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    nums.Add(new List<int>());
                    Narrow(i, j,index);
                    if (i != 0 && i != size - Math.Sqrt(size) && i % ((int)Math.Sqrt(size)) == 0)
                        SpecialCase(j, 0, i,index);

                    if (bul)
                    {
                        if (i < Math.Sqrt(size))
                        {
                            if(Swap(i, j))
                                Narrow(i, j, index);
                        }
                        else
                        {
                            if (i == size - 1)
                            {
                                nums = null;
                                sudoku = null;
                                FillSudoku();
                                return false;
                            }
                            j--;
                            if (j < 0)
                            {
                                j = size - 1;
                                i--;
                            }
                            nums.RemoveAt(index);
                            index--;
                            while (nums[index].Count == 0)
                            {
                                nums.RemoveAt(index);
                                index--;
                                j--;
                                if (j < 0)
                                {
                                    j = size - 1;
                                    i--;
                                }
                            }
                        }
                        bul = false;
                    }
                    try
                    {
                        num = nums[index][rnd.Next(0, nums[index].Count)];
                    }
                    catch
                    {
                        nums = null;
                        sudoku = null;
                        FillSudoku();
                        return false;
                    }
                    nums[index].Remove(num);
                    sudoku[i, j] = num;
                    index++;
                }
            }
            nums = null;
            Empty(); 
            return true;
        }

        private void FillList(int index)
        {
            nums[index].Clear();
            for (int n = 1; n <= size; n++)
                nums[index].Add(n);
        }

        private bool Swap(int i, int j)
        {
            bool g=false;
            List<int> tmp = new List<int>();
            for (int x = 1; x <= size; x++)
                tmp.Add(x);
            for (int x = 0; x < j; x++)
                tmp.Remove(sudoku[i, x]);
            int p = tmp[0];

            for (int z = 0; z < Math.Sqrt(size) - 1; z++)
            {
                g = true;
                for (int x = 0; x < i; x++)
                {
                    for (int y = z * (int)Math.Sqrt(size); y < z * (int)Math.Sqrt(size) + (int)Math.Sqrt(size); y++)
                    {
                        if (sudoku[x, y] == p)
                        {
                            g = false;
                            break;
                        }

                    }
                    if (g == false) break;
                }
                if (g)
                {
                    for (int a = z * (int)Math.Sqrt(size); a < z * (int)Math.Sqrt(size) + (int)Math.Sqrt(size); a++)
                    {
                        g = true;
                        for (int b = 0; b < i; b++)
                        {
                            for (int c = (j / (int)Math.Sqrt(size)) * (int)Math.Sqrt(size); c < (j / (int)Math.Sqrt(size)) * (int)Math.Sqrt(size) + (int)Math.Sqrt(size); c++)
                            {
                                if (sudoku[i, a] == sudoku[b, c])
                                {
                                    g = false;
                                    break;
                                }
                            }
                            if (g == false) break;
                        }
                        if (g)
                        {
                            sudoku[i, a] = p;
                            return g;
                        }
                    }
                }
                if (g) break;
            }
            return g;
        }

        private void SpecialCase(int j, int start, int end,int index)
        {
            List<int> once = new List<int>();
            for (int z = 1; z <= size; z++)
                once.Add(z);
            if ((j + 1) % Math.Sqrt(size) != 0)
            {
                for (int m = start; m < end; m++)
                    once.Remove(sudoku[m, j + 1]);
            }
            else
            {
                for (int m = start; m < end; m++)
                    once.Remove(sudoku[m, j - (int)Math.Sqrt(size) + 1]);
            }
            for (int f = 0; f < once.Count; f++)
            {
                nums[index].Remove(once[f]);
                if (nums[index].Count == 0)
                {
                    bul = true;
                    break;
                }
            }
        }

        private void Narrow(int i, int j,int index)
        {
            FillList(index);

            for (int r = 0; r < j; r++)
                nums[index].Remove(sudoku[i, r]);

            for (int r = 0; r < i; r++)
            {
                nums[index].Remove(sudoku[r, j]);
                if (nums[index].Count == 0)
                {
                    bul = true;
                    break;
                }
            }

            for (int r = (i / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size); r < i; r++)
            {
                for (int c = (j / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size); c < (j / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size) + Math.Sqrt(size); c++)
                {
                    nums[index].Remove(sudoku[r, c]);
                    if (nums[index].Count == 0)
                    {
                        bul = true;
                        break;
                    }
                }
                if (bul) break;
            }
        } 
        #endregion

        #region EmptySudoku
        private void Empty()
        {
            amounts = new int[(int)size, (int)size];
            List <int>opt = new List<int>();
            int Max, Min, total = 0, index, x, y;

            switch (level)
            {
                case 2:
                     Min = 35;Max=60;
                    break;
                case 3:
                     Min = 30;Max=50;
                    break;
                default:
                    Min = 40;Max=70;
                    break;
            }
            for (int i = 0; i < size/2; i++)
            {
                if (i > 0 && total < ((Max + Min) / 2) * size / 100)
                    total = (rnd.Next((Max + Min) / 2, Max) * size) / 100;
                else
                    total = (rnd.Next(Min, Max) * size) / 100;
                for (int g = 0; g < size; g++)
                    opt.Add(g);
                for (int g = 0; g < total; g++)
                {
                    index = rnd.Next(0, opt.Count);
                    x = i / (int)Math.Sqrt(size) * (int)Math.Sqrt(size) + opt[index] / (int)Math.Sqrt(size);
                    y = i % (int)Math.Sqrt(size) * (int)Math.Sqrt(size) + opt[index] % (int)Math.Sqrt(size);
                    opt.Remove(opt[index]);
                    amounts[x, y] = sudoku[x, y];
                    amounts[(int)size - 1 - x, (int)size - 1 - y] = sudoku[(int)size - 1 - x, (int)size - 1 - y];
                }
                opt.Clear();
            }

            if (size % 2 != 0)
            {
                if (total < ((Max + Min) / 2) * size / 100)
                    total = (rnd.Next((Max + Min) / 2, Max) * size) / 100;
                else
                    total = (rnd.Next(Min, Max) * size) / 100;
                for (int f = 0; f < total / 2; f++)
                {
                    x = rnd.Next((int)Math.Sqrt(size), size / 2 + 1);
                    if (x != size / 2)
                        y = rnd.Next((int)Math.Sqrt(size), (int)Math.Sqrt(size) + (int)Math.Sqrt(size));
                    else
                        y = rnd.Next((int)Math.Sqrt(size), size / 2 + 1);
                    if (amounts[x, y] == 0)
                    {
                        amounts[x, y] = sudoku[x, y];
                        amounts[(int)size - 1 - x, (int)size - 1 - y] = sudoku[(int)size - 1 - x, (int)size - 1 - y];
                    }
                    else f -= 1;
                }
            }
        }
        #endregion        

        #region SudokuSolver
        public static bool SolveSudoku(int[,] input)
        {
            int num, index = 0,size=input.GetLength(0);            
            bool bul = false, fromInputData;
            List<List<int>> nums = new List<List<int>>((int)size);
            List<Point> points = new List<Point>();
            List<int> once = new List<int>();
            List<Point> inputData = MakeInputData(input, size);
            Random rnd = new Random();

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    fromInputData = false;
                    if (input[i, j] != 0)
                    {
                        for (int p = 0; p < inputData.Count(); p++)
                        {
                            if (inputData[p].X == i && inputData[p].Y == j)
                            {
                                fromInputData = true;
                                break;
                            }
                        }
                    }
                    if (fromInputData == false)
                    {
                        points.Add(new Point(i, j));
                        nums.Add(new List<int>());
                        FillList2(index, nums, size);

                        for (int r = 0; r < size; r++)
                        {
                            if(r!=j)
                                nums[index].Remove(input[i, r]);
                        }

                        for (int r = 0; r < size; r++)
                        {
                            if (r != i)
                                nums[index].Remove(input[r, j]);
                            if (nums[index].Count == 0)
                            {
                                bul = true;
                                break;
                            }
                        }

                        for (int r = (i / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size); r < (i / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size) + (int)Math.Sqrt(size); r++)
                        {
                            for (int c = (j / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size); c < (j / (int)(Math.Sqrt(size))) * (int)Math.Sqrt(size) + Math.Sqrt(size); c++)
                            {
                                if(r!=i || c!=j)
                                    nums[index].Remove(input[r, c]);
                                if (nums[index].Count == 0)
                                {
                                    bul = true;
                                    break;
                                }
                            }
                            if (bul) break;
                        }

                        if (bul == true)
                        {
                            try
                            {
                                input[points[index].X, points[index].Y] = 0;
                                points.RemoveAt(index);
                                nums.RemoveAt(index);
                                index--;
                                while (nums[index].Count == 0)
                                {
                                    input[points[index].X, points[index].Y] = 0;
                                    points.RemoveAt(index);
                                    nums.RemoveAt(index);
                                    index--;
                                }
                                i = points[index].X;
                                j = points[index].Y;
                                bul = false;
                            }
                            catch { S.checknumbers.Show(); return false; }
                        }
                        num = nums[index][rnd.Next(0, nums[index].Count)];
                        nums[index].Remove(num);
                        input[i, j] = num;
                        index++;
                    }
                }
            }
            return true;
        }

        private static List<Point> MakeInputData(int[,] Input,int size)
        {
            List<Point> InputData = new List<Point>();
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (Input[i, j] != 0)
                        InputData.Add(new Point(i, j));
            return InputData;
        }

        private static void FillList2(int index, List<List<int>> nums, int size)
        {
            nums[index].Clear();
            for (int n = 1; n <= size; n++)
                nums[index].Add(n);
        }
        #endregion

        #region Get Methods
        public int[,] getSudoku()
        {
            return sudoku;
        }

        public int[,] getAmounts()
        {
            return amounts;
        } 
        #endregion
    }
}
