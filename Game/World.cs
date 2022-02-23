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
        private static Texture2D gray, ground, painted_ground, wall, painted_wall;
        private static Texture2D outer_corner_top_left, outer_corner_top_right, inner_corner_top_left, inner_corner_top_right;
        private static Sprite statue1;

        public World(Player player)
        {
            Stuff = new List<PhysicalObject>();
            StuffToRemove = new List<PhysicalObject>();
            PaintedGround = new List<Rectangle>();
            PaintedWalls = new List<Rectangle>();
            Ground = new List<Rectangle>()
            {
                new Rectangle(-200, 0, 2000, 200), // ground
                new Rectangle(-800, -800, 650, 2000), // left wall
                new Rectangle(550, -100, 300, 200), // right bump
            };
        }

        public void Update(GameTime gameTime, Player player)
        {
            foreach (PhysicalObject o in Stuff) o.Update(gameTime, this, player);
            foreach (PhysicalObject o in StuffToRemove) Stuff.Remove(o);
            StuffToRemove.Clear();
        }

        public bool IsOnInk(Vector2 Position)
        {
            Vector2 TestPosition = Position + new Vector2(0, 5);
            foreach(Rectangle r in PaintedGround)
            {
                if (r.Contains(TestPosition)) return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach ((Vector2 position, Sprite sprite) decor in Decor)
            {
                decor.sprite.DrawFromFeet(spriteBatch, decor.position);
            }

            /*
            List<Rectangle> Tops = new List<Rectangle>();
            foreach (Rectangle r1 in Ground)
            {
                Rectangle NewTop = new Rectangle(r1.X, r1.Y, r1.Width, ground.Height);
                foreach (Rectangle r2 in Ground)
                {
                    if(r2 != r1)
                    {
                        if (r2.Contains(NewTop))
                        {
                            NewTop = Rectangle.Empty;
                            break;
                        }
                        else if (r2.Intersects(NewTop))
                        {
                            if (NewTop.Left < r2.Right && r2.Left < NewTop.Right) // top on both sides
                            {

                            }
                            
                         }
                    }
                }
                Tops.Add();
            }
            */

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
            foreach(Rectangle r in Ground)
            {
                Rectangle Top = r;
                Top.Height = ground.Height;
                if(Top.Intersects(PaintZone))
                {
                    Rectangle PaintedZoneAdded = Rectangle.Intersect(PaintZone, Top);
                    PaintedZoneAdded.Height = painted_ground.Height;
                    PaintedGround.Add(PaintedZoneAdded);
                }

                Rectangle LeftWall = r;
                LeftWall.Width = wall.Width;
                if (LeftWall.Intersects(PaintZone))
                {
                    Rectangle PaintedZoneAdded = Rectangle.Intersect(PaintZone, LeftWall);
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
            outer_corner_top_left = Content.Load<Texture2D>("tileset/corner3");
            outer_corner_top_right = Content.Load<Texture2D>("tileset/corner4");
            inner_corner_top_left = Content.Load<Texture2D>("tileset/corner1");
            inner_corner_top_right = Content.Load<Texture2D>("tileset/corner2");
            painted_ground = Content.Load<Texture2D>("tileset/ground_paint");
            painted_wall = Content.Load<Texture2D>("tileset/wall_paint");

            statue1 = new Sprite(Content.Load<Texture2D>("statue1"));

            Decor = new List<(Vector2, Sprite)>
            {
                (new Vector2(400, 0), statue1)
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


    }
}
 