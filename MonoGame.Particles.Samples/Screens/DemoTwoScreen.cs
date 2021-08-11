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
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Samples.Screens
{
    public class DemoTwoScreen : GameScreen
    {
        private World world;
        double steptimer = 16;
        const int CELLSIZE = 16;
        private ContentManager Content;
        Body b1;
        Body b3;
        Body b2;
        Body b4;
        Body b6;

        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private bool colliding;
        private readonly Tweener tweener = new Tweener();

        private Random random = new Random();

        private DrawWorld drawWorld;
        private List<Emitter> emitters = new List<Emitter>();

        Texture2D blank;
        Texture2D fadedcircle;

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
            fadedcircle = Content.Load<Texture2D>("fadedcircle");
            font = Content.Load<SpriteFont>("Debug");

            _spriteBatch = ScreenManager.SpriteBatch;

            Matrix _localProjection = Matrix.CreateOrthographicOffCenter(0f, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
            Matrix _localView = Matrix.Identity;

            drawWorld = new DrawWorld(world, ScreenManager.Game, _localProjection, _localView);
            drawWorld.DrawAABB = false;
            drawWorld.DrawShapes = false;
            drawWorld.DrawInfo = true;

            Emitter em = new Emitter("Boxes", new Vector2(960, 900), new Interval(800, 900), new Interval(-1.2f, -1.9f), 0.5f, new Interval(2500, 2800));
            em.Modifiers.Add(new ColorRangeModifier(Color.White, Color.Black));
            em.LinearDamping = 0.5f;            
            em.Texture = blank;
            em.Start();
            emitters.Add(em);
            em.ParticleDeath += Em_ParticleDeath;           

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
          
            foreach (Emitter e in emitters.ToArray()) e.Update(gameTime.GetElapsedSeconds());

            tweener.Update(gameTime.GetElapsedSeconds());

            emitters.RemoveAll(p => p.CanDestroy());

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            //drawWorld.DrawSpatialGrid();

            int total = 0;
            foreach (Emitter e in emitters) total += e.particles.Count;

            _spriteBatch.DrawString(font, "Emitters:  " + emitters.Count, new Vector2(0, 112), Color.Yellow * 0.8f);
            _spriteBatch.DrawString(font, "Particles: " + total, new Vector2(0, 128), Color.Yellow * 0.8f);

            foreach (BaseEmitter e in emitters.FindAll(p => p.Name.Equals("Boxes")))
            {
                e.Draw(_spriteBatch);               
            }
            
            _spriteBatch.End();


            _spriteBatch.Begin(0, BlendState.Additive);
          
            foreach (Emitter e in emitters.FindAll(p => p.Name.StartsWith("Explosion")))
            {
                e.Draw(_spriteBatch);
            }



            _spriteBatch.End();

            drawWorld.Draw();
            
            base.Draw(gameTime);
        }

       

        private void Explode2(Vector2 pos2)
        {
            Emitter emitter = new Emitter("Explosion", Vector2.Zero, new Interval(100, 350), new Interval(-Math.PI, Math.PI), 10, new Interval(800, 1500));

            Color color = colors[random.Next(colors.Length)];

            emitter.Modifiers.Add(new ColorRangeModifier(color, color));
            emitter.Modifiers.Add(new AlphaFadeModifier());
            emitter.Position = pos2;
            emitter.ParticlesPerSecond = 8000;
            emitter.IgnoreGravity = false;
            emitter.LinearDamping = 0.01f;
            emitter.Texture = blank;

            emitter.ParticleDeath += Emitter_ParticleDeath;

            emitters.Add(emitter);           

            tweener.TweenTo(target: emitter, expression: p => p.ParticlesPerSecond, toValue: 0, duration: 0.35f)
            .Easing(EasingFunctions.ExponentialOut)
            .OnEnd(p =>
            {
                Emitter physicsEmitter = (Emitter)p.Target;
                physicsEmitter.Stop();
            });
            
            emitter.Start();
        }

        private void Emitter_ParticleDeath(object sender, ParticleEventArgs e)
        {
            Glitter(e.Particle.Position);
        }

        private Emitter Trail(Vector2 pos2)
        {
            Emitter emitter = new Emitter("Trail", Vector2.Zero, new Interval(0, 0), new Interval(1.5, 1.6), 20, new Interval(300, 500));


            emitter.Modifiers.Add(new ColorRangeModifier(Color.Orange, Color.Black));
            //emitter.Modifiers.Add(new AlphaFadeModifier());
            emitter.Position = pos2;                                
            emitter.Texture = blank;

            emitters.Add(emitter);

            tweener.TweenTo(target: emitter, expression: p => p.ParticlesPerSecond, toValue: 0, duration: 2.5f)
            .Easing(EasingFunctions.ExponentialInOut)
            .OnEnd(p =>
            {
                Emitter physicsEmitter = (Emitter)p.Target;
                physicsEmitter.Stop();
            });

            emitter.Start();
            return emitter;
        }

        private void Glitter(Vector2 pos2)
        {
            Emitter emitter = new Emitter("Explosion", Vector2.Zero, new Interval(100, 250), new Interval(-Math.PI, Math.PI), 10, new Interval(100, 500));

            
            emitter.Modifiers.Add(new ColorRangeModifier(Color.LightBlue, Color.White));
            emitter.Modifiers.Add(new AlphaFadeModifier());
            emitter.Position = pos2;
            emitter.ParticlesPerSecond = 3000;
            emitter.IgnoreGravity = true;
            emitter.LinearDamping = 0.01f;
            emitter.Texture = blank;

            emitters.Add(emitter);

            tweener.TweenTo(target: emitter, expression: p => p.ParticlesPerSecond, toValue: 0, duration: 0.25f)
            .Easing(EasingFunctions.ExponentialOut)
            .OnEnd(p =>
            {
                Emitter physicsEmitter = (Emitter)p.Target;
                physicsEmitter.Stop();
            });

            emitter.Start();          
        }

    }
}
