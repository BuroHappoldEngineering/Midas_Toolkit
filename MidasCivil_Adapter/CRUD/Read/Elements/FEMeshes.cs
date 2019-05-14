﻿using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SurfaceProperties;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter
    {
        private List<FEMesh> ReadFEMeshes(List<string> ids = null)
        {
            List<FEMesh> bhomMeshes = new List<FEMesh>();

            List<string> elementsText = GetSectionText("ELEMENT");
            List<string> meshText = elementsText.Where(x => x.Contains("PLATE")).ToList();
            Dictionary<string, List<int>> elementGroups = ReadTags("GROUP", 2);

            IEnumerable<Node> bhomNodesList = ReadNodes();
            Dictionary<string, Node> bhomNodes = bhomNodesList.ToDictionary(
                x => x.CustomData[AdapterId].ToString());

            IEnumerable<ISurfaceProperty> bhomSurfacePropertiesList = ReadSurfaceProperties();
            Dictionary<string, ISurfaceProperty> bhomSuraceProperties = bhomSurfacePropertiesList.ToDictionary(
                x => x.CustomData[AdapterId].ToString());

            foreach (string mesh in meshText)
            {
                FEMesh bhomMesh = Engine.MidasCivil.Convert.ToBHoMFEMesh(mesh, bhomNodes,bhomSuraceProperties);
                int bhomID = System.Convert.ToInt32(bhomMesh.CustomData[AdapterId]);
                bhomMesh.Tags = GetGroupAssignments(elementGroups, bhomID);
                bhomMeshes.Add(bhomMesh);
            }

            return bhomMeshes;
        }

    }
}
