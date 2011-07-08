#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
//Xna
using Microsoft.Xna.Framework;
using FPSFramework.Logic;
#endregion

namespace FPSFramework.AI
{
    public class IdleState : IGameState
    {
        public IdleState()
        {
        }


        public void Enter()
        {
        }

        public void Update(GameTime gameTime, Enemy e)
        {
            //e.ModelAnimator.World = Matrix.CreateScale(e.ScaleFactor) *
            //                           Matrix.CreateTranslation(e.Position);      
        }

        public void Exit()
        {
        }
    }
}
