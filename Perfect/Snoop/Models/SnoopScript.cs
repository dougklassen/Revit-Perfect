using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DougKlassen.Revit.Snoop.Models
{
    /// <summary>
    /// A script that is loaded and run by the task engine
    /// </summary>
    public class SnoopScript
    {
        public SnoopScript()
        {
            TaskList = new List<Tuple<SnoopProject, List<Int32>>>();
        }

        /// <summary>
        /// A collection of projects with the task list indicies of the tasks to be run on them
        /// </summary>
        public List<Tuple<SnoopProject, List<Int32>>> TaskList
        {
            get;
            set;
        }

        /// <summary>
        /// Add the entire tasklist for the specified project to a script
        /// </summary>
        /// <param name="project">The project containing the task list to be added</param>
        public void AddProject(SnoopProject project)
        {
            List<Int32> indicies = new List<Int32>();
            for (int i = 0; i < project.TaskList.Count; i++)
            {
                indicies.Add(i);
            }

            TaskList.Add(new Tuple<SnoopProject, List<Int32>>(project, indicies));
        }
    }
}
