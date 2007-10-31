#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
//Xna
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//3rd part
using Xclna.Xna.Animation;
using BoxCollider;
//We
using FPSFramework;
using FPSFramework.Core;
using FPSFramework.AI;
#endregion


namespace FPSFramework.Logic
{
    public class EnemiesList : Dictionary<String, Enemy> { }

    /// <summary>
    /// Class describing enemy's properties
    /// </summary>
    public class Enemy : GameLiveEntity
    {
        private IGameState actualState = null;
        
        private GameEntityAnimationState actualAnimationState = GameEntityAnimationState.Idle;       

        private float scaleFactor = 0.0f;

        private float runSpeed = 0.0f;

        private float walkSpeed = 0.0f;

        private float yawAngle = 0.0f;

        private Vector3 velocity = Vector3.Zero;

        private float auto_move_y = 0.0f;

        private bool on_ground = false;

        private ModelAnimator modelAnimator = null;

        ///<summary>
        /// Animators controllers       
        private AnimationController idleController;
        private AnimationController walkController;
        private AnimationController runController;
        private AnimationController attackController;
        private AnimationController dieController;
        /// </summary>

#region Properties
        public float ScaleFactor
        {
            get { return this.scaleFactor; }
            set { this.scaleFactor = value; }
        }

        public GameEntityAnimationState ActualAnimationState
        {
            get { return this.actualAnimationState; }
            set { this.actualAnimationState = value; }
        }

        public float WalkSpeed
        {
            get { return this.walkSpeed; }
            set { this.walkSpeed = value; }
        }

        public float RunSpeed
        {
            get { return this.runSpeed; }
            set { this.runSpeed = value; }
        }

        public float YawAngle
        {
            get { return this.yawAngle; }
            set { this.yawAngle = value; }
        }

        public bool OnGround
        {
            get { return this.on_ground; }
            set { this.on_ground = value; }
        }

        public Vector3 Velocity
        {
            get { return this.velocity; }
            set { this.velocity = value; }
        }

        public float AutoMoveY
        {
            get { return this.auto_move_y; }
            set { this.auto_move_y = value; }
        }

        public ModelAnimator ModelAnimator
        {
            get { return this.modelAnimator; }
            set { this.modelAnimator = value; }
        }

        public AnimationController IdleController
        {
            get { return this.idleController; }
            set { this.idleController = value; }
        }

        public AnimationController WalkController
        {
            get { return this.walkController; }
            set { this.walkController = value; }
        }

        public AnimationController RunController
        {
            get { return this.runController; }
            set { this.runController = value; }
        }

        public AnimationController AttackController
        {
            get { return this.attackController; }
            set { this.attackController = value; }
        }

        public AnimationController DieController
        {
            get { return this.dieController; }
            set { this.dieController = value; }
        }
#endregion

#region Ctor
        public Enemy()
            : base()
        {
            this.actualState = (IGameState)new IdleState();
        }

        public Enemy(int health, int lives)
            : base(health, lives)
        {
        }
#endregion

        /// <summary>
        /// Custom update method for enemy
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="view">View matrix from Camera</param>
        /// <param name="projection">Projectino matrix from camera</param>
        public virtual void Update(GameTime gameTime, Vector3 cameraPos, ref Matrix view, 
                                    ref Matrix projection, ref CollisionMesh collision)
        {
            //see every state
            if (this.actualState != null)
            {
                if (this.actualState is FollowState)
                {
                    ((FollowState)this.actualState).Update(gameTime, this, cameraPos, ref view, ref projection, ref collision);
                }
                else
                {
                    this.actualState.Update(gameTime, this);
                }
            }

            this.Matrix = Matrix.CreateScale(this.scaleFactor) * 
                            Matrix.CreateTranslation(this.Position);

            //Update view data
            foreach (ModelMesh mesh in this.modelAnimator.Model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    if (effect is BasicEffect)
                    {
                        BasicEffect basic = (BasicEffect)effect;

                        basic.View = view;
                        basic.Projection = projection;
                    }
                    else if (effect is BasicPaletteEffect)
                    {
                        BasicPaletteEffect palette = (BasicPaletteEffect)effect;

                        palette.View = view;
                        palette.Projection = projection;
                    }
                }
            }

            this.ModelAnimator.Update(gameTime);
        }

        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// </summary>
        public static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }


        /// <summary>
        /// Draw method for enimy
        /// </summary>
        /// <param name="gameTime"></param>

        public override void Draw(GameTime gameTime)
        {
            this.ModelAnimator.Draw(gameTime);
            this.RunAnimation(this.actualAnimationState);           

            base.Draw(gameTime);
        }

        public override void ReceiveMessage(GameEntityMessage message)
        {
            switch (message.type)
            {
            case GameEntityMessageType.Hit:
                {
                    if (message.sender is Player)
                    {
                        IGameState gs = (IGameState)new FollowState();
                        this.ChangeState(gs);
                    }
                    break;
                }

            case GameEntityMessageType.Damage:
                {

                    break;
                }
            }

            base.ReceiveMessage(message);
        }


        public virtual void ChangeState(IGameState newState)
        {
            this.actualState = newState;
        }


        // Add this as a new method
        public virtual void RunAnimation(GameEntityAnimationState s)
        {
            AnimationController controller = null;

            switch (s)
            {
            case GameEntityAnimationState.Idle:
                    controller = this.IdleController;
                break;

            case GameEntityAnimationState.Walk:
                    controller = this.WalkController;
                break;

            case GameEntityAnimationState.Run:
                    controller = this.RunController;
                break;

            case GameEntityAnimationState.Attack:
                    controller = this.AttackController;
                break;

            case GameEntityAnimationState.Die:
                    controller = this.DieController;
                break;
            }

            if (controller != null)
            {
                foreach (BonePose p in this.modelAnimator.BonePoses)
                {
                    p.CurrentController = controller;
                    p.CurrentBlendController = null;
                    p.BlendFactor = 0.0f;
                }
            }
        }
        
    }
}
