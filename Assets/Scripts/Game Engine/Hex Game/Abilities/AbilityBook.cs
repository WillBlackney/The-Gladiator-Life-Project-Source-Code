using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Items;
using HexGameEngine.Utilities;

namespace HexGameEngine.Abilities
{
    public class AbilityBook
    {
        #region Properties
        public List<AbilityData> activeAbilities = new List<AbilityData>();
        public List<AbilityData> knownAbilities = new List<AbilityData>();
        #endregion

        #region Learn + Unlearn Abilities
        public void HandleLearnNewAbility(AbilityData ability)
        {
            if (KnowsAbility(ability.abilityName)) return;

            AbilityData newAbility = ObjectCloner.CloneJSON(ability);

            knownAbilities.Add(newAbility);

            if (activeAbilities.Count < 10) activeAbilities.Add(newAbility);
        }
        public void HandleLearnNewAbilities(List<AbilityData> abilities)
        {
            foreach(AbilityData a in abilities)
            {
                HandleLearnNewAbility(a);
            }
        }
        public void HandleUnlearnAbility(string abilityName)
        {
            AbilityData abilityUnlearnt = null;
            foreach (AbilityData a in knownAbilities)
            {
                if (a.abilityName == abilityName)
                {
                    abilityUnlearnt = a;
                    break;
                }
            }

            knownAbilities.Remove(abilityUnlearnt);
            if (activeAbilities.Contains(abilityUnlearnt)) activeAbilities.Remove(abilityUnlearnt);
        }
        public void SetAbilityAsActive(AbilityData a)
        {
            knownAbilities.Add(a);
        }
        public void SetAbilityAsInActive(AbilityData a)
        {
            knownAbilities.Remove(a);
        }
        public void ForgetAllAbilities()
        {
            activeAbilities.Clear();
            knownAbilities.Clear();
        }
        #endregion

        #region Conditional Checks
        public bool KnowsAbility(string abilityName)
        {
            bool ret = false;
            foreach (AbilityData a in knownAbilities)
            {
                if (a.abilityName == abilityName)
                {
                    ret = true;
                    break;
                }
            }

            return ret;
        }
        #endregion

        #region Item Ability Logic
        public List<AbilityData> GenerateAbilitiesFromWeapons(ItemSet itemSet, bool includeLoadOutAbility = true)
        {
            List<AbilityData> ret = new List<AbilityData>();

            // Build main hand weapon abilities
            if (itemSet.mainHandItem != null)
            {
                foreach (AbilityData d in itemSet.mainHandItem.grantedAbilities)
                {
                    // Characters dont gain special weapon ability if they have an off hand weapon
                    if (itemSet.offHandItem == null ||
                        (itemSet.offHandItem != null && d.weaponAbilityType == WeaponAbilityType.Basic) ||
                        (itemSet.offHandItem != null && itemSet.offHandItem.weaponClass == itemSet.mainHandItem.weaponClass))
                    {
                        ret.Add(d);
                    }
                }
            }

            // build off hand weapon abilities
            if (itemSet.offHandItem != null)
            {
                foreach (AbilityData d in itemSet.offHandItem.grantedAbilities)
                {
                    if (itemSet.offHandItem.weaponClass != itemSet.mainHandItem.weaponClass)
                        ret.Add(d);
                }
            }

            if (includeLoadOutAbility)
            {
                AbilityData loadOutAbility = GetLoadoutAbilityDataFromItemSet(itemSet);
                if (loadOutAbility != null)
                    ret.Add(loadOutAbility);
            }

            Debug.Log("AbilityBook.GenerateAbilitiesFromWeapons() generated abilities: " + ret.Count.ToString());
            return ret;
        }
        private AbilityData GetLoadoutAbilityDataFromItemSet(ItemSet itemSet)
        {
            AbilityData loadOutAbility = null;
            // 2H melee: All In
            if (itemSet.mainHandItem != null &&
                itemSet.mainHandItem.IsMeleeWeapon &&
                itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded)
            {

            }
            // Dual wielding 1h: Twin Strike
            if (itemSet.mainHandItem != null &&
                itemSet.mainHandItem.IsMeleeWeapon &&
                itemSet.mainHandItem.handRequirement == HandRequirement.OneHanded &&
                itemSet.offHandItem != null &&
                itemSet.offHandItem.IsMeleeWeapon &&
                itemSet.offHandItem.handRequirement == HandRequirement.OneHanded)
            {

            }

            // 1h melee weapon: shove
            if (itemSet.mainHandItem != null &&
                itemSet.mainHandItem.IsMeleeWeapon &&
                itemSet.mainHandItem.handRequirement == HandRequirement.OneHanded &&
                itemSet.offHandItem == null)
            {

            }
            return loadOutAbility;
        }
        public void HandleLearnAbilitiesFromItemSet(ItemSet itemSet)
        {
            HandleUnlearnAbilitiesFromCurrentItemSet();

            foreach (AbilityData a in GenerateAbilitiesFromWeapons(itemSet))
            {
                HandleLearnNewAbility(a);
            }

        }
        public void HandleUnlearnAbilitiesFromCurrentItemSet()
        {
            List<AbilityData> abilitiesRemoved = new List<AbilityData>();

            foreach (AbilityData a in knownAbilities)
            {
                if (a.derivedFromWeapon || a.derivedFromItemLoadout)
                    abilitiesRemoved.Add(a);
            }

            foreach (AbilityData a in abilitiesRemoved)
            {
                knownAbilities.Remove(a);
                if (activeAbilities.Contains(a))
                    activeAbilities.Remove(a);
            }
        }
        #endregion

        #region Misc
        public AbilityData GetKnownAbility(string abilityName)
        {
            AbilityData ret = null;

            foreach(AbilityData a in knownAbilities)
            {
                if(a.abilityName == abilityName)
                {
                    ret = a;
                    break;
                }
            }

            return ret;
        }
        public List<AbilityData> GetAllKnownNonItemSetAbilities()
        {
            List<AbilityData> ret = new List<AbilityData>();

            foreach(AbilityData a in knownAbilities)
            {
                if (a.derivedFromItemLoadout == false && a.derivedFromWeapon == false)
                    ret.Add(a);
            }

            return ret;
        }
        #endregion

        #region Constructors
        public AbilityBook()
        {

        }
        public AbilityBook(AbilityBook original)
        {
            foreach (AbilityData a in original.knownAbilities)
                HandleLearnNewAbility(a);
        }
        public AbilityBook(SerializedAbilityBook original)
        {
            foreach (AbilityDataSO a in original.activeAbilities)
                 HandleLearnNewAbility(AbilityController.Instance.BuildAbilityDataFromScriptableObjectData(a));
        }
        #endregion
    }

    [System.Serializable]
    public class SerializedAbilityBook
    {
        // Class is used to assign abilities to characters templates / enemy data models in the inspector
        public List<AbilityDataSO> activeAbilities = new List<AbilityDataSO>();
    }
}
