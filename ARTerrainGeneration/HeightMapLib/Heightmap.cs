using System;
using System.Drawing;

namespace HeightMapLib
{
    public delegate float NoiseExpresion(float f);
    public interface ILandGenerator
    {
        int Seed { get; set; }
        float[,] GenMap(int width, int height);
    }

    public class HeightMap
    {
        int width, height;
        NoiseExpresion expresion;

        ILandGenerator LandGenerator { get; set; }

        public HeightMap(int width, int height, ILandGenerator lg, NoiseExpresion exp = null)
        {
            Width = width;
            Height = height;
            LandGenerator = lg;
            expresion = exp;

            GenMap();
        }

        public HeightMap(HeightMap h)
        {
            Width = h.Width;
            Height = h.Height;
            NoiseMap = h.NoiseMap;
            LandGenerator = h.LandGenerator;
            Seed = h.Seed;
        }

        public float[,] NoiseMap { get; private set; }

        public int Width
        {
            get => width;
            set
            {
                if (value > 0)
                    width = value;
                else
                    throw new Exception("Width is positive value");
            }
        }

        public int Height
        {
            get
            {
                return height;
            }

            set
            {
                if (value > 0)
                    height = value;
                else
                    throw new Exception("Height is positive value");
            }
        }

        public int Seed
        {
            get => LandGenerator.Seed;
            set => LandGenerator.Seed = value;
        }

        private void GenMap()
        {
            NoiseMap = LandGenerator.GenMap(Width, Height);

            if (expresion != null)
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        NoiseMap[i, j] = expresion(NoiseMap[i, j]);
        }

        public float this[int i, int j]
        {
            get => NoiseMap[i, j];
            set => NoiseMap[i, j] = value;
        }

        public static HeightMap Add(HeightMap h1, HeightMap h2)
        {
            HeightMap h_temp = new HeightMap(h1);

            if (h1.Width != h2.Width || h1.Height != h2.Height)
                throw new Exception("Sizes isn't equal");

            for (int i = 0; i < h1.Width; i++)
                for (int j = 0; j < h1.Height; j++)
                    h_temp[i, j] += h2[i, j];

            return h_temp;
        }

        public static HeightMap Subtract(HeightMap h1, HeightMap h2)
        {
            HeightMap h_temp = new HeightMap(h1);

            if (h1.Width != h2.Width || h1.Height != h2.Height)
                throw new Exception("Sizes isn't equal");

            for (int i = 0; i < h1.Width; i++)
                for (int j = 0; j < h1.Height; j++)
                    h_temp[i, j] -= h2[i, j];

            return h_temp;
        }

        public static HeightMap MultSingle(HeightMap h, float val)
        {
            HeightMap h_temp = new HeightMap(h);

            for (int i = 0; i < h.Width; i++)
                for (int j = 0; j < h.Height; j++)
                    h_temp[i, j] *= val;

            return h_temp;
        }

        public static HeightMap operator +(HeightMap h1, HeightMap h2) => Add(h1, h2);
        public static HeightMap operator -(HeightMap h1, HeightMap h2) => Subtract(h1, h2);
        public static HeightMap operator *(HeightMap h, float val) => MultSingle(h, val);

        private (float, float) MinMax()
        {
            float max = NoiseMap[0, 0];
            float min = max;

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    min = NoiseMap[i, j] < min ? NoiseMap[i, j] : min;
                    max = NoiseMap[i, j] > max ? NoiseMap[i, j] : max;
                }

            return (min, max);
        }

        public void Normalize()
        {
            var (h_min, h_max) = MinMax();
            float delta = h_max - h_min;

            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    NoiseMap[i, j] = (NoiseMap[i, j] - h_min) / delta;
        }

        //public void SaveToBmp(string path, string name)
        //{
        //    byte[,] hm = new byte[width, height];
        //    var (h_min, h_max) = MinMax();
        //    float delta = h_max - h_min;

        //    for (int i = 0; i < Width; i++)
        //        for (int j = 0; j < Height; j++)
        //            hm[i, j] = (byte)((NoiseMap[i, j] - h_min) / delta * 255);

        //    Bitmap bmp = new Bitmap(width, height);
        //    for (int i = 0; i < width; i++)
        //        for (int j = 0; j < height; j++)
        //            bmp.SetPixel(i, j, Color.FromArgb(hm[i, j], hm[i, j], hm[i, j]));

        //    bmp.Save(path + name + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        //}
    }
}
