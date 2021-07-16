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
using System.ComponentModel;
using BH.oM.Adapters.MidasCivil;
using System.Globalization;
using BH.oM.Reflection.Attributes;


namespace BH.Engine.Adapters.MidasCivil
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Metadata object from a variety of inputs.")]
        [Input("location", "Where the project is based.")]
        [Input("description", "A short description of the project and model.")]
        [Input("discipline", "The discipline responsible for the model.")]
        [Input("creationDate", "The creation date of the model inputted as yyyy-MM-dd. This will default to the current date if no date is provided.")]
        [Input("reviews", "A list of reviews containing reviewers, their comments and the date of review.")]
        [Output("A summary of relevant information for the model.")]
        public static Metadata Metadata(string projectNumber = "", string projectName = "", string location = "", string client = "", 
            string designStage = "", string projectLead = "", string revision = "", string author = "", DateTime? creationDate = null, string email = "", 
            string description = "", string discipline = "", List<Review> reviews = null)
        {
            Metadata data = new Metadata();
            data.ProjectNumber = projectNumber;
            data.ProjectName = projectName;
            data.Location = location;
            data.Client = client;
            data.DesignStage = designStage;
            data.ProjectLead = projectLead;
            data.Revision = revision;

            data.Author = author;
            data.Email = email;

            data.Description = description;
            data.Discipline = discipline;

            data.Reviews = reviews;

            if (creationDate == null)
            {
                data.CreationDate = DateTime.Now;
            }
            else
            {
                data.CreationDate = (DateTime)creationDate;
            }

            return data;
        }

    }
}
