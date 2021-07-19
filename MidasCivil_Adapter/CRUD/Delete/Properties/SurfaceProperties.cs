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

        private int DeleteSurfaceProperties(IEnumerable<object> ids)
        {
            int success = 1;

            if (ids.Count() != 0)
            {
                string path = m_directory + "\\TextFiles\\" + "THICKNESS" + ".txt";

                if (File.Exists(path))
                {
                    List<string> stringIndex = ids.Cast<string>().ToList();

                    List<int> indices = stringIndex.Select(int.Parse).ToList();

                    List<string> thicknesses = File.ReadAllLines(path, Encoding.GetEncoding(1252)).ToList();

                    List<int> thicknessIndexes = new List<int>();

                    for (int i = 0; i < thicknesses.Count; i++)
                    {
                        if (thicknesses[i].Contains(";") || thicknesses[i].Contains("*") || string.IsNullOrWhiteSpace(thicknesses[i]))
                        {
                            int clone = 0;
                            thicknessIndexes.Add(clone);
                        }
                        else if (thicknesses[i].Contains("STIFFENED"))
                        {
                            int clone = 0;
                            thicknessIndexes.Add(clone);
                            thicknessIndexes.Add(clone);
                            thicknessIndexes.Add(clone);
                            i = i + 2;
                        }
                        else
                        {
                            int clone = int.Parse(thicknesses[i].Split(',')[0].Trim());
                            thicknessIndexes.Add(clone);
                        }
                    }

                    foreach (int index in indices)
                    {
                        if (thicknessIndexes.Contains(index))
                        {
                            int thicknessIndex = thicknessIndexes.IndexOf(index);
                            thicknesses[thicknessIndex] = "";
                        }
                    }

                    thicknesses = thicknesses.Where(x => !string.IsNullOrEmpty(x)).ToList();

                    WriteToANSI(path, thicknesses);
                }
            }
            return success;
        }

        /***************************************************/

    }
}

