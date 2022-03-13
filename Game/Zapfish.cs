using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Splatoon2D
{
    public class Zapfish : NPC
    {
        public static bool played_win_animation = false;
        public Zapfish(Vector2 Spawn):base(new Vector2(180, 120), Spawn, Vector2.Zero)
        {
            CurrentSprite = zapfish_idle;
            GroundFactor = 1f;
            WallBounceFactor = 1f;
            GroundBounceFactor = 1f;
            Gravity = 0f;
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            if(player.Hitbox.Intersects(Hurtbox) && !played_win_animation)
            {
                HUD.TriggerWin(player);
                CurrentSprite = zapfish_win;
                FeetPosition = player.FeetPosition - new Vector2(180, 0);
                played_win_animation = true;
            }
            if(player.CurrentState != Player.PlayerState.celebrating && played_win_animation && Velocity == Vector2.Zero)
            {
                Velocity = new Vector2(-1.6453f, 1.22043f);
                CurrentSprite = zapfish_idle;
            }
            CurrentSprite.UpdateFrame(gameTime);
            base.Update(gameTime, world, player);
        }
    }
}
