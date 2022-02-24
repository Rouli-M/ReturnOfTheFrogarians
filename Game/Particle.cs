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
        public Particle(Vector2 Spawn, Sprite sprite):base(new Vector2(10, 10), Spawn, true)
        {
            CurrentSprite = new Sprite(sprite);
        }

        public override void Update(GameTime gameTime, World world, Player player)
        {
            if (CurrentSprite.isOver) world.Remove(this);
            CurrentSprite.UpdateFrame(gameTime);
        }
    }
}
