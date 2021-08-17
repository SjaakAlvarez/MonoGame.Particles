using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Modifiers
{
    public abstract class BirthModifier
    {
        public abstract void Execute(Emitter e, IParticle p);
        public abstract bool SupportsPhysics { get; }
    }
}
