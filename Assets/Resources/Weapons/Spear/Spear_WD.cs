using System.Collections;
using Components;
using DG.Tweening;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using UnityEngine;
using UnityEngine.VFX;

public class Spear_WD : WeaponDesign
{
    // [SerializeField] private LayerMask _floorMask;
    //
    // [SerializeField] private AudioProcessor weaponAudioProcessor;
    //
    // [Header("Spear Settings")] [SerializeField]
    // private Transform _startSpawnPoint;
    //
    // [SerializeField] private float _spawnDuration;
    // [SerializeField] private Animation _spawnAnimation;
    // [SerializeField] private GameObject _spearObject;
    //
    // [Header("Aura")] [SerializeField] private GameObject _spearAura;
    //
    // [Header("VFX Objects")] [SerializeField]
    // private VisualEffect _portalVFX;
    //
    // [SerializeField] private float _portalSoundDelay;
    // private Coroutine _portalSoundCoroutine;
    //
    // [SerializeField] private float _portalOffsetY;
    // private Vector3 _portalSpawnLocalPos;
    //
    //
    // private bool _shouldPortalFollowHandPosStop;
    // private bool _shouldPortalFollowHandRotZStop;
    // private bool _shouldPortalFollowHandRotXStop;
    //
    // private const string TwirlStrength = "TwirlStrength";
    // private const string FeathDistance = "FeathDistance";
    // private const string FeathGradient = "FeathGradient";
    //
    //
    // private void Awake()
    // {
    //     _spearObject.SetActive(false);
    //     _portalSpawnLocalPos = _portalVFX.transform.localPosition;
    // }
    //
    // public override void OnFrameRecognized(string frameName)
    // {
    //     var frameId = GestureMapper.IndexOfName(frameName);
    //     Debug.Log("Design FrameRecognized " + frameId);
    //
    //     audioProcessor.ActivateResource("Frame_" + frameId);
    //
    //     switch (frameId)
    //     {
    //         case 0:
    //             transform.position = playerData.bodyAnchors.Body.position;
    //             transform.rotation = playerData.bodyAnchors.Body.rotation;
    //             RaycastHit hit;
    //             if (Physics.Raycast(_startSpawnPoint.position, Vector3.down, out hit, 10f, _floorMask))
    //             {
    //                 transform.position = hit.point;
    //             }
    //
    //             _portalVFX.gameObject.SetActive(true);
    //             _portalVFX.transform.localPosition = _portalSpawnLocalPos;
    //             _shouldPortalFollowHandPosStop = false;
    //             _shouldPortalFollowHandRotZStop = false;
    //             _shouldPortalFollowHandRotXStop = false;
    //             _portalSoundCoroutine = StartCoroutine(PlayPortalSound());
    //
    //             var endScale = _portalVFX.transform.localScale;
    //             DOVirtual.Vector3(Vector3.zero, endScale, 2f, v => _portalVFX.transform.localScale = v)
    //                 .SetEase(Ease.OutExpo);
    //             break;
    //         case 1:
    //             DOVirtual.Float(1, 8f, 3f, v => _portalVFX.SetFloat(TwirlStrength, v)).SetEase(Ease.InOutQuart);
    //             DOVirtual.Float(0, -2.24f, 2f, v => _portalVFX.SetFloat(FeathDistance, v)).SetEase(Ease.InOutQuad);
    //             break;
    //         case 3:
    //             StartCoroutine(PortalFollowHandPos());
    //             StartCoroutine(PortalFollowHandRot());
    //             break;
    //         case 4:
    //             _shouldPortalFollowHandRotXStop = true;
    //
    //             break;
    //         case 5:
    //             _shouldPortalFollowHandRotZStop = true;
    //             break;
    //         case 7:
    //             _shouldPortalFollowHandPosStop = true;
    //             break;
    //         case 8:
    //             _spearObject.transform.rotation = Quaternion.LookRotation(Vector3.up);
    //
    //             StartCoroutine(SpawnSpear());
    //             StartCoroutine(DestroyPortal());
    //             StopCoroutine(_portalSoundCoroutine);
    //             break;
    //     }
    // }
    //
    // private IEnumerator DestroyPortal()
    // {
    //     DOVirtual.Float(-2.24f, -8f, 1f, v => _portalVFX.SetFloat(FeathDistance, v)).SetEase(Ease.InQuart);
    //
    //     DOVirtual.Float(8, 1, 3f, v => _portalVFX.SetFloat(TwirlStrength, v)).SetEase(Ease.InOutQuart);
    //
    //     var startScale = _portalVFX.transform.localScale;
    //     DOVirtual.Vector3(startScale, Vector3.zero, 3f, v => _portalVFX.transform.localScale = v).SetEase(Ease.InExpo);
    //
    //     yield return new WaitForSeconds(3f);
    //     _portalVFX.gameObject.SetActive(false);
    // }
    //
    // private IEnumerator PlayPortalSound()
    // {
    //     WaitForSeconds delayWFS = new WaitForSeconds(_portalSoundDelay);
    //     while (true)
    //     {
    //         audioProcessor.ActivateResource("Portal");
    //         yield return delayWFS;
    //     }
    // }
    //
    // private IEnumerator PortalFollowHandPos()
    // {
    //     var targetPos = playerData.hands.rightHand.points[0].position.y + _portalOffsetY;
    //     var portalTransform = _portalVFX.transform;
    //
    //     while (!_shouldPortalFollowHandPosStop || Vector3.Distance(portalTransform.position,
    //                new Vector3(portalTransform.position.x, targetPos, portalTransform.position.z)) > 0.01f)
    //     {
    //         if (!_shouldPortalFollowHandPosStop)
    //         {
    //             targetPos = playerData.hands.rightHand.points[0].position.y + _portalOffsetY;
    //         }
    //
    //         var position = portalTransform.position;
    //         position = Vector3.Lerp(position, new Vector3(position.x, targetPos, position.z), 2f * Time.deltaTime);
    //         portalTransform.position = position;
    //
    //         yield return null;
    //     }
    // }
    //
    // private IEnumerator PortalFollowHandRot()
    // {
    //     float targetRotZ = playerData.hands.rightHand.points[0].rotation.eulerAngles.z;
    //     float targetRotX = playerData.hands.rightHand.points[0].rotation.eulerAngles.x;
    //
    //     while (!_shouldPortalFollowHandRotZStop ||
    //            Vector3.Distance(_portalVFX.transform.eulerAngles, new Vector3(targetRotX, 0, targetRotZ)) > 0.1f)
    //     {
    //         if (!_shouldPortalFollowHandRotZStop)
    //             targetRotZ = playerData.hands.rightHand.points[0].rotation.eulerAngles.z;
    //         else targetRotZ = -180f;
    //
    //         if (!_shouldPortalFollowHandRotXStop)
    //             targetRotX = playerData.hands.rightHand.points[0].rotation.eulerAngles.x;
    //         else targetRotX = 0f;
    //
    //
    //         Quaternion targetQuaternion = Quaternion.Euler(targetRotX, 0, targetRotZ);
    //         _portalVFX.transform.rotation =
    //             Quaternion.Slerp(_portalVFX.transform.rotation, targetQuaternion, 8f * Time.deltaTime);
    //         yield return null;
    //     }
    // }
    //
    // private IEnumerator SpawnSpear()
    // {
    //     yield return new WaitForSeconds(1 - _spawnDuration);
    //     _spearObject.SetActive(true);
    //     _spawnAnimation.Play();
    //
    //     yield return new WaitForSeconds(_spawnDuration);
    //
    //     _spearAura.SetActive(true);
    // }
    //
    // public override void OnGestureDetected()
    // {
    //     Debug.Log("GestureDetected");
    // }
    //
    // public override void OnHitHolds()
    // {
    //     //  Debug.Log("HitHolding");
    // }
    //
    // public override void OnHit()
    // {
    //     //Debug.Log("HitCalled");
    //     weaponAudioProcessor.ActivateRandomResource("Swing");
    // }
    //
    // public override void OnImpact(string affected)
    // {
    //     weaponAudioProcessor.ActivateRandomResource("Hit_" + affected);
    // }
    //
    // public override void OnAbilityReleased()
    // {
    //     Debug.Log("AbilityReleased");
    // }
    //
    // public override void OnGrabbed()
    // {
    //     base.OnGrabbed();
    //     _spearAura.SetActive(false);
    // }
    public override void OnReadyToBeCasted()
    {
      //  throw new System.NotImplementedException();
    }

    public override void OnCastCancelled()
    {
        throw new System.NotImplementedException();
    }

    public override void OnGestureCasted()
    {
        throw new System.NotImplementedException();
    }

    public override void OnActivated()
    {
        throw new System.NotImplementedException();
    }

    public override void OnHitStarted()
    {
        throw new System.NotImplementedException();
    }

    public override void OnHitStopped()
    {
        throw new System.NotImplementedException();
    }

    public override void OnDeactivated()
    {
        throw new System.NotImplementedException();
    }

    public override void OnAbilityDestroyed()
    {
        throw new System.NotImplementedException();
    }

    public override void OnFrameRecognized(string frameName)
    {
        throw new System.NotImplementedException();
    }

    public override void OnImpact(string affected)
    {
        throw new System.NotImplementedException();
    }
}