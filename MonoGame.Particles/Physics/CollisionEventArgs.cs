using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Physics
{
    public class CollisionEventArgs : EventArgs
    {
        public Contact manifold;
        public Body otherBody;

        public CollisionEventArgs(Contact m, Body b)
        {
            this.manifold = m;
            this.otherBody = b;
        }
    }
}
