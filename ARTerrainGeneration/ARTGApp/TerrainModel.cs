using HeightMapLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace ARTGApp
{
    class TerrainModel : ModelVisual3D
    {
        HeightMap heightMap;
        ILandGenerator generator;

        MeshGeometry3D mesh;
        Material material;

        public TerrainModel(int width, int height, ILandGenerator landGenerator)
        {
            generator = landGenerator;
            heightMap = new HeightMap(width, height, generator);

            mesh = GetMesh();
            material = new DiffuseMaterial(Brushes.LightGreen);

            GeometryModel3D model3D = new GeometryModel3D();
            model3D.Geometry = mesh;
            model3D.Material = material;

            Content = model3D;
        }

        public TerrainModel(int width, int height) : this(width, height, new PerlinNoise(192))
        {

        }

        MeshGeometry3D GetMesh() 
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection();
            mesh.TriangleIndices = new Int32Collection();

            float topLeftX = heightMap.Width / 2f;
            float topLeftZ = heightMap.Height / 2f;
            int vert_index = 0;

            float height_coef = (float)Math.Sqrt(heightMap.Width * heightMap.Height);

            float min_z = float.PositiveInfinity;

            for (int y = 0; y < heightMap.Height; y++)
                for (int x = 0; x < heightMap.Width; x++)
                    min_z = heightMap[x, y] * height_coef < min_z ? heightMap[x, y] * height_coef : min_z;

            for (int y = 0; y < heightMap.Height; y++) 
            {
                for (int x = 0; x < heightMap.Width; x++) 
                {
                    mesh.Positions.Add(new Point3D((x - topLeftX) * 10f, heightMap[x, y] * height_coef - min_z, (y - topLeftZ) * 10f));

                    if (x < heightMap.Width - 1 && y < heightMap.Height - 1)
                    {
                        mesh.TriangleIndices.Add(vert_index);
                        mesh.TriangleIndices.Add(vert_index + heightMap.Width);
                        mesh.TriangleIndices.Add(vert_index + heightMap.Width + 1);

                        mesh.TriangleIndices.Add(vert_index + heightMap.Width + 1);
                        mesh.TriangleIndices.Add(vert_index + 1);
                        mesh.TriangleIndices.Add(vert_index);
                    }

                    vert_index++;
                }
            }

            return mesh;
        }
    }
}
