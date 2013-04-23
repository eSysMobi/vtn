using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vlastelin.Common
{
    /// <summary>
    /// Database error handler interface
    /// </summary>
    public interface IDatabaseErrorHandler
    {
        /// <summary>
        /// Raise exception
        /// </summary>
        /// <param name="errorCode">error code</param>
        /// <param name="extendedMessage"> extension message</param>
        void RaiseException(int errorCode, string extendedMessage);
    }

}
