using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeightTrackerTests.ModelsTests
{
    internal class Helper
    {
        public static int GetNumErrors(Dictionary<string, List<string>> errors)
        {
            int numErrors = 0;
            foreach (var item in errors)
            {
                numErrors += item.Value.Count;
            }
            return numErrors;
        }
    }
}
