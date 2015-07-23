namespace HearthCap.Features.Decks
{
    using System;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Core;

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

        private static DeckModel emptyEntry = new DeckModel() { Id = Guid.Empty, Key = string.Empty, Name = string.Empty, Server = string.Empty };

        public DeckModel()
        {
            Id = Guid.NewGuid();
            Server = Server = BindableServerCollection.Instance.DefaultName;
            Created = DateTime.Now;
            Modified = Created;
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

        public string Key
        {
            get
            {
                return this.key;
            }
            set
            {
                if (value == this.key)
                {
                    return;
                }
                this.key = value;
                this.NotifyOfPropertyChange(() => this.Key);
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value == this.name)
                {
                    return;
                }
                this.name = value;
                this.NotifyOfPropertyChange(() => this.Name);
                this.NotifyOfPropertyChange(() => this.NameAndServer);
            }
        }

        public DateTime Created
        {
            get
            {
                return this.created;
            }
            set
            {
                if (value.Equals(this.created))
                {
                    return;
                }
                this.created = value;
                this.NotifyOfPropertyChange(() => this.Created);
            }
        }

        public DateTime Modified
        {
            get
            {
                return this.modified;
            }
            set
            {
                if (value.Equals(this.modified))
                {
                    return;
                }
                this.modified = value;
                this.NotifyOfPropertyChange(() => this.Modified);
            }
        }

        public string Server
        {
            get
            {
                return this.server;
            }
            set
            {
                if (value == this.server)
                {
                    return;
                }
                this.server = value;
                this.NotifyOfPropertyChange(() => this.Server);
                this.NotifyOfPropertyChange(() => this.NameAndServer);
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
            get
            {
                return this.notes;
            }
            set
            {
                if (value == this.notes)
                {
                    return;
                }
                this.notes = value;
                this.NotifyOfPropertyChange(() => this.Notes);
            }
        }

        public static DeckModel EmptyEntry
        {
            get
            {
                return emptyEntry;
            }
        }

        public DeckImage Image
        {
            get
            {
                return this.image;
            }
            set
            {
                if (Equals(value, this.image))
                {
                    return;
                }
                this.image = value;
                this.NotifyOfPropertyChange(() => this.Image);
            }
        }

        public bool Deleted
        {
            get
            {
                return this.deleted;
            }
            set
            {
                if (value.Equals(this.deleted))
                {
                    return;
                }
                this.deleted = value;
                this.NotifyOfPropertyChange(() => this.Deleted);
            }
        }
    }
}