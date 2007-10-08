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
    public class CollisionTreeElem
    {
        // bounding box for tree element
        public CollisionBox box;
        // recurse id used to compute selections without element duplicates
		public uint last_recurse_id = 0;

        public CollisionTreeElem()
        {
        }

        public virtual bool PointIntersect(Vector3 ray_origin, Vector3 ray_direction, Vector3[] vert_pos,
            out float intersect_distance, out Vector3 intersect_position, out Vector3 intersect_normal)
        {
            intersect_distance = 0;
            intersect_position = Vector3.Zero;
            intersect_normal = Vector3.Zero;
            return false;
        }

        public virtual bool BoxIntersect(CollisionBox ray_box, Vector3 ray_origin, Vector3 ray_direction, Vector3[] vert_pos,
            out float intersect_distance, out Vector3 intersect_position, out Vector3 intersect_normal)
        {
            intersect_distance = 0;
            intersect_position = Vector3.Zero;
            intersect_normal = Vector3.Zero;
            return false;
        }

        public virtual void AddToNode(CollisionTreeNode n)
        {
        }
    }

    public class CollisionTreeElemDynamic : CollisionTreeElem
    {
        // all tree nodes the dynamic element is included
		List<CollisionTreeNode> nodes = new List<CollisionTreeNode>();

        public CollisionTreeElemDynamic()
            : base()
        {
        }

        public override void AddToNode(CollisionTreeNode n)
        {
			nodes.Add(n);
        }

        public void RemoveFromNodes()
        {
            foreach (CollisionTreeNode n in nodes)
            {
                n.RemoveElement(this);
            }
			nodes.Clear();
        }
    }
}
