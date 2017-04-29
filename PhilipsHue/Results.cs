using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Softopoulos.Crestron.PhilipsHue
{
	internal class Results<T> : List<T>
		where T : ResultBase
	{
		public IEnumerable<T> Errors
		{
			get { return this.Where(x => x.Error != null); }
		}
	}

	internal sealed class BasicResult : ResultBase
	{
		[JsonProperty("success")]
		public string Success { get; internal set; }
	}

	internal sealed class Result : ResultBase
	{
		[JsonProperty("success")]
		public Dictionary<string, object> Success { get; internal set; }
	}

	internal sealed class ErrorOnlyResult : ResultBase
	{
	}

	internal abstract class ResultBase
	{
		[JsonProperty("error")]
		public Error Error { get; internal set; }
	}

	internal class Error
	{
		public Error()
		{
		}

		[JsonProperty("type")]
		public int Type { get; internal set; }

		[JsonProperty("address")]
		public string Address { get; internal set; }

		[JsonProperty("description")]
		public string Description { get; internal set; }
	}
}
