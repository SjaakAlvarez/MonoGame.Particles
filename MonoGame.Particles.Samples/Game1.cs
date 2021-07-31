using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.VectorDraw;
using MonoGame.Particles.Particles;
using MonoGame.Particles.Physics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Particles.Samples
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private PrimitiveBatch _primitiveBatch;
        private PrimitiveDrawing _primitiveDrawing;
        private Matrix _localProjection;
        private Matrix _localView;
        
        PolygonShape polygon0;
        PolygonShape polygon1;
        PolygonShape polygon2;
        PolygonShape polygon3;
        Scene scene;
        Body b1;
        Body b3;
        Body b2;
        Body b4;

        Texture2D blank;        
        SpriteFont font;
        Random rand = new Random();
        double timer=3;
        double steptimer=16;

        const int CELLSIZE = 16;


        Emitter emitter;
        Emitter emitter2;

        private FramesPerSecondCounterComponent framesPerSecondCounter;
        
        //private List<Body> bodies = new List<Body>();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
           
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            scene = new Scene(1.0f, 1, CELLSIZE);           
         
            polygon0 = new PolygonShape();            
            polygon1 = new PolygonShape();
            polygon2 = new PolygonShape();
            polygon3 = new PolygonShape();

            polygon0.SetBox(20, 20);
            polygon1.SetBox(20, 20);
            polygon2.SetBox(20, 20);
            polygon3.SetBox(20, 20);

            b1 = new Body(polygon0, new Vector2(700, 500));            
            b3 = new Body(polygon1, new Vector2(1200, 500));
            b2 = new Body(polygon2, new Vector2(960, 500));
            b4 = new Body(polygon3, new Vector2(1800, 580));
            b1.velocity = new Vector2(0.0f, 0);            
            b3.velocity = new Vector2(0.0f, 0);
            b2.velocity = new Vector2(0, 0);
            b4.velocity = Vector2.Zero;
            
           
            b1.SetOrientation(0.3f);
            b3.SetOrientation(0.2f);
            b2.SetOrientation(-0.1f);
            b4.SetOrientation(-0.1f);
            b1.SetStatic();
            b3.SetStatic();
            b2.SetStatic();
            b4.SetStatic();

            scene.AddBody(b1);            
            scene.AddBody(b3);
            scene.AddBody(b2);
            scene.AddBody(b4);

            Circle circle = new Circle(2);
            PolygonShape box = new PolygonShape();
            box.SetBox(10, 10);
            emitter = new Emitter(scene, circle, Vector2.Zero, 200.0f, new TimeSpan(0, 0, 3));
            emitter2 = new Emitter(scene, box, new Vector2(1200,100), 20.0f, new TimeSpan(0, 0, 3));


            Content.RootDirectory = "Content";
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

            _primitiveBatch = new PrimitiveBatch(GraphicsDevice);
            _primitiveDrawing = new PrimitiveDrawing(_primitiveBatch);
            _localProjection = Matrix.CreateOrthographicOffCenter(0f, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
            _localView = Matrix.Identity;
            blank = Content.Load<Texture2D>("blank");            
            font = Content.Load<SpriteFont>("Arcade");
        }

        protected override void Update(GameTime gameTime)
        {
            framesPerSecondCounter.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            steptimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (steptimer < 0)
            {
                scene.Step(0.96f);// gameTime.ElapsedGameTime.TotalSeconds * 60.0d);
                steptimer += 16;                
            }
            //scene.Step(gameTime.ElapsedGameTime.TotalSeconds * 60.0d);

            b4.SetOrientation(b4.orient +(float)(0.002f* gameTime.ElapsedGameTime.TotalMilliseconds));
            b4.position.X -= (float)(0.03f * gameTime.ElapsedGameTime.TotalMilliseconds);


            /*timer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer < 0)
            {
                
                MouseState state = Mouse.GetState();
                if (state.LeftButton == ButtonState.Pressed)
                {
                    //for (int i = 0; i < 20; i++)
                    {
                        Vector2 pos2 = new Vector2(state.X, state.Y);
                        Circle circle2 = new Circle(2);
                        Body body2 = new Body(circle2, pos2);
                        body2.velocity = new Vector2((float)rand.NextDouble() * 8 - 4.0f, (float)rand.NextDouble() * 4);                        
                        body2.IsParticle = true;
                        scene.AddBody(body2);
                        bodies.Add(body2);
                    }
                }
                
                timer += 2;
                
            }


            List<Body> remove = bodies.FindAll(p => p.position.Y > 1080);           
            bodies.RemoveAll(p=>p.position.Y>1080);
            foreach(Body b in remove) scene.RemoveBody(b);*/

            MouseState state = Mouse.GetState();
            Vector2 pos2 = new Vector2(state.X, state.Y);
            emitter.position = pos2;
            emitter.Update(gameTime.ElapsedGameTime.TotalSeconds);
            emitter2.Update(gameTime.ElapsedGameTime.TotalSeconds);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
           
            framesPerSecondCounter.Draw(gameTime);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            Color dark = new Color(0.1f, 0.1f, 0.1f);

            for(int x = 0; x < 1920; x += CELLSIZE)
            {
                _spriteBatch.Draw(blank, new Rectangle(new Point(x, 540), new Point(1, 1080)), new Rectangle(0, 0, 4, 4),  dark, 0, new Vector2(2, 2), SpriteEffects.None, 0);
            }
            for (int y = 0; y < 1080; y += CELLSIZE)
            {
                _spriteBatch.Draw(blank, new Rectangle(new Point(960, y), new Point(1920,1)), new Rectangle(0, 0, 4, 4), dark, 0, new Vector2(2, 2), SpriteEffects.None, 0);
            }



            _spriteBatch.DrawString(font, framesPerSecondCounter.FramesPerSecond + " FPS", new Vector2(0, 16), Color.White * 0.8f);
            _spriteBatch.DrawString(font, emitter.particles.Count + " Particles", new Vector2(0, 32), Color.White * 0.8f);

            foreach (Body b in emitter.particles)
            {
                _spriteBatch.Draw(blank, new Rectangle(new Point((int)b.position.X, (int)b.position.Y), new Point(4,4)), new Rectangle(0, 0, 4, 4), scene.IsBodySharingAnyCell(b) ? Color.White : Color.White, b.orient, new Vector2(2, 2), SpriteEffects.None, 0);
            }

            foreach (Body b in emitter2.particles)
            {
                _spriteBatch.Draw(blank, new Rectangle(new Point((int)b.position.X, (int)b.position.Y), new Point(20, 20)), new Rectangle(0, 0, 4, 4), Color.Yellow, b.orient, new Vector2(2, 2), SpriteEffects.None, 0);
            }

            _spriteBatch.Draw(blank, new Rectangle(new Point((int)b1.position.X, (int)b1.position.Y), new Point(40, 40)), new Rectangle(0, 0, 4, 4), scene.IsBodySharingAnyCell(b1) ? Color.Red:Color.LightGray, b1.orient, new Vector2(2, 2), SpriteEffects.None, 0) ;
            _spriteBatch.Draw(blank, new Rectangle(new Point((int)b3.position.X, (int)b3.position.Y), new Point(40,40)), new Rectangle(0, 0, 4,4), scene.IsBodySharingAnyCell(b3) ? Color.Red : Color.LightGray, b3.orient, new Vector2(2,2), SpriteEffects.None, 0);
            _spriteBatch.Draw(blank, new Rectangle(new Point((int)b2.position.X, (int)b2.position.Y), new Point(40, 40)), new Rectangle(0, 0, 4,4), scene.IsBodySharingAnyCell(b2) ? Color.Red : Color.LightGray, b2.orient, new Vector2(2, 2), SpriteEffects.None, 0);         
            _spriteBatch.Draw(blank, new Rectangle(new Point((int)b4.position.X, (int)b4.position.Y), new Point(40, 40)), new Rectangle(0, 0, 4, 4), scene.IsBodySharingAnyCell(b4) ? Color.Red : Color.LightGray, b4.orient, new Vector2(2, 2), SpriteEffects.None, 0);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
