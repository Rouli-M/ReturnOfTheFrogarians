using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Splatoon2D
{
    public class Egg:PhysicalObject
    {
        public int count;
        Vector2 offset = Vector2.Zero;
        private static Sprite egg, egg_box;
        public Egg(Vector2 Spawn, int count = 1):base(new Vector2(25, 25), Spawn)
        {
            this.count = count;
            if(count == 1) CurrentSprite = egg;
            else CurrentSprite = egg_box;
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            if(player.Hitbox.Intersects(Hurtbox))
            {
                HUD.SpawnEgg(count, Hurtbox.Center.ToVector2());
                world.Remove(this);
            }
            offset = 6 * new Vector2(0, (float)Math.Sin(lifetime / 10f));
 
            base.Update(gameTime, world, player);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            CurrentSprite.DrawFromFeet(spriteBatch, FeetPosition + offset);
        }

        public static new void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            egg = new Sprite(Content.Load<Texture2D>("egg"));
            egg_box = new Sprite(Content.Load<Texture2D>("egg_box"));
        }
    }
}
