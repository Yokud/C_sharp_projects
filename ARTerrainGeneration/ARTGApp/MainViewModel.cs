using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARTGApp
{
    internal class MainViewModel : ObservableObject
    {
        TerrainModel TerrainModel { get; set; }

        public MainViewModel() 
        {
            //TerrainModel = new TerrainModel(1000, 1000);
        }

        #region Commands

        #endregion
    }
}
