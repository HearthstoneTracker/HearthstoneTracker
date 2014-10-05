// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Exceptions.cs" company="">
//   
// </copyright>
// <summary>
//   Indicates that the provided process does not have a window handle.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Capture
{
    using System;

    /// <summary>
    /// Indicates that the provided process does not have a window handle.
    /// </summary>
    public class ProcessHasNoWindowHandleException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessHasNoWindowHandleException"/> class.
        /// </summary>
        public ProcessHasNoWindowHandleException()
            : base("The process does not have a window handle.")
        {
        }

        #endregion
    }

    /// <summary>
    /// The process already hooked exception.
    /// </summary>
    public class ProcessAlreadyHookedException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessAlreadyHookedException"/> class.
        /// </summary>
        public ProcessAlreadyHookedException()
            : base("The process is already hooked.")
        {
        }

        #endregion
    }

    /// <summary>
    /// The injection failed exception.
    /// </summary>
    public class InjectionFailedException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectionFailedException"/> class.
        /// </summary>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public InjectionFailedException(Exception innerException)
            : base("Injection to the target process failed. See InnerException for more detail.", innerException)
        {
        }

        #endregion
    }
}