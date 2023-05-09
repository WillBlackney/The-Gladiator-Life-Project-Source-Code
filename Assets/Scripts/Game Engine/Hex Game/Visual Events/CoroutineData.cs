using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.VisualEvents
{
    public class TaskTracker
    {
        private bool complete;
        public bool Complete()
        {
            return complete;
        }

        public void MarkAsCompleted()
        {
            complete = true;
        }


    }
}