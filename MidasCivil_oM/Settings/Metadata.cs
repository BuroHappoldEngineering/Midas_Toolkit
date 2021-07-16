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

using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.oM.Adapters.MidasCivil
{

    public class Metadata : BHoMObject
    {
        [Description("Defines the project number of the project.")]
        public virtual string ProjectNumber { get; set; }

        [Description("Defines the name of the project.")]
        public virtual string ProjectName { get; set; }

        [Description("Defines the location of the project.")]
        public virtual string Location { get; set; }

        [Description("Defines the client of the project.")]
        public virtual string Client { get; set; }

        [Description("Defines the design stage of the project.")]
        public virtual string DesignStage { get; set; }

        [Description("Defines the project lead for the project.")]
        public virtual string ProjectLead { get; set; }

        [Description("Defines the revision number of the model.")]
        public virtual string Revision { get; set; }

        [Description("Defines the author of the model.")]
        public virtual string Author { get; set; }

        [Description("Defines the date the model was created on.")]
        public virtual DateTime CreationDate { get; set; }

        [Description("Defines the project email address.")]
        public virtual string Email { get; set; }

        [Description("A short description of the project and model.")]
        public virtual string Description { get; set; }

        [Description("The discipline responsible for the model.")]
        public virtual string Discipline { get; set; }

        [Description("A list of reviews containing reviewers, their comments and the date of review.")]
        public virtual List<Review> Reviews { get; set; }

    }
}

