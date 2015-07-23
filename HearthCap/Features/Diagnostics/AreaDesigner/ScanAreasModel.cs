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

    public class ScanAreasModel : PropertyChangedBase
    {
        private readonly IScanAreaProvider scanAreaProvider;

        private BindableCollection<ScanAreaModel> areas;

        private List<ScanAreas> scanAreas;

        private int baseResolution;

        public ScanAreasModel(IScanAreaProvider scanAreaProvider)
        {
            this.scanAreaProvider = scanAreaProvider;
            this.areas = new BindableCollection<ScanAreaModel>();
        }

        public IObservableCollection<ScanAreaModel> Areas
        {
            get
            {
                return this.areas;
            }
        }

        public Task Save()
        {
            return Task.Run(
                () =>
                {
                    string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                    string filename = Path.Combine(baseDir, "areas.json");
                    
                    ScanAreas areas = CreateScanAreas();
                    var newareas = this.scanAreas.Where(x => x.BaseResolution != areas.BaseResolution).ToList();
                    newareas.Add(areas);
                    var data = JsonConvert.SerializeObject(newareas, Formatting.Indented);
                    File.WriteAllText(filename, data);
                    Initialize();
                });
        }

        public ScanAreaModel AddArea(string key)
        {
            if (this.Areas.Any(x => x.Key == key)) return null;
            var model = new ScanAreaModel()
                            {
                                Width = 64,
                                Height = 64,
                                Key = key,
                                
                            };

            this.areas.Add(model);
            return model;
        }

        private void Initialize()
        {
            scanAreaProvider.Load();
            this.scanAreas = new List<ScanAreas>(this.scanAreaProvider.GetScanAreas());
            RefreshAreas();
        }

        private void RefreshAreas()
        {
            this.areas.Clear();
            var models = new List<ScanAreaModel>();
            var area = scanAreas.FirstOrDefault(x => x.BaseResolution == this.BaseResolution);
            if (area == null)
            {
                area = new ScanAreas() { BaseResolution = this.BaseResolution, Areas = new List<ScanArea>() };
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
                Initialize();
                this.NotifyOfPropertyChange(() => this.BaseResolution);
            }
        }

        private ScanAreas CreateScanAreas()
        {
            var result = new ScanAreas()
                             {
                                 BaseResolution = BaseResolution
                             };

            foreach (var model in this.Areas)
            {
                result.Areas.Add(new ScanArea()
                                     {
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