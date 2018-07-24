using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class CopyablePawn : IExposable
    {
		bool holdingPawn = false;
        Pawn source;

		public CopyablePawn() { }

		public CopyablePawn(Pawn source)
		{
			this.source = source;

		}

		public void CopyTo(Pawn other)
		{
			other.Name = source.Name;
			if(other.story == null)
				other.story = new Pawn_StoryTracker(other);
			other.story.adulthood = source.story.adulthood;
			other.story.childhood = source.story.childhood;
			other.story.traits = source.story.traits;

			foreach(var skill in other.skills.skills) {
				skill.levelInt = source.skills.GetSkill(skill.def).Level;
				skill.passion = source.skills.GetSkill(skill.def).passion;
				skill.Learn(skill.XpRequiredForLevelUp / 2);
			}
		}

		public void ExposeData()
		{
				if(Scribe.mode == LoadSaveMode.Saving && source.Discarded)
					this.holdingPawn = true;
        
			Scribe_Values.Look<bool>(ref this.holdingPawn, "HoldingPawn");
			if(this.holdingPawn)
				Scribe_Deep.Look<Pawn>(ref this.source, "SourceHeld");
			else
				Scribe_References.Look<Pawn>(ref this.source, "Source");
		}
    }
}
