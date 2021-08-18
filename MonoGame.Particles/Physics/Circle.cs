using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Physics
{
    public class Circle : Shape
    {

        public Circle(float r)
        {
            radius = r;
        }

        public override void Initialize(float density = 1.0f)
        {
            ComputeMass(density);
        }

        private void ComputeMass(float density)
        {
            Body.m = (float)Math.PI * radius * radius * density;
            Body.im = (Body.m != 0) ? 1.0f / Body.m : 0.0f;
            Body.I = Body.m * radius * radius;
            Body.iI = (Body.I != 0) ? 1.0f / Body.I : 0.0f;
        }

        public override void SetOrientation(float radians)
        {
            //Must alway be overridden
        }

        public override AABB getAABB(float orient)
        {
            return new AABB(new Vector2(-radius, -radius), new Vector2(radius, radius));
        }

        public override Object Clone()
        {
            return new Circle(radius);
        }

    }
}
