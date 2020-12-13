using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NDR2ndTTB.UI
{
    public class UIManager : MonoBehaviour
    {
        #region Singleton
        public static UIManager instance;
        private void Awake()
        {
            if (!instance) instance = this;
        }
        #endregion



        public GameObject endTurnBtn;
        [SerializeField] GameObject mouseFollower;
        [SerializeField] Text actionPointsTxt;

        private void Update()
        {
            mouseFollower.transform.position = Input.mousePosition;
        }

        public void PressedBtn(EUIButtonType button)
        {
            switch (button)
            {
                case EUIButtonType.EndTurn:
                    GameManager.instance.EndTurn();
                    break;
                case EUIButtonType.ChangeStanceUp:
                    GameManager.instance.ChangeStance(true);
                  
                    break;
                case EUIButtonType.ChangeStanceDown:
                    GameManager.instance.ChangeStance(false);

                    break;
                default:
                    break;
            }
        }

        public void UpdateActionPointsIndicator(int value)
        {
            actionPointsTxt.text = value.ToString();
        }
    }
}