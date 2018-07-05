using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    static public class FoodTypeFlagsExt
    {
		static readonly public int energyConsumableFlag = 0x10000;
    
		static public FoodTypeFlags EnergyConsumableFlag() => (FoodTypeFlags)0x10000;
		static public bool IsEnergyConsumable(this FoodTypeFlags flags) =>
					((int)flags & energyConsumableFlag) != 0;
		static public FoodTypeFlags AddEnergyConsumableFlag(this FoodTypeFlags flags) =>
			(FoodTypeFlags)((int)flags | energyConsumableFlag);
		static public FoodTypeFlags RemoveEnergyConsumableFlag(this FoodTypeFlags flags) =>
			(FoodTypeFlags)((int)flags & (~energyConsumableFlag));
    }
}
