#if SIMPLSHARP
// =============================================================================
//          Definition of Custom Attributes for Declarative Obfuscation
// =============================================================================
//
// This file is only necessary for the projects targeting one of the following:
//
//   - .NET Core version 1.1 or lower
//   - Universal Windows Platform (UWP) version 10.0.15063 or lower
//   - Portable Class Library (PCL)
//   - WinRT
//   - Silverlight
//   - .NET Compact Framework
//
// =============================================================================

using System;

// ReSharper disable once CheckNamespace

namespace System.Reflection
{
    /// <summary>
    /// Instructs obfuscation tools to use their standard obfuscation rules for the appropriate assembly type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class ObfuscateAssemblyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObfuscateAssemblyAttribute"/> class,
        /// specifying whether the assembly to be obfuscated is public or private.
        /// </summary>
        /// <param name="assemblyIsPrivate"><c>true</c> if the assembly is used within the scope of one application; otherwise, <c>false</c>.</param>
        public ObfuscateAssemblyAttribute(bool assemblyIsPrivate)
        {
            m_AssemblyIsPrivate = assemblyIsPrivate;
            m_StripAfterObfuscation = true;
        }

	    readonly bool m_AssemblyIsPrivate;

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value indicating whether the assembly was marked private.
        /// </summary>
        /// <value>
        /// <c>true</c> if the assembly was marked private; otherwise, <c>false</c>.
        /// </value>
        public bool AssemblyIsPrivate
        {
            get { return m_AssemblyIsPrivate; }
        }

        bool m_StripAfterObfuscation;

        /// <summary>
        /// Gets or sets a <see cref="System.Boolean"/> value indicating whether the obfuscation tool should remove the attribute after processing.
        /// </summary>
        /// <value>
        /// <c>true</c> if the obfuscation tool should remove the attribute after processing; otherwise, <c>false</c>.
        /// The default value for this property is <c>true</c>.
        /// </value>
        public bool StripAfterObfuscation
        {
            get { return m_StripAfterObfuscation; }
            set { m_StripAfterObfuscation = value; }
        }
    }

    /// <summary>
    /// Instructs obfuscation tools to take the specified actions for an assembly, type, or member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Parameter | AttributeTargets.Interface | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class ObfuscationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObfuscationAttribute"/> class.
        /// </summary>
        public ObfuscationAttribute()
        {
            m_ApplyToMembers = true;
            m_Exclude = true;
            m_Feature = "all";
            m_StripAfterObfuscation = true;
        }

        bool m_ApplyToMembers;

        /// <summary>
        /// Gets or sets a <see cref="System.Boolean"/> value indicating whether the attribute of a type is to apply to the members of the type.
        /// </summary>
        /// <value>
        /// <c>true</c> if the attribute is to apply to the members of the type; otherwise, <c>false</c>. The default is <c>true</c>.
        /// </value>
        public bool ApplyToMembers
        {
            get { return m_ApplyToMembers; }
            set { m_ApplyToMembers = value; }
        }

        bool m_Exclude;

        /// <summary>
        /// Gets or sets a <see cref="System.Boolean"/> value indicating whether the obfuscation tool should exclude the type or member from obfuscation.
        /// </summary>
        /// <value>
        /// <c>true</c> if the type or member to which this attribute is applied should be excluded from obfuscation; otherwise, <c>false</c>.
        /// The default is <c>true</c>.
        /// </value>
        public bool Exclude
        {
            get { return m_Exclude; }
            set { m_Exclude = value; }
        }

        string m_Feature;

        /// <summary>
        /// Gets or sets a string value that is recognized by the obfuscation tool, and which specifies processing options.
        /// </summary>
        /// <value>
        /// A string value that is recognized by the obfuscation tool, and which specifies processing options. The default is "all".
        /// </value>
        public string Feature
        {
            get { return m_Feature; }
            set { m_Feature = value; }
        }

        bool m_StripAfterObfuscation;

        /// <summary>
        /// Gets or sets a <see cref="System.Boolean"/> value indicating whether the obfuscation tool should remove the attribute after processing.
        /// </summary>
        /// <value>
        /// <c>true</c> if the obfuscation tool should remove the attribute after processing; otherwise, <c>false</c>.
        /// The default value for this property is <c>true</c>.
        /// </value>
        public bool StripAfterObfuscation
        {
            get { return m_StripAfterObfuscation; }
            set { m_StripAfterObfuscation = value; }
        }
    }
}
#endif
