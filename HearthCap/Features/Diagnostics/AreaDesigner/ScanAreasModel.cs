using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using HearthCap.Core.GameCapture.HS;
using Newtonsoft.Json;

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    public class ScanAreasModel : PropertyChangedBase
    {
        private readonly IScanAreaProvider scanAreaProvider;

        private readonly BindableCollection<ScanAreaModel> areas;

        private List<ScanAreas> scanAreas;

        private int baseResolution;

        public ScanAreasModel(IScanAreaProvider scanAreaProvider)
        {
            this.scanAreaProvider = scanAreaProvider;
            areas = new BindableCollection<ScanAreaModel>();
        }

        public IObservableCollection<ScanAreaModel> Areas
        {
            get { return areas; }
        }

        public Task Save()
        {
            return Task.Run(
                () =>
                    {
                        var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                        var filename = Path.Combine(baseDir, "areas.json");

                        var areas = CreateScanAreas();
                        var newareas = scanAreas.Where(x => x.BaseResolution != areas.BaseResolution).ToList();
                        newareas.Add(areas);
                        var data = JsonConvert.SerializeObject(newareas, Formatting.Indented);
                        File.WriteAllText(filename, data);
                        Initialize();
                    });
        }

        public ScanAreaModel AddArea(string key)
        {
            if (Areas.Any(x => x.Key == key))
            {
                return null;
            }
            var model = new ScanAreaModel
                {
                    Width = 64,
                    Height = 64,
                    Key = key
                };

            areas.Add(model);
            return model;
        }

        private void Initialize()
        {
            scanAreaProvider.Load();
            scanAreas = new List<ScanAreas>(scanAreaProvider.GetScanAreas());
            RefreshAreas();
        }

        private void RefreshAreas()
        {
            areas.Clear();
            var models = new List<ScanAreaModel>();
            var area = scanAreas.FirstOrDefault(x => x.BaseResolution == BaseResolution);
            if (area == null)
            {
                area = new ScanAreas { BaseResolution = BaseResolution, Areas = new List<ScanArea>() };
                scanAreas.Add(area);
                return;
            }

            foreach (var scanArea in area.Areas)
            {
                var model = new ScanAreaModel(scanArea);
                models.Add(model);
            }

            areas.AddRange(models);
        }

        public int BaseResolution
        {
            get { return baseResolution; }
            set
            {
                if (value == baseResolution)
                {
                    return;
                }
                baseResolution = value;
                Initialize();
                NotifyOfPropertyChange(() => BaseResolution);
            }
        }

        private ScanAreas CreateScanAreas()
        {
            var result = new ScanAreas
                {
                    BaseResolution = BaseResolution
                };

            foreach (var model in Areas)
            {
                result.Areas.Add(new ScanArea
                    {
                        Key = model.Key,
                        X = model.X,
                        Y = model.Y,
                        Width = model.Width,
                        Height = model.Height,
                        Hash = model.Hash,
                        BaseResolution = model.BaseResolution,
                        Image = model.ImageLocation,
                        Mostly = model.Mostly
                    });
            }

            return result;
        }
    }
}
