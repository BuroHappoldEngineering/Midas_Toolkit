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

using BH.oM.Adapters.MidasCivil;
using BH.Engine.Adapter;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter
    {
        public bool CreateCollection(IEnumerable<AreaUniformlyDistributedLoad> areaUniformlyDistributedLoads)
        {
            string loadGroupPath = CreateSectionFile("LOAD-GROUP");

            foreach (AreaUniformlyDistributedLoad areaUniformlyDistributedLoad in areaUniformlyDistributedLoads)
            {
                List<string> midasPressureLoads = new List<string>();
                string FEMeshLoadPath = CreateSectionFile(areaUniformlyDistributedLoad.Loadcase.Name + "\\PRESSURE");
                string midasLoadGroup = Adapters.MidasCivil.Convert.FromLoadGroup(areaUniformlyDistributedLoad);

                List<IAreaElement> assignedElements = areaUniformlyDistributedLoad.Objects.Elements;

                List<string> assignedFEMeshes = new List<string>();

                foreach (FEMesh mesh in assignedElements)
                {
                    List<FEMeshFace> faces = mesh.Faces;
                    foreach (FEMeshFace face in faces)
                    {
                        assignedFEMeshes.Add(face.AdapterId<string>(typeof(MidasCivilId)));
                    }
                }

                List<double> loadVectors = new List<double> { areaUniformlyDistributedLoad.Pressure.X,
                                                              areaUniformlyDistributedLoad.Pressure.Y,
                                                              areaUniformlyDistributedLoad.Pressure.Z};

                Vector zeroVector = new Vector { X = 0, Y = 0, Z = 0 };

                for (int i = 0; i < 3; i++)
                {
                    areaUniformlyDistributedLoad.Pressure = zeroVector;

                    if (loadVectors[i] != 0)
                    {
                        areaUniformlyDistributedLoad.Pressure = CreateSingleComponentVector(i, loadVectors[i]);

                        foreach (string assignedFEMesh in assignedFEMeshes)
                        {
                            midasPressureLoads.Add(Adapters.MidasCivil.Convert.FromAreaUniformlyDistributedLoad(areaUniformlyDistributedLoad, assignedFEMesh, m_forceUnit, m_lengthUnit));
                        }
                    }
                }

                CompareLoadGroup(midasLoadGroup, loadGroupPath);
                RemoveEndOfDataString(FEMeshLoadPath);
                File.AppendAllLines(FEMeshLoadPath, midasPressureLoads, Encoding.GetEncoding(1252));
            }

            return true;
        }

    }
}

