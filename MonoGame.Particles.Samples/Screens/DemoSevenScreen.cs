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
    public class DemoSevenScreen : DemoScreen
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
        private Texture2D monogame;

        public float reveal;

        public DemoSevenScreen()
        {
            VectorMath.gravity = new Vector2(0, 0);
            world = new World();

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
            monogame = Content.Load<Texture2D>("mono");
            font = Content.Load<SpriteFont>("Debug");


            pixel = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[1] { Color.White });

            _spriteBatch = ScreenManager.SpriteBatch;


            ParticleEmitter emitter = new ParticleEmitter("Stars", world, new Vector2(0, 0), new Interval(100, 150), new Interval(-Math.PI, Math.PI), 200.0f, new Interval(1000, 2000));            
            emitter.AddModifier(new ScaleModifier(1.0f, 2.0f));
            emitter.AddBirthModifier(new TextureBirthModifier(star, circle));
            emitter.AddModifier(new ColorRangeModifier(Color.White, Color.LightBlue, Color.Purple, Color.Orange, Color.Transparent));
            emitter.Origin = new FairyDustAnimatedOrigin(new Rectangle(0, 0, 1920, 1080));           
            emitter.Start();


            /*ParticleEmitter emitter = new ParticleEmitter("Stars", world, new Vector2(960, 540), new Interval(10, 50), new Interval(-Math.PI, Math.PI), 200.0f, new Interval(1000, 2000));
            emitter.AddBirthModifier(new TextureBirthModifier(star, circle));
            emitter.AddModifier(new ColorRangeModifier(Color.White, Color.LightBlue, Color.Purple, Color.Orange, Color.Transparent));
            emitter.AddModifier(new ScaleModifier(1.0f, 2.0f));            
            emitter.Origin = new CircleAnimatedOrigin(200, 3.0d);
            emitter.Start();*/

           /*  emitter = new ParticleEmitter("Boxes", world, new Vector2(100, 100), new Interval(10, 40), new Interval(-Math.PI + 0.5f, -Math.PI - 0.5f), 300.0f, new Interval(1000, 2500));
            emitter.AddBirthModifier(new TextureBirthModifier(circle));
            emitter.AddModifier(new AlphaFadeModifier());
            emitter.AddModifier(new ScaleModifier(0.5f, 1.0f));            
            emitter.Origin = new TextureAnimatedOrigin(monogame, TextureAnimatedDirections.RIGHT, 5);
            emitter.Start();*/

            tweener.TweenTo(target: this, p => p.reveal, monogame.Width, 5);


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
            /*steptimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (steptimer < 0)
            {
                world.Step(0.016d);
                steptimer += 16;
            }*/
            world.Step(gameTime.GetElapsedSeconds());
            //tweener.Update(gameTime.GetElapsedSeconds());

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }       

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            /*_spriteBatch.Begin();

            _spriteBatch.Draw(monogame, new Vector2(100 + (int)reveal - 5, 100), new Rectangle((int)reveal - 5, 0, monogame.Width - (int)reveal - 5, monogame.Height), Color.White * 0.25f);
            _spriteBatch.Draw(monogame, new Vector2(100 + (int)reveal - 10, 100), new Rectangle((int)reveal - 10, 0, monogame.Width - (int)reveal - 10, monogame.Height), Color.White * 0.5f);
            _spriteBatch.Draw(monogame, new Vector2(100 + (int)reveal, 100), new Rectangle((int)reveal, 0, monogame.Width - (int)reveal, monogame.Height), Color.White);

            foreach (ParticleEmitter e in world.emitters.FindAll(p => p.Name.Equals("Boxes")))
            {
                e.Draw(_spriteBatch);
            }
            _spriteBatch.End();*/

            _spriteBatch.Begin(0, blendState: BlendState.Additive);
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

