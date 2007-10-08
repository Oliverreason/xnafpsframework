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
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BoxCollider
{
	public class CollisionTree
	{
		// the tree root node
		CollisionTreeNode root;
		// the last recurse id used (for selections without duplicates)
		uint recurse_id;

		public CollisionTree(CollisionBox box, uint subdiv_level)
		{
			root = new CollisionTreeNode(box, subdiv_level);
			recurse_id = 0;
		}

		public void AddElement(CollisionTreeElem elem)
		{
			root.AddElement(elem);
		}

		public void RemoveElement(CollisionTreeElemDynamic dyn_elem)
		{
			dyn_elem.RemoveFromNodes();
		}

		public void GetElements(CollisionBox b, List<CollisionTreeElem> e)
		{
			root.GetElements(b, e, ++recurse_id);
		}

		public bool PointMove(
			Vector3 point_start, Vector3 point_end, Vector3[] vert_pos,
			float friction_factor, float bump_factor, uint recurse_level,
			out Vector3 point_result)
		{
			point_result = point_start;

			Vector3 delta = point_end - point_start;
			float delta_len = delta.Length();
			if (delta_len < 0.00001f)
				return false;

			float total_dist = delta_len;
			delta *= 1.0f / delta_len;

			float bias = 0.01f;

			point_end += delta * bias;

			bool collision_hit = false;

			while (recurse_level > 0)
			{
				float dist;
				Vector3 pos, norm;
				if (false == PointIntersect(point_start, point_end, vert_pos, out dist, out pos, out norm))
				{

					point_start = point_end - delta * bias;
					break;
				}

				collision_hit = true;

				dist -= bias / Math.Abs(Vector3.Dot(delta, norm));
				if (dist > 0)
				{
					point_start += delta * dist;
					total_dist -= dist;
				}

				Vector3 reflect_dir = Vector3.Normalize(Vector3.Reflect(delta, norm));

				Vector3 n = norm * Vector3.Dot(reflect_dir, norm);
				Vector3 t = reflect_dir - n;

				reflect_dir = friction_factor * t + bump_factor * n;

				point_end = point_start + reflect_dir * total_dist;

				delta = point_end - point_start;
				delta_len = delta.Length();
				if (delta_len < 0.00001f)
					break;
				delta *= 1.0f / delta_len;

				point_end += delta * bias;

				recurse_level--;
			}

			point_result = point_start;
			return collision_hit;
		}

		public bool BoxMove(
			CollisionBox box, Vector3 point_start, Vector3 point_end, Vector3[] vert_pos,
			float friction_factor, float bump_factor, uint recurse_level,
			out Vector3 point_result)
		{
			point_result = point_start;

			Vector3 delta = point_end - point_start;
			float delta_len = delta.Length();
			if (delta_len < 0.00001f)
				return false;

			float total_dist = delta_len;
			delta *= 1.0f / delta_len;

			float bias = 0.01f;

			point_end += delta * bias;

			bool collision_hit = false;

			while (recurse_level > 0)
			{
				float dist;
				Vector3 pos, norm;
				if (false == BoxIntersect(box, point_start, point_end, vert_pos, out dist, out pos, out norm))
				{

					point_start = point_end - delta * bias;
					break;
				}

				collision_hit = true;

				dist -= bias / Math.Abs(Vector3.Dot(delta, norm));
				if (dist > 0)
				{
					point_start += delta * dist;
					total_dist -= dist;
				}

				Vector3 reflect_dir = Vector3.Normalize(Vector3.Reflect(delta, norm));

				Vector3 n = norm * Vector3.Dot(reflect_dir, norm);
				Vector3 t = reflect_dir - n;

				reflect_dir = friction_factor * t + bump_factor * n;

				point_end = point_start + reflect_dir * total_dist;

				delta = point_end - point_start;
				delta_len = delta.Length();
				if (delta_len < 0.00001f)
					break;
				delta *= 1.0f / delta_len;

				point_end += delta * bias;

				recurse_level--;
			}

			point_result = point_start;
			return collision_hit;
		}

		public bool PointIntersect(Vector3 ray_start, Vector3 ray_end, Vector3[] vert_pos,
			out float intersect_distance, out Vector3 intersect_position, out Vector3 intersect_normal)
		{
			intersect_distance = 0.0f;
			intersect_position = ray_start;
			intersect_normal = Vector3.Zero;

			Vector3 ray_direction = ray_end - ray_start;
			float ray_length = ray_direction.Length();
			if (ray_length == 0)
				return false;

			CollisionBox ray_box = new CollisionBox(float.MaxValue, -float.MaxValue);
			ray_box.AddPoint(ray_start);
			ray_box.AddPoint(ray_end);
			Vector3 inflate = new Vector3(0.001f, 0.001f, 0.001f);
			ray_box.min -= inflate;
			ray_box.max += inflate;

			List<CollisionTreeElem> elems = new List<CollisionTreeElem>();
			root.GetElements(ray_box, elems, ++recurse_id);

			ray_direction *= 1.0f / ray_length;
			intersect_distance = ray_length;

			bool intersected = false;

			foreach (CollisionTreeElem e in elems)
			{
				float distance;
				Vector3 position;
				Vector3 normal;
				if (true == e.PointIntersect(ray_start, ray_direction, vert_pos, out distance, out position, out normal))
				{
					if (distance < intersect_distance)
					{
						intersect_distance = distance;
						intersect_position = position;
						intersect_normal = normal;
						intersected = true;
					}
				}
			}

			return intersected;
		}

		public bool BoxIntersect(CollisionBox box, Vector3 ray_start, Vector3 ray_end, Vector3[] vert_pos,
			out float intersect_distance, out Vector3 intersect_position, out Vector3 intersect_normal)
		{
			intersect_distance = 0.0f;
			intersect_position = ray_start;
			intersect_normal = Vector3.Zero;

			Vector3 ray_direction = ray_end - ray_start;
			float ray_length = ray_direction.Length();
			if (ray_length == 0)
				return false;

			CollisionBox ray_box = new CollisionBox(box.min + ray_start, box.max + ray_start);
			ray_box.AddPoint(ray_box.min + ray_direction);
			ray_box.AddPoint(ray_box.max + ray_direction);
			Vector3 inflate = new Vector3(0.001f, 0.001f, 0.001f);
			ray_box.min -= inflate;
			ray_box.max += inflate;

			List<CollisionTreeElem> elems = new List<CollisionTreeElem>();
			root.GetElements(ray_box, elems, ++recurse_id);

			ray_direction *= 1.0f / ray_length;
			intersect_distance = ray_length;

			bool intersected = false;

			foreach (CollisionTreeElem e in elems)
			{
				float distance;
				Vector3 position;
				Vector3 normal;
				if (true == e.BoxIntersect(box, ray_start, ray_direction, vert_pos, out distance, out position, out normal))
				{
					if (distance < intersect_distance)
					{
						intersect_distance = distance;
						intersect_position = position;
						intersect_normal = normal;
						intersected = true;
					}
				}
			}

			return intersected;
		}
	}
}
