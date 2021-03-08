using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using Pecatch_Man_Level_Editor.Properties;

namespace Pecatch_Man_Level_Editor
{
    public partial class Form1 : Form
    {
        static public bool[,] Scene;
        int tilesX, tilesY;
        int tileSize;

        Assembly thisExe;
        bool LMB, RMB;
        Point lastBlock, currentBlock;

        //
        //Form
        //

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            thisExe = Assembly.GetExecutingAssembly();

            tilesX = 11;
            tilesY = 8;

            Scene = new bool[tilesX, tilesY];

            tileSize = 80;
            this.Size = new Size(tilesX * tileSize + 16 + 1, tilesY * tileSize + 38 + 25 + 1);
            centerCursor();
            this.Invalidate();

            this.DoubleBuffered = true;
        }

        void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            currentBlock = getBlock();

            if (currentBlock.X != lastBlock.X || currentBlock.Y != lastBlock.Y)
            {
                setBlock();
            }

            lastBlock = currentBlock;
        }

        void setBlock()
        {
            if (getBlock() != new Point(-1, -1))
            {
                if (LMB) Scene[getBlock().X, getBlock().Y] = true;
                if (RMB) Scene[getBlock().X, getBlock().Y] = false;
                this.Invalidate();
            }
        }

        //
        //Other methods
        //

        void centerCursor()
        {
            Cursor.Position = new Point(this.Location.X + (this.Size.Width / 2), this.Location.Y + (this.Size.Height / 2));
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

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < tilesX; i++)
            {
                for (int j = 0; j < tilesY; j++)
                {
                    if (Scene[i, j]) e.Graphics.FillRectangle(Brushes.DimGray, i * tileSize, j * tileSize + 25, tileSize, tileSize);
                    e.Graphics.DrawRectangle(Pens.Gray, i * tileSize, j * tileSize + 25, tileSize, tileSize);
                }
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) LMB = true;
            if (e.Button == System.Windows.Forms.MouseButtons.Right) RMB = true;
            setBlock();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) LMB = false;
            if (e.Button == System.Windows.Forms.MouseButtons.Right) RMB = false;
            setBlock();
        }

        Point getBlock()
        {
            if (Cursor.Position.X > this.Location.X + 12 && Cursor.Position.Y > this.Location.Y + 38 + 25 &&
                Cursor.Position.X < this.Location.X + this.Size.Width - 12 && Cursor.Position.Y < this.Location.Y + +this.Size.Height - 12)
            {
                return new Point(coordinate_To_Block(Cursor.Position.X - this.Location.X - 10, 0), coordinate_To_Block(Cursor.Position.Y - this.Location.Y - 54, 1));
            }
            else return new Point(-1, -1);
        }

        private void ExportSceneButton_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            
            form2.ShowDialog(this);
            
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < tilesX; i++)
            {
                for (int j = 0; j < tilesY; j++)
                {
                    Scene[i, j] = false;
                }
            }
            this.Invalidate();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
