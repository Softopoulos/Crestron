namespace Softopoulos.Crestron.PhilipsHue
{
	public abstract class IdentifiableHueObject : HueObject
	{
		/// <summary>
		/// ID of this object; This can change if the device is deleted and then recreated/rediscovered by the Hue Bridge.
		/// <para>
		/// Some devices contain unique identifiers that will never change (such as MAC addresses), so when you want to ensure
		/// you are only ever addressing that device, those properties should be used instead.  For flexibility, the methods
		/// on the various objects in this namespace all provide various ways of addressing devices (by index, ID or unique ID).
		/// </para>
		/// </summary>
		public string Id { get; internal set; }
	}
}