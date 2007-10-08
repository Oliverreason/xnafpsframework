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
#endregion

namespace FPSFramework.Core
{
    public class GameEntityList : Dictionary<String, GameEntity> { }


    public abstract class GameEntity 
    {
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
        private Matrix matrix;

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
            get { return this.matrix; }
            set { this.matrix = value; }
        }
        #endregion

        protected GameEntity()
        {
            this.position = Vector3.Zero;
            this.box = null;
            this.matrix = Matrix.Identity;
        }

        protected GameEntity(ModelMesh modelMesh)
        {
            if (modelMesh != null)
                this.position = modelMesh.BoundingSphere.Center;

            this.modelMesh = modelMesh;
            this.matrix = Matrix.Identity;
            this.DefineCollisionBox();
        }

        /// <summary>
        /// Contains default code to update game logic about this game entity
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Contains default code to draw this game entity
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
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
