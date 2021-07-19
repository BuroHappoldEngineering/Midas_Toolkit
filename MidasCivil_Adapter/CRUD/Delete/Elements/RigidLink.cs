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

        private int DeleteRigidLinks(IEnumerable<object> ids)
        {
            int success = 1;

            if (ids.Count() != 0)
            {
                string path = m_directory + "\\TextFiles\\" + "RIGIDLINK" + ".txt";

                if (File.Exists(path))
                {
                    List<string> names = ids.Cast<string>().ToList();
                    List<string> links = File.ReadAllLines(path, Encoding.GetEncoding(1252)).ToList();
                    List<string> linkNames = new List<string>();

                    foreach (string link in links)
                    {
                        if (link.Contains("*") || link.Contains(";") || string.IsNullOrWhiteSpace(link))
                        {
                            string clone = "0";
                            linkNames.Add(clone);
                        }
                        else
                        {
                            string clone = link.Split(',')[3].Trim();
                            linkNames.Add(clone);
                        }
                    }

                    foreach (string name in names)
                    {
                        if (linkNames.Contains(name))
                        {
                            var nameIndexes = linkNames.Select((l, i) => new { l, i })
                                .Where(x => x.l == name)
                                .Select(x => x.i)
                                .ToList();

                            foreach (var index in nameIndexes)
                            {
                                links[index] = "";
                            }
                        }
                    }

                    links = links.Where(x => !string.IsNullOrEmpty(x)).ToList();

                    WriteToANSI(path, links);
                }
            }
            return success;
        }

        /***************************************************/

    }
}

