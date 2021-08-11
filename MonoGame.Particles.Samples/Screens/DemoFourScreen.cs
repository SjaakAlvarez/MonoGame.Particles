using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using MonoGame.ImGui;
using MonoGame.Particles.Particles;
using MonoGame.Particles.Particles.Modifiers;
using MonoGame.Particles.Physics;
using MonoGame.Particles.Physics.Debug;
using MonoGame.Particles.Samples.GameStateManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Samples.Screens
{
    public class DemoFourScreen : GameScreen
    {
        private World world;
        double steptimer = 16;
        const int CELLSIZE = 64;
        private Color WATERCOLOR = Color.DodgerBlue;
        private ContentManager Content;
        Body b1;
        
        public float ballsPerSecond = 1.0f;

        private AlphaTestEffect alphaTest;

        private PhysicsEmitter emitter;

        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private bool colliding;
        private readonly Tweener tweener = new Tweener();

        private DrawWorld drawWorld;
        
        Texture2D blank;
        Texture2D fadedcircle;
        
        Body floor;
        private RenderTarget2D particlesTarget;

        public float orientation=0f;
        public ImGUIRenderer GuiRenderer;

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
            //b1.SetStatic();            

            world.AddBody(b1);

            


            PolygonShape rect = new PolygonShape();
            rect.SetBox(500, 10);
            floor = new Body(rect, new Vector2(960, 800));
            floor.SetStatic();
            floor.Velocity = Vector2.Zero;
            floor.SetOrientation(0);
            world.AddBody(floor);

            
        }

        private ContactAction Emitter_OnCollision(Body sender, Body other, Contact m)
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
            PhysicsEmitter emitter2 = new PhysicsEmitter("Water", world, circle, Vector2.Zero, new Interval(50, 250), new Interval(-Math.PI, Math.PI), 5, new Interval(500, 1000));
            emitter2.Modifiers.Add(new ColorRangeModifier(WATERCOLOR, WATERCOLOR));
            emitter2.Position = pos2;
            emitter2.ParticlesPerSecond = 150;
            emitter2.IgnoreGravity = false;
            emitter2.LinearDamping = 0.01f;
            emitter2.Texture = fadedcircle;
           

            tweener.TweenTo(target: emitter2, expression: p => p.ParticlesPerSecond, toValue: 0, duration: 0.25f)
            .Easing(EasingFunctions.ExponentialOut)
            .OnEnd(p =>
            {
                PhysicsEmitter physicsEmitter = (PhysicsEmitter)p.Target;
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
            font = Content.Load<SpriteFont>("Debug");

            _spriteBatch = ScreenManager.SpriteBatch;

            Matrix _localProjection = Matrix.CreateOrthographicOffCenter(0f, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
            Matrix _localView = Matrix.Identity;

            drawWorld = new DrawWorld(world, ScreenManager.Game, _localProjection, _localView);
            drawWorld.DrawAABB = false;
            drawWorld.DrawShapes = false;
            drawWorld.DrawInfo = true;

            particlesTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);


            Circle circle = new Circle(14);

            emitter = new PhysicsEmitter("Water", world, circle, new Vector2(960, 100), new Interval(100, 110), new Interval(1.2f,1.8f), 250, new Interval(3000, 5000));
            emitter.Modifiers.Add(new ColorRangeModifier(WATERCOLOR, WATERCOLOR));
            emitter.OnCollision += Emitter_OnCollision;            
            emitter.Texture = fadedcircle;
            emitter.Start();

            alphaTest = new AlphaTestEffect(ScreenManager.GraphicsDevice);
            alphaTest.ReferenceAlpha = 200;
            alphaTest.Projection = Matrix.CreateTranslation(-0.5f, -0.5f, 0) *
                    Matrix.CreateOrthographicOffCenter(0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, 0, 0, 1);

            //tweener.TweenTo(this, p => p.orientation, 6.28f, 4).Easing(EasingFunctions.Linear).RepeatForever();

            GuiRenderer = new ImGUIRenderer(ScreenManager.Game).Initialize().RebuildFontAtlas();

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

            //b4.SetOrientation(b4.Orientation + (float)(0.002f * gameTime.ElapsedGameTime.TotalMilliseconds));
            //b1.SetOrientation(orientation);
           
            tweener.Update(gameTime.GetElapsedSeconds());
           
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.SetRenderTarget(particlesTarget);
            ScreenManager.GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin(0, BlendState.Additive);
            foreach (PhysicsEmitter e in world.emitters.FindAll(p => p.Name.Equals("Water")))
            {
                e.Draw(_spriteBatch);
                /* foreach (IParticle b in e.particles)
                 {
                     _spriteBatch.Draw(blank, new Rectangle(new Point((int)b.Position.X, (int)b.Position.Y), new Point(8, 8)), new Rectangle(0, 0, 4, 4), b.Color * b.Alpha, b.Orientation, new Vector2(2, 2), SpriteEffects.None, 0);
                 }*/
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
            
            base.Draw(gameTime);

            GuiRenderer.BeginLayout(gameTime);

            ImGuiNET.ImGui.Begin("Water Demo");

            ImGuiNET.ImGui.SliderFloat("Particles/second", ref emitter.ParticlesPerSecond, 0.1f, 400.0f);

            ImGuiNET.ImGui.CollapsingHeader("Debug");
            ImGuiNET.ImGui.Checkbox("Show shapes", ref drawWorld.DrawShapes);
            ImGuiNET.ImGui.Checkbox("Show AABB", ref drawWorld.DrawAABB);
            ImGuiNET.ImGui.Checkbox("Show info", ref drawWorld.DrawInfo);

          

            ImGuiNET.ImGui.End();


            GuiRenderer.EndLayout();

        }


    }
}
