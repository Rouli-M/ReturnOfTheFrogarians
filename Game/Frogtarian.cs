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
        int turn_timer; // number of frames taken to turn around

        Sprite _idle, _move, _shoot;

        enum FrogtarianState { moving, idle, shooting, dead }
        FrogtarianState state, previousState;

        public Frogtarian(Vector2 Spawn) : base(new Vector2(75, 180), Spawn)
        {
            _idle = new Sprite(frogtarian_idle);
            _move = new Sprite(frogtarian_move);
            _shoot = new Sprite(frogtarian_shoot);
            direction = -1;
            total_life = 13;
            life = total_life;
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

            if(turn_timer > 0)
            {
                if (turn_timer == 1)
                    direction *= -1;
            }
            else switch(state)
            {
                case FrogtarianState.idle:
                    if (ViewPlayer(player)) Shoot(world);
                    else if (state_frames == 61 && previousState == FrogtarianState.shooting)
                    {
                        if (!ViewPlayer(player)) world.Spawn(new Particle(FeetPosition + new Vector2(56, - Hurtbox.Height + 30), interrogation, 61));
                    }
                    else if (state_frames > 100)
                        state = FrogtarianState.moving;
                    break;
                case FrogtarianState.moving:
                    if (ViewPlayer(player)) Shoot(world);
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
                    if (o.Hurtbox.Intersects(Shieldbox) && !s.is_enemy) s.Cancel(world);
                }
            }

            switch(state)
            {
                case FrogtarianState.idle: CurrentSprite = _idle; break;
                case FrogtarianState.moving: CurrentSprite = _move; break;
                case FrogtarianState.shooting: CurrentSprite = _shoot; break;
            }
            if (shoot_cooldown > 0) shoot_cooldown--;
            if (turn_timer > 0) turn_timer--;
            state_frames++;
            if (state!=oldState)
            {
                CurrentSprite.ResetAnimation();
                state_frames = 0;
                previousState = oldState;
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
            ViewBox.X += 80;
            ViewBox.Height += 30;
            if (direction == -1) ViewBox.Offset(new Point(-450 - 80 * 2 + Hurtbox.Width, 0));
            if(player.Hitbox.Intersects(Hitbox))
            {
                player.Damage(30);
                player.Bump(this);
            }
        }

        public override void ShotReaction(World world, Player player, InkShot shot)
        {
            if (Direction(player) != direction && turn_timer == 0)
            {
                world.Spawn(new Particle(FeetPosition + new Vector2(56, -Hurtbox.Height + 30), exclamation, 61));
                turn_timer = 45;
                state = FrogtarianState.idle;
                Velocity.X = 0;
            }
            base.ShotReaction(world, player, shot);
        }

        bool ViewPlayer(Player player)
        {
            return (player.Hitbox.Intersects(ViewBox) && !(player.is_on_ink && player.CurrentForm == Player.PlayerForm.squid));
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
            _shoot.ResetAnimation();
        }

        public override bool ShotTouched(InkShot o)
        {
            return (o.Hurtbox.Intersects(Hitbox));
        }
    }
}
