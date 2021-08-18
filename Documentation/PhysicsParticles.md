Setup the PhysicsParticleEmitter:

```C#
//create a 20px by 20px box
PolygonShape box = new PolygonShape();
box.SetBox(10, 10);

PhysicsParticleEmitter emitter = new PhysicsParticleEmitter("Boxes", world,
box, //use the box as a body
new Vector2(960, 100), //position
new Interval(50, 150), //speed
new Interval(-Math.PI, Math.PI), //direction
5.0f, //particles per second
new Interval(4000, 4000)); lifetime in milliseconds

emitter.AddModifier(new ColorRangeModifier(Color.LightBlue, Color.Purple));            
emitter.Origin = new PointOrigin();
emitter.Texture = boxTexture; //this should be the same size as the body
emitter.Start(); 
```

Make sure to update the world in the Update method

```C#
public override void Update(GameTime gameTime)
{
    world.Step(gameTime.GetElapsedSeconds());
}
```

To draw the particles use the following:

```C#
foreach (PhysicsParticleEmitter e in world.physicsEmitters.FindAll(p => p.Name.Equals("Boxes")))
{
    e.Draw(spriteBatch);
}           
```

