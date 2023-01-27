using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ARTGApp
{
    internal class MainViewModel : ObservableObject
    {
        ProjectionCamera camera;
        Light light;
        TerrainModel terrain;

        public MainViewModel() 
        {
            camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 7500, 20000);
            camera.LookDirection = new Vector3D(0,-7500,-20000);

            light = new DirectionalLight(Colors.White, new Vector3D(-1, -1, -2));

            terrain = new TerrainModel(1000, 1000);
        }

        public Camera Camera => camera;

        public Light LightSource => light;

        public TerrainModel Terrain => terrain;

        #region Commands

        #endregion
    }
}
