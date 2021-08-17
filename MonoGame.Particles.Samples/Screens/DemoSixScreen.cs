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
using MonoGame.Particles.Samples.Modifiers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Samples.Screens
{
    public class DemoSixScreen : DemoScreen
    {        
        private double steptimer = 16;
        const int CELLSIZE = 64;
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
        private Texture2D missile;
        private Texture2D star;
        private Texture2D circle;
        private Texture2D fadedcircle;
        private Texture2D pixel;

        public DemoSixScreen()
        {
            VectorMath.gravity = new Vector2(0, 0);
            world = new World(1, new Vector2(1920, 1080), CELLSIZE);

        }



        public override void Activate(bool instancePreserved)
        {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");

            blank = Content.Load<Texture2D>("blank");
            star = Content.Load<Texture2D>("star");
            circle = Content.Load<Texture2D>("circle");
            fadedcircle = Content.Load<Texture2D>("fadedcircle");
            missile = Content.Load<Texture2D>("missile");
            font = Content.Load<SpriteFont>("Debug");

            pixel = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[1] { Color.White });

            _spriteBatch = ScreenManager.SpriteBatch;


            

            ParticleEmitter emitter = new ParticleEmitter("Boxes", world, new Vector2(200, 200), new Interval(25, 50), new Interval(-Math.PI, Math.PI), 20.0f, new Interval(2000, 2000));
            emitter.AddModifier(new ColorRangeModifier(Color.Transparent, Color.Red, new Color(255, 255, 0), new Color(0, 255, 0), new Color(0, 0, 255), new Color(255, 0, 255), Color.Transparent));
            emitter.Origin = new PointOrigin();
            emitter.Texture = star;
            emitter.Start();

            emitter = new ParticleEmitter("Boxes", world, new Vector2(600, 200), new Interval(15, 80), new Interval(-Math.PI, Math.PI), 300.0f, new Interval(500, 1000));
            emitter.AddModifier(new ColorRangeModifier(Color.Transparent,Color.Red, new Color(255, 255, 0), new Color(0, 255, 0), new Color(0, 0, 255), new Color(255, 0, 255), Color.Transparent));
            
            emitter.AddBirthModifier(new InwardBirthModifier());           
            emitter.Origin = new CircleOrigin(100,true);
            emitter.Texture = star;
            emitter.Start();

            emitter = new ParticleEmitter("Boxes", world, new Vector2(1200, 200), new Interval(5, 25), new Interval(-Math.PI, Math.PI), 200.0f, new Interval(2000, 2000));
            emitter.AddModifier(new ColorRangeModifier(Color.Transparent, Color.Red, new Color(255, 255, 0), new Color(0, 255, 0), new Color(0, 0, 255), new Color(255, 0, 255), Color.Transparent));
            emitter.Origin = new RectangleOrigin(300, 100);
            emitter.AddBirthModifier(new InwardBirthModifier());
            emitter.Texture = star;
            emitter.Start();

            emitter = new ParticleEmitter("Stars", world, new Vector2(200, 500), new Interval(55, 100), new Interval(-Math.PI, Math.PI), 100.0f, new Interval(2000, 2000));
            emitter.AddModifier(new ScaleModifier(1,4));
            emitter.AddModifier(new AlphaFadeModifier());
            emitter.AddBirthModifier(new ColorBirthModifier(Color.Red, new Color(255, 255, 0), new Color(0, 255, 0), new Color(0, 0, 255), new Color(255, 0, 255)));            
            emitter.Origin = new PointOrigin();
            emitter.Texture = star;
            emitter.Start();

            emitter = new ParticleEmitter("Boxes", world, new Vector2(600, 500), new Interval(5, 25), new Interval(-Math.PI, Math.PI), 100.0f, new Interval(2000, 2000));
            emitter.AddModifier(new ColorRangeModifier(Color.Transparent, Color.Red, new Color(255, 255, 0), new Color(0, 255, 0), new Color(0, 0, 255), new Color(255, 0, 255), Color.Transparent));
            emitter.Origin = new CircleOrigin(100, true);
            emitter.AddBirthModifier(new OutwardBirthModifier());
            emitter.Texture = star;
            emitter.Start();

            emitter = new ParticleEmitter("Boxes", world, new Vector2(1200, 500), new Interval(5, 25), new Interval(-Math.PI, Math.PI), 200.0f, new Interval(2000, 2000));
            emitter.AddModifier(new ColorRangeModifier(Color.Transparent, Color.Red, new Color(255, 255, 0), new Color(0, 255, 0), new Color(0, 0, 255), new Color(255, 0, 255), Color.Transparent));
            emitter.Origin = new RectangleOrigin(300, 100, true);
            emitter.AddBirthModifier(new OutwardBirthModifier());
            emitter.Texture = star;
            emitter.Start();


            emitter = new ParticleEmitter("Stars", world, new Vector2(600, 900), new Interval(5, 25), new Interval(-Math.PI, Math.PI), 50.0f, new Interval(2000, 2500));           
            emitter.Origin = new CircleOrigin(10, false);
            emitter.AddModifier(new ScaleModifier(1, 2));
            emitter.AddModifier(new ColorRangeModifier(Color.Blue, Color.Orange, Color.DarkRed,Color.Black,Color.Transparent));
            emitter.AddModifier(new GravityModifier(new Vector2(0, -100)));
            emitter.Texture = fadedcircle;
            emitter.Start();

            emitter = new ParticleEmitter("Stars", world, new Vector2(200, 800), new Interval(100, 150), new Interval(-Math.PI, Math.PI), 50.0f, new Interval(1000, 1500));
            emitter.AddBirthModifier(new ScaleBirthModifier(new Interval(1,3)));
            emitter.AddBirthModifier(new TextureBirthModifier(star,circle));
            emitter.AddBirthModifier(new ColorBirthModifier(Color.LightBlue));
            emitter.AddModifier(new AlphaFadeModifier());
            emitter.Origin = new PointOrigin();
            emitter.Texture = star;
            emitter.Start();            

            emitter = new ParticleEmitter("Stars", world, new Vector2(1200, 800), new Interval(0, 0), new Interval(-Math.PI, Math.PI), 50.0f, new Interval(2000, 6000));                                   
            emitter.AddModifier(new ColorRangeModifier(Color.Transparent,Color.LightBlue, Color.Orange,Color.Transparent));
            emitter.AddBirthModifier(new ScaleBirthModifier(new Interval(2, 15)));
            emitter.AddModifier(new ActionModifier(
                (e,p) => p.Position = e.Position+new Vector2((float)Math.Sin(p.Age/p.MaxAge*6.28)*150,p.Position.Y-e.Position.Y)
                ));
            emitter.Origin = new RectangleOrigin(300,200);
            emitter.Texture = pixel;
            emitter.Start();

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
            //world.Step(gameTime.GetElapsedSeconds());
            tweener.Update(gameTime.GetElapsedSeconds());
           
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            //drawWorld.DrawSpatialGrid();

            _spriteBatch.Begin();
            foreach (ParticleEmitter e in world.emitters.FindAll(p => p.Name.Equals("Boxes")))
            {
                e.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            _spriteBatch.Begin(0,blendState:BlendState.Additive);
            foreach (ParticleEmitter e in world.emitters.FindAll(p => p.Name.Equals("Stars")))
            {
                e.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            drawWorld.Draw();

            GuiRenderer.BeginLayout(gameTime);
            DebugWindow();
            GuiRenderer.EndLayout();

        }

      

    }
}

