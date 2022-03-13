using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Splatoon2D
{
    public class Pearl:NPC
    {
        int crypto_count = 0;
        bool said_dialog = false;
        bool has_given_box = false;
        public Pearl(Vector2 Spawn) : base(new Vector2(100, 100), Spawn, new Vector2(180, -220))
        {
            CurrentSprite = pearl_sprite;
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            if (lifetime % 140 == 12)
            {
                Sprite crypto = crypto1;
                if (crypto_count % 3 == 1) crypto = crypto2;
                if (crypto_count % 3 == 2) crypto = crypto3;
                crypto_count++;

                world.Spawn(new Particle(FeetPosition + new Vector2(70, -60), crypto, new Vector2(0, -1.3f), Vector2.Zero, 60));
            }

            if(DistanceWith(player) < 300)
            {
                Say("i discovered dis awsome internet money!!!", 200, once:true);
                Say("you just hav to buy some and then watch\nthe funny graph go up and down!!", 240, once:true);
                Say("wonder what ill buy with all dis mony;..", () => { said_dialog = true; },200, once:true);
                if(HUD.egg_count >=190 && !has_given_box)
                {
                    Say("btw I found this, take it!!\ni'm so rich anywai", () => { world.Spawn(new Egg(FeetPosition + new Vector2(0, -150), 5)); Say("", 100, priority: true); }, 200, once: true, priority:true);
                    has_given_box = true;
                }
            }

            if (DistanceWith(player) < 700 && said_dialog)
            {
                if(lifetime% 333 == 111)
                {
                    int lineID = r.Next(0, 5);
                    if (lineID == 0) Say("$QUIDCOIN is where its at!!", 120);
                    if (lineID == 1) Say("BUY BUY BUY!!!", 120);
                    if (lineID == 2) Say("NOOOOOOOOOOOOOO", 120);
                    if (lineID == 3) Say("it's going to go up I'm sure of it!!!", 120);
                    if (lineID == 4) Say("deregulate the finance world!!!", 120);
                    if (lineID == 5) Say("my whalet id is s0ci22c3wt5", 120);
                }
            }


            if (CurrentSprite == pearl_inked && frame_since_last_ink_detected > 70)
            {
                CurrentSprite = pearl_sprite;
            }

            CurrentSprite.UpdateFrame(gameTime);
            base.Update(gameTime, world, player);

        }

        public override void ShotReaction(World world, InkShot shot)
        {
            if (CurrentSprite != pearl_inked) Say(GetRandomExclamation(), 70, true);
            CurrentSprite = pearl_inked;
            base.ShotReaction(world, shot);
        }

        public string GetRandomExclamation()
        {
            int id = r.Next(0, 11);
            if (id == 0) return "woah!!";
            if (id == 2) return "stop dat!!";
            if (id == 3) return "??? what";
            if (id == 4) return "why ARE YOU SHOOTING";
            if (id == 5) return "let me trade!!!!";
            if (id == 6) return "this is just like highschool";
            if (id == 7) return "is that an unregistered weapon???";
            if (id == 8) return "you want to start a war or somethin?";
            if (id == 9) return "of course you have purple hair and pronouns";
            if (id == 10) return "are you sanitized??";
            return "hey!!";
        }
    }
}
