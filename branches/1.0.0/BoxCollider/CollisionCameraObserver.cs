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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoxCollider
{
    // observer camera (descent like camera)
    public class CollisionCameraObserver : CollisionCamera
    {
        public CollisionCameraObserver(Vector3 position, Vector3 look_position, float angle, float aspect, float radius)
            :
            base(position, look_position, angle, aspect)
        {
            box = new CollisionBox(-radius, radius);
        }

        public override void Draw(GraphicsDevice gd)
        {
            box.min += world.Translation;
            box.max += world.Translation;

            box.Draw(gd);

            box.min -= world.Translation;
            box.max -= world.Translation;
        }

        public override void Reset(Matrix m)
        {
            world = m;
            view = Matrix.Invert(world);
			frustum = new BoundingFrustum(view * projection);
        }

        public override void Update(TimeSpan elapsed_time, ref CollisionMesh collision_mesh,
            GamePadState gamepad_state, KeyboardState keyboard_state)
        {
            float time_seconds = (float)elapsed_time.TotalSeconds;

			float speed_boost = 0.0f;
			if (gamepad_state.Buttons.LeftStick == ButtonState.Pressed)
				speed_boost = 1.0f;
			if (keyboard_state != null && keyboard_state.IsKeyDown(Keys.LeftShift))
                speed_boost = 1.0f;

            float rot_speed = 2.0f * time_seconds;
            float move_speed = (400.0f + 600.0f * speed_boost) * time_seconds;

            Vector3 position = world.Translation;

            Vector3 axis_x = new Vector3(world.M11, world.M12, world.M13);
            Vector3 axis_y = new Vector3(world.M21, world.M22, world.M23);
            Vector3 axis_z = new Vector3(world.M31, world.M32, world.M33);

            Vector3 translate, rotate;
            GetInputVectors(gamepad_state, keyboard_state, out translate, out rotate);
			if (gamepad_state.Buttons.RightStick == ButtonState.Pressed)
				rotate.X = rotate.Y = 0;

            Vector3 new_position = position;
            new_position += axis_x * (move_speed * translate.X);
            new_position += axis_y * (move_speed * translate.Y);
            new_position -= axis_z * (move_speed * translate.Z);

            collision_mesh.BoxMove(box, position, new_position, 1.0f, 0.0f, 3, out new_position);

            Matrix rot_x = Matrix.CreateFromAxisAngle(axis_x, -rot_speed * rotate.X);
            Matrix rot_y = Matrix.CreateFromAxisAngle(axis_y, -rot_speed * rotate.Y);
            Matrix rot_z = Matrix.CreateFromAxisAngle(axis_z, rot_speed * rotate.Z);

            world.Translation = new Vector3(0, 0, 0);

            world = world * (rot_x * rot_y * rot_z);

            world.Translation = new_position;

            Orthonormalize(ref world);

            view = Matrix.Invert(world);

			frustum = new BoundingFrustum(view * projection);
        }
    }
}
