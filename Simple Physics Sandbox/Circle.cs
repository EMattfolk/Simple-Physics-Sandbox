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
    class Circle : PhysicsObject
    {
        private float radius;

        public Circle (float radius) : base()
        {
            this.radius = radius;
        }

        public Circle (float radius, Vector2 position, Vector2 velocity) : base(position, velocity)
        {
            this.radius = radius;
        }
    }
}
