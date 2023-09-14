using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.TownFeatures
{
    public class TownSpriteSheetAnimator : MonoBehaviour
    {
        [SerializeField] private Image characterImage;

        [SerializeField] private Sprite[] idleAnimationFrames;

        [SerializeField] private float frameSpeed = 0.05f;
        private float countdown;

        private int currentFrame;

        private void Start()
        {
            // Randomize starting point of idle animation
            currentFrame = RandomGenerator.NumberBetween(0, idleAnimationFrames.Length - 1);
        }

        private void Update()
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0)
            {
                countdown = frameSpeed;
                currentFrame++;
                if (currentFrame > idleAnimationFrames.Length - 1)
                {
                    currentFrame = 0;
                }
                characterImage.sprite = idleAnimationFrames[currentFrame];
            }
        }
        private void OnEnable()
        {
            // Randomize starting point of idle animation
            currentFrame = RandomGenerator.NumberBetween(0, idleAnimationFrames.Length - 1);
        }
    }
}
