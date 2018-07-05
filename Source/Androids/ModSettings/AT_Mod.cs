using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class AT_Mod : Mod
    {
		static public AT_ModSettings settings;

		public AT_Mod(ModContentPack contentPack) : base(contentPack)
		{
			settings = GetSettings<AT_ModSettings>();
		}

		public override string SettingsCategory() => "AT_Mod.CategoryLabel".Translate();
	}
}
