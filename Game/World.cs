using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Game.Properties;
using System.Windows.Forms;

namespace Game
{
    public partial class Form1 : Form
    {
        bool[,][,] Scenes;
        bool[,] Screen_buffer;
        PointF mostWorld_Index, World_Index, mostScene_Index, Scene_Index, Block_Index;
        List<bool[,]> Scenes_List = new List<bool[,]>();
        PointF scroll;
        int tilesX, tilesY;
        PointF screenSize, LTScene, RTScene, LBScene, RBScene;

        void Initialize_World()
        {
            screenSize = new PointF(800, 560);
            tilesX = 11; //((int)screenSize.X / 80) + 1;
            tilesY = 8; //((int)screenSize.Y / 80) + 1;

            Scenes = new bool[10, 10][,];
            Screen_buffer = new bool[11, 8];

            initializeScenes();

            //Fill scenes 2D array
            for (int i = 0; i < Scenes.GetLength(0); i++)
            {
                for (int j = 0; j < Scenes.GetLength(1); j++)
                {
                    Scenes[i, j] = Scenes_List[random.Next(Scenes_List.Count)];
                }
            }

            for (int j = 0; j < Scenes.GetLength(1); j++)
            {
                Scenes[random.Next(10), j] = Scenes_List[5];
            }

            bool[,] Spawn = new bool[11, 8];
            Spawn[0, 7] = true;
            Spawn[1, 7] = true;
            Spawn[2, 7] = true;
            Spawn[3, 7] = true;
            Spawn[4, 7] = true;
            Spawn[5, 7] = true;
            Spawn[6, 7] = true;
            Spawn[7, 7] = true;
            Spawn[8, 7] = true;
            Spawn[9, 7] = true;
            Scenes[0, 0] = Spawn;

            scroll.X = 400;
            scroll.Y = 280;
            Scene_Index = new PointF(0, 0);
            mostScene_Index = new PointF(0, 0);
            Block_Index = new PointF(0, 0);
            World_Index = new PointF(0, 0);
            mostWorld_Index = new PointF(0, 0);
        }

        void world_draw(Graphics graphics)
        {
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Screen_buffer[i, j]) graphics.DrawRectangle(new Pen(Color.FromArgb(20, 20, 20), 5), (80 * i) - scroll.X, (80 * j) - scroll.Y, 80, 80);
                }
            }
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Screen_buffer[i, j]) graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 50, 50)), (80 * i) - scroll.X, (80 * j) - scroll.Y, 80, 80);
                }
            }
        }

        void manage_Blocks()
        {
            //Calculates values neccesary for correct positioning of all the elements in the game.
            scroll.X = coordinate_To_Scroll(camera.X - 80 - (screenSize.X / 2));
            scroll.Y = coordinate_To_Scroll(camera.Y - 80 - (screenSize.Y / 2));
            Block_Index.X = tilesX - 1 - coordinate_To_Block(camera.X - 80 - (screenSize.X / 2), 0);
            Block_Index.Y = tilesY - 1 - coordinate_To_Block(camera.Y - 80 - (screenSize.Y / 2), 1);
            Scene_Index.X = coordinate_To_Scene(camera.X - (screenSize.X / 2), 0);
            Scene_Index.Y = coordinate_To_Scene(camera.Y - (screenSize.Y / 2), 1);
            World_Index.X = coordinate_To_World(camera.X - (screenSize.X / 2), 0);
            World_Index.Y = coordinate_To_World(camera.Y - (screenSize.Y / 2), 1);

            load_Blocks();
        }

        void load_Blocks()
        {
            //Loads only visible blocks, from only visible scenes, into screen_buffer. Optimization.

            identifyScenes();

            for (int i = 0; i < tilesX; i++)
            {
                for (int j = 0; j < tilesY; j++)
                {
                    //Scene in the left top corner of the screen
                    if (i < Block_Index.X && j < Block_Index.Y)
                    {
                        Screen_buffer[i, j] = Scenes[(int)LTScene.X, (int)LTScene.Y]
                        [tilesX - (int)Block_Index.X + i,
                        tilesY - (int)Block_Index.Y + j];
                    }

                    //Right top Scene
                    else if (i >= Block_Index.X && j < Block_Index.Y)
                    {
                        Screen_buffer[i, j] = Scenes[(int)RTScene.X, (int)RTScene.Y]
                        [i - (int)Block_Index.X,
                        tilesY - (int)Block_Index.Y + j];
                    }

                    //Left bottom Scene
                    else if (i < Block_Index.X && j >= Block_Index.Y)
                    {
                        Screen_buffer[i, j] = Scenes[(int)LBScene.X, (int)LBScene.Y]
                        [tilesX - (int)Block_Index.X + i,
                        j - (int)Block_Index.Y];
                    }

                    //Right bottom Scene
                    else if (i >= Block_Index.X && j >= Block_Index.Y)
                    {
                        Screen_buffer[i, j] = Scenes[(int)RBScene.X, (int)RBScene.Y]
                        [i - (int)Block_Index.X,
                        j - (int)Block_Index.Y];
                    }
                     
                }
            }
        }

        void identifyScenes()
        {
            // Gets the four scenes that are currently being displayed - their parts already are
            // and/or will be loaded in screen_buffer.

            //Scene in the left top corner of the screen
            LTScene = new PointF(coordinate_To_Scene(camera.X - (screenSize.X / 2), 0),
                                  coordinate_To_Scene(camera.Y - (screenSize.Y / 2), 1));

            //Right top Scene
            RTScene = new PointF(coordinate_To_Scene(camera.X + (screenSize.X / 2) - 1, 0),
                                  coordinate_To_Scene(camera.Y - (screenSize.Y / 2), 1));

            //Left bottom Scene
            LBScene = new PointF(coordinate_To_Scene(camera.X - (screenSize.X / 2), 0),
                                  coordinate_To_Scene(camera.Y + (screenSize.Y / 2) - 1, 1));

            //Right bottom Scene
            RBScene = new PointF(coordinate_To_Scene(camera.X + (screenSize.X / 2) - 1, 0),
                                  coordinate_To_Scene(camera.Y + (screenSize.Y / 2) - 1, 1));
        }

        bool world_collides(float X, float Y)
        {
            if (Scenes
                [coordinate_To_Scene(X, 0), coordinate_To_Scene(Y, 1)]
                [(int)coordinate_To_Block(X, 0), (int)coordinate_To_Block(Y, 1)]) return true;
            else return false;
        }

        int coordinate_To_World(float val, int axis)
        {
            int value = (int)val;
            int tiles;
            if (axis == 0) tiles = tilesX;
            else tiles = tilesY;

            if (value >= 0) return (value / (80 * tiles * 10));
            else return (value / (80 * tiles * 10)) - 1;
        }

        int coordinate_To_Scene(float val, int axis)
        {
            int value = (int)val;
            int tiles;
            if (axis == 0) tiles = tilesX;
            else tiles = tilesY;

            if (value >= 0) return (value / (tiles * 80)) % 10;
            else return 9 + ((value / (tiles * 80)) % 10);
        }

        int coordinate_To_Block(float val, int axis)
        {
            int value = (int)val;
            int tiles;
            if (axis == 0) tiles = tilesX;
            else tiles = tilesY;

            if (value >= 0) return (value / 80) % tiles;
            else return tiles - 1 + ((value / 80) % tiles);
        }

        float coordinate_To_Scroll(float val)
        {
            int value = (int)val;
            if (value >= 0) return value - ((value / 80) * 80);
            else return 80 + (value - ((value / 80) * 80));
        }

        void normalize(ref PointF vector)
        {
            float length = (float)Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2));
            vector.X = vector.X / length;
            vector.Y = vector.Y / length;
        }

        void check_level()
        {
            if ((coordinate_To_Scene(player_position.Y, 1) * 8 * 80) + (coordinate_To_World(player_position.Y, 1) * 10 * 8 * 80) > 
                (mostScene_Index.Y * 8 * 80) + (mostWorld_Index.Y * 10 * 8 * 80))
            {
                for (int i = 0; i < activeNPCs.Count; i++)
                {
                    npc_die(i);
                    i--;
                }

                score_multiplier++;
                if (npc_time > 5) npc_time--;
                score_flash = 250;

                mostScene_Index.Y = coordinate_To_Scene(player_position.Y, 1);
                mostWorld_Index.Y = coordinate_To_World(player_position.Y, 1);
            }
        }

        bool isOnScreen(Point position)
        {
            if (position.X > camera.X - (800 / 2) - 80 && position.X < camera.X + (800 / 2) + 80 &&
               position.Y > camera.Y - (560 / 2) - 80 && position.Y < camera.Y + (560 / 2) + 80)
            {
                return true;
            }
            else return false;
        }

        void initializeScenes()
        {
            bool[,] Scene1 = new bool[11, 8];
            Scene1[0, 7] = true;
            Scene1[1, 1] = true;
            Scene1[1, 2] = true;
            Scene1[1, 7] = true;
            Scene1[2, 1] = true;
            Scene1[2, 2] = true;
            Scene1[2, 7] = true;
            Scene1[3, 1] = true;
            Scene1[3, 7] = true;
            Scene1[4, 1] = true;
            Scene1[4, 6] = true;
            Scene1[4, 7] = true;
            Scene1[5, 1] = true;
            Scene1[5, 5] = true;
            Scene1[5, 6] = true;
            Scene1[5, 7] = true;
            Scene1[6, 1] = true;
            Scene1[6, 6] = true;
            Scene1[6, 7] = true;
            Scene1[7, 1] = true;
            Scene1[7, 7] = true;
            Scene1[8, 1] = true;
            Scene1[8, 2] = true;
            Scene1[8, 7] = true;
            Scene1[9, 1] = true;
            Scene1[9, 2] = true;
            Scene1[9, 7] = true;
            Scene1[10, 7] = true;
            Scenes_List.Add(Scene1);

            bool[,] Scene2 = new bool[11, 8];
            Scene2[0, 7] = true;
            Scene2[2, 4] = true;
            Scene2[2, 5] = true;
            Scene2[2, 6] = true;
            Scene2[2, 7] = true;
            Scene2[3, 5] = true;
            Scene2[3, 6] = true;
            Scene2[3, 7] = true;
            Scene2[4, 6] = true;
            Scene2[4, 7] = true;
            Scene2[6, 4] = true;
            Scene2[6, 5] = true;
            Scene2[6, 6] = true;
            Scene2[6, 7] = true;
            Scene2[7, 5] = true;
            Scene2[7, 6] = true;
            Scene2[7, 7] = true;
            Scene2[8, 6] = true;
            Scene2[8, 7] = true;
            Scene2[10, 7] = true;
            Scenes_List.Add(Scene2);

            bool[,] Scene5 = new bool[11, 8];
            Scene5[0, 7] = true;
            Scene5[1, 7] = true;
            Scene5[2, 6] = true;
            Scene5[2, 7] = true;
            Scene5[3, 6] = true;
            Scene5[3, 7] = true;
            Scene5[4, 5] = true;
            Scene5[4, 6] = true;
            Scene5[4, 7] = true;
            Scene5[5, 5] = true;
            Scene5[5, 6] = true;
            Scene5[5, 7] = true;
            Scene5[6, 5] = true;
            Scene5[6, 6] = true;
            Scene5[6, 7] = true;
            Scene5[7, 6] = true;
            Scene5[7, 7] = true;
            Scene5[8, 6] = true;
            Scene5[8, 7] = true;
            Scene5[9, 7] = true;
            Scene5[10, 7] = true;
            Scenes_List.Add(Scene5);

            bool[,] Scene6 = new bool[11, 8];
            Scene6[0, 7] = true;
            Scene6[1, 1] = true;
            Scene6[1, 7] = true;
            Scene6[2, 1] = true;
            Scene6[2, 6] = true;
            Scene6[2, 7] = true;
            Scene6[3, 1] = true;
            Scene6[3, 5] = true;
            Scene6[3, 6] = true;
            Scene6[3, 7] = true;
            Scene6[4, 4] = true;
            Scene6[4, 5] = true;
            Scene6[4, 6] = true;
            Scene6[4, 7] = true;
            Scene6[5, 4] = true;
            Scene6[5, 5] = true;
            Scene6[5, 6] = true;
            Scene6[5, 7] = true;
            Scene6[6, 4] = true;
            Scene6[6, 5] = true;
            Scene6[6, 6] = true;
            Scene6[6, 7] = true;
            Scene6[7, 1] = true;
            Scene6[7, 5] = true;
            Scene6[7, 6] = true;
            Scene6[7, 7] = true;
            Scene6[8, 1] = true;
            Scene6[8, 6] = true;
            Scene6[8, 7] = true;
            Scene6[9, 1] = true;
            Scene6[9, 7] = true;
            Scene6[10, 7] = true;
            Scenes_List.Add(Scene6);

            bool[,] Scene7 = new bool[11, 8];
            Scene7[0, 7] = true;
            Scene7[1, 7] = true;
            Scene7[2, 2] = true;
            Scene7[2, 6] = true;
            Scene7[2, 7] = true;
            Scene7[3, 2] = true;
            Scene7[3, 5] = true;
            Scene7[3, 6] = true;
            Scene7[3, 7] = true;
            Scene7[4, 2] = true;
            Scene7[4, 7] = true;
            Scene7[5, 7] = true;
            Scene7[6, 4] = true;
            Scene7[7, 6] = true;
            Scene7[7, 7] = true;
            Scene7[8, 2] = true;
            Scene7[8, 7] = true;
            Scene7[9, 2] = true;
            Scene7[9, 7] = true;
            Scene7[10, 2] = true;
            Scene7[10, 7] = true;
            Scenes_List.Add(Scene7);

            bool[,] Scene8 = new bool[11, 8];
            Scene8[0, 2] = true;
            Scene8[0, 7] = true;
            Scene8[1, 7] = true;
            Scene8[2, 2] = true;
            Scene8[2, 6] = true;
            Scene8[2, 7] = true;
            Scene8[3, 6] = true;
            Scene8[3, 7] = true;
            Scene8[4, 2] = true;
            Scene8[5, 2] = true;
            Scene8[6, 2] = true;
            Scene8[7, 6] = true;
            Scene8[7, 7] = true;
            Scene8[8, 2] = true;
            Scene8[8, 6] = true;
            Scene8[8, 7] = true;
            Scene8[9, 7] = true;
            Scene8[10, 2] = true;
            Scene8[10, 7] = true;
            Scenes_List.Add(Scene8);

            bool[,] Scene9 = new bool[11, 8];
            Scene9[0, 2] = true;
            Scene9[0, 3] = true;
            Scene9[0, 7] = true;
            Scene9[1, 3] = true;
            Scene9[1, 7] = true;
            Scene9[2, 3] = true;
            Scene9[3, 3] = true;
            Scene9[3, 7] = true;
            Scene9[4, 2] = true;
            Scene9[4, 3] = true;
            Scene9[4, 7] = true;
            Scene9[6, 2] = true;
            Scene9[6, 3] = true;
            Scene9[6, 7] = true;
            Scene9[7, 3] = true;
            Scene9[7, 7] = true;
            Scene9[8, 3] = true;
            Scene9[9, 3] = true;
            Scene9[9, 7] = true;
            Scene9[10, 2] = true;
            Scene9[10, 3] = true;
            Scene9[10, 7] = true;
            Scenes_List.Add(Scene9);

            bool[,] Scene10 = new bool[11, 8];
            Scene10[0, 7] = true;
            Scene10[1, 3] = true;
            Scene10[1, 7] = true;
            Scene10[2, 3] = true;
            Scene10[2, 4] = true;
            Scene10[2, 7] = true;
            Scene10[3, 4] = true;
            Scene10[3, 7] = true;
            Scene10[4, 7] = true;
            Scene10[5, 2] = true;
            Scene10[5, 6] = true;
            Scene10[5, 7] = true;
            Scene10[6, 2] = true;
            Scene10[7, 6] = true;
            Scene10[7, 7] = true;
            Scene10[8, 2] = true;
            Scene10[8, 3] = true;
            Scene10[8, 7] = true;
            Scene10[9, 3] = true;
            Scene10[9, 7] = true;
            Scene10[10, 3] = true;
            Scene10[10, 7] = true;
            Scenes_List.Add(Scene10);

            bool[,] Scene11 = new bool[11, 8];
            Scene11[0, 6] = true;
            Scene11[0, 7] = true;
            Scene11[1, 1] = true;
            Scene11[1, 5] = true;
            Scene11[1, 6] = true;
            Scene11[1, 7] = true;
            Scene11[2, 1] = true;
            Scene11[2, 4] = true;
            Scene11[2, 5] = true;
            Scene11[2, 6] = true;
            Scene11[2, 7] = true;
            Scene11[3, 1] = true;
            Scene11[3, 4] = true;
            Scene11[4, 1] = true;
            Scene11[4, 4] = true;
            Scene11[4, 7] = true;
            Scene11[5, 1] = true;
            Scene11[5, 4] = true;
            Scene11[5, 7] = true;
            Scene11[6, 1] = true;
            Scene11[6, 4] = true;
            Scene11[6, 7] = true;
            Scene11[7, 1] = true;
            Scene11[7, 4] = true;
            Scene11[7, 7] = true;
            Scene11[8, 1] = true;
            Scene11[8, 7] = true;
            Scene11[9, 1] = true;
            Scene11[9, 2] = true;
            Scene11[9, 3] = true;
            Scene11[9, 4] = true;
            Scene11[9, 7] = true;
            Scene11[10, 7] = true;
            Scenes_List.Add(Scene11);

            bool[,] Scene12 = new bool[11, 8];
            Scene12[0, 2] = true;
            Scene12[0, 3] = true;
            Scene12[0, 7] = true;
            Scene12[1, 3] = true;
            Scene12[1, 7] = true;
            Scene12[2, 3] = true;
            Scene12[2, 7] = true;
            Scene12[3, 3] = true;
            Scene12[4, 3] = true;
            Scene12[4, 4] = true;
            Scene12[4, 5] = true;
            Scene12[4, 6] = true;
            Scene12[4, 7] = true;
            Scene12[6, 3] = true;
            Scene12[6, 4] = true;
            Scene12[6, 5] = true;
            Scene12[6, 6] = true;
            Scene12[6, 7] = true;
            Scene12[7, 3] = true;
            Scene12[8, 3] = true;
            Scene12[8, 7] = true;
            Scene12[9, 3] = true;
            Scene12[9, 7] = true;
            Scene12[10, 2] = true;
            Scene12[10, 3] = true;
            Scene12[10, 7] = true;
            Scenes_List.Add(Scene12);

            bool[,] Scene13 = new bool[11, 8];
            Scene13[0, 7] = true;
            Scene13[1, 2] = true;
            Scene13[1, 7] = true;
            Scene13[2, 2] = true;
            Scene13[2, 7] = true;
            Scene13[3, 6] = true;
            Scene13[3, 7] = true;
            Scene13[4, 5] = true;
            Scene13[4, 6] = true;
            Scene13[4, 7] = true;
            Scene13[6, 5] = true;
            Scene13[6, 6] = true;
            Scene13[6, 7] = true;
            Scene13[7, 6] = true;
            Scene13[7, 7] = true;
            Scene13[8, 2] = true;
            Scene13[8, 7] = true;
            Scene13[9, 2] = true;
            Scene13[9, 7] = true;
            Scene13[10, 7] = true;
            Scenes_List.Add(Scene13);

            bool[,] Scene15 = new bool[11, 8];
            Scene15[0, 2] = true;
            Scene15[0, 3] = true;
            Scene15[0, 4] = true;
            Scene15[0, 7] = true;
            Scene15[1, 4] = true;
            Scene15[1, 7] = true;
            Scene15[2, 6] = true;
            Scene15[2, 7] = true;
            Scene15[3, 2] = true;
            Scene15[3, 7] = true;
            Scene15[4, 2] = true;
            Scene15[4, 7] = true;
            Scene15[5, 2] = true;
            Scene15[6, 2] = true;
            Scenes_List.Add(Scene15);

            bool[,] Scene16 = new bool[11, 8];
            Scene16[4, 2] = true;
            Scene16[5, 2] = true;
            Scene16[5, 7] = true;
            Scene16[6, 2] = true;
            Scene16[6, 7] = true;
            Scene16[7, 2] = true;
            Scene16[7, 6] = true;
            Scene16[7, 7] = true;
            Scene16[8, 7] = true;
            Scene16[9, 4] = true;
            Scene16[9, 7] = true;
            Scene16[10, 7] = true;
            Scenes_List.Add(Scene16);

            bool[,] Scene17 = new bool[11, 8];
            Scene17[0, 7] = true;
            Scene17[1, 7] = true;
            Scene17[2, 2] = true;
            Scene17[2, 6] = true;
            Scene17[2, 7] = true;
            Scene17[3, 2] = true;
            Scene17[3, 6] = true;
            Scene17[3, 7] = true;
            Scene17[4, 2] = true;
            Scene17[4, 6] = true;
            Scene17[4, 7] = true;
            Scene17[5, 5] = true;
            Scene17[5, 6] = true;
            Scene17[5, 7] = true;
            Scene17[6, 5] = true;
            Scene17[6, 6] = true;
            Scene17[6, 7] = true;
            Scene17[7, 6] = true;
            Scene17[7, 7] = true;
            Scene17[8, 7] = true;
            Scene17[9, 7] = true;
            Scene17[10, 7] = true;
            Scenes_List.Add(Scene17);

            bool[,] Scene18 = new bool[11, 8];
            Scene18[0, 7] = true;
            Scene18[1, 7] = true;
            Scene18[2, 3] = true;
            Scene18[2, 7] = true;
            Scene18[3, 3] = true;
            Scene18[3, 7] = true;
            Scene18[4, 3] = true;
            Scene18[4, 7] = true;
            Scene18[5, 3] = true;
            Scene18[5, 4] = true;
            Scene18[5, 6] = true;
            Scene18[5, 7] = true;
            Scene18[6, 3] = true;
            Scene18[6, 7] = true;
            Scene18[7, 3] = true;
            Scene18[7, 7] = true;
            Scene18[8, 3] = true;
            Scene18[8, 7] = true;
            Scene18[9, 7] = true;
            Scene18[10, 7] = true;
            Scenes_List.Add(Scene18);

            bool[,] Scene19 = new bool[11, 8];
            Scene19[0, 7] = true;
            Scene19[1, 2] = true;
            Scene19[1, 7] = true;
            Scene19[2, 2] = true;
            Scene19[2, 7] = true;
            Scene19[3, 2] = true;
            Scene19[3, 7] = true;
            Scene19[4, 6] = true;
            Scene19[4, 7] = true;
            Scene19[5, 7] = true;
            Scene19[6, 4] = true;
            Scene19[6, 7] = true;
            Scene19[7, 4] = true;
            Scene19[7, 7] = true;
            Scene19[8, 4] = true;
            Scene19[8, 7] = true;
            Scene19[9, 7] = true;
            Scene19[10, 7] = true;
            Scenes_List.Add(Scene19);

            bool[,] Scene20 = new bool[11, 8];
            Scene20[0, 7] = true;
            Scene20[1, 6] = true;
            Scene20[1, 7] = true;
            Scene20[2, 5] = true;
            Scene20[2, 6] = true;
            Scene20[2, 7] = true;
            Scene20[3, 5] = true;
            Scene20[3, 7] = true;
            Scene20[4, 5] = true;
            Scene20[4, 6] = true;
            Scene20[4, 7] = true;
            Scene20[5, 6] = true;
            Scene20[5, 7] = true;
            Scene20[6, 7] = true;
            Scene20[7, 1] = true;
            Scene20[7, 2] = true;
            Scene20[7, 3] = true;
            Scene20[7, 7] = true;
            Scene20[8, 1] = true;
            Scene20[8, 3] = true;
            Scene20[8, 7] = true;
            Scene20[9, 1] = true;
            Scene20[9, 2] = true;
            Scene20[9, 3] = true;
            Scene20[9, 7] = true;
            Scene20[10, 7] = true;
            Scenes_List.Add(Scene20);

            bool[,] Scene21 = new bool[11, 8];
            Scene21[0, 7] = true;
            Scene21[1, 7] = true;
            Scene21[2, 2] = true;
            Scene21[2, 4] = true;
            Scene21[2, 6] = true;
            Scene21[2, 7] = true;
            Scene21[3, 2] = true;
            Scene21[3, 4] = true;
            Scene21[3, 6] = true;
            Scene21[4, 1] = true;
            Scene21[4, 2] = true;
            Scene21[4, 4] = true;
            Scene21[4, 6] = true;
            Scene21[5, 4] = true;
            Scene21[5, 6] = true;
            Scene21[6, 1] = true;
            Scene21[6, 2] = true;
            Scene21[6, 3] = true;
            Scene21[6, 4] = true;
            Scene21[6, 6] = true;
            Scene21[7, 6] = true;
            Scene21[8, 1] = true;
            Scene21[8, 2] = true;
            Scene21[8, 3] = true;
            Scene21[8, 4] = true;
            Scene21[8, 6] = true;
            Scene21[9, 4] = true;
            Scene21[9, 6] = true;
            Scene21[9, 7] = true;
            Scene21[10, 7] = true;
            Scenes_List.Add(Scene21);

            bool[,] Scene22 = new bool[11, 8];
            Scene22[0, 7] = true;
            Scene22[1, 4] = true;
            Scene22[1, 7] = true;
            Scene22[2, 2] = true;
            Scene22[2, 4] = true;
            Scene22[2, 7] = true;
            Scene22[3, 4] = true;
            Scene22[3, 7] = true;
            Scene22[4, 7] = true;
            Scene22[5, 3] = true;
            Scene22[5, 6] = true;
            Scene22[5, 7] = true;
            Scene22[6, 3] = true;
            Scene22[6, 6] = true;
            Scene22[7, 3] = true;
            Scene22[7, 6] = true;
            Scene22[8, 6] = true;
            Scene22[9, 2] = true;
            Scene22[9, 6] = true;
            Scene22[10, 6] = true;
            Scenes_List.Add(Scene22);

            bool[,] Scene23 = new bool[11, 8];
            Scene23[0, 7] = true;
            Scene23[1, 2] = true;
            Scene23[1, 7] = true;
            Scene23[2, 2] = true;
            Scene23[2, 6] = true;
            Scene23[2, 7] = true;
            Scene23[3, 2] = true;
            Scene23[3, 7] = true;
            Scene23[4, 4] = true;
            Scene23[4, 7] = true;
            Scene23[5, 3] = true;
            Scene23[5, 4] = true;
            Scene23[5, 5] = true;
            Scene23[5, 7] = true;
            Scene23[6, 4] = true;
            Scene23[6, 7] = true;
            Scene23[7, 2] = true;
            Scene23[7, 7] = true;
            Scene23[8, 2] = true;
            Scene23[8, 6] = true;
            Scene23[8, 7] = true;
            Scene23[9, 2] = true;
            Scene23[9, 7] = true;
            Scene23[10, 7] = true;
            Scenes_List.Add(Scene23);

            bool[,] Scene24 = new bool[11, 8];
            Scene24[0, 7] = true;
            Scene24[1, 7] = true;
            Scene24[2, 3] = true;
            Scene24[2, 7] = true;
            Scene24[3, 7] = true;
            Scene24[4, 3] = true;
            Scene24[4, 4] = true;
            Scene24[4, 5] = true;
            Scene24[4, 7] = true;
            Scene24[5, 3] = true;
            Scene24[5, 7] = true;
            Scene24[6, 3] = true;
            Scene24[6, 5] = true;
            Scene24[6, 7] = true;
            Scene24[7, 3] = true;
            Scene24[7, 5] = true;
            Scene24[7, 7] = true;
            Scene24[8, 5] = true;
            Scene24[8, 7] = true;
            Scene24[9, 3] = true;
            Scene24[9, 4] = true;
            Scene24[9, 5] = true;
            Scene24[9, 7] = true;
            Scene24[10, 7] = true;
            Scenes_List.Add(Scene24);

            bool[,] Scene25 = new bool[11, 8];
            Scene25[0, 7] = true;
            Scene25[1, 2] = true;
            Scene25[1, 3] = true;
            Scene25[1, 4] = true;
            Scene25[1, 7] = true;
            Scene25[2, 2] = true;
            Scene25[2, 7] = true;
            Scene25[3, 2] = true;
            Scene25[3, 4] = true;
            Scene25[3, 7] = true;
            Scene25[4, 2] = true;
            Scene25[4, 4] = true;
            Scene25[4, 7] = true;
            Scene25[5, 4] = true;
            Scene25[5, 7] = true;
            Scene25[6, 2] = true;
            Scene25[6, 4] = true;
            Scene25[6, 7] = true;
            Scene25[7, 2] = true;
            Scene25[7, 4] = true;
            Scene25[7, 7] = true;
            Scene25[8, 2] = true;
            Scene25[8, 7] = true;
            Scene25[9, 2] = true;
            Scene25[9, 3] = true;
            Scene25[9, 4] = true;
            Scene25[9, 7] = true;
            Scene25[10, 7] = true;
            Scenes_List.Add(Scene25);

            bool[,] Scene26 = new bool[11, 8];
            Scene26[0, 7] = true;
            Scene26[1, 6] = true;
            Scene26[1, 7] = true;
            Scene26[2, 7] = true;
            Scene26[3, 6] = true;
            Scene26[3, 7] = true;
            Scene26[4, 2] = true;
            Scene26[4, 7] = true;
            Scene26[5, 2] = true;
            Scene26[5, 6] = true;
            Scene26[5, 7] = true;
            Scene26[6, 7] = true;
            Scene26[7, 2] = true;
            Scene26[7, 6] = true;
            Scene26[7, 7] = true;
            Scene26[8, 7] = true;
            Scene26[9, 6] = true;
            Scene26[9, 7] = true;
            Scene26[10, 7] = true;
            Scenes_List.Add(Scene26);


        }
    }
}
