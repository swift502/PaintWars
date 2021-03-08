using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Game.Properties;

namespace Game
{
    public partial class Form1 : Form
    {
        ////////////////////
        //Global variables//
        ////////////////////
        Timer timer;
        float timeScale;

        bool key_enter, key_escape;
        bool key_left, key_right, key_up, key_down;

        Color ColorPlayer, ColorEnemy;
        bool Menuu;

        System.Reflection.Assembly thisExe;
        Random random = new Random();
        
        public Form1()
        {
            InitializeComponent();
        }

        //////////////
        //Load/Setup//
        //////////////
        void Form1_Load(object sender, EventArgs e)
        {
            //Classes setup
            timer = new Timer();
            //timeScale = 1f;

            Initialize_World();
            Initialize_Player();
            Initialize_NPCS();
            Initialize_Weapon();
            Initialize_Menuuu();
            Initialize_Score();
            Initialize_label();

            //Graphics setup
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            this.DoubleBuffered = true;

            Menuu = true;
            enableMenu(0);
        }

        public void GameLoop()
        {
            while (this.Created)
            {
                timer.Reset();
                GameLogic();
                RenderScene();
                Application.DoEvents();
                while (timer.GetTicks() < 10) ;
            }
        }

        void RenderScene()
        {
            this.Invalidate(true);
        }

        //////////////
        //Game Logic//
        //////////////
        void GameLogic()
        {
            Menuuu_controls();

            if (Menuu)
            {
                Menuuu_logic();
                if (Overview)
                {
                    overviewCamera();
                    manage_Blocks();
                }
            }
            else
            {
                player_move();
                camera_Movement();

                npc_move();
                npc_spawn();
                blood_move();
                pickup_logic();

                manage_Blocks();

                if (mouse_LMB) shoot();
                else weapon_timer = 2 * (1/timeScale);
                bullets_movement();

                check_level();

                
                //Debugging

                /*
                label1.Visible = true; 
                label2.Visible = true; 
                label3.Visible = true; 
                label4.Visible = true; 
                label5.Visible = true; 
                label6.Visible = true; 
                label7.Visible = true;
                label9.Visible = true; 
                 
                label2.Text = "FPS: " + Counter.CalculateFrameRate().ToString();

                label3.Text = "V paměti je " + (Screen_buffer.Length + activeNPCs.Count + paintList.Count + bullets.Count + particles_alive.Count + pickups.Count + 1).ToString() + " objektů";
                label4.Text = "Player: " + player_position.X.ToString() + " " + player_position.Y.ToString();
                label5.Text = "Camera: " + Convert.ToInt32(camera.X).ToString() + " " + Convert.ToInt32(camera.Y).ToString();
                label1.Text = "Scroll: " + scroll.X.ToString() + " " + scroll.Y.ToString();
                label6.Text = "Block index: " + Block_Index.X + " " + Block_Index.Y;
                label7.Text = "Scene index: " + Scene_Index.X + " " + Scene_Index.Y;
                label9.Text = "World index: " + World_Index.X + " " + World_Index.Y;
                 */
            }
        }



        void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.FromArgb(230, 230, 230));

       /////////////
       //Draw Menu//
       /////////////

            if (Menuu)
            {
                if (Overview) world_draw(e.Graphics);
                else drawGameplay(e);
                Menuuu_draw(e.Graphics);
            }
            else
            {
                drawGameplay(e);
            }
        }

        /////////////
        //Draw Game//
        /////////////

        void drawGameplay(PaintEventArgs e)
        {
            drawBullets(e.Graphics);

            player_draw(e.Graphics);
            npc_draw(e.Graphics);
            
            world_draw(e.Graphics);

            blood_draw(e.Graphics);
            pickups_draw(e.Graphics);

            //HUD
            healthBar.draw(e.Graphics, ColorPlayer, health);
            score_draw(e.Graphics);
            label_draw(e.Graphics);

            //Cursor
            if (!Menuu)
            {
                e.Graphics.FillEllipse(new SolidBrush(ColorPlayer), Cursor.Position.X - this.Location.X - 16 - 10, Cursor.Position.Y - this.Location.Y - 38 - 10, 20, 20);
                e.Graphics.DrawEllipse(new Pen(Color.Black, 2), Cursor.Position.X - this.Location.X - 16 - 10, Cursor.Position.Y - this.Location.Y - 38 - 10, 20, 20);
            }
        }

        PointF get_cursor()
        {
            return new PointF(Cursor.Position.X - this.Location.X - 16, Cursor.Position.Y - this.Location.Y - 38);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Game
            if (e.KeyCode == Keys.A)    key_left = true;
            if (e.KeyCode == Keys.D)    key_right = true;
            if (e.KeyCode == Keys.W)    key_up = true;
            if (e.KeyCode == Keys.S)    key_down = true;
            if (e.KeyCode == Keys.S)
            {
                npc_add(player_position.X + 100, player_position.Y - 20);
                label_add("Enemy spawned");
            }
            if (e.KeyCode == Keys.E)
            {
                if (spawning)
                {
                    spawning = false;
                    label_add("Enemy spawning is Off", 150);
                }
                else
                {
                    spawning = true;
                    label_add("Enemy spawning is On", 150);
                }
            }
            if (e.KeyCode == Keys.G)
            {
                if (god_mode)
                {
                    god_mode = false;
                    label_add("God mode is Off", 120);
                }
                else
                {
                    god_mode = true;
                    health = 100;
                    label_add("God mode is On", 120);
                }
            }

            //Menu
            if (e.KeyCode == Keys.Up)       key_up = true;
            if (e.KeyCode == Keys.Down)     key_down = true;
            if (e.KeyCode == Keys.Enter)    key_enter = true;
            if (e.KeyCode == Keys.Escape)   key_escape = true;
            
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //Game
            if (e.KeyCode == Keys.A)    key_left = false;
            if (e.KeyCode == Keys.D)    key_right = false;
            if (e.KeyCode == Keys.W)    key_up = false;
            if (e.KeyCode == Keys.S)    key_down = false;

            //Menu
            if (e.KeyCode == Keys.Up)       key_up = false;
            if (e.KeyCode == Keys.Down)     key_down = false;
            if (e.KeyCode == Keys.Enter)    key_enter = false;
            if (e.KeyCode == Keys.Escape)   key_escape = false;
        }

        float slow(float source, float destination, int slow_factor)
        {
            return source + ((destination - source) / slow_factor);
        }

        PointF slow(PointF source, PointF destination, int slow_factor)
        {
            return new PointF(source.X + ((destination.X - source.X) / slow_factor), source.Y + ((destination.Y - source.Y) / slow_factor));
        }

        PointF slow(PointF source, PointF destination, float slow_factor)
        {
            return new PointF(source.X + ((destination.X - source.X) / slow_factor), source.Y + ((destination.Y - source.Y) / slow_factor));
        }

        PointF offset()
        {
            return new PointF(-camera.X + (800 / 2), -camera.Y + (560 / 2));
        }

        void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) mouse_LMB = true;
        }

        void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) mouse_LMB = false;
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            string time_to_string = "";

            if (e.Delta > 0)
            {
                
                if (timeScale < 2.95f)
                {
                    if (timeScale < 0.025f) timeScale += 0.04f;
                    else if (timeScale < 0.075f) timeScale += 0.05f;
                    else timeScale += 0.1f;
                }
            }
            if (e.Delta < 0)
            {
                if (timeScale > 0.025f)
                {
                    if (timeScale < 0.075f) timeScale -= 0.04f;
                    else if (timeScale < 0.15f) timeScale -= 0.05f;
                    else timeScale -= 0.1f;
                }
            }

            time_to_string = Math.Round((double)timeScale * 100).ToString();

            try
            {
                Label label = labels.First(Label => Label.tag == "time");
                label.text = "Time scale: " + time_to_string + "%";
                if(label.timer > 20) label.timer = 150;
            }
            catch
            {
                label_add("Time scale: " + time_to_string + "%", 150, "time");
            }

        }
    }
}
