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

        public static void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            //bomb = Content.Load<Sprite>("bomb");
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
    }
}