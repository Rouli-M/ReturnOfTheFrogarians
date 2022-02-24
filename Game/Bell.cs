using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Splatoon2D
{
    public class Bell : PhysicalObject
    {
        int egg_to_release;
        public Bell(Vector2 Spawn):base(new Vector2(50, 60), Spawn)
        {
            CurrentSprite = bell_idle;
            Gravity = 0f;
            egg_to_release = 7;
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {

            foreach (PhysicalObject o in world.Stuff)
            {
                if (o is InkShot s)
                {
                    if (o.Hurtbox.Intersects(Hurtbox))
                    {
                        world.Remove(o);
                        bell_hit.ResetAnimation();
                        CurrentSprite = bell_hit;
                        if(egg_to_release > 0)
                        {
                            egg_to_release--;
                            HUD.SpawnEgg(1, Hurtbox.Center.ToVector2());
                        }
                    }
                }
               
            }

            if (CurrentSprite == bell_hit && CurrentSprite.isOver) CurrentSprite = bell_idle;
            CurrentSprite.UpdateFrame(gameTime);

            base.Update(gameTime, world, player);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            CurrentSprite.DrawFromFeet(spriteBatch, FeetPosition + new Vector2(0, 83));
        }
    }
}
