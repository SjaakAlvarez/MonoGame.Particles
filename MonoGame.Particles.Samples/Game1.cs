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
using MonoGame.Particles.Samples.Properties;
using System;
using System.Collections.Generic;

namespace MonoGame.Particles.Samples
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        World world;
        Body b1;
        Body b3;
        Body b2;
        Body b4;

        Texture2D blank;
        Texture2D fadedcircle;
        Texture2D nuke;
        SpriteFont font;
        double steptimer = 16;
        const int CELLSIZE = 16;
        bool boom;

        private DrawWorld drawWorld;
        private Emitter emitter;
        private Emitter emitter2;
        private Emitter emitter3;
        bool colliding;

        private FramesPerSecondCounterComponent framesPerSecondCounter;
        private Tweener tweener=new Tweener();
        

        private List<Emitter> explosions = new List<Emitter>();
        private List<Emitter> debris = new List<Emitter>();
        private List<Emitter> boxes = new List<Emitter>();

        private List<Tweener> tweeners = new List<Tweener>();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.SynchronizeWithVerticalRetrace = false;
            //_graphics.HardwareModeSwitch = false;
            this.IsFixedTimeStep = false;
            Window.IsBorderless = true;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            world = new World(1, new Vector2(1920, 1080), CELLSIZE);

            PolygonShape polygon0 = new PolygonShape();
            PolygonShape polygon1 = new PolygonShape();
            PolygonShape polygon2 = new PolygonShape();
            PolygonShape polygon3 = new PolygonShape();

            polygon0.SetBox(20, 20);
            polygon1.SetBox(20, 20);
            polygon2.SetBox(20, 20);
            polygon3.SetBox(20, 20);

            b1 = new Body(polygon0, new Vector2(700, 500));
            b2 = new Body(polygon2, new Vector2(960, 500));
            b3 = new Body(polygon1, new Vector2(1200, 500));
            b4 = new Body(polygon3, new Vector2(1100, 580));

            b1.velocity = Vector2.Zero;
            b2.velocity = Vector2.Zero;
            b3.velocity = Vector2.Zero;
            b4.velocity = Vector2.Zero;

            b1.SetOrientation(0.3f);
            b2.SetOrientation(-0.1f);
            b3.SetOrientation(0.2f);
            b4.SetOrientation(-0.1f);

            b1.SetStatic();
            b2.SetStatic();
            b3.SetStatic();
            b4.SetStatic();

            world.AddBody(b1);
            world.AddBody(b2);
            world.AddBody(b3);
            world.AddBody(b4);

            PolygonShape rect = new PolygonShape();
            rect.SetBox(500, 10);
            Body b5 = new Body(rect, new Vector2(960, 800));
            b5.SetStatic();
            b5.velocity = Vector2.Zero;
            b5.SetOrientation(0);
            world.AddBody(b5);

            rect = new PolygonShape();
            rect.SetBox(40, 40);
            b5 = new Body(rect, new Vector2(800, 750));
            b5.SetStatic();
            b5.velocity = Vector2.Zero;
            b5.SetOrientation(0);
            world.AddBody(b5);


            b4.OnCollision += B4_OnCollision;
            b4.OnSeparation += B4_OnSeparation;


            PolygonShape box = new PolygonShape();
            box.SetBox(10, 10);


            emitter2 = new Emitter(world, box, new Vector2(960, 100), new Interval(1, 4), 2.0f, new Interval(3000, 4000));
            emitter2.Modifiers.Add(new ColorRangeModifier(Color.LightBlue, Color.Purple));            
            emitter2.OnCollision += Emitter2_OnCollision;
            emitter2.Start();
            boxes.Add(emitter2);



            //Load content from a resource file
            Content.RootDirectory = "Content";
            //ResourceContentManager resourceContentManager = new ResourceContentManager(this.Services, Resources.ResourceManager);
            //Content = resourceContentManager;

            framesPerSecondCounter = new FramesPerSecondCounterComponent(this);

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        private bool Emitter2_OnCollision(Body sender, Body other, Contact m)
        {
            Explode(((Particle)sender).position);
            //delete the particle
            return false;
        }

        

        private void Explode(Vector2 pos)
        {
            Vector2 pos2 = new Vector2(pos.X, pos.Y);

            Circle circle = new Circle(8);
            PolygonShape box2 = new PolygonShape();
            box2.SetBox(4, 4);

            emitter = new Emitter(world, circle, Vector2.Zero, new Interval(0, 10), 10, new Interval(500, 1000));
            emitter.Modifiers.Add(new ColorRangeModifier(Color.Orange, Color.Black));
            emitter.position = pos2;
            emitter.ParticlesPerSecond = 2000;
            emitter.IgnoreGravity = true;
            emitter.LinearDamping = 0.08f;
            emitter.Texture = fadedcircle;

            emitter3 = new Emitter(world, box2, new Vector2(1200, 100), new Interval(0, 10), 20.0f, new Interval(1000, 2500));
            emitter3.Modifiers.Add(new AlphaFadeModifier());
            emitter3.position = pos2;
            emitter3.ParticlesPerSecond = 1000;
            emitter3.LinearDamping = 0.02f;                      

            tweener.TweenTo(target: emitter, expression: p => p.ParticlesPerSecond, toValue: 0, duration: 0.25f)
            .Easing(EasingFunctions.ExponentialOut)
            .OnEnd(p =>
            {                
                emitter.Stop();
            });

            tweener.TweenTo(target: emitter3, expression: p => p.ParticlesPerSecond, toValue: 0, duration: 0.25f)
            .Easing(EasingFunctions.ExponentialOut)
            .OnEnd(p =>
            {
                emitter3.Stop();
            });

            explosions.Add(emitter);
            debris.Add(emitter3);
            emitter3.Start();
            emitter.Start();
        }

        private void B4_OnSeparation(Body sender, Body other)
        {
            colliding = false;
        }

        private bool B4_OnCollision(Body sender, Body other, Contact m)
        {
            colliding = true;
            return true;
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Matrix _localProjection = Matrix.CreateOrthographicOffCenter(0f, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
            Matrix _localView = Matrix.Identity;

            font = Content.Load<SpriteFont>("Arcade");
            blank = Content.Load<Texture2D>("blank");
            fadedcircle = Content.Load<Texture2D>("fadedcircle");
            nuke = Content.Load<Texture2D>("nuke");

            drawWorld = new DrawWorld(world, this.GraphicsDevice, _localProjection, _localView);
        }

        protected override void Update(GameTime gameTime)
        {
            framesPerSecondCounter.Update(gameTime);



            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            steptimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (steptimer < 0)
            {
                world.Step(0.96f);
                steptimer += 16;
            }



            b4.SetOrientation(b4.orientation + (float)(0.002f * gameTime.ElapsedGameTime.TotalMilliseconds));
            //b4.position.X -= (float)(0.03f * gameTime.ElapsedGameTime.TotalMilliseconds);

            MouseState state = Mouse.GetState();
            if (state.LeftButton == ButtonState.Pressed && !boom)
            {
                //TODO
            }

            //foreach (Tweener t in tweeners) t.Update(gameTime.GetElapsedSeconds());
            foreach (Emitter e in explosions) e.Update(gameTime.GetElapsedSeconds());
            foreach (Emitter e in debris) e.Update(gameTime.GetElapsedSeconds());
            foreach (Emitter e in boxes) e.Update(gameTime.GetElapsedSeconds());

            tweener.Update(gameTime.GetElapsedSeconds());
           
            debris.RemoveAll(p => p.CanDestroy());
            explosions.RemoveAll(p => p.CanDestroy());            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            framesPerSecondCounter.Draw(gameTime);
            GraphicsDevice.Clear(Color.Black);

            drawWorld.DrawSpatialGrid();

            _spriteBatch.Begin();

            _spriteBatch.DrawString(font, framesPerSecondCounter.FramesPerSecond + " FPS", new Vector2(0, 16), Color.White * 0.8f);
            _spriteBatch.DrawString(font, world.bodies.Count + " Particles", new Vector2(0, 32), Color.White * 0.8f);

            foreach (Emitter e in boxes)
            {
                foreach (Particle b in e.particles)
                {
                    _spriteBatch.Draw(blank, new Rectangle(new Point((int)b.position.X, (int)b.position.Y), new Point(20, 20)), new Rectangle(0, 0, 4, 4), b.Color * b.Alpha, b.orientation, new Vector2(2, 2), SpriteEffects.None, 0);
                }
            }

            _spriteBatch.Draw(blank, new Rectangle(new Point((int)b1.position.X, (int)b1.position.Y), new Point(40, 40)), new Rectangle(0, 0, 4, 4), world.IsBodySharingAnyCell(b1) ? Color.Red : Color.Green, b1.orientation, new Vector2(2, 2), SpriteEffects.None, 0);
            _spriteBatch.Draw(blank, new Rectangle(new Point((int)b3.position.X, (int)b3.position.Y), new Point(40, 40)), new Rectangle(0, 0, 4, 4), world.IsBodySharingAnyCell(b3) ? Color.Red : Color.Green, b3.orientation, new Vector2(2, 2), SpriteEffects.None, 0);
            _spriteBatch.Draw(blank, new Rectangle(new Point((int)b2.position.X, (int)b2.position.Y), new Point(40, 40)), new Rectangle(0, 0, 4, 4), world.IsBodySharingAnyCell(b2) ? Color.Red : Color.Green, b2.orientation, new Vector2(2, 2), SpriteEffects.None, 0);
            _spriteBatch.Draw(blank, new Rectangle(new Point((int)b4.position.X, (int)b4.position.Y), new Point(40, 40)), new Rectangle(0, 0, 4, 4), colliding ? Color.Red : Color.Green, b4.orientation, new Vector2(2, 2), SpriteEffects.None, 0);
            _spriteBatch.Draw(blank, new Rectangle(new Point(960, 800), new Point(1000, 20)), new Rectangle(0, 0, 4, 4), Color.Green, 0, new Vector2(2, 2), SpriteEffects.None, 0);

            _spriteBatch.Draw(blank, new Rectangle(new Point(800, 750), new Point(80, 80)), new Rectangle(0, 0, 4, 4), Color.SandyBrown, 0, new Vector2(2, 2), SpriteEffects.None, 0);


            foreach (Emitter e in debris)
            {
                foreach (Particle b in e.particles)
                {
                    _spriteBatch.Draw(blank, new Rectangle(new Point((int)b.position.X, (int)b.position.Y), new Point(8, 8)), new Rectangle(0, 0, 4, 4), b.Color * b.Alpha, b.orientation, new Vector2(2, 2), SpriteEffects.None, 0);
                }
            }

            _spriteBatch.End();

            _spriteBatch.Begin(0, BlendState.Additive);

            foreach (Emitter e in explosions)
            {                
                e.Draw(_spriteBatch);                
            }

            _spriteBatch.End();


            //drawWorld.Draw();

            base.Draw(gameTime);
        }
    }
}
