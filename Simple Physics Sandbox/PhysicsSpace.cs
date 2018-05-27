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
    class PhysicsSpace
    {
        private int width, height, gridSize;
        private float friction, gravity = 982f;
        private List<PhysicsObject>[][] grid;
        private List<PhysicsObject> physicsObjects;

        public PhysicsSpace (int width, int height, int gridSize, float friction)
        {
            this.width = width;
            this.height = height;
            this.friction = friction;
            if (width % gridSize == 0 && height % gridSize == 0)
            {
                this.gridSize = gridSize;
            }
            else
            {
                throw new Exception("width or height not divisible by gridsize");
            }

            physicsObjects = new List<PhysicsObject>();
            grid = new List<PhysicsObject>[height/gridSize][];
            for (int i = 0; i < height / gridSize; i++)
            {
                grid[i] = new List<PhysicsObject>[width / gridSize];
                for (int j = 0; j < width / gridSize; j++)
                {
                    grid[i][j] = new List<PhysicsObject>();
                }
            }
        }

        public void UpdateObjects (GameTime gameTime)
        {
            //Update objects
            foreach (PhysicsObject physicsObject in physicsObjects)
            {
                physicsObject.UpdatePosition(gameTime, friction, gravity);
                physicsObject.CollisionIds.Clear();
                if (physicsObject.GetPhysicsObjectType() == PhysicsObjectType.Circle)
                {
                    Circle c = (Circle)physicsObject;
                    if (c.Position.X + c.Radius > width)
                    {
                        physicsObject.Position = new Vector2(width - c.Radius, physicsObject.Position.Y);
                        physicsObject.Velocity = new Vector2(-physicsObject.Velocity.X, physicsObject.Velocity.Y);
                    }
                    if (c.Position.X - c.Radius < 0)
                    {
                        physicsObject.Position = new Vector2(c.Radius, physicsObject.Position.Y);
                        physicsObject.Velocity = new Vector2(-physicsObject.Velocity.X, physicsObject.Velocity.Y);
                    }
                    if (c.Position.Y + c.Radius > height)
                    {
                        physicsObject.Position = new Vector2(physicsObject.Position.X, height - c.Radius);
                        physicsObject.Velocity = new Vector2(physicsObject.Velocity.X, -physicsObject.Velocity.Y);
                    }
                    if (c.Position.Y - c.Radius < 0)
                    {
                        physicsObject.Position = new Vector2(physicsObject.Position.X, c.Radius);
                        physicsObject.Velocity = new Vector2(physicsObject.Velocity.X, -physicsObject.Velocity.Y);
                    }

                    //Place objects in grid
                    int[] rows = { (int)((c.Position.Y - c.Radius) / gridSize), MathHelper.Min((int)((c.Position.Y + c.Radius) / gridSize), height / gridSize - 1) };
                    int[] cols = { (int)((c.Position.Y - c.Radius) / gridSize), MathHelper.Min((int)((c.Position.Y + c.Radius) / gridSize), width / gridSize - 1) };

                    for (int i = rows[0]; i <= rows[1]; i++)
                    {
                        for (int j = cols[0]; j <= cols[1]; j++)
                        {
                            grid[i][j].Add(physicsObject);
                        }
                    }
                }
            }
            
            //Check for collisions
            foreach (List<PhysicsObject>[] row in grid)
            {
                foreach (List<PhysicsObject> cell in row)
                {
                    if (cell.Count > 1)
                    {
                        for (int i = 0; i < cell.Count; i++)
                        {
                            for (int j = i+1; j < cell.Count; j++)
                            {
                                if (cell[i].CollisionIds.Contains(cell[j].Id)) continue;

                                //Handle collision for 2 circles
                                if (cell[i].GetPhysicsObjectType() == PhysicsObjectType.Circle && cell[j].GetPhysicsObjectType() == PhysicsObjectType.Circle)
                                {
                                    Circle c1 = (Circle)cell[i], c2 = (Circle)cell[j];
                                    if ((c1.Position - c2.Position).Length() < c1.Radius + c2.Radius)
                                    {
                                        Vector2 angle = c1.Position - c2.Position;
                                        angle.Normalize();
                                        Vector2 midpoint = (c1.Position - angle * c1.Radius + c2.Position + angle * c2.Radius) / 2;
                                        float impulse = (angle.X * (c2.Velocity.X - c1.Velocity.X) + angle.Y * (c2.Velocity.Y - c1.Velocity.Y)) / (1f / (2 * c1.Mass) + 1f / (2 * c2.Mass));
                                        cell[i].Position = midpoint + angle * c1.Radius;
                                        cell[j].Position = midpoint - angle * c2.Radius;
                                        cell[i].Velocity += angle * (impulse / c1.Mass);
                                        cell[j].Velocity -= angle * (impulse / c2.Mass);
                                        cell[i].CollisionIds.Add(cell[j].Id);
                                        cell[j].CollisionIds.Add(cell[i].Id);
                                    }
                                }
                            }
                        }
                    }
                    //Clear used cell
                    cell.Clear();
                }
            }
            
            /*
            for (int i = 0; i < physicsObjects.Count; i++)
            {
                for (int j = i+1; j < physicsObjects.Count; j++)
                {
                    if (physicsObjects[i].CollisionIds.Contains(physicsObjects[j].Id)) continue;

                    //Handle collision for 2 circles
                    if (physicsObjects[i].GetPhysicsObjectType() == PhysicsObjectType.Circle && physicsObjects[j].GetPhysicsObjectType() == PhysicsObjectType.Circle)
                    {
                        if ((physicsObjects[i].Position - physicsObjects[j].Position).Length() < ((Circle)physicsObjects[i]).Radius + ((Circle)physicsObjects[j]).Radius)
                        {
                            Vector2 angle = physicsObjects[i].Position - physicsObjects[j].Position, midpoint = (physicsObjects[i].Position + physicsObjects[j].Position) / 2; ;
                            angle.Normalize();
                            float impulse = (angle.X * (physicsObjects[j].Velocity.X - physicsObjects[i].Velocity.X) + angle.Y * (physicsObjects[j].Velocity.Y - physicsObjects[i].Velocity.Y)) / (1f / (2 * physicsObjects[i].Mass) + 1f / (2 * physicsObjects[i].Mass));
                            physicsObjects[i].Position = midpoint + angle * ((Circle)physicsObjects[i]).Radius;
                            physicsObjects[j].Position = midpoint - angle * ((Circle)physicsObjects[j]).Radius;
                            physicsObjects[i].Velocity += angle * (impulse / physicsObjects[i].Mass);
                            physicsObjects[j].Velocity -= angle * (impulse / physicsObjects[j].Mass);
                            physicsObjects[i].CollisionIds.Add(physicsObjects[j].Id);
                            physicsObjects[j].CollisionIds.Add(physicsObjects[i].Id);
                        }
                    }
                }
            }
            */
        }

        public List<PhysicsObject> PhysicsObjects { get => physicsObjects; set => physicsObjects = value; }
    }
}
