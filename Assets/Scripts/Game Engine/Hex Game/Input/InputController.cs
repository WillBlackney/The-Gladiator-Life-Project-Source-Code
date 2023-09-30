using UnityEngine;
using WeAreGladiators.Abilities;
using WeAreGladiators.Characters;
using WeAreGladiators.TurnLogic;
using WeAreGladiators.UI;

namespace WeAreGladiators.Utilities
{
    public class InputController : Singleton<InputController>
    {
        public ShowCharacterWorldUiState CharacterWorldUiState { get; private set; } = ShowCharacterWorldUiState.Always;
        private void Update()
        {
            if (GameController.Instance.GameState == GameState.CombatActive)
            {
                HexCharacterModel character = null;
                if (TurnController.Instance != null)
                {
                    character = TurnController.Instance.EntityActivated;
                }

                // Spell bar hotkeys
                if (character != null && character.controller == Controller.Player)
                {
                    bool didPress = false;
                    int key = 0;
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        key = 1;
                        didPress = true;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        key = 2;
                        didPress = true;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        key = 3;
                        didPress = true;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha4))
                    {
                        key = 4;
                        didPress = true;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha5))
                    {
                        key = 5;
                        didPress = true;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha6))
                    {
                        key = 6;
                        didPress = true;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha7))
                    {
                        key = 7;
                        didPress = true;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha8))
                    {
                        key = 8;
                        didPress = true;
                    }

                    else if (Input.GetKeyDown(KeyCode.Alpha9))
                    {
                        key = 9;
                        didPress = true;
                    }

                    else if (Input.GetKeyDown(KeyCode.Alpha0))
                    {
                        key = 10;
                        didPress = true;
                    }

                    if (didPress && CombatUIController.Instance.AbilityButtons[key - 1] != null)
                    {
                        HexCharacterModel uiCharacter = TurnController.Instance.EntityActivated;
                        if (uiCharacter.controller == Controller.Player)
                        {
                            AbilityData ab = CombatUIController.Instance.AbilityButtons[key - 1].MyAbilityData;
                            int cost = AbilityController.Instance.GetAbilityActionPointCost(uiCharacter, ab);
                            CombatUIController.Instance.EnergyBar.OnAbilityButtonMouseEnter(uiCharacter.currentActionPoints, cost);
                            AbilityController.Instance.OnAbilityButtonClicked(CombatUIController.Instance.AbilityButtons[key - 1]);
                        }

                    }

                }

                // Toggle character world space GUI
                if (Input.GetKeyDown(KeyCode.LeftAlt))
                {
                    OnAltKeyPressed();
                }

                // End turn with E
                if (TurnController.Instance != null && Input.GetKeyDown(KeyCode.E))
                {
                    TurnController.Instance.OnEndTurnButtonClicked();
                }

                // Delay turn with SPACE
                if (TurnController.Instance != null && Input.GetKeyDown(KeyCode.Space))
                {
                    TurnController.Instance.OnDelayTurnButtonClicked();
                }
            }

        }

        public void OnAltKeyPressed()
        {
            if (CharacterWorldUiState == ShowCharacterWorldUiState.Always)
            {
                CharacterWorldUiState = ShowCharacterWorldUiState.OnMouseOver;

                // hide all character world UI's
                foreach (HexCharacterModel c in HexCharacterController.Instance.AllCharacters)
                {
                    HexCharacterController.Instance.FadeOutCharacterWorldCanvas(c.hexCharacterView, null, 0.25f, 0.001f);
                }
            }
            else if (CharacterWorldUiState == ShowCharacterWorldUiState.OnMouseOver)
            {
                CharacterWorldUiState = ShowCharacterWorldUiState.Always;

                // show all character UI's
                foreach (HexCharacterModel c in HexCharacterController.Instance.AllCharacters)
                {
                    HexCharacterController.Instance.FadeInCharacterWorldCanvas(c.hexCharacterView, null, 0.25f);
                }
            }
        }
    }

    public enum ShowCharacterWorldUiState
    {
        OnMouseOver = 0,
        Always = 1
    }

   
}
