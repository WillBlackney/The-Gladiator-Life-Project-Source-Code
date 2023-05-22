using CardGameEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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

        float selfMoveSpeed = 22.5f;
        private bool cheering = false;
        private bool dissapointing = false;
        bool moveDown = false;
        #endregion

        #region Initialization
        private void Start()
        {
            InitializeSelfMove();
        }
        private void InitializeSelfMove()
        {
            selfMoveSpeed = RandomGenerator.NumberBetween(15, 30);
            moveDown = RandomGenerator.NumberBetween(1, 2) == 1;
        }
        #endregion

        #region Animate and Move
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P)) DoCheerAnimation();
            if (Input.GetKeyDown(KeyCode.K)) DoDissapointedAnimation();

            SelfMove();
        }
        public void DoCheerAnimation()
        {
            if (cheering || dissapointing) return;

            float speed = RandomGenerator.NumberBetween(85, 115) * 0.01f;

            dissapointedArmsParent.SetActive(false);
            cheeringArmsParent.SetActive(true);
            cheeringArmsParent.transform.DOKill();
            cheeringArmsParent.transform.localPosition = Vector3.zero;
            cheeringArmsParent.transform.DOScale(0.25f, 0f);
            cheeringArmsParent.transform.DOScale(1f, 0.25f * speed).SetEase(Ease.OutCubic);
            cheeringArmsParent.transform.DOLocalMoveY(20f, 0.2f * speed).SetLoops(16, LoopType.Yoyo).OnComplete(()=>
            {
                cheeringArmsParent.transform.DOLocalMoveY(-50f, 0.2f * speed);
                cheeringArmsParent.transform.DOScale(0.25f, 0.25f * speed).OnComplete(() => cheeringArmsParent.SetActive(false));
            });            

            reactionMovementParent.transform.DOKill();
            reactionMovementParent.transform.localPosition = Vector3.zero;
            reactionMovementParent.transform.DOLocalMoveY(50, 0.5f * speed).SetEase(Ease.OutCubic).OnComplete(() =>
                DOVirtual.DelayedCall(2f * speed, () =>
                {
                    reactionMovementParent.transform.DOLocalMove(Vector3.zero, 0.5f * speed).SetEase(Ease.Linear);
                    cheering = false;
                })
            ); 

        }
        public void DoDissapointedAnimation()
        {
            if (cheering || dissapointing) return;

            float speed = RandomGenerator.NumberBetween(85, 115) * 0.01f;

            cheeringArmsParent.SetActive(false);
            dissapointedArmsParent.SetActive(true);
            dissapointedArmsParent.transform.DOKill();
            dissapointedArmsParent.transform.DOScale(0.25f, 0f);
            dissapointedArmsParent.transform.DOScale(1f, 0.75f * speed).SetEase(Ease.OutCubic);

            reactionMovementParent.transform.DOKill();
            reactionMovementParent.transform.localPosition = Vector3.zero;
            reactionMovementParent.transform.DOLocalMoveY(50, 0.5f * speed).SetEase(Ease.OutCubic).OnComplete(() =>
                DOVirtual.DelayedCall(2f * speed, () =>
                {
                    reactionMovementParent.transform.DOLocalMove(Vector3.zero, 0.5f * speed).SetEase(Ease.Linear);
                    dissapointedArmsParent.transform.DOScale(0.25f, 0.5f * speed).OnComplete(()=> dissapointedArmsParent.SetActive(false));
                    dissapointing = false;
                })
            );

        }
        private void SelfMove()
        {
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

    }
}