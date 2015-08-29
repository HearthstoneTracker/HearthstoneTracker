using System;
using Caliburn.Micro;
using HearthCap.Data;
using HearthCap.Features.Core;

namespace HearthCap.Features.Decks
{
    public class DeckModel : PropertyChangedBase
    {
        private Guid id;

        private string key;

        private string name;

        private DateTime created;

        private DateTime modified;

        private string server;

        private DeckImage image;

        private string notes;

        private bool deleted;

        private static readonly DeckModel emptyEntry = new DeckModel { Id = Guid.Empty, Key = string.Empty, Name = string.Empty, Server = string.Empty };

        public DeckModel()
        {
            Id = Guid.NewGuid();
            Server = Server = BindableServerCollection.Instance.DefaultName;
            Created = DateTime.Now;
            Modified = Created;
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

        public string Key
        {
            get { return key; }
            set
            {
                if (value == key)
                {
                    return;
                }
                key = value;
                NotifyOfPropertyChange(() => Key);
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (value == name)
                {
                    return;
                }
                name = value;
                NotifyOfPropertyChange(() => Name);
                NotifyOfPropertyChange(() => NameAndServer);
            }
        }

        public DateTime Created
        {
            get { return created; }
            set
            {
                if (value.Equals(created))
                {
                    return;
                }
                created = value;
                NotifyOfPropertyChange(() => Created);
            }
        }

        public DateTime Modified
        {
            get { return modified; }
            set
            {
                if (value.Equals(modified))
                {
                    return;
                }
                modified = value;
                NotifyOfPropertyChange(() => Modified);
            }
        }

        public string Server
        {
            get { return server; }
            set
            {
                if (value == server)
                {
                    return;
                }
                server = value;
                NotifyOfPropertyChange(() => Server);
                NotifyOfPropertyChange(() => NameAndServer);
            }
        }

        public string NameAndServer
        {
            get
            {
                if (String.IsNullOrEmpty(Name))
                {
                    return String.Empty;
                }

                return String.Format("{0} ({1})", Name, Server);
            }
        }

        public string NameAndSlot
        {
            get
            {
                if (String.IsNullOrEmpty(Name))
                {
                    return String.Empty;
                }

                if (String.IsNullOrEmpty(Key))
                {
                    return Name;
                }

                return String.Format("#{0}: {1}", Key, Name);
            }
        }

        public string Notes
        {
            get { return notes; }
            set
            {
                if (value == notes)
                {
                    return;
                }
                notes = value;
                NotifyOfPropertyChange(() => Notes);
            }
        }

        public static DeckModel EmptyEntry
        {
            get { return emptyEntry; }
        }

        public DeckImage Image
        {
            get { return image; }
            set
            {
                if (Equals(value, image))
                {
                    return;
                }
                image = value;
                NotifyOfPropertyChange(() => Image);
            }
        }

        public bool Deleted
        {
            get { return deleted; }
            set
            {
                if (value.Equals(deleted))
                {
                    return;
                }
                deleted = value;
                NotifyOfPropertyChange(() => Deleted);
            }
        }
    }
}
