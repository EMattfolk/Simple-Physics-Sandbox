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
        private Vector2 position, velocity;
        
        public PhysicsObject ()
        {
            position = new Vector2();
            velocity = new Vector2();
        }

        public PhysicsObject (Vector2 position, Vector2 velocity)
        {
            this.position = position;
            this.velocity = velocity;
        }

        public void UpdatePosition (GameTime gameTime)
        {
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public Vector2 Position { get => position; set => position = value; }
        public Vector2 Velocity { get => velocity; set => velocity = value; }

        abstract public PhysicsObjectType GetPhysicsObjectType ();
    }
}
