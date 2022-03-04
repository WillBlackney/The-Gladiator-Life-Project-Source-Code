using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CardGameEngine
{
    [Serializable]
    public class SerializedItemManagerModel
    {
        public ItemDataSO mainHandItem;
        public ItemDataSO offHandItem;
        public ItemDataSO trinketOne;
        public ItemDataSO trinketTwo;
        public ItemDataSO trinketThree;
    }
}