#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
//We
using FPSFramework;
using FPSFramework.Core;
#endregion

namespace FPSFramework.Logic
{
    public class AmmoPack : GameObjectEntity, ICatchable
    {
        #region Fields
        /// <summary>
        /// Quantity of bullets there's inside medpack
        /// </summary>
        private int quantity;

        /// <summary>
        /// Type of gun that bullets are for
        /// </summary>
        private GunType gunType;
        #endregion

        #region Ctor
        public AmmoPack()
            : base()
        {
            this.quantity = 0;
            this.gunType = GunType.None;
        }
        #endregion

        #region Getters/Setters
        public int Quantity
        {
            get { return this.quantity; }
            set { this.quantity = value; }
        }

        public GunType GunType
        {
            get { return this.gunType;  }
            set { this.gunType = value;  }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Attach this object property to Player
        /// </summary>
        /// <param name="player">Catchers object</param>
        public void Attach(Player player)
        {
            player.AddBulletsToGun( this.GunType, this.quantity );
        }
        #endregion
    }
}
