using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Game
{
        public partial class Form1 : Form
    {

        int score_multiplier;
        float score_size, score;
        StringFormat format = new StringFormat(StringFormatFlags.DirectionRightToLeft);
        Font levelfont, scorefont;
        int score_flash;

        void Initialize_Score()
        {
            score = 0;
            score_multiplier = 1;
            score_size = 0;
        }

        void score_add(int value)
        {
            score += value * score_multiplier ;
            if (score_size < 500) score_size += value / 2;
        }

        void score_draw(Graphics graphics)
        {
            scorefont = new Font(fontFamily, 32 + score_size, FontStyle.Bold, GraphicsUnit.Pixel);
            levelfont = new Font(fontFamily, 16, FontStyle.Bold, GraphicsUnit.Pixel);

            graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Black)), this.Size.Width - 135 - (int)(score_size * 2.5) - graphics.MeasureString(score.ToString(), font).Width, 15, graphics.MeasureString(score.ToString(), font).Width + 110 + (int)(score_size * 2.5), 45 + (int)score_size);
            graphics.DrawString(":Score", font, Brushes.White, new Point(this.Size.Width - 25 - (int)(score_size * 2.5) - (int)graphics.MeasureString(score.ToString(), font).Width, 20), format);
            graphics.DrawString(score.ToString(), scorefont, Brushes.White, new Point(this.Size.Width - 30, 20), format);

            if (score_size > 0 && !Menuu) score_size -= score_size / 10;

            graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Black)), this.Size.Width - graphics.MeasureString("Level: " + score_multiplier.ToString(), levelfont).Width - 33, 63 + (int)(score_size + 3), graphics.MeasureString(score_multiplier.ToString(), levelfont).Width + 60, 20);
            graphics.DrawString("Level: " + score_multiplier, levelfont, Brushes.White, new Point(this.Size.Width - 29, 63 + (int)((score_size + 3))), format);

            if (score_flash != 0)
            {
                Font flashFont = new Font(fontFamily, 32 + (score_flash / 2), FontStyle.Bold, GraphicsUnit.Pixel);
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(score_flash, Color.White)), 0, 0, this.Size.Width, this.Size.Height);
                graphics.DrawString(score_multiplier.ToString(), fontTitle, new SolidBrush(Color.FromArgb(score_flash, 220, 220, 220)), new Point((this.Size.Width / 2) - (int)graphics.MeasureString(score_multiplier.ToString(), fontTitle).Width / 2, (this.Size.Height / 2) - (int)graphics.MeasureString(score_multiplier.ToString(), fontTitle).Height / 2));
                if (!Menuu) score_flash -= 10;
            }
        }
    }
}
