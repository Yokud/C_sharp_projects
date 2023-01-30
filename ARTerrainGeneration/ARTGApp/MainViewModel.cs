using HeightMapLib;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using UI;

namespace ARTGApp
{
    internal class MainViewModel : ObservableObject
    {
        ProjectionCamera camera;
        Light light;
        TerrainModel terrain;

        readonly string defaultStatusString = "Карта высот ещё не сгенерирована.";
        string statusString;

        double currRotationValue = 0;

        public MainViewModel() 
        {
            statusString = defaultStatusString;

            camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 7500, 20000);
            camera.LookDirection = new Vector3D(0,-7500,-20000);

            light = new DirectionalLight(Colors.White, new Vector3D(-1, -1, -2));
        }

        #region Properties
        public Camera Camera => camera;

        public Light LightSource => light;

        public Model3D? Terrain => terrain?.Content;

        public bool IsTerrainGenerated => terrain is not null;

        public string StatusString
        {
            get => statusString;
            set 
            { 
                statusString = value;
                OnPropertyChanged(nameof(StatusString));
            }
        }
        #endregion

        #region Commands
        RelayCommand generateTerrainCommand;
        public RelayCommand GenerateTerrainCommand
        {
            get => generateTerrainCommand ??= new RelayCommand(obj =>
            {
                GenMapWindow wnd = new GenMapWindow();
                wnd.ShowDialog();

                if (wnd.DialogResult.HasValue && wnd.DialogResult.Value)
                {
                    terrain = new TerrainModel(wnd.MapWidth, wnd.MapHeight, new PerlinNoise(wnd.Scale, wnd.Octaves, wnd.Lacunarity, wnd.Persistence, wnd.Seed));
                    OnPropertyChanged(nameof(Terrain));
                    OnPropertyChanged(nameof(IsTerrainGenerated));
                    StatusString = $"Карта высот была сгенерирована с параметрами:\n- Ширина карты высот: {wnd.MapWidth}\n- Высота карты высот: {wnd.MapHeight}\n- Масштаб: {wnd.Scale}\n- Кол-во октав: {wnd.Octaves}\n- Лакунарность: {wnd.Lacunarity}\n- Стойкость: {wnd.Persistence}\n- Ключ генерации: {wnd.Seed}";
                }
            });
        }

        RelayCommand getMarkerCommand;
        public RelayCommand GetMarkerCommand
        {
            get => getMarkerCommand ??= new RelayCommand(obj =>
            {

            });
        }

        RelayCommand rotateTerrainCommand;
        public RelayCommand RotateTerrainCommand
        {
            get => rotateTerrainCommand ??= new RelayCommand(obj =>
            {
                var strVal = obj as string;
                if (string.IsNullOrEmpty(strVal))
                    return;
                var canRotate = double.TryParse(strVal, NumberStyles.Number, CultureInfo.InvariantCulture, out double rotationDelta);
                if (!canRotate)
                    return;
                
                currRotationValue += rotationDelta;
                terrain.Content.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), currRotationValue));
                OnPropertyChanged(nameof(Terrain));
            });
        }

        RelayCommand scaleTerrainCommand;
        #endregion
    }
}
