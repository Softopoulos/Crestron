using System;

using Bool = System.Int16;
using Softopoulos.Crestron.Core;

namespace Softopoulos.Crestron.PhilipsHue
{
	public class ErrorOccurredEventArgs : EventArgs
	{
		internal ErrorOccurredEventArgs(string errorMessage, bool isUserError)
		{
			ErrorMessage = errorMessage;
			IsUserError = PlatformConverter.ToPlatformBool(isUserError);
		}

		/// <summary>
		/// Message describing any error that occurred during the current method call or any attempt to change a Hue property
		/// <para>
		/// Use the <see cref="IsUserError"/> property to determine if the this error is a user error
		/// (e.g. not pressing the link button on the Hue Bridge during authentication)
		/// </para>
		/// </summary>
		public string ErrorMessage { get; private set; }

		/// <summary>
		/// True if this error is due to user error;
		/// (e.g. not pressing the link button on the Hue Bridge during authentication)
		/// </summary>
		public Bool IsUserError { get; private set; }
	}

	public class CheckForSoftwareUpdatesCompletedEventArgs : EventArgs
	{
		internal CheckForSoftwareUpdatesCompletedEventArgs(CheckForSoftwareUpdatesResult result)
		{
			Result = result;
		}

		public CheckForSoftwareUpdatesResult Result { get; private set; }
	}

	public class ApplySoftwareUpdatesCompletedEventArgs : EventArgs
	{
		internal ApplySoftwareUpdatesCompletedEventArgs(ApplySoftwareUpdatesResult result)
		{
			Result = result;
		}

		public ApplySoftwareUpdatesResult Result { get; private set; }
	}

	public class SearchForNewLightBulbsCompletedEventArgs : EventArgs
	{
		internal SearchForNewLightBulbsCompletedEventArgs(SearchForNewLightBulbsResult result)
		{
			Result = result;
		}

		public SearchForNewLightBulbsResult Result { get; private set; }
	}

	public class FoundLightBulbEventArgs : EventArgs
	{
		internal FoundLightBulbEventArgs(string id, string lightBulbName)
		{
			Id = id;
			LightBulbName = lightBulbName;
		}

		public string Id { get; private set; }
		public string LightBulbName { get; private set; }
	}

	public class LightBulbPropertiesChangedEventArgs : EventArgs
	{
		internal LightBulbPropertiesChangedEventArgs(LightBulb lightBulb)
		{
			Index = lightBulb.Index;
			Id = lightBulb.Id;
			UniqueId = lightBulb.UniqueId;
		}

		public ushort Index { get; private set; }
		public string Id { get; private set; }
		public string UniqueId { get; private set; }
	}

	public class ScenePropertiesChangedEventArgs : EventArgs
	{
		internal ScenePropertiesChangedEventArgs(Scene scene)
		{
			Index = scene.Index;
			Id = scene.Id;
		}

		public ushort Index { get; private set; }
		public string Id { get; private set; }
	}
}
