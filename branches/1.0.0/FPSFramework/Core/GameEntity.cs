#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
//Xna
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//3rd part
using BoxCollider;
//We
using FPSFramework.AI;
using FPSFramework.Logic;
#endregion

namespace FPSFramework.Core
{
    public class GameEntityList : Dictionary<String, GameEntity> { }


    public abstract class GameEntity 
    {
        public Vector3 velocity;       // current velocity vector used only by gravity

        public float gravity = 980.0f;          // gravity intensity

        public bool on_ground = false;         // is player on ground (false if in air)

        public float auto_move_y;      // distance to move in Y axis on next update in order to climb up/down a step

        /// <summary>
        /// Position of 3D Model in scene
        /// </summary>
        private Vector3 position;

        /// <summary>
        /// Collision box for collision detection
        /// </summary>
        private CollisionBox box;

        /// <summary>
        /// Model mesh reference
        /// </summary>
        private ModelMesh modelMesh;

        /// <summary>
        /// Matrix
        /// </summary>
        private Matrix transform;

        #region Properties
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public CollisionBox Box
        {
            get { return this.box; }
            set { this.box = value; }
        }

        public ModelMesh ModelMesh
        {
            get { return this.modelMesh; }
            set 
            { 
                this.modelMesh = value;

                if (value != null)
                {
                    this.position = modelMesh.BoundingSphere.Center;
                    DefineCollisionBox();
                }                
            }
        }

        public Matrix Matrix
        {
            get { return this.transform; }
            set { this.transform = value; }
        }
        #endregion

        protected GameEntity()
        {
            this.position = Vector3.Zero;
            this.box = null;
            this.transform = Matrix.Identity;
        }

        protected GameEntity(ModelMesh modelMesh)
        {
            if (modelMesh != null)
            {
                this.position = modelMesh.BoundingSphere.Center;
            }

            this.modelMesh = modelMesh;
            this.transform = Matrix.Identity;
            this.DefineCollisionBox();
        }

        /// <summary>
        /// Contains default code to update game logic about this game entity
        /// </summary>
        public virtual void Update(GameTime gameTime, CollisionMesh collision_mesh)
        {
            float time_seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (on_ground == false)
            {
                velocity.Y -= gravity * time_seconds;
            }
            else
            {
                velocity.Y = 0.0f;                
            }

            /*
            Vector3 axis_x = new Vector3(transform.M11, transform.M12, transform.M13);
            Vector3 axis_y = new Vector3(0, 1, 0);
            Vector3 axis_z = new Vector3(transform.M31, transform.M32, transform.M33);
            */

            Vector3 position = transform.Translation;
            Vector3 new_position = position;

            new_position += velocity * time_seconds;

            float move_y = 12.5f * time_seconds;           

            if (auto_move_y >= 0)
            {
                if (move_y > auto_move_y)
                {
                    move_y = auto_move_y;
                }
            }
            else
            {
                move_y = -move_y;

                if (move_y < auto_move_y)
                {
                    move_y = auto_move_y;
                }
            }

            new_position.Y += move_y;
            auto_move_y = 0;

            collision_mesh.BoxMove(box, position, new_position, 1.0f, 0.0f, 3, out new_position);
            
            if ((Math.Abs(new_position.Y - position.Y) < 0.0001f) && (velocity.Y > 0.0f))
            {
                velocity.Y = 0.0f;
            }

            float dist;
            Vector3 pos, norm;

            if (velocity.Y < 0)
            {
                if (true == collision_mesh.BoxIntersect(box,
                                    new_position,
                                    new_position + new Vector3(0, -2, 0),
                                    out dist, out pos, out norm))
                {
                    if (norm.Y > 0.70710678f)
                    {
                        on_ground = true;
                        auto_move_y = dist;
                    }
                    else
                    {
                        on_ground = false;
                    }
                }
                else
                {
                    on_ground = false;
                }
            }

            transform.Translation = new_position;
        }

        /// <summary>
        /// Contains default code to draw this game entity
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
            if ((SystemResources.Device != null) && (SystemResources.Device.IsDisposed == false))
            {
                this.box.Draw(SystemResources.Device);
            }
        }

        /// <summary>
        /// AI Messaging system
        /// </summary>
        /// <param name="message">Message</param>
        public virtual void ReceiveMessage(GameEntityMessage message)
        {
        }

        /// <summary>
        /// Defines a CollisionBox for a model mesh
        /// </summary>
        /// <param name="mesh">Model mesh</param>
        /// <returns></returns>        
        public virtual void DefineCollisionBox()
        {
            if (this.modelMesh == null)
                return;

            ModelMesh mesh = this.modelMesh;

            Vector3[] vertices = null;

            int nv = mesh.VertexBuffer.SizeInBytes / mesh.MeshParts[0].VertexStride;
            vertices = new Vector3[nv];

            if (mesh.MeshParts[0].VertexStride == 16)
            {
                VertexPositionColor[] mesh_vertices = new VertexPositionColor[nv];
                mesh.VertexBuffer.GetData<VertexPositionColor>(mesh_vertices);

                for (int i = 0; i < nv; i++)
                    vertices[i] = mesh_vertices[i].Position;
            }

            if (mesh.MeshParts[0].VertexStride == 20)
            {
                VertexPositionTexture[] mesh_vertices = new VertexPositionTexture[nv];
                mesh.VertexBuffer.GetData<VertexPositionTexture>(mesh_vertices);

                for (int i = 0; i < nv; i++)
                    vertices[i] = mesh_vertices[i].Position;
            }
            else if (mesh.MeshParts[0].VertexStride == 24)
            {
                VertexPositionColorTexture[] mesh_vertices = new VertexPositionColorTexture[nv];
                mesh.VertexBuffer.GetData<VertexPositionColorTexture>(mesh_vertices);

                for (int i = 0; i < nv; i++)
                    vertices[i] = mesh_vertices[i].Position;
            }
            else if (mesh.MeshParts[0].VertexStride == 32)
            {
                VertexPositionNormalTexture[] mesh_vertices = new VertexPositionNormalTexture[nv];
                mesh.VertexBuffer.GetData<VertexPositionNormalTexture>(mesh_vertices);

                for (int i = 0; i < nv; i++)
                    vertices[i] = mesh_vertices[i].Position;
            }

            //Defining collisionbox...
            CollisionBox box = new CollisionBox(float.MaxValue, -float.MaxValue);

            for (int i = 0; i < nv; i++)
                box.AddPoint(vertices[i]);

            this.box = box;
        }
    }
}
