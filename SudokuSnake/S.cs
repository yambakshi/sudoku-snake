using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Runtime.InteropServices;
using MediaPlayer = System.Windows.Media.MediaPlayer;

namespace SudokuSnake
{
    static class S
    {
        #region MessageBoxes
        public static MyMessageBox fucku = new MyMessageBox(new Bitmap(Application.StartupPath + "\\fucku.png"), "FUCK U");
        public static MyMessageBox checknumbers = new MyMessageBox(new Bitmap(Application.StartupPath + "\\check.png"), "CHECK!");
        public static MyMessageBox mistakes = new MyMessageBox(new Bitmap(Application.StartupPath + "\\mistakes.png"), "MISTAKES BITCH!");
        public static MyMessageBox poc = new MyMessageBox(new Bitmap(Application.StartupPath + "\\poc.png"), "HA HA HA!"); 
        #endregion

        #region Colors
        public static Color error = Color.Red;
        public static Color perm = Color.Goldenrod;
        public static Color focus = Color.CornflowerBlue; 
        #endregion

        [DllImport("user32")]
        private static extern bool HideCaret(IntPtr hWnd);
        public static void HideCaret(TextBox curr)
        {
            HideCaret(curr.Handle);
        }
        public static void MouseMove(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Cursor = Cursors.Hand;
        }

        #region Sound
        //TrackBar
        public static Size SliderSize = new Size(150, 30);
        public static int caretX;
        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        //BackgroundMusic
        public static bool noTypeSound;        
        public static MediaPlayer m = new MediaPlayer();
        public static void m_MediaEnded(object sender, EventArgs e)
        {
            S.m.Position = new TimeSpan();
        }

        public static SoundPlayer type = new SoundPlayer(Application.StartupPath + "\\Sounds\\click.wav");
        public static SoundPlayer yes = new SoundPlayer(Application.StartupPath + "\\Sounds\\blip.wav");
        public static SoundPlayer walk = new SoundPlayer(Application.StartupPath + "\\Sounds\\blip2.wav");
        public static SoundPlayer but = new SoundPlayer(Application.StartupPath + "\\Sounds\\metal_button.wav");
        public static SoundPlayer mis = new SoundPlayer(Application.StartupPath + "\\Sounds\\mistake.wav");
        public static SoundPlayer punch = new SoundPlayer(Application.StartupPath + "\\Sounds\\PUNCH.wav");

        //Snake Sound
        public static SoundPlayer eat = new SoundPlayer(Application.StartupPath + "\\Sounds\\eat.wav");
        public static SoundPlayer snake = new SoundPlayer(Application.StartupPath + "\\Sounds\\snake.wav");

        //Cam Effect Sound
        public static SoundPlayer swoosh = new SoundPlayer(Application.StartupPath + "\\Sounds\\swoosh.wav");
        public static SoundPlayer swoosh_reverse = new SoundPlayer(Application.StartupPath + "\\Sounds\\swoosh_reverse.wav"); 
        #endregion

        #region Font Size
        public static Font GetFontForTextBoxHeight(int TextBoxHeight, Font OriginalFont)
        {
            // What is the target size of the textbox?
            float desiredheight = (float)TextBoxHeight;

            // Set the font from the existing TextBox font.
            // We use the fnt = new Font(...) method so we can ensure that
            //  we're setting the GraphicsUnit to Pixels.  This avoids all
            //  the DPI conversions between point & pixel.
            Font fnt = new Font(OriginalFont.FontFamily,
                                OriginalFont.Size,
                                OriginalFont.Style,
                                GraphicsUnit.Pixel);

            // TextBoxes never size below 8 pixels. This consists of the
            // 4 pixels above & 3 below of whitespace, and 1 pixel line of
            // greeked text.
            if (desiredheight < 8)
                desiredheight = 8;

            // Determine the Em sizes of the font and font line spacing
            // These values are constant for each font at the given font style.
            // and screen DPI.
            float FontEmSize = fnt.FontFamily.GetEmHeight(fnt.Style);
            float FontLineSpacing = fnt.FontFamily.GetLineSpacing(fnt.Style);

            // emSize is the target font size.  TextBoxes have a total of
            // 7 pixels above and below the FontHeight of the font.
            float emSize = (desiredheight - 7) * FontEmSize / FontLineSpacing;

            // Create the font, with the proper size.
            fnt = new Font(fnt.FontFamily, emSize, fnt.Style, GraphicsUnit.Pixel);

            return fnt;
        }
        #endregion        
    }
}
