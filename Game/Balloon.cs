using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Splatoon2D
{
    public class Balloon : Hittable
    {
        Vector2 offset = Vector2.Zero;
        public Balloon(Vector2 Spawn):base(new Vector2(70, 80), Spawn)
        {
            loot = 2;
            total_life = 8;
            life = total_life;
            Gravity = 0f;
            shake_force = 6;
            CurrentSprite = balloon1;
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            /*
            if (life > max_life * 0.75f) CurrentSprite = balloon1;
            else if (life > max_life * 0.5f) CurrentSprite = balloon2;
            else if (life > max_life * 0.25f) CurrentSprite = balloon3;
            else CurrentSprite = balloon3;
            */
            if (player.Hitbox.Intersects(Hurtbox))
            {
                player.Bump(this);
            }
            offset = 10 * new Vector2((float)Math.Cos(lifetime / 200f), (float)Math.Sin(lifetime / 25f));

            base.Update(gameTime, world, player);
        }

        public override void Die(World world)
        {
            SoundEffectPlayer.Play(ballon_pop_sound);
            base.Die(world);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawInkedSprite(spriteBatch, CurrentSprite, _InkEffect, life / (float)total_life, FeetPosition + new Vector2(0, 80) + offset + shake_offset);
        }

    }

}
