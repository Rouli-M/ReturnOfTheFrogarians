using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Splatoon2D
{
    static class Input
    {
        public static float movement_direction; // float from -1.0 to 1.0
        public static int aim_direction;
        public static bool Jump, Shoot, Squid;
        public static float Angle = 0f; // between zero and 2 * Math.PI

        public static void Update()
        {
            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);
            // If there a controller attached, handle it
            if (capabilities.IsConnected)
            {
                GamePadState gp = GamePad.GetState(PlayerIndex.One);
                Jump = gp.IsButtonDown(Buttons.A);
                Shoot = gp.Triggers.Right > 0.5;
                Squid = gp.Triggers.Left > 0.5;
                movement_direction = gp.ThumbSticks.Left.X;

                if (gp.ThumbSticks.Right.X != 0 || gp.ThumbSticks.Right.Y != 0)
                {
                    Angle = (float)Math.Atan2(gp.ThumbSticks.Right.Y, gp.ThumbSticks.Right.X);
                    aim_direction = Math.Sign(gp.ThumbSticks.Right.X);
                }
                else if (gp.ThumbSticks.Left.X != 0)
                {
                    aim_direction = Math.Sign(gp.ThumbSticks.Right.X);
                    if (gp.ThumbSticks.Left.X > 0) Angle = 0;
                    else Angle = (float)Math.PI;
                }

                Console.WriteLine(Angle);
            }

            Angle = (float)(Angle % (Math.PI * 2));
        }
    }
}
