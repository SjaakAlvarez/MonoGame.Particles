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
using MonoGame.Particles.Physics.Debug;
using MonoGame.Particles.Samples.GameStateManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Samples.Screens
{
    public class DemoOneScreen : DemoScreen
    {        
        private double steptimer = 16;
        const int CELLSIZE = 16;
        private ContentManager Content;
        private Body b1;
        private Body b3;
        private Body b2;
        private Body b4;
        private Body b6;

        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private bool colliding;
        private readonly Tweener tweener = new Tweener();       

        private Texture2D blank;
        private Texture2D fadedcircle;
        private Circle circle = new Circle(8);
        private Circle circle2 = new Circle(4);

        public DemoOneScreen()
        {
            VectorMath.gravity = new Vector2(0, 200);
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

            b1.Velocity = Vector2.Zero;
            b2.Velocity = Vector2.Zero;
            b3.Velocity = Vector2.Zero;
            b4.Velocity = Vector2.Zero;

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
            b5.Velocity = Vector2.Zero;
            b5.SetOrientation(0);
            world.AddBody(b5);


            rect = new PolygonShape();
            rect.SetBox(40, 40);
            b6 = new Body(rect, new Vector2(700, 750));
            b6.Velocity = Vector2.Zero;
            b6.SetStatic();
            b6.SetOrientation(0);
            world.AddBody(b6);

            b4.OnCollision += B4_OnCollision;
            b4.OnSeparation += B4_OnSeparation;


            PolygonShape box = new PolygonShape();
            box.SetBox(10, 10);

            PhysicsParticleEmitter emitter2 = new PhysicsParticleEmitter("Boxes", world, box, new Vector2(960, 100), new Interval(50, 150), new Interval(-Math.PI, Math.PI), 5.0f, new Interval(4000, 4000));
            emitter2.AddModifier(new ColorRangeModifier(Color.LightBlue, Color.Purple));            
            emitter2.Origin = new PointOrigin();
            emitter2.OnCollision += Emitter2_OnCollision;
            emitter2.Start();           
            
        }

        private ContactAction Emitter2_OnCollision(PhysicsParticle sender, Body other, Contact m)
        {
            Explode(sender.Position);
            //delete the particle
            return ContactAction.DESTROY;
        }


        public override void Activate(bool instancePreserved)
        {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");

            blank = Content.Load<Texture2D>("blank");
            fadedcircle = Content.Load<Texture2D>("fadedcircle");
            font = Content.Load<SpriteFont>("Debug");

            _spriteBatch = ScreenManager.SpriteBatch;

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

            b4.SetOrientation(b4.Orientation + (float)(0.002f * gameTime.ElapsedGameTime.TotalMilliseconds));           
            tweener.Update(gameTime.GetElapsedSeconds());
           
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            drawWorld.DrawSpatialGrid();
            
            foreach (PhysicsParticleEmitter e in world.physicsEmitters.FindAll(p => p.Name.Equals("Boxes")))
            {
                foreach (IParticle b in e.Particles)
                {
                    _spriteBatch.Draw(blank, new Rectangle(new Point((int)b.Position.X, (int)b.Position.Y), new Point(20, 20)), new Rectangle(0, 0, 4, 4), b.Color * b.Alpha, b.Orientation, new Vector2(2, 2), SpriteEffects.None, 0);
                }
            }
            
            _spriteBatch.Draw(blank, new Rectangle(new Point((int)b1.Position.X, (int)b1.Position.Y), new Point(40, 40)), new Rectangle(0, 0, 4, 4), Color.Green, b1.Orientation, new Vector2(2, 2), SpriteEffects.None, 0);
            _spriteBatch.Draw(blank, new Rectangle(new Point((int)b3.Position.X, (int)b3.Position.Y), new Point(40, 40)), new Rectangle(0, 0, 4, 4), Color.Green, b3.Orientation, new Vector2(2, 2), SpriteEffects.None, 0);
            _spriteBatch.Draw(blank, new Rectangle(new Point((int)b2.Position.X, (int)b2.Position.Y), new Point(40, 40)), new Rectangle(0, 0, 4, 4), Color.Green, b2.Orientation, new Vector2(2, 2), SpriteEffects.None, 0);
            _spriteBatch.Draw(blank, new Rectangle(new Point((int)b6.Position.X, (int)b6.Position.Y), new Point(80, 80)), new Rectangle(0, 0, 4, 4), Color.Green, b6.Orientation, new Vector2(2, 2), SpriteEffects.None, 0);
            _spriteBatch.Draw(blank, new Rectangle(new Point((int)b4.Position.X, (int)b4.Position.Y), new Point(40, 40)), new Rectangle(0, 0, 4, 4), colliding ? Color.Red : Color.Green, b4.Orientation, new Vector2(2, 2), SpriteEffects.None, 0);
            _spriteBatch.Draw(blank, new Rectangle(new Point(960, 800), new Point(1000, 20)), new Rectangle(0, 0, 4, 4), Color.Green, 0, new Vector2(2, 2), SpriteEffects.None, 0);

            foreach (PhysicsParticleEmitter e in world.physicsEmitters.FindAll(p => p.Name.Equals("Debris")))
            {
                foreach (IParticle b in e.Particles)
                {
                    _spriteBatch.Draw(blank, new Rectangle(new Point((int)b.Position.X, (int)b.Position.Y), new Point(8, 8)), new Rectangle(0, 0, 4, 4), b.Color * b.Alpha, b.Orientation, new Vector2(2, 2), SpriteEffects.None, 0);
                }
            }           
            _spriteBatch.End();


            _spriteBatch.Begin(0, BlendState.Additive);
            foreach (PhysicsParticleEmitter e in world.physicsEmitters.FindAll(p => p.Name.StartsWith("Explosion")))
            {
                e.Draw(_spriteBatch);
            }            
            _spriteBatch.End();

            drawWorld.Draw();

            GuiRenderer.BeginLayout(gameTime);
            DebugWindow();
            GuiRenderer.EndLayout();

        }

        private void Explode(Vector2 pos2)
        {
            PhysicsParticleEmitter emitter = new PhysicsParticleEmitter("Explosion", world, circle, Vector2.Zero, new Interval(0, 150), new Interval(-Math.PI, Math.PI), 10, new Interval(500, 1000));

            emitter.AddModifier(new ColorRangeModifier(Color.Orange, Color.Black));
            emitter.Origin = new PointOrigin();
            emitter.Position = pos2;
            emitter.ParticlesPerSecond = 2000;
            emitter.IgnoreGravity = true;
            emitter.LinearDamping = 0.04f;
            emitter.Texture = fadedcircle;
           
            PhysicsParticleEmitter emitter3 = new PhysicsParticleEmitter("Debris", world, circle2, new Vector2(1200, 100), new Interval(0, 200), new Interval(-Math.PI, Math.PI), 15.0f, new Interval(1000, 2000));
            emitter3.AddModifier(new AlphaFadeModifier());
            emitter3.AddModifier(new ColorRangeModifier(Color.LightBlue, Color.Purple));
            emitter3.Origin = new PointOrigin();
            emitter3.Position = pos2;
            emitter3.ParticlesPerSecond = 1000;
            emitter3.LinearDamping = 0.02f;
            
            tweener.TweenTo(target: emitter, expression: p => p.ParticlesPerSecond, toValue: 0, duration: 0.25f)
            .Easing(EasingFunctions.ExponentialOut)
            .OnEnd(p =>
            {
                PhysicsParticleEmitter physicsEmitter = (PhysicsParticleEmitter)p.Target;
                physicsEmitter.Stop();
            });
            tweener.TweenTo(target: emitter3, expression: p => p.ParticlesPerSecond, toValue: 0, duration: 0.25f)
            .Easing(EasingFunctions.ExponentialOut)
            .OnEnd(p =>
            {
                ((PhysicsParticleEmitter)p.Target).Stop();
            });

            emitter3.Start();
            emitter.Start();
        }


        private void B4_OnSeparation(Body sender, Body other)
        {
            colliding = false;
        }

        private ContactAction B4_OnCollision(Body sender, Body other, Contact m)
        {
            colliding = true;
            return ContactAction.COLLIDE;
        }

    }
}
