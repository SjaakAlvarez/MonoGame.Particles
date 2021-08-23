using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Data
{
    public class ParticlePooledObjectPolicy : IPooledObjectPolicy<IParticle>
    {
        public IParticle Create()
        {
            return new Particle();
        }

        public bool Return(IParticle obj)
        {
            return true;
        }
    }
}
