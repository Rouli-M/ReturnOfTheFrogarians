using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Windows;
using System.Windows.Forms;

namespace Splatoon2D
{
    static class Input
    {
        public static float movement_direction; // float from -1.0 to 1.0
        public static Vector2 movement_vector;
        public static bool GamepadUsed = false;
        public static int aim_direction;
        public static bool Jump, Shoot, Squid;
        public static float Angle = 0f; // between zero and 2 * Math.PI
        public static KeyboardState ks, old_ks;
        public static MouseState ms, old_ms;

        public static void Update(Player player)
        {
            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);
            ks = Keyboard.GetState();
            ms = Mouse.GetState();

            if (old_ks == null) old_ks = ks;
            if (old_ms == null) old_ms = ms;
            
            // If there a controller attached, handle it
            if (capabilities.IsConnected)
            {
                GamepadUsed = true;
                GamePadState gp = GamePad.GetState(PlayerIndex.One);
                Jump = gp.IsButtonDown(Buttons.A);
                Shoot = gp.Triggers.Right > 0.5;
                Squid = gp.Triggers.Left > 0.5;
                movement_direction = gp.ThumbSticks.Left.X;
                movement_vector = gp.ThumbSticks.Left;

                if (gp.ThumbSticks.Right.X != 0 || gp.ThumbSticks.Right.Y != 0)
                {
                    Angle = (float)Math.Atan2(gp.ThumbSticks.Right.Y, gp.ThumbSticks.Right.X);
                    //aim_direction = Math.Sign(gp.ThumbSticks.Right.X);
                }
                else if (gp.ThumbSticks.Left.X != 0)
                {
                    //aim_direction = Math.Sign(gp.ThumbSticks.Right.X);
                    if (gp.ThumbSticks.Left.X > 0) Angle = 0;
                    else Angle = (float)Math.PI;
                }
            }
            else
            {
                GamepadUsed = false;
                Jump = ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Z) || ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W) || ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up);
                Shoot = ms.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                Squid = ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);

                if (LeftPressed(ks) && RightPressed(ks) || !LeftPressed(ks) && !RightPressed(ks)) movement_direction = 0;
                else if (LeftPressed(ks)) movement_direction = -1;
                else if (RightPressed(ks)) movement_direction = 1;

                if (!Jump) movement_vector = new Vector2(movement_direction, 0);
                else if (movement_direction != 0) movement_vector = new Vector2(movement_direction * 0.5f, -0.5f);
                else movement_vector = new Vector2(0, -1f);

                Vector2 ScreenMousePosition = ms.Position.ToVector2() * 1 / Camera.Zoom + Camera.TopLeftCameraPosition; // dark magic stuff

                Vector2 PlayerToMouse = ScreenMousePosition - player.FeetPosition - player.GetArmRelativePoint();
                Angle = (float)Math.Atan2(-PlayerToMouse.Y, PlayerToMouse.X);
            }

            Angle = (float)(Angle % (Math.PI * 2));
            aim_direction = Math.Sign((float)Math.Cos(Angle));

#if DEBUG
            if (ms.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) Clipboard.SetText(HUD.GetPointerPosition(ms.Position.ToVector2()));
#endif
        }

        private static bool LeftPressed(KeyboardState ks)
        {
            return (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q) || ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A) || ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left));
        }

        private static bool RightPressed(KeyboardState ks)
        {
            return (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D) || ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right));
        }
    }
}
