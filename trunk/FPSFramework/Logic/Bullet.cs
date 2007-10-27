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
                this.lifeTime = b.lifeTime;
                this.ModelMesh = b.ModelMesh;
            }
        }
#endregion


        public override void Draw(GameTime gameTime)
        {
            //SceneComponent.Batch.Draw(
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            this.elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            this.dead = (this.elapsedTime >= this.lifeTime);

            Vector3 pos = this.Position;
            pos.Z += 1.0f;
            this.Position = pos;

            this.Matrix = Matrix.CreateTranslation(this.Position);
            /*
            Vector3 pos = this.Position;

            pos.X += 1.0f;
            pos.Z += 1.0f;
            */
            base.Update(gameTime);
        }
    }
}
