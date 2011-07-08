#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
//Xna
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//We
using FPSFramework;
using FPSFramework.Core;
//3rd Part
using BoxCollider;
#endregion

namespace FPSFramework.Logic
{
    public class BulletList : List<Bullet> { }

    public class Bullet : GameObjectEntity
    {
        /// <summary>
        /// Maximum life time of bullet
        /// </summary>
        private int lifeTime;

        /// <summary>
        /// Elapsed time since creation of bullet
        /// </summary>
        private int elapsedTime;

        /// <summary>
        /// Indicates if this bullet is dead or not and
        /// can be deleted
        /// </summary>
        private bool dead = false;

        private int damage = 0;

        Vector3 position;

        Vector3 speed;

        Model model;

#region Properties
        public int LifeTime
        {
            get { return this.lifeTime; }
            set { this.lifeTime = value; }
        }

        public int ElapsedTime
        {
            get { return this.elapsedTime; }
            set { this.elapsedTime = value; }
        }

        public bool Dead
        {
            set { this.dead = value; }
            get { return this.dead; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public int Damage
        {
            get { return damage; }
            set { this.damage = value; }
        }

        public Model BulletModel
        {
            get { return model; }
            set { this.model = value; }
        }
#endregion

#region Ctor
        public Bullet()
            : base()
        {
        }

        public Bullet(int lifeTime) : 
            base()
        {
            this.lifeTime = lifeTime;
        }

        public Bullet(int lifeTime, ModelMesh model)
            : base(model)
        {
            this.lifeTime = lifeTime;
        }

        public Bullet(Bullet b)
        {
            if (b != null)
            {
                this.lifeTime = b.LifeTime;
                this.ModelMesh = b.ModelMesh;
                this.model = b.model;
                this.damage = b.damage;
            }
        }
#endregion

        public void Draw(GameTime gameTime, CollisionCamera camera)
        {
            GraphicsDevice gd = SystemResources.Device;

            Matrix[] transforms = new Matrix[this.model.Bones.Count];
            
            this.model.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix transform = Matrix.CreateTranslation(this.position);

            foreach (ModelMesh mesh in model.Meshes)
            {
                Matrix world = transform * transforms[mesh.ParentBone.Index];

                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = camera.view;

                    effect.Projection = camera.projection;

                    effect.World = world;

                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime, CollisionMesh collision_mesh)
        {
            this.elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            this.dead = (this.elapsedTime >= this.lifeTime);

            this.Matrix = Matrix.CreateTranslation(this.Position);
            base.Update(gameTime, collision_mesh);
        }

    }
}
