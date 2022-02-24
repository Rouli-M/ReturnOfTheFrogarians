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
        static Sprite player_shot_sprite, enemy_shot_sprite, player_shot_denied_sprite;
        Sprite ink_shot_sprite;
        public bool is_enemy;
        public InkShot(Vector2 SpawnPos, float Angle, bool is_enemy = false) : base(new Vector2(5, 5), SpawnPos)
        {
            Velocity = (is_enemy ? 4.5f : 17) * new Vector2((float)Math.Cos(Angle), - (float)Math.Sin(Angle));
            Gravity = 0f;
            this.is_enemy = is_enemy;
            if (is_enemy) ink_shot_sprite = enemy_shot_sprite;
            else ink_shot_sprite = player_shot_sprite;
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            if (lifetime > 14 && !is_enemy) Gravity = 2f;
            else Gravity = 0.05f;
            XTreshold = 0.001f;

            base.Update(gameTime, world, player);

            if (groundcollision || wallcollision)
            {
                Console.WriteLine("Remove InkShot at " + FeetPosition);
                world.Remove(this);
                Rectangle PaintZone = Hurtbox;
                
                if(is_enemy) PaintZone.Inflate(80, 80);
                else PaintZone.Inflate(40, 30);
                world.Paint(PaintZone, is_enemy);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float angle = (float)Math.Atan2(Velocity.Y, Velocity.X);

            ink_shot_sprite.Draw(spriteBatch, FeetPosition - new Vector2(ink_shot_sprite.frameWidth/2, ink_shot_sprite.frameHeight), angle, new Vector2(20, 7));
            //ink_shot_sprite.DrawFromFeet(spriteBatch, FeetPosition, angle);
        }

        public void Cancel(World world)
        {
            world.Spawn(new Particle(FeetPosition, player_shot_denied_sprite));
            world.Remove(this);
        }

        public static void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            player_shot_sprite = new Sprite(Content.Load<Texture2D>("shot"));
            enemy_shot_sprite = new Sprite(Content.Load<Texture2D>("enemy_bullet"));
            player_shot_denied_sprite = new Sprite(3, 17, 34, 80, Content.Load<Texture2D>("shot_reject"), loopAnimation:false);
        }
    }
}
