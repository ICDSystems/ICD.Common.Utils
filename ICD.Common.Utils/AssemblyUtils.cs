using System.Linq;
using System.Text;
using ICD.Common.Properties;
using ICD.Common.Utils.IO;
#if SIMPLSHARP
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronXmlLinq;
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif

namespace ICD.Common.Utils
{
	public static class AssemblyUtils
	{
		/// <summary>
		/// Gets the process executable in the default application domain. In other application domains,
		/// this is the first executable that was executed by ExecuteAssembly(String).
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		public static Assembly GetEntryAssembly()
		{
#if SIMPLSHARP
			string appDir = InitialParametersClass.ProgramDirectory.ToString();

			if (CrestronEnvironment.RuntimeEnvironment == eRuntimeEnvironment.SIMPL)
				return null;

			string proginfo = IcdFile.ReadToEnd(IcdPath.Combine(appDir, "ProgramInfo.config"), Encoding.UTF8);
			XDocument doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n" + proginfo);
			XElement entry = doc.Descendants("EntryPoint").FirstOrDefault();
			return entry == null ? null : Assembly.Load(entry.Value);
#else
			return Assembly.GetEntryAssembly();
#endif
		}
	}
}
