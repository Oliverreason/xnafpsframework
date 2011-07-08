#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
//Xna
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//3rd part
using BoxCollider;
using Xclna.Xna.Animation;
//We
using FPSFramework.Logic;
#endregion

namespace FPSFramework.Core
{
    public static class XmlLoaderHelper
    {
        static XmlLoaderHelper()
        {
        }

        /// <summary>
        /// Gets players attibutes from XML document
        /// </summary>
        /// <param name="xmlDoc"></param>
        public static void LoadPlayerAttributes(XmlDocument xmlDoc, float aspectRatio, 
                                                ref CollisionCameraPerson camera, ref Player player)
        {
            Debug.Assert(xmlDoc != null);

            //Searchs by a "player" node in XML
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("player");

            Debug.Assert(nodeList.Count != 0);

            XmlElement nodeElement = (XmlElement)nodeList.Item(0);
            Single[] playerAttributes = new Single[10];

            try
            {
                //Get player settings
                playerAttributes[0] = Convert.ToSingle(nodeElement.GetAttribute("positionX"));
                playerAttributes[1] = Convert.ToSingle(nodeElement.GetAttribute("positionY"));
                playerAttributes[2] = Convert.ToSingle(nodeElement.GetAttribute("positionZ"));
                playerAttributes[3] = Convert.ToSingle(nodeElement.GetAttribute("width"));
                playerAttributes[4] = Convert.ToSingle(nodeElement.GetAttribute("height"));
                playerAttributes[5] = Convert.ToSingle(nodeElement.GetAttribute("step_height"));
                playerAttributes[6] = Convert.ToSingle(nodeElement.GetAttribute("head_height"));
                playerAttributes[7] = Convert.ToSingle(nodeElement.GetAttribute("up_and_rotation"));
                playerAttributes[8] = Convert.ToSingle(nodeElement.GetAttribute("gravity"));
                playerAttributes[9] = Convert.ToSingle(nodeElement.GetAttribute("jump_height"));
            }
            catch (InvalidCastException e)
            {
                Log.Write("Scene constructor: InvalidCastExpcetion raised (did you forget some player setting?)");
                Log.Write(e.Message);
            }

            Vector3 initialPosition = new Vector3(playerAttributes[0], playerAttributes[1], playerAttributes[2]);

            camera = new CollisionCameraPerson(initialPosition,  //position
                                                Vector3.Left, //lookat
                                                MathHelper.ToRadians(60), //field of vision
                                                aspectRatio, //aspect ratio
                                                playerAttributes[3], //width default
                                                playerAttributes[4], //height 
                                                playerAttributes[5], //step height
                                                playerAttributes[6], //head height
                                                playerAttributes[7], //up and down rotation
                                                playerAttributes[8], //gravity
                                                playerAttributes[9]); //jump height

            player = new Player(100,3, ref camera);
            player.Width = playerAttributes[3];
            player.Height = playerAttributes[4];
        }
        
        /// <summary>
        /// Get med packs attributes
        /// </summary>
        /// <param name="xmlDoc">XML document that contains the attributes</param>
        public static void LoadMedPacksAttributes(XmlDocument xmlDoc, ref GameEntityList sceneObjects)
        {
            Debug.Assert(xmlDoc != null);
            Debug.Assert(sceneObjects != null);

            //try to get medpack list
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("medpacks");

            if (nodeList.Count > 0)
            {
                XmlElement nodeElement = (XmlElement)nodeList.Item(0);

                foreach (XmlElement xmle in nodeElement.ChildNodes)
                {
                    MedPack mp = new MedPack();
                    String assetName = null;

                    try
                    {
                        assetName = Convert.ToString(xmle.GetAttribute("name"));
                        mp.Quantity = Convert.ToInt32(xmle.GetAttribute("quantity"));
                    }
                    catch (InvalidCastException e)
                    {
                        Log.Write("BuildObjectsList: InvalidCastExpcetion raised (did you forget some medpack setting?)");
                        Log.Write(e.Message);
                    }
                    
                    sceneObjects.Add(assetName, mp);
                }
            }
        }

        /// <summary>
        /// Get ammo packs attributes
        /// </summary>
        /// <param name="xmlDoc">XML document that contains the attributes</param>
        public static void LoadAmmoPacksAttributes(XmlDocument xmlDoc, ref GameEntityList sceneObjects)
        {
            Debug.Assert(xmlDoc != null);
            Debug.Assert(sceneObjects != null);

            //try to get ammopack list
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("ammopacks");

            if (nodeList.Count > 0) //if XML has ammopacks...
            {
                //get list of ammopacks in scene
                XmlElement nodeElement = (XmlElement)nodeList.Item(0);

                //List available gun types in project
                List<String> guntypeNames = new List<String>(Enum.GetNames(typeof(GunType)));
                Int32[] guntypeValues = (Int32[])Enum.GetValues(typeof(GunType));

                //get all ammopacks definitions from XML file
                foreach (XmlElement xmle in nodeElement.ChildNodes)
                {
                    AmmoPack ap = new AmmoPack();
                    String assetName = null;

                    try
                    {
                        assetName = Convert.ToString(xmle.GetAttribute("name"));
                        Int32 position = guntypeNames.IndexOf(xmle.GetAttribute("guntype"));

                        ap.Quantity = Convert.ToInt32(xmle.GetAttribute("quantity"));
                        ap.GunType = (GunType)guntypeValues[position];
                    }
                    catch (FormatException fe)
                    {
                        Log.Write("BuildObjectsList: FormatExpcetion raised");
                        Log.Write(fe.Message);
                    }
                    catch (IndexOutOfRangeException ioore)
                    {
                        Log.Write("BuildObjectsList: IndexOutOfRangeException raised");
                        Log.Write(ioore.Message);
                    }
                    catch (InvalidCastException ice)
                    {
                        Log.Write("BuildObjectsList: InvalidCastExpcetion raised (did you forget some ammopack setting?)");
                        Log.Write(ice.Message);
                    }

                    sceneObjects.Add(assetName, ap);
                }
            }
        }

        /// <summary>
        /// Get ammo packs attributes
        /// </summary>
        /// <param name="xmlDoc">XML document that contains the attributes</param>
        public static void LoadGunsAttributes(XmlDocument xmlDoc, ref GameEntityList sceneObjects)
        {
            Debug.Assert(xmlDoc != null);
            Debug.Assert(sceneObjects != null);

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("guns");

            if (nodeList.Count > 0) //if XML has ammopacks...
            {
                //get list of guns in scene
                XmlElement nodeElement = (XmlElement)nodeList.Item(0);

                //List available gun types in project
                List<String> guntypeNames = new List<String>(Enum.GetNames(typeof(GunType)));
                Int32[] guntypeValues = (Int32[])Enum.GetValues(typeof(GunType));

                //get all guns definitions from XML file
                foreach (XmlElement xmle in nodeElement.ChildNodes)
                {                    
                    String assetName = null;
                    Gun g = new Gun();

                    try
                    {
                        assetName = Convert.ToString(xmle.GetAttribute("name"));                        
                        g.NumberOfBullets = Convert.ToInt32(xmle.GetAttribute("bullets"));                       

                        //traduz o string em um tipo enumerado
                        Int32 position = guntypeNames.IndexOf(xmle.GetAttribute("guntype"));                        
                        g.GunType = (GunType)guntypeValues[position];
                        g.IsRotatable = Convert.ToBoolean(xmle.GetAttribute("isRotatable"));

                        if (g.IsRotatable == true)
                            g.AngleOffset = Convert.ToSingle(xmle.GetAttribute("angleOffset"));

                        String spriteName = Convert.ToString(xmle.GetAttribute("spriteAssetName"));
                        String bulletAssetName = Convert.ToString(xmle.GetAttribute("bulletAssetName"));
                        Model bulletModel = SystemResources.Content.Load<Model>(@bulletAssetName);
                        int bulletLifeTime = Convert.ToInt32(xmle.GetAttribute("bulletLifeTime"));

                        g.Sprite = SystemResources.Content.Load<Texture2D>(@spriteName);
                        g.Bullet = new Bullet(bulletLifeTime, bulletModel.Meshes[0]);
                        g.Bullet.Damage = Convert.ToInt32(xmle.GetAttribute("bulletDamage"));
                        g.Bullet.BulletModel = bulletModel;
                    }
                    catch (InvalidCastException e)
                    {
                        Log.Write("BuildObjectsList: InvalidCastExpcetion raised (did you forget some ammopack setting?)");
                        Log.Write(e.Message);
                    }

                    sceneObjects.Add(assetName, g);
                }
            }
        }

        /// <summary>
        /// Get enemies attributes
        /// </summary>
        /// <param name="xmlDoc">XML doc with attributes</param>
        /// <param name="enemies">enemies list</param>
        public static void LoadEnemiesAttributes(Game game, XmlDocument xmlDoc, ref EnemiesList enemies)
        {
            Debug.Assert(xmlDoc != null);
            Debug.Assert(enemies != null);
            Debug.Assert(game != null);

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("enemies");

            if (nodeList.Count > 0) //if XML has ammopacks...
            {
                //get list of guns in scene
                XmlElement nodeElement = (XmlElement)nodeList.Item(0);
                
                //get all guns definitions from XML file
                foreach (XmlElement xmle in nodeElement.ChildNodes)
                {
                    String assetName = null;
                    String[] statesNames = new String[5];
                    Enemy e = new Enemy();

                    try
                    {
                        String modelName = Convert.ToString(xmle.GetAttribute("assetName"));
                        Model m = SystemResources.Content.Load<Model>(@modelName);

                        assetName = Convert.ToString(xmle.GetAttribute("name"));                                             
                        e.ModelAnimator = new ModelAnimator(game, m);
                        e.ScaleFactor = Convert.ToSingle(xmle.GetAttribute("scaleFactor"));
                        e.RunSpeed = Convert.ToSingle(xmle.GetAttribute("runSpeed"));
                        e.WalkSpeed = Convert.ToSingle(xmle.GetAttribute("walkSpeed"));
                        e.Health = Convert.ToInt32(xmle.GetAttribute("health"));
                        int seconds = Convert.ToInt32(xmle.GetAttribute("dieTime"));
                        e.TotalDieTime = new TimeSpan(0, 0, seconds);

                        game.Components.Remove(e.ModelAnimator); //We manage update and call

                        statesNames[0] = Convert.ToString(xmle.GetAttribute("idleState"));
                        statesNames[1] = Convert.ToString(xmle.GetAttribute("walkState"));
                        statesNames[2] = Convert.ToString(xmle.GetAttribute("runState"));
                        statesNames[3] = Convert.ToString(xmle.GetAttribute("attackState"));
                        statesNames[4] = Convert.ToString(xmle.GetAttribute("dieState"));

                        e.IdleController = new AnimationController(game, e.ModelAnimator.Animations[statesNames[0]]);
                        e.WalkController = new AnimationController(game, e.ModelAnimator.Animations[statesNames[1]]);
                        e.RunController = new AnimationController(game, e.ModelAnimator.Animations[statesNames[2]]);
                        e.AttackController = new AnimationController(game, e.ModelAnimator.Animations[statesNames[3]]);
                        e.DieController = new AnimationController(game, e.ModelAnimator.Animations[statesNames[4]]);                        
                    }
                    catch (InvalidCastException ice)
                    {
                        Log.Write("BuildObjectsList: InvalidCastExpcetion raised (did you forget some enemy setting?)");
                        Log.Write(ice.Message);
                    }

                    enemies.Add(assetName, e);
                }
            }
        }

    }
}

