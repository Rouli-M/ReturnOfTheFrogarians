using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;

namespace Splatoon2D
{
    public static class HUD
    {
        private static Sprite MouseSprite;
        public static void Draw(SpriteBatch spriteBatch)
        {
            MouseSprite.ScreenDraw(spriteBatch, Input.ms.Position.ToVector2());
        }

        public static void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            HUD.MouseSprite = new Sprite(Content.Load<Texture2D>("cursor1"));
        }
    }
}
