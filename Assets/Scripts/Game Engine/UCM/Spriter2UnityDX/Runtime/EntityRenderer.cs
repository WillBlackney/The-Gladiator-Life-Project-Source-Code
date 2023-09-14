using System;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.UCM;

namespace Spriter2UnityDX
{
    [DisallowMultipleComponent] [ExecuteInEditMode] [AddComponentMenu("")]
    public class EntityRenderer : MonoBehaviour
    {
        public enum MaskInteractionn
        {
            None,
            VisibleInsideMask,
            VisibleOutsideMask
        }
        public bool ucm = true;

        [HideInInspector] public SpriteRenderer[] renderers = new SpriteRenderer [0];

        [SerializeField] [HideInInspector] private int sortingOrder;

        [SerializeField] [HideInInspector] private bool visibleOutsideMask;

        [SerializeField] [HideInInspector] private bool applySpriterZOrder;
        private SpriteRenderer _first;
        private SortingOrderUpdater[] updaters = new SortingOrderUpdater [0];
        private SpriteRenderer first
        {
            get
            {
                if (_first == null && renderers.Length > 0)
                {
                    _first = renderers[0];
                }
                return _first;
            }
        }
        public Color Color
        {
            get => first != null ? first.color : default(Color);
            set { DoForAll(x => x.color = value); }
        }

        public Material Material
        {
            get => first != null ? first.sharedMaterial : null;
            set { DoForAll(x => x.sharedMaterial = value); }
        }

        public int SortingLayerID
        {
            get => first != null ? first.sortingLayerID : 0;
            set { DoForAll(x => x.sortingLayerID = value); }
        }

        public string SortingLayerName
        {
            get => first != null ? first.sortingLayerName : null;
            set { DoForAll(x => x.sortingLayerName = value); }
        }
        public int SortingOrder
        {
            get => sortingOrder;
            set
            {
                sortingOrder = value;
                if (applySpriterZOrder)
                {
                    for (int i = 0; i < updaters.Length; i++)
                    {
                        updaters[i].SortingOrder = value;
                    }
                }
                else
                {
                    DoForAll(x => x.sortingOrder = value + x.GetComponent<UniversalCharacterModelElement>().sortingOrderBonus);
                    CharacterModel model = GetComponent<CharacterModel>();
                    CharacterModeller.AutoSetSortingOrderValues(model);
                    /*
                    if (ucm)
                    {
                        DoForAll(x => x.sortingOrder = value + x.GetComponent<UniversalCharacterModelElement>().sortingOrderBonus);
                        CharacterModel model = GetComponent<CharacterModel>();
                        CharacterModeller.AutoSetSortingOrderValues(model);
                    }*/

                }
            }
        }
        public bool VisibleWithinMask
        {
            get => visibleOutsideMask;
            set
            {
                visibleOutsideMask = value;

                if (visibleOutsideMask)
                {
                    DoForAll(x => x.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask);
                }
                else
                {
                    DoForAll(x => x.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None);
                }

            }
        }
        public bool ApplySpriterZOrder
        {
            get => applySpriterZOrder;
            set
            {
                applySpriterZOrder = value;
                if (applySpriterZOrder)
                {
                    List<SortingOrderUpdater> list = new List<SortingOrderUpdater>();
                    int spriteCount = renderers.Length;
                    foreach (SpriteRenderer renderer in renderers)
                    {
                        SortingOrderUpdater updater = renderer.GetComponent<SortingOrderUpdater>();
                        if (updater == null)
                        {
                            updater = renderer.gameObject.AddComponent<SortingOrderUpdater>();
                        }
                        updater.SortingOrder = sortingOrder;
                        updater.SpriteCount = spriteCount;
                        list.Add(updater);
                    }
                    updaters = list.ToArray();
                }
                else
                {
                    for (int i = 0; i < updaters.Length; i++)
                    {
                        if (Application.isPlaying)
                        {
                            Destroy(updaters[i]);
                        }
                        else
                        {
                            DestroyImmediate(updaters[i]);
                        }
                    }
                    updaters = new SortingOrderUpdater [0];
                    DoForAll(x => x.sortingOrder = sortingOrder);
                }
            }
        }

        private void Awake()
        {
            RefreshRenders();
        }
        private void Start()
        {
            int refreshValue = SortingOrder;
            SortingOrder = refreshValue;
        }

        private void OnEnable()
        {
            DoForAll(x => x.enabled = true);
        }

        private void OnDisable()
        {
            DoForAll(x => x.enabled = false);
        }

        public void DoForAll(Action<SpriteRenderer> action)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                action(renderers[i]);
            }
        }

        public void RefreshRenders()
        {
            renderers = GetComponentsInChildren<SpriteRenderer>(true);
            updaters = GetComponentsInChildren<SortingOrderUpdater>(true);
            int length = updaters.Length;
            for (int i = 0; i < length; i++)
            {
                updaters[i].SpriteCount = length;
            }
            _first = null;

        }
    }
}
