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
    // base camera class 
	public abstract class CollisionCamera : CollisionTreeElemDynamic
    {
        public float near_plane = 1.0f;
        public float far_plane = 10000.0f;

		public float angle = MathHelper.ToRadians(45);
        public float aspect = 1.0f;

        public Matrix world;        // camera position and rotation
        public Matrix view;         // view = inverse( world )
        public Matrix projection;   // projection matrix
		
		public BoundingFrustum frustum; // camera frustum

        public CollisionCamera(Vector3 position, Vector3 look_position, float angle, float aspect)
        {
            this.angle = angle;
            this.aspect = aspect;

            projection = Matrix.CreatePerspectiveFieldOfView(angle, aspect, near_plane, far_plane);

            view = Matrix.CreateLookAt(position, look_position, Vector3.Up);

            world = Matrix.Invert(view);

			frustum = new BoundingFrustum(view * projection);
        }

		public void SetAspect(float aspect)
		{
			this.aspect = aspect;
			projection = Matrix.CreatePerspectiveFieldOfView(angle, aspect, near_plane, far_plane);
			frustum = new BoundingFrustum(view * projection);
		}

		public void SetAngle(float angle)
		{
			this.angle = angle;
			projection = Matrix.CreatePerspectiveFieldOfView(angle, aspect, near_plane, far_plane);
			frustum = new BoundingFrustum(view * projection);
		}

		public void SetPlanes(float near_plane, float far_plane)
		{
			this.near_plane = near_plane;
			this.far_plane = far_plane;
			projection = Matrix.CreatePerspectiveFieldOfView(angle, aspect, near_plane, far_plane);
			frustum = new BoundingFrustum(view * projection);
		}
		
		// get world matrix axis or its tranlation component (0 for X, 1 for Y, 2 for Z and 3 for translation)
        public Vector3 GetWorldVector(int axis)
        {
            switch (axis)
            {
                case 0: return new Vector3(world.M11, world.M12, world.M13);
                case 1: return new Vector3(world.M21, world.M22, world.M23);
                case 2: return new Vector3(world.M31, world.M32, world.M33);
                case 3: return new Vector3(world.M41, world.M42, world.M43);
            }

            return Vector3.Zero;
        }

		// get view matrix axis or its tranlation component (0 for X, 1 for Y, 2 for Z and 3 for translation)
		public Vector3 GetViewVector(int axis)
		{
			switch (axis)
			{
				case 0: return new Vector3(view.M11, view.M12, view.M13);
				case 1: return new Vector3(view.M21, view.M22, view.M23);
				case 2: return new Vector3(view.M31, view.M32, view.M33);
				case 3: return new Vector3(view.M41, view.M42, view.M43);
			}

			return Vector3.Zero;
		}

        // get tranlation and rotation from input devices
        static public void GetInputVectors(
            GamePadState gamepad_state, KeyboardState keyboard_state,
            out Vector3 translate, out Vector3 rotate)
        {
            translate = Vector3.Zero;
            rotate = Vector3.Zero;

			translate.X = gamepad_state.ThumbSticks.Left.X;
			if (keyboard_state.IsKeyDown(Keys.Q))
				translate.X -= 1.0f;
			if (keyboard_state.IsKeyDown(Keys.E))
				translate.X += 1.0f;

			translate.Y = 0;

			translate.Z = gamepad_state.ThumbSticks.Left.Y;
			if (keyboard_state.IsKeyDown(Keys.W))
				translate.Z += 1.0f;
			if (keyboard_state.IsKeyDown(Keys.S))
				translate.Z -= 1.0f;

			rotate.X = gamepad_state.ThumbSticks.Right.Y;
			if (keyboard_state.IsKeyDown(Keys.Down))
				rotate.X -= 0.7f;
			if (keyboard_state.IsKeyDown(Keys.Up))
				rotate.X += 0.7f;

			rotate.Y = gamepad_state.ThumbSticks.Right.X;
			if (keyboard_state.IsKeyDown(Keys.Left))
				rotate.Y -= 0.7f;
			if (keyboard_state.IsKeyDown(Keys.Right))
				rotate.Y += 0.7f;
			
            rotate.Z = 0;
            if (gamepad_state.Buttons.LeftShoulder == ButtonState.Pressed ||
                keyboard_state.IsKeyDown(Keys.A))
                rotate.Z += 0.7f;
            if (gamepad_state.Buttons.RightShoulder == ButtonState.Pressed ||
                keyboard_state.IsKeyDown(Keys.D))
                rotate.Z -= 0.7f;

            if (rotate.X >= 0.00001f && rotate.X < 0.00001f)
                rotate.X = 0;
            if (rotate.Y >= 0.00001f && rotate.Y < 0.00001f)
                rotate.Y = 0;
            if (rotate.Z >= 0.00001f && rotate.Z < 0.00001f)
                rotate.Z = 0;
        }

        // make sure matrix axis are perpendicular and unit size
        static public void Orthonormalize(ref Matrix m)
        {
            Vector3 axis_x = new Vector3(m.M11, m.M12, m.M13);
            Vector3 axis_y = new Vector3(m.M21, m.M22, m.M23);
            Vector3 axis_z = new Vector3(m.M31, m.M32, m.M33);
            axis_z = Vector3.Normalize(Vector3.Cross(axis_x, axis_y));
            axis_y = Vector3.Normalize(Vector3.Cross(axis_z, axis_x));
            axis_x = Vector3.Normalize(Vector3.Cross(axis_y, axis_z));
            m.M11 = axis_x.X; m.M12 = axis_x.Y; m.M13 = axis_x.Z;
            m.M21 = axis_y.X; m.M22 = axis_y.Y; m.M23 = axis_y.Z;
            m.M31 = axis_z.X; m.M32 = axis_z.Y; m.M33 = axis_z.Z;
        }

        public abstract void Draw(GraphicsDevice gd);
        public abstract void Reset(Matrix m);
        public abstract void Update(TimeSpan elapsed_time, ref CollisionMesh collision_mesh,
            GamePadState gamepad_state, KeyboardState keyboard_state);
    }
}
