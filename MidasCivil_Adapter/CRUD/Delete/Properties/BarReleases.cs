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

        private int DeleteBarReleases(IEnumerable<object> ids)
        {
            int success = 1;

            if (ids.Count() != 0)
            {
                string path = m_directory + "\\TextFiles\\" + "FRAME-RLS" + ".txt";

                if (File.Exists(path))
                {
                    List<string> names = ids.Cast<string>().ToList();
                    List<string> releases = File.ReadAllLines(path, Encoding.GetEncoding(1252)).ToList();
                    List<string> releaseNames = new List<string>();

                    for (int i = 0; i < releases.Count; i++)
                    {
                        if (releases[i].Contains("*") || releases[i].Contains(";") || string.IsNullOrWhiteSpace(releases[i]))
                        {
                            string clone = "0";
                            releaseNames.Add(clone);
                        }
                        else
                        {
                            string clone = releases[i + 1].Split(',')[7].Trim();
                            releaseNames.Add(clone);
                            releaseNames.Add(clone);
                            i++;
                        }
                    }

                    foreach (string name in names)
                    {
                        if (releaseNames.Contains(name))
                        {
                            var nameIndexes = releaseNames.Select((l, i) => new { l, i })
                                .Where(x => x.l == name)
                                .Select(x => x.i)
                                .ToList();

                            foreach (var index in nameIndexes)
                            {
                                releases[index] = "";
                            }
                        }
                    }

                    releases = releases.Where(x => !string.IsNullOrEmpty(x)).ToList();

                    WriteToANSI(path, releases);
                }
            }
            return success;
        }

        /***************************************************/

    }
}

