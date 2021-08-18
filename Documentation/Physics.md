```C#
VectorMath.gravity = new Vector2(0, 300);
world = new World(1, new Vector2(1920, 1080), 256);

PolygonShape rect = new PolygonShape();
rect.SetBox(500, 10);
floor = new Body(rect, new Vector2(960, 800));
floor.SetStatic();
floor.Restitution = 0.4f;
floor.DynamicFriction = 0.3f;
floor.StaticFriction = 0.3f;
floor.SetOrientation(0.0f);
world.AddBody(floor);
```

```C#
Matrix _localProjection = Matrix.CreateOrthographicOffCenter(0f, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
Matrix _localView = Matrix.Identity;

//create a debug view for the physics engine
drawWorld = new DrawWorld(world, this, _localProjection, _localView);   
drawWorld.DrawAABB = true;
drawWorld.DrawShapes = true;
drawWorld.ShowInfo = true; 
```

```C#
public override void Update(GameTime gameTime)
{   
    world.Step(gameTime.GetElapsedSeconds());    
}

public override void Draw(GameTime gameTime)
{
    base.Draw(gameTime);
    drawWorld.DrawSpatialGrid();
    drawWorld.Draw();
}
```