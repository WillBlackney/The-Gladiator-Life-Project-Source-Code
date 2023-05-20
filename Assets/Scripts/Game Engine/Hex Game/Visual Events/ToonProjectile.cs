using Codice.CM.Common;
using Sirenix.OdinInspector;
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

        [Header("Movement Settings")]
        [SerializeField] float movementSpeed = 10f;
        [SerializeField] bool moveAsParabola = true;
        [Tooltip("Determines the amount of parabola effect on the projectile. " +
            "A value of 0 means the projectile will travel in absolutely straight line from starting point to target.")]
        [Range(0, 0.5f)]
        [ShowIf("ShowMaxParabolaY")]
        [SerializeField] float maxParabolaY = 0.25f;

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
        float initialDistanceX;
        float nextX;
        float baseY;
        float height;
        Action onImpactCallback;

        // Setup + Initialization
        #region

        public void Initialize(int sortingOrder, float scaleModifier, Vector3 startPos, Vector3 endPos, Action onImpactCallback)
        {
            // Get core components (rigid body, colliders, etc)
            GetMyComponents();

            // Set values
            transform.position = startPos;
            start = startPos;
            destination = endPos;
            initialDistanceX = Mathf.Abs(destination.x - start.x);
            if (initialDistanceX <= 0f) initialDistanceX = 0.01f;
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

            if (transform.position.x == destination.x) moveAsParabola = false;

            if (moveAsParabola) MoveParabola();
            else MoveNormally();

            float vectorDistance = Vector2.Distance(transform.position, destination);
            if (transform.position == destination || vectorDistance <= 0.05f)
            {
                destinationReached = true;
                OnDestinationReached();
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
            transform.position = movePosition;

        }
        private void MoveNormally()
        {
            Vector3 movePosition = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
            transform.rotation = Projectile.LookAtTarget(movePosition - transform.position);
            transform.position = movePosition;
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

        public bool ShowMaxParabolaY()
        {
            return moveAsParabola;
        }
    }
}
