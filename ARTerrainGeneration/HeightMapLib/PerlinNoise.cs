using System;
using System.Numerics;

namespace HeightMapLib
{
    public class PerlinNoise : ILandGenerator, INoisable
    {
        int scale, octaves, seed;
        float lacunarity, persistence;

        public int Scale
        {
            get
            {
                return scale;
            }

            set
            {
                if (value > 0)
                    scale = value;
                else
                    throw new Exception("Scale is positive value");
            }
        }
        public int Octaves
        {
            get
            {
                return octaves;
            }

            set
            {
                if (value > 0)
                    octaves = value;
                else
                    throw new Exception("Octaves is positive value");
            }
        }
        public float Lacunarity
        {
            get
            {
                return lacunarity;
            }

            set
            {
                if (value > 0)
                    lacunarity = value;
                else
                    throw new Exception("Lacunarity is positive value");
            }
        }
        public float Persistence
        {
            get
            {
                return persistence;
            }

            set
            {
                if (value > 0)
                    persistence = value;
                else
                    throw new Exception("Persistence is positive value");
            }
        }

        public Vector2[] Gradients { get; private set; }
        private int[] SeedNums { get; set; }
        public int Seed
        {
            get
            {
                return seed;
            }

            set
            {
                if (value >= 0)
                    seed = value;
                else
                    throw new Exception("Seed is positive value");
            }
        }

        public PerlinNoise(int scale, int octaves = 1, float lacunarity = 2f, float persistence = 0.5f, int seed = -1)
        {
            Scale = scale;
            Octaves = octaves;
            Lacunarity = lacunarity;
            Persistence = persistence;

            Gradients = new Vector2[256];
            for (int i = 0; i < Gradients.Length; i++)
            {
                double val = 2.0 * Math.PI / 256 * i;
                Gradients[i].X = (float)Math.Cos(val);
                Gradients[i].Y = (float)Math.Sin(val);
            }

            Random rd;
            if (seed == -1)
            {
                rd = new Random();
                Seed = Environment.TickCount;
            }
            else
            {
                Seed = seed;
                rd = new Random(seed);
            }

            SeedNums = new int[256];
            for (int i = 0; i < SeedNums.Length; i++)
                SeedNums[i] = rd.Next(0, SeedNums.Length);
        }

        public float GenNoise(int x, int y)
        {
            Vector2 pos = new Vector2((float)x / Scale, (float)y / Scale);

            float x0 = (float)Math.Floor(pos.X);
            float x1 = x0 + 1;
            float y0 = (float)Math.Floor(pos.Y);
            float y1 = y0 + 1;

            Vector2 g0 = GetGradient(x0, y0);
            Vector2 g1 = GetGradient(x1, y0);
            Vector2 g2 = GetGradient(x0, y1);
            Vector2 g3 = GetGradient(x1, y1);

            Vector2 d0 = new Vector2(pos.X - x0, pos.Y - y0);
            Vector2 d1 = new Vector2(pos.X - x1, pos.Y - y0);
            Vector2 d2 = new Vector2(pos.X - x0, pos.Y - y1);
            Vector2 d3 = new Vector2(pos.X - x1, pos.Y - y1);

            float sd0 = Vector2.Dot(g0, d0);
            float sd1 = Vector2.Dot(g1, d1);
            float sd2 = Vector2.Dot(g2, d2);
            float sd3 = Vector2.Dot(g3, d3);

            float sx = SmootherStep(d0.X);
            float sy = SmootherStep(d0.Y);

            float blendx1 = sd0 + sx * (sd1 - sd0);
            float blendx2 = sd2 + sx * (sd3 - sd2);
            float blendy = blendx1 + sy * (blendx2 - blendx1);

            return blendy;
        }

        public float[,] GenMap(int width, int height)
        {
            float[,] map = new float[width, height];

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    float amplitude = 1;
                    float max_amp = 0;
                    int temp_octs = Octaves;
                    int temp_scale = scale;

                    while (temp_octs > 0)
                    {
                        map[i, j] += GenNoise(i, j) * amplitude;

                        max_amp += amplitude;
                        amplitude *= persistence;
                        scale = (int)Math.Round(scale / lacunarity, MidpointRounding.AwayFromZero);
                        temp_octs--;
                    }

                    map[i, j] /= max_amp;
                    scale = temp_scale;
                }

            return map;
        }

        private Vector2 GetGradient(float x, float y)
        {
            int hash = (int)((((int)x * 1836311903) ^ ((int)y * 2971215073) + 4807526976) & 1023);
            return Gradients[SeedNums[Math.Abs(hash) % SeedNums.Length]];
        }

        private float SmootherStep(float t) => t * t * t * (6 * t * t - 15 * t + 10);
    }
}
