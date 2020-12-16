using UnityEngine;
using System.Collections;

namespace NDR2ndTTB.UI
{
    public class Tooltip : MonoBehaviour
    {
        public string descriptionIndex;
        public void OnPointEnter()
        {
            UIManager.instance.UpdateTooltipIndicator(descriptionIndex);
        }

        public void OnPointerExit()
        {
            UIManager.instance.CloseTooltipIndicator();
        }
    }
}