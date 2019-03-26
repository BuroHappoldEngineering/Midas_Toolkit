﻿using System;
using System.Linq;
using System.Collections.Generic;
using BH.oM.Structure.Loads;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;

namespace BH.Engine.MidasCivil
{
    public static partial class Convert
    {
        public static List<ILoad> CombineLoads(List<BarUniformlyDistributedLoad> loads, List<Bar> bars)
        {
            List<ILoad> resultant = new List<ILoad>();
            var groupedByLoadCase = loads.GroupBy(x => x.Loadcase);

            Dictionary<string, Bar> barDictionary = bars.ToDictionary(
                x => x.CustomData[AdapterId].ToString());

            foreach (var loadcaseGroup in groupedByLoadCase)
            {
                // Sort loads by Axis and Projection
                List<List<BarUniformlyDistributedLoad>> sortedLoads = new List<List<BarUniformlyDistributedLoad>> { new List<BarUniformlyDistributedLoad>(),
                                                                                                                new List<BarUniformlyDistributedLoad>(),
                                                                                                                new List<BarUniformlyDistributedLoad>(),
                                                                                                                new List<BarUniformlyDistributedLoad>()};

                foreach (BarUniformlyDistributedLoad load in loadcaseGroup)
                {
                    switch (load.Axis)
                    {
                        case (LoadAxis.Global):
                            if (load.Projected)
                                sortedLoads[0].Add(load);
                            else
                                sortedLoads[1].Add(load);
                            break;

                        case (LoadAxis.Local):
                            if (load.Projected)
                                sortedLoads[2].Add(load);
                            else
                                sortedLoads[3].Add(load);
                            break;
                    }
                }

                // Sort through each of the sorted Loads

                foreach (var sortedLoad in sortedLoads)
                {
                    List<List<string>> loadBars = new List<List<string>>();

                    // Extract associated bar numbers
                    foreach (var load in sortedLoad)
                    {
                        List<string> loadBar = new List<string>();

                        foreach (var element in load.Objects.Elements)
                        {
                            loadBar.Add(element.CustomData[AdapterId].ToString());
                        }

                        loadBars.Add(loadBar);
                    }

                    List<string> hitBars = new List<string>();
                    List<double[]> vectors = new List<double[]>();

                    // Sum forces and moments for each bar associated with the sorted load
                    for (int i = 0; i < loadBars.Count; i++)
                    {
                        foreach (string bar in loadBars[i])
                        {
                            if (!hitBars.Contains(bar))
                            {
                                hitBars.Add(bar);
                                double[] vector = { sortedLoad[i].Force.X, sortedLoad[i].Force.Y, sortedLoad[i].Force.Z,
                                            sortedLoad[i].Moment.X, sortedLoad[i].Moment.Y, sortedLoad[i].Moment.Z};
                                vectors.Add(vector);
                            }
                            else
                            {
                                int index = hitBars.FindIndex(x => x == bar);

                                vectors[index][0] = vectors[index][0] + sortedLoad[i].Force.X;
                                vectors[index][1] = vectors[index][1] + sortedLoad[i].Force.Y;
                                vectors[index][2] = vectors[index][2] + sortedLoad[i].Force.Z;
                                vectors[index][3] = vectors[index][3] + sortedLoad[i].Moment.X;
                                vectors[index][4] = vectors[index][4] + sortedLoad[i].Moment.Y;
                                vectors[index][5] = vectors[index][5] + sortedLoad[i].Moment.Z;
                            }
                        }
                    }

                    // Get distinct resultant loads for each bar and extract all bars that have that distinct load
                    List<double[]> distinctLoads = vectors.Distinct(new ArrayComparer()).ToList();
                    List<List<string>> matchingBars = new List<List<string>>();

                    foreach (double[] distinctLoad in distinctLoads)
                    {
                        ArrayComparer comparer = new ArrayComparer();

                        var distinctMatches = vectors.Select((v, i) => new { v, i })
                            .Where(x => comparer.Equals(x.v, distinctLoad));

                        List<string> match = new List<string>();

                        foreach (var bar in distinctMatches)
                        {
                            match.Add(hitBars[bar.i]);
                        }

                        matchingBars.Add(match);
                    }

                    // Get corresponding BhoM bar and create resultant load

                    for (int i = 0; i < distinctLoads.Count; i++)
                    {
                        List<Bar> matchingBhomBars = new List<Bar>();

                        foreach (string bar in matchingBars[i])
                        {
                            Bar bhomBar;
                            barDictionary.TryGetValue(bar, out bhomBar);
                            matchingBhomBars.Add(bhomBar);
                        }

                        Vector forceVector = new Vector { X = distinctLoads[i][0], Y = distinctLoads[i][1], Z = distinctLoads[i][2] };
                        Vector momentVector = new Vector { X = distinctLoads[i][3], Y = distinctLoads[i][4], Z = distinctLoads[i][5] };

                        resultant.Add(Engine.Structure.Create.BarUniformlyDistributedLoad(loadcaseGroup.First().Loadcase, matchingBhomBars, forceVector, momentVector, sortedLoad.First().Axis, sortedLoad.First().Projected, sortedLoad.First().Name));
                    }
                }
            }

            return resultant;
        }

        private class ArrayComparer : IEqualityComparer<double[]>
        {
            public bool Equals(double[] x, double[] y)
            {
                if (x.Length != y.Length)
                    return false;

                for (int i = 0; i < x.Length; ++i)
                    if (!x[i].Equals(y[i]))
                        return false;

                return true;
            }

            public int GetHashCode(double[] obj)
            {
                return string.Join("", obj).GetHashCode();
            }
        }
    }
}
