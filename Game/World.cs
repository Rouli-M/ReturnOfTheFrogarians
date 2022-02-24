using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Splatoon2D
{
    public class World
    {
        public List<Rectangle> Ground;
        public List<Rectangle> PaintedGround;
        public List<Rectangle> PaintedWalls;
        public static List<(Vector2, Sprite)> Decor;
        public List<PhysicalObject> Stuff;
        private List<PhysicalObject> StuffToRemove;
        private static Texture2D gray, ground, painted_ground, wall, painted_wall, background;
        private static Texture2D outer_corner_top_left, outer_corner_top_right, inner_corner_top_left, inner_corner_top_right;
        private static Sprite statue1, statue2;

        public World(Player player)
        {
            StuffToRemove = new List<PhysicalObject>();
            PaintedGround = new List<Rectangle>();
            PaintedWalls = new List<Rectangle>();
            Ground = new List<Rectangle>()
            {
                new Rectangle(-2200, 0, 3000, 200), // ground
                new Rectangle(-2000, -1000, 650, 7000), // most left wall
                new Rectangle(-3000, -2000, 1100, 7000), // most most left wall
                new Rectangle(-4000, -2000, 3000, 800), // roof
                new Rectangle(-3650, -500, 650 + 3000, 700 - 500), // roof above spawn
                new Rectangle(0, -60, 300, 200), // right bump
                new Rectangle(250, -300, 300 + 1500, 500), // big right bump

                new Rectangle(1500 + 300, -150, 5000, 7000), // after bump, low ground level
                new Rectangle(1500 + 300, -200, 1000, 7000), // high ground level
                new Rectangle(1500 + 1500, -200, 300, 7000), // high ground level plateform
                new Rectangle(1500 + 2000, -200, 300, 7000), // high ground level plateform
                new Rectangle(1500 + 2500, -200, 2000, 7000), // end of hole zone, high level
            };

            Stuff = new List<PhysicalObject>()
            {
                new Balloon(new Vector2(-270, -150)), // spawn balloon
                new Balloon(new Vector2(591, -430)), // second balloon


                new Egg(new Vector2(-2000 + 650 + 60, -230)), // left from spawn eggs
                new Egg(new Vector2(-2000 + 650 + 60, -140)), // left from spawn eggs
                new Egg(new Vector2(-2000 + 650 + 60, -40)), // left from spawn eggs

                

                new Egg(new Vector2(-1700, -1000 - 50), 5), // hidden top left
                new Egg(new Vector2(- 200, - 500 - 50), 5) // above spawn
            };

            Stuff.AddRange(EggLine(new Vector2(220, -100), new Vector2(0, -1), 3)); // line mur à droite du spawn

            //Stuff.Clear();
        }

        public void Update(GameTime gameTime, Player player)
        {
            foreach (PhysicalObject o in Stuff) o.Update(gameTime, this, player);
            foreach (PhysicalObject o in StuffToRemove) Stuff.Remove(o);
            StuffToRemove.Clear();
        }

        public bool IsOnInkGround(Vector2 Position)
        {
            Vector2 TestPosition = Position + new Vector2(0, 5);
            foreach(Rectangle r in PaintedGround)
            {
                if (r.Contains(TestPosition)) return true;
            }
            return false;
        }

        public bool IsOnInkWall(Vector2 Position)
        {
            Vector2 TestPosition = Position;
            foreach (Rectangle r in PaintedWalls)
            {
                if (r.Contains(TestPosition)) return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Game1.DrawRectangle(spriteBatch, new Rectangle(-100000, -10000, 200000, 20000), Color.White, background);

            foreach ((Vector2 position, Sprite sprite) decor in Decor)
            {
                decor.sprite.DrawFromFeet(spriteBatch, decor.position);
            }

            // first draw ground
            foreach (Rectangle r in Ground)
            {
                //Game1.DrawRectangle(spriteBatch, r, Color.White, gray);
                Game1.DrawRectangle(spriteBatch, new Rectangle(r.X, r.Y - ground.Height / 2, r.Width, ground.Height), Color.White, ground, true);
            }

            // then draw walls
            foreach (Rectangle r in Ground)
            {
                Game1.DrawRectangle(spriteBatch, new Rectangle(r.X, r.Y - ground.Height / 2, wall.Width, r.Height), Color.White, wall, true);
                Game1.DrawRectangle(spriteBatch, new Rectangle(r.Right - wall.Width, r.Y - ground.Height / 2, wall.Width, r.Height), Color.White, wall, true);
            }

            // then draw corners
            foreach (Rectangle r in Ground)
            {
                // top left corner
               // Game1.DrawRectangle(spriteBatch, new Rectangle(r.X - 1, r.Y - 1 - ground.Height / 2, outer_corner_top_left.Width, outer_corner_top_left.Height), Color.White, outer_corner_top_left, true, true);

            }

            // then draw painted ground
            foreach (Rectangle r in PaintedGround)
            {
                Game1.DrawRectangle(spriteBatch, new Rectangle(r.X, r.Y - ground.Height / 2, r.Width, ground.Height), Color.White, painted_ground, true);
            }

            // then draw painted walls
            foreach (Rectangle r in PaintedWalls)
            {
                Game1.DrawRectangle(spriteBatch, new Rectangle(r.X, r.Y - ground.Height / 2, wall.Width, r.Height), Color.White, painted_wall, true);
            }

            // then draw inner ground
            foreach (Rectangle r in Ground)
            {
                Rectangle InnerGround = r;
                InnerGround.Y += ground.Height / 2;
                InnerGround.Height -= ground.Height / 2;
                InnerGround.X += wall.Width;
                InnerGround.Width -= wall.Width * 2;
                Game1.DrawRectangle(spriteBatch, InnerGround, Color.White, gray);
            }

            // then draw the objects
            foreach (PhysicalObject o in Stuff) o.Draw(spriteBatch);
        }

        public void Paint(Rectangle PaintZone)
        {
            PaintZone.X -= PaintZone.X % 5;
            PaintZone.Width += 5 - PaintZone.Width % 5;
            PaintZone.Y -= PaintZone.Y % 5;
            PaintZone.Height += 5 - PaintZone.Height % 5;

            foreach (Rectangle r in Ground)
            {
                Rectangle Top = r;
                Top.Height = ground.Height;
                if(Top.Intersects(PaintZone))
                {
                    Rectangle PaintedZoneAdded = Rectangle.Intersect(PaintZone, Top);
                    if (PaintedZoneAdded.Height < painted_ground.Height) return;
                    PaintedZoneAdded.Height = painted_ground.Height;
                    PaintedGround.Add(PaintedZoneAdded);
                }

                Rectangle LeftWall = r;
                LeftWall.Width = wall.Width;
                if (LeftWall.Intersects(PaintZone))
                {
                    Rectangle PaintedZoneAdded = Rectangle.Intersect(PaintZone, LeftWall);
                    if (PaintedZoneAdded.Width < wall.Width) return;
                    PaintedZoneAdded.Width = wall.Width;
                    PaintedWalls.Add(PaintedZoneAdded);
                }

                Rectangle RightWall = r;
                RightWall.Width = wall.Width;
                RightWall.X = r.Right - wall.Width;
                if (RightWall.Intersects(PaintZone))
                {
                    Rectangle PaintedZoneAdded = Rectangle.Intersect(PaintZone, RightWall);
                    PaintedZoneAdded.Width = wall.Width;
                    PaintedWalls.Add(PaintedZoneAdded);
                }
            }
        }

        public static void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            gray = Content.Load<Texture2D>("tileset/gray");
            ground = Content.Load<Texture2D>("tileset/ground");
            wall = Content.Load<Texture2D>("tileset/wall");
            background = Content.Load<Texture2D>("tileset/background");
            outer_corner_top_left = Content.Load<Texture2D>("tileset/corner3");
            outer_corner_top_right = Content.Load<Texture2D>("tileset/corner4");
            inner_corner_top_left = Content.Load<Texture2D>("tileset/corner1");
            inner_corner_top_right = Content.Load<Texture2D>("tileset/corner2");
            painted_ground = Content.Load<Texture2D>("tileset/ground_paint");
            painted_wall = Content.Load<Texture2D>("tileset/wall_paint");

            statue1 = new Sprite(Content.Load<Texture2D>("statue1"));
            statue2 = new Sprite(Content.Load<Texture2D>("statue2"));

            Decor = new List<(Vector2, Sprite)>
            {
                (new Vector2( - 400, 0), statue1),
                 (new Vector2( - 900, 0), statue2)
            };
        }

        public bool CheckCollision(Rectangle rectangle, Vector2 movement)
        {
            Rectangle moved_rectangle = rectangle;
            moved_rectangle.Offset(movement);
            bool collision = false;
            foreach (Rectangle r in this.Ground)
            {
                if (moved_rectangle.Intersects(r))
                {
                    collision = true;
                }
            }
            return collision;
        }

        public void Remove(PhysicalObject o)
        {
            StuffToRemove.Add(o);
        }

        public bool CheckCollision(Vector2 FeetPosition, Vector2 Size)
        {
            Rectangle new_rectangle = new Rectangle();
            new_rectangle.X = (int)Math.Floor(FeetPosition.X - Size.X / 2);
            new_rectangle.Y = (int)Math.Floor(FeetPosition.Y - Size.Y);
            new_rectangle.Width = (int)Math.Floor(Size.X);
            new_rectangle.Height = (int)Math.Floor(Size.Y);
            bool collision = false;
            foreach (Rectangle r in this.Ground)
            {
                if (new_rectangle.Intersects(r))
                {
                    collision = true;
                }
            }
            return collision;
        }

        public List<Rectangle> CheckCollisionReturnRectangles(Rectangle rectangle, Vector2 movement)
        {
            Rectangle moved_rectangle = rectangle;
            moved_rectangle.Offset(movement);
            List<Rectangle> ToReturn = new List<Rectangle>() { };
            foreach (Rectangle r in this.Ground)
            {
                if (moved_rectangle.Intersects(r))
                {
                    ToReturn.Add(r);
                }
            }
            return ToReturn;
        }

        private List<Egg> EggLine(Vector2 start, Vector2 dir, int count)
        {
            dir.Normalize();
            List<Egg> eggs = new List<Egg>();
            for (int i = 0; i < count; i++)
            {
                eggs.Add(new Egg(start + dir * 70 * i));
            }
            return eggs;
        }

    }
}
 