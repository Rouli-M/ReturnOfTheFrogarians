using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Splatoon2D
{
    public class BigFrogarian:Hittable
    {
        Sprite _idle, _press;
        Rectangle ViewField;
        public BigFrogarian(Vector2 Spawn):base(new Vector2(120,120), Spawn)
        {
            _idle = new Sprite(big_frogarian_idle);
            _press = new Sprite(big_frogarian_press);
            CurrentSprite = _idle;

            total_life = 16;
            life = total_life;
            loot = 5;
            shake_force = 5;
        }
        public override void Update(GameTime gameTime, World world, Player player)
        {
            PreviousSprite = CurrentSprite;
            ViewField = new Rectangle((int)FeetPosition.X - 400, (int)FeetPosition.Y - 300, 800, 600);
            if(CurrentSprite == _idle && ViewPlayer(player))
            {
                CurrentSprite = _press;
            }
            else if(CurrentSprite == _press && CurrentSprite.frame_event == 1)
            {
                // shoot
                world.Spawn(new InkShot(FeetPosition + new Vector2( +115, -30), (float)(Math.PI / 2 - Math.PI / 2 * 0.8f), true));
                world.Spawn(new InkShot(FeetPosition + new Vector2( -115, -30), (float)(Math.PI / 2 + Math.PI / 2 * 0.8f), true));
                world.Spawn(new InkShot(FeetPosition + new Vector2( +75, -91), (float)(Math.PI / 2 - Math.PI / 2 * 0.2f), true));
                world.Spawn(new InkShot(FeetPosition + new Vector2( -75, -91), (float)(Math.PI / 2 + Math.PI / 2 * 0.2f), true));

            }
            else if (CurrentSprite == _press && CurrentSprite.isOver)
                CurrentSprite = _idle;

            if (CurrentSprite != PreviousSprite) CurrentSprite.ResetAnimation();
            CurrentSprite.UpdateFrame(gameTime);
            base.Update(gameTime, world, player);
        }

        public override void Die(World world)
        {
            // copy pasted from  class. Should be in a base class
            SoundEffectPlayer.Play(explosion);
            base.Die(world);
        }

        bool ViewPlayer(Player player)
        {
            // copy pasted from  class. Should be in a base class
            return (player.Hitbox.Intersects(ViewField) && !(player.is_on_ink && player.CurrentForm == Player.PlayerForm.squid));
        }
    }
}
