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
        public static List<(Vector2, Sprite)> FrontDecor;
        public List<PhysicalObject> Stuff;
        private List<PhysicalObject> StuffToRemove, StuffToAdd;
        private static Texture2D gray, ground, painted_ground, wall, painted_wall, background, painted_ground_enemy, painted_wall_enemy;
        private static Texture2D outer_corner_top_left, outer_corner_top_right, inner_corner_top_left, inner_corner_top_right;
        private static Sprite statue1, statue2, statue3, statue4, statue5, statue6, statue7;
        private static Sprite decor1, decor2, decor3, decor4, decor5, decor6;

        public World(Player player)
        {
            StuffToRemove = new List<PhysicalObject>();
            StuffToAdd = new List<PhysicalObject>();
            PaintedGround = new List<(Rectangle, bool)>();
            PaintedWalls = new List<(Rectangle, bool)>();
            Ground = new List<Rectangle>()
                        {
                            new Rectangle(-1000, -60, 300, 200) ,
                            new Rectangle(-3200, 0, 3000, 200) ,
                            new Rectangle(-4000, -2000, 1100, 7000) ,
                            new Rectangle(-4650, -500, 3650, 200) ,
                            new Rectangle(-2075, -1275, 550, 75) ,
                            new Rectangle(-3000, -1000, 650, 7000) ,
                            new Rectangle(-5000, -1600, 3000, 400) ,
                            new Rectangle(-1825, -1600, 425, 65) ,
                            new Rectangle(-525, -1600, 500, 70) ,
                            new Rectangle(-575, -1600, 100, 280) ,
                            new Rectangle(-575, -1375, 400, 190) ,
                            new Rectangle(-2975, -1950, 450, 420) ,
                            new Rectangle(2250, -700, 200, 350) ,
                            new Rectangle(2350, -700, 600, 150) ,
                            new Rectangle(3500, -200, 300, 7000) ,
                            new Rectangle(4000, -200, 2000, 7000) ,
                            new Rectangle(4100, -625, 100, 150) ,
                            new Rectangle(3850, -675, 350, 80) ,
                            new Rectangle(4300, -500, 100, 110) ,
                            new Rectangle(4100, -550, 300, 70) ,
                            new Rectangle(4300, -425, 300, 75) ,
                            new Rectangle(5775, -650, 500, 765) ,
                            new Rectangle(4675, -875, 500, 745) ,
                            new Rectangle(5100, -800, 450, 940) ,
                            new Rectangle(5350, -725, 550, 790) ,
                            new Rectangle(6000, -400, 875, 330) ,
                            new Rectangle(6450, -1025, 125, 365) ,
                            new Rectangle(5700, -1400, 475, 240) ,
                            new Rectangle(6100, -1400, 125, 440) ,
                            new Rectangle(4075, -1400, 100, 290) ,
                            new Rectangle(4100, -1400, 400, 110) ,
                            new Rectangle(6500, -875, 500, 215) ,
                            new Rectangle(3000, -200, 300, 7000) ,
                            new Rectangle(5225, -1400, 200, 120) ,
                            new Rectangle(4775, -1400, 200, 110) ,
                            new Rectangle(3475, -1200, 700, 115) ,
                            new Rectangle(225, -1900, 1775, 715) ,
                            new Rectangle(1950, -1300, 375, 110) ,
                            new Rectangle(-3050, -2400, 11650, 535) ,
                            new Rectangle(7925, -1925, 775, 345) ,
                            new Rectangle(7225, -775, 550, 95) ,
                            new Rectangle(7475, -725, 475, 1020) ,
                            new Rectangle(7675, -1675, 925, 2555) ,
                            new Rectangle(800, -150, 6900, 7000) ,
                            new Rectangle(1850, -200, 950, 120) ,
                            new Rectangle(-750, -300, 2725, 500) ,
                            new Rectangle(1100, -335, 650, 500) ,
                        };
            Stuff = new List<PhysicalObject>()
            {
                new Zapfish(new Vector2(1213 - 1000, -570)),
                new Bumper(new Vector2(1874- 1000, -419), -0.3f),
                new Bumper(new Vector2(1510- 1000, -580), 0.3f),
                new Bumper(new Vector2(-960- 1000, -940), (float)Math.PI/2), // full right
                new Bumper(new Vector2(-490- 1000, -940),  0), // full right 2
                new Bumper(new Vector2(1047- 1000, -1121),  -0.45f), 
                new Bumper(new Vector2(3100 + 80, -1173 - 120 + 80),  -0.76f), // secret tofu access
                new Bumper(new Vector2(2759, -1444 - 120),  -1.7f), // secret tofu access 2
                //new Bumper(new Vector2(2620, -830),  0f), // secret tofu access bottom to ink
                new Bumper(new Vector2(-5 - 1000, -1526), turning:true),

                new Frogtarian(new Vector2(- 1000-400, -1000)),
                new Frogtarian(new Vector2(3637, -235)),
                new Frogtarian(new Vector2(4279, -1448)),
                new Frogtarian(new Vector2(4935, -906)),
                new Frogtarian(new Vector2(6659, -431)),
                new Frogtarian(new Vector2(1695, -371)),
                
                new BigFrogarian(new Vector2(4007, -675)),
                new BigFrogarian(new Vector2(5713, -725)),
                new BigFrogarian(new Vector2(7228, -150)),
                new BigFrogarian(new Vector2(5316, -1400)),
                new BigFrogarian(new Vector2(6870 + 50, -875)),

                new Balloon(new Vector2(- 1000 + -270, -150)), // spawn balloon
                new Balloon(new Vector2(- 1000 + 591, -430)), // second balloon
                new Balloon(new Vector2(2886 - 300, -430)), // third ballon, hold field
                new Balloon(new Vector2(3393, -430)), // fourth ballon, hold field
                new Balloon(new Vector2(3887, -430)), // fifth ballon, hold field

                new Balloon(new Vector2(2041, -531)), // ballon at intersection top/bottom
                new Balloon(new Vector2(1841, -827)), // ballon jsut above
                new Balloon(new Vector2(7353, -1030)), // rabbit 1
                new Balloon(new Vector2(7549, -1030)), // rabbit 2
                //new Balloon(new Vector2(-490, -1100)), // above bumper
                new Balloon(new Vector2(4872, -1580)), // on plateform above in plateform series
                new Balloon(new Vector2(2940, -1420)), // above bumper right
                new Balloon(new Vector2(0- 1000, -1745)), // above bumper turning
                new Balloon(new Vector2(1475- 1000, -860)), // above statue have to use bumper
                new Balloon(new Vector2(6330, -836)), // on narrow ascending space to rabbit
                new Balloon(new Vector2(4288, -1700)), // above highest frogarian
                new Balloon(new Vector2(1390, -651)), // above new ground


                new Egg(new Vector2(- 1000-2000 + 650 + 60, -230)), // left from spawn eggs
                new Egg(new Vector2(- 1000-2000 + 650 + 60, -140)), // left from spawn eggs
                new Egg(new Vector2(- 1000-2000 + 650 + 60, -40)), // left from spawn eggs

                new Marina(new Vector2(823- 1000, -317)),
                new Pearl(new Vector2(-1382- 1000, -1608)),
                new Tofu(new Vector2(7841, -1689)),
                
                // egg boxes
                new Egg(new Vector2(- 1000-1700, -1000 - 50), 5), // hidden top left
                new Egg(new Vector2(- 1000-1268, -571), 5), // above spawn
                new Egg(new Vector2(2986, -472), 5), // above hole field, have to drop from above
                new Egg(new Vector2(3150, -818), 5), // above hole field, have to jump
                new Egg(new Vector2(3630, -810), 5), // above hole field on right, have to jump
                new Egg(new Vector2(3780, -1476), 5), // on plateform high above hole field
                new Egg(new Vector2(520 + 70- 1000, -1429), 5), // hidden stash in small cove end game
                new Egg(new Vector2(570 + 70- 1000, -1459), 5), // hidden stash in small cove end game
                new Egg(new Vector2(620 + 70- 1000, -1429), 5), // hidden stash in small cove end game
                new Egg(new Vector2(0, -0) + new Vector2(2132, -1380), 5), // hidden stash in high cove end game
                // new Egg(new Vector2(50, -30) + new Vector2(2132, -1380), 5), // hidden stash in high cove end game
                new Egg(new Vector2(100, -0)+ new Vector2(2132, -1380), 5), // hidden stash in high cove end game
                new Egg(new Vector2(1668- 1000, -676), 5), // above statue have to use bumper
            };

            Bell bell = new Bell(new Vector2(1348 - 1000, -300 - 83));
            Stuff.Add(bell);
            Stuff.Add(new Rabbit(new Vector2(7500, -791), bell));

            Stuff.AddRange(EggLine(new Vector2(220 - 1000, -100), new Vector2(0, -1), 3)); // line mur � droite du spawn
            Stuff.AddRange(EggLine(new Vector2(2300, -748), new Vector2(1, 0), 9)); // plateform above before hole filed
            Stuff.AddRange(EggLine(new Vector2(3389, -1218), new Vector2(0, 1), 9)); // Falling in middle of hole field
            Stuff.AddRange(EggLine(new Vector2(7440, -660), new Vector2(0, 1), 6)); // On wall bottom full right
            Stuff.AddRange(EggLine(new Vector2(5025, -1501), new Vector2(1, 0), 3)); // au dessus du vide en jump des plateformes
            Stuff.AddRange(EggLine(new Vector2(4557, -1501), new Vector2(1, 0), 3)); // deuxi�me au dessus du vide en jump des plateformes
            Stuff.AddRange(EggLine(new Vector2(-970 - 1000, -1330), new Vector2(1, 0), 7)); // above after bumpers
            Stuff.AddRange(EggLine(new Vector2(1668 - 1000, -626), new Vector2(0, 1), 5)); // statue below eggbox

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

            foreach ((Vector2 position, Sprite sprite) decor in FrontDecor)
            {
                decor.sprite.DrawFromFeet(spriteBatch, decor.position);
            }
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
            statue3 = new Sprite(Content.Load<Texture2D>("statue3"));
            statue4 = new Sprite(Content.Load<Texture2D>("statue4"));
            statue5 = new Sprite(Content.Load<Texture2D>("statue5"));
            statue6 = new Sprite(Content.Load<Texture2D>("statue6"));
            statue7 = new Sprite(Content.Load<Texture2D>("statue7"));

            decor1 = new Sprite(Content.Load<Texture2D>("decor/1"));
            decor2 = new Sprite(Content.Load<Texture2D>("decor/2"));
            decor3 = new Sprite(Content.Load<Texture2D>("decor/3"));
            decor4 = new Sprite(Content.Load<Texture2D>("decor/4"));
            decor5 = new Sprite(Content.Load<Texture2D>("decor/5"));
            decor6 = new Sprite(Content.Load<Texture2D>("decor/6"));

            Decor = new List<(Vector2, Sprite)>
            {
                (new Vector2(-1000 +  - 400, 0), statue1),
                (new Vector2(-1000 + 430, -300), statue1),
                (new Vector2(-1000 + 1050, -300), statue1),
                (new Vector2(-1000 + 1050 + 1000 - 380, -300), statue1),
                (new Vector2(-1000 +  - 900, 0), statue1),
                (new Vector2(2100, -180), statue1),

                (new Vector2(2626, -190), statue1),
                (new Vector2(3140, -190), statue4),
                (new Vector2(3644, -190), Flip(statue4)),
                (new Vector2(4154, -190), statue1),
                (new Vector2(4907, -870), statue2),
                (new Vector2(5347, -800), statue2),
                (new Vector2(5713, -720), statue2),
                (new Vector2(6073, -640), statue2),
                (new Vector2(6560, -400), statue1),
                (new Vector2(6784, -860), statue1),
                (new Vector2(3757, -1190), statue3),
                (new Vector2(4300, -1400), statue1),
                (new Vector2(-1000 + -663, -505), statue5),
                (new Vector2(-1000 + -1055, -505), statue1),
                (new Vector2(-1000 + -663 + 392, -505), statue1),
                (new Vector2(5954, -1380), statue6),
                (new Vector2(2601, -700), statue1),
                (new Vector2(-1000 + -766, -1270), statue1),
                (new Vector2(-1000 + -1177, -1580), statue1),
                (new Vector2(-1000 + -613, -1580), statue1),
                (new Vector2(-1000 + 699, -1580), statue1),
                (new Vector2(4011, -683), statue1),
                (new Vector2(7155, -149), statue6),
                (new Vector2(2155, -1280), statue6),
                (new Vector2(1420, -337), statue7),
            };

            FrontDecor = new List<(Vector2, Sprite)>
            {
                (new Vector2(-1000 + 191, 100), decor2),
                (new Vector2(-1000 + -2080, -958), decor2),
                (new Vector2(-1000 + 416, -114), decor1),
                (new Vector2(-1000 + -1639, -284), decor1),
                (new Vector2(-1000 + -349, -340), decor3),
                (new Vector2(-1000 + -1604, -765), decor4),
                (new Vector2(-1000 + -866, -350), decor5),

                (new Vector2(3633, -18), decor5),
                (new Vector2(2344, -471), decor4),
                (new Vector2(-1000 + 1866, -103), decor3),
                (new Vector2(-1000 + -1403, 136), decor3),

                (new Vector2(6078, -1225), decor1),
                (new Vector2(5761, -578), decor2),
                (new Vector2(6622, -724), decor3),
                (new Vector2(6677, -210), decor4),
                (new Vector2(7389, 0), decor5),
                (new Vector2(7725, -444), decor1),
                (new Vector2(7965, -752), decor2),
                (new Vector2(4929, -675), decor6),

                (new Vector2(1806, -1327), decor3),
                (new Vector2(1532, -1751), decor1),
                (new Vector2(1464, -1248), decor2),
                (new Vector2(2344, -1951), decor4),
                (new Vector2(619, -1217), decor1),
                (new Vector2(1452, -1493), decor1),
                (new Vector2(1579, -1716), decor1),
                (new Vector2(-1000 + -1274, -1308), decor6),
                (new Vector2(-1000 + -1710, -1461), decor2),
                (new Vector2(-1000 + -1707, -1894), decor1),
                (new Vector2(-1000 + -938, -1938), decor4),
                (new Vector2(7975, -1463), decor6),
                (new Vector2(4917, -112), decor4),

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

        static Sprite Flip(Sprite s)
        {
            Sprite flipped = new Sprite(s);
            flipped.direction *= -1;
            flipped.UpdateFrame(new GameTime());
            return flipped;
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
 