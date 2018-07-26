using System;
namespace MOARANDROIDS
{
    public enum ConnectWhenTag
    {
        Never,
        WhenTouching,
        WhenClose  
    }

	public struct ConnectWhen
	{
		public ConnectWhenTag when;
		public float distance;

		ConnectWhen(ConnectWhenTag when, float distance)
		{
			this.when = when;
			this.distance = distance;
		}
	}    
}
