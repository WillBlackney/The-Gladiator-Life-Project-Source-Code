using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Perks
{
    public class PerkLayoutPanel : MonoBehaviour
    {
        [SerializeField] private bool showBothModals;
        public List<PerkIconView> PerkIcons { get; } = new List<PerkIconView>();

        // Update Passive Icons and Panel View
        #region

        public void HandleAddNewIconToPanel(PerkIconData iconData, int stacksGainedOrLost)
        {
            if (PerkIcons.Count > 0)
            {
                bool matchFound = false;

                foreach (PerkIconView icon in PerkIcons)
                {
                    if (iconData.passiveName == icon.StatusName)
                    {
                        // Icon already exists in character's list
                        UpdatePassiveIconOnPanel(icon, stacksGainedOrLost);
                        matchFound = true;
                        break;
                    }
                }

                if (matchFound == false)
                {
                    AddNewPassiveIconToPanel(iconData, stacksGainedOrLost);
                }
            }
            else
            {
                AddNewPassiveIconToPanel(iconData, stacksGainedOrLost);
            }
        }
        private void AddNewPassiveIconToPanel(PerkIconData iconData, int stacksGained)
        {
            // only create an icon if the the effects' stacks are at least 1 or -1
            if (stacksGained != 0)
            {
                GameObject newIconGO = Instantiate(PrefabHolder.Instance.PassiveIconViewPrefab, gameObject.transform);
                PerkIconView newIcon = newIconGO.GetComponent<PerkIconView>();
                newIcon.Build(iconData, showBothModals);
                newIcon.ModifyIconViewStacks(stacksGained);
                PerkIcons.Add(newIcon);
            }

        }
        private void RemovePassiveIconFromPanel(PerkIconView iconToRemove)
        {
            PerkIcons.Remove(iconToRemove);
            Destroy(iconToRemove.gameObject);
        }
        private void UpdatePassiveIconOnPanel(PerkIconView iconToUpdate, int stacksGainedOrLost)
        {
            iconToUpdate.ModifyIconViewStacks(stacksGainedOrLost);
            if (iconToUpdate.StatusStacks == 0)
            {
                RemovePassiveIconFromPanel(iconToUpdate);
            }

        }
        public void ResetPanel()
        {
            foreach (PerkIconView p in PerkIcons)
            {
                Destroy(p.gameObject);
            }
            PerkIcons.Clear();
        }
        public void BuildFromPerkManager(PerkManagerModel perkManager)
        {
            foreach (ActivePerk ap in perkManager.perks)
            {
                HandleAddNewIconToPanel(ap.Data, ap.stacks);
            }
        }

        #endregion
    }
}
