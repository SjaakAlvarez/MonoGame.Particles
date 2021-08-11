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


namespace MonoGame.Particles.Samples.Screens
{
    public class DemoThreeScreen : GameScreen
    {
        private World world;
        double steptimer = 16;
        const int CELLSIZE = 512;
        private ContentManager Content;

        private Body floor;
        List<Body> balls = new List<Body>();
        private Random random = new Random();

        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private readonly Tweener tweener = new Tweener();

        private DrawWorld drawWorld;

        Texture2D blank;
        Texture2D ball;
        Texture2D ballshadow;
        private double balltimer;

        public float orientation { get; set; } = 0.3f;

        public ImGUIRenderer GuiRenderer;

        public float ballStaticFriction=0.2f;
        public float ballDynamicFriction=0.2f;
        public float ballRestitution=0.3f;
        public float ballsPerSecond = 1.0f;

        public float floorStaticFriction = 0.2f;
        public float floorDynamicFriction = 0.2f;
        public float floorRestitution = 0.3f;

        public DemoThreeScreen()
        {
            VectorMath.gravity = new Vector2(0, 300);
            world = new World(1, new Vector2(1920, 1080), CELLSIZE);

            PolygonShape rect = new PolygonShape();
            rect.SetBox(500, 10);
            floor = new Body(rect, new Vector2(960, 800));
            floor.SetStatic();
            floor.Velocity = Vector2.Zero;
            floor.Restitution = 0.7f;
            floor.DynamicFriction = 0.7f;
            floor.StaticFriction = 0.7f;
            floor.SetOrientation(0.1f);            
            world.AddBody(floor);

        }

        public void AddBall()
        {
            Circle circle = new Circle(40);
            Body body = new Body(circle, new Vector2(random.Next(600)+660, -100));
            body.Velocity = Vector2.Zero;
            body.SetOrientation(0.0f);
            body.Restitution = ballRestitution;
            body.DynamicFriction = ballDynamicFriction;
            body.StaticFriction = ballStaticFriction;
            body.LinearDamping = 0.0f;            
            world.AddBody(body);
            balls.Add(body);
        }

        private ImGuiNET.ImFontPtr uifont; 
        

        public override void Activate(bool instancePreserved)
        {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");

            blank = Content.Load<Texture2D>("blank");           
            font = Content.Load<SpriteFont>("Debug");
            ball = Content.Load<Texture2D>("ball");
            ballshadow = Content.Load<Texture2D>("ballshadow");

            _spriteBatch = ScreenManager.SpriteBatch;

            Matrix _localProjection = Matrix.CreateOrthographicOffCenter(0f, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
            Matrix _localView = Matrix.Identity;

            drawWorld = new DrawWorld(world, ScreenManager.Game, _localProjection, _localView);
            drawWorld.DrawAABB = false;
            drawWorld.DrawShapes = false;
            drawWorld.DrawInfo = true;

            tweener.TweenTo(target: this, p => p.orientation, -0.3f, 5)
                .Easing(EasingFunctions.QuadraticInOut)
                .AutoReverse()
                .RepeatForever();

            GuiRenderer = new ImGUIRenderer(ScreenManager.Game).Initialize().RebuildFontAtlas();

            //uifont=ImGuiNET.ImGui.GetIO().Fonts.AddFontFromFileTTF("monogram.ttf",18);
            //GuiRenderer.RebuildFontAtlas();
            
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

            balltimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (balltimer < 0)
            {
                AddBall();
                balltimer += 1000.0d/ballsPerSecond;
            }

            floor.SetOrientation(orientation);            

            foreach (Body b in balls.FindAll(p => p.Position.Y > 1200)) world.RemoveBody(b);
            balls.RemoveAll(p => p.Position.Y > 1200);

            floor.Restitution = floorRestitution;
            floor.DynamicFriction = floorDynamicFriction;
            floor.StaticFriction = floorStaticFriction;
          
            tweener.Update(gameTime.GetElapsedSeconds());
            
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            drawWorld.DrawSpatialGrid();
           
            foreach (Body b in balls)
            {
                _spriteBatch.Draw(ball, new Vector2(b.Position.X, b.Position.Y), new Rectangle(0, 0, ball.Width, ball.Height), Color.White, b.Orientation, new Vector2(ball.Width, ball.Height) / 2, 1.0f, SpriteEffects.None, 0);
            }

            _spriteBatch.Draw(blank, new Rectangle(new Point(960, 800), new Point(1000, 20)), new Rectangle(0, 0, 4, 4), Color.Green, floor.Orientation, new Vector2(2, 2), SpriteEffects.None, 0);


            _spriteBatch.End();

            _spriteBatch.Begin(0,blendState:BlendState.AlphaBlend);
            foreach (Body b in balls)
            {                                
                 _spriteBatch.Draw(ballshadow, new Vector2(b.Position.X, b.Position.Y), new Rectangle(0, 0, ballshadow.Width, ballshadow.Height),Color.White, 0, new Vector2(ballshadow.Width, ballshadow.Height) / 2, 1.0f, SpriteEffects.None, 0);
            }
            _spriteBatch.End();


            _spriteBatch.Begin(0, BlendState.Additive);                    

            _spriteBatch.End();

            drawWorld.Draw();

            base.Draw(gameTime);

            GuiRenderer.BeginLayout(gameTime);

            //ImGuiNET.ImGui.PushFont(uifont);

            ImGuiNET.ImGui.Begin("Bounce Demo");

            ImGuiNET.ImGui.SliderFloat("Balls/second", ref ballsPerSecond, 0.1f, 10.0f);

            ImGuiNET.ImGui.CollapsingHeader("Debug");
            ImGuiNET.ImGui.Checkbox("Show shapes", ref drawWorld.DrawShapes);
            ImGuiNET.ImGui.Checkbox("Show AABB", ref drawWorld.DrawAABB);
            ImGuiNET.ImGui.Checkbox("Show info", ref drawWorld.DrawInfo);

            ImGuiNET.ImGui.CollapsingHeader("Floor");
            ImGuiNET.ImGui.SliderFloat("F Static friction", ref floorStaticFriction, 0, 2);
            ImGuiNET.ImGui.SliderFloat("F Dynamic friction", ref floorDynamicFriction, 0, 2);
            ImGuiNET.ImGui.SliderFloat("Floor Restitution", ref floorRestitution, 0, 1);            

            ImGuiNET.ImGui.CollapsingHeader("Balls");
            ImGuiNET.ImGui.SliderFloat("B Static friction", ref ballStaticFriction, 0, 2);
            ImGuiNET.ImGui.SliderFloat("B Dynamic friction", ref ballDynamicFriction, 0, 2);
            ImGuiNET.ImGui.SliderFloat("Ball Restitution", ref ballRestitution, 0, 1);
                       

            ImGuiNET.ImGui.End();

            //ImGuiNET.ImGui.PopFont();


            ImGuiNET.ImGui.ShowDemoWindow();
            


            GuiRenderer.EndLayout();


        }

       
    }
}
