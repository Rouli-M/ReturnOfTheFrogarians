using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Linq.Expressions;
using System.Diagnostics;


namespace Splatoon2D
{
    public class Player : PhysicalObject
    {
        static Sprite idle, walk, to_squid, to_kid;
        static Sprite squid_idle, squid_walk, squid_rise, squid_top, squid_fall;
        static Sprite gun_rest, gun_shoot, gun_cocked;
        static Sprite ArmSprite;
        static SoundEffect shoot_sound, to_squid_sound;
        static Effect Jam;

        public static Vector2 human_size;

        public Vector2 MaskPosition, MaskSpriteOffset, CurrentMaskPosition, LastTrailGenerated;
        List<Vector2> CurrentMaskPositionList;
        public enum PlayerState { idle, run, jump, attack, to_squid, to_kid }
        public PlayerState CurrentState, PreviousState;
        public enum PlayerForm { kid, squid }
        public PlayerForm CurrentForm, PreviousForm;

        World world;
        Random random = new Random();

        public Vector2 HitboxSize, HitboxOffset;
        public Rectangle Hitbox;
        public int Direction;

        public int form_frames { get; private set; } 
        public int state_frames { get; private set; }
        public int shoot_cooldown = 0;

        public Player():base(human_size, new Vector2(0,0))
        {
            FeetPosition = new Vector2(200, -1);
            CurrentState = PlayerState.idle;
            CurrentForm = PlayerForm.kid;
           
            WallBounceFactor = 0f;
            GroundBounceFactor = 0f;
            GroundFactor = 0.5f;
            Gravity = 0.7f;
            
            HurtboxSize = human_size;
            Velocity = new Vector2(0, 0);
            Direction = 1;
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            this.world = world;
            //juice = 70;

            if (PreviousForm == CurrentForm) form_frames += 1;
            else form_frames = 0;
            PreviousForm = CurrentForm;
            if (PreviousState == CurrentState) state_frames += 1;
            else state_frames = 0;
            PreviousState = CurrentState;

            if (FeetPosition.Y > 1500)
            {
                FeetPosition = new Vector2(0, -1);
                Velocity = new Vector2(0, 0);
            }

            GroundFactor = 0.5f;

            HitboxSize = new Vector2(0, 0);
            HitboxOffset = new Vector2(0, 0);

            switch (CurrentForm)
            {
                case PlayerForm.kid:
                    {
                        switch (CurrentState)
                        {
                            case (PlayerState.idle):
                                {
                                    if (Input.aim_direction != 0) Direction = Input.aim_direction;
                                    else if (Input.movement_direction != 0) Direction = Math.Sign(Input.movement_direction);

                                    if (Input.movement_direction != 0)
                                    {
                                        Velocity.X = 1 * Input.movement_direction ;
                                        //ApplyForce(new Vector2(Input.movement_direction, 0));
                                        CurrentState = PlayerState.run;
                                    }
                                    break;
                                }
                            case (PlayerState.run):
                                {
                                    ApplyForce(new Vector2(5 * Input.movement_direction, 0));
                                    if (Input.movement_direction == 0) CurrentState = PlayerState.idle;

                                    if (Input.aim_direction != 0) Direction = Input.aim_direction;
                                    else if (Input.movement_direction != 0) Direction = Math.Sign(Input.movement_direction);

                                    break;
                                }
                            case (PlayerState.jump):
                                {
                                    break;
                                }
                                
                            case (PlayerState.to_kid):
                                {
                                    CurrentState = PlayerState.idle;
                                    break;
                                }
                                
                            case (PlayerState.to_squid):
                                {
                                    if(state_frames > 1)
                                    {
                                        CurrentForm = PlayerForm.squid;
                                        CurrentState = PlayerState.idle;
                                    }
                                    break;
                                }

                        }
                        if(Input.Squid)
                        {
                            CurrentState = PlayerState.to_squid;
                        }
                        break;
                    }
                case PlayerForm.squid:
                    {
                        switch (CurrentState)
                        {
                            case (PlayerState.idle):
                                {
                                    if (Input.movement_direction != 0)
                                    {
                                        //Velocity.X = 1 * Input.movement_direction;
                                        //ApplyForce(new Vector2(Input.movement_direction, 0));
                                        CurrentState = PlayerState.run;
                                    }
                                    break;
                                }
                            case (PlayerState.run):
                                {
                                    GroundFactor = 0.93f;
                                    if (CurrentSprite.firstFrame)
                                    {
                                        ApplyForce(new Vector2(Input.movement_direction * 7, 0));
                                        Direction = Math.Sign(Input.movement_direction);
                                    }
                                    else if (CurrentSprite.isOver)
                                    {
                                        if (Input.movement_direction == 0) CurrentState = PlayerState.idle;
                                        else CurrentSprite.ResetAnimation();
                                    }
                                    break;
                                }
                            case (PlayerState.jump):
                                {

                                    break;
                                }
                            case (PlayerState.to_squid):
                                {
                                    CurrentState = PlayerState.idle;
                                    break;
                                }
                            case (PlayerState.to_kid):
                                {
                                    if (state_frames > 2)
                                    {
                                        CurrentForm = PlayerForm.kid;
                                        CurrentState = PlayerState.idle;
                                    }
                                    break;
                                }
                        }
                        if (!Input.Squid)
                        {
                            CurrentState = PlayerState.to_kid;
                        }
                        break;
                    }
            }

            if (Input.Shoot)
            {
                if (shoot_cooldown == 0)
                {
                    Vector2 InkSpawnPoint = FeetPosition;
                    Vector2 ArmJointPos = GetArmRelativePoint();
                    if (Direction == -1) ArmJointPos.X *= -1;

                    Vector2 OffsetInAimDirection = 77 * new Vector2((float)Math.Cos(Input.Angle), -(float)Math.Sin(Input.Angle));
                    Vector2 OffsetInWeaponExitDirection = (Direction == 1 ? 1 : -1) * 14 * new Vector2((float)Math.Cos(Input.Angle + Math.PI/2f), -(float)Math.Sin(Input.Angle + Math.PI / 2f));
                    InkSpawnPoint += ArmJointPos;
                    InkSpawnPoint += OffsetInAimDirection;
                    //InkSpawnPoint += OffsetInWeaponExitDirection;
                    
                    world.Stuff.Add(new InkShot(InkSpawnPoint, Input.Angle));
                    gun_shoot.ResetAnimation();
                    ArmSprite = gun_shoot;
                    shoot_cooldown = 14;
                }
                else
                {
                    if (ArmSprite != gun_shoot || ArmSprite == gun_shoot && ArmSprite.isOver) ArmSprite = gun_cocked;
                }
            }
            if (shoot_cooldown > 0) shoot_cooldown--;
            else ArmSprite = gun_rest;

            //
            // SPRITE DETERMINATION
            //
            PreviousSprite = CurrentSprite;
            switch (CurrentForm)
            {
                case (PlayerForm.kid):
                    {
                        if (CurrentState == PlayerState.idle) CurrentSprite = idle;
                        else if (CurrentState == PlayerState.run) CurrentSprite = walk;
                        else if (CurrentState == PlayerState.jump)
                        {

                        }
                        else if (CurrentState == PlayerState.to_squid) CurrentSprite = to_squid;
                        break;
                    }
                case (PlayerForm.squid):
                    {
                        if (CurrentState == PlayerState.idle) CurrentSprite = squid_idle;
                        else if (CurrentState == PlayerState.run) CurrentSprite = squid_walk;
                        else if (CurrentState == PlayerState.jump)
                        {
                            if (Velocity.Y < -2) CurrentSprite = squid_rise;
                            else if (Velocity.Y > 2) CurrentSprite = squid_fall;
                            else CurrentSprite = squid_top;
                        }
                        else if (CurrentState == PlayerState.to_kid) CurrentSprite = to_kid;
                        break;
                    }
            }

           // Console.WriteLine(CurrentState + " form: " + CurrentForm);

            if (CurrentSprite != PreviousSprite)
            {
                CurrentSprite.ResetAnimation();
                //Console.WriteLine("Switched to sprite " + CurrentSprite.Texture.Name);
            }
            CurrentSprite.direction = Direction;
            CurrentSprite.UpdateFrame(gameTime);
            ArmSprite.direction = Direction;
            ArmSprite.UpdateFrame(gameTime);

            foreach (PhysicalObject o in world.Stuff) { }

            base.Update(gameTime, world, this);
            Hitbox = UpdateHitbox(FeetPosition);
        }



        public void Draw(SpriteBatch spriteBatch)
        {
            CurrentSprite.DrawFromFeet(spriteBatch, FeetPosition) ;

            if(Direction == 1) ArmSprite.Draw(spriteBatch, FeetPosition + GetArmRelativePoint() - GetWeaponRelativePoint(), -Input.Angle, GetWeaponRelativePoint());
            else ArmSprite.Draw(spriteBatch, FeetPosition + new Vector2(-GetArmRelativePoint().X, GetArmRelativePoint().Y) - new Vector2(ArmSprite.frameWidth, 0) + new Vector2(GetWeaponRelativePoint().X, -GetWeaponRelativePoint().Y), (float)Math.PI - Input.Angle, new Vector2(ArmSprite.frameWidth, 0) + new Vector2(-GetWeaponRelativePoint().X, GetWeaponRelativePoint().Y));
        }

        public Vector2 GetArmRelativePoint()
        {
            Sprite sprite = CurrentSprite;
            if(sprite == idle)
            {
                if (sprite.frameIndex == 0) return new Vector2(-5, -76);
                else return new Vector2(-5, -74);
            }
            else if (sprite == walk)
            {
                if (sprite.frameIndex == 3) return new Vector2(4, -73);
                else if(sprite.frameIndex == 0) return new Vector2(4, -73-1);
                else if(sprite.frameIndex == 1) return new Vector2(4, -73+1);
                else return new Vector2(4, -73+2);
            }
            return new Vector2(0, -100000000); // dirty but life's dirty
        }

        public Vector2 GetWeaponRelativePoint()
        {
            if(ArmSprite == gun_rest) return new Vector2(7, 16);
            else if (ArmSprite == gun_cocked) return new Vector2(7, 28);
            else if (ArmSprite == gun_shoot) return new Vector2(7, 28);
            return new Vector2(0, 0);
        }


        private Rectangle UpdateHitbox(Vector2 FPosition)
        {
            Rectangle MovedHitbox = new Rectangle();
            if (HitboxSize != new Vector2(0, 0))
            {
                if (HitboxOffset == Vector2.Zero)
                {
                    MovedHitbox.X = (int)Math.Floor(FPosition.X - HitboxSize.X / 2);
                    MovedHitbox.Y = (int)Math.Floor(FPosition.Y - HitboxSize.Y);
                    MovedHitbox.Width = (int)Math.Ceiling(HitboxSize.X);
                    MovedHitbox.Height = (int)Math.Ceiling(HitboxSize.Y);
                }
                else
                {
                    MovedHitbox.X = (int)Math.Floor(FPosition.X + HitboxOffset.X);
                    MovedHitbox.Y = (int)Math.Floor(FPosition.Y + HitboxOffset.Y);
                    MovedHitbox.Width = (int)Math.Ceiling(HitboxSize.X);
                    MovedHitbox.Height = (int)Math.Ceiling(HitboxSize.Y);

                    if (Direction == -1) MovedHitbox.Offset(new Vector2(-1 * (HitboxSize.X + 2 * HitboxOffset.X), 0));
                }
            }
            else MovedHitbox = new Rectangle(0, 0, 0, 0);

            return MovedHitbox;
        }
        
        public override void CheckCollisions(World world)
        {
            // Rectangle SupposedHitbox = UpdateHitbox(FeetPosition + Velocity); 
            // SupposedHitbox.Inflate(2, 2);
            Vector2 IntVelocity = Velocity;
            if (IntVelocity.Y > 0) IntVelocity.Y = (float)Math.Ceiling(IntVelocity.Y);
            if (IntVelocity.X > 0) IntVelocity.X = (float)Math.Ceiling(IntVelocity.X);
            if (IntVelocity.Y < 0) IntVelocity.Y = (float)Math.Floor(IntVelocity.Y);
            if (IntVelocity.X < 0) IntVelocity.X = (float)Math.Floor(IntVelocity.X);

            Rectangle SupposedHitbox = UpdateHitbox(FeetPosition + IntVelocity);
           
            foreach (PhysicalObject o in world.Stuff) if (o.is_solid && o != this) if (SupposedHitbox.Intersects(o.Hurtbox)) o.player_hit = true;

            groundcollision = false;
            foreach (PhysicalObject o in world.Stuff) if (!groundcollision) if (o.is_solid && o != this) // Collision with other objects
                            if (o.CheckCollision(Hurtbox, new Vector2(0, IntVelocity.Y)))
                            {
                                Velocity.Y = -GroundBounceFactor * Velocity.Y;
                                groundcollision = true;
                            }
            if (!groundcollision) if (world.CheckCollision(Hurtbox, new Vector2(0, IntVelocity.Y))) // Check collision with the world
                {
                    Velocity.Y = -GroundBounceFactor * Velocity.Y;
                    groundcollision = true;
                    // Should Offset hitbox to ground ?
                }
            FeetPosition.Y += Velocity.Y; // Apply Y movement
            Hurtbox.Y = (int)Math.Floor(FeetPosition.Y - HurtboxSize.Y);
            Hurtbox.Height = (int)Math.Floor(HurtboxSize.Y);

            List<Rectangle> WallCollisionCandidates = new List<Rectangle>();

            wallcollision = false;
            foreach (PhysicalObject o in world.Stuff) if (!wallcollision) if (o.is_solid && o != this) // Collision X with other objects
                            if (o.CheckCollision(Hurtbox, new Vector2(IntVelocity.X, 0)))
                            {
                                WallCollisionCandidates.Add(o.Hurtbox);
                            }
            WallCollisionCandidates.AddRange(world.CheckCollisionReturnRectangles(Hurtbox, new Vector2(IntVelocity.X, 0)));
            if (WallCollisionCandidates.Count > 0)
            {
                wallcollision = true;
                if(IntVelocity.X > 0) // bump to left of rectangle
                {
                    Rectangle ClosestRectangle = WallCollisionCandidates[0];
                    foreach (Rectangle r in WallCollisionCandidates) if (r.Left < ClosestRectangle.Left) ClosestRectangle = r;
                    if (!world.CheckCollision(Hurtbox, new Vector2(ClosestRectangle.Left - Hurtbox.Right, 0))) FeetPosition.X += ClosestRectangle.Left - Hurtbox.Right;
                }
                else // if (Velocity.X < 0)
                {

                    Rectangle ClosestRectangle = WallCollisionCandidates[0];
                    foreach (Rectangle r in WallCollisionCandidates) if (r.Right > ClosestRectangle.Right) ClosestRectangle = r;
                    if (!world.CheckCollision(Hurtbox, new Vector2(ClosestRectangle.Right - Hurtbox.Left, 0))) FeetPosition.X += ClosestRectangle.Right - Hurtbox.Left;
                }
                Velocity.X = -Velocity.X * WallBounceFactor;
            }

            FeetPosition.X += Velocity.X;
            Hurtbox.X = (int)Math.Floor(FeetPosition.X - HurtboxSize.X / 2);
            Hurtbox.Width = (int)Math.Floor(HurtboxSize.X);
        }
        

        public static void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            idle = new Sprite(2, 81, 133, 250, Content.Load<Texture2D>("idle"), FeetOffset:1);
            walk = new Sprite(4, 436/4, 131, 130, Content.Load<Texture2D>("walk"), FeetOffset: 1);
            to_squid = new Sprite(Content.Load<Texture2D>("uuuh"), FeetYOffset: 1);
            to_kid = new Sprite(Content.Load<Texture2D>("uuuh"), FeetYOffset:1);
            squid_idle = new Sprite(2, 190/2, 30, 400, Content.Load<Texture2D>("squid_idle"), FeetOffset: 6);
            squid_walk = new Sprite(3, 351/3, 35, 150, Content.Load<Texture2D>("squid_move"), 1f, false, FeetOffset: 6);
            gun_rest = new Sprite(Content.Load<Texture2D>("gun"));
            gun_cocked = new Sprite(Content.Load<Texture2D>("gun_cocked"));
            gun_shoot = new Sprite(2, 85, 39, 70, Content.Load<Texture2D>("gun_shoot"), loopAnimation:false);

            // Init here because Constructor seems to be called before the content is actually loaded?
            ArmSprite = gun_rest;
        }
    }
}