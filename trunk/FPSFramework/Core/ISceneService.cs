#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
//Xna
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//3rd part
using BoxCollider;
//We
using FPSFramework.Core;
using FPSFramework.Logic;
#endregion

namespace FPSFramework.Core
{
    public interface ISceneService
    {
        GraphicsDevice Device
        {
            get;
        }

        XmlDocument XmlDocument
        {
            get;
        }

        /// Scene management
        CollisionMesh CollisionMesh
        {
            get;
        }

        Model SceneModel
        {
            get;
        }

        Player Player
        {
            get;
        }

        CollisionCameraPerson Camera
        {
            get;
        }

        GameEntityList SceneObjects
        {
            get;
        }
    }
}