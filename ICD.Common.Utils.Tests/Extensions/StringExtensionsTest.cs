﻿using ICD.Common.Utils.Extensions;
using NUnit.Framework;
using System.Linq;

namespace ICD.Common.Utils.Tests_NetStandard.Extensions
{
    [TestFixture]
    public sealed class StringExtensionsTest
    {
        [Test]
        public void IndexOfTest()
        {
            Assert.Inconclusive();
        }

        [TestCase(true, "12345", '1')]
        [TestCase(false, "12345", '2')]
        public void StartsWithTest(bool expected, string value, char character)
        {
            Assert.AreEqual(expected, value.StartsWith(character));
        }

        [TestCase(true, "12345", '5')]
        [TestCase(false, "12345", '2')]
        public void EndsWithTest(bool expected, string value, char character)
        {
            Assert.AreEqual(expected, value.EndsWith(character));
        }

        [Test]
        public void SplitCharCountTest()
        {
            string[] split = "123-456-789-0".Split('-', 2).ToArray();

            Assert.AreEqual(2, split.Length);
            Assert.AreEqual("123", split[0]);
            Assert.AreEqual("456-789-0", split[1]);
        }

        [Test]
        public void SplitChunksizeTest()
        {
            string[] split = "1234567890".Split(3).ToArray();

            Assert.AreEqual(4, split.Length);
            Assert.AreEqual("123", split[0]);
            Assert.AreEqual("456", split[1]);
            Assert.AreEqual("789", split[2]);
            Assert.AreEqual("0", split[3]);
        }

        [Test]
        public void SplitStringDelimiterTest()
        {
            string[] split = "1234567890".Split("23").ToArray();

            Assert.AreEqual(2, split.Length);
            Assert.AreEqual("14567890", string.Join("", split));
        }

        [Test]
        public void SplitStringDelimitersTest()
        {
            string[] split = "1234567890".Split(new string[] { "23", "67" }).ToArray();

            Assert.AreEqual(3, split.Length);
            Assert.AreEqual("145890", string.Join("", split));
        }

        [TestCase("12345", "   12 3 4  \t 5\n")]
        public void RemoveWhitespaceTest(string expected, string value)
        {
            Assert.AreEqual(expected, value.RemoveWhitespace());
        }

        [Test]
        public void RemoveCharactersTest()
        {
            Assert.AreEqual("13457890", "1234567890".Remove(new char[] { '2', '6' }));
        }

        [TestCase(true, "27652")]
        [TestCase(false, "a27652")]
        public void IsNumericTest(bool expected, string value)
        {
            Assert.AreEqual(expected, value.IsNumeric());
        }
    }
}