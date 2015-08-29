/******************************************************************************* 
 *  _                      _     _ _ _         
 * | |   _   _  __ _ _   _(_) __| (_) |_ _   _ 
 * | |  | | | |/ _` | | | | |/ _` | | __| | | |
 * | |__| |_| | (_| | |_| | | (_| | | |_| |_| |
 * |_____\__, |\__, |\__,_|_|\__,_|_|\__|\__, |
 *       |___/    |_|                    |___/ 
 * 
 *  ValidatingScreen for Caliburn Micro
 *  Version: 1.0.0.0
 *  Generated: Monday Feb 15 12:00:00 GMT 2011 
 * 
 * ***************************************************************************** 
 *  Copyright Lyquidity Solutions Limited 2011
 *  http://www.lyquidity.com
 * ***************************************************************************** 
 * History: 
 * Feb 18th 2011:
 * Added protected virtual methods for the IDataErrorInfo methods to call so the
 * descendent class has an opportunity to aggregate, change or otherwise 
 * influence the final validation error reporting.  For example, when on field
 * changes it might be neccessary to restrict or change the validation of 
 * another field.
 * 
 * Feb 19th:
 * Added support for validation groups so properties can be validated within 
 * named groups.  New HasErrorsByGroup methods allow a caller to define the
 * group name to use.
 * 
 * Feb 22nd:
 * Added support for validating only when a guard property is enabled.  By
 * default the guard property is Can[PropertyName].  But the guard property 
 * a validation controller will use can be set explicitly.
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Caliburn.Micro;

namespace HearthCap.Framework.Validation
{
    /// <summary>
    ///     Subclasses the Screen class to provide field validation which
    ///     is able to grab the validation attributes (if any) declared
    ///     on descendent classes.
    /// </summary>
    /// <typeparam name="TViewModel">
    ///     The view model to which this instance is being applied.
    ///     This is necessary to static methods know which type to use.
    /// </typeparam>
    /// <remarks>
    ///     This class is taken from the post
    ///     http://caliburnmicro.codeplex.com/discussions/243212
    ///     which in turn is taken from the idea here
    ///     http://weblogs.asp.net/marianor/archive/2009/04/17/wpf-validation-with-attributes-and-idataerrorinfo-interface-in-mvvm.aspx
    ///     and has been adapted to fit CM a little better.
    ///     The implementation uses Linq but could also have been implemented using standard code.
    /// </remarks>
    public abstract class ValidatingScreen<TViewModel> : Screen, IDataErrorInfo
        where TViewModel : ValidatingScreen<TViewModel>
    {
        /// <summary>
        ///     Grab all the property getter MethodInfo instances which have validations declared and hold them in a static class
        /// </summary>
        private static readonly Dictionary<string, Func<TViewModel, object>> propertyGetters =
            (from p in typeof(TViewModel).GetProperties()
                where p.GetAttributes<ValidationAttribute>(true).ToArray().Length != 0
                select p
                ).ToDictionary(p => p.Name, p => GetValueGetter(p));

        /// <summary>
        ///     Create a dictionary of the validators (if any) associated with each class property
        /// </summary>
        private static readonly Dictionary<string, ValidationAttribute[]> validators =
            (from p in typeof(TViewModel).GetProperties()
                let attrs = p.GetAttributes<ValidationAttribute>(true).ToArray()
                where attrs.Length != 0
                select new KeyValuePair<string, ValidationAttribute[]>(p.Name, attrs)
                ).ToDictionary(p => p.Key, p => p.Value);

        /// <summary>
        ///     Create a dictionary of the validation groups creating a default validation group for properties without one
        /// </summary>
        private static readonly Dictionary<string, ValidationGroupAttribute> validationGroup =
            (from p in typeof(TViewModel).GetProperties()
                let groups = p.GetAttributes<ValidationGroupAttribute>(true).ToArray()
                where p.GetAttributes<ValidationAttribute>(true).ToArray().Length != 0
                select new KeyValuePair<string, ValidationGroupAttribute>
                    (p.Name, groups.Length == 0 ? new ValidationGroupAttribute() : groups[0])
                ).ToDictionary(p => p.Key, p => p.Value);

        /// <summary>
        ///     Returns True if any of the property values generate a validation error
        /// </summary>
        public bool HasErrors
        {
            get
            {
                var result = !string.IsNullOrEmpty(Error);
                return result;
            }
        }

        /// <summary>
        ///     Returns True if any of the property values in the Default validation group generate a validation error
        /// </summary>
        public bool HasErrorsByGroup()
        {
            return HasErrorsByGroup(ValidationGroupAttribute.DefaultGroupName);
        }

        /// <summary>
        ///     Returns True if any of the property values in the named group generate a validation error
        /// </summary>
        public bool HasErrorsByGroup(string groupName)
        {
            var result = !string.IsNullOrEmpty(ErrorByGroup(groupName));
            return result;
        }

        /// <summary>
        ///     This protected method is called when WPF calls the Error method and give the class a chance to extend the list of
        ///     reported errors
        /// </summary>
        /// <param name="errors">The list of errrors generated by validators</param>
        protected virtual void OnError(List<string> errors)
        {
            // Does nothing
        }

        /// <summary>
        ///     This protected method is called when WPF calls the Error method and give the class a chance to extend the list of
        ///     reported errors
        /// </summary>
        /// <param name="columnName">The name of the column being tested</param>
        /// <param name="errors">The list of errrors generated by validators</param>
        protected virtual void OnColumnrror(string columnName, Dictionary<Type, string> errors)
        {
            // Does nothing
        }

        /// <summary>
        ///     Executes the validators for a property
        /// </summary>
        /// <typeparam name="TProperty">An expression that return the name of a property</typeparam>
        /// <param name="property">The property to be tested</param>
        /// <returns>True if all validators are valid</returns>
        public virtual bool IsValid<TProperty>(Expression<Func<TProperty>> property)
        {
            var name = property.GetMemberInfo().Name;
            var value = propertyGetters[name]((TViewModel)this);
            return validators[name].All(v => v.IsValid(value));
        }

        /// <summary>
        ///     Test all validators for all properties and returns a string containing messages if any need to be reported.
        /// </summary>
        public string Error
        {
            get
            {
                // Run the validation but only for groups which should be included
                var errors = from i in validators
                    from v in i.Value
                    let vgroup = validationGroup[i.Key]
                    where (vgroup != null && vgroup.IncludeInErrorsValidation) &&
                          !v.IsValid(propertyGetters[i.Key]((TViewModel)this))
                    select (v.FormatErrorMessage(""));

                // Realize a list and send to the OnError() method
                var errorList = errors.ToList();
                OnError(errorList);

                return string.Join(Environment.NewLine, errorList.ToArray());
            }
        }

        /// <summary>
        ///     Test all validators for all properties but for a named group and returns a string containing messages if any need
        ///     to be reported.
        /// </summary>
        public string ErrorByGroup(string groupName)
        {
            // Run the validation but only for groups which should be included
            var errors = from i in validators
                from v in i.Value
                let vgroup = validationGroup[i.Key]
                where (vgroup != null && string.Compare(vgroup.GroupName, groupName, true) == 0) &&
                      !v.IsValid(propertyGetters[i.Key]((TViewModel)this))
                select (v.FormatErrorMessage(""));

            // Realize a list and send to the OnError() method
            var errorList = errors.ToList();
            OnError(errorList);

            return string.Join(Environment.NewLine, errorList.ToArray());
        }

        /// <summary>
        ///     Tests for validation errors for using the validators of a named property
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns>An error string or an empty string if there's nothing to do.</returns>
        public string this[string columnName]
        {
            get
            {
                try
                {
                    if (propertyGetters.ContainsKey(columnName))
                    {
                        var value = propertyGetters[columnName]((TViewModel)this);

                        // This assumes a specific type of validator will be declared only once
                        // Groups are not used in this instance
                        var errors =
                            (from v in validators[columnName]
                                where IsEnabled(this, v, columnName) && !v.IsValid(value)
                                select v
                                ).ToDictionary(v => v.GetType(), v => v.FormatErrorMessage(""));

                        // Realize a list and send to the OnColumnrror() method
                        OnColumnrror(columnName, errors);

                        return string.Join(Environment.NewLine, errors.Values.ToArray());
                    }

                    return string.Empty;
                }
                finally
                {
                    // NotifyOfPropertyChange(() => this.Error);
                }
            }
        }

        private static bool IsEnabled(IScreen instance, ValidationAttribute validator, string propertyName)
        {
            // Get the IValidationControl interface
            var ivc = validator as IValidationControl;

            // If the option is set to validate while disabled it doesn't matter what other conditions exist.
            if (ivc != null
                && ivc.ValidateWhileDisabled)
            {
                return true;
            }

            // Find a property with the name Can<propertyName>
            var pi = instance.GetType().GetProperty(
                ivc == null || string.IsNullOrEmpty(ivc.GuardProperty)
                    ? "Can" + propertyName
                    : ivc.GuardProperty
                );

            if (pi == null)
            {
                return true; // There is no such guard property
            }

            // Get the result
            var result = pi.GetValue(instance, null);
            if (result == null
                || !(result is bool))
            {
                return true; // No result or the guard property does not return a bool
            }

            return (bool)result;
        }

        /// <summary>
        ///     Uses Linq to create a list of MethodInfo.GetGetProperty methods.
        /// </summary>
        /// <param name="property">The property for which to get the getter</param>
        /// <returns>The Getter</returns>
        private static Func<TViewModel, object> GetValueGetter(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof(TViewModel), "i");
            var cast = Expression.TypeAs(Expression.Property(instance, property), typeof(object));
            return (Func<TViewModel, object>)Expression.Lambda(cast, instance).Compile();
        }
    }
}
