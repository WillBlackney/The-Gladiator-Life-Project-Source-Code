using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.TownFeatures
{
    public class TownHive : MonoBehaviour
    {
        #region Components + Variables
        [Header("Settings")]
        [Range(0.25f,10)]
        [SerializeField] private float minSpawnCooldown = 1;
        [Range(0.25f, 10)]
        [SerializeField] private float maxSpawnCooldown = 2;
        [Space(20)]

        [Header("Scene Components")]
        [SerializeField] private Transform charactersParent;
        [SerializeField] private TownPath[] paths;
        [Space(20)]

        [Header("Data")]
        [SerializeField] private TownCharacterView characterPrefab;
        [SerializeField] private TownCharacterSeed[] characterSeeds;

        private float currentSpawnCooldown;

        private List<TownPath> currentPathStack = new List<TownPath>();
        #endregion

        #region Getters + Accessors
        public float MinSpawnCooldown => minSpawnCooldown;
        public float MaxSpawnCooldown => maxSpawnCooldown;
        #endregion

        private void Start()
        {
            StartTownPopulation();
        }
        private void Update()
        {
            TickDownSpawnTimer();
        }

        private void TickDownSpawnTimer()
        {
            currentSpawnCooldown -= Time.deltaTime;
            if (currentSpawnCooldown < 0)
            {
                currentSpawnCooldown = RandomGenerator.NumberBetween(MinSpawnCooldown, MaxSpawnCooldown);
                SpawnCharacterOnPath();
            }
        }
       
        public void StartTownPopulation()
        {
            // Set intial path randomization
            currentPathStack = new List<TownPath>();
            currentPathStack.AddRange(paths);
            currentPathStack.Shuffle();

            // Set initial cooldown
            currentSpawnCooldown = RandomGenerator.NumberBetween(MinSpawnCooldown, MaxSpawnCooldown);

            paths.ForEach(path =>
            {
                path.Initialize(this);
            });
        }
        private void SpawnCharacterOnPath()
        {
            if(currentPathStack.Count == 0)
            {
                currentPathStack.AddRange(paths);
                currentPathStack.Shuffle();
            }

            TownPath path = currentPathStack.GetRandomElement();
            currentPathStack.Remove(path);

            TownCharacterView newCharacter = Instantiate(characterPrefab, charactersParent);
            TownCharacterSeed characterSeed = characterSeeds.GetRandomElement();
            newCharacter.Initialize(characterSeed, path);
        }
    }
}