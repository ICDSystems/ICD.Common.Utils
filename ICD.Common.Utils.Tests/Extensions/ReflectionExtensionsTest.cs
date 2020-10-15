using System;
using ICD.Common.Utils.Extensions;
using NUnit.Framework;

namespace ICD.Common.Utils.Tests.Extensions
{
	[TestFixture]
	public sealed class ReflectionExtensionsTest
	{
		[Test]
		public void GetCustomAttributesTest()
		{
			Assert.Inconclusive();
		}

		#region Get/Set Property Tests


		private sealed class PropertyTestClass
		{
			private readonly InternalPropertyTestClass m_InternalClass;
			public string PropertyString { get; set; }
			public int PropertyInt { get; set; }
			public DateTime PropertyDateTime { get; set; }
			public InternalPropertyTestClass InternalClass { get { return m_InternalClass; } }

			public PropertyTestClass()
			{
				m_InternalClass = new InternalPropertyTestClass();
			}

			internal sealed class InternalPropertyTestClass
			{
				private readonly DeepPropertyTestClass m_DeepClass;

				public string InternalPropertyString { get; set; }
				public int InternalPropertyInt { get; set; }
				public DateTime InternalPropertyDateTime { get; set; }

				public DeepPropertyTestClass DeepClass { get { return m_DeepClass; } }

				public InternalPropertyTestClass()
				{
					m_DeepClass = new DeepPropertyTestClass();
				}

				internal sealed class DeepPropertyTestClass
				{
					public string DeepPropertyString { get; set; }
					public int DeepPropertyInt { get; set; }
					public DateTime DeepPropertyDateTime { get; set; }
				}
			}
		}


		[Test]
		public void GetPropertyTest()
		{
			var testClass = new PropertyTestClass();

			int testInt = 15;
			string testString = "TestString";
			var testDateTime = DateTime.Parse("1987-08-27");

			testClass.PropertyInt = testInt;
			testClass.PropertyString = testString;
			testClass.PropertyDateTime = testDateTime;

			int internalTestInt = 34;
			string internalTestString = "InternalTestString";
			var internalTestDateTime = DateTime.Parse("2011-12-05");

			testClass.InternalClass.InternalPropertyInt = internalTestInt;
			testClass.InternalClass.InternalPropertyString = internalTestString;
			testClass.InternalClass.InternalPropertyDateTime = internalTestDateTime;

			int deepTestInt = int.MaxValue;
			string deepTestString = "HelloThere";
			var deepTestDateTime = DateTime.Parse("1776-07-04");

			testClass.InternalClass.DeepClass.DeepPropertyInt = deepTestInt;
			testClass.InternalClass.DeepClass.DeepPropertyString = deepTestString;
			testClass.InternalClass.DeepClass.DeepPropertyDateTime = deepTestDateTime;

			object propertyInt;
			object propertyString;
			object propertyDateTime;
			object fakeObject;

			// Test at first level
			Assert.True(testClass.GetProperty(out propertyInt, "PropertyInt"), "Couldn't get PropertyInt");
			Assert.AreEqual(testInt, propertyInt, "PropertyInt wasn't expected value");
			Assert.True(testClass.GetProperty(out propertyString, "PropertyString"), "Couldn't Get PropertyString");
			Assert.AreEqual(testString, propertyString, "PropertyString wasn't expected value");
			Assert.True(testClass.GetProperty(out propertyDateTime, "PropertyDateTime"), "Couldn't get PropertyDateTime");
			Assert.AreEqual(testDateTime, propertyDateTime, "PropertyDateTime wasn't expected value");
			// Test properties that don't exist
			Assert.False(testClass.GetProperty(out fakeObject, "FakePropertyName"));


			// Test at second level
			Assert.True(testClass.GetProperty(out propertyInt, "InternalClass", "InternalPropertyInt"), "Couldn't get InternalPropertyInt");
			Assert.AreEqual(internalTestInt, propertyInt, "InternalPropertyInt wasn't expected value");
			Assert.True(testClass.GetProperty(out propertyString, "InternalClass", "InternalPropertyString"), "Couldn't Get InternalPropertyString");
			Assert.AreEqual(internalTestString, propertyString, "InternalPropertyString wasn't expected value");
			Assert.True(testClass.GetProperty(out propertyDateTime, "InternalClass", "InternalPropertyDateTime"), "Couldn't get InternalPropertyDateTime");
			Assert.AreEqual(internalTestDateTime, propertyDateTime, "InternalPropertyDateTime wasn't expected value");
			// Test properties that don't exist
			Assert.False(testClass.GetProperty(out fakeObject, "InternalClass", "FakePropertyName"));

			// Test at third level
			Assert.True(testClass.GetProperty(out propertyInt, "InternalClass", "DeepClass", "DeepPropertyInt"), "Couldn't get DeepPropertyInt");
			Assert.AreEqual(deepTestInt, propertyInt, "DeepPropertyInt wasn't expected value");
			Assert.True(testClass.GetProperty(out propertyString, "InternalClass", "DeepClass", "DeepPropertyString"), "Couldn't Get DeepPropertyString");
			Assert.AreEqual(deepTestString, propertyString, "DeepPropertyString wasn't expected value");
			Assert.True(testClass.GetProperty(out propertyDateTime, "InternalClass", "DeepClass", "DeepPropertyDateTime"), "Couldn't get DeepPropertyDateTime");
			Assert.AreEqual(deepTestDateTime, propertyDateTime, "DeepPropertyDateTime wasn't expected value");
			// Test properties that don't exist
			Assert.False(testClass.GetProperty(out fakeObject, "InternalClass", "DeepClass", "FakePropertyName"));


			// Test Default/Null Cases
			var emptyTestClass = new PropertyTestClass();
			Assert.True(emptyTestClass.GetProperty(out propertyInt, "PropertyInt"), "Couldn't get empty PropertyInt");
			Assert.AreEqual(default(int), propertyInt, "Empty PropertyInt wasn't expected value");
			Assert.True(emptyTestClass.GetProperty(out propertyString, "PropertyString"), "Couldn't Get empty PropertyString");
			Assert.AreEqual(default(string), propertyString, "Empty PropertyString wasn't expected value");
			Assert.True(emptyTestClass.GetProperty(out propertyDateTime, "PropertyDateTime"), "Couldn't get empty PropertyDateTime");
			Assert.AreEqual(default(DateTime), propertyDateTime, "Empty PropertyDateTime wasn't expected value");


		}

		[Test]
		public void SetPropertyTest()
		{
			var testClass = new PropertyTestClass();

			int testInt = 15;
			string testString = "TestString";
			var testDateTime = DateTime.Parse("1987-08-27");

			int internalTestInt = 34;
			string internalTestString = "InternalTestString";
			var internalTestDateTime = DateTime.Parse("2011-12-05");

			int deepTestInt = int.MaxValue;
			string deepTestString = "HelloThere";
			var deepTestDateTime = DateTime.Parse("1776-07-04");

			// Test at first level
			Assert.True(testClass.SetProperty(testInt, "PropertyInt"), "Couldn't get PropertyInt");
			Assert.AreEqual(testInt, testClass.PropertyInt, "PropertyInt wasn't expected value");
			Assert.True(testClass.SetProperty(testString, "PropertyString"), "Couldn't Get PropertyString");
			Assert.AreEqual(testString, testClass.PropertyString, "PropertyString wasn't expected value");
			Assert.True(testClass.SetProperty(testDateTime, "PropertyDateTime"), "Couldn't get PropertyDateTime");
			Assert.AreEqual(testDateTime, testClass.PropertyDateTime, "PropertyDateTime wasn't expected value");
			// Test properties that don't exist
			Assert.False(testClass.SetProperty("SomeObject", "FakePropertyName"));


			// Test at second level
			Assert.True(testClass.SetProperty(internalTestInt, "InternalClass", "InternalPropertyInt"), "Couldn't get InternalPropertyInt");
			Assert.AreEqual(internalTestInt, testClass.InternalClass.InternalPropertyInt, "InternalPropertyInt wasn't expected value");
			Assert.True(testClass.SetProperty(internalTestString, "InternalClass", "InternalPropertyString"), "Couldn't Get InternalPropertyString");
			Assert.AreEqual(internalTestString, testClass.InternalClass.InternalPropertyString, "InternalPropertyString wasn't expected value");
			Assert.True(testClass.SetProperty(internalTestDateTime, "InternalClass", "InternalPropertyDateTime"), "Couldn't get InternalPropertyDateTime");
			Assert.AreEqual(internalTestDateTime, testClass.InternalClass.InternalPropertyDateTime, "InternalPropertyDateTime wasn't expected value");
			// Test properties that don't exist
			Assert.False(testClass.SetProperty("SomeAdditionalObject", "InternalClass", "FakePropertyName"));

			// Test at third level
			Assert.True(testClass.SetProperty(deepTestInt, "InternalClass", "DeepClass", "DeepPropertyInt"), "Couldn't get DeepPropertyInt");
			Assert.AreEqual(deepTestInt, testClass.InternalClass.DeepClass.DeepPropertyInt, "DeepPropertyInt wasn't expected value");
			Assert.True(testClass.SetProperty(deepTestString, "InternalClass", "DeepClass", "DeepPropertyString"), "Couldn't Get DeepPropertyString");
			Assert.AreEqual(deepTestString, testClass.InternalClass.DeepClass.DeepPropertyString, "DeepPropertyString wasn't expected value");
			Assert.True(testClass.SetProperty(deepTestDateTime, "InternalClass", "DeepClass", "DeepPropertyDateTime"), "Couldn't get DeepPropertyDateTime");
			Assert.AreEqual(deepTestDateTime, testClass.InternalClass.DeepClass.DeepPropertyDateTime, "DeepPropertyDateTime wasn't expected value");
			// Test properties that don't exist
			Assert.False(testClass.SetProperty("ThisIsAnObjectToo", "InternalClass", "DeepClass", "FakePropertyName"));


			// Test Set Default/Null
			Assert.True(testClass.SetProperty(default(int), "PropertyInt"), "Couldn't get empty PropertyInt");
			Assert.AreEqual(default(int), testClass.PropertyInt, "Empty PropertyInt wasn't expected value");
			Assert.True(testClass.SetProperty(default(string), "PropertyString"), "Couldn't Get empty PropertyString");
			Assert.AreEqual(default(string), testClass.PropertyString, "Empty PropertyString wasn't expected value");
			Assert.True(testClass.SetProperty(default(DateTime), "PropertyDateTime"), "Couldn't get empty PropertyDateTime");
			Assert.AreEqual(default(DateTime), testClass.PropertyDateTime, "Empty PropertyDateTime wasn't expected value");


		}

		[Test]
		public void GetPropertyInfoTest()
		{
			var testClass = new PropertyTestClass();

			object instance;

			// Test GetPropertyInfo at various levels
			Assert.AreEqual(testClass.GetType().GetProperty("PropertyString"), 
			                testClass.GetPropertyInfo(out instance, "PropertyString"), 
			                "First level property not expected value");
			Assert.AreEqual(testClass, instance, "Unexpected property parent");
			Assert.AreEqual(testClass.InternalClass.GetType().GetProperty("InternalPropertyString"), 
			                testClass.GetPropertyInfo(out instance, "InternalClass", "InternalPropertyString"), 
			                "Second level property not expected value");
			Assert.AreEqual(testClass.InternalClass, instance, "Unexpected property parent");
			Assert.AreEqual(testClass.InternalClass.DeepClass.GetType().GetProperty("DeepPropertyString"), 
			                testClass.GetPropertyInfo(out instance, "InternalClass", "DeepClass", "DeepPropertyString"),
			                "Third level property not expected value");
			Assert.AreEqual(testClass.InternalClass.DeepClass, instance, "Unexpected property parent");

			// Property that doesn't exits should return null
			Assert.IsNull(testClass.GetPropertyInfo(out instance, "InternalClass", "DeepClass", "NonExistent"));
			Assert.IsNull(instance);
			Assert.IsNull(testClass.GetPropertyInfo(out instance, "FakeFirstLevel", "FakeSecondLevel" , "FakeThridLevel"));
			Assert.IsNull(instance);
			Assert.IsNull(testClass.GetPropertyInfo(out instance, "InternalClass", "FakeSecondLevel", "ThirdLevelCanNotBeReal"));
			Assert.IsNull(instance);
			Assert.IsNull(testClass.GetPropertyInfo(out instance, "InternalClass", "FakeSecondLevel"));
			Assert.IsNull(instance);
			Assert.IsNull(testClass.GetPropertyInfo(out instance, "FakeFirstLevel"));
			Assert.IsNull(instance);

		}

		#endregion
	}
}
