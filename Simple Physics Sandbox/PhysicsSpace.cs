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
            //Clear current grid
            foreach (List<PhysicsObject>[] row in grid)
            {
                foreach (List<PhysicsObject> cell in row)
                {
                    cell.Clear();
                }
            }

            //Update objects
            foreach (PhysicsObject physicsObject in physicsObjects)
            {
                physicsObject.UpdatePosition(gameTime, friction, gravity);
                physicsObject.CollisionIds.Clear();
                if (physicsObject.GetPhysicsObjectType() == PhysicsObjectType.Circle)
                {
                    Circle c = (Circle)physicsObject;
                    if (c.Position.X - c.Radius < 0)
                    {
                        physicsObject.Position = new Vector2(c.Radius, physicsObject.Position.Y);
                        physicsObject.Velocity = new Vector2(-physicsObject.Velocity.X, physicsObject.Velocity.Y);
                    }
                    else if (c.Position.X + c.Radius > width)
                    {
                        physicsObject.Position = new Vector2(width - c.Radius, physicsObject.Position.Y);
                        physicsObject.Velocity = new Vector2(-physicsObject.Velocity.X, physicsObject.Velocity.Y);
                    }
                    if (c.Position.Y - c.Radius < 0)
                    {
                        physicsObject.Position = new Vector2(physicsObject.Position.X, c.Radius);
                        physicsObject.Velocity = new Vector2(physicsObject.Velocity.X, -physicsObject.Velocity.Y);
                    }
                    else if (c.Position.Y + c.Radius > height)
                    {
                        physicsObject.Position = new Vector2(physicsObject.Position.X, height - c.Radius);
                        physicsObject.Velocity = new Vector2(physicsObject.Velocity.X, -physicsObject.Velocity.Y);
                    }

                    int[] rows = { (int)((c.Position.Y - c.Radius) / gridSize), (int)((c.Position.Y + c.Radius) / gridSize) };
                    int[] cols = { (int)((c.Position.Y - c.Radius) / gridSize), (int)((c.Position.Y + c.Radius) / gridSize) };

                    grid[rows[0]][cols[0]].Add(physicsObject);
                    if (rows[0] != rows[1] && rows[1] < height / gridSize)
                    {
                        grid[rows[1]][cols[0]].Add(physicsObject);
                        if (cols[0] != cols[1] && cols[1] < width / gridSize)
                        {
                            grid[rows[0]][cols[1]].Add(physicsObject);
                            grid[rows[1]][cols[1]].Add(physicsObject);
                        }
                    }
                    else if (cols[0] != cols[1] && cols[1] < width / gridSize)
                    {
                        grid[rows[0]][cols[1]].Add(physicsObject);
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
                                    if ((cell[i].Position - cell[j].Position).Length() < ((Circle)cell[i]).Radius + ((Circle)cell[j]).Radius)
                                    {
                                        Vector2 angle = cell[i].Position - cell[j].Position, midpoint = (cell[i].Position + cell[j].Position) / 2;
                                        angle.Normalize();
                                        float impulse = (angle.X * (cell[j].Velocity.X - cell[i].Velocity.X) + angle.Y * (cell[j].Velocity.Y - cell[i].Velocity.Y)) / (1f / (2 * cell[i].Mass) + 1f / (2 * cell[i].Mass));
                                        cell[i].Position = midpoint + angle * ((Circle)cell[i]).Radius;
                                        cell[j].Position = midpoint - angle * ((Circle)cell[j]).Radius;
                                        cell[i].Velocity += angle * (impulse / cell[i].Mass);
                                        cell[j].Velocity -= angle * (impulse / cell[j].Mass);
                                        cell[i].CollisionIds.Add(cell[j].Id);
                                        cell[j].CollisionIds.Add(cell[i].Id);
                                    }
                                }
                            }
                        }
                    }
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
