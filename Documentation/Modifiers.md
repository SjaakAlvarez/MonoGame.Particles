## ActionModifier

Executes a custom action for every particle during it's lifetime. _Can be used on a 
PhysicsParticleEmitter as long as Position, Orientation, Velocity and AngularVelocity are
not modified._

```C#
//make the particle grow
emitter.AddModifier(
    new ActionModifier(
        (e,p) => p.Scale = p.Age/p.MaxAge
    ));
```

## AlphaFadeModifier

Modifies the alpha of the particle from 1 to 0 over it's lifetime

## ColorRangeModifier

Modifies the color of the particle over it's lifetime

## GravityModifier

Applies gravity to a particle. **Cannot be used on a PhysicsParticleEmitter!**

## ScaleModifier

Modifies the scale of the particle over it's lifetime. **Cannot be used on a PhysicsParticleEmitter!**