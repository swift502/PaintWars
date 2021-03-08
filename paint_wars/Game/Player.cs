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
        PointF player_position;
        PointF player_moment, camera, latecamera;
        float lateCursorX, lateCursorY;
        bool mouse_LMB, dead;
        bool OnGround;
        int health;
        HealthBar healthBar;
        int player_timer;
        eye player_eye;

        bool god_mode;

        void Initialize_Player()
        {
            player_position.X = 400;
            player_position.Y = 280;
            camera.X = player_position.X;
            camera.Y = player_position.Y;
            health = 100;
            healthBar = new HealthBar(20, 20, 200, 30);
            player_timer = 0;

            player_eye = new eye(player_position.X, player_position.Y);

            god_mode = false;
        }

        void player_move()
        {
            if (!dead)
            {
                //Calculate moments

                zombie_attacks_player();

                //X
                if (key_left) player_moment.X += 1f;
                if (key_right) player_moment.X -= 1f;
                player_moment.X -= player_moment.X / 8;

                //Y
                player_moment.Y += 0.5f * timeScale;
                if (key_up && OnGround) player_moment.Y = -15;
                //if (key_up) moment.Y -= 1F;
                //if (key_down) moment.Y += 1F;
                //moment.Y -= moment.Y / 8;

                //Add moments to position
                //X
                player_position.X -= player_moment.X * timeScale;
                if (player_collides_X())
                {
                    player_position.X += player_moment.X * timeScale;
                    player_moment.X -= (player_moment.X * timeScale) / 2;
                }

                //Y
                player_position.Y += player_moment.Y * timeScale;
                if (player_collides_X())
                {
                    if (player_moment.Y > 0) OnGround = true;
                    player_position.Y -= player_moment.Y * timeScale;
                    player_moment.Y -= (player_moment.Y * timeScale) / 2;
                }
                else OnGround = false;
                
                //pickup
                pickup_collected(player_position);

                //eyes
                player_eye.position.X = player_position.X;
                player_eye.position.Y = player_position.Y - 5;
                //player_eye.slow_target = slow(player_eye.slow_target, new PointF(get_cursor().X - offset().X, get_cursor().Y - offset().Y), 5 / timeScale);
            }
            else
            {
                if (player_timer == 100)
                {
                    player_timer = 0;
                    enableMenu(3);
                    Cursor.Show();
                }
                else
                {
                    player_timer++;
                }
            }
        }

        void player_hurt(PointF direction, float force)
        {
            player_moment.X += direction.X / 3 * force;
            player_moment.Y -= direction.Y / 9 * force;

            if(!god_mode) health -= 5;

            healthBar.size += force* (100 - health);

            if (health <= 0)
            {
                dead = true;
                npc_timeReset();
                blood_generate(player_position, new PointF(0, -1), 200, 8, 1f, ColorPlayer);
            }
        }

        class HealthBar
        {
            int posX, posY, sizeX, sizeY;
            public float size;

            public HealthBar(int positionX, int positionY, int width, int height)
            {
                posX = positionX;
                posY = positionY;
                sizeX = width;
                sizeY = height;
                size = 0;
            }

            public void draw(Graphics graphics, Color ColorPlayer, float player_health)
            {
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.Black)), posX, posY, sizeX + size, sizeY + size);
                graphics.DrawRectangle(new Pen(Brushes.Black, 4), posX, posY, sizeX + size, sizeY + size);
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, ColorPlayer)), posX + 2, posY + 2, ((sizeX - 4) * player_health / 100) + (((float)player_health / 100) * size), (sizeY - 4) + size);
                size -= size / 10;
            }
        }

        void zombie_attacks_player()
        {
            int npc_index;
            bool hit;

            //v1
            if (X_collides_npc(player_position.X + 30, player_position.Y + 30, out npc_index)) hit = true;
            //v2
            else if (X_collides_npc(player_position.X - 30, player_position.Y + 30, out npc_index)) hit = true;
            //v3
            else if (X_collides_npc(player_position.X + 30, player_position.Y - 30, out npc_index)) hit = true;
            //v4
            else if (X_collides_npc(player_position.X - 30, player_position.Y - 30, out npc_index)) hit = true;

            else hit = false;

            if (hit)
            {
                player_hurt(new PointF(activeNPCs[npc_index].position.X - player_position.X, activeNPCs[npc_index].position.Y - player_position.Y), 1);
                //Generate blood
                blood_generate(new PointF(player_position.X, player_position.Y), new PointF(player_position.X - activeNPCs[npc_index].position.X - player_moment.X * 10, player_position.Y - activeNPCs[npc_index].position.Y - 20 + player_moment.Y * 10), 30, 10, 0.7f, ColorPlayer);

                npc_hurt(npc_index, new PointF((float)activeNPCs[npc_index].position.X - player_position.X, (float)activeNPCs[npc_index].position.Y - player_position.Y), 0.2f, 0);
            }
        }

        bool player_collides_X()
        {
            //v1
                 if (world_collides(player_position.X - 30, player_position.Y + 30)) return true;
            //v2
            else if (world_collides(player_position.X + 30, player_position.Y + 30)) return true;
            //v3
            else if (world_collides(player_position.X + 30, player_position.Y - 30)) return true;
            //v4
            else if (world_collides(player_position.X - 30, player_position.Y - 30)) return true;
            else return false;
        }

        void camera_Movement()
        {
            PointF cursor = get_cursor();

            latecamera.X = slow(latecamera.X, player_position.X, 10);
            latecamera.Y = slow(latecamera.Y, player_position.Y, 7);

            camera.X = latecamera.X;
            camera.Y = latecamera.Y;

            lateCursorX = slow(lateCursorX, cursor.X, 10);
            lateCursorY = slow(lateCursorY, cursor.Y, 10);

            float cursor_deltaX = lateCursorX - player_position.X - offset().X;
            float cursor_deltaY = lateCursorY - player_position.Y - offset().Y;

            camera.X += cursor_deltaX * 0.4f;
            camera.Y += cursor_deltaY * 0.7f;
        }

        void player_draw(Graphics graphics)
        {
            if (!dead)
            {
                graphics.FillEllipse(new SolidBrush(ColorPlayer), player_position.X - 30 + offset().X, player_position.Y - 30 + offset().Y, 60, 60);
                graphics.DrawEllipse(new Pen(Color.Black, 3), player_position.X - 30 + offset().X, player_position.Y - 30 + offset().Y, 60, 60);

                eye_draw(player_eye, 0, new PointF(get_cursor().X - offset().X, get_cursor().Y - offset().Y), graphics);
            }
        }
    }
}

