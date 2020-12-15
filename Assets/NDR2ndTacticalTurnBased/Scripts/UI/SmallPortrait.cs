using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace NDR2ndTTB
{
    public class SmallPortrait : MonoBehaviour
    {
        [SerializeField] string charID;
        [SerializeField] Image portrait;
        [SerializeField] Slider health;
        [SerializeField] Slider stamina;
        [SerializeField] Slider morale;
        [SerializeField] Text ap;

        public Image Portrait { get => portrait; set => portrait = value; }
        public Slider Health { get => health; set => health = value; }
        public Slider Stamina { get => stamina; set => stamina = value; }
        public Slider Morale { get => morale; set => morale = value; }
        public string CharID { get => charID; set => charID = value; }

        public void OnPress()
        {
            GameManager.instance.ActivateUnitWithID(charID);
        }
    }
}