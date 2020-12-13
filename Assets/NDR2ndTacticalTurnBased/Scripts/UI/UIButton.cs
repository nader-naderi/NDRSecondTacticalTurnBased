using UnityEngine;
using System.Collections;

namespace NDR2ndTTB.UI
{
    public class UIButton : MonoBehaviour
    {
        public EUIButtonType buttonType;

        public void Press()
        {
            UIManager.instance.PressedBtn(buttonType);
        }
    }
}