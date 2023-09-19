using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Audio;
using WeAreGladiators.Characters;
using WeAreGladiators.UCM;
using WeAreGladiators.UI;

namespace WeAreGladiators.TownFeatures
{
    public class DeploymentNodeView : MonoBehaviour
    {

        // Conditional Checks
        #region

        public bool IsUnoccupied()
        {
            return MyCharacterData == null;
        }

        #endregion

        // Components + Properties
        #region

        [Header("Components")]
        [SerializeField]
        private Vector2 gridPosition;
        [SerializeField] private Image portraitSprite;
        [SerializeField] private GameObject portraitVisualParent;
        [SerializeField] private UniversalCharacterModel portraitModel;
        [SerializeField] private Allegiance allowedCharacter;

        // Non inspector values

        #endregion

        // Getters + Accessors
        #region

        public static DeploymentNodeView NodeMousedOver { get; private set; }
        public Allegiance AllowedCharacter => allowedCharacter;
        public HexCharacterData MyCharacterData { get; private set; }
        public Vector2 GridPosition => gridPosition;

        #endregion

        // Input
        #region

        public void OnRightClick()
        {
            if (MyCharacterData != null && allowedCharacter == Allegiance.Player)
            {
                AudioManager.Instance.PlaySound(Sound.UI_Drag_Drop_End);
                SetUnoccupiedState();
                TownController.Instance.UpdateCharactersDeployedText();
            }
        }
        public void OnLeftClick()
        {
            if (MyCharacterData != null &&
                allowedCharacter == Allegiance.Enemy &&
                !EnemyInfoPanel.Instance.PanelIsActive &&
                !CharacterRosterViewController.Instance.MainVisualParent.activeSelf)
            {
                AudioManager.Instance.PlaySound(Sound.UI_Button_Click);
                EnemyInfoPanel.Instance.HandleBuildAndShowPanel(MyCharacterData);
            }
        }
        public void LeftMouseDown()
        {
            if (allowedCharacter == Allegiance.Player)
            {
                PortraitDragController.Instance.OnDeploymentNodeDragStart(this);
            }
        }
        public void MouseEnter()
        {
            Debug.Log("MouseEnter");
            NodeMousedOver = this;
        }
        public void MouseExit()
        {
            Debug.Log("MouseExit");
            NodeMousedOver = null;
        }

        #endregion

        // Logic
        #region

        public void BuildFromCharacterData(HexCharacterData character)
        {
            Debug.Log("DeploymentNodeView.BuildFromCharacterData() character = " + character.myName);
            portraitVisualParent.SetActive(true);
            MyCharacterData = character;

            if (MyCharacterData.ModelPrefab != null)
            {
                portraitSprite.gameObject.SetActive(true);
                portraitModel.gameObject.SetActive(false);
                portraitSprite.sprite = MyCharacterData.ModelPrefab.PortraitSprite;
            }
            else
            {
                portraitSprite.gameObject.SetActive(false);
                portraitModel.gameObject.SetActive(true);

                // Build model mugshot
                CharacterModeller.BuildModelFromStringReferencesAsMugshot(portraitModel, character.modelParts);
                CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, portraitModel);
            }

        }
        public void SetUnoccupiedState()
        {
            MyCharacterData = null;
            portraitVisualParent.SetActive(false);
        }

        #endregion
    }
}
