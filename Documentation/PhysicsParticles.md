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
emitter.Start(); 
```