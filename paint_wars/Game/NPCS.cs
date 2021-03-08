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
        List<npc> activeNPCs = new List<npc>();
        int npc_timer, npc_time;
        bool spawning = true;

        void Initialize_NPCS()
        {
            npc_timer = 0;
            npc_timeReset();
        }

        class npc
        {
            public PointF position;
            public PointF moment;
            public int health;

            public npc(PointF positionP, PointF momentP, int healthP)
            {
                position = positionP;
                moment = momentP;
                health = healthP;
            }
        }

        void npc_timeReset()
        {
            npc_time = 80;
        }

        void npc_add( float positionX, float positionY)
        {                                          //Position                     //Moment
            activeNPCs.Add(new npc ( new PointF( positionX, positionY), new PointF( 0, 0 ), 100));
            enemy_eyes.Add(new eye(positionX, positionY - 7));
            if (activeNPCs.Count > 300)
            {
                activeNPCs.RemoveAt(0);
                enemy_eyes.RemoveAt(0);
            }
        }

        void npc_spawn()
        {
            if (npc_timer * timeScale < npc_time)
            {
                npc_timer++;
            }
            else if(spawning)
            {
                //int margin = 1600;
                float level = coordinate_To_Scene(player_position.Y, 1);
                int world = coordinate_To_World(player_position.Y, 1);
                int sign = random.Next(2);
                //if (sign > 0) sign = 1;
                //else sign = -1;
                bool spawned = false;

                while (!spawned)
                {
                    if (sign == 0)
                    {
                        for (int i = 80; i > 0; i--)
                        {
                            if (!world_collides((camera.X + 460) + 30, (world * 10 * 8 * 80) + (level * 8 * 80 + (i * 8) + 30)) &&
                                !world_collides((camera.X + 460) - 30, (world * 10 * 8 * 80) + (level * 8 * 80 + (i * 8) + 30)) &&
                                !world_collides((camera.X + 460) + 30, (world * 10 * 8 * 80) + (level * 8 * 80 + (i * 8) - 30)) &&
                                !world_collides((camera.X + 460) - 30, (world * 10 * 8 * 80) + (level * 8 * 80 + (i * 8) - 30)))
                            {
                                npc_add((camera.X + 460), (world * 10 * 8 * 80) + (level * 8 * 80 + (i * 8)));
                                spawned = true;
                            }
                            if (spawned) break;
                        }
                    }
                    else
                    {
                        for (int i = 80; i > 0; i--)
                        {
                            if (!world_collides((camera.X - 460) + 30, (world * 10 * 8 * 80) + (level * 8 * 80 + (i * 8) + 30)) &&
                                !world_collides((camera.X - 460) - 30, (world * 10 * 8 * 80) + (level * 8 * 80 + (i * 8) + 30)) &&
                                !world_collides((camera.X - 460) + 30, (world * 10 * 8 * 80) + (level * 8 * 80 + (i * 8) - 30)) &&
                                !world_collides((camera.X - 460) - 30, (world * 10 * 8 * 80) + (level * 8 * 80 + (i * 8) - 30)))
                            {
                                npc_add((camera.X - 460), (world * 10 * 8 * 80) + (level * 8 * 80 + (i * 8)));
                                spawned = true;
                            }
                            if (spawned) break;
                        }
                    }
                }
                npc_timer = 0;
            }
        }

        void npc_move()
        {
            for (int i = 0; i < activeNPCs.Count; i++)
            {
                PointF NPCposition = new PointF(activeNPCs.ElementAt(i).position.X, activeNPCs.ElementAt(i).position.Y);
                PointF moment = new PointF(activeNPCs.ElementAt(i).moment.X, activeNPCs.ElementAt(i).moment.Y);
                bool OnGround = false;

                PointF chasePlayerVector = new PointF(player_position.X - NPCposition.X, player_position.Y - NPCposition.Y);

                if (chasePlayerVector.X != 0) normalize(ref chasePlayerVector);

                //Y
                //Gravity
                moment.Y += 0.5F * timeScale;

                //Add moment to position
                NPCposition.Y += moment.Y * timeScale;
                if (npc_collides_X(NPCposition))
                {
                    if (moment.Y > 0) OnGround = true;
                    NPCposition.Y -= moment.Y * timeScale;
                    moment.Y -= (moment.Y * timeScale) / 2;
                }
                else OnGround = false;

                //X
                if (Math.Abs(moment.X) * ((Math.Sign(moment.X)) * (Math.Sign(chasePlayerVector.X))) < 3)
                {
                    moment.X += (chasePlayerVector.X * timeScale) / 2;
                }
                else moment.X -= moment.X * 0.01f;

                //Add moment to position
                NPCposition.X += moment.X * timeScale;
                if (npc_collides_X(NPCposition))
                {
                    NPCposition.X -= moment.X * timeScale;
                    moment.X -= (moment.X * timeScale) / 2;

                    //If you collide on X and you're on the ground - jump
                    if (OnGround && Math.Abs(moment.X) * ((Math.Sign(moment.X)) * (Math.Sign(chasePlayerVector.X))) > 0) moment.Y = -10;
                }


                //Update values
                activeNPCs[i] = new npc ( new PointF(NPCposition.X, NPCposition.Y),
                                                new PointF(moment.X, moment.Y),
                                                activeNPCs[i].health);

                for (int e = 0; e < enemy_eyes.Count; e++)
                {
                    enemy_eyes[e].position.X = activeNPCs[e].position.X;
                    enemy_eyes[e].position.Y = activeNPCs[e].position.Y - 7;
                }
            }
        }

        void npc_hurt(int index, PointF direction, float force, int takeHealth)
        {
            activeNPCs[index] = new npc ( new PointF(activeNPCs[index].position.X, activeNPCs[index].position.Y),
                                                new PointF(activeNPCs[index].moment.X + direction.X * force, activeNPCs[index].moment.Y + direction.Y * force),
                                                activeNPCs[index].health - takeHealth);

            if (activeNPCs[index].health <= 0)
            {
                npc_die(index);
            }
        }

        void npc_die(int index)
        {
            blood_generate(activeNPCs[index].position, new PointF(0, -10), 50, 5, 1f, ColorEnemy);

            if (random.Next(3) == 0)
            {
                addPickup(activeNPCs[index].position);
            }

            activeNPCs.RemoveAt(index);
            enemy_eyes.RemoveAt(index);
            score_add(random.Next(30, 100));

        }

        bool npc_collides_X(PointF position)
        {
            //v1
            if (world_collides(position.X + 30, position.Y + 30)) return true;
            //v2
            else if (world_collides(position.X - 30, position.Y + 30)) return true;
            //v3
            else if (world_collides(position.X + 30, position.Y - 30)) return true;
            //v4
            else if (world_collides(position.X - 30, position.Y - 30)) return true;

            else return false;
        }

        bool X_collides_npc(float X, float Y, out int index)
        {
            for(int i = 0; i < activeNPCs.Count; i++)
            {
                if (X > activeNPCs[i].position.X - 30 && X <= activeNPCs[i].position.X + 30 &&
                    Y > activeNPCs[i].position.Y - 30 && Y <= activeNPCs[i].position.Y + 30)
                {
                    index = i;
                    return true;
                }
            }
            index = 0;
            return false;
        }

        void npc_draw(Graphics graphics)
        {
            for (int i = 0; i < activeNPCs.Count; i++)
            {

                if (isOnScreen(new Point((int)activeNPCs[i].position.X, (int)activeNPCs[i].position.Y)))
                {
                    graphics.FillRectangle(new SolidBrush(ColorEnemy), activeNPCs[i].position.X - 30 + offset().X, activeNPCs[i].position.Y - 30 + offset().Y, 60, 60);
                    graphics.DrawRectangle(new Pen(Color.Black, 3), activeNPCs[i].position.X - 30 + offset().X, activeNPCs[i].position.Y - 30 + offset().Y, 60, 60);

                    eye_draw(enemy_eyes[i], 1, player_position, graphics);
                }
            }
        }
    }
}
