using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Splatoon2D
{
    public class Particle:PhysicalObject
    {
        int total_lifetime;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public Particle(Vector2 Spawn, Sprite sprite, int total_life = 0):base(new Vector2(10, 10), Spawn, true)
        {
            CurrentSprite = new Sprite(sprite);
            total_lifetime = total_life;
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
        }
        public Particle(Vector2 Spawn, Sprite sprite, Vector2 Velocity, Vector2 Acceleration, int total_life = 0) : base(new Vector2(10, 10), Spawn, true)
        {
            CurrentSprite = new Sprite(sprite);
            total_lifetime = total_life;
            this.Velocity = Velocity;
            this.Acceleration = Acceleration;
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            if (CurrentSprite.isOver) world.Remove(this);
            CurrentSprite.UpdateFrame(gameTime);
            if (lifetime > total_lifetime && total_lifetime > 0)
                world.Remove(this);

            Velocity += Acceleration;
            FeetPosition += Velocity;

            lifetime++;
        }
    }
}
