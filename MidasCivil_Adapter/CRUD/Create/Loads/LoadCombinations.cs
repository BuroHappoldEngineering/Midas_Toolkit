﻿using BH.oM.Structure.Loads;
using System.Collections.Generic;
using System.IO;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter
    {
        public bool CreateCollection(IEnumerable<LoadCombination> loadCombinations)
        {
            return true;
        }
    }
}
