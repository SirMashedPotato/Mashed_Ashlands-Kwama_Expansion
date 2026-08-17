using UnityEngine;
using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class Mashed_Ashlands_Kwama_Mod : Mod
    {
        Mashed_Ashlands_Kwama_ModSettings settings;

        public Mashed_Ashlands_Kwama_Mod(ModContentPack contentPack) : base(contentPack)
        {
            settings = GetSettings<Mashed_Ashlands_Kwama_ModSettings>();
            Log.Message("[Mashed's Ashlands - Kwama] version " + Content.ModMetaData.ModVersion);
        }

        public override string SettingsCategory() => "Mashed_Ashlands_Kwama_ModName".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);

            listing_Standard.CheckboxLabeled("Mashed_Ashlands_Kwama_EnableStartingKwamaNestEntrance".Translate(), ref settings.Mashed_Ashlands_Kwama_EnableStartingKwamaNestEntrance);
            listing_Standard.Gap();

            listing_Standard.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
}
