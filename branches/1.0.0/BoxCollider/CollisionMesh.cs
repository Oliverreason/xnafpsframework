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
using Microsoft.Xna.Framework.Graphics;

namespace BoxCollider
{
    public class CollisionMesh
    {
        // mesh vertices
        Vector3[] vertices;
        // mesh faces
        CollisionFace[] faces;
        // tree with meshes faces
        CollisionTree tree;

        public CollisionMesh(Model model, uint subdiv_level)
        {
            int total_num_faces = 0;
			int total_num_verts = 0;

            foreach (ModelMesh mesh in model.Meshes)
            {
                if (IsDynamicEntity(mesh))
                    continue;

				int nv, ni;
				nv = mesh.VertexBuffer.SizeInBytes / mesh.MeshParts[0].VertexStride;
				if(mesh.IndexBuffer.IndexElementSize == IndexElementSize.SixteenBits)
					ni = mesh.IndexBuffer.SizeInBytes / sizeof(short);
				else
					ni = mesh.IndexBuffer.SizeInBytes / sizeof(int);
                
				total_num_verts += nv;
				total_num_faces += ni / 3;
            }

			vertices = new Vector3[total_num_verts];
			faces = new CollisionFace[total_num_faces];

            int vcount = 0;
            int fcount = 0;

            foreach (ModelMesh mesh in model.Meshes)
            {
                if (IsDynamicEntity(mesh))
                    continue;

				int nv = mesh.VertexBuffer.SizeInBytes / mesh.MeshParts[0].VertexStride;

				if (mesh.MeshParts[0].VertexStride == 16)
				{
					VertexPositionColor[] mesh_vertices = new VertexPositionColor[nv];
					mesh.VertexBuffer.GetData<VertexPositionColor>(mesh_vertices);

					for (int i = 0; i < nv; i++)
						vertices[i + vcount] = mesh_vertices[i].Position;
				}

				if (mesh.MeshParts[0].VertexStride == 20)
				{
					VertexPositionTexture[] mesh_vertices = new VertexPositionTexture[nv];
					mesh.VertexBuffer.GetData<VertexPositionTexture>(mesh_vertices);

					for (int i = 0; i < nv; i++)
						vertices[i + vcount] = mesh_vertices[i].Position;
				}
				else if (mesh.MeshParts[0].VertexStride == 24)
				{
					VertexPositionColorTexture[] mesh_vertices = new VertexPositionColorTexture[nv];
					mesh.VertexBuffer.GetData<VertexPositionColorTexture>(mesh_vertices);

					for (int i = 0; i < nv; i++)
						vertices[i + vcount] = mesh_vertices[i].Position;
				}
				else if (mesh.MeshParts[0].VertexStride == 32)
				{
					VertexPositionNormalTexture[] mesh_vertices = new VertexPositionNormalTexture[nv];
					mesh.VertexBuffer.GetData<VertexPositionNormalTexture>(mesh_vertices);

					for (int i = 0; i < nv; i++)
						vertices[i + vcount] = mesh_vertices[i].Position;
				}

				int nf = 0;

				if (mesh.IndexBuffer.IndexElementSize == IndexElementSize.SixteenBits)
				{
					short[] mesh_indices = new short[mesh.IndexBuffer.SizeInBytes / sizeof(short)];
					mesh.IndexBuffer.GetData<short>(mesh_indices);

					int count = 0;
					foreach (ModelMeshPart mesh_part in mesh.MeshParts)
					{
						for (int i = 0; i < mesh_part.PrimitiveCount; i++)
						{
							faces[nf + fcount] = new CollisionFace(count, mesh_indices, vcount + mesh_part.BaseVertex, vertices);
							count += 3;
							nf++;
						}
					}
				}
				else
				{
					int[] mesh_indices = new int[mesh.IndexBuffer.SizeInBytes / sizeof(int)];
					mesh.IndexBuffer.GetData<int>(mesh_indices);

					int count = 0;
					foreach (ModelMeshPart mesh_part in mesh.MeshParts)
					{
						for (int i = 0; i < mesh_part.PrimitiveCount; i++)
						{
							faces[nf + fcount] = new CollisionFace(count, mesh_indices, vcount + mesh_part.BaseVertex, vertices);
							count += 3;
							nf++;
						}
					}
				}

                vcount += nv;
                fcount += nf;
            }

            CollisionBox box = new CollisionBox(float.MaxValue, -float.MaxValue);
            for (int i = 0; i < vcount; i++)
                box.AddPoint(vertices[i]);


            if (subdiv_level > 6)
                subdiv_level = 6; // max 8^6 nodes
            tree = new CollisionTree(box, subdiv_level);
            for (int i = 0; i < fcount; i++)
                tree.AddElement(faces[i]);
        }

        public bool PointIntersect(
            Vector3 ray_start, Vector3 ray_end, 
            out float intersect_distance, out Vector3 intersect_position, out Vector3 intersect_normal)
        {
            return tree.PointIntersect(ray_start, ray_end, vertices, 
                out intersect_distance, out intersect_position, out intersect_normal);
        }

        public bool BoxIntersect(
            CollisionBox box, Vector3 ray_start, Vector3 ray_end,
            out float intersect_distance, out Vector3 intersect_position, out Vector3 intersect_normal)
        {
            return tree.BoxIntersect(box, ray_start, ray_end, vertices, 
                out intersect_distance, out intersect_position, out intersect_normal);
        }

        public void PointMove(
            Vector3 point_start, Vector3 point_end,
            float friction_factor, float bump_factor, uint recurse_level,
            out Vector3 point_result)
        {
            tree.PointMove(point_start, point_end, vertices, friction_factor, bump_factor, recurse_level,
                out point_result);
        }

        public bool BoxMove(
            CollisionBox box, Vector3 point_start, Vector3 point_end,
            float friction_factor, float bump_factor, uint recurse_level, 
            out Vector3 point_result)
        {
            return tree.BoxMove(box, point_start, point_end, vertices, friction_factor, bump_factor, recurse_level, 
                out point_result);
        }

        public void GetElements(CollisionBox b, List<CollisionTreeElem> e)
        {
            tree.GetElements(b, e);
        }
		
		public void AddElement(CollisionTreeElem e)
		{
			tree.AddElement(e);
		}

		public void RemoveElement(CollisionTreeElemDynamic e)
		{
			tree.RemoveElement(e);
		}

        public bool IsDynamicEntity(ModelMesh mesh)
        {
            return (mesh.Name[0] == '_');
        }
    }
}
