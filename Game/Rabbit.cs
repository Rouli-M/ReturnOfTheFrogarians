using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace Splatoon2D
{
    public class Rabbit:NPC
    {
        enum RabbitState { spawn, race_wait, race, race_end }
        RabbitState currentState;
        int charge_jump_frames;
        int jump_number;
        int sprite_frames = 0;
        int prerace_timer = 350, intro_timer = 0;
        bool intro_said = false, waiting_for_loser = false;
        Bell bell;
        Vector2 Spawn;

        public Rabbit(Vector2 Spawn, Bell bell):base(new Vector2(20, 80), Spawn, new Vector2(-30, -200))
        {
            CurrentSprite = rabbit_idle;
            currentState = RabbitState.spawn;
            this.bell = bell;
            this.Spawn = Spawn;
            Gravity = 0.6f;
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            Sprite PreviousSprite = CurrentSprite;
            if (intro_said) intro_timer++;
            if (currentState == RabbitState.spawn)
            {
                if (DistanceWith(player) < 200 && !intro_said)
                {
                    intro_said = true;
                    Say("Hey there", 120, false, true);
                    Say("You seem like the type of person\nwho'd be down for a race", 220, false, true);
                    Say("Say, first to\n ring the bell wins?", 220, false, true);
                    Say("When you're ready get behind me", ()=> { currentState = RabbitState.race_wait; } , 140, false, true);
                }
            }
            else if(currentState == RabbitState.race_wait)
            {
                if(player.FeetPosition.X < FeetPosition.X + 50)
                {
                    CurrentSprite = rabbit_idle;
                    if (prerace_timer < 350) Say("Don't try to cheat!", 100, true, clear:true);
                    prerace_timer = 350;
                    if (lifetime % 500 == 244) Say("Get behind me when you're ready");
                }
                else
                {
                    CurrentSprite = rabbit_charge;
                    prerace_timer--;
                    if (prerace_timer == 300) Say("3...", 60, true, clear: true);
                    if (prerace_timer == 200) Say("2...", 60, true, clear: true);
                    if (prerace_timer == 100) Say("1...", 60, true, clear: true);
                    if (prerace_timer == 2) Say("Go!", 30, true, clear: true);
                    if (prerace_timer == 1) BeginRace();
                }
            }
            else if(currentState == RabbitState.race)
            {
                if(CurrentSprite == rabbit_charge)
                {
                    charge_jump_frames++;
                    if(charge_jump_frames > 22)
                    {
                        CurrentSprite = rabbit_rise;
                        ApplyForce(GetJumpForce(jump_number));
                        jump_number++;
                        charge_jump_frames = 0;
                    }
                }
                else if (CurrentSprite == rabbit_rise)
                {
                    if (Velocity.Y > -7) CurrentSprite = rabbit_fall;
                }
                else if (CurrentSprite == rabbit_fall)
                {
                    if (IsOnGround(world))
                    {
                        if (jump_number == 12)
                        {
                            CurrentSprite = rabbit_idle;
                            currentState = RabbitState.race_end;
                            if(bell.hit_by_player)
                            {
                                Say("Woah, you're pretty fast!", 180);
                                Say("I guess I have some\ntraining to do", 180);
                                Say("Here, I think this\nmight prove usefull", () => { CurrentSprite = rabbit_button; }, 180);
                            }
                            else
                            {
                                waiting_for_loser = true;
                            }
                            bell.GetRaceUnReady();
                        }
                        else CurrentSprite = rabbit_charge;
                    }
                }
            }
            else if(currentState == RabbitState.race_end)
            {
                if(CurrentSprite == rabbit_idle && waiting_for_loser && DistanceWith(player) < 300)
                {
                    waiting_for_loser = false;
                    Say("Too slow!");
                    Say("Have you been eating correctly?", () =>
                    {
                        CurrentSprite = rabbit_disappear;
                        Say("Try again I guess");
                    }, 180);
                }
                else if (CurrentSprite == rabbit_disappear && CurrentSprite.isOver)
                {
                    currentState = RabbitState.race_wait;
                    CurrentSprite = rabbit_idle;
                    FeetPosition = Spawn;
                }
                else if (CurrentSprite == rabbit_button)
                {
                    if (sprite_frames > 50) CurrentSprite = rabbit_press;
                }
                else if (CurrentSprite == rabbit_press)
                {
                    if (CurrentSprite.frame_event == 3)
                    {
                        Bumper.unlocked = true;
                        SoundEffectPlayer.Play(victory);
                    }
                    if (sprite_frames > 200)
                    {
                        CurrentSprite = rabbit_idle;
                        Say("Happy bouncing!");
                    }
                }
            }

            CurrentSprite.UpdateFrame(gameTime);

            if (PreviousSprite != CurrentSprite)
            {
                sprite_frames = 0;
                CurrentSprite.ResetAnimation();
            }
            else sprite_frames++;

            base.Update(gameTime, world, player);
        }

        void BeginRace()
        {
            currentState = RabbitState.race;
            jump_number = 1;
            CurrentSprite = rabbit_charge;
            bell.GetRaceReady();
        }

        public override void SpeakEvent()
        {
            SoundEffectPlayer.RandomPlay(1f, (float)r.NextDouble() * 0.5f, rabbit_sound1, rabbit_sound2, rabbit_sound3);
        }


        Vector2 GetJumpForce(int jumpID)
        {
            if (jumpID == 1) return new Vector2(-11, -19);
            if (jumpID == 2) return new Vector2(-6f, -20);
            if (jumpID == 3) return new Vector2(-6f, -28);
            if (jumpID == 4) return new Vector2(-10f, -13);
            if (jumpID == 5) return new Vector2(-8f, -16);
            if (jumpID == 6) return new Vector2(-5f, -17);
            if (jumpID == 7) return new Vector2(-11f, -12);
            if (jumpID == 8) return new Vector2(-11f, -12);
            if (jumpID == 9) return new Vector2(-7f, -31);
            if (jumpID == 10) return new Vector2(-15f, -14f);
            if (jumpID == 11) return new Vector2(-10f, -14f);
                          
            return new Vector2(0, -15);        
        }
    }
}
