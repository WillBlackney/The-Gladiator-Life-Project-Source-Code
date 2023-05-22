using DG.Tweening;
using HexGameEngine.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexGameEngine.UI
{
    public class CrowdMember : MonoBehaviour
    {
        #region Components + Variables
        [Header("Core Components")]
        [SerializeField] GameObject movementParent;
        [SerializeField] GameObject reactionMovementParent;

        [Header("Arm Components")]
        [SerializeField] GameObject cheeringArmsParent;
        [SerializeField] GameObject dissapointedArmsParent;

        bool allowSelfMove = false;
        float selfMoveSpeed = 22.5f;
        private bool cheering = false;
        private bool dissapointing = false;
        bool moveDown = false;
        Vector3 startingLocalPos;
        #endregion

        #region Initialization + Setup + Reset        
        private void StartSelfMove()
        {
            startingLocalPos = movementParent.transform.localPosition;
            allowSelfMove = true;
            selfMoveSpeed = RandomGenerator.NumberBetween(15, 30);
            moveDown = RandomGenerator.NumberBetween(1, 2) == 1;
        }
        public static void StopAllCrowdMembers(bool reset = false)
        {
            AllCrowdMembers.ForEach(x =>
            {
                x.allowSelfMove = false;
                x.dissapointing = false;
                x.cheering = false;
                x.movementParent.transform.localPosition = x.startingLocalPos;

                x.dissapointedArmsParent.transform.DOKill();
                x.dissapointedArmsParent.SetActive(false);
                x.dissapointedArmsParent.transform.localPosition = Vector3.zero;

                x.cheeringArmsParent.transform.DOKill();
                x.cheeringArmsParent.SetActive(false);
                x.cheeringArmsParent.transform.localPosition = Vector3.zero;

                x.reactionMovementParent.transform.DOKill();
                x.reactionMovementParent.transform.localPosition = Vector3.zero;

                if (reset == true)
                {
                    x.StartSelfMove();
                }
            });
        }
        #endregion

        #region Animate and Move
        private void Update()
        {
            SelfMove();
        }
        private void DoCheerAnimation()
        {
            if (cheering || dissapointing) return;

            float speed = RandomGenerator.NumberBetween(80, 120) * 0.01f;
            float startDelay = RandomGenerator.NumberBetween(1, 3) * 0.1f;

            DelayUtils.DelayedCall(startDelay, () =>
            {
                dissapointedArmsParent.SetActive(false);
                cheeringArmsParent.SetActive(true);
                cheeringArmsParent.transform.DOKill();
                cheeringArmsParent.transform.localPosition = Vector3.zero;
                cheeringArmsParent.transform.DOScale(0.25f, 0f);
                cheeringArmsParent.transform.DOScale(1f, 0.5f * speed).SetEase(Ease.OutCubic);
                cheeringArmsParent.transform.DOLocalMoveY(20f, 0.4f * speed).SetLoops(8, LoopType.Yoyo).OnComplete(() =>
                {
                    cheeringArmsParent.transform.DOLocalMoveY(-20f, 0.4f * speed);
                    cheeringArmsParent.transform.DOScale(0.25f, 0.5f * speed).OnComplete(() => cheeringArmsParent.SetActive(false));
                });

                reactionMovementParent.transform.DOKill();
                reactionMovementParent.transform.localPosition = Vector3.zero;
                reactionMovementParent.transform.DOLocalMoveY(15, 0.75f * speed).SetEase(Ease.OutCubic).OnComplete(() =>
                    DelayUtils.DelayedCall(2f * speed, () =>
                    {
                        reactionMovementParent.transform.DOLocalMove(Vector3.zero, 0.75f * speed).SetEase(Ease.Linear);
                        cheering = false;
                    })
                );
            });             

        }
        private void DoDissapointedAnimation()
        {
            if (cheering || dissapointing) return;

            float speed = RandomGenerator.NumberBetween(85, 115) * 0.01f;
            float startDelay = RandomGenerator.NumberBetween(1, 3) * 0.1f;

            DelayUtils.DelayedCall(startDelay, () =>
            {
                cheeringArmsParent.SetActive(false);
                dissapointedArmsParent.SetActive(true);
                dissapointedArmsParent.transform.DOKill();
                dissapointedArmsParent.transform.DOScale(0.25f, 0f);
                dissapointedArmsParent.transform.DOScale(1f, 0.75f * speed).SetEase(Ease.OutCubic);

                reactionMovementParent.transform.DOKill();
                reactionMovementParent.transform.localPosition = Vector3.zero;
                reactionMovementParent.transform.DOLocalMoveY(15, 0.5f * speed).SetEase(Ease.OutCubic).OnComplete(() =>
                    DelayUtils.DelayedCall(2f * speed, () =>
                    {
                        reactionMovementParent.transform.DOLocalMove(Vector3.zero, 0.5f * speed).SetEase(Ease.Linear);
                        dissapointedArmsParent.transform.DOScale(0.25f, 0.5f * speed).OnComplete(() => dissapointedArmsParent.SetActive(false));
                        dissapointing = false;
                    })
                );
            });            

        }
        private void DoCombatFinishedAnimation()
        {
            if (cheering || dissapointing) return;

            float speed = RandomGenerator.NumberBetween(80, 120) * 0.01f;
            float startDelay = RandomGenerator.NumberBetween(1, 3) * 0.1f;

            DelayUtils.DelayedCall(startDelay, () =>
            {
                dissapointedArmsParent.SetActive(false);
                cheeringArmsParent.SetActive(true);
                cheeringArmsParent.transform.DOKill();
                cheeringArmsParent.transform.localPosition = Vector3.zero;
                cheeringArmsParent.transform.DOScale(0.25f, 0f);
                cheeringArmsParent.transform.DOScale(1f, 0.5f * speed).SetEase(Ease.OutCubic);
                cheeringArmsParent.transform.DOLocalMoveY(20f, 0.4f * speed).SetLoops(-1, LoopType.Yoyo);

                reactionMovementParent.transform.DOKill();
                reactionMovementParent.transform.localPosition = Vector3.zero;
                reactionMovementParent.transform.DOLocalMoveY(15, 0.75f * speed).SetEase(Ease.OutCubic);
            });

        }
        private void SelfMove()
        {
            if (!allowSelfMove) return;

            if (moveDown)
            {
                Debug.Log("XX Moving Down");
                movementParent.transform.localPosition = Vector2.MoveTowards(movementParent.transform.localPosition, new Vector2(0, -10), selfMoveSpeed * Time.deltaTime);
                if (movementParent.transform.localPosition.y <= -10) moveDown = false;
            }
            else
            {
                Debug.Log("XX Moving Up");
                movementParent.transform.localPosition = Vector2.MoveTowards(movementParent.transform.localPosition, new Vector2(0, 10), selfMoveSpeed * Time.deltaTime);
                if (movementParent.transform.localPosition.y >= 10) moveDown = true;
            }
        }
        #endregion

        #region Static Logic
        private static List<CrowdMember> allCrowdMembers;
        public static List<CrowdMember> AllCrowdMembers
        {
            get
            {
                if (allCrowdMembers == null) allCrowdMembers = FindObjectsOfType<CrowdMember>().ToList();
                return allCrowdMembers;
            }
        }
        public static void AnimateCrowdOnHit()
        {
            List<CrowdMember> animatedMembers = AllCrowdMembers.ShuffledCopy();
            int minAnims = (int) (animatedMembers.Count * 0.2f);
            int maxAnims = (int) (animatedMembers.Count * 0.3f);
            int totalAnims = RandomGenerator.NumberBetween(minAnims, maxAnims);
            for (int i = 0; i < totalAnims; i++) animatedMembers[i].DoCheerAnimation();
        }
        public static void AnimateCrowdOnMiss()
        {
            List<CrowdMember> animatedMembers = AllCrowdMembers.ShuffledCopy();
            int minAnims = (int)(animatedMembers.Count * 0.2f);
            int maxAnims = (int)(animatedMembers.Count * 0.3f);
            int totalAnims = RandomGenerator.NumberBetween(minAnims, maxAnims);
            for (int i = 0; i < totalAnims; i++) animatedMembers[i].DoDissapointedAnimation();
        }
        public static void AnimateCrowdOnCombatVictory()
        {
            List<CrowdMember> animatedMembers = AllCrowdMembers.ShuffledCopy();
            int totalAnims = (int)(animatedMembers.Count * 0.7f);
            for (int i = 0; i < totalAnims; i++) animatedMembers[i].DoCombatFinishedAnimation();
        }
       
        #endregion
    }
}