using HarmonyLib;
using Verse;
using System.Reflection;

namespace Mashed_Ashlands_Kwama
{
    [StaticConstructorOnStartup]
    public class Main
    {
        static Main()
        {
            Harmony harmony = new Harmony("com.Mashed_Ashlands_Kwama");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
