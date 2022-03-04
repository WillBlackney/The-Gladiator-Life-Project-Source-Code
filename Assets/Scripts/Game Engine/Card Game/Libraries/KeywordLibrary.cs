using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using System;
#endif

namespace CardGameEngine
{
    public class KeywordLibrary : Singleton<KeywordLibrary>
    {
        [Header("All Key Word Data")]
        public KeyWordData[] allKeyWordData;

        [Header("All Racial Data")]
        public RacialData[] allRacialData;

        public RacialData GetRacialData(CharacterRace race)
        {
            RacialData dataReturned = null;
            foreach (RacialData data in allRacialData)
            {
                if (data.race == race)
                {
                    dataReturned = data;
                    break;
                }
            }

            return dataReturned;
        }

    }


}