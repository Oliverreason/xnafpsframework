#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
//We
using FPSFramework.Logic;
#endregion

namespace FPSFramework.Core
{
    interface ICatchable
    {
        /// <summary>
        /// Let an object be catched by another GameLiveEntity object.
        /// </summary>
        /// <param name="o">Catcher object</param>
        void Attach(Player player);
    }
}
