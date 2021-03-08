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
        Color_Slider color_slider = new Color_Slider(220, 320);

        static FontFamily fontFamily = new FontFamily("Arial");
        public static Font font = new Font(fontFamily, 32, FontStyle.Bold, GraphicsUnit.Pixel);
        Font fontTitle = new Font(fontFamily, 80, FontStyle.Bold, GraphicsUnit.Pixel);

        bool lastMouse_LMB;

        PointF cameraMom, cameraMomTarget;

        Color halfBlack;

        List<Button> menuButtons = new List<Button>();

        int intro, selection;
        bool lastUp, lastDown, lastEnter, lastEscape;
        bool Overview, Colors, Title, ScoreBool;

        void Initialize_Menuuu()
        {
            intro = 255;
            halfBlack = Color.FromArgb(180, Color.Black);
            lastMouse_LMB = false;
        }

        public void enableMenu(int type)
        {
            Menuu = true;
            switch (type)
            {
                case 0:
                    {
                        Overview = true;
                        intro = 254;
                        Title = true;
                        return;
                    }

                //Color
                case 1:
                    {
                        Overview = true;
                        Colors = true;
                        selection = 0;
                        Menuuu_button_add(120, 50, "Start", true);
                        return;
                    }
                //Paused
                case 2:
                    {
                        selection = 0;
                        Menuuu_button_add(160, 20, "Resume", true);
                        Menuuu_button_add(240, 40, "Change color", false);
                        Menuuu_button_add(120, 60, "Quit", false);
                        return;
                    }
                //Died
                case 3:
                    {
                        Overview = true;
                        selection = 0;
                        ScoreBool = true;
                        Menuuu_button_add(170, 20, "Restart", true);
                        Menuuu_button_add(240, 40, "Change color", false);
                        Menuuu_button_add(120, 60, "Quit", false);
                        return;
                    }
            }
        }

        void disableMenu()
        {
            menuButtons.Clear();
            Overview = false;
            Colors = false;
            ScoreBool = false;
            Menuu = false;
        }

        public void Menuuu_button_add(int width , int position_Y, string text, bool highlight)
        {
            menuButtons.Add(new Button(400 - (width/2), 280 + (int)(((float)position_Y / 100) * 280), width, 50, text, highlight));
        }

        public void Menuuu_removeButton(int index)
        {
            menuButtons.RemoveAt(index);
        }

        public void Menuuu_controls()
        {
            if (!Menuu && !lastEscape && key_escape)
            {
                Cursor.Show();
                enableMenu(2);
            }
            if (Menuu)
            {
                if (!lastUp && key_up) Menuuu_moveUp();
                if (!lastDown && key_down) Menuuu_moveDown();
                if (!lastEnter && key_enter) Menuuu_select();


                lastUp = key_up;
                lastDown = key_down;
                lastEnter = key_enter;
            }
            lastEscape = key_escape;
        }

        public void Menuuu_moveUp()
        {
            menuButtons.ElementAt(selection).selected = false;

            if (selection != 0)
            {
                menuButtons.ElementAt(selection - 1).selected = true;
                menuButtons.ElementAt(selection - 1).pulse = 252;
                selection--;
            }
            else
            {
                menuButtons.ElementAt(menuButtons.Count - 1).selected = true;
                menuButtons.ElementAt(menuButtons.Count - 1).pulse = 252;
                selection = menuButtons.Count - 1;
            }


        }

        void Menuuu_moveDown()
        {
            menuButtons.ElementAt(selection).selected = false;

            if (selection != menuButtons.Count - 1)
            {
                menuButtons.ElementAt(selection + 1).selected = true;
                menuButtons.ElementAt(selection + 1).pulse = 252;
                selection++;
            }
            else
            {
                menuButtons.ElementAt(0).selected = true;
                menuButtons.ElementAt(0).pulse = 252;
                selection = 0;
            }
        }

        void Menuuu_select()
        {
            if (menuButtons.Count == 0) return;
            if (menuButtons.ElementAt(selection).text == "Start")
            {
                setColors(color_slider.value);
                restart();
                disableMenu();
                Cursor.Hide();
            }
            else if (menuButtons.ElementAt(selection).text == "Quit")
            {
                Application.Exit();
            }
            else if (menuButtons.ElementAt(selection).text == "Change color")
            {
                disableMenu();
                restart();
                enableMenu(1);
            }
            else if (menuButtons.ElementAt(selection).text == "Restart")
            {
                restart();
                disableMenu();
                Cursor.Hide();
            }
            else if (menuButtons.ElementAt(selection).text == "Resume")
            {
                disableMenu();
                Cursor.Hide();
            }
        }

        void restart()
        {
            score = 0;
            score_multiplier = 1;
            timeScale = 1f;

            dead = false;
            god_mode = false;
            health = 100;
            player_position = new Point(400, 280);

            mostWorld_Index = new PointF(0, 0);
            mostScene_Index = new PointF(0, 0);

            activeNPCs.Clear();
            particles_alive.Clear();
            paintList.Clear();
            pickups.Clear();
            enemy_eyes.Clear();
        }

        void setColors(int Hue)
        {
            ColorPlayer = HSV.HsvToRgb(Hue, 0.7f, 1f);
            int inverseHue;
            if (Hue < 180) inverseHue = Hue + 180;
            else inverseHue = Hue - 180;
            ColorEnemy = HSV.HsvToRgb(inverseHue, 0.7f, 1f);
        }

        void Menuuu_draw(Graphics graphics)
        {
            graphics.FillRectangle(new SolidBrush(halfBlack), 0, 0, 800, 560);
            foreach (Button button in menuButtons)
            {
                button.draw(graphics);
            }

            if (Title)
            {
                graphics.DrawString("Paint Wars", fontTitle, Brushes.White, (this.Size.Width/2) - (graphics.MeasureString("Paint Wars",fontTitle).Width/2), 230);
                if (intro != 0) graphics.FillRectangle(new SolidBrush(Color.FromArgb(intro, Color.Black)), 0, 0, 800, 560);
            }

            if (Colors)
            {
                graphics.DrawString("Choose color:", font, Brushes.White, 180, 100);
                color_slider.draw(graphics);
                graphics.FillEllipse(new SolidBrush(HSV.HsvToRgb(color_slider.value, 0.7f, 1f)), 370, 200, 60, 60);
                graphics.DrawEllipse(new Pen(Color.Black, 3), 370, 200, 60, 60);
            }

            if (ScoreBool)
            {
                graphics.DrawString("Score: " + score, fontTitle, Brushes.White, new PointF((this.Size.Width / 2) - (graphics.MeasureString("Score: " + score.ToString(), fontTitle).Width / 2), 180));
            }
        }

        void Menuuu_logic()
        {
            for (int i = 0; i < menuButtons.Count; i++)
            {
                if (get_cursor().X > menuButtons[i].posX - 10 && get_cursor().X < menuButtons[i].posX + menuButtons[i].sizeX - 10 &&
                    get_cursor().Y > menuButtons[i].posY - 10 && get_cursor().Y < menuButtons[i].posY + menuButtons[i].sizeY - 10)
                {
                    if (!lastMouse_LMB && mouse_LMB)
                    {
                        selection = i;
                        Menuuu_select();
                    }
                    else
                    {
                        menuButtons[i].selected = true;
                        selection = i;
                    }
                }
                else if (menuButtons.Count > 1)
                {
                    menuButtons[i].selected = false;
                    menuButtons[selection].selected = true;
                }
            }

            lastMouse_LMB = mouse_LMB;

            if (Title)
            {
                if (intro != 0) intro -= 2;
                else
                {
                    disableMenu();
                    Title = false;
                    Colors = true;
                    enableMenu(1);
                }
            }

            if (Colors)
            {
                color_slider.controls(mouse_LMB, (int)get_cursor().X, (int)get_cursor().Y);
            }
        }

        void overviewCamera()
        {
            if (Math.Abs(cameraMom.X - cameraMomTarget.X) < 5 && Math.Abs(cameraMom.Y - cameraMomTarget.Y) < 5)
            {
                cameraMomTarget = new PointF((float)random.Next(-20, 20), (float)random.Next(-20, 20));
            }
            cameraMom = slow(cameraMom, cameraMomTarget, 100);
            //Update
            camera = new PointF(camera.X + cameraMom.X, camera.Y + cameraMom.Y);
        }
    }

    ///////////
    //Buttons//
    ///////////

    class Button
    {
        public int posX, posY, sizeX, sizeY;
        public string text;
        public bool selected;
        public int pulse;

        public Button(int position_X, int position_Y, int size_X, int size_Y, string buttonText, bool IsSelected)
        {
            posX = position_X;
            posY = position_Y;
            sizeX = size_X;
            sizeY = size_Y;
            text = buttonText;
            selected = IsSelected;

            pulse = 252;
        }

        public void draw(Graphics graphics)
        {
            PointF textLocation = new PointF(posX + (sizeX / 2) - (graphics.MeasureString(text, Form1.font).Width / 2), posY + (sizeY / 2) - Form1.font.Height / 2);
            if (selected)
            {
                //Pulse
                Color pulseCol = Color.FromArgb(pulse, Color.White);
                int shift = (252 - pulse) / 20;
                graphics.FillRectangle(new SolidBrush(pulseCol), posX - shift, posY - shift, sizeX + 2 * shift, sizeY + 2 * shift);
                pulse -= 4;
                if (pulse == 0) pulse = 252;

                //Button
                graphics.FillRectangle(Brushes.White, posX, posY, sizeX, sizeY);
                graphics.DrawString(text, Form1.font, Brushes.Black, textLocation);
            }
            else
            {
                //Button
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, 80, 80, 80)), posX, posY, sizeX, sizeY);
                graphics.DrawString(text, Form1.font, new SolidBrush(Color.FromArgb(200, 40, 40, 40)), textLocation);
            }
        }
    }

    class Color_Slider
    {

        int posX, posY, pulse;
        public int value;
        public bool grabbed, lastLMB;

        public Color_Slider(int x, int y)
        {
            posX = x;
            posY = y;
            value = new Random().Next(360);
            grabbed = false;
            pulse = 128;
        }

        public void draw(Graphics graphics)
        {
            Color pulseCol = Color.FromArgb(pulse, Color.White);
            int offset = (252 - pulse) / 20;
            graphics.FillRectangle(new SolidBrush(pulseCol), posX - 4 - offset, posY - 4 - offset, 360 + (2 * offset) + 8, 30 + (2 * offset) + 8);
            pulse -= 4;
            if (pulse == 0) pulse = 252;

            Color color;

            graphics.FillRectangle(Brushes.White, posX - 4, posY - 4, 368, 38);
            for (int i = 0; i < 360; i++)
            {
                color = HSV.HsvToRgb(i, 0.8f, 1f);
                graphics.FillRectangle(new SolidBrush(color), posX + i, posY, 1, 30);
            }
            //color = HSV.HsvToRgb(value, 1f, 1f);
            graphics.FillRectangle(Brushes.White, posX + value - 8, posY - 4, 16, 38);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(30, 30, 30)), posX + value - 4, posY - 4, 8, 38);
        }

        public void controls(bool LMB, int x, int y)
        {
            if (!lastLMB && LMB)
            {
                if (x > posX - 8 && x < posX + 352 && y > posY - 8 && y < posY + 22)
                {
                    grabbed = true;
                }
            }
            else if (lastLMB && !LMB)
            {
                grabbed = false;
            }

            if(grabbed)
            {
                value = x - posX + 8;
                if (value < 0) value = 0;
                if (value > 360) value = 360;
            }

            lastLMB = LMB;
        }
    }
}
