using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Physics
{
    public class Shape
    {        
        public Body Body { get; set; }
                
        public float radius;

        // For Polygon shape
        public Matrix u; // Orientation matrix from model to world

        public virtual void Initialize(float density=1.0f)
        {

        }

        public virtual void SetOrientation(float r)
        {

        }

        public virtual AABB getAABB(float orient)
        {
            return null;
        }

    }
}
