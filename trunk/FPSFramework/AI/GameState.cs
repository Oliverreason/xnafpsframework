#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
//Xna
using Microsoft.Xna.Framework;
//We
using FPSFramework.Core;
using FPSFramework.Logic;
#endregion

namespace FPSFramework.AI
{   
    public enum GameEntityAnimationState
    {
        Idle,
        Walk,
        Run,
        Attack,
        Die
    }

    public enum GameEntityBehaviorState
    {
        Idle,
        Follow,
        Attack,
        Die
    }

    public interface IGameState
    {
        /// <summary>
        /// Executes on entering game state
        /// </summary>
        void Enter();

        /// <summary>
        /// Executes on each update
        /// </summary>
        /// <param name="gameTime"></param>
        void Update(GameTime gameTime, Enemy e);

        /// <summary>
        /// Executes on exiting game state
        /// </summary>
        void Exit();
    }
}
