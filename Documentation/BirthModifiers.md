## ActionBirthModifier

Executes a custom action for every particle right after birth. _Can be used on a 
PhysicsParticleEmitter as long as Position, Orientation, Velocity and AngularVelocity are
not modified._

## ColorBirthModifier

Changes the color of the particle at birth.

## InwardBirthModifier :warning:

Makes the particle move to the center of the origin. Use this on Circle or Rectangle Origins. 
**Cannot be used on a PhysicsParticleEmitter!**

## OutwardBirthModifier :warning:

Makes the particle move away from the center of the origin. Use this on Circle or Rectangle Origins. 
**Cannot be used on a PhysicsParticleEmitter!**

## ScaleBirthModifier :warning:

Changes the size of the particle at birth. 
**Cannot be used on a PhysicsParticleEmitter!**

## TextureBirthModifier

Changes the texture of the particle at birth. Multiple textures can be used.