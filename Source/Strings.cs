using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WhoIsUsingThat {
    public static class Strings {
        public const string ID = "kathanon.WhoIsUsingThat";
        public const string Name = "Who's Using That?";

        private static readonly string AddedDescriptionKeyFormat = 
            "kathanon.WhoIsUsingThat.AddedDescription.Job{0}.Count{1}";

        public static string AddedDescription(string name, string job, int count) 
            => string.Format(AddedDescriptionKeyFormat, (job != null), (count > 0))
                     .Translate(name, job, count);
    }
}
