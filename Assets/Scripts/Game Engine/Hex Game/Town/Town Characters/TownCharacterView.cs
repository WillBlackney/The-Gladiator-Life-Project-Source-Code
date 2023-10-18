using DG.Tweening;
using Sirenix.OdinInspector;
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

        [Header("Settings")]
        [SerializeField] private bool staticCharacter;
        [ShowIf("ShowStaticFields")]
        [SerializeField] private TownCharacterSeed characterSeed;
        [ShowIf("ShowStaticFields")]
        [SerializeField] private int sortOrder;
        [ShowIf("ShowStaticFields")]
        [SerializeField] private bool faceLeft = false;

        private List<TownMovementNode> currentPath = new List<TownMovementNode>();
        private const float MIN_WALK_SPEED = 25;
        private const float MAX_WALK_SPEED = 35;
        private float myMoveSpeed;

        public UniversalCharacterModel Ucm => ucm;

        public void InitializeAsStatic()
        {
            if (!staticCharacter) return;

            gameObject.SetActive(true);
            CharacterModeller.BuildModelFromStringReferences(ucm, characterSeed.GetRandomAppearance());
            ucm.SetIdleAnim(); 
            ucm.RootSortingGroup.sortingOrder = sortOrder;
            if(faceLeft)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }

        public IEnumerator InitializeRoamingCharacter(TownCharacterSeed seed, List<TownMovementNode> path)
        {
            // Setup path
            currentPath.Clear();
            currentPath.AddRange(path);
            myMoveSpeed = RandomGenerator.NumberBetween(MIN_WALK_SPEED, MAX_WALK_SPEED) * 0.01f;

            // Setup model
            CharacterModeller.BuildModelFromStringReferences(ucm, seed.GetRandomAppearance());
            ucm.SetWalkAnim();
            yield return new WaitForSeconds(0.25f);
            // Place at at start
            TownMovementNode start = path[0];
            transform.DOMove(start.transform.position, 0f);
            start.OnCharacterArrived(this);
            currentPath.RemoveAt(0);

            // Start move
            MoveToNode(currentPath[0]);

        }

        private IEnumerator OnNodeReached(TownMovementNode node)
        {
            currentPath.Remove(node);
            node.OnCharacterArrived(this);

            if (currentPath.Count == 0 )
            {
                ucm.SetIdleAnim();
                CharacterModeller.FadeOutCharacterModel(ucm, 1f, () => Destroy(gameObject));
                yield break;
            }

            // Randomly pause movement
            int roll = RandomGenerator.NumberBetween(1, 5);
            if(roll == 1 && node.myPausedCharacter == null)
            {
                node.myPausedCharacter = this;
                ucm.SetIdleAnim();
                yield return new WaitForSeconds(RandomGenerator.NumberBetween(4, 8));
                node.myPausedCharacter = null;
                ucm.SetWalkAnim();
            }

            MoveToNode(currentPath[0]);
        }

        private void MoveToNode(TownMovementNode nextNode)
        {
            SetFacing(transform.position, nextNode.transform.position);
            transform.DOMove(nextNode.transform.position, myMoveSpeed)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnComplete(()=> 
                { 
                    if(gameObject.activeInHierarchy)
                    {
                        StartCoroutine(OnNodeReached(nextNode));
                    }
                });
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

        public bool ShowStaticFields()
        {
            return staticCharacter;
        }

        public void TearDown()
        {
            StopAllCoroutines();
            transform.DOKill();
            Destroy(gameObject);
        }
    }
}