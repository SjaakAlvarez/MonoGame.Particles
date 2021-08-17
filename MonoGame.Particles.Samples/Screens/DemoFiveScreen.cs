using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Particles.Physics;
using MonoGame.Particles.Samples.GameStateManagement;
using System;


namespace MonoGame.Particles.Samples.Screens
{
    public class DemoFiveScreen : DemoScreen
    {
        const int CELLSIZE = 256;

        private ContentManager Content;
        private readonly Body floor;
        private readonly Random random = new Random();
        private SpriteBatch _spriteBatch;
        private float orientation;

        public DemoFiveScreen()
        {
            VectorMath.gravity = new Vector2(0, 300);
            world = new World(1, new Vector2(1920, 1080), CELLSIZE);

            PolygonShape rect = new PolygonShape();
            rect.SetBox(500, 10);
            floor = new Body(rect, new Vector2(960, 800));
            floor.SetStatic();
            floor.Velocity = Vector2.Zero;
            floor.Restitution = 0.4f;
            floor.DynamicFriction = 0.3f;
            floor.StaticFriction = 0.3f;
            floor.SetOrientation(0.0f);
            world.AddBody(floor);

        }

        public void AddCircle()
        {
            Circle circle = new Circle(random.Next(10, 50));
            Body body = new Body(circle, new Vector2(random.Next(600) + 660, -100));
            body.SetOrientation(0.0f);
            body.Restitution = 0.5f;
            body.DynamicFriction = 0.3f;
            body.StaticFriction = 0.3f;
            body.LinearDamping = 0.0f;
            world.AddBody(body);
        }

        public void AddRectangle()
        {
            PolygonShape polygon = new PolygonShape();
            polygon.SetBox(random.Next(10, 50), random.Next(10, 50));
            Body body = new Body(polygon, new Vector2(random.Next(600) + 660, -100));
            body.SetOrientation(random.Next(6));
            body.Restitution = 0.5f;
            body.DynamicFriction = 0.3f;
            body.StaticFriction = 0.3f;
            body.LinearDamping = 0.0f;
            world.AddBody(body);
        }

        public void AddNGon(int sides)
        {
            PolygonShape polygon = new PolygonShape();
            Vector2[] vertices = new Vector2[sides];

            Vector2 dist = new Vector2(random.Next(10, 50), 0);
            for (int i = 0; i < sides; i++)
            {
                vertices[i] = Vector2.Transform(dist, Matrix.CreateRotationZ(i * (float)Math.PI * 2 / sides));
            }

            polygon.Set(vertices, sides);

            Body body = new Body(polygon, new Vector2(random.Next(600) + 660, -100));
            body.SetOrientation(random.Next(6));
            body.Restitution = 0.5f;
            body.DynamicFriction = 0.3f;
            body.StaticFriction = 0.3f;
            body.LinearDamping = 0.0f;
            world.AddBody(body);
        }

        public void AddRandomShape()
        {
            PolygonShape polygon = new PolygonShape();

            int sides = random.Next(4, 6);
            Vector2[] vertices = new Vector2[sides];

            for (int i = 0; i < sides; i++)
            {
                vertices[i] = new Vector2(random.Next(30,100), random.Next(30, 100));
            }
            polygon.Set(vertices, sides);

            Body body = new Body(polygon, new Vector2(random.Next(600) + 660, -100));
            body.SetOrientation(random.Next(6));
            body.Restitution = 0.5f;
            body.DynamicFriction = 0.3f;
            body.StaticFriction = 0.3f;
            body.LinearDamping = 0.0f;
            world.AddBody(body);
        }

        public override void Activate(bool instancePreserved)
        {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");

            _spriteBatch = ScreenManager.SpriteBatch;
            base.Activate(instancePreserved);
            ShowShapes();
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
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            world.Step(gameTime.GetElapsedSeconds());
            floor.SetOrientation(orientation);
            world.bodies.RemoveAll(p => p.Position.Y > 1200);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            drawWorld.DrawSpatialGrid();
            drawWorld.Draw();

            GuiRenderer.BeginLayout(gameTime);

            ImGuiNET.ImGui.Begin("Physics Demo");

            ImGuiNET.ImGui.SliderAngle("Floor angle", ref orientation, -15, 15);

            ImGuiNET.ImGui.NewLine();

            if (ImGuiNET.ImGui.Button("Add Circle"))
            {
                AddCircle();
            }
            ImGuiNET.ImGui.SameLine();
            if (ImGuiNET.ImGui.Button("Add Rectangle"))
            {
                AddRectangle();
            }
            ImGuiNET.ImGui.SameLine();
            if (ImGuiNET.ImGui.Button("Add Triangle"))
            {
                AddNGon(3);
            }
            if (ImGuiNET.ImGui.Button("Add Pentagon"))
            {
                AddNGon(5);
            }
            ImGuiNET.ImGui.SameLine();
            if (ImGuiNET.ImGui.Button("Add Hexagon"))
            {
                AddNGon(6);
            }
            ImGuiNET.ImGui.SameLine();
            if (ImGuiNET.ImGui.Button("Add Random Shape"))
            {
                AddRandomShape();
            }

            ImGuiNET.ImGui.End();

            DebugWindow();

            GuiRenderer.EndLayout();


        }


    }
}
