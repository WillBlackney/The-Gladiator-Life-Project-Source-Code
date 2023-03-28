using UnityEngine;
using System.Collections;

namespace EpicToonFX
{
    public class ETFXLightFade : MonoBehaviour
    {
        [Header("Seconds to dim the light")]
        public float life = 0.2f;
        public bool killAfterLife = true;

        private Light li;
        private float initIntensity;

        public Light Li
        {
            get 
            { 
                if (li == null) li = GetComponent<Light>();
                return li;
            }
            private set { li = value; }
        }

        // Use this for initialization
        void Start()
        {
            if (Li != null) initIntensity = Li.intensity;            
        }
        
        // Update is called once per frame
        void Update()
        {
            if (Li != null)
            {
                Li.intensity -= initIntensity * (Time.deltaTime / life);
                /*
                if (killAfterLife && li.intensity <= 0)
                    //Destroy(gameObject);
					Destroy(gameObject.GetComponent<Light>());
                */
            }
        }
    }
}