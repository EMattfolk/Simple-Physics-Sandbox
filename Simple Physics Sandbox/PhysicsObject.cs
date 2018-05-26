using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Simple_Physics_Sandbox
{
    enum PhysicsObjectType
    {
        Circle
    }

    abstract class PhysicsObject
    {
        private static int counter = 0;
        private int mass, id;
        private Vector2 position, velocity;
        private HashSet<int> collisionIds;
        
        public PhysicsObject () : this(new Vector2(), new Vector2(), 100) { }

        public PhysicsObject (Vector2 position, Vector2 velocity, int mass)
        {
            this.position = position;
            this.velocity = velocity;
            this.mass = mass;
            collisionIds = new HashSet<int>();
            id = counter;
            counter++;
        }

        public void UpdatePosition (GameTime gameTime)
        {
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public Vector2 Position { get => position; set => position = value; }
        public Vector2 Velocity { get => velocity; set => velocity = value; }
        public int Mass { get => mass; set => mass = value; }
        public int Id { get => id; }
        public HashSet<int> CollisionIds { get => collisionIds; set => collisionIds = value; }

        abstract public PhysicsObjectType GetPhysicsObjectType ();
    }
}
