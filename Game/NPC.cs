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
        List<string> AlreadySaid;

        public NPC(Vector2 Size, Vector2 Spawn, Vector2 SpeakingOffset):base(Size, Spawn)
        {
            this.SpeakingOffset = SpeakingOffset;
            Discourse = new List<(string text, int time, int total_time, Action todo)>();
            AlreadySaid = new List<string>();
            Gravity = 1f;
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            //List<(string text, int time, int total_time)> newDiscourse = new List<(string text, int time, int total_time)>();
            if(Discourse.Count > 0)
            {
                if (Discourse[0].time == Discourse[0].total_time)
                {
                    Discourse[0].todo();
                    Discourse.RemoveAt(0);
                    // Remove next text if it's too short
                    while(Discourse.Count > 1 && Discourse[0].time > Discourse[0].total_time - 20)
                        Discourse.RemoveAt(0);
                }
                else Discourse[0] = (Discourse[0].text, Discourse[0].time + 1, Discourse[0].total_time, Discourse[0].todo);
            }

            SpeakingPoint = FeetPosition + SpeakingOffset;
            base.Update(gameTime, world, player);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(Discourse.Count > 0)
            {
                StringCenterDraw(spriteBatch, Discourse[0].text, SpeakingPoint, Color.Black, 0.5f);
            }
            base.Draw(spriteBatch);
        }

        public static void StringCenterDraw(SpriteBatch spriteBatch, string str, Vector2 Position, Color text_color, float scale)
        {
            spriteBatch.DrawString(Game1.Rouli, str, (Position - Game1.Rouli.MeasureString(str) * 0.5f * scale - Camera.TopLeftCameraPosition) * Camera.Zoom, text_color, 0f, default, scale * Camera.Zoom, SpriteEffects.None, 0f);
        }

        public void Say(string text_to_say, int text_duration = 110, bool priority = false, bool once = false, bool clear = false)
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

        public void Say(string text_to_say, Action todo, int text_duration = 110, bool priority = false, bool once = false, bool clear = false)
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
    }
}
