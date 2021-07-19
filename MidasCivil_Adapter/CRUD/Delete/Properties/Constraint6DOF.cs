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

        private int DeleteConstraints(IEnumerable<object> ids)
        {
            int success = 1;

            if (ids.Count() != 0)
            {
                string constraintPath = m_directory + "\\TextFiles\\" + "CONSTRAINT" + ".txt";
                string springPath = m_directory + "\\TextFiles\\" + "SPRING" + ".txt";
                List<string> paths = new List<string> { constraintPath, springPath };

                foreach (string path in paths)
                {
                    List<string> names = ids.Cast<string>().ToList();
                    List<string> constraints = File.ReadAllLines(path, Encoding.GetEncoding(1252)).ToList();
                    List<string> constraintNames = new List<string>();

                    if (File.Exists(path))
                    {
                        foreach (string constraint in constraints)
                        {
                            if (constraint.Contains("*") || constraint.Contains(";") || string.IsNullOrWhiteSpace(constraint))
                            {
                                string clone = 0.ToString();
                                constraintNames.Add(clone);
                            }
                            else
                            {
                                if (path.Contains("CONSTRAINT"))
                                    constraintNames.Add(constraint.Split(',').Reverse().First().Trim());
                                else
                                    constraintNames.Add(constraint.Split(',')[15].Trim());
                            }
                        }

                        foreach (string name in names)
                        {
                            if (constraintNames.Contains(name))
                            {
                                int constraintIndex = constraintNames.IndexOf(name);
                                constraints[constraintIndex] = "";
                            }
                        }

                        constraints = constraints.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                        WriteToANSI(path, constraints);
                    }
                }
            }
            return success;
        }

        /***************************************************/

    }
}

