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
        List<Particle> particles_alive = new List<Particle>();
        List<Painttt> paintList = new List<Painttt>();

        class Particle
        {
            public PointF position;
            public PointF moment;
            public Color color;

            public Particle(PointF positionP, PointF momentP, Color colorP)
            {
                position = positionP;
                moment = momentP;
                color = colorP;
            }
        }

        void blood_generate(PointF position, PointF direction, int number_of_particles, int speed, float spread, Color color)
        {
            normalize(ref direction);

            for (int i = 0; i < number_of_particles; i++)
            {
                //Random factor
                float randomFloatX = (1 - ((float)(random.NextDouble()) * 2)) * spread;
                float randomFloatY = (1 - ((float)(random.NextDouble()) * 2)) * spread;
                PointF randomVector = new PointF(randomFloatX, randomFloatY - 0.5f);

                particles_alive.Add(new Particle(position, new PointF((direction.X + randomVector.X) * speed, (direction.Y + randomVector.Y) * speed), color));
            }
        }

        void blood_move()
        {
            for (int i = 0; i < particles_alive.Count; i++)
            {
                PointF position = particles_alive[i].position;
                PointF moment = particles_alive[i].moment;
                Color color = particles_alive[i].color;

                //Gravity
                moment.Y += 0.5f * timeScale;
                //Add particle's moment to it's position
                position.X += moment.X * timeScale;
                position.Y += moment.Y * timeScale;
                //Update
                particles_alive[i].position = position;
                particles_alive[i].moment = moment;

                //Collisions
                if (world_collides((int)position.X, (int)position.Y))
                {
                    paint_add(position, moment, color);
                    particles_alive.RemoveAt(i);
                    i--;
                }
            }
            if (particles_alive.Count > 1000) particles_alive.RemoveRange(0, particles_alive.Count - 1000);    //Limit na počet částic
        }

        void blood_draw(Graphics graphics)
        {
            Pen outline = new Pen(Brushes.Black, 4);

            foreach (Particle particle in particles_alive)
            {
                if (isOnScreen(new Point((int)particle.position.X, (int)particle.position.Y)))
                {
                    SolidBrush parColor = new SolidBrush(particle.color);
                    if (particle.color == ColorPlayer) graphics.DrawEllipse(outline, particle.position.X - 5 + offset().X, particle.position.Y - 5 + offset().Y, 10, 10);
                    else graphics.DrawRectangle(outline, particle.position.X - 5 + offset().X, particle.position.Y - 5 + offset().Y, 10, 12);
                }
            }
            foreach (Particle particle in particles_alive)
            {
                if (isOnScreen(new Point((int)particle.position.X, (int)particle.position.Y)))
                {
                    SolidBrush parColor = new SolidBrush(particle.color);
                    if (particle.color == ColorPlayer) graphics.FillEllipse(parColor, particle.position.X - 5 + offset().X, particle.position.Y - 5 + offset().Y, 10, 10);
                    else graphics.FillRectangle(parColor, particle.position.X - 5 + offset().X, particle.position.Y - 5 + offset().Y, 10, 12);
                }
            }

            foreach (Painttt paint in paintList)
            {
                int paintX = (paint.world.X * 80 * 11 * 10) + (paint.scene.X * 11 * 80) + (paint.block.X * 80);
                int paintY = (paint.world.Y * 80 * 8 * 10) + (paint.scene.Y * 8 * 80) + (paint.block.Y * 80);
                if (isOnScreen(new Point( paintX, paintY)))
                {
                    graphics.DrawRectangle(outline, (paint.world.X * 80 * 11 * 10) + (paint.scene.X * 11 * 80) + (paint.block.X * 80) + offset().X, (paint.world.Y * 80 * 8 * 10) + (paint.scene.Y * 8 * 80) + (paint.block.Y * 80) + offset().Y, 80, 10);
                }
            }
            foreach (Painttt paint in paintList)
            {
                int paintX = (paint.world.X * 80 * 11 * 10) + (paint.scene.X * 11 * 80) + (paint.block.X * 80);
                int paintY = (paint.world.Y * 80 * 8 * 10) + (paint.scene.Y * 8 * 80) + (paint.block.Y * 80);
                if (isOnScreen(new Point(paintX, paintY)))
                {
                    graphics.FillRectangle(new SolidBrush(paint.paintColor), (paint.world.X * 80 * 11 * 10) + (paint.scene.X * 11 * 80) + (paint.block.X * 80) + offset().X, (paint.world.Y * 80 * 8 * 10) + (paint.scene.Y * 8 * 80) + (paint.block.Y * 80) + offset().Y, 80, 10);
                }
            }

        }

        class Painttt
        {
            public Point world;
            public Point scene;
            public Point block;
            public Color paintColor;

            public Painttt(Point worldP, Point sceneP, Point blockP, Color paintColorP)
            {
                world = worldP;
                scene = sceneP;
                block = blockP;
                paintColor = paintColorP;
            }
        }

        void paint_add(PointF position, PointF moment, Color color)
        {
            Point world = new Point(coordinate_To_World(position.X, 0), coordinate_To_World(position.Y, 1));
            Point scene = new Point(coordinate_To_Scene(position.X, 0), coordinate_To_Scene(position.Y, 1));
            Point block = new Point(coordinate_To_Block(position.X, 0), coordinate_To_Block(position.Y, 1));

            int targetBlock = isBlockPainted(world, scene, block);

            if (moment.Y > 0 && !world_collides(position.X, position.Y - 80))
            {

                if (isBlockPainted(world, scene, block) == -1)
                {
                    paintList.Add(new Painttt(
                    new Point(coordinate_To_World(position.X, 0), coordinate_To_World(position.Y, 1)),
                    new Point(coordinate_To_Scene(position.X, 0), coordinate_To_Scene(position.Y, 1)),
                    new Point(coordinate_To_Block(position.X, 0), coordinate_To_Block(position.Y, 1)),
                    color));

                    if (paintList.Count > 100) paintList.RemoveRange(0, paintList.Count - 100);
                }
                else if (paintList.ElementAt(targetBlock).paintColor != color)
                {
                    paintList.ElementAt(targetBlock).paintColor = color;
                }
            }
        }

        int isBlockPainted(Point world, Point scene, Point block)
        {
            for (int i = 0; i < paintList.Count; i++)
            {
                Painttt paint = paintList.ElementAt(i);
                if (world.X == paint.world.X && world.Y == paint.world.Y && scene.X == paint.scene.X && scene.Y == paint.scene.Y && block.X == paint.block.X && block.Y == paint.block.Y)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
