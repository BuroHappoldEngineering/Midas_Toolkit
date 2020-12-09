﻿/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using System;
using System.Linq;
using BH.oM.Adapters.MidasCivil;
using BH.Engine.Adapter;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.IO;
using BH.oM.Structure.SectionProperties;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private bool CreateCollection(IEnumerable<BarDifferentialTemperatureLoad> BarDifferentialTemperatureLoads)
        {
            string loadGroupPath = CreateSectionFile("LOAD-GROUP");
            foreach (BarDifferentialTemperatureLoad barDifferentialTemperatureLoad in BarDifferentialTemperatureLoads)
            {
                if (barDifferentialTemperatureLoad.TemperatureProfile.Keys.Count > 21)
                {
                    Engine.Reflection.Compute.RecordWarning("There is a maximium of 20 layers in the temperature profile");
                    return true;
                }
                List<string> midasTemperatureLoads = new List<string>();
                string barLoadPath = CreateSectionFile(barDifferentialTemperatureLoad.Loadcase.Name + "\\BSTEMPER");
                string midasLoadGroup = Adapters.MidasCivil.Convert.FromLoadGroup(barDifferentialTemperatureLoad);
                var groupedBars = barDifferentialTemperatureLoad.Objects.Elements.GroupBy(x => x.SectionProperty);
                string ids = "";
                ISectionProperty sectionProperty = null;
                foreach (var barGroup in groupedBars)
                {
                    //This if function below is to separate the same load that has different section properties
                    if (sectionProperty == null) { }
                    else
                    {
                        if (sectionProperty.Asy == barGroup.First().SectionProperty.Asy && sectionProperty.Asz == barGroup.First().SectionProperty.Asz) { }
                        else
                        {
                            midasTemperatureLoads.AddRange(Adapters.MidasCivil.Convert.FromBarDifferentialTemperatureLoad(barDifferentialTemperatureLoad, ids, sectionProperty, m_temperatureUnit));
                            ids = "";
                        }
                    }
                    sectionProperty = barGroup.First().SectionProperty;
                    if (sectionProperty == null)
                    {
                        Engine.Reflection.Compute.RecordWarning("Section Property is required for inputting differential temperature load");
                        return true;
                    }
                    foreach (Bar bar in barGroup)
                    {
                        ids = ids + " " + (bar.AdapterId<string>(typeof(MidasCivilId)));
                    }
                    //this if function below is to add temperature load for the final load and for cases where there is only one bar
                    if (barGroup.Last().BHoM_Guid.ToString() == groupedBars.Last().Last().BHoM_Guid.ToString())
                    {
                        midasTemperatureLoads.AddRange(Adapters.MidasCivil.Convert.FromBarDifferentialTemperatureLoad(barDifferentialTemperatureLoad, ids, sectionProperty, m_temperatureUnit));
                    }
                }
                CompareLoadGroup(midasLoadGroup, loadGroupPath);
                RemoveEndOfDataString(barLoadPath);
                File.AppendAllLines(barLoadPath, midasTemperatureLoads);
            }
            return true;
        }

        /***************************************************/

    }
}
