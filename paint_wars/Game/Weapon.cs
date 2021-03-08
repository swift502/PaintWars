using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Game
{
    public partial class Form1 : Form
    {
        List<PointF[]> bullets = new List<PointF[]>();
        float weapon_timer;

        void Initialize_Weapon()
        {
            weapon_timer = 2;
        }

        void shoot()
        {
            if (weapon_timer * timeScale < 2) //Frekvence střelby
            {
                weapon_timer++;
            }
            else if(!dead)
            {
                PointF position = new PointF ( player_position.X, player_position.Y );
                PointF moment = new PointF((get_cursor().X - offset().X) - position.X,
                                             (get_cursor().Y - offset().Y) - position.Y);
                normalize(ref moment);

                float random_mul = 0.1f;
                for (int i = 0; i < 1; i++)
                {
                    //Random factor
                    float randomFloatX = (1 - ((float)(random.NextDouble()) * 2)) * random_mul;
                    float randomFloatY = (1 - ((float)(random.NextDouble()) * 2)) * random_mul;
                    PointF randomVector = new PointF ( randomFloatX, randomFloatY );

                    bullets.Add(new PointF[2] {new PointF(position.X, position.Y), new PointF(moment.X + randomVector.X, moment.Y + randomVector.Y)});
                }
                
                weapon_timer = 0;
            }
        }

        void bullets_movement()
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                PointF position = new PointF(bullets[i][0].X, bullets[i][0].Y);
                PointF moment = new PointF(bullets[i][1].X, bullets[i][1].Y);

                int index;
                if (world_collides(position.X + (moment.X * 30f * timeScale), position.Y + (moment.Y * 30f * timeScale)))
                {
                    paint_add(new PointF(position.X + (moment.X * 30f * timeScale), position.Y + (moment.Y * 30f * timeScale)), moment, ColorPlayer);

                    bool horizontal;
                    if (world_collides(position.X + (moment.X * 30f), position.Y)) horizontal = false;
                    else horizontal = true;

                    if(horizontal) blood_generate(position, new PointF(moment.X, -moment.Y), 3, 5, 0.5f, ColorPlayer);
                    else blood_generate(position, new PointF( -moment.X, moment.Y), 3, 3, 0.5f, ColorPlayer);

                    bullets.RemoveAt(i);
                }
                else if (X_collides_npc(position.X, position.Y, out index))
                {
                    npc_hurt(index, moment, 3, 10);

                    //Generate blood
                    blood_generate(position, moment, 10, 10, 0.2f, ColorEnemy);
                    blood_generate(position, new PointF(0, -10), 2, 1, 3f, ColorEnemy);

                    bullets.RemoveAt(i);

                    score_add(random.Next(10));
                }
                else
                {
                    //Add moment to position
                    position.X += moment.X * 30f * timeScale; //Rychlost střel
                    position.Y += moment.Y * 30f * timeScale;

                    //Update
                    bullets[i][0].X = position.X;
                    bullets[i][0].Y = position.Y;
                    bullets[i][1].X = moment.X;
                    bullets[i][1].Y = moment.Y;
                }
            }
        }

        void drawBullets(Graphics graphics)
        {
            foreach (PointF[] bullet in bullets)
            {
                if (isOnScreen(new Point((int)bullet[0].X, (int)bullet[0].Y)))
                {
                    graphics.FillEllipse(new SolidBrush(ColorPlayer), bullet[0].X - 5 + offset().X, bullet[0].Y - 5 + offset().Y, 15, 15);
                    graphics.DrawEllipse(new Pen(Color.Black, 2), bullet[0].X - 5 + offset().X, bullet[0].Y - 5 + offset().Y, 15, 15);
                }
            }
        }
    }
}
