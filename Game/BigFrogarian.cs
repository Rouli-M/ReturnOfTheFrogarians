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
        int shoot_cooldown = 0, exclamation_cooldown = 0, question_cooldown = 0, queue_big_shot_frames = 0;
        bool player_seen = false;
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
            ViewField = new Rectangle((int)FeetPosition.X - 400, (int)FeetPosition.Y - 300, 800, 550);
            if (shoot_cooldown > 0) shoot_cooldown--;
            if (exclamation_cooldown > 0) exclamation_cooldown--;
            if (question_cooldown > 0) question_cooldown--;
            if (queue_big_shot_frames > 0) queue_big_shot_frames--;
            if(CurrentSprite == _idle && (ViewPlayer(player) || queue_big_shot_frames > 0) && shoot_cooldown == 0)
            {
                CurrentSprite = _press;
            }
            else if(CurrentSprite == _press && CurrentSprite.frame_event == 1)
            {
                Shoot(world);
                queue_big_shot_frames = 0;
            }
            else if (CurrentSprite == _press && CurrentSprite.isOver)
                CurrentSprite = _idle;

            if(player_seen && !ViewPlayer(player))
            {
                if (question_cooldown == 0)
                {
                    world.Spawn(new Particle(FeetPosition + new Vector2(80, -Hurtbox.Height + 10), interrogation, 61));
                    SoundEffectPlayer.Play(question_sound);
                    question_cooldown = 200;
                }                    
                player_seen = false;
            }

            if (CurrentSprite != PreviousSprite) CurrentSprite.ResetAnimation();
            CurrentSprite.UpdateFrame(gameTime);
            base.Update(gameTime, world, player);
            if (ViewPlayer(player)) player_seen = true;
        }

        void Shoot(World world)
        {
            SoundEffectPlayer.Play(enemy_shoot);
            world.Spawn(new InkShot(FeetPosition + new Vector2(+115, -30), (float)(Math.PI / 2 - Math.PI / 2 * 0.8f), true, queue_big_shot_frames > 0));
            world.Spawn(new InkShot(FeetPosition + new Vector2(-115, -30), (float)(Math.PI / 2 + Math.PI / 2 * 0.8f), true, queue_big_shot_frames > 0));
            world.Spawn(new InkShot(FeetPosition + new Vector2(+75, -91), (float)(Math.PI / 2 - Math.PI / 2 * 0.2f), true , queue_big_shot_frames > 0));
            world.Spawn(new InkShot(FeetPosition + new Vector2(-75, -91), (float)(Math.PI / 2 + Math.PI / 2 * 0.2f), true , queue_big_shot_frames > 0));
            shoot_cooldown = 130;
        }

        public override void ShotReaction(World world, Player player, InkShot shot)
        {
            if (!ViewPlayer(player) && exclamation_cooldown == 0)
            {
                world.Spawn(new Particle(FeetPosition + new Vector2(80, -Hurtbox.Height + 10), exclamation, 61));
                exclamation_cooldown = 200;
            }
            queue_big_shot_frames = 60;
            base.ShotReaction(world, player, shot);
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
