using System;
using System.Collections.Generic;
using System.Text;

namespace FPSFramework.Core
{
    interface IAttachable
    {
        /// <summary>
        /// Add the object to the list of this object
        /// </summary>
        /// <param name="o"></param>
        void Add(GameObjectEntity o);

        /// <summary>
        /// Clear the list of all objects
        /// </summary>
        void ClearAll();
    }
}
