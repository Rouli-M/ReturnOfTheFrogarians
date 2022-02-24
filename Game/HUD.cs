using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Splatoon2D
{
    public static class HUD
    {
        public static Sprite MouseSprite, EggSprite, SmallEggSprite, EggBoxSprite;
        public static Sprite dmg1, dmg2, dmg3, dmg4, dmg5;
        private static SpriteFont RouliFont;
        private static Random R = new Random();
        private static Player _player;
        private static List<(int, Vector2, Vector2)> eggs = new List<(int, Vector2, Vector2)>();
        private static Vector2 EggAttractionPoint = new Vector2(100, 600);
        public static int egg_count = 0;
        public static void Draw(SpriteBatch spriteBatch)
        {
            Sprite overlay_dmg_sprite = null;
            if (_player.damage > 5 * 100 / 6f) overlay_dmg_sprite = dmg5;
            else if (_player.damage > 4 * 100 / 6f) overlay_dmg_sprite = dmg4;
            else if (_player.damage > 3 * 100 / 6f) overlay_dmg_sprite = dmg3;
            else if (_player.damage > 2 * 100 / 6f) overlay_dmg_sprite = dmg2;
            else if (_player.damage > 1 * 100 / 6f) overlay_dmg_sprite = dmg1;
            if(overlay_dmg_sprite != null) overlay_dmg_sprite.ScreenDraw(spriteBatch, Vector2.Zero);

            if (!Input.GamepadUsed) MouseSprite.ScreenDraw(spriteBatch, Input.ms.Position.ToVector2());

            EggSprite.ScreenDraw(spriteBatch, new Microsoft.Xna.Framework.Vector2(30, 600));
            spriteBatch.DrawString(RouliFont, "x" + egg_count, new Vector2(133, 622), Color.Black);

            foreach ((int i, Vector2 p, Vector2 v) e in eggs)
            {
                SmallEggSprite.ScreenDraw(spriteBatch, e.p);
            }
        }

        public static void Update(Player player)
        {
            _player = player;
            List<(int, Vector2, Vector2)> updatedEggs = new List<(int, Vector2, Vector2)>();
            foreach((int i, Vector2 p, Vector2 v) e in eggs)
            {
                Vector2 new_v = e.v;
                if(e.i < 25)
                {
                    new_v *= 0.9f;
                }
                else
                {
                    Vector2 EggDirection = EggAttractionPoint - e.p;
                    EggDirection.Normalize();
                    new_v += EggDirection * 4.5f;
                }
                if (e.p.X < 70 || e.p.Y > 700) egg_count++ ;
                else updatedEggs.Add((e.i + 1, e.p + new_v, new_v));
            }
            eggs = updatedEggs;
        }

        public static void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            HUD.MouseSprite = new Sprite(Content.Load<Texture2D>("cursor1"));
            HUD.EggSprite = new Sprite(Content.Load<Texture2D>("frog_egg"), scale:2f);
            SmallEggSprite = new Sprite(Content.Load<Texture2D>("egg"), scale: 2f);
            EggBoxSprite = new Sprite(Content.Load<Texture2D>("egg_box"), scale: 2f);
            RouliFont = Content.Load<SpriteFont>("RouliXL");

            dmg1 = new Sprite(Content.Load<Texture2D>("HUD/screen_damage1"), scale: 6f);
            dmg2 = new Sprite(Content.Load<Texture2D>("HUD/screen_damage2"), scale: 6f);
            dmg3 = new Sprite(Content.Load<Texture2D>("HUD/screen_damage3"), scale: 6f);
            dmg4 = new Sprite(Content.Load<Texture2D>("HUD/screen_damage4"), scale: 6f);
            dmg5 = new Sprite(Content.Load<Texture2D>("HUD/screen_damage5"), scale: 6f);
        }

        public static void SpawnEgg(int count, Vector2 SpawnPosition)
        {
            Vector2 ScreenPos = (SpawnPosition - Camera.TopLeftCameraPosition) * Camera.Zoom;
            for (int i = 0; i < count; i++)
            {
                eggs.Add((0, ScreenPos, new Vector2(R.Next(-12, 12), R.Next(-12, 12))));
            }
        }

        public static string GetPointerPosition(Vector2 ScreenPos)
        {
            Vector2 WorldPos = GetPointerWorldVector(ScreenPos);
            return "new Vector2(" + (int)WorldPos.X + ", " + (int)WorldPos.Y + ")"; 
        }

        public static Vector2 GetPointerWorldVector(Vector2 ScreenPos)
        {
            return ScreenPos / (float)Camera.Zoom + Camera.TopLeftCameraPosition;
        }
    }
}
