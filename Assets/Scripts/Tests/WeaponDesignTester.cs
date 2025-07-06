
using Components;
using Scripts.PlayerLogic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Tests
{
    [RequireComponent(typeof(WeaponDesign))][ExecuteInEditMode]
    public class WeaponDesignTester: MonoBehaviour
    {
        public Player debugPLayer;
        public int frameIndex;
        private WeaponDesign _weaponDesign;
        [SerializeField][Tooltip("The UPDATE method of your WD should be overriden")]
        private bool simulateUpdate = false;

        [Button("OnFrameRecognized", ButtonSizes.Medium)]
        public void TestOnFrameRecognized()
        {
            _weaponDesign ??= GetComponent<WeaponDesign>();
            _weaponDesign.playerData ??= debugPLayer.GetRawPlayerData();
            _weaponDesign.OnFrameRecognized(name.Split('_')[0] + '_' + frameIndex);
        }
        [Button("Next Frame", ButtonSizes.Medium)]
        public void TestOnNextFrameRecognized()
        {
            ++frameIndex;
            TestOnFrameRecognized();
        }
        [Button("Previous Frame", ButtonSizes.Medium)]
        public void TestOnPrevFrameRecognized()
        {
            --frameIndex;
            TestOnFrameRecognized();
        }
        
        [Button("OnGestureCasted", ButtonSizes.Medium)]
        public void TestOnGestureCasted()
        {
            _weaponDesign ??= GetComponent<WeaponDesign>();
            _weaponDesign.playerData ??= debugPLayer.GetRawPlayerData();
            _weaponDesign.OnGestureCasted();
        }
        [Button("OnReadyToBeCasted", ButtonSizes.Medium)]
        public void TestOnReadyToBeCasted()
        {
            _weaponDesign ??= GetComponent<WeaponDesign>();
            _weaponDesign.playerData ??= debugPLayer.GetRawPlayerData();
            _weaponDesign.OnReadyToBeCasted();
        }
        [Button("OnCastCancelled", ButtonSizes.Medium)]
        public void TestOnCastCancelled()
        {
            _weaponDesign ??= GetComponent<WeaponDesign>();
            _weaponDesign.playerData ??= debugPLayer.GetRawPlayerData();
            _weaponDesign.OnCastCancelled();
        }
        [Button("OnActivated", ButtonSizes.Medium)]
        public void TestOnActivated()
        {
            _weaponDesign ??= GetComponent<WeaponDesign>();
            _weaponDesign.playerData ??= debugPLayer.GetRawPlayerData();
            _weaponDesign.OnActivated();
        }
        [Button("OnHitStarted", ButtonSizes.Medium)]
        public void TestOnHitStarted()
        {
            _weaponDesign ??= GetComponent<WeaponDesign>();
            _weaponDesign.playerData ??= debugPLayer.GetRawPlayerData();
            _weaponDesign.OnHitStarted();
        }
        [Button("OnHitStopped", ButtonSizes.Medium)]
        public void TestOnHitStopped()
        {
            _weaponDesign ??= GetComponent<WeaponDesign>();
            _weaponDesign.playerData ??= debugPLayer.GetRawPlayerData();
            _weaponDesign.OnHitStopped();
        }
        [Button("OnDeactivated", ButtonSizes.Medium)]
        public void TestOnDeactivated()
        {
            _weaponDesign ??= GetComponent<WeaponDesign>();
            _weaponDesign.playerData ??= debugPLayer.GetRawPlayerData();
            _weaponDesign.OnDeactivated();
        }
        [Button("OnAbilityDestroyed", ButtonSizes.Medium)]
        public void TestOnAbilityDestroyed()
        {
            _weaponDesign ??= GetComponent<WeaponDesign>();
            _weaponDesign.playerData ??= debugPLayer.GetRawPlayerData();
            _weaponDesign.OnAbilityDestroyed();
        }
        [Button("OnImpact", ButtonSizes.Medium)]
        public void TestOnImpact()
        {
            _weaponDesign ??= GetComponent<WeaponDesign>();
            _weaponDesign.playerData ??= debugPLayer.GetRawPlayerData();
            _weaponDesign.OnImpact("test");
        }
        

        private void Update()
        {
            if (simulateUpdate && _weaponDesign && _weaponDesign.playerData != null)
            {
                _weaponDesign.Update();
            }
        }
        
    }
}