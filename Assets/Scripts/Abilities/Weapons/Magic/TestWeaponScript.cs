using UnityEngine.UI;

namespace Scripts.Weapons.Magic
{
    public class TestWeaponScript:Weapon

    {
        public Button testCastStart;
        public Button testFrameRecognized;
        public Button testAbilityActivated;

        protected override void OnInitialized()
        {
            testCastStart.onClick.AddListener(
                () => {ReadyToBeCastedEvent.Invoke();});
            testFrameRecognized.onClick.AddListener(
                ()=>{FrameRecognizedEvent.Invoke("piska");});
        }
    }
}