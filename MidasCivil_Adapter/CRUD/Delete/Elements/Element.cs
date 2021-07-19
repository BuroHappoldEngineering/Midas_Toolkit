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

        private int DeleteElements(IEnumerable<object> ids)
        {
            int success = 1;

            if (ids.Count() != 0)
            {
                string path = m_directory + "\\TextFiles\\" + "ELEMENT" + ".txt";

                if (File.Exists(path))
                {
                    List<string> stringIndex = ids.Select(x => x.ToString()).ToList();

                    List<int> indices = stringIndex.Select(int.Parse).ToList();

                    List<string> elements = File.ReadAllLines(path, Encoding.GetEncoding(1252)).ToList();
                    List<int> elementIndexes = new List<int>();

                    foreach (string element in elements)
                    {
                        if (element.Contains("*") || element.Contains(";") || string.IsNullOrWhiteSpace(element))
                        {
                            int clone = 0;
                            elementIndexes.Add(clone);
                        }
                        else
                        {
                            int clone = int.Parse(element.Split(',')[0].Trim());
                            elementIndexes.Add(clone);
                        }
                    }

                    foreach (int index in indices)
                    {
                        if (elementIndexes.Contains(index))
                        {
                            int elementIndex = elementIndexes.IndexOf(index);
                            elements[elementIndex] = "";
                        }
                    }

                    elements = elements.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                    WriteToANSI(path, elements);
                }
            }
            return success;
        }

        /***************************************************/

    }
}

