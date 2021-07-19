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

        private int DeletePointLoads(IEnumerable<object> ids)
        {
            int success = 1;

            if (ids.Count() != 0)
            {
                string[] loadcaseNames = Directory.GetDirectories(m_directory + "\\TextFiles\\");

                foreach (string loadcaseName in loadcaseNames)
                {
                    string path = loadcaseName + "\\CONLOAD.txt";
                    List<string> loadgroups = ids.Cast<string>().ToList();

                    if (File.Exists(path))
                    {
                        List<string> loads = File.ReadAllLines(path, Encoding.GetEncoding(1252)).ToList();

                        List<string> loadNames = new List<string>();
                        foreach (string load in loads)
                        {
                            if (!load.Contains(";") || load.Contains("*") || string.IsNullOrWhiteSpace(load))
                            {
                                loadNames.Add("0");
                            }
                            else
                            {
                                string loadName = load.Split(',').Reverse().First().Trim();
                                loadNames.Add(loadName);
                            }
                        }

                        foreach (string loadgroup in loadgroups)
                        {
                            if (loadNames.Contains(loadgroup))
                            {
                                int nameIndex = loadNames.IndexOf(loadgroup);
                                loads[nameIndex] = "";
                            }
                        }

                        loads = loads.Where(x => !string.IsNullOrEmpty(x)).ToList();

                        WriteToANSI(path, loads);
                    }
                }
            }

            return success;
        }

        /***************************************************/

    }
}

