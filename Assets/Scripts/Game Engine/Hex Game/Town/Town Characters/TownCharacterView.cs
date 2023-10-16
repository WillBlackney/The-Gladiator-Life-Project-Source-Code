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
        private const float WALK_SPEED = 0.33f;

        public UniversalCharacterModel Ucm => ucm;

        public void Initialize(TownCharacterSeed seed, TownPath path)
        {
            // Setup path
            currentPath.Clear();
            currentPath.AddRange(path.Nodes);

            // Setup model
            CharacterModeller.BuildModelFromStringReferences(ucm, seed.GetRandomAppearance());
            ucm.SetWalkAnim();

            DelayUtils.DelayedCall(0.5f, () =>
            {
                // Place at at start
                TownMovementNode start = path.Nodes[0];
                transform.DOMove(start.transform.position, 0f);
                start.OnCharacterArrived(this);                
                currentPath.RemoveAt(0);

                // Start move
                MoveToNode(currentPath[0]);
            });
            

        }

        private void OnNodeReached(TownMovementNode node)
        {
            currentPath.Remove(node);
            node.OnCharacterArrived(this);

            if (currentPath.Count == 0 )
            {
                ucm.SetIdleAnim();
                CharacterModeller.FadeOutCharacterModel(ucm, 1f, () => Destroy(gameObject));
                return;
            }
            MoveToNode(currentPath[0]);
        }

        private void MoveToNode(TownMovementNode nextNode)
        {
            SetFacing(transform.position, nextNode.transform.position);
            transform.DOMove(nextNode.transform.position, WALK_SPEED)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnComplete(()=> OnNodeReached(nextNode));
        }

        private void SetFacing(Vector3 current, Vector3 next)
        {
            if(current.x < next.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
}