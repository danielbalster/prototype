﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Prototype
{
    static class MeshFactory
    {
        public static MeshGeometry3D CreateCylinder(double radius, double height, int steps)
        {
            var geometry = new MeshGeometry3D();
            var positions = new Point3DCollection();
            var indicies = new Int32Collection();
            var normals = new Vector3DCollection();

            // center
            positions.Add(new Point3D(0,height,0));
            normals.Add(new Vector3D(0, 1, 0));

            var delta = 360.0 / steps;
            for (int i=0; i<steps; ++i)
            {
                var rad = Helper.DegreesToRadians * (i * delta);
                var v1 = new Point3D(Math.Sin(rad)*radius, 0, Math.Cos(rad) * radius);
                var v2 = new Point3D(Math.Sin(rad) * radius, height, Math.Cos(rad) * radius);
                var n = new Vector3D(Math.Sin(rad), 0, Math.Cos(rad));
                positions.Add(v1);
                normals.Add(n);
                positions.Add(v2);
                normals.Add(n);
            }

            // top disc

            for (int i=0; i<steps; ++i)
            {
                indicies.Add(0);
                int j = i;
                int k = j + 1;
                j %= steps;
                k %= steps;
                indicies.Add((j+1)*2);
                indicies.Add((k+1)*2);
            }

            // border quads

            for (int i = 0; i < steps; ++i)
            {
                int j = i;
                int k = j + 1;
                j %= steps;
                k %= steps;
                indicies.Add(((j + 1) * 2) - 1);
                indicies.Add(((k + 1) * 2) + 0);
                indicies.Add(((j + 1) * 2) + 0);

                indicies.Add(((k + 1) * 2) - 1);
                indicies.Add(((k + 1) * 2) + 0);
                indicies.Add(((j + 1) * 2) - 1);
            }

            geometry.TriangleIndices = indicies;
            geometry.Positions = positions;
            geometry.Normals = normals;
            return geometry;
        }

        public static MeshGeometry3D CreateCheckerboard(double size, int steps, int odd)
        {
            var geometry = new MeshGeometry3D();
            var positions = new Point3DCollection();
            var indicies = new Int32Collection();
            var normals = new Vector3DCollection();

            for (int i=0; i<steps; ++i)
            {
                for (int j=0; j<steps; ++j)
                {
                    positions.Add(new Point3D((size * i)-(size*steps*0.5), 0, (size * j)- (size * steps * 0.5)));
                    normals.Add(new Vector3D(0, -1, 0));
                }
            }

            int n = 0;
            for (int i = 0; i < steps-1; ++i)
            {
                for (int j = 0; j < steps-1; ++j)
                {
                    n++;
                    if ((n & 1) == odd) continue;

                    int k = (i * steps) + j;
                    indicies.Add(k);
                    indicies.Add(k+1);
                    indicies.Add(k+steps);
                    indicies.Add(k+1);
                    indicies.Add(k+steps+1);
                    indicies.Add(k + steps);
                }
            }

            geometry.TriangleIndices = indicies;
            geometry.Positions = positions;
            geometry.Normals = normals;
            return geometry;
        }
    }
}
