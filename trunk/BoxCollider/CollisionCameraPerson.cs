/*
	XNA BoxCollider - 3D Collision Detection and Response Library
    Copyright (C) 2007 Fabio Policarpo (fabio.policarpo@gmail.com)

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace BoxCollider
{
    // person camera (quake like camera)
    public class CollisionCameraPerson : CollisionCamera
    {
        public Matrix transform;       // the person transform matrix (without up/down view rotation)
        public Vector3 velocity;       // current velocity vector used only by gravity

        public float head_height;      // height from center of box to eye position
        public float step_height;      // max height for step player can climb without jumping

        public float gravity;          // gravity intensity
        public bool on_ground;         // is player on ground (false if in air)
        public float jump_height;      // height player will reach when jumping

        public float up_down_rot;      // up/down view rotation
        public float auto_move_y;      // distance to move in Y axis on next update in order to climb up/down a step

        public CollisionCameraPerson(Vector3 position, Vector3 look_position, float angle, float aspect,
                    float width, float height, float step_height, float head_height, 
                    float up_down_rot, float gravity, float jump_height)
            :
            base(position, look_position, angle, aspect)
        {
            width *= 0.5f;
            height *= 0.5f;

            this.step_height = step_height;
            this.head_height = head_height - height;
            this.up_down_rot = up_down_rot;
            this.gravity = gravity;
            this.jump_height = jump_height;

            transform = world;

            on_ground = false;
            velocity = Vector3.Zero;

            box = new CollisionBox(
                        new Vector3(-width, -height + step_height, -width),
                        new Vector3(width, height, width));
        }

        public override void Draw(GraphicsDevice gd)
        {
            box.min += world.Translation;
            box.max += world.Translation;
            box.min.Y -= head_height;
            box.max.Y -= head_height;

            box.Draw(gd);

            box.min -= world.Translation;
            box.max -= world.Translation;
            box.min.Y += head_height;
            box.max.Y += head_height;
        }

        public override void Reset(Matrix m)
        {
            // make sure matrix Y axis is (0,1,0)
            transform = m;
            if (transform.M22 < 0.9999f)
            {
                // rotate Y to (0,1,0)
                Vector3 axis_y = new Vector3(transform.M21, transform.M22, transform.M23);
                float ang = (float)Math.Acos(axis_y.Y);
                Vector3 axis = Vector3.Normalize(Vector3.Cross(axis_y, Vector3.UnitY));
                Vector3 pos = transform.Translation;
                transform.Translation = Vector3.Zero;
                transform = transform * Matrix.CreateFromAxisAngle(axis, ang);
                transform.Translation = pos;
            }
            up_down_rot = 0.0f;
            world = transform;
            view = Matrix.Invert(world);
			frustum = new BoundingFrustum(view * projection);
        }

        public override void Update(TimeSpan elapsed_time, ref CollisionMesh collision_mesh,
            GamePadState gamepad_state, KeyboardState keyboard_state)
        {
            float time_seconds = (float)elapsed_time.TotalSeconds;

            float speed_boost = gamepad_state.Triggers.Left;
            if (keyboard_state.IsKeyDown(Keys.LeftShift))
                speed_boost = 1.0f;

            float rot_speed = 2.0f * time_seconds;
            float move_speed = (300.0f + 400.0f * speed_boost) * time_seconds;

            if (on_ground == false)
                velocity.Y -= gravity * time_seconds;
            else
            {
                if (gamepad_state.Buttons.A == ButtonState.Pressed ||
                    keyboard_state.IsKeyDown(Keys.Space))
                {
                    velocity.Y = (float)Math.Sqrt(gravity * 2.0f * jump_height);
                    on_ground = false;
                }
                else
                    velocity.Y = 0.0f;
            }

            Vector3 position = transform.Translation;

            Vector3 axis_x = new Vector3(transform.M11, transform.M12, transform.M13);
            Vector3 axis_y = new Vector3(0, 1, 0);
            Vector3 axis_z = new Vector3(transform.M31, transform.M32, transform.M33);

            Vector3 translate, rotate;
            GetInputVectors(gamepad_state, keyboard_state, out translate, out rotate);

            Vector3 new_position = position;
            new_position += axis_x * (move_speed * translate.X);
            new_position -= axis_z * (move_speed * translate.Z);
            new_position += velocity * time_seconds;

            float move_y = 12.5f * step_height * time_seconds;
            if (auto_move_y >= 0)
            {
                if (move_y > auto_move_y)
                    move_y = auto_move_y;
            }
            else
            {
                move_y = -move_y;
                if (move_y < auto_move_y)
                    move_y = auto_move_y;
            }
            new_position.Y += move_y;
            auto_move_y = 0;

            collision_mesh.BoxMove(box, position, new_position, 1.0f, 0.0f, 3, out new_position);

            if (Math.Abs(new_position.Y - position.Y) < 0.0001f && velocity.Y > 0.0f)
                velocity.Y = 0.0f;

            float dist;
            Vector3 pos, norm;
            if (velocity.Y <= 0)
                if (true == collision_mesh.BoxIntersect(box, 
                                                        new_position, 
                                                        new_position + new Vector3(0, -2 * step_height, 0), 
                                                        out dist, out pos, out norm))
                {
                    if (norm.Y > 0.70710678f)
                    {
                        on_ground = true;
                        auto_move_y = step_height - dist;
                    }
                    else
                        on_ground = false;
                }
                else
                    on_ground = false;

            up_down_rot -= rot_speed * rotate.X;
            if (up_down_rot > 1)
                up_down_rot = 1;
            else
                if (up_down_rot < -1)
                    up_down_rot = -1;

            Matrix rot_x = Matrix.CreateFromAxisAngle(axis_x, up_down_rot);
            Matrix rot_y = Matrix.CreateFromAxisAngle(axis_y, -rot_speed * rotate.Y);

            transform.Translation = Vector3.Zero;
            transform = transform * rot_y;
            Orthonormalize(ref transform);

            world.Translation = Vector3.Zero;
            world = transform * rot_x;

            transform.Translation = new_position;
            new_position.Y += head_height;
            world.Translation = new_position;

            view = Matrix.Invert(world);

			frustum = new BoundingFrustum(view * projection);
        }
    }
}
