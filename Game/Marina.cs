using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Splatoon2D
{
    public class Marina:PhysicalObject
    {
        int frame_since_last_ink_detected = 10000;
        public Marina(Vector2 Spawn):base(new Vector2(55, 130), Spawn)
        {
            CurrentSprite = marina_idle;
            Gravity = 1f;
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            foreach(PhysicalObject o in world.Stuff)
            {
                if(o is InkShot s)
                {
                    if(Vector2.Distance(o.FeetPosition, FeetPosition) < 250)
                    {
                        CurrentSprite = marina_inked;
                        frame_since_last_ink_detected = 0;
                    }
                }
            }
            frame_since_last_ink_detected++;

            if (CurrentSprite == marina_inked && frame_since_last_ink_detected > 70)
            {
                CurrentSprite = marina_idle;
            }

            CurrentSprite.UpdateFrame(gameTime);
            base.Update(gameTime, world, player);
        }
    }
}
