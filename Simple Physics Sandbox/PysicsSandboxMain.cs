using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Simple_Physics_Sandbox
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class PysicsSandboxMain : Game
    {
        public static int MAXSHAPERADIUS = 80;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        PhysicsSpace physicsSpace;
        Texture2D circle;
        MouseState prevMouseState;
        Vector2 mouseDownPosition;

        public PysicsSandboxMain()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            physicsSpace = new PhysicsSpace(1280, 720, 80, 0.03f);
            IsMouseVisible = true;
            prevMouseState = Mouse.GetState();
            mouseDownPosition = new Vector2();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            circle = Content.Load<Texture2D>("sprites/circle");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Check for inputs
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
            {
                mouseDownPosition = mouseState.Position.ToVector2();
            }
            else if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
            {
                physicsSpace.PhysicsObjects.Add(new Circle(10, mouseState.Position.ToVector2(), mouseDownPosition - mouseState.Position.ToVector2(), 100));
            }
            prevMouseState = mouseState;

            //Update physicsObjects
            physicsSpace.UpdateObjects(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            foreach (PhysicsObject physicsObject in physicsSpace.PhysicsObjects)
            {
                if (physicsObject.GetPhysicsObjectType() == PhysicsObjectType.Circle)
                {
                    Circle c = (Circle)physicsObject;
                    spriteBatch.Draw(circle, c.Position, null, Color.White, 0, new Vector2(circle.Width / 2), 2 * c.Radius / circle.Width, SpriteEffects.None, 0);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
