using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System.Globalization;
using System.Diagnostics;

namespace Splatoon2D
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        static Texture2D rectangle;
        public Player player;
        public World world;
        int totalGameTime, FramerateProblemFrames;
        Matrix matrix;
        public static MouseState ms, previous_ms;
        public static KeyboardState ks;
        public SpriteFont Rouli;
        static public float GlobalResizeRatio, BadFramerateRegister;
        static List<Rectangle> RectangleToDrawList = new List<Rectangle>() { };

        public Game1()
        {
            Console.WriteLine("Game1 instanced");

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.SupportedOrientations = Microsoft.Xna.Framework.DisplayOrientation.LandscapeLeft | Microsoft.Xna.Framework.DisplayOrientation.LandscapeRight;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferMultiSampling = false;

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            var ActualWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            var ActualHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Window.Title = "Splatoon 2D fangame";
            Window.AllowUserResizing = false;

            string culture = Convert.ToString(CultureInfo.CurrentCulture.Name);
            Console.WriteLine("TwoLetterISOLanguageName" + CultureInfo.CurrentCulture.TwoLetterISOLanguageName);

            GlobalResizeRatio = Math.Max((float)ActualWidth / graphics.PreferredBackBufferWidth, (float)ActualWidth / graphics.PreferredBackBufferHeight);

            Console.WriteLine("end of Game1 instanciation");
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Console.WriteLine("Initialize() called");
          
            float ActualWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            float ActualHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            float TestWidth = ActualWidth * 720 / 1280f;
            float TestHeight = ActualHeight * 1280 / 720f;
            if (TestWidth > TestHeight)
            {
                ActualHeight = ActualWidth * 1280 / 720f;
            }
            else
            {
                ActualWidth = ActualHeight * 1280 / 720f;

            }
            var scaleX = 1f;
            var scaleY = 1f;

            base.Initialize();


            matrix = Matrix.CreateScale(scaleX, scaleY, 1.0f);
            player = new Player();
            world = new World(player);
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Console.WriteLine("LoadContent called");
            DateTime LoadContentBegin = System.DateTime.Now;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Rouli = Content.Load<SpriteFont>("RouliXL");
            //rectangle = Content.Load<Texture2D>("rectangle");
            Player.LoadContent(Content);
            PhysicalObject.LoadContent(Content);
            World.LoadContent(Content);
            HUD.LoadContent(Content);
            InkShot.LoadContent(Content);
            Hittable.LoadContent(Content);
            Egg.LoadContent(Content);
            base.LoadContent(); // ???
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            ks = Keyboard.GetState();
            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);
            // If there a controller attached, handle it
            Input.Update(player);

            previous_ms = ms;
            player.Update(gameTime, world, player);
            world.Update(gameTime, player);
            Camera.Update(player, world);
            HUD.Update();
            SoundEffectPlayer.Update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            totalGameTime += gameTime.ElapsedGameTime.Milliseconds;
            //spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null, null, transformMatrix: matrix);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicWrap, null, null, null, transformMatrix: matrix);

            GraphicsDevice.Clear(Color.White);
            world.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearWrap, null, null, null, transformMatrix: matrix); // we need Immediate only for the body in order to draw the effect
            player.Draw(spriteBatch);
            HUD.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void InitGame()
        {
            player = new Player();
            world = new World(player);
           
            Camera.Reset(player);
        }

        public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangleToDraw, Color color, Texture2D texture = null, bool start_top_off_texture = false, bool start_left_off_texture = false)
        {
            if (texture == null) texture = rectangle;
            spriteBatch.Draw(texture,
                new Rectangle((int)((rectangleToDraw.X - Camera.TopLeftCameraPosition.X) * Camera.Zoom),
                              (int)((rectangleToDraw.Y - (int)Camera.TopLeftCameraPosition.Y) * Camera.Zoom),
                              (int)(rectangleToDraw.Width * Camera.Zoom),
                              (int)(rectangleToDraw.Height * Camera.Zoom)),
                new Rectangle(start_left_off_texture ? 0 : rectangleToDraw.X % texture.Width,
                              start_top_off_texture ? 0 : rectangleToDraw.Y % texture.Height,
                              (int)(rectangleToDraw.Width),
                              (int)(rectangleToDraw.Height)),
                color,
                0f,
                new Vector2(0, 0),
                SpriteEffects.None,
                0f);
        }

        public static void StringCenterDraw(SpriteBatch spriteBatch, string str, SpriteFont font, float Y_Position, Color text_color, float scale)
        {
            spriteBatch.DrawString(font, str, new Vector2(1280 * 0.5f, Y_Position) - font.MeasureString(str) * 0.5f * scale, text_color, 0f, default, scale, SpriteEffects.None, 0f);
        }

        public static void DrawRectangle(Rectangle rectangleToDraw)
        {
            Game1.RectangleToDrawList.Add(rectangleToDraw);
        }

        public static void DrawRectangleScreen(SpriteBatch spriteBatch, Rectangle rectangleToDraw, Color color, Rectangle Source, Texture2D texture = null)
        {
            if (texture == null) texture = rectangle;
            spriteBatch.Draw(texture,
                new Rectangle((int)(rectangleToDraw.X),
                              (int)(rectangleToDraw.Y),
                              (int)(rectangleToDraw.Width),
                              (int)(rectangleToDraw.Height)),
                new Rectangle((int)(Source.X % Source.Width),
                              (int)(Source.Y % Source.Height),
                              (int)(rectangleToDraw.Width),
                              (int)(rectangleToDraw.Height)),
                color, 0f, new Vector2(0, 0), SpriteEffects.None, 0f);
        }

        public static void DrawRectangleScreen(SpriteBatch spriteBatch, Rectangle rectangleToDraw, Color color, Texture2D texture = null)
        {
            if (texture == null) texture = rectangle;
            spriteBatch.Draw(texture,
                new Rectangle((int)(rectangleToDraw.X),
                              (int)(rectangleToDraw.Y),
                              (int)(rectangleToDraw.Width),
                              (int)(rectangleToDraw.Height)),
                new Rectangle(rectangleToDraw.X % texture.Width,
                              rectangleToDraw.Y % texture.Height,
                              (int)(rectangleToDraw.Width),
                              (int)(rectangleToDraw.Height)),
                color, 0f, new Vector2(0, 0),SpriteEffects.None, 0f);
        }

        public static void DrawPoint(SpriteBatch spriteBatch, Vector2 PointToDraw, Color color, int size = 5, Texture2D texture = null)
        {
            if (texture == null) texture = rectangle;
            spriteBatch.Draw(texture,
                new Rectangle((int)((PointToDraw.X - size/2f - Camera.TopLeftCameraPosition.X) * Camera.Zoom),
                              (int)((PointToDraw.Y - size/2f - (int)Camera.TopLeftCameraPosition.Y) * Camera.Zoom),
                              (int)(size* Camera.Zoom),
                              (int)(size * Camera.Zoom)),
                new Rectangle((int)PointToDraw.X % texture.Width,
                              (int)PointToDraw.Y % texture.Height,
                              (int)size,
                              (int)size),
                color,
                0f,
                new Vector2(0, 0),
                SpriteEffects.None,
                0f);
        }

        public static bool GetBool(Random r, float nb)
        {
            return (r.NextDouble() < nb);
        }

        public void DrawString(SpriteBatch spriteBatch, String str, Vector2 pos)
        {
            spriteBatch.DrawString(Rouli, str, Camera.Zoom * (pos - Camera.TopLeftCameraPosition), Color.White);
        }
        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Window_ClientSizeChanged");
            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            graphics.ApplyChanges();
        }

    }
}
