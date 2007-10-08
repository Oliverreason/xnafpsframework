#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
//Xna
using Microsoft.Xna.Framework;
//We
using FPSFramework.Core;
#endregion


namespace FPSFramework.AI
{
    public class StateMachine
    {
        /// <summary>
        /// Current state of this FSM
        /// Can be null
        /// </summary>
        private IGameState currentState = null;

        /// <summary>
        /// Constructor of a new FSM
        /// </summary>
        public StateMachine()
        {
        }

        /// <summary>
        /// Updates the current state of this FSM
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public void Update(GameTime gameTime)
        {
            if (this.currentState != null)
            {
                //this.currentState.Update(gameTime);
            }
        }

        /// <summary>
        /// Changes the current state of this FSM to a new state
        /// </summary>
        /// <param name="newState">New desired state</param>
        public void ChangeState(IGameState newState)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }

            currentState = newState;

            if (newState != null)
            {
                currentState.Enter();
            }
        }
    }
}
