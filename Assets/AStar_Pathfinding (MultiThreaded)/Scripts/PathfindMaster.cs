using UnityEngine;
using System.Collections;
using NDR2ndTTB;
using System.Threading;
using System.Collections.Generic;

namespace NDRPathfinder
{
    public class PathfindMaster : MonoBehaviour
    {
        #region Singleton
        public static PathfindMaster instance;
        private void Awake()
        {
            if (!instance) instance = this;
            else Destroy(this);
        }
        #endregion

        public int maxJobs;

        public delegate void PathfindingJobComplete(List<Node> node);

        private List<Pathfinder> currentJobs;
        private List<Pathfinder> toDoJobs;

        private void Start()
        {
            currentJobs = new List<Pathfinder>();
            toDoJobs = new List<Pathfinder>();
        }

        private void Update()
        {
            int i = 0;

            // check for completed jobs...
            while (i < currentJobs.Count)
            {
                if(currentJobs[i].jobDone)
                {
                    currentJobs[i].NotifyComplete();
                    currentJobs.RemoveAt(i);
                    // Job well done.
                }
                else
                {
                    i++;
                }
            }

            if(toDoJobs.Count > 0 && currentJobs.Count < maxJobs)
            {
                Pathfinder job = toDoJobs[0];
                toDoJobs.RemoveAt(0);
                currentJobs.Add(job);

                //start new thread.

                Thread jobThread = new Thread(job.FindPath);

                // since we have garbage collector in C#, we're not borry about anything.there is no need to keep reference 
                // of thread ,no need to close it.Thanks to the .NET :)
                jobThread.Start();

                // FYI : 
                // doc : 
                //https://docs.microsoft.com/en-us/dotnet/api/system.threading.thread?view=net-5.0
                //the thread continues to exectue until the thread procedure is complete.

            }
        }

        public void RequestPathfind(Node start, Node target, PathfindingJobComplete completeCallback)
        {
            Pathfinder newJob = new Pathfinder(start, target, completeCallback);
            toDoJobs.Add(newJob);
        }

    }
}