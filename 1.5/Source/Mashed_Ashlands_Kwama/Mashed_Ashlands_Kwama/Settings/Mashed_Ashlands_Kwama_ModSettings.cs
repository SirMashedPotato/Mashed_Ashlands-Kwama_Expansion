using Verse;

namespace Mashed_Ashlands_Kwama
{
    public class Mashed_Ashlands_Kwama_ModSettings : ModSettings
    {
        private static Mashed_Ashlands_Kwama_ModSettings _instance;

        /* ==========[GETTERS]========== */
        public static bool EnableStartingKwamaNestEntrance => _instance.Mashed_Ashlands_Kwama_EnableStartingKwamaNestEntrance;

        /* ==========[VARIABLES]========== */
        public bool Mashed_Ashlands_Kwama_EnableStartingKwamaNestEntrance = true;

        public Mashed_Ashlands_Kwama_ModSettings()
        {
            _instance = this;
        }

        /* ==========[SAVING]========== */
        public override void ExposeData()
        {
            Scribe_Values.Look(ref Mashed_Ashlands_Kwama_EnableStartingKwamaNestEntrance, "Mashed_Ashlands_Kwama_EnableStartingKwamaNestEntrance", true);
        }
    }
}
