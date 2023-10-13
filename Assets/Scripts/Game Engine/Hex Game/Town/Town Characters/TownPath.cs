using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.TownFeatures
{
    public class TownPath : MonoBehaviour
    {
        [Header("Scene Components")]
        [SerializeField] private TownMovementNode[] nodes;

        private float currentSpawnCooldown;
        private TownHive myHive;
        private bool spawning;
        public TownMovementNode[] Nodes => nodes;

        private void Update()
        {
            TickDownSpawnTimer();
        }

        private void TickDownSpawnTimer()
        {
            currentSpawnCooldown -= Time.deltaTime;
            if(currentSpawnCooldown < 0)
            {
                currentSpawnCooldown = RandomGenerator.NumberBetween(myHive.MinSpawnCooldown, myHive.MaxSpawnCooldown);
                myHive.SpawnCharacterOnPath(this);
            }
        }

        public void Initialize(TownHive hive)
        {
            myHive = hive;
            currentSpawnCooldown = RandomGenerator.NumberBetween(myHive.MinSpawnCooldown, myHive.MaxSpawnCooldown);
            spawning = true;
        }

    }
}