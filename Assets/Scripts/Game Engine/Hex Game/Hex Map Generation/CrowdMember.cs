using DG.Tweening;
using HexGameEngine.HexTiles;
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

        [Header("Cheering Components")]
        [SerializeField] GameObject cheeringArmsParent;
        [SerializeField] GameObject cheeringRightArmParent;
        [SerializeField] GameObject cheeringLeftArmParent;

        [Header("Dissapointed Components")]
        [SerializeField] GameObject dissapointedArmsParent;
        [SerializeField] GameObject dissapointedRightArmParent;
        [SerializeField] GameObject dissapointedLeftArmParent;

        bool allowSelfMove = false;
        float selfMoveSpeed = 22.5f;
        private bool cheering = false;
        private bool dissapointing = false;
        bool moveDown = false;
        Vector3 startingLocalPos;
        #endregion

        #region Initialization + Setup + Reset        
        public void StartSelfMove()
        {
            startingLocalPos = movementParent.transform.localPosition;
            allowSelfMove = true;
            selfMoveSpeed = RandomGenerator.NumberBetween(15, 30);
            moveDown = RandomGenerator.NumberBetween(1, 2) == 1;
        }
        public void ResetSelf()
        {
            allowSelfMove = false;
            dissapointing = false;
            cheering = false;
            movementParent.transform.localPosition = startingLocalPos;

            dissapointedArmsParent.transform.DOKill();
            dissapointedArmsParent.SetActive(false);
            dissapointedArmsParent.transform.localPosition = Vector3.zero;

            cheeringArmsParent.transform.DOKill();
            cheeringArmsParent.SetActive(false);
            cheeringArmsParent.transform.localPosition = Vector3.zero;

            reactionMovementParent.transform.DOKill();
            reactionMovementParent.transform.localPosition = Vector3.zero;
        }
        #endregion

        #region Animate and Move
        private void Update()
        {
            SelfMove();
        }
        public void DoCheerAnimation()
        {
            if (cheering || dissapointing) return;

            cheering = true;
            float speed = RandomGenerator.NumberBetween(80, 120) * 0.01f;
            float startDelay = RandomGenerator.NumberBetween(1, 3) * 0.1f;

            cheeringRightArmParent.SetActive(false);
            cheeringLeftArmParent.SetActive(false);

            DelayUtils.DelayedCall(startDelay, () =>
            {
                // Randomize anim / arms
                int anim = RandomGenerator.NumberBetween(1, 3);
                if (anim == 1) cheeringRightArmParent.SetActive(true);
                else if (anim == 2) cheeringLeftArmParent.SetActive(true);
                else
                {
                    cheeringRightArmParent.SetActive(true);
                    cheeringLeftArmParent.SetActive(true);
                }

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
        public void DoDissapointedAnimation()
        {            
            if (cheering || dissapointing) return;
            dissapointing = true;
            float speed = RandomGenerator.NumberBetween(85, 115) * 0.01f;
            float startDelay = RandomGenerator.NumberBetween(1, 3) * 0.1f;

            dissapointedRightArmParent.SetActive(false);
            dissapointedLeftArmParent.SetActive(false);

            DelayUtils.DelayedCall(startDelay, () =>
            {
                // Randomize anim / arms
                int anim = RandomGenerator.NumberBetween(1, 3);
                if (anim == 1) dissapointedLeftArmParent.SetActive(true);
                else if (anim == 2) dissapointedRightArmParent.SetActive(true);
                else
                {
                    dissapointedRightArmParent.SetActive(true);
                    dissapointedLeftArmParent.SetActive(true);
                }
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
        public void DoCombatFinishedAnimation()
        {
            if (cheering || dissapointing) return;

            float speed = RandomGenerator.NumberBetween(80, 120) * 0.01f;
            float startDelay = RandomGenerator.NumberBetween(1, 3) * 0.1f;
            //float speed = 1f;
            //float startDelay = 0.1f;

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
                movementParent.transform.localPosition = Vector2.MoveTowards(movementParent.transform.localPosition, new Vector2(0, -10), selfMoveSpeed * Time.deltaTime);
                if (movementParent.transform.localPosition.y <= -10) moveDown = false;
            }
            else
            {
                movementParent.transform.localPosition = Vector2.MoveTowards(movementParent.transform.localPosition, new Vector2(0, 10), selfMoveSpeed * Time.deltaTime);
                if (movementParent.transform.localPosition.y >= 10) moveDown = true;
            }
        }
        #endregion

    }
}