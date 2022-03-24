using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Splatoon2D
{
    public class PhysicalObject
    {
        internal static int IDcount = 0;
        internal int ID, lifetime;
        internal static Sprite bomb;
        internal Vector2 Velocity;
        internal Vector2 FeetPosition;
        internal Rectangle Hurtbox;
        internal Vector2 HurtboxSize;
        internal Sprite PreviousSprite, CurrentSprite;
        internal int SpriteFrames = 0;
        internal static Random r = new Random();
        internal float WallBounceFactor, GroundBounceFactor = 0f, GroundFactor, Gravity, XTreshold = 0.1f;
        internal bool is_particle = false, wallcollision = false, groundcollision = false, push_player = false, is_solid = false, player_hit = false, is_boxable = true, is_movable = true;

        public static Sprite bell_idle, bell_hit, marina_idle, marina_inked;
        public static Sprite rabbit_idle, rabbit_disappear, rabbit_rise, rabbit_fall, rabbit_charge, rabbit_button, rabbit_press;
        public static Sprite pearl_sprite, pearl_inked, tofu_sprite, crypto1, crypto2, crypto3;
        public static Sprite bump_locked, bump_idle, bump_bump, zapfish_idle, zapfish_win;
        public static SoundEffect marina_sound1, marina_sound2, explosion, bump, question_sound, victory, enemy_shoot, frog_hit, rabbit_sound1, rabbit_sound2, rabbit_sound3;
        public static void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            bell_idle = new Sprite(Content.Load<Texture2D>("bell_idle"));
            bell_hit = new Sprite(3, 209, 172, 100, Content.Load<Texture2D>("bell_ring"), loopAnimation:false);
            marina_idle = new Sprite(4, 129, 154, 220, Content.Load<Texture2D>("marina_idle"));
            marina_inked = new Sprite(2, 129, 154, 220, Content.Load<Texture2D>("marina_inked"));
            marina_sound1 = Content.Load<SoundEffect>("marina1");
            marina_sound2 = Content.Load<SoundEffect>("marina2");

            rabbit_idle = new Sprite(2, 218/2, 162, 240, Content.Load<Texture2D>("rabbit/idle"));
            rabbit_press = new Sprite(4, 484/4, 167, 130, Content.Load<Texture2D>("rabbit/press_button"), loopAnimation: false);
            rabbit_disappear = new Sprite(6, 726/6, 162, 130, Content.Load<Texture2D>("rabbit/disappear"), loopAnimation:false);
            rabbit_rise = new Sprite(Content.Load<Texture2D>("rabbit/rise"));
            rabbit_fall = new Sprite(Content.Load<Texture2D>("rabbit/fall"));
            rabbit_charge = new Sprite(Content.Load<Texture2D>("rabbit/charge_jump"));
            rabbit_button = new Sprite(Content.Load<Texture2D>("rabbit/button_out"));
            tofu_sprite = new Sprite(2, 69, 54, 224, Content.Load<Texture2D>("tofu"));
            pearl_sprite = new Sprite(4, 552/4, 184, 30, Content.Load<Texture2D>("pearl"));
            crypto1 = new Sprite(Content.Load<Texture2D>("crypto1"));
            crypto2 = new Sprite(Content.Load<Texture2D>("crypto2"));
            crypto3 = new Sprite(Content.Load<Texture2D>("crypto3"));
            rabbit_sound1 = Content.Load<SoundEffect>("rabbit/sound1");
            rabbit_sound2 = Content.Load<SoundEffect>("rabbit/sound2");
            rabbit_sound3 = Content.Load<SoundEffect>("rabbit/sound3");
            bump_idle = new Sprite(Content.Load<Texture2D>("bumper_neutral"));
            bump_bump = new Sprite(2, 123, 75, 88, Content.Load<Texture2D>("bumper_bump"), loopAnimation: false);
            bump_locked = new Sprite(Content.Load<Texture2D>("bumper_empty"));
            pearl_inked = new Sprite(Content.Load<Texture2D>("pearl_protect"));
            zapfish_idle = new Sprite(2, 366/2, 145, 230, Content.Load<Texture2D>("zapfish"));
            zapfish_win = new Sprite(Content.Load<Texture2D>("zapfish_celebrate"));

            // misc sound effect
            explosion = Content.Load<SoundEffect>("explosion");
            bump = Content.Load<SoundEffect>("bump");
            question_sound = Content.Load<SoundEffect>("question");
            victory = Content.Load<SoundEffect>("victory");
            enemy_shoot = Content.Load<SoundEffect>("enemy_shoot");
            frog_hit = Content.Load<SoundEffect>("frog_hit");
        }

        public PhysicalObject(Vector2 HurtboxSize, Vector2 FeetPosition, bool isParticle = false)
        {
            is_particle = isParticle;
            IDcount += 1;
            ID = IDcount;
            lifetime = 0;

            this.HurtboxSize = HurtboxSize;
            this.FeetPosition = FeetPosition;

            UpdateHurtbox();
        }

        public void UpdateHurtbox()
        {
            Hurtbox.X = (int)Math.Floor(FeetPosition.X - HurtboxSize.X / 2);
            Hurtbox.Width = (int)Math.Floor(HurtboxSize.X);
            Hurtbox.Y = (int)Math.Floor(FeetPosition.Y - HurtboxSize.Y);
            Hurtbox.Height = (int)Math.Floor(HurtboxSize.Y);
        }
        public virtual void Update(GameTime gameTime, World world, Player player)
        {
            lifetime += 1;

            if (PreviousSprite == CurrentSprite)
                SpriteFrames += 1;
            else SpriteFrames = 0;

            PreviousSprite = CurrentSprite;

            if (IsOnGround(world)) Velocity.X *= GroundFactor;

            ApplyForce(new Vector2(0, Gravity));

            if (Velocity.X * Velocity.X < XTreshold) Velocity.X = 0;
            if (Velocity.Y * Velocity.Y < XTreshold) Velocity.Y = 0; // if you remove this bullet will fall for some reason

            CheckCollisions(world);
        }

        public virtual void CheckCollisions(World world)
        {
            Vector2 IntVelocity = Velocity;
            if (IntVelocity.Y > 0) IntVelocity.Y = (float)Math.Ceiling(IntVelocity.Y);
            if (IntVelocity.X > 0) IntVelocity.X = (float)Math.Ceiling(IntVelocity.X);
            if (IntVelocity.Y < 0) IntVelocity.Y = (float)Math.Floor(IntVelocity.Y);
            if (IntVelocity.X < 0) IntVelocity.X = (float)Math.Floor(IntVelocity.X);
                groundcollision = false;

            if (!groundcollision) if (world.CheckCollision(Hurtbox, new Vector2(0, IntVelocity.Y))) // Check collision with the world
                {
                    groundcollision = true;
                    // Should Offset hitbox to ground ?
                }
            if (groundcollision) Velocity.Y = -GroundBounceFactor * Velocity.Y;
            FeetPosition.Y += Velocity.Y; // Apply Y movement
            Hurtbox.Y = (int)Math.Floor(FeetPosition.Y - HurtboxSize.Y);
            Hurtbox.Height = (int)Math.Floor(HurtboxSize.Y);

            wallcollision = false;

            if (!wallcollision) if (world.CheckCollision(Hurtbox, new Vector2(IntVelocity.X, 0)))// Collision X with the world
                {
                    wallcollision = true;
                }
            if(wallcollision) Velocity.X = -WallBounceFactor * Velocity.X;

            FeetPosition.X += Velocity.X;
            Hurtbox.X = (int)Math.Floor(FeetPosition.X - HurtboxSize.X / 2);
            Hurtbox.Width = (int)Math.Floor(HurtboxSize.X);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            CurrentSprite.DrawFromFeet(spriteBatch, new Vector2(FeetPosition.X, FeetPosition.Y + 1));
        }
        public virtual void ApplyForce(Vector2 force)
        {
            if(is_movable) Velocity += force;
        }
        public bool IsOnGround(World world)
        {
            if ((Velocity.Y >= 0) && (world.CheckCollision(Hurtbox, new Vector2(0, Velocity.Y + 2)))) return true;
            return false;
        }

        public bool IsOnWorldGround(World world)
        {
            if (Velocity.Y >= 0)
            {
                return (world.CheckCollision(Hurtbox, new Vector2(0, Velocity.Y + 2))) ;
            }
            else return false;
        }

        public bool CheckCollision(Rectangle OtherObjectHurtbox, Vector2 OtherObjectVelocity)
        {
            Rectangle moved_rectangle = OtherObjectHurtbox;
            moved_rectangle.Offset(OtherObjectVelocity);
            return moved_rectangle.Intersects(Hurtbox);
        }

        public int Direction(PhysicalObject o)
        {
            if (o.FeetPosition.X >= FeetPosition.X) return 1;
            else return -1;
        }

        public float DistanceWith(PhysicalObject o)
        {
            return (o.Hurtbox.Center.ToVector2() - Hurtbox.Center.ToVector2()).Length();
        }
    }
}