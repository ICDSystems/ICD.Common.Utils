using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Types;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Types
{
    [TestFixture]
    public class GenericNotifyFlagsChangedTest
    {
        [Flags]
        private enum eTestFlagsEnum
        {
            None = 0,
            A = 1,
            B = 2,
            C = 4,
            D = 32,
            BandC = B | C
        }

        [Test]
        public void TestSingleFlags()
        {
            List<GenericEventArgs<eTestFlagsEnum>> addedFlags = new List<GenericEventArgs<eTestFlagsEnum>>();
            List<GenericEventArgs<eTestFlagsEnum>> removedFlags = new List<GenericEventArgs<eTestFlagsEnum>>();

            GenericNotifyFlagsChanged<eTestFlagsEnum> genericNotify = new GenericNotifyFlagsChanged<eTestFlagsEnum>();

            genericNotify.OnFlagsSet += (sender, args) => addedFlags.Add(args);
            genericNotify.OnFlagsUnset += (sender, args) => removedFlags.Add(args);

            // Initial State
            Assert.AreEqual(0, addedFlags.Count);
            Assert.AreEqual(0, removedFlags.Count);
            
            // Add No flags
            genericNotify.Data = eTestFlagsEnum.None;
            Assert.AreEqual(0, addedFlags.Count);
            Assert.AreEqual(0, removedFlags.Count);

            // Add Flag
            genericNotify.Data = eTestFlagsEnum.B;
            Assert.AreEqual(1, addedFlags.Count);
            Assert.AreEqual(0, removedFlags.Count);
            Assert.AreEqual(eTestFlagsEnum.B, addedFlags[0].Data);
            Assert.AreEqual(eTestFlagsEnum.B, genericNotify.Data);

            // Add Another Flag
            genericNotify.Data |= eTestFlagsEnum.C;
            Assert.AreEqual(2, addedFlags.Count);
            Assert.AreEqual(0, removedFlags.Count);
            Assert.AreEqual(eTestFlagsEnum.C, addedFlags[1].Data);
            Assert.AreEqual(eTestFlagsEnum.B | eTestFlagsEnum.C, genericNotify.Data);
            
            // Remove a Flag
            genericNotify.Data = genericNotify.Data & ~ eTestFlagsEnum.B;
            Assert.AreEqual(2, addedFlags.Count);
            Assert.AreEqual(1, removedFlags.Count);
            Assert.AreEqual(eTestFlagsEnum.B, removedFlags[0].Data);
            Assert.AreEqual(eTestFlagsEnum.C, genericNotify.Data);
            
            // Add Already Existing Flags
            genericNotify.Data |= eTestFlagsEnum.C;
            Assert.AreEqual(2, addedFlags.Count);
            Assert.AreEqual(1, removedFlags.Count);
            
            // Clear Flags
            genericNotify.Data = eTestFlagsEnum.None;
            Assert.AreEqual(2, addedFlags.Count);
            Assert.AreEqual(2, removedFlags.Count);
            Assert.AreEqual(eTestFlagsEnum.C, removedFlags[1].Data);
            Assert.AreEqual(eTestFlagsEnum.None, genericNotify.Data);
        }
        
        [Test]
        public void TestMultipleFlags()
        {
            List<GenericEventArgs<eTestFlagsEnum>> addedFlags = new List<GenericEventArgs<eTestFlagsEnum>>();
            List<GenericEventArgs<eTestFlagsEnum>> removedFlags = new List<GenericEventArgs<eTestFlagsEnum>>();

            GenericNotifyFlagsChanged<eTestFlagsEnum> genericNotify = new GenericNotifyFlagsChanged<eTestFlagsEnum>();

            genericNotify.OnFlagsSet += (sender, args) => addedFlags.Add(args);
            genericNotify.OnFlagsUnset += (sender, args) => removedFlags.Add(args);

            // Initial State
            Assert.AreEqual(0, addedFlags.Count);
            Assert.AreEqual(0, removedFlags.Count);
            
            // Add No flags
            genericNotify.Data = eTestFlagsEnum.None;
            Assert.AreEqual(0, addedFlags.Count);
            Assert.AreEqual(0, removedFlags.Count);

            // Add Flag
            genericNotify.Data = eTestFlagsEnum.B | eTestFlagsEnum.D;
            Assert.AreEqual(1, addedFlags.Count);
            Assert.AreEqual(0, removedFlags.Count);
            Assert.AreEqual(eTestFlagsEnum.B | eTestFlagsEnum.D, addedFlags[0].Data);
            Assert.AreEqual(eTestFlagsEnum.B | eTestFlagsEnum.D, genericNotify.Data);

            // Add Another Flag
            genericNotify.Data |= eTestFlagsEnum.C;
            Assert.AreEqual(2, addedFlags.Count);
            Assert.AreEqual(0, removedFlags.Count);
            Assert.AreEqual(eTestFlagsEnum.C, addedFlags[1].Data);
            Assert.AreEqual(eTestFlagsEnum.B | eTestFlagsEnum.C | eTestFlagsEnum.D, genericNotify.Data);
            
            // Remove a Flag
            genericNotify.Data = eTestFlagsEnum.D;
            Assert.AreEqual(2, addedFlags.Count);
            Assert.AreEqual(1, removedFlags.Count);
            Assert.AreEqual(eTestFlagsEnum.B | eTestFlagsEnum.C, removedFlags[0].Data);
            Assert.AreEqual(eTestFlagsEnum.D, genericNotify.Data);
            
            // Add Already Existing Flags
            genericNotify.Data |= eTestFlagsEnum.D;
            Assert.AreEqual(2, addedFlags.Count);
            Assert.AreEqual(1, removedFlags.Count);
            
            // Clear Flags
            genericNotify.Data = eTestFlagsEnum.None;
            Assert.AreEqual(2, addedFlags.Count);
            Assert.AreEqual(2, removedFlags.Count);
            Assert.AreEqual(eTestFlagsEnum.D, removedFlags[1].Data);
            Assert.AreEqual(eTestFlagsEnum.None, genericNotify.Data);
        }
    }
}