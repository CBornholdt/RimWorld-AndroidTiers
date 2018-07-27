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

		public ConnectWhen(ConnectWhenTag when, float distance = 0)
		{
			this.when = when;
			this.distance = distance;
		}
	}    
}
