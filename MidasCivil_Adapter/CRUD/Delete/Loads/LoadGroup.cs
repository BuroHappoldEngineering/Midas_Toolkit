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

        private int DeleteLoadGroups(IEnumerable<object> ids)
        {
            int success = 1;

            if (ids.Count() != 0)
            {
                string path = m_directory + "\\TextFiles\\" + "LOADGROUP" + ".txt";

                if (File.Exists(path))
                {
                    List<string> names = ids.Cast<string>().ToList();

                    List<string> loadcases = File.ReadAllLines(path, Encoding.GetEncoding(1252)).ToList();

                    List<string> loadcaseNames = new List<string>();
                    foreach (string loadcase in loadcases)
                    {
                        if (loadcase.Contains(";") || loadcase.Contains("*") || string.IsNullOrWhiteSpace(loadcase))
                        {
                            string clone = 0.ToString();
                            loadcaseNames.Add(clone);
                        }
                        else
                        {
                            string clone = loadcase.Split(',')[0].Trim();
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

                    WriteToANSI(path, loadcases);
                }
            }
            return success;
        }

        /***************************************************/

    }
}

