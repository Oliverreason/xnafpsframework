#region Dependencies
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
//XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
//3rd-part binaries
using BoxCollider;
//We
using FPSFramework.Core;
using FPSFramework.AI;
#endregion

namespace FPSFramework.Logic
{
    public class SceneComponent : DrawableGameComponent, ISceneService
    {
        ///Graphics Device Manager reference
        protected GraphicsDevice device = null;

        protected XmlDocument xmlDoc = null;

        protected static SpriteBatch sb = null;

        protected static float width = 800.0f;

        protected static float height = 600.0f;

        /// Scene management
        protected CollisionMesh collisionMesh;

        protected Model sceneModel;

        protected Player player;

        protected CollisionCameraPerson camera;

        protected GameEntityList sceneObjects;

        protected EnemiesList enemies = new EnemiesList();

        private BulletList bullets = new BulletList();        

#region Properties 
        public GraphicsDevice Device
        {
            get { return this.device; }
        }

        public XmlDocument XmlDocument
        {
            get { return this.xmlDoc; }
        }

        public static SpriteBatch Batch
        {
            get { return sb; }
        }

        public static float Width
        {
            get { return width; }
        }

        public static float Height
        {
            get { return height; }
        }

        public CollisionMesh CollisionMesh
        {
            get { return this.collisionMesh; }
        }

        public Model SceneModel
        {
            get { return this.sceneModel; }
        }

        public Player Player
        {
            get { return this.player; }
        }

        public CollisionCameraPerson Camera
        {
            get { return this.camera; }
        }

        public GameEntityList SceneObjects
        {
            get { return this.sceneObjects; }
        }
#endregion

        public SceneComponent(Game game, Model sceneModel, XmlDocument xmlDoc) : base(game)
        {
            Debug.Assert(xmlDoc != null, @"You must provide a XML configuration file for your scene");
            Debug.Assert(sceneModel != null);

            this.xmlDoc = xmlDoc;
            this.sceneModel = sceneModel;

            IGraphicsDeviceService gds = (IGraphicsDeviceService)this.Game.Services.GetService(typeof(IGraphicsDeviceService));
            this.device = gds.GraphicsDevice;

            sb = new SpriteBatch(this.device);
            width = this.device.Viewport.Width;
            height = this.device.Viewport.Height;

            this.Game.Components.Add(this);
            this.Game.Services.AddService(typeof(SceneComponent), this);

            this.Initialize();
        }

        public override void Initialize()
        {
            float aspectRatio = this.device.Viewport.Width /
                                 this.device.Viewport.Height;

            this.collisionMesh = new CollisionMesh(sceneModel, 1);
            this.sceneObjects = new GameEntityList();

            XmlLoaderHelper.LoadPlayerAttributes(xmlDoc, aspectRatio, ref this.camera, ref this.player);
            XmlLoaderHelper.LoadGunsAttributes(xmlDoc, ref this.sceneObjects);
            XmlLoaderHelper.LoadAmmoPacksAttributes(xmlDoc, ref this.sceneObjects);
            XmlLoaderHelper.LoadMedPacksAttributes(xmlDoc, ref this.sceneObjects);
            XmlLoaderHelper.LoadEnemiesAttributes(this.Game, xmlDoc, ref this.enemies);
            

            //Create collisionbox for each dynamic entity
            if (this.sceneObjects != null)
            {
                foreach (String meshName in this.sceneObjects.Keys)
                {
                    ModelMesh mesh = null;

                    this.sceneModel.Meshes.TryGetValue(meshName, out mesh);

                    if (mesh == null)
                        continue;

                    GameEntity geOut = null;

                    this.sceneObjects.TryGetValue(meshName, out geOut);

                    if (geOut != null)
                    {
                        geOut.ModelMesh = mesh;
                        mesh.Tag = new object(); //only marks it as a dynamic object
                    }
                }

                foreach (String meshName in this.enemies.Keys)
                {
                    ModelMesh mesh = null;

                    this.sceneModel.Meshes.TryGetValue(meshName, out mesh);

                    if (mesh == null)
                        continue;

                    Enemy eOut = null;

                    this.enemies.TryGetValue(meshName, out eOut);

                    if (eOut != null)
                    {
                        eOut.ModelMesh = mesh;
                        Vector3 pos = mesh.BoundingSphere.Center;
                        eOut.Position = pos;
                        mesh.Tag = null; //don't render mesh marker
                    }
                }
            }

            base.Initialize();
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);
        }

        /// <summary>
        /// Update scene logics
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            this.camera.Update(gameTime.ElapsedGameTime,
                         ref this.collisionMesh,
                         GamePad.GetState(PlayerIndex.One),
                         Keyboard.GetState());
           
            this.player.Update(gameTime, this.collisionMesh);

            GamePadState gps = GamePad.GetState(PlayerIndex.One);
            KeyboardState kbs = Keyboard.GetState();

            if (gps.Buttons.Y == ButtonState.Pressed || kbs.IsKeyDown(Keys.Enter))
            {
                if (this.player.ActualGun != null)
                {
                    Bullet b = this.player.ActualGun.Shot();
                    this.bullets.Add(b);
                }
            }

            this.CheckCollisions(gameTime);
            
            foreach (Enemy e in this.enemies.Values)
            {
                e.Update(gameTime, this.camera.world.Translation, ref this.camera.view, 
                            ref this.camera.projection, ref this.collisionMesh);               
            }

            ///Manages bullets
            BulletList deadBullets = new BulletList();

            foreach (Bullet b in this.bullets)
            {
                if (b.Dead)
                {
                    deadBullets.Add(b);
                }
                else
                {
                    b.Update(gameTime, this.collisionMesh);
                }
            }

            foreach (Bullet b in deadBullets)
            {
                this.bullets.Remove(b);
            }
        }

        /// <summary>
        /// Draw all scene
        /// </summary>
        /// <param name="gameTime">Game Time</param>
        public override void Draw(GameTime gameTime)
        {
            Matrix[] transforms = new Matrix[this.sceneModel.Bones.Count];
            Matrix worldMatrix = Matrix.Identity;
            GameEntity ge = null;

            this.sceneModel.CopyAbsoluteBoneTransformsTo(transforms);         

            foreach (ModelMesh mesh in this.sceneModel.Meshes)
            {                
                if (this.collisionMesh.IsDynamicEntity(mesh))
                {
                    if (mesh.Tag != null) //if is visible
                    {                                                
                        this.sceneObjects.TryGetValue(mesh.Name, out ge);

                        if (ge != null)
                        {
                            worldMatrix = ge.Matrix;
                            ge.Draw(gameTime);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                worldMatrix = worldMatrix * transforms[mesh.ParentBone.Index];

                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.World = worldMatrix; 
                    effect.View = this.camera.view;
                    effect.Projection = this.camera.projection;                    
                }

                mesh.Draw();                
            }
            
            foreach (Enemy e in this.enemies.Values)
            {
                e.Draw(gameTime);
            }

            foreach (Bullet b in this.bullets)
            {
                worldMatrix = b.Matrix;

                foreach (BasicEffect effect in b.ModelMesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.World = worldMatrix * Matrix.CreateScale(20.0f); 
                    effect.View = this.camera.view;
                    effect.Projection = this.camera.projection;                    
                }

                b.ModelMesh.Draw();
            }
            
            if (this.player.ActualGun != null)
                this.player.ActualGun.Draw(gameTime);
        }

        /// <summary>
        /// Check collisions for dynamic objects
        /// </summary>
        public virtual void CheckCollisions(GameTime gameTime)
        {
            GameEntityList deadObjects = new GameEntityList();

            CollisionBox cameraBox = new CollisionBox(this.camera.box);

            cameraBox.min += this.camera.world.Translation;
            cameraBox.max += this.camera.world.Translation;
            cameraBox.min.Y -= this.camera.head_height + this.camera.step_height;
            cameraBox.max.Y += this.camera.head_height + this.camera.step_height;

            ///Dynamic objects with player
            foreach (GameEntity ge in this.sceneObjects.Values)
            {
                if (cameraBox.BoxIntersect(ge.Box))
                {
                    if (ge is ICatchable)
                    {
                        ((ICatchable)ge).Attach(this.player);

                        deadObjects.Add(ge.ModelMesh.Name, ge);
                        ge.ModelMesh.Tag = null;
                    }
                    else if (ge is IAttachable)
                    {
                        if (ge is GameObjectEntity)
                            this.player.Add((GameObjectEntity)ge);
                    }
                }

                ge.Update(gameTime, this.collisionMesh);
            }

            foreach (GameEntity ge in deadObjects.Values)
            {
                this.sceneObjects.Remove(ge.ModelMesh.Name);
            }

            deadObjects.Clear();

            ///Enemies with player            
            foreach (Enemy e in this.enemies.Values)
            {
                if (cameraBox.BoxIntersect(e.Box))
                {
                    this.SendMessage(player, e, GameEntityMessageType.Hit, gameTime);
                }

                e.Update(gameTime, this.collisionMesh);
            }             

            ///Bullets with enemies, static objects...
            foreach (Bullet b in this.bullets)
            {
                //b.Update(gameTime, ref this.collisionMesh, ref this.enemies);
            }
        }

        public virtual void SendMessage(GameEntity sender, GameEntity receiver, 
                                         GameEntityMessageType type, GameTime timeStamp)
        {
            GameEntityMessage gem = new GameEntityMessage();

            gem.sender = sender;
            gem.receiver = receiver;
            gem.type = type;
            gem.timeStamp = timeStamp;

            receiver.ReceiveMessage(gem);
        }
    }
}