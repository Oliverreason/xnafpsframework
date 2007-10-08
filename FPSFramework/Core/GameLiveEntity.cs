#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace FPSFramework.Core
{
    public abstract class GameLiveEntity : GameEntity
    {
        #region Fields
        /// <summary>
        /// Quantity of GameLivesEntity's health (can be a player, enemy)
        /// </summary>
        private int health;

        /// <summary>
        /// Quantity of lives
        /// </summary>
        private int lives;
        #endregion

        #region Ctor
        protected GameLiveEntity() : base()
        {
            this.health = 100;
            this.lives = 3;
        }

        protected GameLiveEntity(int health, int lives)
        {
            this.health = health;
            this.lives = lives;
        }
        #endregion

        #region Getters/Setters
        public int Lives
        {
            get { return lives; }
            set { if (value >= 0) lives = value; }
        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// As described by name, decrease the number of lives from GameLiveEntity
        /// </summary>
        public void decreaseLives()
        {
            if (this.lives > 0)
            {
                this.lives--;
            }
        }
        #endregion
    }
}
