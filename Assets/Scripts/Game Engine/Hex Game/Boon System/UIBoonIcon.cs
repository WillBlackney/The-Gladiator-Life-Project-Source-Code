using HexGameEngine.UI;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HexGameEngine.Boons
{
    public class UIBoonIcon : MonoBehaviour
    {
        [Header("Core Components")]
        public GameObject visualParent;
        public Image boonImage;

        public BoonData MyBoonData { get; private set; }

        public void HideAndReset()
        {
            visualParent.SetActive(false);
            MyBoonData = null;
        }
        public void BuildAndShowFromBoonData(BoonData data)
        {
            MyBoonData = data;
            visualParent.SetActive(true);
            boonImage.sprite = data.BoonSprite;
        }
        public void MouseEnter()
        {
            if (MyBoonData != null)
                MainModalController.Instance.BuildAndShowModal(MyBoonData);
        }
        public void MouseExit()
        {
            MainModalController.Instance.HideModal();
        }
    }
}