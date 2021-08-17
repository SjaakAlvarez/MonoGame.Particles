using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using MonoGame.Particles.Particles;
using MonoGame.Particles.Particles.Modifiers;
using MonoGame.Particles.Particles.Origins;
using MonoGame.Particles.Physics;
using MonoGame.Particles.Samples.GameStateManagement;
using System;

namespace MonoGame.Particles.Samples.Screens
{
    public class DemoTwoScreen : DemoScreen
    {
        const int CELLSIZE = 16;
        
        private double steptimer = 16;        
        private ContentManager Content;
        private SpriteBatch _spriteBatch;
        private readonly Tweener tweener = new Tweener();
        private Random random = new Random();               
        private Texture2D blank;        

        private Color[] colors = new Color[] { Color.Red,Color.White, Color.Blue, Color.Orange, Color.Green, Color.Purple};

        public DemoTwoScreen()
        {
            VectorMath.gravity = new Vector2(0, 200);
            world = new World(1, new Vector2(1920, 1080), CELLSIZE);                        
        }

        private void Em_ParticleDeath(object sender, ParticleEventArgs e)
        {
            Explode2(e.Particle.Position);
        }
       

        public override void Activate(bool instancePreserved)
        {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");

            blank = Content.Load<Texture2D>("blank");
            
            _spriteBatch = ScreenManager.SpriteBatch;            

            ParticleEmitter em = new ParticleEmitter("Boxes", world, new Vector2(960, 900), new Interval(800, 900), new Interval(-1.2f, -1.9f), 0.5f, new Interval(2500, 2800));
            em.AddModifier(new ColorRangeModifier(Color.White, Color.Black));
            em.Origin = new PointOrigin();
            em.LinearDamping = 0.5f;            
            em.Texture = blank;
            em.Start();            
            em.ParticleDeath += Em_ParticleDeath;
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

            _spriteBatch.Begin();
            foreach (Emitter e in world.emitters.FindAll(p => p.Name.Equals("Boxes")))
            {
                e.Draw(_spriteBatch);               
            }            
            _spriteBatch.End();

            _spriteBatch.Begin(0, BlendState.Additive);          
            foreach (ParticleEmitter e in world.emitters.FindAll(p => p.Name.StartsWith("Explosion")))
            {
                e.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            drawWorld.Draw();

            GuiRenderer.BeginLayout(gameTime);
            DebugWindow();
            GuiRenderer.EndLayout();

        }

       

        private void Explode2(Vector2 pos2)
        {
            ParticleEmitter emitter = new ParticleEmitter("Explosion", world, Vector2.Zero, new Interval(100, 350), new Interval(-Math.PI, Math.PI), 10, new Interval(800, 1500));

            Color color = colors[random.Next(colors.Length)];

            emitter.AddModifier(new ColorRangeModifier(color, color));
            emitter.AddModifier(new AlphaFadeModifier());
            emitter.Origin = new PointOrigin();
            emitter.Position = pos2;
            emitter.ParticlesPerSecond = 8000;
            emitter.IgnoreGravity = false;
            emitter.LinearDamping = 0.01f;
            emitter.Texture = blank;
            emitter.ParticleDeath += Emitter_ParticleDeath;

            tweener.TweenTo(target: emitter, expression: p => p.ParticlesPerSecond, toValue: 0, duration: 0.35f)
            .Easing(EasingFunctions.ExponentialOut)
            .OnEnd(p =>
            {
                ParticleEmitter physicsEmitter = (ParticleEmitter)p.Target;
                physicsEmitter.Stop();
            });
            
            emitter.Start();
        }

        private void Emitter_ParticleDeath(object sender, ParticleEventArgs e)
        {
            Glitter(e.Particle.Position);
        }

        private void Glitter(Vector2 pos2)
        {
            ParticleEmitter emitter = new ParticleEmitter("Explosion", world, Vector2.Zero, new Interval(100, 250), new Interval(-Math.PI, Math.PI), 10, new Interval(100, 500));

            
            emitter.AddModifier(new ColorRangeModifier(Color.LightBlue, Color.White));
            emitter.AddModifier(new AlphaFadeModifier());
            emitter.Origin = new PointOrigin();
            emitter.Position = pos2;
            emitter.ParticlesPerSecond = 3000;
            emitter.IgnoreGravity = true;
            emitter.LinearDamping = 0.01f;
            emitter.Texture = blank;

            tweener.TweenTo(target: emitter, expression: p => p.ParticlesPerSecond, toValue: 0, duration: 0.25f)
            .Easing(EasingFunctions.ExponentialOut)
            .OnEnd(p =>
            {
                ParticleEmitter physicsEmitter = (ParticleEmitter)p.Target;
                physicsEmitter.Stop();
            });

            emitter.Start();          
        }

    }
}
