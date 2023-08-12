using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Verse;

namespace WhoIsUsingThat;
[HarmonyPatch]
public static class Patch_Inspect {
    private static readonly string[] DescPatterns = {
        "Reserved by {0}",
        "Reserved by {0} for {1}",
        "{2} reserved by {0}",
        "{2} reserved by {0} for {1}",
    };

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

        var name = pawn.NameShortColored;
        Regex.Escape(self.Label);
        string job = res.Job?.GetReport(pawn)?.Trim();
        string pattern = "([ ]+|(?<![a-zA-Z0-9]))" + Regex.Escape(self.LabelNoCount) + "( x[0-9]+)?([ ]+|(?![a-zA-Z0-9]))";
        job = Regex.Replace(job, pattern, " ", RegexOptions.IgnoreCase);
        int count = (res.StackCount < self.stackCount) ? res.StackCount : 0;
        int i = ((job != null) ? 1 : 0) + ((count != 0) ? 2 : 0);
        var desc = string.Format(DescPatterns[i], name, job, count);
        __result = (__result.NullOrEmpty()) ? desc : $"{__result}\n{desc}";
    }
}
