#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace FPSFramework.Core
{
    public static class SystemResources
    {       
        /// <summary>
        /// Content reference used by all classes of FPS Framework
        /// </summary>
        public static ContentManager Content = null;

        public static SpriteBatch Batch = null;   

        public static GraphicsDevice Device = null;

        public static void Register( ref GraphicsDevice device, ref SpriteBatch sb, ref ContentManager content )
        {
            Device = device;
            Batch = sb;
            Content = content;
        }
    }
}
