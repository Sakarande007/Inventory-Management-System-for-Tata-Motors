using System.Collections.Generic;

namespace PaintShopIMS.Helpers
{
    public static class DefectCodes
    {
        public static readonly Dictionary<string, string> Codes = new()
        {
            { "01", "01 Damage" },
            { "02", "02 Scratch" },
            { "03", "03 Dust" },
            { "04", "04 Rundown" },
            { "05", "05 Lint" },
            { "06", "06 Other" }
        };
    }
}
