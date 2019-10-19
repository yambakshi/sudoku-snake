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
    class Table
    {
        #region Data
        public Panel panel;
        public TextBox t;

        private SizeF textBoxSize;
        private int gap = 1;        
        private int size; 
        #endregion

        #region Ctor
        public Table(int size, Point start, Size s)
        {
            this.size = size;
            panel = new Panel();
            panel.Size = s;
            panel.Location = start;
            panel.BackColor = Color.Black;

            DrawTable();
        } 
        #endregion

        #region Draw Table
        private void DrawTable()
        {
            textBoxSize = new SizeF(
                (panel.Width - (float)((gap + 4) * 2 + (gap + 2) * (Math.Sqrt(size) - 1) + gap * (size - Math.Sqrt(size)))) / size,
                (panel.Height - (float)((gap + 4) * 2 + (gap + 2) * (Math.Sqrt(size) - 1) + gap * (size - Math.Sqrt(size)))) / size);

            float modus = textBoxSize.Width - (int)textBoxSize.Width;
            panel.Width = panel.Size.Width - (int)(modus * size);
            panel.Height = panel.Size.Height - (int)(modus * size);

            int plusx = 0, plusy = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (j > 0)
                    {
                        plusx += gap;
                        if (j % Math.Sqrt(size) == 0)
                            plusx += 2;
                    }

                    t = new TextBox();
                    t.TextAlign = HorizontalAlignment.Center;
                    t.Location = new Point((gap + 4) + j * (int)textBoxSize.Width + plusx, (gap + 4) + i * (int)textBoxSize.Height + plusy);
                    t.Width = (int)textBoxSize.Width;
                    t.Font = S.GetFontForTextBoxHeight((int)textBoxSize.Height, t.Font);
                    t.BackColor = Color.White;
                    panel.Controls.Add(t);
                }
                plusx = 0;
                plusy += gap;
                if (i > 0 && (i + 1) % Math.Sqrt(size) == 0)
                    plusy += 2;
            }
        } 
        #endregion
    }
}
