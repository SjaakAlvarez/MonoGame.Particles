using Microsoft.Xna.Framework;
using MonoGame.Particles.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles.Origins
{
    public class PointOrigin : Origin
    {
        
        public override Vector2 GetPosition()
        {
            return Vector2.Zero;
        }
    }
}
