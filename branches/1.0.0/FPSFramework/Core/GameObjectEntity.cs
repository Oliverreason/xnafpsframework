#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
//Xna
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace FPSFramework.Core
{
    /// <summary>
    /// Describes a game object entity (like guns, ammopacks, medpacks, etc.)
    /// </summary>
    public abstract class GameObjectEntity : GameEntity
    {

        protected GameObjectEntity() : base()
        {
        }

        protected GameObjectEntity(ModelMesh model) : base(model)
        {
        }

    }
}
