using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.TownFeatures
{
    public class TownHive : MonoBehaviour
    {
        #region Components + Variables
        [Header("Settings")]
        [Range(1,10)]
        [SerializeField] private int minSpawnCooldown = 3;
        [Range(1, 10)]
        [SerializeField] private int maxSpawnCooldown = 5;
        [Space(20)]

        [Header("Scene Components")]
        [SerializeField] private Transform charactersParent;
        [SerializeField] private TownPath[] paths;
        [Space(20)]

        [Header("Data")]
        [SerializeField] private TownCharacterView characterPrefab;
        [SerializeField] private TownCharacterSeed[] characterSeeds;
        #endregion

        #region Getters + Accessors
        public int MinSpawnCooldown => minSpawnCooldown;
        public int MaxSpawnCooldown => maxSpawnCooldown;
        #endregion

        private void Start()
        {
            StartTownPopulation();
        }
        public void StartTownPopulation()
        {
            paths.ForEach(path =>
            {
                path.Initialize(this);
            });
        }

        public void SpawnCharacterOnPath(TownPath path)
        {
            TownCharacterView newCharacter = Instantiate(characterPrefab, charactersParent);
            TownCharacterSeed characterSeed = characterSeeds.GetRandomElement();
            newCharacter.Initialize(characterSeed, path);
        }
    }
}