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
    /// <summary>
    /// Lists all gun types of game
    /// </summary>
    public enum GunType
    {
        None,
        Pistol,
        Revolver,
        Shotgun,
        MachineGun,
        SubmachineGun
    }   

    public class Gun : GameObjectEntity, ICatchable
    {
        /// <summary>
        /// As described by name, this is type of gun
        /// </summary>
        private GunType gunType;

        /// <summary>
        /// Owner
        /// </summary>
        private Player owner = null;

        /// <summary>
        /// Quantity of bullets in the gun
        /// </summary>
        private int numberOfBullets;

        /// <summary>
        /// Indicates if it was catched by the player
        /// </summary>
        private bool isCatched = false;

        /// <summary>
        /// Rotate?
        /// </summary>
        private bool isRotatable = false;

        /// <summary>
        /// Angle offset to rotate the object
        /// </summary>
        private float angleOffset;

        /// <summary>
        /// Sprite representation of object
        /// </summary>
        private Texture2D sprite;

        /// <summary>
        /// Bullet of gun
        /// </summary>
        private Bullet bullet;

#region Getters/Setters
        public int NumberOfBullets
        {
            get { return this.numberOfBullets; }
            set { this.numberOfBullets = value; }
        }

        public GunType GunType
        {
            get { return this.gunType; }
            set { this.gunType = value; }
        }

        public bool IsRotatable
        {
            get { return this.isRotatable; }
            set { this.isRotatable = value; }
        }

        public float AngleOffset
        {
            get { return this.angleOffset; }
            set { this.angleOffset = value; }
        }

        public bool IsCatched
        {
            get { return this.isCatched; }
        }

        public Player Owner
        {
            get { return this.owner; }
            set { this.owner = value; }
        }

        public Texture2D Sprite
        {
            set { this.sprite = value; }
            get { return this.sprite; }
        }

        public int BulletLifeTime
        {
            get { return this.bullet.LifeTime; }
            set { this.bullet.LifeTime = value; }
        }

        public Bullet Bullet
        {
            get { return this.bullet; }
            set { this.bullet = value; }
        }
#endregion


        public Gun()
            : base()
        {
            this.numberOfBullets = 0;
        }

        public Gun(int numberOfBullets)
            : base()
        {
            this.numberOfBullets = ((numberOfBullets >= 0) ? (numberOfBullets) : (0));
        }

        /// <summary>
        /// Add another gun to the arms list
        /// </summary>
        /// <param name="player">Jogador</param>
        public virtual void Attach(Player player)
        {
            player.Add(this);
            this.isCatched = true;
            this.owner = player;
            this.ModelMesh.Tag = null;
        }

        public virtual void Rotate(GameTime gameTime, float anAngle)
        {
            anAngle *= (float)gameTime.TotalGameTime.Ticks / 10000;

            Vector3 vec = this.Position;

            this.Matrix = Matrix.CreateTranslation(-vec) *
                          Matrix.CreateRotationY(anAngle) *
                          Matrix.CreateTranslation(vec);
        }

        public override void Update(GameTime gameTime)
        {
            if (this.isCatched == false)
            {
                if (this.isRotatable)
                {
                    this.Rotate(gameTime, this.angleOffset);
                }
            }
            else
            {
                /*
                BulletList deads = new BulletList();

                foreach (Bullet b in this.bullets)
                {
                    b.Update(gameTime);

                    if (b.Dead)
                        deads.Add(b);
                }

                foreach (Bullet b in deads)
                    this.bullets.Remove(b);

                deads.Clear();
                 */
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.isCatched == true)
            {             
                float width = SceneComponent.Width / 2;
                float height = SceneComponent.Height - this.sprite.Height;

                if (SceneComponent.Batch.IsDisposed == false)
                {
                    SceneComponent.Batch.Begin(SpriteBlendMode.AlphaBlend, 
                                                        SpriteSortMode.Deferred, 
                                                        SaveStateMode.SaveState);
                    SceneComponent.Batch.Draw(this.sprite, new Vector2(width, height), Color.White);
                    SceneComponent.Batch.End();
                }
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Emulates a bullet factory
        /// </summary>
        /// <returns>A Bullet</returns>
        public virtual Bullet Shoot()
        {
            if (this.numberOfBullets > 0)
            {
                this.numberOfBullets--;

                Bullet retBullet = new Bullet(this.bullet);
                Vector3 scale = Vector3.Zero;
                Vector3 translation = Vector3.Zero;
                Quaternion q = Quaternion.Identity;

                this.owner.Camera.world.Decompose(out scale, out q, out translation);

                retBullet.Matrix = /*Matrix.CreateRotationY(MathHelper.ToRadians(90.0f)) */ 
                            /*Matrix.CreateFromQuaternion(q) */
                            Matrix.CreateTranslation(translation);
                retBullet.Position = translation;

                return retBullet;
            }

            return null;
        }
    }
}
