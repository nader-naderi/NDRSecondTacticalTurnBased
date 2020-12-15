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



        public GameObject charcaterGrid;
        [SerializeField] RectTransform characterGridTrans;
        public GameObject smallPortraitTemplte;

        public GameObject CharacterPanel;
        public Image portraitBig;
        public Text apIdicator;

        public Slider health;
        public Slider stamina;
        public Slider morale;

        public Image stanceIcon;


        public GameObject endTurnBtn;
        [SerializeField] GameObject mouseFollower;
        [SerializeField] Text actionPointsTxt;

        public void AddSmallPortrait(UnitStats stats)
        {
            Character character = ResourcesManager.instance.GetCharacter(stats.CharID);
            GameObject g = Instantiate(smallPortraitTemplte, characterGridTrans) as GameObject;
            g.SetActive(true);
            //g.transform.SetParent(charcaterGrid.transform);
            GridLayoutGroup grid = charcaterGrid.GetComponent<GridLayoutGroup>();

            Vector2 newPos = characterGridTrans.anchoredPosition;
            newPos.y += grid.cellSize.y + grid.spacing.y;

            //characterGridTrans.anchoredPosition = newPos;

            SmallPortrait sm = g.GetComponent<SmallPortrait>();
            sm.Portrait.sprite = character.portraiteIcon;

            sm.CharID = stats.CharID;
        }

        private void Update()
        {
            mouseFollower.transform.position = Input.mousePosition;
        }

        public void UpdateCharacterPanel(UnitStats stats)
        {
            Character character = ResourcesManager.instance.GetCharacter(stats.CharID);
            portraitBig.sprite = character.portraiteIcon;
            health.maxValue = stats.health;
            stamina.maxValue = stats.agility;
            health.value = stats.CurrentHealth;
            stamina.value = stats.agility;
            UpdateAponCharacterPanel(stats.CurrentActionPoints);
            CharacterPanel.SetActive(true);
        }

        public void PressedBtn(EUIButtonType button)
        {
            GameManager gameManager = GameManager.instance;

            switch (button)
            {
                case EUIButtonType.EndTurn:
                    GameManager.instance.EndTurn();
                    break;
                case EUIButtonType.Up:
                    gameManager.ChangeStance(EStance.Normal);
                  
                    break;
                case EUIButtonType.Crouch:
                    gameManager.ChangeStance(EStance.Crouch);
                    break;
                case EUIButtonType.Run:
                    gameManager.ChangeStance(EStance.Run);
                    break;
                case EUIButtonType.Prone:
                    gameManager.ChangeStance(EStance.Prone);
                    break;
                default:
                    break;
            }
        }

        public void UpdateActionPointsIndicator(int value)
        {
            string v = value.ToString();
            actionPointsTxt.text = v;
            apIdicator.text = v;
        }

        public void UpdateAponCharacterPanel(int value)
        {
            string v = value.ToString();
            apIdicator.text = v;
        }

    }
}