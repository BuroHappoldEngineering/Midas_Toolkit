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

using System.Text;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.IO;
using BH.Engine.Adapter;
using BH.oM.Adapters.MidasCivil;
using BH.Engine.Reflection;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter
    {
        public bool CreateCollection(IEnumerable<AreaDifferentialTemperatureLoad> areaDifferentialTemperatureLoads)
        {
            string loadGroupPath = CreateSectionFile("LOAD-GROUP");

            foreach (AreaDifferentialTemperatureLoad areaDifferentialTemperatureLoad in areaDifferentialTemperatureLoads)
            {
                StringBuilder midasTemperatureLoads = new StringBuilder();
                string feMeshLoadPath = CreateSectionFile(areaDifferentialTemperatureLoad.Loadcase.Name + "\\THERGRAD");
                string midasLoadGroup = Adapters.MidasCivil.Convert.FromLoadGroup(areaDifferentialTemperatureLoad);
                List<IAreaElement> assignedElements = areaDifferentialTemperatureLoad.Objects.Elements;
                List<string> assignedFEMeshes = new List<string>();
                AreaUniformTemperatureLoad areaUniformTemperatureLoad = new AreaUniformTemperatureLoad();
                areaUniformTemperatureLoad.TemperatureChange = (areaDifferentialTemperatureLoad.TemperatureProfile[0] - areaDifferentialTemperatureLoad.TemperatureProfile[1]) / 2 + areaDifferentialTemperatureLoad.TemperatureProfile[1];
                areaUniformTemperatureLoad.Name = areaDifferentialTemperatureLoad.Name;
                areaUniformTemperatureLoad.Objects = areaDifferentialTemperatureLoad.Objects;
                areaUniformTemperatureLoad.Loadcase = areaDifferentialTemperatureLoad.Loadcase;
                Compute.RecordWarning("Please note the AreaDifferentialTemperatureLoad is input to MidasCivil as a Temperature Gradient and an Element Temperature Load. \n Any AreaTemperatureLoads should be pushed in a separate Loadcase to avoid errors when pulling AreaDifferentialTemperatureLoads.");
                CreateCollection(new List<AreaUniformTemperatureLoad>() { areaUniformTemperatureLoad });
                foreach (FEMesh mesh in assignedElements)
                {
                    List<FEMeshFace> faces = mesh.Faces;
                    foreach (FEMeshFace face in faces)
                    {
                        assignedFEMeshes.Add(face.AdapterId<string>(typeof(MidasCivilId)));
                    }
                }
                foreach (string assignedFEMesh in assignedFEMeshes)
                {
                    midasTemperatureLoads.AppendLine(Adapters.MidasCivil.Convert.FromAreaDifferentialTemperatureLoad(areaDifferentialTemperatureLoad, assignedFEMesh, m_temperatureUnit));
                }
                CompareLoadGroup(midasLoadGroup, loadGroupPath);
                RemoveEndOfDataString(feMeshLoadPath);
                File.AppendAllText(feMeshLoadPath, midasTemperatureLoads.ToString(), Encoding.GetEncoding(1252));
            }
            return true;
        }
    }
}
