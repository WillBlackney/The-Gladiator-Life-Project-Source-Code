using HexGameEngine.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HexGameEngine.TownFeatures
{
    public class TownSpriteSheetAnimator : MonoBehaviour
    {
        [SerializeField] Image characterImage;

        [SerializeField] Sprite[] idleAnimationFrames;

        [SerializeField] float frameSpeed = 0.05f;
        float countdown = 0f;

        int currentFrame = 0;

        private void Start()
        {
            // Randomize starting point of idle animation
            currentFrame = RandomGenerator.NumberBetween(0, idleAnimationFrames.Length - 1);
        }
        private void OnEnable()
        {
            // Randomize starting point of idle animation
            currentFrame = RandomGenerator.NumberBetween(0, idleAnimationFrames.Length - 1);
        }

        private void Update()
        {
            countdown -= Time.deltaTime;
            if(countdown <= 0)
            {
                countdown = frameSpeed;
                currentFrame++;
                if (currentFrame > idleAnimationFrames.Length - 1) currentFrame = 0;
                characterImage.sprite = idleAnimationFrames[currentFrame];
            }
        }
    }
}