using System;
using ICD.Common.Properties;

namespace ICD.Common.Utils
{
    public static class BigEndianBitConverter
    {
        private const byte FULL_BYTE = 0xFF;

        public static ushort ToUshort([NotNull] byte[] value, int startIndex)
        {
            return unchecked((ushort)ToShort(value, startIndex));
        }
        
        public static short ToShort([NotNull] byte[] value, int startIndex)
        {
            if (!BitConverter.IsLittleEndian)
                return BitConverter.ToInt16(value, startIndex);
            
            const int bytes = sizeof(short);
            
            if (value == null)
                throw new ArgumentNullException("value");
            if (startIndex < 0 || startIndex >= value.Length)
                throw new ArgumentOutOfRangeException("startIndex");
            if (startIndex > value.Length - bytes)
                throw new ArgumentException("Array plus start index too small");
            
            short result = 0;
            for (int i = 0; i < bytes; i++)
                result |= (short)(value[i + startIndex] << GetBitShift(i, bytes));
            
            return result;
        }
        
        public static uint ToUint([NotNull] byte[] value, int startIndex)
        {
            return unchecked((uint)ToInt(value, startIndex));
        }
        
        public static int ToInt([NotNull] byte[] value, int startIndex)
        {
            if (!BitConverter.IsLittleEndian)
                return BitConverter.ToInt32(value, startIndex);
            
            const int bytes = sizeof(int);
            
            if (value == null)
                throw new ArgumentNullException("value");
            if (startIndex < 0 || startIndex >= value.Length)
                throw new ArgumentOutOfRangeException("startIndex");
            if (startIndex > value.Length - bytes)
                throw new ArgumentException("Array plus start index too small");
            
            int result = 0;
            for (int i = 0; i < bytes; i++)
                result |= value[i + startIndex] << GetBitShift(i, bytes);
            
            return result;
        }
        
        public static ulong ToUlong([NotNull] byte[] value, int startIndex)
        {
            return unchecked((ulong)ToLong(value, startIndex));
        }

        public static long ToLong([NotNull] byte[] value, int startIndex)
        {
            if (!BitConverter.IsLittleEndian)
                return BitConverter.ToInt64(value, startIndex);
            
            const int bytes = sizeof(long);
            
            if (value == null)
                throw new ArgumentNullException("value");
            if (startIndex < 0 || startIndex >= value.Length)
                throw new ArgumentOutOfRangeException("startIndex");
            if (startIndex > value.Length - bytes)
                throw new ArgumentException("Array plus start index too small");
            
            int result = 0;
            for (int i = 0; i < bytes; i++)
                result |= value[i + startIndex] << GetBitShift(i, bytes);

            return result;
        }

        public static byte[] GetBytes(int value)
        {
            if (!BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value);

            const int total_bytes = sizeof(int);
            
            byte[] response = new byte[total_bytes];

            for (int i = 0; i < total_bytes; i++)
                response[i] = (byte)(value >> GetBitShift(i,total_bytes) & FULL_BYTE);

            return response;
        }
        public static byte[] GetBytes(uint value)
        {
            if (!BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value);

            const int total_bytes = sizeof(uint);
            
            byte[] response = new byte[total_bytes];

            for (int i = 0; i < total_bytes; i++)
                response[i] = (byte)(value >> GetBitShift(i,total_bytes) & FULL_BYTE);

            return response;
        }
        
        public static byte[] GetBytes(short value)
        {
            if (!BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value);

            const int total_bytes = sizeof(short);
            
            byte[] response = new byte[total_bytes];

            for (int i = 0; i < total_bytes; i++)
                response[i] = (byte)(value >> GetBitShift(i,total_bytes) & FULL_BYTE);

            return response;
        }

        public static byte[] GetBytes(ushort value)
        {
            if (!BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value);

            const int total_bytes = sizeof(short);
            
            byte[] response = new byte[total_bytes];

            for (int i = 0; i < total_bytes; i++)
                response[i] = (byte)(value >> GetBitShift(i,total_bytes) & FULL_BYTE);

            return response;
        }

        public static byte[] GetBytes(long value)
        {
            if (!BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value);

            const int total_bytes = sizeof(long);
            
            byte[] response = new byte[total_bytes];

            for (int i = 0; i < total_bytes; i++)
                response[i] = (byte)(value >> GetBitShift(i,total_bytes) & FULL_BYTE);

            return response;
        }

        public static byte[] GetBytes(ulong value)
        {
            if (!BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value);

            const int total_bytes = sizeof(ulong);
            
            byte[] response = new byte[total_bytes];

            for (int i = 0; i < total_bytes; i++)
                response[i] = (byte)(value >> GetBitShift(i,total_bytes) & FULL_BYTE);

            return response;
        }
        private static int GetBitShift(int byteNumber, int totalBytes)
        {
            return ((totalBytes - 1 - byteNumber) * 8);
        }
        
        
    }
}