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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using BH.Engine.Base.Objects;
using BH.oM.Adapters.MidasCivil;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Loads;
using BH.Engine.Adapters.MidasCivil.Comparer;
using BH.Engine.Structure;
using BH.oM.Adapter.Commands;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter : BHoMAdapter
    {

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        //Add any applicable constructors here, such as linking to a specific file or anything else as well as linking to that file through the (if existing) com link via the API
        public MidasCivilAdapter(string filePath, bool active = false, string version = "")
        {
            if (active)
            {
                AdapterIdFragmentType = typeof(MidasCivilId);

                Modules.Structure.ModuleLoader.LoadModules(this);

                AdapterComparers = new Dictionary<Type, object>
                {
                    {typeof(Node), new NodeDistanceComparer(3) },   //The 3 in here sets how many decimal places to look at for node merging. 3 decimal places gives mm precision
                    {typeof(Bar), new BarEndNodesDistanceComparer(3) },
                    {typeof(FEMesh), new MeshCentreComparer() },
                    {typeof(Constraint6DOF), new NameOrDescriptionComparer() },
                    {typeof(RigidLink), new NameOrDescriptionComparer() },
                    {typeof(BarRelease), new NameOrDescriptionComparer() },
                    {typeof(SteelSection), new NameOrDescriptionComparer() },
                    {typeof(ISectionProperty), new NameOrDescriptionComparer() },
                    {typeof(Steel), new NameOrDescriptionComparer() },
                    {typeof(Concrete), new NameOrDescriptionComparer() },
                    {typeof(IMaterialFragment), new NameOrDescriptionComparer() },
                    {typeof(LinkConstraint), new NameOrDescriptionComparer() },
                    {typeof(ConstantThickness), new NameOrDescriptionComparer() },
                    {typeof(ISurfaceProperty), new NameOrDescriptionComparer() },
                    {typeof(Loadcase), new BHoMObjectNameComparer() },
                    {typeof(PointLoad), new BHoMObjectNameComparer() },
                    {typeof(GravityLoad), new BHoMObjectNameComparer() },
                    {typeof(BarUniformlyDistributedLoad), new BHoMObjectNameComparer() },
                    {typeof(BarVaryingDistributedLoad), new BHoMObjectNameComparer() },
                    {typeof(BarDifferentialTemperatureLoad), new BHoMObjectNameComparer() },
                    {typeof(BarPointLoad), new BHoMObjectNameComparer() },
                    {typeof(AreaUniformlyDistributedLoad), new BHoMObjectNameComparer() },
                    {typeof(AreaUniformTemperatureLoad), new BHoMObjectNameComparer() },
                    {typeof(BarUniformTemperatureLoad), new BHoMObjectNameComparer() },
                    {typeof(LoadCombination), new BHoMObjectNameComparer() },
                };

                DependencyTypes = new Dictionary<Type, List<Type>>
                {
                    {typeof(Node), new List<Type> { typeof(Constraint6DOF)} },
                    {typeof(Bar), new List<Type> { typeof(Node) , typeof(ISectionProperty), typeof(BarRelease) } },
                    {typeof(FEMesh), new List<Type> { typeof(Node), typeof(ISurfaceProperty) } },
                    {typeof(ISectionProperty), new List<Type> { typeof(IMaterialFragment) } },
                    {typeof(RigidLink), new List<Type> { typeof(Node) } },
                    {typeof(ISurfaceProperty), new List<Type> { typeof(IMaterialFragment) } },
                    {typeof(ILoad), new List<Type> {typeof(Loadcase) } }
                };

                m_midasCivilVersion = version;
                Execute(new Open() { FileName = filePath });
            }
        }

        public static bool IsApplicationRunning()
        {
            return (Process.GetProcessesByName("CVlw").Length > 0) ? true : false;
        }

        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        private List<string> m_midasText;
        private string m_directory;
        public string m_midasCivilVersion { get; protected set; }
        private string m_forceUnit = "N";
        private string m_lengthUnit = "M";
        private string m_heatUnit = "KJ";
        private string m_temperatureUnit = "C";
        private readonly int m_groupCharacterLimit = 80;
        private readonly int m_sectionPropertyCharacterLimit = 28;
        private readonly int m_materialCharacterLimit = 15;
        private Dictionary<Type, Dictionary<int, HashSet<string>>> m_tags = new Dictionary<Type, Dictionary<int, HashSet<string>>>();

        /***************************************************/
    }
}

