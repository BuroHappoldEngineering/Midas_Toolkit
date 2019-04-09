﻿using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Elements;
using System.IO;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter
    {
        private List<ILoad> ReadPointForces(List<string> ids = null)
        {
            List<ILoad> bhomPointForces = new List<ILoad>();
            List<Loadcase> bhomLoadcases = ReadLoadcases();
            Dictionary<string, Loadcase> loadcaseDictionary = bhomLoadcases.ToDictionary(
                        x => x.Name);

            string[] loadcaseFolders = Directory.GetDirectories(directory + "\\TextFiles");

            int i = 1;

            foreach (string loadcaseFolder in loadcaseFolders)
            {
                string loadcase = Path.GetFileName(loadcaseFolder);
                List<string> pointForceText = GetSectionText(loadcase + "\\CONLOAD");

                if (pointForceText.Count != 0)
                {
                    List<Node> bhomNodes = ReadNodes();
                    Dictionary<string, Node> nodeDictionary = bhomNodes.ToDictionary(
                                                                x => x.CustomData[AdapterId].ToString());

                    List<string> pointForceComparison = new List<string>();
                    List<string> pointForceNodes = new List<string>();

                    foreach (string pointForce in pointForceText)
                    {
                        List<string> delimitted = pointForce.Split(',').ToList();
                        pointForceNodes.Add(delimitted[0].Replace(" ", ""));
                        delimitted.RemoveAt(0);
                        pointForceComparison.Add(String.Join(",", delimitted));
                    }

                    List<string> distinctPointForces = pointForceComparison.Distinct().ToList();

                    foreach (string distinctPointForce in distinctPointForces)
                    {
                        List<int> indexMatches = pointForceComparison.Select((pointload, index) => new { pointload, index })
                                                   .Where(x => string.Equals(x.pointload, distinctPointForce))
                                                   .Select(x => x.index)
                                                   .ToList();
                        List<string> matchingNodes = new List<string>();
                        indexMatches.ForEach(x => matchingNodes.Add(pointForceNodes[x]));
                        PointForce bhomPointForce = Engine.MidasCivil.Convert.ToBHoMPointForce(distinctPointForce, matchingNodes, loadcase, loadcaseDictionary, nodeDictionary, i);
                        bhomPointForces.Add(bhomPointForce);

                        if (String.IsNullOrWhiteSpace(distinctPointForce.Split(',').ToList()[6]))
                        {
                            i = i + 1;
                        }

                    }
                }
            }

            return bhomPointForces;
        }

    }
}