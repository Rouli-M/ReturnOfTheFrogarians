using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Splatoon2D
{
    public class Bumper:PhysicalObject
    {
        float angle;
        bool turning;
        Sprite _bump;
        public static bool unlocked;
        List<(int ID, int frames)> RecentlyBumped;
        public Bumper(Vector2 Spawn, float angle = 0f, bool turning = false):base(new Vector2(65, 65), Spawn)
        {
            CurrentSprite = bump_idle;
            this.turning = turning;    
            this.angle = angle - (float)Math.PI/2;
            _bump = new Sprite(bump_bump);
            RecentlyBumped = new List<(int, int)>() ;
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            if (!unlocked)
            {
                CurrentSprite = bump_locked;
                return;
            }
            else if (CurrentSprite == bump_locked) CurrentSprite = bump_idle;

            List<(int ID, int frames)> newRecentlyBumped = new List<(int, int)>();
            foreach((int ID, int frames) in RecentlyBumped)
            {
                if(frames < 30)
                    newRecentlyBumped.Add((ID, frames + 1));
            }
            RecentlyBumped = newRecentlyBumped;

            if (CurrentSprite == _bump && CurrentSprite.isOver)
            {
                CurrentSprite.ResetAnimation();
                CurrentSprite = bump_idle;
            }
            if(player.Hitbox.Intersects(Hurtbox))
            {
                Bump(player, 22);
            }
            foreach(PhysicalObject o in world.Stuff)
            {
                if(o is InkShot s && o.Hurtbox.Intersects(Hurtbox))
                {
                    Bump(o, 13);
                    s.lifetime = -16;
                }
            }
            
            if(turning && CurrentSprite != _bump) angle += (float)Math.PI / 80f;
            CurrentSprite.UpdateFrame(gameTime);

            base.Update(gameTime, world, player);
        }

        void Bump(PhysicalObject o, int force)
        {
            foreach ((int ID, int frames) in RecentlyBumped)
                if (ID == o.ID) return;

            RecentlyBumped.Add((o.ID, 0));
            if (o is Player p)
            {
                o.FeetPosition = Hurtbox.Center.ToVector2() + new Vector2(0, p.HitboxSize.Y / 2);
                p.dash_frames = 20;
                p.CurrentState = Player.PlayerState.jump;
            }
            else o.FeetPosition = Hurtbox.Center.ToVector2() + new Vector2(0, o.HurtboxSize.Y / 2);
            if(_bump.firstFrame) SoundEffectPlayer.Play(bump);
            o.Velocity = force * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            CurrentSprite = _bump;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pivot = new Vector2(62, 38);
            CurrentSprite.Draw(spriteBatch, FeetPosition - new Vector2(CurrentSprite.frameWidth / 2, CurrentSprite.frameHeight), angle, pivot);

        }
    }
}
