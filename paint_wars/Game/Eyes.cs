using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Game
{
    public partial class Form1 : Form
    {
        List<eye> enemy_eyes = new List<eye>();

        class eye
        {
            public PointF position;
            public PointF slow_target;

            public eye(float position_X, float position_Y)
            {
                position = new PointF(position_X, position_Y);
            }
        }

        void eye_draw(eye eye, int shape, PointF target, Graphics graphics) //0 = circle, 1 = box
        {
            if(!Menuu) eye.slow_target = slow(eye.slow_target, target, 5/timeScale);
            PointF direction = new PointF(eye.slow_target.X - eye.position.X, eye.slow_target.Y - eye.position.Y);
            normalize(ref direction);

            float distance = (float)Math.Sqrt(Math.Pow(eye.slow_target.X - eye.position.X, 2) + Math.Pow(eye.slow_target.Y - eye.position.Y, 2));
            if (distance > 100f) distance = 100f;
            float distance_reduction = 0.1f;

            int eye_size = 22;
            int eye_gap = 12;

            float eye_X = (eye.position.X - (eye_size / 2)) + (direction.X * distance * distance_reduction);
            float eye_Y = (eye.position.Y - (eye_size / 2)) + (direction.Y * distance * distance_reduction);

            float pupil_X = (eye.position.X - (eye_size / 4)) + (direction.X * 1.4f * distance * distance_reduction);
            float pupil_Y = (eye.position.Y - (eye_size / 4)) + (direction.Y * 1.4f * distance * distance_reduction);

            if (shape == 0)
            {
                //Left eye
                graphics.FillEllipse(Brushes.White,             eye_X - eye_gap + offset().X, eye_Y + offset().Y, eye_size, eye_size);
                graphics.DrawEllipse(new Pen(Brushes.Black, 2), eye_X - eye_gap + offset().X, eye_Y + offset().Y, eye_size, eye_size);
                //Pupil
                graphics.FillEllipse(Brushes.Black,             pupil_X - eye_gap + offset().X, pupil_Y + offset().Y, eye_size / 2, eye_size / 2);

                //Right eye
                graphics.FillEllipse(Brushes.White,             eye_X + eye_gap + offset().X, eye_Y + offset().Y, eye_size, eye_size);
                graphics.DrawEllipse(new Pen(Brushes.Black, 2), eye_X + eye_gap + offset().X, eye_Y + offset().Y, eye_size, eye_size);
                //Pupil
                graphics.FillEllipse(Brushes.Black,             pupil_X + eye_gap + offset().X, pupil_Y + offset().Y, eye_size / 2, eye_size / 2);
            }
            if (shape == 1)
            {
                //Left eye
                graphics.FillRectangle(Brushes.White, eye_X - eye_gap + offset().X, eye_Y + offset().Y, eye_size, eye_size);
                graphics.DrawRectangle(new Pen(Brushes.Black, 2), eye_X - eye_gap + offset().X, eye_Y + offset().Y, eye_size, eye_size);
                //Pupil
                graphics.FillRectangle(Brushes.Black, pupil_X - eye_gap + offset().X, pupil_Y + offset().Y, eye_size / 2, eye_size / 2);

                //Right eye
                graphics.FillRectangle(Brushes.White, eye_X + eye_gap + offset().X, eye_Y + offset().Y, eye_size, eye_size);
                graphics.DrawRectangle(new Pen(Brushes.Black, 2), eye_X + eye_gap + offset().X, eye_Y + offset().Y, eye_size, eye_size);
                //Pupil
                graphics.FillRectangle(Brushes.Black, pupil_X + eye_gap + offset().X, pupil_Y + offset().Y, eye_size / 2, eye_size / 2);
            }
        }
    }
}
