using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TbsFramework.Units;

namespace HexGameEngine
{
    public class PlayerUnit : Unit
    {
        public Color LeadingColour;
        public Renderer myRenderer;

        public override void Initialize()
        {
            base.Initialize();
            transform.localPosition -= new Vector3(0, 0, 1);
            myRenderer.material.color = LeadingColour;
        }

        public override void MarkAsAttacking(Unit target)
        {
        }

        public override void MarkAsDefending(Unit aggressor)
        {
        }

        public override void MarkAsDestroyed()
        {
        }

        public override void MarkAsFinished()
        {

        }

        public override void MarkAsFriendly()
        {
            myRenderer.material.color = LeadingColour += new Color(0.8f, 1, 0.8f);
        }

        public override void MarkAsReachableEnemy()
        {
            myRenderer.material.color = LeadingColour += Color.red;
        }

        public override void MarkAsSelected()
        {
            myRenderer.material.color = LeadingColour += Color.green;
        }

        public override void UnMark()
        {
            myRenderer.material.color = LeadingColour;
        }
    }
}