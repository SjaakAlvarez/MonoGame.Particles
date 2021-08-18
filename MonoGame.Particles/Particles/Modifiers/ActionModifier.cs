using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Modifiers
{
    public delegate void ParticleAction(Emitter e, IParticle p); 

    public class ActionModifier : Modifier
    {
        private readonly ParticleAction _action;

        public ActionModifier(ParticleAction action)
        {
            _action = action;
        }

        public override bool SupportsPhysics { get => true; }

        public override void Execute(Emitter e, double seconds, IParticle p)
        {
            _action.Invoke(e,p);
        }
    }
}
