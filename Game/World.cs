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
        public static List<(Vector2, Sprite)> Decor;
        public List<PhysicalObject> Stuff;
        private List<PhysicalObject> StuffToRemove;
        private static Texture2D gray, ground, painted_ground;
        private static Sprite statue1;

        public World(Player player)
        {
            Stuff = new List<PhysicalObject>();
            StuffToRemove = new List<PhysicalObject>();
            PaintedGround = new List<Rectangle>();
            Ground = new List<Rectangle>()
            {
                new Rectangle(-200, 0, 2000, 200)
            };
        }

        public void Update(GameTime gameTime, Player player)
        {
            foreach (PhysicalObject o in Stuff) o.Update(gameTime, this, player);
            foreach (PhysicalObject o in StuffToRemove) Stuff.Remove(o);
            StuffToRemove.Clear();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach ((Vector2 position, Sprite sprite) decor in Decor)
            {
                decor.sprite.DrawFromFeet(spriteBatch, decor.position);
            }

            foreach (Rectangle r in Ground)
            {
                Game1.DrawRectangle(spriteBatch, r, Color.White, gray);
                Game1.DrawRectangle(spriteBatch, new Rectangle(r.X, r.Y - ground.Height / 2, r.Width, ground.Height), Color.White, ground, true);
            }

            foreach (Rectangle r in PaintedGround)
            {
                Game1.DrawRectangle(spriteBatch, new Rectangle(r.X, r.Y - ground.Height / 2, r.Width, ground.Height), Color.White, painted_ground, true);
            }

            foreach (PhysicalObject o in Stuff) o.Draw(spriteBatch);
        }

        public void Paint(Rectangle PaintZone)
        {
            PaintZone.X -= PaintZone.X % 5;
            PaintZone.Width += 5 - PaintZone.Width % 5;
            foreach(Rectangle r in Ground)
            {
                if(r.Intersects(PaintZone))
                {
                    Rectangle PaintedZoneAdded = Rectangle.Intersect(PaintZone, r);
                    PaintedZoneAdded.Height = painted_ground.Height;
                    PaintedGround.Add(PaintedZoneAdded);
                }
            }
        }

        public static void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            gray = Content.Load<Texture2D>("tileset/gray");
            ground = Content.Load<Texture2D>("tileset/ground");
            painted_ground = Content.Load<Texture2D>("tileset/ground_paint");

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
 