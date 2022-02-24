using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Splatoon2D
{
    public class Frogtarian:Hittable
    {
        Rectangle Hitbox, Shieldbox, ViewBox;
        int state_frames;
        int direction;
        int shoot_cooldown;

        Sprite _idle, _move, _shoot;

        enum FrogtarianState { moving, idle, shooting, dead }
        FrogtarianState state;

        public Frogtarian(Vector2 Spawn) : base(new Vector2(75, 180), Spawn)
        {
            _idle = new Sprite(frogtarian_idle);
            _move = new Sprite(frogtarian_move);
            _shoot = new Sprite(frogtarian_shoot);
            direction = -1;
            life = 10;
            loot = 3;
            state = FrogtarianState.idle;
            state_frames = 0;
            GroundFactor = 0.9f;
            XTreshold = 0.001f;
            Gravity = 1f;
            shake_force = 5;
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            FrogtarianState oldState = state;

            switch(state)
            {
                case FrogtarianState.idle:
                    if (player.Hitbox.Intersects(ViewBox) && !player.is_on_ink) Shoot(world);
                    else if (state_frames > 100)
                        state = FrogtarianState.moving;
                    break;
                case FrogtarianState.moving:
                    if (player.Hitbox.Intersects(ViewBox) && !player.is_on_ink) Shoot(world);
                    else
                    {
                        if (world.CheckCollision(FeetPosition + new Vector2(direction * (Hurtbox.Width / 2 + 10), 0), Vector2.One)
                        || !world.CheckCollision(FeetPosition + new Vector2(direction * (Hurtbox.Width / 2 + 10), 10), Vector2.One))
                            direction *= -1;

                        ApplyForce(new Vector2(direction * 0.2f, 0));

                        if (state_frames > 300) state = FrogtarianState.idle;
                    }
                    break;
                case FrogtarianState.shooting:
                    if (CurrentSprite.isOver)
                        state = FrogtarianState.idle;
                    break;
            }

            foreach(PhysicalObject o in world.Stuff)
            {
                if(o is InkShot s)
                {
                    if (o.Hurtbox.Intersects(Shieldbox) && !s.is_enemy) world.Remove(o);
                }
            }

            switch(state)
            {
                case FrogtarianState.idle: CurrentSprite = _idle; break;
                case FrogtarianState.moving: CurrentSprite = _move; break;
                case FrogtarianState.shooting: CurrentSprite = _shoot; break;
            }
            if (shoot_cooldown > 0) shoot_cooldown--;
            state_frames++;
            if (state!=oldState)
            {
                CurrentSprite.ResetAnimation();
                state_frames = 0;
            }

            CurrentSprite.direction = direction * -1;
            CurrentSprite.UpdateFrame(gameTime);

            base.Update(gameTime, world, player);
            Hitbox = Hurtbox;
            Hitbox.Height /= 2; // only the top of the hurtbox is touchable (where the frog is)
            Shieldbox = new Rectangle(Hurtbox.X - 50, Hurtbox.Y, 30, 120);
            if (direction == 1) Shieldbox.Offset(new Point(Hurtbox.Width + 50 * 2 - Shieldbox.Width,0));
            ViewBox = Hurtbox;
            ViewBox.Width = 450;
            if (direction == -1) ViewBox.Offset(new Point(-450 + Hurtbox.Width, 0));
            Game1.DrawRectangle(Hurtbox);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Game1.DrawRectangle(spriteBatch, ViewBox, Color.Green, HUD.EggBoxSprite.Texture);
            base.Draw(spriteBatch);
        }

        private void Shoot(World world)
        {
            if (shoot_cooldown > 0) return;
            shoot_cooldown = 110;
            state = FrogtarianState.shooting;
            world.Spawn(new InkShot(FeetPosition + new Vector2(direction * 75, -61), (float)(Math.PI/2 - direction * Math.PI/2 * 0.8f) , true));
        }

        public override bool ShotTouched(InkShot o)
        {
            return (o.Hurtbox.Intersects(Hitbox));
        }
    }
}
