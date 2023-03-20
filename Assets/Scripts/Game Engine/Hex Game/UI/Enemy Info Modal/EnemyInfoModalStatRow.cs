using CardGameEngine;
using HexGameEngine.Perks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HexGameEngine.UI
{
    public class EnemyInfoModalStatRow : MonoBehaviour
    {
        [SerializeField] UIPerkIcon icon;
        [SerializeField] TextMeshProUGUI perkNameText;

        public void BuildAndShow(ActivePerk p)
        {
            gameObject.SetActive(true);
            icon.BuildFromActivePerk(p);
            perkNameText.text = TextLogic.SplitByCapitals(p.perkTag.ToString());
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}