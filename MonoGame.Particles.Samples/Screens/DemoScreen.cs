using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.ImGui;
using MonoGame.Particles.Physics;
using MonoGame.Particles.Physics.Debug;
using MonoGame.Particles.Samples.GameStateManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Samples.Screens
{
    public class DemoScreen : GameScreen
    {
        protected World world;
        protected DrawWorld drawWorld;
        protected FramesPerSecondCounterComponent framesPerSecondCounter;
        protected ImGUIRenderer GuiRenderer;
        protected Queue<float> fpsQueue = new Queue<float>(100);
        protected double fpsTimer = 1000;

        private bool showInfo;
        private bool drawAABB;
        private bool drawShapes;

        public DemoScreen()
        {
            for (int i = 0; i < 100; i++) fpsQueue.Enqueue(0);
        }

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            Matrix _localProjection = Matrix.CreateOrthographicOffCenter(0f, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
            Matrix _localView = Matrix.Identity;

            drawWorld = new DrawWorld(world, ScreenManager.Game, _localProjection, _localView);     
            
            GuiRenderer = new ImGUIRenderer(ScreenManager.Game).Initialize().RebuildFontAtlas();

            framesPerSecondCounter = new FramesPerSecondCounterComponent(ScreenManager.Game);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            framesPerSecondCounter.Update(gameTime);

            fpsTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (fpsTimer < 0)
            {
                fpsQueue.Enqueue(framesPerSecondCounter.FramesPerSecond);
                fpsQueue.Dequeue();
                fpsTimer += 1000;
            }
            drawWorld.DrawAABB = drawAABB;
            drawWorld.DrawShapes = drawShapes;
            drawWorld.ShowInfo = showInfo;
        }

        public override void Draw(GameTime gameTime)
        {
            framesPerSecondCounter.Draw(gameTime);            
            base.Draw(gameTime);
        }

        public void DebugWindow()
        {
            ImGuiNET.ImGui.Begin("Debug");
            ImGuiNET.ImGui.Text("Enable/disable debug view options here:");
            ImGuiNET.ImGui.Checkbox("Show shapes", ref drawShapes);
            ImGuiNET.ImGui.Checkbox("Show AABB", ref drawAABB);
            ImGuiNET.ImGui.Checkbox("Show info", ref showInfo);

            float[] temp = fpsQueue.ToArray();
            ImGuiNET.ImGui.PlotLines("FPS", ref temp[0], 100, 0, "", 0, 5000, new System.Numerics.Vector2(0, 100));
            ImGuiNET.ImGui.End();
        }

        public void ShowShapes()
        {
            drawShapes = true;
        }
    }
}
