using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles
{
    public class ParticleEventArgs
    {
        public IParticle Particle { get; set; }

        public ParticleEventArgs(IParticle p)
        {
            Particle = p;
        }
    }
}
