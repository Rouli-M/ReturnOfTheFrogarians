using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Splatoon2D
{
    public class NPC:PhysicalObject
    {
        Vector2 SpeakingPoint, SpeakingOffset;
        List<(string text, int time, int total_time, Action todo)> Discourse;
        public List<string> AlreadySaid;
        public Effect _InkEffect;
        float percentInked;
        public int frame_since_last_ink_detected = 10000;

        public NPC(Vector2 Size, Vector2 Spawn, Vector2 SpeakingOffset):base(Size, Spawn)
        {
            this.SpeakingOffset = SpeakingOffset;
            Discourse = new List<(string text, int time, int total_time, Action todo)>();
            AlreadySaid = new List<string>();
            Gravity = 1f;
            _InkEffect = Hittable.InkEffect.Clone();
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            frame_since_last_ink_detected++;
            foreach (PhysicalObject o in world.Stuff)
            {
                if (o is InkShot s)
                {
                    if (Hurtbox.Contains(s.FeetPosition))
                    {
                        ShotReaction(world, s);
                    }
                }
            }

            if (frame_since_last_ink_detected > 40 && frame_since_last_ink_detected % 15 == 0 && percentInked > 0)
                percentInked -= 0.1f;

            //List<(string text, int time, int total_time)> newDiscourse = new List<(string text, int time, int total_time)>();
            if (Discourse.Count > 0)
            {
                if (Discourse[0].time == Discourse[0].total_time)
                {
                    Action do_next = Discourse[0].todo;
                    Discourse.RemoveAt(0);
                    do_next();
                    // Remove next text if it's too short
                    while (Discourse.Count > 1 && Discourse[0].time > Discourse[0].total_time - 20)
                    {
                        Action do_next2 = Discourse[0].todo;
                        Discourse.RemoveAt(0);
                        do_next2();
                    }
                }
                else
                {
                    if (Discourse[0].time == 0) SpeakEvent();
                    Discourse[0] = (Discourse[0].text, Discourse[0].time + 1, Discourse[0].total_time, Discourse[0].todo);
                }
            }

            SpeakingPoint = FeetPosition + SpeakingOffset;
            base.Update(gameTime, world, player);
        }

        public virtual void ShotReaction(World world, InkShot shot)
        {
            if(percentInked < 1f) percentInked += 0.1f;
            frame_since_last_ink_detected = 0;
            shot.Cancel(world);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(Discourse.Count > 0)
            {
                float offset = (float)Math.Cos(0.5f * Math.PI * Math.Min(1, Discourse[0].time / 10f));
                StringCenterDraw(spriteBatch, Discourse[0].text, SpeakingPoint + new Vector2(0, 20f * offset), Color.Black * (1 - offset), 0.5f);
            }
            //base.Draw(spriteBatch);
            Hittable.DrawInkedSprite(spriteBatch, CurrentSprite, _InkEffect, 1f - percentInked, FeetPosition);
        }

        public static void StringCenterDraw(SpriteBatch spriteBatch, string str, Vector2 Position, Color text_color, float scale)
        {
            spriteBatch.DrawString(Game1.Rouli, str, (Position - Game1.Rouli.MeasureString(str) * 0.5f * scale - Camera.TopLeftCameraPosition) * Camera.Zoom, text_color, 0f, default, scale * Camera.Zoom, SpriteEffects.None, 0f);
        }

        public virtual void Say(string text_to_say, int text_duration = 110, bool priority = false, bool once = false, bool clear = false)
        {
            if (clear) Discourse.Clear();
            if (once)
            {
                if (AlreadySaid.Contains(text_to_say)) return;
                AlreadySaid.Add(text_to_say);
            }
            if (priority) Discourse.Insert(0, (text_to_say, 0, text_duration, () => { } ));
            else Discourse.Add((text_to_say, 0, text_duration, () => { }));
        }

        public virtual void Say(string text_to_say, Action todo, int text_duration = 110, bool priority = false, bool once = false, bool clear = false)
        {
            if (clear) Discourse.Clear();
            if (once)
            {
                if (AlreadySaid.Contains(text_to_say)) return;
                AlreadySaid.Add(text_to_say);
            }
            if (priority) Discourse.Insert(0, (text_to_say, 0, text_duration, todo));
            else Discourse.Add((text_to_say, 0, text_duration, todo));
        }

        public virtual void SpeakEvent()
        {

        }
    }
}
