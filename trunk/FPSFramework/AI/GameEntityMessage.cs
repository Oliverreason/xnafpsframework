#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
//Xna
using Microsoft.Xna.Framework;
//We
using FPSFramework;
using FPSFramework.Core;

#endregion

namespace FPSFramework.AI
{
    public enum GameEntityMessageType
    {
        None,
        Hit,
        Damage
    }

    public struct GameEntityMessage
    {
        public GameEntity sender;
        public GameEntity receiver;
        public GameEntityMessageType type;
        public GameTime timeStamp; 
    }
}
