// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SupportViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The support view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Support
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;

    using Caliburn.Micro;
    using Caliburn.Micro.Recipes.Filters;

    using HearthCap.Features.Analytics;

    using NLog;

    using UserVoice;

    using LogManager = NLog.LogManager;

    /// <summary>
    /// The support view model.
    /// </summary>
    [Export(typeof(SupportViewModel))]
    public class SupportViewModel : Screen, IDataErrorInfo
    {
        /// <summary>
        /// The property getters.
        /// </summary>
        private readonly Dictionary<string, Func<Screen, object>> propertyGetters;

        /// <summary>
        /// The validators.
        /// </summary>
        private readonly Dictionary<string, ValidationAttribute[]> validators;

        /// <summary>
        /// The log.
        /// </summary>
        private static Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The email.
        /// </summary>
        private string email;

        /// <summary>
        /// The message.
        /// </summary>
        private string message;

        /// <summary>
        /// The attach log.
        /// </summary>
        private bool attachLog;

        /// <summary>
        /// The subject.
        /// </summary>
        private string subject;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportViewModel"/> class.
        /// </summary>
        public SupportViewModel()
        {
            this.DisplayName = "Send a support request.";
            this.AttachLog = true;

            this.validators = this.GetType()
                .GetProperties()
                .Where(p => this.GetValidations(p).Length != 0)
                .ToDictionary(p => p.Name, this.GetValidations);

            this.propertyGetters = this.GetType()
                .GetProperties()
                .Where(p => this.GetValidations(p).Length != 0)
                .ToDictionary(p => p.Name, this.GetValueGetter);

        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [EmailAddress(ErrorMessage = "Enter a valid e-mail address.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a valid e-mail address.")]
        public string Email
        {
            get
            {
                return this.email;
            }

            set
            {
                if (value == this.email)
                {
                    return;
                }

                this.email = value;
                this.NotifyOfPropertyChange(() => this.Email);
            }
        }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        [StringLength(int.MaxValue, MinimumLength = 10, ErrorMessage = "Enter at least 10 characters.")]
        public string Subject
        {
            get
            {
                return this.subject;
            }

            set
            {
                if (value == this.subject)
                {
                    return;
                }

                this.subject = value;
                this.NotifyOfPropertyChange(() => this.Subject);
            }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [StringLength(int.MaxValue, MinimumLength = 50, ErrorMessage = "Enter at least 50 characters.")]
        public string Message
        {
            get
            {
                return this.message;
            }

            set
            {
                if (value == this.message)
                {
                    return;
                }

                this.message = value;
                this.NotifyOfPropertyChange(() => this.Message);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether attach log.
        /// </summary>
        public bool AttachLog
        {
            get
            {
                return this.attachLog;
            }

            set
            {
                if (value.Equals(this.attachLog))
                {
                    return;
                }

                this.attachLog = value;
                this.NotifyOfPropertyChange(() => this.AttachLog);
            }
        }

        /// <summary>
        /// The cancel.
        /// </summary>
        public void Cancel()
        {
            this.TryClose();
        }

        /// <summary>
        /// The send.
        /// </summary>
        [Dependencies("Email", "Subject", "Message")]
        public void Send()
        {
            Task.Run(
                () =>
                {
                    try
                    {
                        var apikey = "hVAAtCM7wCEaJe9DqlR52w";
                        var apiSecret = "haN4L38nqnyZTTfVyudb7WpR2vSAcOWB3PUEm6XQQ";
                        var subdomain = "hearthstonetracker";

                        var client = new Client(subdomain, apikey, apiSecret);

                        object attachments = new object[] { };
                        if (this.AttachLog)
                        {
                            var logPath = Path.Combine((string)AppDomain.CurrentDomain.GetData("DataDirectory"), "logs");
                            var dirInfo = new DirectoryInfo(logPath);
                            var latestLogfiles = dirInfo.GetFiles().Where(x => x.Extension == ".txt").OrderByDescending(x => x.LastWriteTime).Take(3).ToList();

                            if (latestLogfiles.Count > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    using (var zipfile = new ZipArchive(ms, ZipArchiveMode.Create, true))
                                    {
                                        foreach (var latestLogfile in latestLogfiles)
                                        {
                                            zipfile.CreateEntryFromFile(latestLogfile.FullName, latestLogfile.Name, CompressionLevel.Optimal);
                                        }
                                    }

                                    ms.Position = 0;
                                    var bytes = new byte[ms.Length];
                                    ms.Read(bytes, 0, bytes.Length);

                                    attachments = new object[]
                                                     {
                                                         new
                                                             {
                                                                 name = "logfiles.zip", 
                                                                 content_type = "application/zip", 
                                                                 data = Convert.ToBase64String(bytes)
                                                             }
                                                     };
                                }
                            }
                        }

                        var ticket = new
                        {
                            email = this.Email, 
                            ticket = new
                            {
                                state = "open", 
                                subject = this.Subject, 
                                message = this.Message, 
                                user_agent = "HearthstoneTracker App", 
                                attachments = attachments
                            }
                        };
                        client.Post("/api/v1/tickets.json", ticket);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                });

            this.TryClose();
            MessageBox.Show("Your support request has been sent.", "Support request sent.", MessageBoxButton.OK);
        }

        /// <summary>
        /// The can send.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanSend()
        {
            return string.IsNullOrEmpty(this.Error);
        }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <returns>
        /// The error message for the property. The default is an empty string ("").
        /// </returns>
        /// <param name="columnName">
        /// The name of the property whose error message to get. 
        /// </param>
        public string this[string columnName]
        {
            get
            {
                if (this.propertyGetters.ContainsKey(columnName))
                {
                    var propertyValue = this.propertyGetters[columnName](this);
                    var errorMessages = this.validators[columnName]
                        .Where(v => !v.IsValid(propertyValue))
                        .Select(v => v.ErrorMessage).ToArray();

                    return string.Join(Environment.NewLine, errorMessages);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <returns>
        /// An error message indicating what is wrong with this object. The default is an empty string ("").
        /// </returns>
        public string Error
        {
            get
            {
                var errors = from validator in this.validators
                             from attribute in validator.Value
                             where !attribute.IsValid(this.propertyGetters[validator.Key](this))
                             select attribute.ErrorMessage;

                return string.Join(Environment.NewLine, errors.ToArray());
            }
        }

        /// <summary>
        /// The get validations.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// The <see cref="ValidationAttribute[]"/>.
        /// </returns>
        private ValidationAttribute[] GetValidations(PropertyInfo property)
        {
            return (ValidationAttribute[])property.GetCustomAttributes(typeof(ValidationAttribute), true);
        }

        /// <summary>
        /// The get value getter.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// The <see cref="Func"/>.
        /// </returns>
        private Func<Screen, object> GetValueGetter(PropertyInfo property)
        {
            return viewmodel => property.GetValue(viewmodel, null);
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            Tracker.TrackEventAsync(Tracker.CommonCategory, "Support", "Support");
        }
    }
}