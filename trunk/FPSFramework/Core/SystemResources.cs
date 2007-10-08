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
        /// Indicates if all XNA System Resources are registered
        /// </summary>
        private static bool isRegistered = false;
        
        /// <summary>
        /// Content reference used by all classes of FPS Framework
        /// </summary>
        private static ContentManager content = null;

#region Properties
        public static bool IsRegistered
        {
            get { return isRegistered; }
            set 
            {
                if (value == true)
                {
                    if (content != null)
                    {
                        isRegistered = value;
                    }
                }
                else
                {
                    isRegistered = value;
                }
            }
        }

        public static ContentManager Content
        {
            get { return (isRegistered) ? content : null; }
            set { if (value != null) content = value; }
        }
#endregion

        public static bool Register( ref ContentManager content )
        {
            bool bInitialChecking = (content != null);

            if (bInitialChecking)
            {                
                Content = content;

                IsRegistered = bInitialChecking;
                return bInitialChecking;
            }

            return bInitialChecking;
        }
    }
}
