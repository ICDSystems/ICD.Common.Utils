using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Common.Utils.Types
{
    /// <summary>
    /// Class to raise events when flags are set and unset on the data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class GenericNotifyFlagsChanged<T> : AbstractNotifyFlagsChanged<T> where T : struct, IConvertible
    {
        /// <summary>
        /// Converts the enum to the backing int value
        /// Override for performance improvements
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override int GetIntValue(T value)
        {
            return (int)(object)value;
        }

        /// <summary>
        /// Converts the backing int value to enum
        /// Override for performance improvements 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override T GetEnumValue(int value)
        {
            return (T)(object)value;
        }
    }
}