using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.TownFeatures
{
    public class TownHive : MonoBehaviour
    {
        #region Components + Variables
        [Header("Settings")]
        [SerializeField] private bool prewarm;
        [Range(0.25f,10)]
        [SerializeField] private float minSpawnCooldown = 1;
        [Range(0.25f, 10)]
        [SerializeField] private float maxSpawnCooldown = 2;
        [Space(20)]

        [Header("Scene Components")]
        [SerializeField] private Transform charactersParent;
        [SerializeField] private TownCharacterView[] staticCharacters;
        [SerializeField] private TownPath[] paths;
        [Space(20)]

        [Header("Data")]
        [SerializeField] private TownCharacterView characterPrefab;
        [SerializeField] private TownCharacterSeed[] characterSeeds;

        private bool hiveIsActive = false;
        private float currentSpawnCooldown;
        public List<TownCharacterView> activeRoamers = new List<TownCharacterView>();
        private List<TownPath> currentPathStack = new List<TownPath>();
        #endregion

        #region Getters + Accessors
        public bool HiveIsActive => hiveIsActive;
        public float MinSpawnCooldown => minSpawnCooldown;
        public float MaxSpawnCooldown => maxSpawnCooldown;
        #endregion              

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
            hiveIsActive = true;
            activeRoamers.Clear();
            currentPathStack = new List<TownPath>();
            currentPathStack.AddRange(paths);
            currentPathStack.Shuffle();

            staticCharacters.ForEach(staticCharacter =>
            {
                staticCharacter.InitializeAsStatic();
            });

            // Set initial cooldown
            currentSpawnCooldown = RandomGenerator.NumberBetween(MinSpawnCooldown, MaxSpawnCooldown);

            if (prewarm)
            {
                Prewarm();
            }
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
            CreateTownCharacter(path.Nodes.ToList());
        }

        private void Prewarm()
        {
            for(int i = 0; i < paths.Length; i++)
            {
                for(int x = 1; x < 3; x++)
                {
                    if (paths[i].Nodes.Length < 4) continue;

                    Debug.Log("Creating pre-warm character");

                    TownPath path = paths[i];
                    int startIndex = x;
                    List<TownMovementNode> newNodes = new List<TownMovementNode>();
                    for (int j = startIndex; j < path.Nodes.Length; j++)
                    {
                        newNodes.Add(path.Nodes[i]);
                    }

                    CreateTownCharacter(newNodes);
                }
               
            }
        }

        private void CreateTownCharacter(List<TownMovementNode> nodes)
        {
            TownCharacterView newCharacter = Instantiate(characterPrefab, charactersParent);
            TownCharacterSeed characterSeed = characterSeeds.GetRandomElement();
            activeRoamers.Add(newCharacter);
            StartCoroutine(newCharacter.InitializeRoamingCharacter(characterSeed, nodes));            
        }

        public void TearDown()
        {
            staticCharacters.ForEach((character) => 
            {
                character.Ucm.SetBaseAnim();
                character.gameObject.SetActive(false);
            });

            activeRoamers.ForEach(character =>
            {
                if (character != null)
                {
                    character.TearDown();
                }
            });

            activeRoamers.Clear();
            hiveIsActive = false;
        }
    }
}