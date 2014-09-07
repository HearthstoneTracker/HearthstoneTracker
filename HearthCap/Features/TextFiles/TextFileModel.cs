namespace HearthCap.Features.TextFiles
{
    using System;
    using System.IO;

    using Caliburn.Micro;

    public class TextFileModel : PropertyChangedBase
    {
        private const int cutoff = 40;
        private Guid id;

        private string filename;

        private string template;

        public TextFileModel()
        {
            Id = Guid.NewGuid();
        }

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
                if (String.IsNullOrEmpty(filename))
                {
                    return String.Empty;
                }
                var dirname = info.DirectoryName;
                if (String.IsNullOrEmpty(dirname))
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