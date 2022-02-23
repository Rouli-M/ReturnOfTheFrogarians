using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Splatoon2D
{
    public class Hittable : PhysicalObject
    {
        public int life, loot, shake_force;
        public static Sprite balloon1, balloon2, balloon3, balloon4;
        public Vector2 shake_offset;
        public Hittable(Vector2 Size, Vector2 Position):base(Size, Position)
        {

        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            if (lifetime % 5 == 1) shake_offset *= -0.6f;

            foreach(PhysicalObject o in world.Stuff)
            {
                if(o is InkShot shot)
                {
                    if(o.Hurtbox.Intersects(this.Hurtbox))
                    {
                        life--;
                        world.Remove(shot);
                        //display_offset = shake_force * new Vector2(-1, 0);
                        Vector2 shake_direction = Hurtbox.Center.ToVector2() - shot.Hurtbox.Center.ToVector2();
                        shake_direction.Normalize();
                        shake_offset = shake_force * shake_direction;
                    }
                }
            }

            if (life <= 0)
            {
                HUD.SpawnEgg(loot, FeetPosition);
                world.Remove(this);
            }

            base.Update(gameTime, world, player);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            CurrentSprite.DrawFromFeet(spriteBatch, FeetPosition + shake_offset);
        }

        public static void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            balloon1 = new Sprite(Content.Load<Texture2D>("balloon/balloon1"));
            balloon2 = new Sprite(Content.Load<Texture2D>("balloon/balloon2"));
            balloon3 = new Sprite(Content.Load<Texture2D>("balloon/balloon3"));
            balloon4 = new Sprite(Content.Load<Texture2D>("balloon/balloon4"));
        }
    }
}
