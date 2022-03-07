using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;

namespace Splatoon2D
{
    public static class DebugManager
    {
        // This class was taken and adapted from Mask Up editor
#if DEBUG
       
        public static bool editor_enabled = true, show_hitbox = false, show_anchors = false, show_enemies = false, dezoom = false, show_infos = false, show_ground = false;
        public static bool is_placing_object, is_placing_ground, is_placing_exit, is_placing_anchors, play, realtime = false;
        private static ValueTuple<int, Vector2> PlacedObject, SelectedObject = (666, Vector2.Zero); // Placed_ = thing (object or ground rectangle) currently edited/being placed
        private static List<ValueTuple<int, Vector2>> EditedRoomObjects;
        public static Rectangle PlacedGround, SelectedGround; // Selected_ = thing selected with the mouse
        public static KeyboardState old_ks, ks;
        public static MouseState old_ms, ms;
        private static Point RectangleInitPoint, PlacedExit;
        private static Game1 LocalGame;
        private static Player LocalPlayer;
        private static World LocalWorld;
        private static int EntityLoaded, ParticlesCount, RectangleMoveCD, theme_index = 0;

        public static void Update(Game1 game, GameTime gameTime, World world, Player player, KeyboardState ks, MouseState ms)
        {
            // Console.WriteLine(SelectedObject.ToString());

            LocalGame = game; // hhhhhhhhhhhhhh
            LocalWorld = world;
            LocalPlayer = player;

            old_ms = DebugManager.ms;
            DebugManager.ms = ms;
            old_ks = DebugManager.ks;
            DebugManager.ks = ks;

            if (JustPressed(Keys.F1)) show_hitbox = !show_hitbox;
            if (JustPressed(Keys.F2)) show_anchors = !show_anchors;
            if (JustPressed(Keys.F3)) editor_enabled = !editor_enabled;
            if (JustPressed(Keys.F4)) dezoom = !dezoom;
            if (JustPressed(Keys.F5)) show_infos = !show_infos;
            if (JustPressed(Keys.F6)) show_ground = !show_ground;
            if (JustPressed(Keys.F7)) Camera.static_cam = !Camera.static_cam;
            if (JustPressed(Keys.F11)) game.graphics.ToggleFullScreen();
            if (JustPressed(Keys.C)) world.PaintedGround.Clear();
            if (JustPressed(Keys.C)) world.PaintedWalls.Clear();
            if (JustPressed(Keys.V)) world.Paint(new Rectangle(-10000, -10000,  20000, 20000));
            if (JustPressed(Keys.B)) Bumper.unlocked = !Bumper.unlocked;
            if (JustPressed(Keys.X))
            {
                foreach (PhysicalObject o in world.Stuff) if (o is Hittable h) h.Die(world);
            }

            if (!editor_enabled) return;

            if (JustPressed(Keys.P)) PrintRoom();
            

            //Vector2 ScreenMousePosition = ms.Position.ToVector2() * 1 / Camera.Zoom + Camera.TopLeftCameraPosition;
            Vector2 ScreenMousePosition = HUD.GetPointerWorldVector(ms.Position.ToVector2());
            if (ks.IsKeyDown(Keys.LeftShift)) ScreenMousePosition = (ScreenMousePosition - new Vector2(ScreenMousePosition.X % 5, ScreenMousePosition.Y % 5));
            if (JustPressed(Keys.O))
            {
                is_placing_ground = !is_placing_ground;
                if (!is_placing_ground) PlacedGround = new Rectangle(0, 0, 0, 0);
            }
            if (is_placing_ground)
            {
                if (Click())
                {
                    PlacedGround.X = (int)(ScreenMousePosition.X - ScreenMousePosition.X % 25);
                    PlacedGround.Y = (int)(ScreenMousePosition.Y - ScreenMousePosition.Y % 25);
                    RectangleInitPoint = new Point(PlacedGround.X, PlacedGround.Y);
                }
                else if (ms.LeftButton == ButtonState.Pressed)
                {
                    Point ScreenMouseGridPos = new Point((int)(ScreenMousePosition.X - ScreenMousePosition.X % 25),
                        (int)(ScreenMousePosition.Y - ScreenMousePosition.Y % 10));
                    //PlacedGround.Width = (int)(ScreenMousePosition.X - PlacedGround.X);
                    if (ScreenMouseGridPos.X > RectangleInitPoint.X)
                    {
                        PlacedGround.X = RectangleInitPoint.X;
                        PlacedGround.Width = ScreenMouseGridPos.X - PlacedGround.X;
                    }
                    else
                    {
                        PlacedGround.X = ScreenMouseGridPos.X;
                        PlacedGround.Width = RectangleInitPoint.X - ScreenMouseGridPos.X;
                    }
                    if (ScreenMouseGridPos.Y > RectangleInitPoint.Y)
                    {
                        PlacedGround.Y = RectangleInitPoint.Y;
                        PlacedGround.Height = ScreenMouseGridPos.Y - PlacedGround.Y;
                    }
                    else
                    {
                        PlacedGround.Y = ScreenMouseGridPos.Y;
                        PlacedGround.Height = RectangleInitPoint.Y - ScreenMouseGridPos.Y;
                    }
                }
                else if (PlacedGround.Width > 25 && PlacedGround.Height > 25)
                {
                    world.Ground.Add(PlacedGround);
                    PlacedGround = new Rectangle((int)(ScreenMousePosition.X - ScreenMousePosition.X % 25),
                                                    (int)(ScreenMousePosition.Y - ScreenMousePosition.Y % 25),
                                                    25, 25);
                }
                else PlacedGround = new Rectangle((int)(ScreenMousePosition.X - ScreenMousePosition.X % 25),
                                                    (int)(ScreenMousePosition.Y - ScreenMousePosition.Y % 25),
                                                    25, 25);
            }
            
            else
            {
                if (SelectedGround != Rectangle.Empty)
                {
                    if (JustPressed(Keys.Delete)) SelectedGround = Rectangle.Empty;

                    /*
                    SelectedGround.Location += new Point((int)(4 * GetMovement().X - (4 * GetMovement().X) % 5),
                                                            (int)(4 * GetMovement().Y - (4 * GetMovement().Y) % 5)); 
                */
                    if (GetMovement() != Vector2.Zero)
                    {
                        if (RectangleMoveCD <= 0)
                        {
                            int MagicOffsetX = (int)(GetMovement().X);
                            int MagicOffsetY = (int)(GetMovement().Y);

                            if (ks.IsKeyDown(Keys.Tab)) // inflate
                            {
                                if (GetMovement().X != 0)
                                {
                                    SelectedGround.Width += MagicOffsetX * 25;
                                    if (GetMovement().X < 0)
                                    {
                                        SelectedGround.Width -= 2 * MagicOffsetX * 25;
                                        SelectedGround.X += MagicOffsetX * 25;
                                    }
                                }
                                if (GetMovement().Y != 0)
                                {
                                    SelectedGround.Height += MagicOffsetY * 25;
                                    if (GetMovement().Y < 0)
                                    {
                                        SelectedGround.Height -= 2 * MagicOffsetY * 25;
                                        SelectedGround.Y += MagicOffsetY * 25;
                                    }
                                }
                            }
                            else if (ks.IsKeyDown(Keys.LeftShift)) // deflate/decrease size
                            {
                                if (GetMovement().X != 0)
                                {
                                    SelectedGround.Width -= Math.Abs(MagicOffsetX * 25);
                                    if (GetMovement().X > 0)
                                    {
                                        SelectedGround.X += MagicOffsetX * 25;
                                    }
                                }
                                if (GetMovement().Y != 0)
                                {
                                    SelectedGround.Height -= Math.Abs(MagicOffsetY * 25);
                                    if (GetMovement().Y > 0)
                                    {
                                        SelectedGround.Y += MagicOffsetY * 25;
                                    }
                                }
                            }
                            else // move
                            {
                                SelectedGround.Location += new Point((int)(25 * GetMovement().X),
                                                                (int)(25 * GetMovement().Y));
                            }
                            RectangleMoveCD = 5;
                        }
                        else RectangleMoveCD--;
                    }
                    else RectangleMoveCD = 0;
                }
                if (Click())
                {
                    if (SelectedGround != Rectangle.Empty)
                    {
                        world.Ground.Add(SelectedGround);
                    }
                    SelectedObject.Item1 = 666;
                    SelectedGround = Rectangle.Empty;

                    
                        /*foreach (Room r in world.Rooms) */
                        foreach (Rectangle g in world.Ground)
                        {
                            if (g.Contains(ScreenMousePosition))
                            {
                                SelectedGround = g;
                                world.Ground.Remove(g); // not sure this works
                                break;
                            }
                        }
                }


            }
        }

        public static bool Click()
        {
            return (old_ms.LeftButton == ButtonState.Released && ms.LeftButton == ButtonState.Pressed);
        }
        public static bool Hold()
        {

            return (old_ms.Position != ms.Position && old_ms.LeftButton == ButtonState.Pressed && ms.LeftButton == ButtonState.Pressed);
        }

        public static bool ScrollValueChanged()
        {
            return (old_ms.ScrollWheelValue != ms.ScrollWheelValue);
        }

        public static Vector2 GetMovement()
        {
            Vector2 Movement = Vector2.Zero;
            if (ks.IsKeyDown(Keys.Up)) Movement.Y -= 1;
            if (ks.IsKeyDown(Keys.Down)) Movement.Y += 1;
            if (ks.IsKeyDown(Keys.Right)) Movement.X += 1;
            if (ks.IsKeyDown(Keys.Left)) Movement.X -= 1;
            if (ks.IsKeyDown(Keys.LeftControl)) Movement *= 4f;

            return Movement;
        }

        public static void SetEditorWorld(Player player, World world)
        {
            
            player.FeetPosition = new Vector2(800, -1);
            player.Velocity = Vector2.Zero;
            
            if (EditedRoomObjects == null) EditedRoomObjects = new List<ValueTuple<int, Vector2>>() { };
            SelectedObject = (666, Vector2.Zero);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            // Basic info display at anypoint in the game

            if (DebugManager.show_hitbox)
            {
                Game1.DrawRectangle(spriteBatch, LocalPlayer.Hurtbox, Color.White * 0.5f);
                Game1.DrawRectangle(spriteBatch, LocalPlayer.Hitbox, Color.Red * 0.5f);
            }
            
            // EDITOR DRAW LOGIC

            if (is_placing_ground)
            {
                Game1.DrawRectangle(spriteBatch, PlacedGround, Color.Pink);// EditedRoom.GetThemeColor());
                LocalGame.DrawString(spriteBatch, PlacedGround.Location.ToString(), PlacedGround.Location.ToVector2());
            }
            
            if (SelectedGround != null)
            {
                Game1.DrawRectangle(spriteBatch, SelectedGround, Color.HotPink * 0.5f);
            }

        }

        

        public static bool JustPressed(Keys k)
        {
            return (ks.IsKeyDown(k) && old_ks.IsKeyUp(k));
        }

        private static void PrintRoom()
        {
            Camera.Shake(new Vector2(600, 500));
            string SixTabs = "\t\t\t\t\t\t";
            string SeptTabs = "\t\t\t\t\t\t\t";
            StreamWriter writer = new StreamWriter("Room " + DateTime.Now.ToString("dd.MM.HH.mm.ss") + ".txt");
           
            writer.Write(SixTabs + "Ground = new List<Rectangle>()");
            writer.Write("\n");
            writer.Write(SixTabs + "{");
            writer.Write("\n");
            foreach (Rectangle r in LocalWorld.Ground)
            {
                writer.Write(SeptTabs + RectangleToString(r) + ",");
                writer.Write("\n");
            }
            writer.Write(SixTabs + "};");
            writer.Close();
        }

       
        public static string RectangleToString(Rectangle rectangle)
        {
            return "new Rectangle(" + Convert.ToString(rectangle.X) + ", " + Convert.ToString(rectangle.Y) + ", " + Convert.ToString(rectangle.Width) + ", " + Convert.ToString(rectangle.Height) + ") ";
        }
#endif
    }

}
