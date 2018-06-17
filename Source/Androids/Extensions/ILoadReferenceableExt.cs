using System;
using Verse;
using Harmony;

namespace MOARANDROIDS
{
    static public class ILoadReferenceableExt
    {
		static public void ForceRegisterReferenceable(this ILoadReferenceable loadReferenceable)
		{
            if(Scribe.mode == LoadSaveMode.LoadingVars)
                Traverse.Create(Scribe.loader.crossRefs).Field("loadedObjectDirectory")
                    .Method("RegisterLoaded", new object[1] { loadReferenceable }).GetValue();
		}
    }
}
