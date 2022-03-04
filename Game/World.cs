using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Splatoon2D
{
    public class World
    {
        public List<Rectangle> Ground;
        public List<(Rectangle, bool)> PaintedGround;
        public List<(Rectangle, bool)> PaintedWalls;
        public static List<(Vector2, Sprite)> Decor;
        public List<PhysicalObject> Stuff;
        private List<PhysicalObject> StuffToRemove, StuffToAdd;
        private static Texture2D gray, ground, painted_ground, wall, painted_wall, background, painted_ground_enemy, painted_wall_enemy;
        private static Texture2D outer_corner_top_left, outer_corner_top_right, inner_corner_top_left, inner_corner_top_right;
        private static Sprite statue1, statue2;

        public World(Player player)
        {
            StuffToRemove = new List<PhysicalObject>();
            StuffToAdd = new List<PhysicalObject>();
            PaintedGround = new List<(Rectangle, bool)>();
            PaintedWalls = new List<(Rectangle, bool)>();
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

                new Rectangle(2250, - 700, 200, 350), // plateform above holes
                new Rectangle(2250 + 100, - 700, 700 - 100, 150),
            };

            Stuff = new List<PhysicalObject>()
            {
                new Frogtarian(new Vector2(-400, 0)),
                new Frogtarian(new Vector2(-400, -1000)),
                new Frogtarian(new Vector2(3637, -235)),

                new Balloon(new Vector2(-270, -150)), // spawn balloon
                new Balloon(new Vector2(591, -430)), // second balloon
                new Balloon(new Vector2(2886, -430)), // third ballon, hold field
                new Balloon(new Vector2(3393, -430)), // fourth ballon, hold field
                new Balloon(new Vector2(3887, -430)), // fifth ballon, hold field


                new Egg(new Vector2(-2000 + 650 + 60, -230)), // left from spawn eggs
                new Egg(new Vector2(-2000 + 650 + 60, -140)), // left from spawn eggs
                new Egg(new Vector2(-2000 + 650 + 60, -40)), // left from spawn eggs

                new Bell(new Vector2(1348, -300 - 83)),
                new Marina(new Vector2(823, -317)),
                
                // egg boxes
                new Egg(new Vector2(-1700, -1000 - 50), 5), // hidden top left
                new Egg(new Vector2(- 200, - 500 - 50), 5), // above spawn
                new Egg(new Vector2(2986, -472), 5) // above hole field, have to drop from above
            };

            Stuff.AddRange(EggLine(new Vector2(220, -100), new Vector2(0, -1), 3)); // line mur à droite du spawn
            Stuff.AddRange(EggLine(new Vector2(2300, -748), new Vector2(1, 0), 5)); // plateform above before hole filed

            //Stuff.Clear();
        }

        public void Update(GameTime gameTime, Player player)
        {
            foreach (PhysicalObject o in Stuff) o.Update(gameTime, this, player);
            foreach (PhysicalObject o in StuffToRemove) Stuff.Remove(o);
            foreach (PhysicalObject o in StuffToAdd) Stuff.Add(o);

            StuffToRemove.Clear();
            StuffToAdd.Clear();
        }

        public bool IsOnInkGround(Vector2 Position, bool test_if_enemy = false)
        {
            Vector2 TestPosition = Position + new Vector2(0, 5);
            foreach((Rectangle r, bool enemy_ink) in PaintedGround)
            {
                if (r.Contains(TestPosition) && enemy_ink == test_if_enemy) return true;
            }
            return false;
        }

        public bool IsOnInkWall(Vector2 Position)
        {
            Vector2 TestPosition = Position;
            foreach ((Rectangle r, bool enemy_ink) in PaintedWalls)
            {
                if (r.Contains(TestPosition) && !enemy_ink) return true;
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
            foreach ((Rectangle r, bool enemy_ink) in PaintedGround)
            {
                if(enemy_ink)
                    Game1.DrawRectangle(spriteBatch, new Rectangle(r.X, r.Y - ground.Height / 2, r.Width, ground.Height), Color.White, painted_ground_enemy, true);
                else
                    Game1.DrawRectangle(spriteBatch, new Rectangle(r.X, r.Y - ground.Height / 2, r.Width, ground.Height), Color.White, painted_ground, true);
            }

            // then draw painted walls
            foreach ((Rectangle r, bool enemy_ink) in PaintedWalls)
            {
                if(enemy_ink)
                    Game1.DrawRectangle(spriteBatch, new Rectangle(r.X, r.Y - ground.Height / 2, wall.Width, r.Height), Color.White, painted_wall_enemy, true);
                else     
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

        public void Paint(Rectangle PaintZone, bool enemy = false)
        {
            PaintZone.X -= PaintZone.X % 5;
            PaintZone.Width += 5 - PaintZone.Width % 5;
            PaintZone.Y -= PaintZone.Y % 5;
            PaintZone.Height += 5 - PaintZone.Height % 5;
            List<Rectangle> SmallListOfAddedPaintedGrounds = new List<Rectangle>();
            List<Rectangle> SmallListOfAddedPaintedWalls = new List<Rectangle>();

            foreach (Rectangle r in Ground)
            {
                Rectangle Top = r;
                Top.Height = ground.Height;
                if(Top.Intersects(PaintZone))
                {
                    Rectangle PaintedZoneAdded = Rectangle.Intersect(PaintZone, Top);
                    if (PaintedZoneAdded.Height < painted_ground.Height) break;
                    PaintedZoneAdded.Height = painted_ground.Height;
                    SmallListOfAddedPaintedGrounds.Add(PaintedZoneAdded);
                }

                Rectangle LeftWall = r;
                LeftWall.Width = wall.Width;
                if (LeftWall.Intersects(PaintZone))
                {
                    Rectangle PaintedZoneAdded = Rectangle.Intersect(PaintZone, LeftWall);
                    if (PaintedZoneAdded.Width < wall.Width) break ;
                    PaintedZoneAdded.Width = wall.Width;
                    SmallListOfAddedPaintedWalls.Add(PaintedZoneAdded);
                }

                Rectangle RightWall = r;
                RightWall.Width = wall.Width;
                RightWall.X = r.Right - wall.Width;
                if (RightWall.Intersects(PaintZone))
                {
                    Rectangle PaintedZoneAdded = Rectangle.Intersect(PaintZone, RightWall);
                    if (PaintedZoneAdded.Width < wall.Width) break;
                    PaintedZoneAdded.Width = wall.Width;
                    SmallListOfAddedPaintedWalls.Add(PaintedZoneAdded);
                }
            }

            // if the new ink landed on some opposite team ink, we need to remove/reduce the old ink

            // first for the ground:
            List<(Rectangle, bool)> PaintedGroundToRemove = new List<(Rectangle, bool)>();
            List<(Rectangle, bool)> PaintedGroundToAdd = new List<(Rectangle, bool)>();

            foreach (Rectangle newly_painted_wall in SmallListOfAddedPaintedGrounds)
            {
                Rectangle rectangle_to_check_for_closing_gaps = Rectangle.Empty;

                foreach ((Rectangle r, bool enemy_ink) in PaintedGround)
                {
                    if (enemy_ink != enemy)
                    {
                        if (newly_painted_wall.Intersects(r))
                        {
                            PaintedGroundToRemove.Add((r, enemy_ink));
                            if (r.Left < newly_painted_wall.Left)
                                PaintedGroundToAdd.Add((new Rectangle(r.X, r.Y, newly_painted_wall.Left - r.Left, r.Height), enemy_ink));
                            if (newly_painted_wall.Right < r.Right)
                                PaintedGroundToAdd.Add((new Rectangle(newly_painted_wall.Right, r.Y, r.Right - newly_painted_wall.Right, r.Height), enemy_ink));
                        }
                    }
                    else
                    {
                        if(r.Intersects(newly_painted_wall))
                        {
                            PaintedGroundToRemove.Add((r, enemy_ink));
                            PaintedGroundToRemove.Add((newly_painted_wall, enemy_ink));
                            if (rectangle_to_check_for_closing_gaps != Rectangle.Empty) // a fusion occured in the very same loop! closing gap
                            {
                                PaintedGroundToAdd.Remove((rectangle_to_check_for_closing_gaps, enemy));
                                PaintedGroundToAdd.Add((Rectangle.Union(r, rectangle_to_check_for_closing_gaps), enemy));
                            }
                            else
                            {
                                rectangle_to_check_for_closing_gaps = Rectangle.Union(r, newly_painted_wall);
                                PaintedGroundToAdd.Add((rectangle_to_check_for_closing_gaps, enemy));
                            }
                        }
                    }
                }
            }

            foreach (Rectangle r in SmallListOfAddedPaintedGrounds) PaintedGroundToAdd.Add((r, enemy));

            foreach ((Rectangle, bool) zone in PaintedGroundToAdd) PaintedGround.Add(zone);
            foreach ((Rectangle, bool) zone in PaintedGroundToRemove) PaintedGround.Remove(zone);

            // then for the walls
            List<(Rectangle, bool)> PaintedWallsToRemove = new List<(Rectangle, bool)>();
            List<(Rectangle, bool)> PaintedWallsToAdd = new List<(Rectangle, bool)>();

            foreach (Rectangle newly_painted_wall in SmallListOfAddedPaintedWalls)
            {
                Rectangle rectangle_to_check_for_closing_gaps = Rectangle.Empty;

                foreach ((Rectangle r, bool enemy_ink) in PaintedWalls)
                {
                    if (enemy_ink != enemy)
                    {
                        if (newly_painted_wall.Intersects(r))
                        {
                            PaintedWallsToRemove.Add((r, enemy_ink));
                            if (r.Top < newly_painted_wall.Top)
                                PaintedWallsToAdd.Add((new Rectangle(r.X, r.Y, r.Width, newly_painted_wall.Top - r.Top), enemy_ink));
                            if (newly_painted_wall.Bottom < r.Bottom)
                                PaintedWallsToAdd.Add((new Rectangle(r.X, newly_painted_wall.Bottom, r.Width, r.Bottom - newly_painted_wall.Bottom), enemy_ink));
                        }
                    }
                    else
                    {
                        if (r.Intersects(newly_painted_wall))
                        {
                            PaintedWallsToRemove.Add((r, enemy_ink));
                            PaintedWallsToRemove.Add((newly_painted_wall, enemy_ink));
                            if (rectangle_to_check_for_closing_gaps != Rectangle.Empty) // a fusion occured in the very same loop! closing gap
                            {
                                PaintedWallsToAdd.Remove((rectangle_to_check_for_closing_gaps, enemy));
                                PaintedWallsToAdd.Add((Rectangle.Union(r, rectangle_to_check_for_closing_gaps), enemy));
                            }
                            else
                            {
                                rectangle_to_check_for_closing_gaps = Rectangle.Union(r, newly_painted_wall);
                                PaintedWallsToAdd.Add((rectangle_to_check_for_closing_gaps, enemy));
                            }
                        }
                    }
                }
            }

            foreach (Rectangle r in SmallListOfAddedPaintedWalls) PaintedWallsToAdd.Add((r, enemy));

            foreach ((Rectangle, bool) zone in PaintedWallsToAdd) PaintedWalls.Add(zone);
            foreach ((Rectangle, bool) zone in PaintedWallsToRemove) PaintedWalls.Remove(zone);


            Console.WriteLine("Total paint rectangles: ");
            Console.WriteLine("     Ground: " + PaintedGround.Count);
            Console.WriteLine("     Wall: " + PaintedWalls.Count);
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
            painted_ground_enemy = Content.Load<Texture2D>("tileset/ground_paint_green");
            painted_wall_enemy = Content.Load<Texture2D>("tileset/wall_paint_green");


            statue1 = new Sprite(Content.Load<Texture2D>("statue1"));
            statue2 = new Sprite(Content.Load<Texture2D>("statue2"));

            Decor = new List<(Vector2, Sprite)>
            {
                (new Vector2( - 400, 0), statue1),
                (new Vector2(430, -300), statue1),
                (new Vector2(1050, -300), statue1),
                (new Vector2(1050 + 1000 - 380, -300), statue1),
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

        public void Spawn(PhysicalObject o)
        {
            StuffToAdd.Add(o);
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
 