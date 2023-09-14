using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WeAreGladiators.VisualEvents
{
    public class Projectile : MonoBehaviour
    {

        // Initialization 
        #region

        public void Initialize(Vector3 startPos, Vector3 endPos, Action onImpactCallback = null)
        {
            transform.position = new Vector3(startPos.x, startPos.y + yOffset, startPos.z);
            start = transform.position;
            destination = endPos;
            initialDistanceX = Mathf.Abs(destination.x - start.x);
            if (initialDistanceX == 0f)
            {
                initialDistanceX = 0.01f;
            }
            this.onImpactCallback = onImpactCallback;
            readyToMove = true;
        }

        #endregion

        public bool ShowMaxParabolaY()
        {
            return moveAsParabola;
        }
        // This script is for NON PARTICLE FX based projectiles
        // Properties + Component References
        #region

        // Inspector Fields
        [Header("Movement Settings")]
        [Tooltip("Adjusts the starting position of the projectile.")]
        [SerializeField] private float yOffset;
        [SerializeField] private float movementSpeed = 10f;
        [SerializeField] private bool moveAsParabola = true;
        [Tooltip("Determines the amount of parabola effect on the projectile. " +
            "A value of 0 means the projectile will travel in absolutely straight line from starting point to target.")]
        [Range(0, 0.5f)]
        [ShowIf("ShowMaxParabolaY")]
        [SerializeField]
        private float maxParabolaY = 0.25f;

        // Fields
        private Vector3 start;
        private Vector3 destination;
        private bool readyToMove;
        private bool destinationReached;
        private float initialDistanceX;
        private float nextX;
        private float baseY;
        private float height;
        private Action onImpactCallback;

        #endregion

        // Movement logic
        #region

        private void Update()
        {
            if (readyToMove)
            {
                MoveTowardsTarget();
            }
        }
        private void MoveTowardsTarget()
        {
            if (destinationReached)
            {
                return;
            }
            if (transform.position.x == destination.x)
            {
                moveAsParabola = false;
            }
            if (moveAsParabola)
            {
                MoveParabola();
            }
            else
            {
                MoveNormally();
            }

            float vectorDistance = Vector2.Distance(transform.position, destination);
            if (transform.position == destination || vectorDistance <= 0.05f)
            {
                destinationReached = true;
                if (onImpactCallback != null)
                {
                    onImpactCallback.Invoke();
                }
                DestroySelf();
            }
        }
        private void MoveParabola()
        {
            nextX = Mathf.MoveTowards(transform.position.x, destination.x, movementSpeed * Time.deltaTime);
            float lerpX = Mathf.Abs(nextX - start.x);
            float lerpY = nextX - destination.x;
            float dirMod = -0.25f;
            baseY = Mathf.Lerp(start.y, destination.y, lerpX / initialDistanceX);
            height = Mathf.Abs(maxParabolaY * lerpX * lerpY / (dirMod * initialDistanceX * initialDistanceX));

            Vector3 movePosition = new Vector3(nextX, baseY + height, transform.position.z);
            transform.rotation = LookAtTarget(movePosition - transform.position);
            transform.position = movePosition;
        }
        private void MoveNormally()
        {
            Vector3 movePosition = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
            transform.rotation = LookAtTarget(movePosition - transform.position);
            transform.position = movePosition;
        }

        #endregion

        // Misc Logic
        #region

        public static Quaternion LookAtTarget(Vector2 rotation)
        {
            return Quaternion.Euler(0, 0, Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg);
        }
        private void DestroySelf()
        {
            gameObject.SetActive(false);
            Destroy(gameObject, 0.25f);
        }

        #endregion
    }
}
