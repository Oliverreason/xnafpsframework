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
    public class MedPack : GameObjectEntity, ICatchable
    {
        #region Fields
        /// <summary>
        /// Quantity of health there's inside medpack
        /// </summary>
        private int quantity;
        #endregion

        #region Ctor
        public MedPack()
            : base()
        {
        }
        #endregion

        #region Getters/Setters
        public int Quantity
        {
            get { return this.quantity; }
            set { this.quantity = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Attach this object property to Player
        /// </summary>
        /// <param name="player">Catchers object</param>
        public void Attach(Player player)
        {
            player.Health += quantity;
        }
        #endregion
    }
}
