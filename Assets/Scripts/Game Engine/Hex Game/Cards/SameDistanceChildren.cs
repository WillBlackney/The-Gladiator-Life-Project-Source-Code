﻿using UnityEngine;

namespace WeAreGladiators.Cards
{
    public class SameDistanceChildren : MonoBehaviour
    {

        public Transform[] Children;

        // Use this for initialization
        private void Awake()
        {
            Vector3 firstElementPos = Children[0].transform.position;
            Vector3 lastElementPos = Children[Children.Length - 1].transform.position;

            // dividing by Children.Length - 1 because for example: between 10 points that are 9 segments
            float XDist = (lastElementPos.x - firstElementPos.x) / (Children.Length - 1);
            float YDist = (lastElementPos.y - firstElementPos.y) / (Children.Length - 1);
            float ZDist = (lastElementPos.z - firstElementPos.z) / (Children.Length - 1);

            Vector3 Dist = new Vector3(XDist, YDist, ZDist);

            for (int i = 1; i < Children.Length; i++)
            {
                Children[i].transform.position = Children[i - 1].transform.position + Dist;
            }
        }
    }
}
