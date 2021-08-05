using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Physics
{

    public delegate bool OnCollisionEventHandler(Body sender, Body other, Contact m);
    public delegate void OnSeparationEventHandler(Body sender, Body other);

    public class Body
    {
        public Vector2 position;
        public Vector2 velocity;        

        public float angularVelocity;
        public float torque;
        public float orientation;
        public Vector2 Force { get; set; }

        public bool IgnoreGravity { get; set; }
        
        public float I;  // moment of inertia
        public float iI; // inverse inertia
        public float m;  // mass
        public float im; // inverse masee

        public float staticFriction;
        public float dynamicFriction;
        public float restitution;
        public bool IsParticle;        
        public float LinearDamping = 0.0f;
        public AABB aabb;

        public Shape shape;
        private static Random rand = new Random();

        public List<int> gridIndex { get; set; }

        public Dictionary<Body, bool> collidingWith = new Dictionary<Body, bool>();
        public Dictionary<Body, bool> newCollisions = new Dictionary<Body, bool>();


        internal OnCollisionEventHandler onCollisionEventHandler;
        public event OnCollisionEventHandler OnCollision
        {
            add { onCollisionEventHandler += value; }
            remove { onCollisionEventHandler -= value; }
        }

        internal OnSeparationEventHandler onSeparationEventHandler;
        public event OnSeparationEventHandler OnSeparation
        {
            add { onSeparationEventHandler += value; }
            remove { onSeparationEventHandler -= value; }
        }



        /* public event OnCollisionEventHandler OnCollision;
         public virtual bool? HandleCollision(Body sender, Body other, Manifold m)
         {
             return OnCollision?.Invoke(sender, other, m);
         }*/


        public Body(Shape shape, Vector2 pos)
        {
            this.shape = shape;
            this.shape.Body = this;
            this.position = pos;
            this.velocity = Vector2.Zero;
            
            orientation = rand.Next(-3, 3);
            staticFriction = 0.5f;
            dynamicFriction = 0.3f;
            restitution = 0.2f;
            Force = Vector2.Zero;
            angularVelocity = 0;

            gridIndex = new List<int>();

            this.shape.Initialize();
        }

        public void calculateAABB()
        {
            if(shape is Circle c)
            {
                aabb = c.getAABB(orientation);               
            }
            else if(shape is PolygonShape p)
            {
                aabb = p.getAABB(orientation);
            }
            aabb.min += position;
            aabb.max += position;
        }

        public void SetOrientation(float radians)
        {
            orientation = radians;
            shape.SetOrientation(radians);
        }

        public void ApplyForce(Vector2 f)
        {
            Force += f;
        }

        public void ApplyImpulse(Vector2 impulse, Vector2 contactVector)
        {            
            velocity += im * impulse;
            angularVelocity += iI * VectorMath.Cross(contactVector, impulse);
        }

        public void SetStatic()
        {
            I = 0.0f;
            iI = 0.0f;
            m = 0.0f;
            im = 0.0f;
        }

        public bool DoCollision(Body other, Contact m)
        {

            newCollisions[other] = false;

            if (collidingWith.ContainsKey(other))
            {
                //already colliding
                return collidingWith[other];                
            }
            else
            {
                //new collision, send event
                bool handle = true;
                if (onCollisionEventHandler != null)
                {
                    handle = onCollisionEventHandler(this, other, m);
                }
                collidingWith[other] = handle;
                return handle;
            }
            
        }

        public void FinishCollisions()
        {
            List<Body> remove = new List<Body>();

            foreach (KeyValuePair<Body, bool> kv in collidingWith)
            {
                if (!newCollisions.ContainsKey(kv.Key))
                {
                    //separation
                    remove.Add(kv.Key);
                    if (onSeparationEventHandler != null)
                    {
                        onSeparationEventHandler(this, kv.Key);
                    }
                }
            }

            foreach (Body b in remove) collidingWith.Remove(b);

            newCollisions.Clear();
        }


    }
}
