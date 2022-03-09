using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Splatoon2D
{
    public class Tofu:NPC
    {
        bool said_dialog = false;
        public Tofu(Vector2 Spawn):base(new Vector2(50, 50), Spawn, new Vector2(0, -70))
        {
            CurrentSprite = tofu_sprite;
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            if(DistanceWith(player) < 100)
            {
                Say("tofu tofu", 120, false, true);
                Say("tofu", 80, false, true);
                Say("tofu..", 80, false, true);
                Say("...", 150, false, true);
                Say("tofu", 100, false, true);
                Say("Get it? I'm tofu", () => { said_dialog = true; }, 180, false, true);
                if(said_dialog && lifetime % 333 == 222)
                    Say("tofu", 50, false);
            }
            CurrentSprite.UpdateFrame(gameTime);
            base.Update(gameTime, world, player);
        }
    }
}
