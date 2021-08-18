using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Modifiers
{
    public class ActionBirthModifier : BirthModifier
    {
        private readonly ParticleAction _action;

        public ActionBirthModifier(ParticleAction action)
        {
            _action = action;
        }

        public override bool SupportsPhysics { get => true; }

        public override void Execute(Emitter e, IParticle p)
        {
            _action.Invoke(e, p);
        }
    }
}
