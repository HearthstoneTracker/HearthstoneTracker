namespace Capture
{
    using System;

    /// <summary>
    /// Indicates that the provided process does not have a window handle.
    /// </summary>
    public class ProcessHasNoWindowHandleException : Exception
    {
        #region Constructors and Destructors

        public ProcessHasNoWindowHandleException()
            : base("The process does not have a window handle.")
        {
        }

        #endregion
    }

    public class ProcessAlreadyHookedException : Exception
    {
        #region Constructors and Destructors

        public ProcessAlreadyHookedException()
            : base("The process is already hooked.")
        {
        }

        #endregion
    }

    public class InjectionFailedException : Exception
    {
        #region Constructors and Destructors

        public InjectionFailedException(Exception innerException)
            : base("Injection to the target process failed. See InnerException for more detail.", innerException)
        {
        }

        #endregion
    }
}