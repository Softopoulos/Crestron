using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if SIMPL_SHARP
namespace Softopoulos.Crestron.Core.Diagnostics
#else
namespace Softopoulos.Core.Diagnostics
#endif
{
	public enum DebugLevel
	{
		None = 0,
		Info,
		Normal,
		Debug,
		Verbose,
	}
}