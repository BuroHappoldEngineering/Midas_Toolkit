/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private int DeleteLoadCombinations(IEnumerable<object> ids)
        {
            int success = 1;

            if (ids.Count() != 0)
            {
                string path = m_directory + "\\TextFiles\\" + "LOADCOMB" + ".txt";

                if (File.Exists(path))
                {
                    List<string> names = ids.Cast<string>().ToList();

                    List<string> loadCombinations = File.ReadAllLines(path, Encoding.GetEncoding(1252)).ToList();

                    List<string> loadCombinationNames = new List<string>();

                    for (int i = 0; i < loadCombinations.Count; i++)
                    {
                        if (loadCombinations[i].Contains(";") || loadCombinations[i].Contains("*") || string.IsNullOrWhiteSpace(loadCombinations[i]))
                        {
                            string clone = 0.ToString();
                            loadCombinationNames.Add(clone);
                        }
                        else
                        {
                            string clone = loadCombinations[i].Split(',')[0].Split('=')[1].Trim();
                            loadCombinationNames.Add(clone);
                            loadCombinationNames.Add(clone);
                            i++;
                        }
                    }

                    foreach (string name in names)
                    {
                        if (loadCombinationNames.Contains(name))
                        {
                            var nameIndexes = loadCombinationNames.Select((l, i) => new { l, i })
                                .Where(x => x.l == name)
                                .Select(x => x.i)
                                .ToList();

                            foreach (var index in nameIndexes)
                            {
                                loadCombinations[index] = "";
                            }
                        }
                    }

                    loadCombinations = loadCombinations.Where(x => !string.IsNullOrEmpty(x)).ToList();

                    WriteToANSI(path, loadCombinations);
                }
            }
            return success;
        }

        /***************************************************/

    }
}

