using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Physics
{

    public enum ContactAction { IGNORE, COLLIDE, DESTROY }
    public delegate ContactAction OnCollisionEventHandler(Body sender, Body other, Contact m);
    public delegate void OnSeparationEventHandler(Body sender, Body other);

    public class Body
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float AngularVelocity { get; set; }
        public float Orientation { get; set; }
        public bool IgnoreGravity { get; set; }
        public float Torque { get; set; }
        public Vector2 Force { get; set; }
        public float Restitution { get; set; }
        public bool IsParticle { get; set; }
        public float LinearDamping { get; set; }
        public bool FixedPosition { get; set; }

        public Shape Shape { get; }

        public float I;  // moment of inertia
        public float iI; // inverse inertia
        public float m;  // mass
        public float im; // inverse masee

        public float StaticFriction { get; set; }
        public float DynamicFriction { get; set; }

        //SpatialHash
        public AABB AABB { get; set; }
        public List<int> GridIndex { get; set; }

        private static Random rand = new Random();

        public Dictionary<Body, ContactAction> collidingWith = new Dictionary<Body, ContactAction>();
        private Dictionary<Body, ContactAction> newCollisions = new Dictionary<Body, ContactAction>();
        private List<Body> remove = new List<Body>();

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


        public Body(Shape shape, Vector2 pos)
        {
            Shape = shape;
            Shape.Body = this;
            Position = pos;
            Velocity = Vector2.Zero;

            Orientation = rand.Next(-3, 3);
            StaticFriction = 0.5f;
            DynamicFriction = 0.3f;
            Restitution = 0.2f;
            Force = Vector2.Zero;
            AngularVelocity = 0;

            GridIndex = new List<int>();

            Shape.Initialize();
        }

        public void calculateAABB()
        {
            if (Shape is Circle c)
            {
                AABB = c.getAABB(Orientation);
            }
            else if (Shape is PolygonShape p)
            {
                AABB = p.getAABB(Orientation);
            }
            AABB.min += Position;
            AABB.max += Position;
        }

        public void SetOrientation(float radians)
        {
            Orientation = radians;
            Shape.SetOrientation(radians);
        }

        public void ApplyForce(Vector2 f)
        {
            Force += f;
        }

        public void ApplyImpulse(Vector2 impulse, Vector2 contactVector)
        {
            if (!FixedPosition) Velocity += im * impulse;
            AngularVelocity += iI * VectorMath.Cross(contactVector, impulse);
        }

        public void SetStatic()
        {
            I = 0.0f;
            iI = 0.0f;
            m = 0.0f;
            im = 0.0f;
        }

        public ContactAction DoCollision(Body other, Contact m)
        {
            newCollisions[other] = ContactAction.IGNORE;

            if (collidingWith.ContainsKey(other))
            {
                //already colliding
                return collidingWith[other];
            }
            else
            {
                //new collision, send event
                ContactAction action = ContactAction.COLLIDE;

                if (other.onCollisionEventHandler != null)
                {
                    action = other.onCollisionEventHandler(other, this, m);
                }

                if (onCollisionEventHandler != null)
                {
                    action = onCollisionEventHandler(this, other, m);
                }
                collidingWith[other] = action;
                return action;
            }

        }

        public void FinishCollisions()
        {
            remove.Clear();

            foreach (KeyValuePair<Body, ContactAction> kv in collidingWith)
            {
                if (!newCollisions.ContainsKey(kv.Key))
                {
                    //separation
                    remove.Add(kv.Key);

                    if (kv.Key.onSeparationEventHandler != null)
                    {
                        kv.Key.onSeparationEventHandler(kv.Key, this);
                    }

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
