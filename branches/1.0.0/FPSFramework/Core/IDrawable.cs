#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace FPSFramework.Core
{
    public interface IDrawable : Microsoft.Xna.Framework.IDrawable
    {
        void Draw(GraphicsDevice graphics);


    }
}
