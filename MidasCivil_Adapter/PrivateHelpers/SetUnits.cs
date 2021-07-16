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

using System.IO;
using System;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private bool SetUnits(string length = "M", string force = "N", string temperature = "C", string heat = "KJ")
        {
                string path = CreateSectionFile("UNIT");

                string units = "*UNIT\n";

                string[] lengths = { "M", "CM", "MM", "FT", "IN" };
                string[] forces = { "N", "KN", "KGF", "TONF", "LBF", "KIPS" };
                string[] temperatures = { "C", "F" };
                string[] heats = { "CAL", "KCAL", "J", "KJ", "BTU" };

                if (Array.Exists(forces, element => element == force))
                {
                    units += force + ", ";
                }
                else
                {
                    Engine.Reflection.Compute.RecordWarning($"Unit input {force} not recognised using Newtons (N) instead.");
                    units += "N" + ", ";
                }

                if (Array.Exists(lengths, element => element == length))
                {
                    units += length + ", ";
                }
                else
                {
                    Engine.Reflection.Compute.RecordWarning($"Unit input {length} not recognised using meters (m) instead.");
                    units += "M" + ", ";
                }

                if (Array.Exists(heats, element => element == heat))
                {
                    units += heat + ", ";
                }
                else
                {
                    Engine.Reflection.Compute.RecordWarning($"Unit input {heat} not recognised using kilojoules (kJ) instead.");
                    units += "KJ" + ", ";
                }
                if (Array.Exists(temperatures, element => element == temperature))
                {
                    units += temperature;
                }
                else
                {
                    Engine.Reflection.Compute.RecordWarning($"Unit input {temperature} not recognised using celcius (C) instead.");
                    units += "C";
                }
                units += "\n";
                File.WriteAllText(path, units);

            return true;
        }        
        /***************************************************/

    }
}
