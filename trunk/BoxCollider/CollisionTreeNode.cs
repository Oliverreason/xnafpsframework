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
    public class CollisionTreeNode
    {
        // the bounding box for the node
        CollisionBox box;
        // the node childs (if null node is a leaf)
        CollisionTreeNode[] childs;
        // list with elements included in the node (only created on leaf nodes)
        List<CollisionTreeElem> elems;

        public CollisionTreeNode(CollisionBox b,uint subdiv_level)
        {
            box = b;
            if (subdiv_level>0)
            {
                subdiv_level--;
                childs = new CollisionTreeNode[8];
                CollisionBox[] childs_box = box.GetChilds();
                for( uint i=0;i<8;i++ )
                    childs[i] = new CollisionTreeNode(childs_box[i], subdiv_level);
            }
        }

        public void AddElement(CollisionTreeElem e)
        {
            if (e.box.BoxIntersect(box) == false)
                return;

            if (childs == null)
            {
                if (elems == null)
                    elems = new List<CollisionTreeElem>();
                elems.Add(e);
                e.AddToNode(this);
            }
            else
            {
                foreach(CollisionTreeNode n in childs)
                    n.AddElement(e);
            }
        }

        public void RemoveElement(CollisionTreeElem e)
        {
            if (elems != null)
                elems.Remove( e );
        }

        public void GetElements(CollisionBox b, List<CollisionTreeElem> e, uint recurse_id)
        {
            if (b.BoxIntersect(box) == false)
                return;

            if (elems != null)
            {
                foreach (CollisionTreeElem elem in elems)
                {
                    if (elem.last_recurse_id < recurse_id)
                    {
                        if (elem.box.BoxIntersect(b))
                            e.Add(elem);
                        elem.last_recurse_id = recurse_id;
                    }
                }
            }

            if (childs != null)
            {
                foreach (CollisionTreeNode n in childs)
                    n.GetElements(b, e, recurse_id);
            }
        }
    }
}
