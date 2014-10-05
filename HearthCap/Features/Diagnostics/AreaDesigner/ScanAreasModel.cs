// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScanAreasModel.cs" company="">
//   
// </copyright>
// <summary>
//   The scan areas model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.HS;

    using Newtonsoft.Json;

    /// <summary>
    /// The scan areas model.
    /// </summary>
    public class ScanAreasModel : PropertyChangedBase
    {
        /// <summary>
        /// The scan area provider.
        /// </summary>
        private readonly IScanAreaProvider scanAreaProvider;

        /// <summary>
        /// The areas.
        /// </summary>
        private BindableCollection<ScanAreaModel> areas;

        /// <summary>
        /// The scan areas.
        /// </summary>
        private List<ScanAreas> scanAreas;

        /// <summary>
        /// The base resolution.
        /// </summary>
        private int baseResolution;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanAreasModel"/> class.
        /// </summary>
        /// <param name="scanAreaProvider">
        /// The scan area provider.
        /// </param>
        public ScanAreasModel(IScanAreaProvider scanAreaProvider)
        {
            this.scanAreaProvider = scanAreaProvider;
            this.areas = new BindableCollection<ScanAreaModel>();
        }

        /// <summary>
        /// Gets the areas.
        /// </summary>
        public IObservableCollection<ScanAreaModel> Areas
        {
            get
            {
                return this.areas;
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task Save()
        {
            return Task.Run(
                () =>
                {
                    string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                    string filename = Path.Combine(baseDir, "areas.json");
                    
                    ScanAreas areas = this.CreateScanAreas();
                    var newareas = this.scanAreas.Where(x => x.BaseResolution != areas.BaseResolution).ToList();
                    newareas.Add(areas);
                    var data = JsonConvert.SerializeObject(newareas, Formatting.Indented);
                    File.WriteAllText(filename, data);
                    this.Initialize();
                });
        }

        /// <summary>
        /// The add area.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ScanAreaModel"/>.
        /// </returns>
        public ScanAreaModel AddArea(string key)
        {
            if (this.Areas.Any(x => x.Key == key)) return null;
            var model = new ScanAreaModel {
                                Width = 64, 
                                Height = 64, 
                                Key = key, 
                                
                            };

            this.areas.Add(model);
            return model;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        private void Initialize()
        {
            this.scanAreaProvider.Load();
            this.scanAreas = new List<ScanAreas>(this.scanAreaProvider.GetScanAreas());
            this.RefreshAreas();
        }

        /// <summary>
        /// The refresh areas.
        /// </summary>
        private void RefreshAreas()
        {
            this.areas.Clear();
            var models = new List<ScanAreaModel>();
            var area = this.scanAreas.FirstOrDefault(x => x.BaseResolution == this.BaseResolution);
            if (area == null)
            {
                area = new ScanAreas { BaseResolution = this.BaseResolution, Areas = new List<ScanArea>() };
                this.scanAreas.Add(area);
                return;
            }

            foreach (var scanArea in area.Areas)
            {
                var model = new ScanAreaModel(scanArea);
                models.Add(model);
            }

            this.areas.AddRange(models);
        }

        /// <summary>
        /// Gets or sets the base resolution.
        /// </summary>
        public int BaseResolution
        {
            get
            {
                return this.baseResolution;
            }

            set
            {
                if (value == this.baseResolution)
                {
                    return;
                }

                this.baseResolution = value;
                this.Initialize();
                this.NotifyOfPropertyChange(() => this.BaseResolution);
            }
        }

        /// <summary>
        /// The create scan areas.
        /// </summary>
        /// <returns>
        /// The <see cref="ScanAreas"/>.
        /// </returns>
        private ScanAreas CreateScanAreas()
        {
            var result = new ScanAreas {
                                 BaseResolution = this.BaseResolution
                             };

            foreach (var model in this.Areas)
            {
                result.Areas.Add(new ScanArea {
                                         Key = model.Key, 
                                         X = model.X, 
                                         Y = model.Y, 
                                         Width = model.Width, 
                                         Height = model.Height, 
                                         Hash = model.Hash, 
                                         BaseResolution = model.BaseResolution, 
                                         Image = model.ImageLocation, 
                                         Mostly = model.Mostly, 
                                     });
            }

            return result;
        }
    }
}