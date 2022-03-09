using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Splatoon2D
{
    public class Marina:NPC
    {
        int frame_since_last_ink_detected = 10000;
        bool spoke_intro = false, has_given_box = false;
        public Marina(Vector2 Spawn):base(new Vector2(140, 130), Spawn, new Vector2(30, -190))
        {
            CurrentSprite = marina_idle;
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            foreach(PhysicalObject o in world.Stuff)
            {
                if(o is InkShot s)
                {
                    if (Hurtbox.Contains(s.FeetPosition))
                    {
                        s.Cancel(world);
                        if (CurrentSprite != marina_inked) Say(GetRandomExclamation(), 70, true);
                        CurrentSprite = marina_inked;
                        frame_since_last_ink_detected = 0;
                    }
                    else if (Vector2.Distance(o.FeetPosition, FeetPosition) < 250)
                    {

                    }

                }
            }
            frame_since_last_ink_detected++;

            if ((player.FeetPosition - FeetPosition).Length() < 400)
            {
                if (HUD.egg_count >= 190 && !has_given_box)
                {
                    Say("Since you like them so much,\n take this, weirdo.", () => { world.Spawn(new Egg(FeetPosition + new Vector2(0, -150), 5)); Say("", 100, priority: true); }, 200, once: true, priority: true);
                    has_given_box = true;
                }
                if (!spoke_intro)
                {
                    spoke_intro = true;
                    Say("Oh hey you!", 150);
                    Say("You're not a laywer\n  or something?", 200);
                    Say("Good, I'm gonna\nneed your help", 200);
                    Say("These Frogarians\nare everywhere...", 200);
                    Say("I don't know how\nthey got there", 200);
                    Say("But obviously they should'nt\ninvade us like that", 200);
                    Say("Could you maybe\ntake care of them?", 200);
                }
                if (HUD.egg_count > 150) Say("This egg obsession is\nstarting to be weird.", 200, false, true);
                else if (HUD.egg_count > 100)
                {
                    Say("What are you going to do\nwith all these eggs?", 200, false, true);
                    Say("An omelet?", 120, false, true);
                }
                else if (HUD.egg_count > 75) Say("Woah, that's a lot of eggs!", 200, false, true);
                else if (HUD.egg_count > 50) Say("You've been doing great,\n    keep it up!", 200, false, true);
                else if (HUD.egg_count > 10) Say("You get to keep the eggs,\n   lucky you!", 200, false, true);
            }


            if (CurrentSprite == marina_inked && frame_since_last_ink_detected > 70)
            {
                CurrentSprite = marina_idle;
            }

            CurrentSprite.UpdateFrame(gameTime);
            base.Update(gameTime, world, player);
        }


        public override void SpeakEvent()
        {
            SoundEffectPlayer.RandomPlay(1f, (float)r.NextDouble() * 0.4f, marina_sound1, marina_sound2);
        }

        public string GetRandomExclamation()
        {
            int id = r.Next(0, 11);
            if (id == 0) return "Hey!!";
            if (id == 2) return "Can you not??";
            if (id == 3) return "Help!";
            if (id == 4) return "I'm calling\nmy agent..";
            if (id == 5) return "Stop!";
            if (id == 6) return "Ouch!";
            if (id == 7) return "Be carefull!!";
            if (id == 8) return "I'm not a frog!!";
            if (id == 9) return "I'm under attack!";
            return "Not cool!";
        }
    }
}
