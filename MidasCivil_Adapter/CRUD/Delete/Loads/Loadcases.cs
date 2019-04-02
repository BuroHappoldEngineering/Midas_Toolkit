﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter
    {
        public int DeleteLoadcases(IEnumerable<object> ids)
        {
            int success = 1;

            if (ids != null)
            {
                string path = directory + "\\TextFiles\\" + "STLDCASE" + ".txt";

                if (File.Exists(path))
                {
                    List<string> names = ids.Cast<string>().ToList();

                    List<string> loadcases = File.ReadAllLines(path).ToList();

                    List<string> loadcaseNames = new List<string>();
                    foreach (string loadcase in loadcases)
                    {
                        if (loadcase.Contains(";") || loadcase.Contains("*"))
                        {
                            string clone = 0.ToString();
                            loadcaseNames.Add(clone);
                        }
                        else
                        {
                            string clone = loadcase.Split(',')[0].Replace(" ", "");
                            loadcaseNames.Add(clone);
                        }
                    }

                    foreach (string name in names)
                    {
                        if (loadcaseNames.Contains(name))
                        {
                            int nameIndex = loadcaseNames.IndexOf(name);
                            loadcases[nameIndex] = "";
                        }
                    }

                    loadcases = loadcases.Where(x => !string.IsNullOrEmpty(x)).ToList();

                    File.Delete(path);
                    File.WriteAllLines(path, loadcases.ToArray());
                }
            }
            return success;
        }
    }
}
