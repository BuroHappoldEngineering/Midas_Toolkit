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
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private int DeleteNodes(IEnumerable<object> ids)
        {
            int success = 1;

            if (ids.Count() != 0)
            {
                string path = m_directory + "\\TextFiles\\" + "NODE" + ".txt";

                if (File.Exists(path))
                {
                    List<string> stringIndex = ids.Select(x => x.ToString()).ToList();

                    List<int> indices = stringIndex.Select(int.Parse).ToList();

                    List<string> nodes = File.ReadAllLines(path, Encoding.GetEncoding(1252)).ToList();

                    List<int> nodeIndexes = new List<int>();
                    foreach (string node in nodes)
                    {
                        if (node.Contains(";") || node.Contains("*") || string.IsNullOrWhiteSpace(node))
                        {
                            int clone = 0;
                            nodeIndexes.Add(clone);
                        }
                        else
                        {
                            int clone = int.Parse(node.Split(',')[0].Trim());
                            nodeIndexes.Add(clone);
                        }
                    }

                    foreach (int index in indices)
                    {
                        if (nodeIndexes.Contains(index))
                        {
                            int nodeIndex = nodeIndexes.IndexOf(index);
                            nodes[nodeIndex] = "";
                        }
                    }

                    nodes = nodes.Where(x => !string.IsNullOrEmpty(x)).ToList();

                    WriteToANSI(path, nodes);
                }
            }
            return success;
        }

        /***************************************************/

    }
}

