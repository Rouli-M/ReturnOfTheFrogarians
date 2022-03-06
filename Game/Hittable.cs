using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Splatoon2D
{
    public class Hittable : PhysicalObject
    {
        public int total_life, life, loot, shake_force;
        public static Sprite balloon1, balloon2, balloon3, balloon4;
        public static Sprite frogtarian_idle, frogtarian_move, frogtarian_shoot, interrogation, exclamation;
        public static Sprite big_frogarian_idle, big_frogarian_press;
        public static SoundEffect bell_sound, ballon_pop_sound;
        //Color inkable_surface_color; // color of the sprite that will receive player ink
        static Effect InkEffect;
        static Texture2D ink_text0, ink_text1, ink_text2, ink_text3, ink_text4, ink_text5;
        public Effect _InkEffect;
        public Vector2 shake_offset;
        public Hittable(Vector2 Size, Vector2 Position):base(Size, Position)
        {
            _InkEffect = InkEffect.Clone();
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            if (lifetime % 5 == 1) shake_offset *= -0.6f;

            foreach(PhysicalObject o in world.Stuff)
            {
                if(o is InkShot shot)
                {
                    if(ShotTouched(shot))
                    {
                        ShotReaction(world, player, shot);
                    }
                }
            }

            if (life <= 0)
            {
                Die(world);
            }

            base.Update(gameTime, world, player);
        }

        public virtual void ShotReaction(World world, Player player, InkShot shot)
        {
            Console.WriteLine("Shot touched by " + this);
            life--;
            world.Remove(shot);
            //display_offset = shake_force * new Vector2(-1, 0);
            Vector2 shake_direction = Hurtbox.Center.ToVector2() - shot.Hurtbox.Center.ToVector2();
            shake_direction.Normalize();
            shake_offset = shake_force * shake_direction;
        }

        public virtual bool ShotTouched(InkShot o)
        {
            return o.Hurtbox.Intersects(this.Hurtbox);
        }

        public virtual void Die(World world)
        {
            HUD.SpawnEgg(loot, FeetPosition);
            world.Remove(this);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearWrap, null, null, null, transformMatrix: Game1.matrix);
            /*
            */
            //
            _InkEffect.Parameters["jam_texture"].SetValue(GetInkText());

            float x_offset = (CurrentSprite.frameIndex % (CurrentSprite.Texture.Width / CurrentSprite.frameWidth)) * CurrentSprite.frameWidth;
            float y_offset = (CurrentSprite.frameIndex - (CurrentSprite.frameIndex % (CurrentSprite.Texture.Width / CurrentSprite.frameWidth))) * CurrentSprite.frameHeight;
            _InkEffect.Parameters["spritesheet_offset_x"].SetValue(x_offset); // (float)(CurrentSprite.frameWidth / (float)CurrentSprite.Texture.Width) *
            _InkEffect.Parameters["spritesheet_offset_y"].SetValue(y_offset);// (float)(CurrentSprite.frameIndex - (CurrentSprite.frameIndex % (CurrentSprite.Texture.Width / CurrentSprite.frameWidth))) / (CurrentSprite.Texture.Width / CurrentSprite.frameWidth)); // (CurrentSprite.frameHeight / (float)CurrentSprite.Texture.Height) *
            _InkEffect.Parameters["spritesheet_width"].SetValue((float)CurrentSprite.Texture.Width); // (float)(CurrentSprite.frameWidth / (float)CurrentSprite.Texture.Width) *
            _InkEffect.Parameters["spritesheet_height"].SetValue((float)CurrentSprite.Texture.Height);// (float)(CurrentSprite.frameIndex - (CurrentSprite.frameIndex % (CurrentSprite.Texture.Width / CurrentSprite.frameWidth))) / (CurrentSprite.Texture.Width / CurrentSprite.frameWidth)); // (CurrentSprite.frameHeight / (float)CurrentSprite.Texture.Height) *
            _InkEffect.Parameters["frame_number_x"].SetValue(CurrentSprite.Texture.Width / (float)CurrentSprite.frameWidth); // (float)(CurrentSprite.frameWidth / (float)CurrentSprite.Texture.Width) *
            _InkEffect.Parameters["frame_number_y"].SetValue(CurrentSprite.Texture.Height / (float)CurrentSprite.frameHeight);// (float)(CurrentSprite.frameIndex - (CurrentSprite.frameIndex % (CurrentSprite.Texture.Width / CurrentSprite.frameWidth))) / (CurrentSprite.Texture.Width / CurrentSprite.frameWidth)); // (CurrentSprite.frameHeight / (float)CurrentSprite.Texture.Height) *

            _InkEffect.CurrentTechnique.Passes[0].Apply();

            DrawCurrentSprite(spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicWrap, null, null, null, transformMatrix: Game1.matrix);
        }

        public virtual void DrawCurrentSprite(SpriteBatch spriteBatch)
        {
            CurrentSprite.DrawFromFeet(spriteBatch, FeetPosition + shake_offset);
        }

        public Texture2D GetInkText()
        {
            if (life / (float)total_life > 5 / 6f) return ink_text0;
            if (life / (float)total_life > 4 / 6f) return ink_text1;
            if (life / (float)total_life > 3 / 6f) return ink_text2;
            if (life / (float)total_life > 2 / 6f) return ink_text3;
            if (life / (float)total_life > 1 / 6f) return ink_text4;
            return ink_text5;
        }

        new public static void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            balloon1 = new Sprite(Content.Load<Texture2D>("balloon/balloon1"));
            balloon2 = new Sprite(Content.Load<Texture2D>("balloon/balloon2"));
            balloon3 = new Sprite(Content.Load<Texture2D>("balloon/balloon3"));
            balloon4 = new Sprite(Content.Load<Texture2D>("balloon/balloon4"));
            interrogation = new Sprite(Content.Load<Texture2D>("interrogation"));
            exclamation = new Sprite(Content.Load<Texture2D>("exclamation"));

            ballon_pop_sound = Content.Load<SoundEffect>("balloon/pop");
            bell_sound = Content.Load<SoundEffect>("bell_sound");

            frogtarian_idle = new Sprite(2, 151, 191, 333, Content.Load<Texture2D>("frog_idle"), FeetYOffset:10);
            frogtarian_move = new Sprite(6, 151, 191, 120, Content.Load<Texture2D>("frog_walk"), FeetYOffset: 10);
            frogtarian_shoot = new Sprite(3, 151, 191, 100, Content.Load<Texture2D>("frog_attack"), FeetYOffset: 10);

            big_frogarian_idle = new Sprite(2, 564/2, 185, 333, Content.Load<Texture2D>("large_frog_idle"), FeetYOffset: 0);
            big_frogarian_press = new Sprite(4, 564/2, 185, 300, Content.Load<Texture2D>("large_frog_attack"), FeetYOffset: 0, loopAnimation:false);

            InkEffect = Content.Load<Effect>("InkEffect");
            ink_text0 = Content.Load<Texture2D>("ink_effect/ink_text_0");
            ink_text1 = Content.Load<Texture2D>("ink_effect/ink1");
            ink_text2 = Content.Load<Texture2D>("ink_effect/ink2");
            ink_text3 = Content.Load<Texture2D>("ink_effect/ink3");
            ink_text4 = Content.Load<Texture2D>("ink_effect/ink4");
            ink_text5 = Content.Load<Texture2D>("ink_effect/ink5");

        }
    }
}
