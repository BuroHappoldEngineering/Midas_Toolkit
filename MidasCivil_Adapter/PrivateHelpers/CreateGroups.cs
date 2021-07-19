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
using System.Linq;
using System.IO;
using System.Text;
using BH.oM.Adapters.MidasCivil;
using BH.Engine.Adapter;
using BH.oM.Structure.Elements;

namespace BH.Adapter.MidasCivil
{
    public partial class MidasCivilAdapter
    {
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private void CreateGroups(IEnumerable<Node> nodes)
        {
            Dictionary<string, List<int>> existingNodeGroups = GetTags("GROUP", 1);
            Dictionary<string, List<int>> existingElementGroups = GetTags("GROUP", 2);
            Dictionary<string, string> groupsToAdd = new Dictionary<string, string>();
            Dictionary<string, string> existingStringElementGroups = new Dictionary<string, string>();
            List<string> groups = existingNodeGroups.Keys.ToList();

            if (groups.Count != 0)
            {
                List<string> keys = existingNodeGroups.Keys.ToList();

                foreach (string key in keys)
                {
                    List<int> nodeValues = new List<int>();
                    existingNodeGroups.TryGetValue(key, out nodeValues);
                    List<int> elementValues = new List<int>();
                    existingElementGroups.TryGetValue(key, out elementValues);
                    string nodeStringValue = "";
                    string elementStringValue = "";

                    foreach (int value in nodeValues)
                    {
                        nodeStringValue = nodeStringValue + " " + value.ToString();
                    }

                    foreach (int value in elementValues)
                    {
                        elementStringValue = elementStringValue + " " + value.ToString();
                    }

                    groupsToAdd.Add(key, nodeStringValue);
                    existingStringElementGroups.Add(key, elementStringValue);
                }
            }

            foreach (Node node in nodes)
            {
                foreach (string tag in node.Tags)
                {
                    if (!groupsToAdd.ContainsKey(tag))
                    {
                        groupsToAdd.Add(tag, node.AdapterId<string>(typeof(MidasCivilId)));
                    }
                    else
                    {
                        string assignedNodes;
                        groupsToAdd.TryGetValue(tag, out assignedNodes);
                        string bhomID = node.AdapterId<string>(typeof(MidasCivilId));

                        if (!assignedNodes.Contains(bhomID))
                        {
                            assignedNodes = assignedNodes + " " + bhomID;
                        }

                        groupsToAdd[tag] = assignedNodes;
                    }
                }
            }

            List<string> totalKeys = groupsToAdd.Keys.ToList();
            File.Delete(m_directory + "\\TextFiles\\GROUP.txt");
            string path = CreateSectionFile("GROUP");


            using (StreamWriter sw = new StreamWriter(path, true, Encoding.GetEncoding(1252), 65536))
            {
                foreach (string key in totalKeys)
                {
                    string nodeList;
                    groupsToAdd.TryGetValue(key, out nodeList);
                    string elementList;
                    existingStringElementGroups.TryGetValue(key, out elementList);
                    string line = key + "," + nodeList + "," + elementList + ",0";
                    sw.WriteLine(line);
                }
                sw.Flush();
                sw.Close();
            }
        }

        /***************************************************/

        private void CreateGroups(IEnumerable<Bar> bars)
        {
            Dictionary<string, List<int>> existingNodeGroups = GetTags("GROUP", 1);
            Dictionary<string, List<int>> existingElementGroups = GetTags("GROUP", 2);
            Dictionary<string, string> groupsToAdd = new Dictionary<string, string>();
            Dictionary<string, string> existingStringNodeGroups = new Dictionary<string, string>();
            List<string> groups = existingNodeGroups.Keys.ToList();

            if (groups.Count != 0)
            {
                List<string> keys = existingNodeGroups.Keys.ToList();

                foreach (string key in keys)
                {
                    List<int> nodeValues = new List<int>();
                    existingNodeGroups.TryGetValue(key, out nodeValues);
                    List<int> elementValues = new List<int>();
                    existingElementGroups.TryGetValue(key, out elementValues);
                    string nodeStringValue = "";
                    string elementStringValue = "";

                    foreach (int value in nodeValues)
                    {
                        nodeStringValue = nodeStringValue + " " + value.ToString();
                    }

                    foreach (int value in elementValues)
                    {
                        elementStringValue = elementStringValue + " " + value.ToString();
                    }

                    groupsToAdd.Add(key, elementStringValue);
                    existingStringNodeGroups.Add(key, nodeStringValue);
                }
            }

            foreach (Bar bar in bars)
            {
                foreach (string tag in bar.Tags)
                {
                    if (!groupsToAdd.ContainsKey(tag))
                    {
                        groupsToAdd.Add(tag, bar.AdapterId<string>(typeof(MidasCivilId)));
                    }
                    else
                    {
                        string assignedBars;
                        groupsToAdd.TryGetValue(tag, out assignedBars);
                        string bhomID = bar.AdapterId<string>(typeof(MidasCivilId));

                        if (!assignedBars.Contains(bhomID))
                        {
                            assignedBars = assignedBars + " " + bhomID;
                        }

                        groupsToAdd[tag] = assignedBars;
                    }
                }
            }

            List<string> totalKeys = groupsToAdd.Keys.ToList();
            File.Delete(m_directory + "\\TextFiles\\GROUP.txt");
            string path = CreateSectionFile("GROUP");


            using (StreamWriter sw = new StreamWriter(path, true, Encoding.GetEncoding(1252), 65536))
            {
                foreach (string key in totalKeys)
                {
                    string nodeList;
                    existingStringNodeGroups.TryGetValue(key, out nodeList);
                    string elementList;
                    groupsToAdd.TryGetValue(key, out elementList);
                    string line = key + "," + nodeList + "," + elementList + ",0";
                    sw.WriteLine(line);
                }
                sw.Flush();
                sw.Close();
            }
        }

        private void CreateGroups(IEnumerable<FEMesh> meshes)
        {
            Dictionary<string, List<int>> existingNodeGroups = GetTags("GROUP", 1);
            Dictionary<string, List<int>> existingElementGroups = GetTags("GROUP", 2);
            Dictionary<string, string> groupsToAdd = new Dictionary<string, string>();
            Dictionary<string, string> existingStringNodeGroups = new Dictionary<string, string>();
            List<string> groups = existingNodeGroups.Keys.ToList();

            if (groups.Count != 0)
            {
                List<string> keys = existingNodeGroups.Keys.ToList();

                foreach (string key in keys)
                {
                    List<int> nodeValues = new List<int>();
                    existingNodeGroups.TryGetValue(key, out nodeValues);
                    List<int> elementValues = new List<int>();
                    existingElementGroups.TryGetValue(key, out elementValues);
                    string nodeStringValue = "";
                    string elementStringValue = "";

                    foreach (int value in nodeValues)
                    {
                        nodeStringValue = nodeStringValue + " " + value.ToString();
                    }

                    foreach (int value in elementValues)
                    {
                        elementStringValue = elementStringValue + " " + value.ToString();
                    }

                    groupsToAdd.Add(key, elementStringValue);
                    existingStringNodeGroups.Add(key, nodeStringValue);
                }
            }

            foreach (FEMesh mesh in meshes)
            {
                 foreach (string tag in mesh.Tags)
                {
                    if (!groupsToAdd.ContainsKey(tag))
                    {
                        StringBuilder meshTags = new StringBuilder(mesh.AdapterId<string>(typeof(MidasCivilId)));
                        foreach(FEMeshFace feMeshFace in mesh.Faces)
                        {
                            meshTags.Append(" " + feMeshFace.AdapterId<string>(typeof(MidasCivilId)));
                        }

                        groupsToAdd.Add(tag, meshTags.ToString());
                    }
                    else
                    {
                        string assignedFEMesh;
                        groupsToAdd.TryGetValue(tag, out assignedFEMesh);
                        string meshId = mesh.AdapterId<string>(typeof(MidasCivilId));

                        StringBuilder assignedIds = new StringBuilder(assignedFEMesh);

                        if (!assignedFEMesh.Contains(meshId))
                        {
                            assignedIds.Append(" " + meshId);
                        }

                        foreach(FEMeshFace feMeshFace in mesh.Faces)
                        {
                            string faceId = feMeshFace.AdapterId<string>(typeof(MidasCivilId));
                            if (!assignedFEMesh.Contains(faceId))
                            {
                                assignedIds.Append(" " + faceId);
                            }
                        }

                        groupsToAdd[tag] = assignedIds.ToString();;
                    }
                }

            }

            List<string> totalKeys = groupsToAdd.Keys.ToList();
            File.Delete(m_directory + "\\TextFiles\\GROUP.txt");
            string path = CreateSectionFile("GROUP");


            using (StreamWriter sw = new StreamWriter(path, true, Encoding.GetEncoding(1252), 65536))
            {
                foreach (string key in totalKeys)
                {
                    string nodeList;
                    existingStringNodeGroups.TryGetValue(key, out nodeList);
                    string elementList;
                    groupsToAdd.TryGetValue(key, out elementList);
                    string line = key + "," + nodeList + "," + elementList + ",0";
                    sw.WriteLine(line);
                }
                sw.Flush();
                sw.Close();
            }
        }
    }
}

