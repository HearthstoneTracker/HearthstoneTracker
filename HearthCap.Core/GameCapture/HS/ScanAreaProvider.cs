// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScanAreaProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The scan area provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Web.Script.Serialization;

    /// <summary>
    /// The scan area provider.
    /// </summary>
    [Export(typeof(IScanAreaProvider))]
    public class ScanAreaProvider : IScanAreaProvider
    {
        /// <summary>
        /// The scan areas.
        /// </summary>
        private IEnumerable<ScanAreas> scanAreas;

        /// <summary>
        /// The load.
        /// </summary>
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

        /// <summary>
        /// The get image.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        public Image GetImage(string name)
        {
            var asm = Assembly.GetExecutingAssembly();
            name = name.Replace("\\", ".");
            return Image.FromStream(asm.GetManifestResourceStream("HearthCap.Core." + name));
        }

        /// <summary>
        /// The get scan areas.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<ScanAreas> GetScanAreas()
        {
            if (this.scanAreas == null)
            {
                this.Load();
            }

            return this.scanAreas;
        }
    }

    /// <summary>
    /// The embedded resource.
    /// </summary>
    public class EmbeddedResource : IEmbeddedResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedResource"/> class.
        /// </summary>
        /// <param name="assembly">
        /// The assembly.
        /// </param>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        public EmbeddedResource(Assembly assembly, string resourceName)
            : this(assembly, resourceName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedResource"/> class.
        /// </summary>
        /// <param name="assembly">
        /// The assembly.
        /// </param>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public EmbeddedResource(Assembly assembly, string resourceName, string fileName)
        {
            this.Assembly = assembly;
            this.ResourceName = resourceName;
            this.FileName = fileName;
        }

        /// <summary>
        /// Gets the resource name.
        /// </summary>
        public string ResourceName { get; private set; }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the assembly.
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// The get resource stream.
        /// </summary>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public Stream GetResourceStream()
        {
            var stream = (this.Assembly ?? Assembly.GetExecutingAssembly()).GetManifestResourceStream(this.ResourceName);
            if (stream != null) return stream;
            throw new InvalidOperationException("Resource not found: " + this.ResourceName);
        }
    }

    /// <summary>
    /// The EmbeddedResource interface.
    /// </summary>
    public interface IEmbeddedResource
    {
        /// <summary>
        /// Gets the resource name.
        /// </summary>
        string ResourceName { get; }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Gets the assembly.
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// The get resource stream.
        /// </summary>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        Stream GetResourceStream();
    }
}