# World object

For simple particles, initialze the World object like this:

```C#
world = new World();
```

And in your update do this:

```C#
public override void Update(GameTime gameTime)
{
    world.Step(gameTime.GetElapsedSeconds());
}
```


For physics particles,  more setup is needed. The first parameter is the number of iterations.
This parameter should always be 1. The second parameter is the size of the Spatial Hash.
This should be the size of the screen. The last parameter is the cell size of the spatial hash.
This depends on the size of the bodies in the physics engine. Use the average size of the 
physics bodies here.

```C#
world = new World(1, new Vector2(1920,1080), 64);
```
