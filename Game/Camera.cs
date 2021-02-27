using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Splatoon2D
{
    public static class Camera
    {
        public static float RegularZoom = 1.3f, Zoom = 1.3f;
        public static Vector2 TopLeftCameraPosition, CenterPositionDestination, CenterPosition = new Vector2(0,0), CameraOffset = new Vector2(0,0);
        public static Vector2 RoomRail, TopLeftRailCameraPosition, ScreenShake = new Vector2(0, 0);
        private static bool player_locked = false, totally_fixed = false;
        public static void Update(Player player, World world)
        {
            //ScreenShake *= 0.9f;
            if(player.lifetime % 5 == 0) ScreenShake *= -0.5f;

            CameraOffset = new Vector2();
            int i = 0;

            CameraOffset.Y += -200 * (1 / (2 * Zoom));

            if (totally_fixed) CenterPositionDestination = new Vector2(400, -150);
            else if (player_locked) CenterPositionDestination = new Vector2((player.FeetPosition.X ), player.FeetPosition.Y - 50);
            else CenterPositionDestination = new Vector2((player.FeetPosition.X + 120 * (float)Math.Cos(Input.Angle)), player.FeetPosition.Y - 50 * (float)Math.Sin(Input.Angle) - 50); // center the camera on the player
            CenterPositionDestination += CameraOffset;
            CenterPositionDestination += ScreenShake;

            //if (Input.Squid) Zoom += 0.01f;
            if(Zoom < RegularZoom) Zoom = RegularZoom;

            CenterPosition = 0.1f * (8 * CenterPosition + 2 * CenterPositionDestination);
            TopLeftCameraPosition = CenterPosition - 1 / Zoom * 0.5f * new Vector2(1280, 720);
        }

        public static void Reset(Player player)
        {
            CenterPosition = new Vector2(player.FeetPosition.X, player.FeetPosition.Y - 30);
            CameraOffset = new Vector2(0, 0);
        }

        public static void Shake(Vector2 ShakeDirection)
        {
            ScreenShake = ShakeDirection;
        }
    }
}