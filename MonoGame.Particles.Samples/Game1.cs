using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using MonoGame.Particles.Particles;
using MonoGame.Particles.Particles.Modifiers;
using MonoGame.Particles.Physics;
using MonoGame.Particles.Physics.Debug;
using MonoGame.Particles.Samples.GameStateManagement;
using MonoGame.Particles.Samples.Properties;
using MonoGame.Particles.Samples.Screens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGuiNET;
using MonoGame.ImGui;

namespace MonoGame.Particles.Samples
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
       
        private FramesPerSecondCounterComponent framesPerSecondCounter;             
        private readonly ScreenManager screenManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.SynchronizeWithVerticalRetrace = false;
            _graphics.HardwareModeSwitch = false;
            this.IsFixedTimeStep = false;
            Window.IsBorderless = true;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);
        }

        protected override void Initialize()
        {
            
            //Load content from a resource file
            Content.RootDirectory = "Content";
            //ResourceContentManager resourceContentManager = new ResourceContentManager(this.Services, Resources.ResourceManager);
            //Content = resourceContentManager;

            framesPerSecondCounter = new FramesPerSecondCounterComponent(this);

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
           
            base.Initialize();
        }        

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);            
            font = Content.Load<SpriteFont>("Debug");            
        }
    
        protected override void Update(GameTime gameTime)
        {
            framesPerSecondCounter.Update(gameTime);            

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            KeyboardState state = Keyboard.GetState();
            if(state.IsKeyDown(Keys.D1) && screenManager.GetScreens().Length==0) screenManager.AddScreen(new DemoOneScreen(), null);
            if (state.IsKeyDown(Keys.D2) && screenManager.GetScreens().Length == 0) screenManager.AddScreen(new DemoTwoScreen(), null);
            if (state.IsKeyDown(Keys.D3) && screenManager.GetScreens().Length == 0) screenManager.AddScreen(new DemoThreeScreen(), null);
            if (state.IsKeyDown(Keys.D4) && screenManager.GetScreens().Length == 0) screenManager.AddScreen(new DemoFourScreen(), null);
            if (state.IsKeyDown(Keys.D5) && screenManager.GetScreens().Length == 0) screenManager.AddScreen(new DemoFiveScreen(), null);
            if (state.IsKeyDown(Keys.D6) && screenManager.GetScreens().Length == 0) screenManager.AddScreen(new DemoSixScreen(), null);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            framesPerSecondCounter.Draw(gameTime);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            if (screenManager.GetScreens().Length == 1)
            {
                _spriteBatch.DrawString(font, "Press X to return to main screen. Press ESC to exit.", new Vector2(0, 1056), Color.White * 0.8f);
            }
            else
            {
                _spriteBatch.DrawString(font, "Press 1-6 for demo. Press ESC to exit.", new Vector2(0, 1056), Color.White * 0.8f);
            }
            
            _spriteBatch.End();

            base.Draw(gameTime);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "FPS:              " + framesPerSecondCounter.FramesPerSecond, new Vector2(0, 16), Color.White * 0.8f);
            _spriteBatch.End();

        }
    }
}
