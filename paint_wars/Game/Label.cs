using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Game
{
    public partial class Form1 : Form
    {
        List<Label> labels = new List<Label>();

        void Initialize_label()
        {
        }

        void label_add(string text)
        {
            labels.Add(new Label(text, 100, ""));
        }

        void label_add(string text, int screen_time)
        {
            labels.Add(new Label(text, screen_time, ""));
        }

        void label_add(string text, int screen_time, string tag)
        {
            labels.Add(new Label(text, screen_time, tag));
        }

        class Label
        {
            public string text;
            public float fade, shift;
            public int target_fade, target_shift;
            public int timer;
            public float late_i;
            public string tag;

            public Label(string Displayed_text, int screen_time, string tagp)
            {
                text = Displayed_text;

                fade = 0;
                target_fade = 255;

                shift = 10;
                target_shift = 0;

                timer = screen_time;

                tag = tagp;
            }
        }

        void label_draw(Graphics graphics)
        {
            for (int i = 0; i < labels.Count; i++)
            {
                Label label = labels[i];

                SizeF text = new SizeF(graphics.MeasureString(label.text, levelfont));

                label.fade = slow(label.fade, label.target_fade, 10);
                label.shift = slow(label.shift, label.target_shift, 5);
                label.late_i = slow(label.late_i, i, 5);

                graphics.FillRectangle(new SolidBrush(Color.FromArgb((int)label.fade / 2, Color.Black)), 0, 560 - ((label.late_i + label.shift + 1) * 30), text.Width + 10, 30);
                graphics.DrawString(label.text, levelfont, new SolidBrush(Color.FromArgb((int)label.fade, Color.White)), 5, 560 + 5 - ((label.late_i + label.shift + 1) * 30));

                label.timer--;

                if (label.timer == 20)
                {
                    label.target_fade = 0;
                    label.target_shift = -1;
                }

                if (label.timer == 0)
                {
                    labels.RemoveAt(i);
                }
                else
                {
                    labels[i] = label;
                }
            }
        }
    }
}
