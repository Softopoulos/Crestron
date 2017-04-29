using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Softopoulos.Crestron.Core
{
	public static class PlatformConverter
	{
		public static short ToPlatformBool(bool value)
		{
			return value ? (short)1 : (short)0;
		}
		public static bool ToBool(short value)
		{
			return value != 0;
		}
	}
}