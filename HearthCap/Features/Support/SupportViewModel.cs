using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Caliburn.Micro.Recipes.Filters;
using HearthCap.Features.Analytics;
using UserVoice;
using LogManager = NLog.LogManager;

namespace HearthCap.Features.Support
{
    [Export(typeof(SupportViewModel))]
    public class SupportViewModel : Screen, IDataErrorInfo
    {
        private readonly Dictionary<string, Func<Screen, object>> propertyGetters;
        private readonly Dictionary<string, ValidationAttribute[]> validators;

        private static readonly NLog.Logger Log = LogManager.GetCurrentClassLogger();

        private string email;

        private string message;

        private bool attachLog;

        private string subject;

        public SupportViewModel()
        {
            DisplayName = "Send a support request.";
            AttachLog = true;

            validators = GetType()
                .GetProperties()
                .Where(p => GetValidations(p).Length != 0)
                .ToDictionary(p => p.Name, GetValidations);

            propertyGetters = GetType()
                .GetProperties()
                .Where(p => GetValidations(p).Length != 0)
                .ToDictionary(p => p.Name, GetValueGetter);
        }

        [EmailAddress(ErrorMessage = "Enter a valid e-mail address.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter a valid e-mail address.")]
        public string Email
        {
            get { return email; }
            set
            {
                if (value == email)
                {
                    return;
                }
                email = value;
                NotifyOfPropertyChange(() => Email);
            }
        }

        [StringLength(int.MaxValue, MinimumLength = 10, ErrorMessage = "Enter at least 10 characters.")]
        public string Subject
        {
            get { return subject; }
            set
            {
                if (value == subject)
                {
                    return;
                }
                subject = value;
                NotifyOfPropertyChange(() => Subject);
            }
        }

        [StringLength(int.MaxValue, MinimumLength = 50, ErrorMessage = "Enter at least 50 characters.")]
        public string Message
        {
            get { return message; }
            set
            {
                if (value == message)
                {
                    return;
                }
                message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        public bool AttachLog
        {
            get { return attachLog; }
            set
            {
                if (value.Equals(attachLog))
                {
                    return;
                }
                attachLog = value;
                NotifyOfPropertyChange(() => AttachLog);
            }
        }

        public void Cancel()
        {
            TryClose();
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
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
                            if (AttachLog)
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
                                        var bytes = new Byte[ms.Length];
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
                                    email = Email,
                                    ticket = new
                                        {
                                            state = "open",
                                            subject = Subject,
                                            message = Message,
                                            user_agent = "HearthstoneTracker App", attachments
                                        }
                                };
                            client.Post("/api/v1/tickets.json", ticket);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                        }
                    });

            TryClose();
            MessageBox.Show("Your support request has been sent.", "Support request sent.", MessageBoxButton.OK);
        }

        public bool CanSend()
        {
            return String.IsNullOrEmpty(Error);
        }

        /// <summary>
        ///     Gets the error message for the property with the given name.
        /// </summary>
        /// <returns>
        ///     The error message for the property. The default is an empty string ("").
        /// </returns>
        /// <param name="columnName">The name of the property whose error message to get. </param>
        public string this[string columnName]
        {
            get
            {
                if (propertyGetters.ContainsKey(columnName))
                {
                    var propertyValue = propertyGetters[columnName](this);
                    var errorMessages = validators[columnName]
                        .Where(v => !v.IsValid(propertyValue))
                        .Select(v => v.ErrorMessage).ToArray();

                    return string.Join(Environment.NewLine, errorMessages);
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///     Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <returns>
        ///     An error message indicating what is wrong with this object. The default is an empty string ("").
        /// </returns>
        public string Error
        {
            get
            {
                var errors = from validator in validators
                    from attribute in validator.Value
                    where !attribute.IsValid(propertyGetters[validator.Key](this))
                    select attribute.ErrorMessage;

                return string.Join(Environment.NewLine, errors.ToArray());
            }
        }

        private ValidationAttribute[] GetValidations(PropertyInfo property)
        {
            return (ValidationAttribute[])property.GetCustomAttributes(typeof(ValidationAttribute), true);
        }

        private Func<Screen, object> GetValueGetter(PropertyInfo property)
        {
            return viewmodel => property.GetValue(viewmodel, null);
        }

        /// <summary>
        ///     Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            Tracker.TrackEventAsync(Tracker.CommonCategory, "Support", "Support");
        }
    }
}
