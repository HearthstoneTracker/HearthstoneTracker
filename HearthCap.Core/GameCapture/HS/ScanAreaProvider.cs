using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

namespace HearthCap.Core.GameCapture.HS
{
    [Export(typeof(IScanAreaProvider))]
    public class ScanAreaProvider : IScanAreaProvider
    {
        private IEnumerable<ScanAreas> scanAreas;

        public void Load()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var filename = Path.Combine(baseDir, "areas.json");

            string json;
            if (File.Exists(filename))
            {
                json = File.ReadAllText(filename);
            }
            else
            {
                var asm = Assembly.GetExecutingAssembly();
                using (var sr = new StreamReader(asm.GetManifestResourceStream("HearthCap.Core.data.areas.json")))
                {
                    json = sr.ReadToEnd();
                }
            }

            var s = new JavaScriptSerializer();
            var data = s.Deserialize<List<ScanAreas>>(json);

            scanAreas = data;
        }

        public Image GetImage(string name)
        {
            var asm = Assembly.GetExecutingAssembly();
            name = name.Replace("\\", ".");
            return Image.FromStream(asm.GetManifestResourceStream("HearthCap.Core." + name));
        }

        public IEnumerable<ScanAreas> GetScanAreas()
        {
            if (scanAreas == null)
            {
                Load();
            }

            return scanAreas;
        }
    }

    public class EmbeddedResource : IEmbeddedResource
    {
        public EmbeddedResource(Assembly assembly, string resourceName)
            : this(assembly, resourceName, null)
        {
        }

        public EmbeddedResource(Assembly assembly, string resourceName, string fileName)
        {
            Assembly = assembly;
            ResourceName = resourceName;
            FileName = fileName;
        }

        public string ResourceName { get; private set; }

        public string FileName { get; private set; }

        public Assembly Assembly { get; private set; }

        public Stream GetResourceStream()
        {
            var stream = (Assembly ?? Assembly.GetExecutingAssembly()).GetManifestResourceStream(ResourceName);
            if (stream != null)
            {
                return stream;
            }
            throw new InvalidOperationException("Resource not found: " + ResourceName);
        }
    }

    public interface IEmbeddedResource
    {
        string ResourceName { get; }

        string FileName { get; }

        Assembly Assembly { get; }

        Stream GetResourceStream();
    }
}
