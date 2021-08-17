using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using MonoGame.ImGui;
using MonoGame.Particles.Particles;
using MonoGame.Particles.Particles.Modifiers;
using MonoGame.Particles.Particles.Origins;
using MonoGame.Particles.Physics;
using MonoGame.Particles.Physics.Debug;
using MonoGame.Particles.Samples.GameStateManagement;
using System;

namespace MonoGame.Particles.Samples.Screens
{
    public class DemoFourScreen : DemoScreen
    {        
        double steptimer = 16;
        const int CELLSIZE = 64;
        private Color WATERCOLOR = Color.DodgerBlue;
        private ContentManager Content;
        private Body b1;             
        private AlphaTestEffect alphaTest;
        private PhysicsParticleEmitter emitter;
        private SpriteBatch _spriteBatch;
        private readonly Tweener tweener = new Tweener();             
        private Texture2D blank;
        private Texture2D fadedcircle;        
        private Body floor;
        private RenderTarget2D particlesTarget;
        private float orientation=0f;        

        public DemoFourScreen()
        {
            VectorMath.gravity = new Vector2(0, 500);
            world = new World(1, new Vector2(1920, 1080), CELLSIZE);

            PolygonShape polygon0 = new PolygonShape();
            
            polygon0.SetBox(100, 20);
           

            b1 = new Body(polygon0, new Vector2(900, 500));           
            b1.Velocity = Vector2.Zero;           
            b1.SetOrientation(orientation);       
            b1.Restitution=0;
            b1.LinearDamping = 0.1f;
            b1.IgnoreGravity = true;
            b1.FixedPosition = true;            
            world.AddBody(b1);
         
            PolygonShape rect = new PolygonShape();
            rect.SetBox(500, 10);
            floor = new Body(rect, new Vector2(960, 800));
            floor.SetStatic();
            floor.Velocity = Vector2.Zero;
            floor.SetOrientation(0);
            world.AddBody(floor);            
        }

        private ContactAction Emitter_OnCollision(PhysicsParticle sender, Body other, Contact m)
        {
            if (other == floor)
            {
                Splash(sender.Position);
                return ContactAction.DESTROY;
            }
            else
                return ContactAction.COLLIDE;
        }

        private void Splash(Vector2 pos2)
        {
            Circle circle = new Circle(14);
            PhysicsParticleEmitter emitter2 = new PhysicsParticleEmitter("Water", world, circle, Vector2.Zero, new Interval(50, 250), new Interval(-Math.PI, Math.PI), 5, new Interval(500, 1000));
            emitter2.AddModifier(new ColorRangeModifier(WATERCOLOR, WATERCOLOR));
            emitter2.Origin = new PointOrigin();
            emitter2.Position = pos2;
            emitter2.ParticlesPerSecond = 150;
            emitter2.IgnoreGravity = false;
            emitter2.LinearDamping = 0.01f;
            emitter2.Texture = fadedcircle;
           
            tweener.TweenTo(target: emitter2, expression: p => p.ParticlesPerSecond, toValue: 0, duration: 0.25f)
            .Easing(EasingFunctions.ExponentialOut)
            .OnEnd(p =>
            {
                PhysicsParticleEmitter physicsEmitter = (PhysicsParticleEmitter)p.Target;
                physicsEmitter.Stop();
            });
           
            
            emitter2.Start();
        }

        public override void Activate(bool instancePreserved)
        {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");

            blank = Content.Load<Texture2D>("blank");
            fadedcircle = Content.Load<Texture2D>("fadedcircle");
           
            _spriteBatch = ScreenManager.SpriteBatch;

            particlesTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);


            Circle circle = new Circle(14);

            emitter = new PhysicsParticleEmitter("Water", world, circle, new Vector2(960, 100), new Interval(100, 110), new Interval(1.2f,1.8f), 250, new Interval(3000, 5000));
            emitter.AddModifier(new ColorRangeModifier(WATERCOLOR, WATERCOLOR));
            emitter.Origin = new CircleOrigin(20);
            emitter.OnCollision += Emitter_OnCollision;            
            emitter.Texture = fadedcircle;
            emitter.Start();

            alphaTest = new AlphaTestEffect(ScreenManager.GraphicsDevice);
            alphaTest.ReferenceAlpha = 200;
            alphaTest.Projection = Matrix.CreateTranslation(-0.5f, -0.5f, 0) *
                    Matrix.CreateOrthographicOffCenter(0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, 0, 0, 1);

            base.Activate(instancePreserved);            
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {                
                ScreenManager.RemoveScreen(this);
            }
                
            base.HandleInput(gameTime, input);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            steptimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (steptimer < 0)
            {
                world.Step(0.016d);
                steptimer += 16;
            }
           
            tweener.Update(gameTime.GetElapsedSeconds());
           
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            ScreenManager.GraphicsDevice.SetRenderTarget(particlesTarget);
            ScreenManager.GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin(0, BlendState.Additive);
            foreach (PhysicsParticleEmitter e in world.physicsEmitters.FindAll(p => p.Name.Equals("Water")))
            {
                e.Draw(_spriteBatch);                
            }
            _spriteBatch.End();
            ScreenManager.GraphicsDevice.SetRenderTarget(null);

            drawWorld.DrawSpatialGrid();
           
            _spriteBatch.Begin(0,effect:alphaTest);
            _spriteBatch.Draw(particlesTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();

            _spriteBatch.Begin();
            _spriteBatch.Draw(blank, new Rectangle(new Point((int)b1.Position.X, (int)b1.Position.Y), new Point(200, 40)), new Rectangle(0, 0, 4, 4), Color.Green, b1.Orientation, new Vector2(2, 2), SpriteEffects.None, 0);
            _spriteBatch.Draw(blank, new Rectangle(new Point(960, 800), new Point(1000, 20)), new Rectangle(0, 0, 4, 4), Color.Green, floor.Orientation, new Vector2(2, 2), SpriteEffects.None, 0);
            _spriteBatch.End();


            drawWorld.Draw();
            
          

            GuiRenderer.BeginLayout(gameTime);

            ImGuiNET.ImGui.Begin("Water Demo");

            ImGuiNET.ImGui.SliderFloat("Particles/second", ref emitter.ParticlesPerSecond, 0.1f, 400.0f);

            
            ImGuiNET.ImGui.End();

            DebugWindow();

            GuiRenderer.EndLayout();

        }


    }
}
