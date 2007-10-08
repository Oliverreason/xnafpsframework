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

namespace BoxCollider
{
    public class CollisionFace : CollisionTreeElem
    {
        // indices for the three face vertices
        int[] vertices;

        // face constructor
        public CollisionFace(int offset, int[] vert_indx, int vert_offset, Vector3[] vert_pos)
        {
            vertices = new int[3];
            box = new CollisionBox(float.MaxValue, -float.MaxValue);
            for (int i = 0; i < 3; i++)
            {
                vertices[i] = vert_indx[i + offset] + vert_offset;
                box.AddPoint(vert_pos[vertices[i]]);
            }
        }

		public CollisionFace(int offset, short[] vert_indx, int vert_offset, Vector3[] vert_pos)
		{
			vertices = new int[3];
			box = new CollisionBox(float.MaxValue, -float.MaxValue);
			for (int i = 0; i < 3; i++)
			{
				vertices[i] = (int)vert_indx[i + offset] + vert_offset;
				box.AddPoint(vert_pos[vertices[i]]);
			}
		}
		
		// remove vector component (vector3 to vector2)
        public static Vector2 Vector3RemoveComponent(Vector3 v, uint i)
        {
            switch (i)
            {
                case 0: return new Vector2(v.Y, v.Z);
                case 1: return new Vector2(v.X, v.Z);
                case 2: return new Vector2(v.X, v.Y);
                default: return Vector2.Zero;
            }
        }

        // intersect edge (p1,p2) moving in direction (dir) colliding with edge (p3,p4) 
        // return true on a collision with collision distance (dist) and intersection point (ip)
        public static bool EdgeIntersect(Vector3 p1, Vector3 p2, Vector3 dir, Vector3 p3, Vector3 p4, out float dist, out Vector3 ip)
        {
            dist = 0;
            ip = Vector3.Zero;

            // edge vectors
            Vector3 v1 = p2 - p1;
            Vector3 v2 = p4 - p3;

            // build plane based on edge (p1,p2) and move direction (dir)
            Vector3 plane_dir;
            float plane_w;
            plane_dir = Vector3.Cross(v1, dir);
            plane_dir.Normalize();
            plane_w = Vector3.Dot(plane_dir, p1);

            // if colliding edge (p3,p4) does not cross plane return no collision
            // same as if p3 and p4 on same side of plane return 0
            float temp = (Vector3.Dot(plane_dir, p3) - plane_w) * (Vector3.Dot(plane_dir, p4) - plane_w);
            if (temp > 0)
                return false;

            // if colliding edge (p3,p4) and plane are paralell return no collision
            v2.Normalize();
            temp = Vector3.Dot(plane_dir, v2);
            if (temp == 0)
                return false;

            // compute intersection point of plane and colliding edge (p3,p4)
            ip = p3 + v2 * ((plane_w - Vector3.Dot(plane_dir, p3)) / temp);

            // get largest 2D plane projection
            plane_dir.X = Math.Abs(plane_dir.X);
            plane_dir.Y = Math.Abs(plane_dir.Y);
            plane_dir.Z = Math.Abs(plane_dir.Z);
            uint i;
            if (plane_dir.X > plane_dir.Y)
            {
                i = 0;
                if (plane_dir.X < plane_dir.Z)
                    i = 2;
            }
            else
            {
                i = 1;
                if (plane_dir.Y < plane_dir.Z)
                    i = 2;
            }

            // remove component with largest absolute value 
            Vector2 p1_2d = CollisionFace.Vector3RemoveComponent(p1, i);
            Vector2 v1_2d = CollisionFace.Vector3RemoveComponent(v1, i);
            Vector2 ip_2d = CollisionFace.Vector3RemoveComponent(ip, i);
            Vector2 dir_2d = CollisionFace.Vector3RemoveComponent(dir, i);

            // compute distance of intersection from line (ip,-dir) to line (p1,p2)
            dist = (v1_2d.X * (ip_2d.Y - p1_2d.Y) - v1_2d.Y * (ip_2d.X - p1_2d.X)) /
                   (v1_2d.X * dir_2d.Y - v1_2d.Y * dir_2d.X);
            if (dist < 0)
                return false;

            // compute intesection point on edge (p1,p2)
            ip -= dist * dir;

            // check if intersection point (ip) is between egde (p1,p2) vertices
            temp = Vector3.Dot(p1 - ip, p2 - ip);
            if (temp >= 0)
                return false; // no collision

            return true; // collision found!
        }

        // triangle intersect from http://www.graphics.cornell.edu/pubs/1997/MT97.pdf
        public static bool RayTriangleIntersect(Vector3 ray_origin, Vector3 ray_direction,
                    Vector3 vert0, Vector3 vert1, Vector3 vert2,
                    out float t, out float u, out float v)
        {
            t = 0; u = 0; v = 0;

            Vector3 edge1 = vert1 - vert0;
            Vector3 edge2 = vert2 - vert0;

            Vector3 tvec, pvec, qvec;
            float det, inv_det;

            pvec = Vector3.Cross(ray_direction, edge2);

            det = Vector3.Dot(edge1, pvec);

            if (det > -0.00001f)
                return false;

            inv_det = 1.0f / det;

            tvec = ray_origin - vert0;

            u = Vector3.Dot(tvec, pvec) * inv_det;
            if (u < -0.0001f || u > 1.0001f)
                return false;

            qvec = Vector3.Cross(tvec, edge1);

            v = Vector3.Dot(ray_direction, qvec) * inv_det;
            if (v < -0.0001f || u + v > 1.0001f)
                return false;

            t = Vector3.Dot(edge2, qvec) * inv_det;

            if (t <= 0)
                return false;

            return true;
        }

        // ray intersect face and return intersection distance, point and normal
        public override bool PointIntersect(Vector3 ray_origin, Vector3 ray_direction, Vector3[] vert_pos,
            out float intersect_distance, out Vector3 intersect_position, out Vector3 intersect_normal)
        {
            intersect_distance = 0.0f;
            intersect_position = ray_origin;
            intersect_normal = Vector3.Zero;

            Vector3 v1 = vert_pos[vertices[0]];
            Vector3 v2 = vert_pos[vertices[1]];
            Vector3 v3 = vert_pos[vertices[2]];

            Vector3 uvt;
            if (CollisionFace.RayTriangleIntersect(ray_origin, ray_direction, v1, v2, v3, out uvt.Z, out uvt.X, out uvt.Y))
            {
                intersect_distance = uvt.Z;
                intersect_position = (1.0f - uvt.X - uvt.Y) * v1 + uvt.X * v2 + uvt.Y * v3;
                intersect_normal = Vector3.Normalize(Vector3.Cross(v3 - v1, v2 - v1));
                return true;
            }
            return false;
        }

        // box intersect face and return intersection distance, point and normal
        public override bool BoxIntersect(CollisionBox ray_box, Vector3 ray_origin, Vector3 ray_direction, Vector3[] vert_pos,
           out float intersect_distance, out Vector3 intersect_position, out Vector3 intersect_normal)
        {
            intersect_distance = float.MaxValue;
            intersect_position = ray_origin;
            intersect_normal = Vector3.Zero;

            bool intersected = false;
            Vector3 p1, p2, p3, p4;
            uint i, j;

            CollisionBox world_box = new CollisionBox(ray_box.min + ray_origin, ray_box.max + ray_origin);

            Vector3[] box_verts = world_box.GetVertices();
            Vector3[] box_edges = world_box.GetEdges();
            
            // intersect box edges to face edges
            for (i = 0; i < 12; i++)
            {
                // cull edges with normal more than 135 degree from moving direction
                if (Vector3.Dot(CollisionBox.edge_normals[i], ray_direction) < -0.70710678)
                    continue;

                p1 = box_edges[i * 2];
                p2 = box_edges[i * 2 + 1];
                p4 = vert_pos[vertices[0]];
                for (j = 0; j < vertices.Length; j++)
                {
                    p3 = p4;
                    p4 = vert_pos[vertices[(j + 1) % vertices.Length]];

                    float distance;
                    Vector3 position;
                    if (CollisionFace.EdgeIntersect(p1, p2, ray_direction, p3, p4, out distance, out position))
                    {
                        if (distance < intersect_distance)
                        {
                            intersect_distance = distance;
                            intersect_position = position;
                            intersect_normal = Vector3.Normalize(Vector3.Cross(p2-p1,p3-p4));
                            if (Vector3.Dot(ray_direction, intersect_normal) > 0)
                                intersect_normal = Vector3.Negate(intersect_normal);
                            intersected = true;
                        }
                    }
                }
            }
            
            // intersect from face vertices to box
            for (i = 0; i < 3; i++)
            {
                float tnear, tfar;
                p1 = vert_pos[vertices[i]];
                int box_face_id = world_box.RayIntersect(p1, -ray_direction, out tnear, out tfar);
                if (box_face_id > -1)
                {
                    if (tnear < intersect_distance)
                    {
                        intersect_distance = tnear;
                        intersect_position = p1;
                        intersect_normal = -CollisionBox.face_normals[box_face_id];
                        intersected = true;
                    }
                }
            }
            
            // intersect from box vertices to face polygon
            Vector3 v1 = vert_pos[vertices[0]];
            Vector3 v2 = vert_pos[vertices[1]];
            Vector3 v3 = vert_pos[vertices[2]];
            for (i = 0; i < 8; i++)
            {
                // cull vertices with normal more than 135 degree from moving direction
                if (Vector3.Dot(CollisionBox.vertex_normals[i], ray_direction) < -0.70710678)
                    continue;

                Vector3 uvt;
                if (CollisionFace.RayTriangleIntersect(box_verts[i], ray_direction, v1, v2, v3, out uvt.Z, out uvt.X, out uvt.Y))
                {
                    if (uvt.Z < intersect_distance)
                    {
                        intersect_distance = uvt.Z;
                        intersect_position = (1.0f - uvt.X - uvt.Y) * v1 + uvt.X * v2 + uvt.Y * v3;
                        intersect_normal = Vector3.Normalize(Vector3.Cross(v3 - v1, v2 - v1));
                        intersected = true;
                    }
                }
            }
            
            return intersected;
        }
    }
}
