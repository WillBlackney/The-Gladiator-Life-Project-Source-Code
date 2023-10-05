using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WeAreGladiators.Items
{
    public class ItemFilterUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private ItemGridScrollView[] targets;
        [Space(20)]

        [Header("Buttons")]
        [SerializeField] private Button filterAll;
        [SerializeField] private Button filterWeapons;
        [SerializeField] private Button filterHead;
        [SerializeField] private Button filterBody;
        [SerializeField] private Button filterTrinket;
        [SerializeField] private Button filterBooks;

        private bool initialized = false;
        private void OnEnable()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (initialized) return;

            initialized = true;
            filterAll.onClick.AddListener(()=> DispatchFilter(FilterSetting.All));
            filterWeapons.onClick.AddListener(() => DispatchFilter(FilterSetting.Weapons));
            filterHead.onClick.AddListener(() => DispatchFilter(FilterSetting.Head));
            filterBody.onClick.AddListener(() => DispatchFilter(FilterSetting.Body));
            filterTrinket.onClick.AddListener(() => DispatchFilter(FilterSetting.Trinket));
            filterBooks.onClick.AddListener(() => DispatchFilter(FilterSetting.AbilityBooks));
        }

        private void DispatchFilter(FilterSetting filterMode)
        {
            Debug.Log("ItemFilterUI.DispatchFilter() new filter = " + filterMode.ToString());
            targets.ForEach(t =>
            {
                t.SetFilter(filterMode);
                t.BuildInventoryView();
            });
        }


    }
}