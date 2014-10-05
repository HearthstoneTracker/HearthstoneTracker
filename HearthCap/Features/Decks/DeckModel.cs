// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeckModel.cs" company="">
//   
// </copyright>
// <summary>
//   The deck model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Decks
{
    using System;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Core;

    /// <summary>
    /// The deck model.
    /// </summary>
    public class DeckModel : PropertyChangedBase
    {
        /// <summary>
        /// The id.
        /// </summary>
        private Guid id;

        /// <summary>
        /// The key.
        /// </summary>
        private string key;

        /// <summary>
        /// The name.
        /// </summary>
        private string name;

        /// <summary>
        /// The created.
        /// </summary>
        private DateTime created;

        /// <summary>
        /// The modified.
        /// </summary>
        private DateTime modified;

        /// <summary>
        /// The server.
        /// </summary>
        private string server;

        /// <summary>
        /// The image.
        /// </summary>
        private DeckImage image;

        /// <summary>
        /// The notes.
        /// </summary>
        private string notes;

        /// <summary>
        /// The deleted.
        /// </summary>
        private bool deleted;

        /// <summary>
        /// The empty entry.
        /// </summary>
        private static DeckModel emptyEntry = new DeckModel { Id = Guid.Empty, Key = string.Empty, Name = string.Empty, Server = string.Empty };

        /// <summary>
        /// Initializes a new instance of the <see cref="DeckModel"/> class.
        /// </summary>
        public DeckModel()
        {
            this.Id = Guid.NewGuid();
            this.Server = this.Server = BindableServerCollection.Instance.DefaultName;
            this.Created = DateTime.Now;
            this.Modified = this.Created;
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
        /// Gets or sets the key.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the modified.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
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

        /// <summary>
        /// Gets the name and server.
        /// </summary>
        public string NameAndServer
        {
            get
            {
                if (string.IsNullOrEmpty(this.Name))
                {
                    return string.Empty;
                }

                return string.Format("{0} ({1})", this.Name, this.Server);
            }
        }

        /// <summary>
        /// Gets the name and slot.
        /// </summary>
        public string NameAndSlot
        {
            get
            {
                if (string.IsNullOrEmpty(this.Name))
                {
                    return string.Empty;
                }

                if (string.IsNullOrEmpty(this.Key))
                {
                    return this.Name;
                }

                return string.Format("#{0}: {1}", this.Key, this.Name);
            }
        }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
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

        /// <summary>
        /// Gets the empty entry.
        /// </summary>
        public static DeckModel EmptyEntry
        {
            get
            {
                return emptyEntry;
            }
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether deleted.
        /// </summary>
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