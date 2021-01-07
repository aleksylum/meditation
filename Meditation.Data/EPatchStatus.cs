//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System.Xml.Serialization;

namespace Meditation.Data
{
	public enum EPatchStatus
	{
		[XmlEnum("1")] New = 1,
		[XmlEnum("2")] Patched = 2,
		[XmlEnum("3")] HasErrorDuringPatching = 3,
		[XmlEnum("4")] WaitingForUnpatcing = 4,
		[XmlEnum("5")] Unpatched = 5,
		[XmlEnum("6")] HasErrorDuringUnpatching = 6,
	}
}
