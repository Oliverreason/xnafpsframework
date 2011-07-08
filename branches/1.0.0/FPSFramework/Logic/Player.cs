#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
//Xna
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
//3rd part
using BoxCollider;
//We
using FPSFramework.Core;
#endregion

namespace FPSFramework.Logic
{
    public class Player : GameLiveEntity, IAttachable
    {
        /// <summary>
        /// List of arms of player
        /// </summary>
        private List<Gun> arms;

        /// <summary>
        /// Actual gun
        /// </summary>
        private Gun actualGun = null;

        /// <summary>
        /// Camera for the player
        /// </summary>
        private CollisionCameraPerson camera;

        /// <summary>
        /// Player width
        /// </summary>
        private float width = 0.0f;

        /// <summary>
        /// Player height
        /// </summary>
        private float height = 0.0f;

#region Getters/Setters
        public List<Gun> Arms
        {
            get { return this.arms; }
        }

        public Gun ActualGun
        {
            get { return this.actualGun; }
            set { this.actualGun = value; }
        }

        public CollisionCameraPerson Camera
        {
            get { return this.camera; }
        }

        public float Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        public float Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
#endregion

        #region Ctor
        public Player()
            : base()
        {
            this.arms = new List<Gun>(10);
        }

        public Player(int health, int lives)
            : base(health, lives)
        {
            this.arms = new List<Gun>(10);
        }

        public Player(int health, int lives, ref CollisionCameraPerson camera)
            : base(health, lives)
        {
            this.camera = camera;
            this.arms = new List<Gun>(10);
        }
        #endregion

        
        #region Methods
        /// <summary>
        /// Add a new gun to the list of arms
        /// </summary>
        /// <param name="g">IT MUST HAVE TO BE A GUN!!!</param>
        public void Add(GameObjectEntity goe)
        {
            if (goe is Gun)
            {
                Gun g = (Gun)goe;
                arms.Add(g);

                if (this.actualGun == null)
                    this.actualGun = g;
            }
            else
            {
                Log.Write("Erro: Player.Add esperava um objeto do tipo 'Gun'");
            }
        }

        /// <summary>
        /// Clear list of arms
        /// </summary>
        public void ClearAll()
        {
            arms.Clear();
        }

        public void AddBulletsToGun(GunType gunType, int numberOfBullets)
        {
            foreach (Gun g in this.arms)
            {
                if (g.GunType == gunType)
                {
                    g.NumberOfBullets += numberOfBullets;
                    break;
                }
            }
        }

        public override void Update(GameTime gameTime, CollisionMesh collision_mesh)
        {
            this.Position = this.camera.transform.Translation;

            if (this.actualGun != null)
            {
                this.ActualGun.Update(gameTime, collision_mesh);
            }
        }

        #endregion
    }
}
