using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;

namespace HexGameEngine.LoadingScreen
{
    public class LoadingScreenController : Singleton<LoadingScreenController>
    {
        [Header("Core Components")]
        [SerializeField] GameObject visualParent;
        [SerializeField] CanvasGroup mainCg;
        [SerializeField] TextMeshProUGUI tipsText;
        [SerializeField] TextMeshProUGUI loadText;
        [SerializeField] Image mainImage;

        [Header("Images + Tips Data")]
        [SerializeField] List<Sprite> allBackgroundSprites;
        private List<Sprite> sessionSpriteQueue = new List<Sprite>();

        [SerializeField] List<string> allTips;
        private List<string> sessionTipsQueue = new List<string>();

        protected override void Awake()
        {
            base.Awake();
            DOVirtual.DelayedCall(4f, () =>
            {
                if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
                {
                    // Starting from the preloader scene
                    Debug.LogWarning("LoadingScreenController.Preloader!");
                    StartCoroutine(StartPreloaderSequence());
                }
            });
            
        }

        public void ShowLoadingScreen(float showInSpeed = 1.5f, float hideOutSpeed = 1f, Action onPauseReached = null, Action onPauseFinished = null, Func<bool> awaitCondition = null)
        {
            StartCoroutine(ShowLoadingScreenCoroutine(showInSpeed, hideOutSpeed, onPauseReached, onPauseFinished, awaitCondition));
        }
        private IEnumerator ShowLoadingScreenCoroutine(float showInSpeed = 0.5f, float hideOutSpeed = 0.5f, Action onPauseReached = null, Action onPauseFinished = null, Func<bool> awaitCondition = null)
        {
            float showDuration = RandomGenerator.NumberBetween(25, 35) * 0.1f;

            // Reset + setup
            mainCg.DOKill();
            mainCg.blocksRaycasts = true;
            mainCg.alpha = 0;
            visualParent.SetActive(true);

            // Build content
            mainImage.sprite = GetNextBackgroundImage();
            tipsText.text = GetNextTip();

            // Fade in
            mainCg.DOFade(1f, showInSpeed);
            yield return new WaitForSeconds(showInSpeed);
            if (awaitCondition != null) yield return new WaitUntil(awaitCondition);
            if (onPauseReached != null) onPauseReached.Invoke();

            // Hold screen to show tip
            yield return new WaitForSeconds(showDuration);
            if (onPauseFinished != null) onPauseFinished.Invoke();

            // Start hide views
            mainCg.DOFade(0f, hideOutSpeed).OnComplete(() =>
            {
                mainCg.blocksRaycasts = false;
                visualParent.SetActive(false);
            });
        }


        #region Background Image Logic
        private void PopulateAndShuffleSessionSpriteQueue()
        {
            sessionSpriteQueue.Clear();
            sessionSpriteQueue.AddRange(allBackgroundSprites);
            sessionSpriteQueue.Shuffle();
        }
        private Sprite GetNextBackgroundImage()
        {
            Sprite ret = null;
            if (sessionSpriteQueue.Count == 0) PopulateAndShuffleSessionSpriteQueue();
            ret = sessionSpriteQueue[0];
            sessionSpriteQueue.RemoveAt(0);
            return ret;            
        }
        #endregion

        #region Tips Logic
        private void PopulateAndShuffleSessionTipsQueue()
        {
            sessionTipsQueue.Clear();
            sessionTipsQueue.AddRange(allTips);
            sessionTipsQueue.Shuffle();
        }
        private string GetNextTip()
        {
            string ret = null;
            if (sessionTipsQueue.Count == 0) PopulateAndShuffleSessionTipsQueue();
            ret = sessionTipsQueue[0];
            sessionTipsQueue.RemoveAt(0);
            return ret;
        }
        #endregion

        #region Preloader logic
        IEnumerator StartPreloaderSequence()
        {
            // Reset + setup
            mainCg.blocksRaycasts = true;
            mainCg.alpha = 0f;

            // Build content
            loadText.gameObject.SetActive(true);
            mainImage.sprite = GetNextBackgroundImage();
            tipsText.text = GetNextTip();
            visualParent.SetActive(true);
            yield return null;

            // Fade in
            mainCg.DOFade(1f, 1f);
            yield return new WaitForSeconds(1f);

            // Load main game scene
            AsyncOperation operation = SceneManager.LoadSceneAsync(1);
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f);
                Debug.Log("Load progress = " + progress);
                loadText.text = "Loading " + ((int)(progress * 100f)).ToString() + "%";
                yield return null;
            }

            loadText.gameObject.SetActive(false);

            // Start hide views
            mainCg.DOFade(0f, 1f).OnComplete(() =>
            {
                mainCg.blocksRaycasts = false;
                visualParent.SetActive(false);
            });
        }

        #endregion
    }

    public class LoadingScreenTracker
    {
        public bool finishedFadeIn = false;
        public bool startedFadeOut = false;
        public bool finished = false;
    }
}