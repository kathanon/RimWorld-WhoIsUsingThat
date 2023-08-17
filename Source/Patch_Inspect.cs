using HarmonyLib;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Verse;

namespace WhoIsUsingThat;
[HarmonyPatch]
public static class Patch_Inspect {
    private static bool logException = true;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Thing), nameof(Thing.GetInspectString))]
    public static void GetInspectString(Thing __instance, ref string __result) {
        var self = __instance;
        var manager = self.Map?.reservationManager;
        var faction = Find.FactionManager.OfPlayer;
        var res = manager?.ReservationsReadOnly
            .Where(x => x.Target == self && x.Faction == faction)
            .FirstOrDefault();
        if (res == null) return;

        var pawn = res.Claimant;
        if (pawn == null) return;

        try {
            var name = pawn.NameShortColored;
            string job = res.Job?.GetReport(pawn)?.Trim();
            if (job != null) {
                string pattern = "[ ]*\\b" + Regex.Escape(self.LabelNoCount) + "( x[0-9]+)?[ ]*";
                job = Regex.Replace(job, pattern, " ", RegexOptions.IgnoreCase);
            }
            int count = (res.StackCount < self.stackCount) ? res.StackCount : 0;
            var desc = Strings.AddedDescription(name, job, count);
            __result = (__result.NullOrEmpty()) ? desc : $"{__result}\n{desc}";
        } catch (Exception ex) {
            if (logException) {
                var text = $"GetInspectString exception on {self}:\n{ex}";
                Log.Error(text);
                logException = false;
            }
        }
    }
}
