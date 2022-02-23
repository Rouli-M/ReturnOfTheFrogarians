using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Splatoon2D
{
    public class InkShot : PhysicalObject
    {
        static Sprite ink_shot_sprite;
        public InkShot(Vector2 SpawnPos, float Angle) : base(new Vector2(5, 5), SpawnPos)
        {
            Velocity = 14 * new Vector2((float)Math.Cos(Angle), - (float)Math.Sin(Angle));
            Gravity = 0f;
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {

            if (lifetime > 14) Gravity = 2f;

            base.Update(gameTime, world, player);

            if (groundcollision || wallcollision)
            {
                world.Remove(this);
                Rectangle PaintZone = Hurtbox;
                PaintZone.Inflate(40, 30);
                world.Paint(PaintZone);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float angle = (float)Math.Atan2(Velocity.Y, Velocity.X);

            ink_shot_sprite.Draw(spriteBatch, FeetPosition - new Vector2(ink_shot_sprite.frameWidth/2, ink_shot_sprite.frameHeight), angle, new Vector2(20, 7));
            //ink_shot_sprite.DrawFromFeet(spriteBatch, FeetPosition, angle);
        }

        public static void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            ink_shot_sprite = new Sprite(Content.Load<Texture2D>("shot"));
        }
    }
}
