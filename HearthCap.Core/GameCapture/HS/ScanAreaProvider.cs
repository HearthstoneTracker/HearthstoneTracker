namespace HearthCap.Core.GameCapture.HS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Web.Script.Serialization;

    [Export(typeof(IScanAreaProvider))]
    public class ScanAreaProvider : IScanAreaProvider
    {
        private IEnumerable<ScanAreas> scanAreas;

        public void Load()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string filename = Path.Combine(baseDir, "areas.json");

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

            this.scanAreas = data;
        }

        public Image GetImage(string name)
        {
            var asm = Assembly.GetExecutingAssembly();
            name = name.Replace("\\", ".");
            return Image.FromStream(asm.GetManifestResourceStream("HearthCap.Core." + name));
        }

        public IEnumerable<ScanAreas> GetScanAreas()
        {
            if (this.scanAreas == null)
            {
                this.Load();
            }

            return this.scanAreas;
        }
    }

    public class EmbeddedResource : IEmbeddedResource
    {
        public EmbeddedResource(Assembly assembly, string resourceName)
            : this(assembly, resourceName, (string)null)
        {
        }

        public EmbeddedResource(Assembly assembly, string resourceName, string fileName)
        {
            this.Assembly = assembly;
            this.ResourceName = resourceName;
            this.FileName = fileName;
        }

        public string ResourceName { get; private set; }

        public string FileName { get; private set; }

        public Assembly Assembly { get; private set; }

        public Stream GetResourceStream()
        {
            var stream = (this.Assembly ?? Assembly.GetExecutingAssembly()).GetManifestResourceStream(this.ResourceName);
            if (stream != null) return stream;
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