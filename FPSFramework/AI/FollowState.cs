#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
//Xna
using Microsoft.Xna.Framework;
//3rd part
using BoxCollider;
//We
using FPSFramework.Logic;
#endregion

namespace FPSFramework.AI
{
    public class FollowState : IGameState
    {
        public FollowState()
        {
        }


        public void Enter()
        {
        }

        public void Update(GameTime gameTime, Enemy e)
        {
        }


        public void Update(GameTime gameTime, Enemy e, Vector3 cameraPos, ref Matrix view, 
                            ref Matrix projection, ref CollisionMesh collision)
        {
            /*Vector3 direction = Vector3.Subtract(e.Position, cameraPos);

            Matrix rotation = Matrix.CreateRotationY(e.YawAngle);
            Vector3 initialFacing = Vector3.Cross(Vector3.UnitZ, Vector3.Up);

            initialFacing = Vector3.Transform(initialFacing, rotation);
            initialFacing.Normalize();
            direction.Normalize();

            float res = Vector3.Dot(initialFacing, direction);

            float x = direction.X - e.Position.X;
            float y = direction.Y - e.Position.Y;
            float desiredAngle = (float)Math.Atan2(y, x);
            
            if (desiredAngle >= MathHelper.ToRadians(30.0f))
            {
                float speed = 0.25f;


                float difference = Enemy.WrapAngle(desiredAngle - e.YawAngle);
                difference = MathHelper.Clamp(difference, -speed, speed);

                e.YawAngle = Enemy.WrapAngle(e.YawAngle + difference);
                e.ActualAnimationState = GameEntityAnimationState.Idle;
            }
            else
            {
                e.ActualAnimationState = GameEntityAnimationState.Walk;
                Vector3 pos01 = e.Position;

                pos01.X -= e.WalkSpeed * direction.X;
                pos01.Z -= e.WalkSpeed * direction.Z;

                e.Position = pos01;
            }         

            float time_seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float rot_speed = 2.0f * time_seconds;
            float move_speed = (300.0f + 400.0f) * time_seconds;

            if (e.OnGround == false)
            {
                Vector3 vector = e.Velocity;
                vector.Y -= 2000.0f * time_seconds;
            }

            Vector3 position = e.Position;

            Vector3 axis_x = new Vector3(e.Matrix.M11, e.Matrix.M12, e.Matrix.M13);
            Vector3 axis_y = new Vector3(0, 1, 0);
            Vector3 axis_z = new Vector3(e.Matrix.M31, e.Matrix.M32, e.Matrix.M33);

            Vector3 translate = Vector3.Zero;
            Vector3 rotate = Vector3.Zero;

            Vector3 new_position = position;
            new_position += axis_x * (move_speed * translate.X);
            new_position -= axis_z * (move_speed * translate.Z);
            new_position += e.Velocity * time_seconds;
            
            float move_y = 12.5f * time_seconds;
            if (e.AutoMoveY >= 0)
            {
                if (move_y > e.AutoMoveY)
                    move_y = e.AutoMoveY;
            }
            else
            {
                move_y = -move_y;
                if (move_y < e.AutoMoveY)
                    move_y = e.AutoMoveY;
            }

            collision.BoxMove(e.Box, position, new_position, 1.0f, 0.0f, 3, out new_position);

            if (Math.Abs(new_position.Y - position.Y) < 0.0001f && e.Velocity.Y > 0.0f)
            {
                Vector3 v = e.Velocity;
                v.Y = 0.0f;
                e.Velocity = v;
            }

            float dist;
            Vector3 pos, norm;

            if (e.Velocity.Y <= 0)
                if (true == collision.BoxIntersect(e.Box,
                                                        new_position,
                                                        new_position + new Vector3(0, -2, 0),
                                                        out dist, out pos, out norm))
                {
                    if (norm.Y > 0.70710678f)
                    {
                        e.AutoMoveY = dist;
                    }
                    else
                        e.OnGround = false;
                }
                else
                    e.OnGround = false;


            Matrix rot_y = Matrix.CreateFromAxisAngle(axis_y, -rot_speed * rotate.Y);

            Matrix m = e.Matrix;
            m.Translation = Vector3.Zero;
            m = m * rot_y;

            CollisionCamera.Orthonormalize(ref m);

            m.Translation = Vector3.Zero;
            m.Translation = new_position;*/
            Vector3 direction = Vector3.Subtract(e.Position, cameraPos);

            Matrix rotation = Matrix.CreateRotationY(e.YawAngle);
            Vector3 initialFacing = Vector3.Cross(Vector3.UnitZ, Vector3.Up);

            initialFacing = Vector3.Transform(initialFacing, rotation);
            initialFacing.Normalize();
            direction.Normalize();

            float res = Vector3.Dot(initialFacing, direction);

            float x = direction.X - e.Position.X;
            float y = direction.Y - e.Position.Y;
            float desiredAngle = (float)Math.Atan2(y, x);

            if (desiredAngle >= MathHelper.ToRadians(30.0f))
            {
                float speed = 0.25f;


                float difference = Enemy.WrapAngle(desiredAngle - e.YawAngle);
                difference = MathHelper.Clamp(difference, -speed, speed);

                e.YawAngle = Enemy.WrapAngle(e.YawAngle + difference);
                e.ActualAnimationState = GameEntityAnimationState.Idle;
            }
            else
            {
                e.ActualAnimationState = GameEntityAnimationState.Walk;
                Vector3 pos01 = e.Position;

                pos01.X -= e.WalkSpeed * direction.X;
                pos01.Z -= e.WalkSpeed * direction.Z;

                e.Box.min.X -= e.WalkSpeed * direction.X;
                e.Box.min.Z -= e.WalkSpeed * direction.Z;
                e.Box.max.X -= e.WalkSpeed * direction.X;
                e.Box.max.Z -= e.WalkSpeed * direction.Z;
                
                e.Position = pos01;
                
            }
            /*
            float time_seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float rot_speed = 2.0f * time_seconds;
            float move_speed = (300.0f + 400.0f) * time_seconds;

            if (e.OnGround == false)
            {
                Vector3 vector = e.Velocity;
                vector.Y -= 2000.0f * time_seconds;
            }

            Vector3 position = e.Position;

            Vector3 axis_x = new Vector3(e.Matrix.M11, e.Matrix.M12, e.Matrix.M13);
            Vector3 axis_y = new Vector3(0, 1, 0);
            Vector3 axis_z = new Vector3(e.Matrix.M31, e.Matrix.M32, e.Matrix.M33);

            Vector3 translate = Vector3.Zero;
            Vector3 rotate = Vector3.Zero;

            Vector3 new_position = position;
            new_position += axis_x * (move_speed * translate.X);
            new_position -= axis_z * (move_speed * translate.Z);
            new_position += e.Velocity * time_seconds;

            float move_y = 12.5f * time_seconds;
            if (e.AutoMoveY >= 0)
            {
                if (move_y > e.AutoMoveY)
                    move_y = e.AutoMoveY;
            }
            else
            {
                move_y = -move_y;
                if (move_y < e.AutoMoveY)
                    move_y = e.AutoMoveY;
            }

            collision.BoxMove(e.Box, position, new_position, 1.0f, 0.0f, 3, out new_position);

            if (Math.Abs(new_position.Y - position.Y) < 0.0001f && e.Velocity.Y > 0.0f)
            {
                Vector3 v = e.Velocity;
                v.Y = 0.0f;
                e.Velocity = v;
            }

            float dist;
            Vector3 pos, norm;

            if (e.Velocity.Y <= 0)
                if (true == collision.BoxIntersect(e.Box,
                                                        new_position,
                                                        new_position + new Vector3(0, -2, 0),
                                                        out dist, out pos, out norm))
                {
                    if (norm.Y > 0.70710678f)
                    {
                        e.AutoMoveY = dist;
                    }
                    else
                        e.OnGround = false;
                }
                else
                    e.OnGround = false;


            Matrix rot_y = Matrix.CreateFromAxisAngle(axis_y, -rot_speed * rotate.Y);

            Matrix m = e.Matrix;
            m.Translation = Vector3.Zero;
            m = m * rot_y;

            CollisionCamera.Orthonormalize(ref m);

            m.Translation = Vector3.Zero;
            m.Translation = new_position;*/
        }

        public void Exit()
        {
        }
    }
}
