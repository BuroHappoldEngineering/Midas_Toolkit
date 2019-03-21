﻿using System;
using System.Collections.Generic;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using System.Linq;
using System.IO;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter
    {
        private List<ILoad> ReadBarUniformlyDistributedLoads(List<string> ids = null)
        {
            List<ILoad> bhomBarUniformlyDistributedLoads = new List<ILoad>();

            // Read Loadcases
            List<Loadcase> bhomLoadcases = ReadLoadcases();
            Dictionary<string, Loadcase> loadcaseDictionary = bhomLoadcases.ToDictionary(
                        x => x.Name);

            string[] loadcaseFolders = Directory.GetDirectories(directory + "\\TextFiles");

            foreach (string loadcaseFolder in loadcaseFolders)
            {
                // Extract Beam UDL loads
                string loadcase = Path.GetFileName(loadcaseFolder);
                List<string> barUniformlyDistributedLoadText = GetSectionText(loadcase + "\\BEAMLOAD");

                if (barUniformlyDistributedLoadText.Contains("LINE"))
                {
                    Engine.Reflection.Compute.RecordWarning("The BHoM does not support line loads, only beams loads");
                }

                List<string> barUniformLoads = barUniformlyDistributedLoadText.Where(x => x.Contains("UNILOAD")).ToList();
                barUniformLoads.AddRange(barUniformlyDistributedLoadText.Where(x => x.Contains("UNIMOMENT")).ToList());

                // Filter the UDls to ensure the loads is spread over whole bar length and is constant in magnitude

                if (barUniformLoads.Count != 0)
                {
                    List<string> barComparison = new List<string>();
                    List<string> loadedBars = new List<string>();

                    foreach (string barUniformlyDistributedLoad in barUniformLoads)
                    {
                        List<string> delimitted = barUniformlyDistributedLoad.Split(',').ToList();

                        if (delimitted[11] == delimitted[13])
                        {
                            if (delimitted[10].Replace(" ", "") == 0.ToString() && delimitted[12].Replace(" ", "") == 1.ToString())
                            {
                                loadedBars.Add(delimitted[0].Replace(" ", ""));
                                delimitted.RemoveAt(0);
                                barComparison.Add(String.Join(",", delimitted));
                            }
                        }
                    }

                    List<List<Bar>> bhomLoadedBars = new List<List<Bar>>();

                    if (barComparison.Count != 0)
                    {
                        // Read Bars
                        List<Bar> bhomBars = ReadBars();
                        Dictionary<string, Bar> barDictionary = bhomBars.ToDictionary(
                                                                    x => x.CustomData[AdapterId].ToString());

                        // Find matcing bars from dictionary
                        List<List<string>> barIndices = new List<List<string>>();
                        List<string> distinctBarLoads = barComparison.Distinct().ToList();

                        foreach (string barList in distinctBarLoads)
                        {
                            List<int> indexMatches = barComparison.Select((barload, index) => new { barload, index })
                                                       .Where(x => string.Equals(x.barload, barList))
                                                       .Select(x => x.index)
                                                       .ToList();
                            List<string> matchingBars = new List<string>();
                            indexMatches.ForEach(x => matchingBars.Add(loadedBars[x]));
                            barIndices.Add(matchingBars);
                        }

                        // Convert to BHoM UDL

                        for (int i = 0; i < distinctBarLoads.Count; i++)
                        {
                            BarUniformlyDistributedLoad bhomBarUniformlyDistributedLoad = Engine.MidasCivil.Convert.ToBHoMBarUniformlyDistributedLoad(distinctBarLoads[i], barIndices[i], loadcase, loadcaseDictionary, barDictionary, i + 1);
                            bhomBarUniformlyDistributedLoads.Add(bhomBarUniformlyDistributedLoad);
                        }
                        
                    }
                }
            }
            return bhomBarUniformlyDistributedLoads;
        }

    }
}
