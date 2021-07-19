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
using System.Text.RegularExpressions;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter
    {
        public int DeleteAreaUniformlyDistributedLoads(IEnumerable<object> ids)
        {
            int success = 1;

            if (ids.Count()!=0)
            {
                string[] loadcaseNames = Directory.GetDirectories(m_directory+ "\\TextFiles\\");

                foreach (string loadcaseName in loadcaseNames)
                {
                    string path = loadcaseName + "\\PRESSURE.txt";
                    List<string> loadgroups = ids.Cast<string>().ToList();

                    if (File.Exists(path))
                    {
                        List<string> loads = File.ReadAllLines(path, Encoding.GetEncoding(1252)).ToList();

                        List<string> loadNames = new List<string>();
                        foreach (string load in loads)
                        {
                            if (load.Contains(";") || load.Contains("*") || string.IsNullOrWhiteSpace(load))
                            {
                                loadNames.Add("0");
                            }
                            else
                            {
                                string[] delimitted = load.Split(',');
                                string clone = delimitted[14].Trim();
                                loadNames.Add(clone);
                            }
                        }

                        foreach (string loadgroup in loadgroups)
                        {
                            if (loadNames.Contains(loadgroup))
                            {
                                var nameIndexes = loadNames.Select((l, i) => new { l, i })
                                    .Where(x => x.l == loadgroup)
                                    .Select(x => x.i)
                                    .ToList();

                                foreach (var index in nameIndexes)
                                {
                                    loads[index] = "";
                                }
                            }
                        }

                        loads = loads.Where(x => !string.IsNullOrEmpty(x)).ToList();

                        WriteToANSI(path, loads);
                    }
                }
            }

            return success;
        }
    }
}

