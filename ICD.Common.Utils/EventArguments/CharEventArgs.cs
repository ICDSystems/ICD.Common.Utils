using ICD.Common.Properties;

namespace ICD.Common.Utils.EventArguments
{
	[PublicAPI]
	public sealed class CharEventArgs : GenericEventArgs<char>
	{
		public CharEventArgs(char value) : base(value)
		{
		}
	}
}
