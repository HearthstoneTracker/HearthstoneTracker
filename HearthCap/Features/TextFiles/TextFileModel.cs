using System;
using System.IO;
using Caliburn.Micro;

namespace HearthCap.Features.TextFiles
{
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
            get { return id; }
            set
            {
                if (value.Equals(id))
                {
                    return;
                }
                id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public string Filename
        {
            get { return filename; }
            set
            {
                if (value == filename)
                {
                    return;
                }
                filename = value;
                NotifyOfPropertyChange(() => Filename);
                NotifyOfPropertyChange(() => ShortFilename);
            }
        }

        public string Template
        {
            get { return template; }
            set
            {
                if (value == template)
                {
                    return;
                }
                template = value;
                NotifyOfPropertyChange(() => Template);
            }
        }

        public string ShortFilename
        {
            get
            {
                var info = new FileInfo(Filename);
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
                if (filename.Length >= cutoff)
                {
                    return filename;
                }

                if (info.FullName.Length > cutoff)
                {
                    dirname = dirname.Substring(0, cutoff - filename.Length - 4) + "\\...";
                }

                return dirname + "\\" + filename;
            }
        }
    }
}
