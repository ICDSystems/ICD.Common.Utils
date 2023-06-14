using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Common.Utils.Types
{
    public abstract class AbstractNotifyFlagsChanged<T> where T : struct, IConvertible
    {
        private T m_Data;

        /// <summary>
        /// Raised when flags are set on the data
        /// </summary>
        public event EventHandler<GenericEventArgs<T>> OnFlagsSet;

        /// <summary>
        /// Raised when flags are unset on the data
        /// </summary>
        public event EventHandler<GenericEventArgs<T>> OnFlagsUnset;

        /// <summary>
        /// Raised when the data changes
        /// </summary>
        public event EventHandler<GenericEventArgs<T>> OnChange;

        /// <summary>
        /// Data
        /// </summary>
        public T Data
        {
            get { return m_Data; }
            set
            {
                if (DataIsEqual(m_Data, value))
                    return;

                int intData = GetIntValue(m_Data);
                int intValue = GetIntValue(value);

                int setFlags = intValue & ~intData;
                int unsetFlags = intData & ~intValue;

                m_Data = value;
                
                OnChange.Raise(this, value);
                
                if (setFlags != 0)
                    OnFlagsSet.Raise(this, GetEnumValue(setFlags));
                if (unsetFlags != 0)
                    OnFlagsUnset.Raise(this, GetEnumValue(unsetFlags));
            }
        }

        /// <summary>
        /// Converts the enum to the backing int value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract int GetIntValue(T value);

        /// <summary>
        /// Converts the backing int value to enum
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract T GetEnumValue(int value);

        /// <summary>
        /// Checks enums for equality
        /// Override for performance improvements
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual bool DataIsEqual(T a, T b)
        {
            return GetIntValue(a) == GetIntValue(b);
        }
    }
}