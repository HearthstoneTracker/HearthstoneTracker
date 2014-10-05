// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextFileModel.cs" company="">
//   
// </copyright>
// <summary>
//   The text file model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.TextFiles
{
    using System;
    using System.IO;

    using Caliburn.Micro;

    /// <summary>
    /// The text file model.
    /// </summary>
    public class TextFileModel : PropertyChangedBase
    {
        /// <summary>
        /// The cutoff.
        /// </summary>
        private const int cutoff = 40;

        /// <summary>
        /// The id.
        /// </summary>
        private Guid id;

        /// <summary>
        /// The filename.
        /// </summary>
        private string filename;

        /// <summary>
        /// The template.
        /// </summary>
        private string template;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFileModel"/> class.
        /// </summary>
        public TextFileModel()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.id;
            }

            set
            {
                if (value.Equals(this.id))
                {
                    return;
                }

                this.id = value;
                this.NotifyOfPropertyChange(() => this.Id);
            }
        }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        public string Filename
        {
            get
            {
                return this.filename;
            }

            set
            {
                if (value == this.filename)
                {
                    return;
                }

                this.filename = value;
                this.NotifyOfPropertyChange(() => this.Filename);
                this.NotifyOfPropertyChange(() => this.ShortFilename);
            }
        }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        public string Template
        {
            get
            {
                return this.template;
            }

            set
            {
                if (value == this.template)
                {
                    return;
                }

                this.template = value;
                this.NotifyOfPropertyChange(() => this.Template);
            }
        }

        /// <summary>
        /// Gets the short filename.
        /// </summary>
        public string ShortFilename
        {
            get
            {
                var info = new FileInfo(this.Filename);
                if (info.FullName.Length <= cutoff)
                {
                    return info.FullName;
                }

                var filename = info.Name;
                if (string.IsNullOrEmpty(filename))
                {
                    return string.Empty;
                }

                var dirname = info.DirectoryName;
                if (string.IsNullOrEmpty(dirname))
                {
                    return filename;
                }

                if (filename.Length >= cutoff) return filename;

                if (info.FullName.Length > cutoff)
                {
                    dirname = dirname.Substring(0, cutoff - filename.Length - 4) + "\\...";
                }

                return dirname + "\\" + filename;
            }
        }
    }
}