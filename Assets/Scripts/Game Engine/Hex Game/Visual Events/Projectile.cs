﻿using UnityEngine;

namespace HexGameEngine.VisualEvents
{
    public class Projectile : MonoBehaviour
    {
        // This script is for NON PARTICLE FX based projectiles

        // Properties + Component References
        #region
        [Header("Properties")]
        private Vector3 start;
        private Vector3 destination;
        private bool readyToMove;
        private bool destinationReached;
        private float travelSpeed;
        [SerializeField] private float yOffset;

        float distance;
        float nextX;
        float baseY;
        float height;
        float maxParabolaY = 0.35f;

        public bool DestinationReached
        {
            get { return destinationReached; }
            private set { destinationReached = true; }
        }
        #endregion

        // Initialization 
        #region

        public void InitializeSetup(Vector3 startPos, Vector3 endPos, float speed)
        {
            transform.position = new Vector3(startPos.x, startPos.y + yOffset, startPos.z);
            start = transform.position;
            destination = endPos;
            travelSpeed = speed;
            FaceDestination(destination);
            readyToMove = true;
        }
        #endregion

        // Movement logic
        #region
        private void Update()
        {
            if (readyToMove) MoveTowardsTarget();            
        }
        public void MoveTowardsTarget()
        {
            if (destinationReached) return;
            distance = destination.x - start.x;
            nextX = Mathf.MoveTowards(transform.position.x, destination.x, travelSpeed * Time.deltaTime);
            baseY = Mathf.Lerp(start.y, destination.y, (nextX - destination.x) / distance);
            height = maxParabolaY * (nextX - start.x) * (nextX - destination.x) / (-0.25f * distance * distance);

            Vector3 movePosition = new Vector3(nextX, baseY + height, transform.position.z);
            transform.position = movePosition;
            FaceDestination(movePosition);

            if (transform.position == destination)
            {
                destinationReached = true;
                DestroySelf();
            }

            /*
            if (transform.position != destination)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, travelSpeed * Time.deltaTime);
                if (transform.position == destination)
                {
                    destinationReached = true;
                    DestroySelf();
                }
            }*/
        }
        #endregion

        // Misc Logic
        #region
        private void FaceDestination(Vector3 dest)
        {
            Vector2 direction = dest - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10000f);
        }
        private void DestroySelf()
        {
            gameObject.SetActive(false);
            Destroy(gameObject, 0.25f);
        }
        #endregion

    }
}
