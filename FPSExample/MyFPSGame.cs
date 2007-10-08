#region Using Statements
using System;
using System.Collections.Generic;
using System.Xml;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using BoxCollider;

using FPSFramework;
using FPSFramework.Logic;
using FPSFramework.Core;
#endregion

namespace FPSExample
{    
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MyFPSGame : Microsoft.Xna.Framework.Game
    {        
        //XNA stuff
        GraphicsDeviceManager graphics;
        ContentManager content;

        //For drawing text on the screen
        SpriteFont debugFont;
        SpriteBatch sb;
        string debugString;       

        SceneComponent cenario;

        public MyFPSGame()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);
            debugString = "XNA FPS Framework v. 0.2.1";            
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            this.Window.Title = debugString;
            this.Window.AllowUserResizing = true;

            base.Initialize();
        }        

        /// <summary>
        /// Load your graphics content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadGraphicsContent(bool loadAllContent)
        {                       
            if (loadAllContent)
            {
                this.graphics.GraphicsDevice.RenderState.AlphaBlendEnable = true;
                this.graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
                this.graphics.GraphicsDevice.RenderState.AlphaTestEnable = false;
                this.graphics.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

                Model scene = content.Load<Model>(@"Content\cena_teste02");

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(@"Content\FPSExample.xml");

                //saida de texto
                this.debugFont = content.Load<SpriteFont>("DebugFont");
                this.sb = new SpriteBatch(graphics.GraphicsDevice);

                //FPS Framework
                FPSFramework.Core.SystemResources.Register(ref this.content);

                this.cenario = new SceneComponent(this, scene, xmlDoc);
            }

            // TODO: Load any ResourceManagementMode.Manual content
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
          
            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            this.graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;    //VERY IMPORTANT FOR BoxCollider!!!

            base.Draw(gameTime);
        }

        /// <summary>
        /// Unload your graphics content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent)
            {
                // TODO: Unload any ResourceManagementMode.Automatic content
                content.Unload();
            }

            // TODO: Unload any ResourceManagementMode.Manual content
        }
    }
}
