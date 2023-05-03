using Codice.CM.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.VisualEvents
{
    public class ToonProjectile : MonoBehaviour
    {
        [Header("View Properties")]
        [SerializeField] private float myScaleModifier;
        [SerializeField] private int mySortingOrder;
        [SerializeField] private float projectileDestroyDelay;
        [Tooltip("Determines the amount of parabola effect on the projectile. " +
            "A value of 0 means the projectile will travel in absolutely straight line from starting point to target.")]
        [Range(0, 0.5f)]
        [SerializeField] float maxParabolaY = 0.2f;

        [Header("Component References")]
        [SerializeField] private Rigidbody myRigidBody;
        [SerializeField] private SphereCollider mySphereCollider;

        [Header("Prefab References")]
        [SerializeField] private GameObject impactParticle; // Effect spawned when projectile hits a collider
        [SerializeField] private GameObject projectileParticle; // Effect attached to the gameobject as child
        [SerializeField] private GameObject muzzleParticle; // Effect instantly spawned when gameobject is spawned

        [Header("Misc Properties")]
        [SerializeField] private float colliderRadius = 1f;
        [Range(0f, 1f)] // This is an offset that moves the impact effect slightly away from the point of impact to reduce clipping of the impact effect
        [SerializeField] private float collideOffset = 0.15f;

        Vector3 start;
        Vector3 destination;
        bool readyToMove = false;
        bool destinationReached = false;
        float travelSpeed;
        float distance;
        float nextX;
        float nextY;
        float baseY;
        float height;
        Action onImpactCallback;

        // Setup + Initialization
        #region

        public void Initialize(int sortingOrder, float scaleModifier, Vector3 startPos, Vector3 endPos, float speed, Action onImpactCallback)
        {
            Debug.Log("TOON PROJECTILE InitializeSetup");

            // Get core components (rigid body, colliders, etc)
            GetMyComponents();

            // Set values
            start = startPos;
            destination = endPos;
            travelSpeed = speed;
            this.onImpactCallback = onImpactCallback;

            // Set parent scale + sorting order
            myScaleModifier = scaleModifier;
            mySortingOrder = sortingOrder;

            // Create missle 
            projectileParticle = Instantiate(projectileParticle, transform.position, transform.rotation) as GameObject;
            projectileParticle.transform.parent = transform;
            SetSortingOrder(projectileParticle, sortingOrder);
            SetScale(projectileParticle, myScaleModifier);

            // Create muzzle
            if (muzzleParticle)
            {
                muzzleParticle = Instantiate(muzzleParticle, transform.position, transform.rotation) as GameObject;
                SetSortingOrder(muzzleParticle, sortingOrder);
                SetScale(muzzleParticle, myScaleModifier);
                Destroy(muzzleParticle, 1.5f); // 2nd parameter is lifetime of effect in seconds
            }

            readyToMove = true;

        }
        private void GetMyComponents()
        {
            if (myRigidBody == null) myRigidBody = GetComponent<Rigidbody>();
            if (mySphereCollider == null) mySphereCollider = transform.GetComponent<SphereCollider>();            
        }
        private void SetSortingOrder(GameObject parent, int sortingOrder)
        {
            Debug.Log("SetSortingOrder() called on " + parent.name);

            List<ParticleSystem> allMyPSystems = new List<ParticleSystem>();

            // get the ps on the parent
            ParticleSystem parentPS = parent.GetComponent<ParticleSystem>();

            // get the p systems on the children
            ParticleSystem[] pSystems = parent.GetComponentsInChildren<ParticleSystem>();

            // combine into one list
            if (parentPS)
            {
                Debug.Log("SetSortingOrder() found a particle system component on " + parent.name);
                allMyPSystems.Add(parentPS);
            }
            allMyPSystems.AddRange(pSystems);

            // set new sorting order value on each particle system found
            foreach (ParticleSystem ps in allMyPSystems)
            {
                Renderer psr = ps.GetComponent<Renderer>();
                psr.sortingOrder += sortingOrder;
            }
        }
        private void SetScale(GameObject parent, float scalePercentage)
        {
            Debug.Log("SetScale() called for " + parent.name + ", scaling by " + scalePercentage.ToString());

            // get current scale
            Vector3 originalScale = parent.transform.localScale;
            Debug.Log(parent.name + " original scale: " + originalScale.x.ToString() + ", " + originalScale.y.ToString() + ", " + originalScale.z.ToString());

            // calculate new scale
            Vector3 newScale = new Vector3(originalScale.x * scalePercentage, originalScale.y * scalePercentage, originalScale.z * scalePercentage);

            // set new scale
            parent.transform.localScale = newScale;
            Debug.Log(parent.name + " new scale: " + parent.transform.localScale.x.ToString() + ", " + parent.transform.localScale.y.ToString() + ", " + parent.transform.localScale.z.ToString());
        }
        #endregion

        // Travel Logic
        #region
       
        private void Update()
        {
            if (readyToMove) MoveTowardsTarget();
        }
        private void MoveTowardsTarget()
        {
            if (destinationReached) return;
            distance = destination.x - start.x;
            nextX = Mathf.MoveTowards(transform.position.x, destination.x, travelSpeed * Time.deltaTime);
            if(maxParabolaY > 0f)
            {
                baseY = Mathf.Lerp(start.y, destination.y, (nextX - start.x) / distance);
                height = maxParabolaY * (nextX - start.x) * (nextX - destination.x) / (-0.25f * distance * distance);
                nextY = baseY + height;
            }
            else nextY = Mathf.MoveTowards(transform.position.y, destination.y, travelSpeed * Time.deltaTime);

            Vector3 movePosition = new Vector3(nextX, nextY, transform.position.z);
            transform.position = movePosition;

            if (transform.position == destination || distance < 0.05f)
            {
                destinationReached = true;
                OnDestinationReached();
            }
        }
        private void OnDestinationReached()
        {
            // Create impact / explosion
            GameObject impactP = Instantiate(impactParticle, transform.position, Quaternion.identity) as GameObject;
            SetSortingOrder(impactP, mySortingOrder);
            SetScale(impactP, myScaleModifier);

            // Detach all children + particle systems from the parent
            ParticleSystem[] trails = GetComponentsInChildren<ParticleSystem>();
            for (int i = 1; i < trails.Length; i++)
            {
                ParticleSystem trail = trails[i];

                if (trail.gameObject.name.Contains("Trail"))
                {
                    trail.transform.SetParent(null);
                    Destroy(trail.gameObject, 2f);
                }
            }

            // Destroy everything
            Destroy(projectileParticle, projectileDestroyDelay); // Destroy projectile
            Destroy(impactP, 3.5f); // Destroy impact / explosion effect
            Destroy(gameObject, 10); // Destroy this script + gameobject

            if (onImpactCallback != null) onImpactCallback.Invoke();
        }
        #endregion
    }
}
