using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGameEngine
{
    public class EndTurnButton : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                /*
                if(ActivationManager.Instance != null)
                    ActivationManager.Instance.OnEndTurnButtonClicked();
                */

                if (HexGameEngine.TurnLogic.TurnController.Instance != null)
                    HexGameEngine.TurnLogic.TurnController.Instance.OnEndTurnButtonClicked();
            }
        }
    }
}