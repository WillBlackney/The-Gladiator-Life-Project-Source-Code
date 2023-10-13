using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.UCM;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.TownFeatures
{
    public class TownCharacterView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private UniversalCharacterModel ucm;

        private List<TownMovementNode> currentPath = new List<TownMovementNode>();
        private const float WALK_SPEED = 0.25F;

        public void Initialize(TownCharacterSeed seed, TownPath path)
        {
            // Setup path
            currentPath.Clear();
            currentPath.AddRange(path.Nodes);

            // Setup model
            CharacterModeller.BuildModelFromStringReferences(ucm, seed.GetRandomAppearance());
            ucm.SetWalkAnim();

            // Place at at start
            TownMovementNode start = path.Nodes[0];
            transform.DOMove(start.transform.position, 0f);
            currentPath.RemoveAt(0);

            // Start move
            MoveToNode(currentPath[0]);
        }

        private void OnNodeReached(TownMovementNode node)
        {
            currentPath.Remove(node);
            // to do apply effects of node
            if(currentPath.Count == 0 )
            {
                // to do: handle end of path
                Destroy(gameObject);
                return;
            }
            MoveToNode(currentPath[0]);
        }

        private void MoveToNode(TownMovementNode nextNode)
        {
            //float speed = Vector2.Distance(transform.position, nextNode.transform.position) / 1f;
            transform.DOMove(nextNode.transform.position, WALK_SPEED)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnComplete(()=> OnNodeReached(nextNode));
        }
    }
}