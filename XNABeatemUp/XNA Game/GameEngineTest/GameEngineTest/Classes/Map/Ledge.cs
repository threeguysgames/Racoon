using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace GameEngineTest
{
    public class Edge
    {
        public Vector2 p1 = new Vector2();
        public Vector2 p2 = new Vector2();

    }

    public class Ledge
    {
        Vector2[] node = new Vector2[16];
        Edge[] edges = new Edge[16];
        public int totalNodes = 0;
        public int totalEdges = 0;
        public int flags = 0;

        public Vector2[] Nodes
        {
            get { return node; }
        }

        public void populateEdges()
        {
            for (int i = 0; i < this.totalNodes; i++)
            {
                edges[i] = new Edge();
                if (totalNodes > 1)
                {
                    if (i < totalNodes - 1)
                    {
                        edges[i].p1 = node[i];
                        edges[i].p2 = node[i];
                    }
                    else if (i == totalNodes - 1)
                    {
                        edges[i].p1 = node[i];
                        edges[i].p2 = node[0];
                    }

                    Console.WriteLine(edges[i].p1.X + " " + edges[i].p1.Y);
                }
            }

            totalEdges = totalNodes;
        }

        public bool isInside(Vector2 hitPoint)
        {
            int i, j = this.totalEdges - 1;
            bool oddNodes = false;

            for (i = 0; i < this.totalEdges; i++)
            {
                if (this.node[i].Y < hitPoint.Y && this.node[j].Y >= hitPoint.Y
                || this.node[j].Y < hitPoint.Y && this.node[i].Y >= hitPoint.Y)
                {
                    if (this.node[i].X + (hitPoint.Y - this.node[i].Y) / (this.node[j].Y - this.node[i].Y) * (this.node[j].X - this.node[i].X) < hitPoint.X)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }
            return oddNodes;
        }

        public bool hasEdges()
        {
            return (totalNodes > 1);
        }
    }
}
