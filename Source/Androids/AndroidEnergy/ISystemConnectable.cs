using System;
namespace MOARANDROIDS
{
    public interface ISystemConnectable
    {
		ConnectWhen whenToConnect();

		bool availableToConnect();
    }
}
