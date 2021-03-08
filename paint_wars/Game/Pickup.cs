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

        List<Pickup> pickups = new List<Pickup>();

        class Pickup
        {
            public int posX, posY;
            public float Float, sin;
            public float pulse;

            public Pickup(int position_X, int position_Y)
            {
                posX = position_X;
                posY = position_Y;

                pulse = 252;

                sin = 0;
            }

            public void floating(float timeScale)
            {
                Float = (float)Math.Sin(sin) * 10;
                if (sin < Math.PI * 2) sin += 0.03f * timeScale;
                else sin = 0;
            }
        }

        void addPickup(PointF position)
        {
            pickups.Add(new Pickup((int)position.X, (int)position.Y));
            if (pickups.Count > 100) pickups.RemoveAt(0);
        }

        void pickup_logic()
        {
            foreach (Pickup pickup in pickups)
            {
                pickup.floating(timeScale);

                pickup.pulse -= 4 * timeScale;
                if (pickup.pulse < 1) pickup.pulse = 252;
            }
        }

        void pickups_draw(Graphics graphics)
        {
            foreach (Pickup pickup in pickups)
            {
                if (isOnScreen(new Point(pickup.posX, pickup.posY)))
                {
                    float shift = (255 - pickup.pulse) / 8;
                    int size = 12;

                    graphics.FillEllipse(new SolidBrush(Color.FromArgb((int)pickup.pulse, ColorPlayer)), pickup.posX - (size / 2) - shift + offset().X,
                                                                                                    pickup.posY + pickup.Float - (size / 2) - shift + offset().Y,
                                                                                                    size + (2 * shift), size + (2 * shift));

                    graphics.FillEllipse(new SolidBrush(ColorPlayer), pickup.posX - (size / 2) + offset().X, pickup.posY + pickup.Float - (size / 2) + offset().Y, size, size);
                    graphics.DrawEllipse(new Pen(Color.FromArgb(20, 20, 20), 2), pickup.posX - (size / 2) + offset().X, pickup.posY + pickup.Float - (size / 2) + offset().Y, size, size);
                }
            }
        }

        void pickup_collected(PointF position)
        {
            for( int i =0; i < pickups.Count; i++)
            {
                if (position.X > pickups[i].posX - 30 && position.X < pickups[i].posX + 30 &&
                    position.Y > pickups[i].posY - 30 && position.Y < pickups[i].posY + 30)
                {
                    if ( health + 20 < 100) health += random.Next(20, 30);
                    else health += 100 - health;
                    healthBar.size += 30;
                    pickups.RemoveAt(i);
                }
            }
        }
    }
}
