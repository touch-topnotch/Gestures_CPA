using UnityEngine;

namespace Scripts.Adapters
{
    public class C_OVRManager : MonoBehaviour
    {
        [Header("Performance/Quality")]
       
        [Tooltip("If true, Unity will use the optimal antialiasing level for quality/performance on the current hardware.")]
         
        /// <summary>
        /// If true, Unity will use the optimal antialiasing level for quality/performance on the current hardware.
        /// </summary>
        
        public bool useRecommendedMSAALevel = true;

        private static bool _isHmdPresentCached = false;
        private static bool _isHmdPresent = false;
        private static bool _wasHmdPresent = false;

        /// <summary>
        /// If true, a head-mounted display is connected and present.
        /// </summary>
        public static bool isHmdPresent
        {
            get
            {
                if (!_isHmdPresentCached)
                {
                    _isHmdPresentCached = true;
                    _isHmdPresent = OVRNodeStateProperties.IsHmdPresent();
                }

                return _isHmdPresent;
            }

            private set
            {
                _isHmdPresentCached = true;
                _isHmdPresent = value;
            }
        }

        /// <summary>
        /// If true, both eyes will see the same image, rendered from the center eye pose, saving performance.
        /// </summary>
        [SerializeField]
        [Tooltip("If true, both eyes will see the same image, rendered from the center eye pose, saving performance.")]
        private bool _monoscopic = false;

        public bool monoscopic
        {
            get
            {
                if (!isHmdPresent)
                    return _monoscopic;

                return OVRPlugin.monoscopic;
            }

            set
            {
                if (!isHmdPresent)
                    return;

                OVRPlugin.monoscopic = value;
                _monoscopic = value;
            }
        }

        [SerializeField] [Tooltip("The sharpen filter of the eye buffer. This amplifies contrast and fine details.")]
        private OVRPlugin.LayerSharpenType _sharpenType = OVRPlugin.LayerSharpenType.None;

        /// <summary>
        /// The sharpen type for the eye buffer
        /// </summary>
        public OVRPlugin.LayerSharpenType sharpenType
        {
            get { return _sharpenType; }
            set
            {
                _sharpenType = value;
                OVRPlugin.SetEyeBufferSharpenType(_sharpenType);
            }
        }

        [HideInInspector] private OVRManager.ColorSpace _colorGamut = OVRManager.ColorSpace.P3;

        /// <summary>
        /// The target color gamut the HMD will perform a color space transformation to
        /// </summary>
        public OVRManager.ColorSpace colorGamut
        {
            get { return _colorGamut; }
            set
            {
                _colorGamut = value;
                OVRPlugin.SetClientColorDesc((OVRPlugin.ColorSpace)_colorGamut);
            }
        }

        /// <summary>
        /// The native color gamut of the target HMD
        /// </summary>
        public OVRManager.ColorSpace nativeColorGamut
        {
            get { return (OVRManager.ColorSpace)OVRPlugin.GetHmdColorDesc(); }
        }

        [SerializeField]
        [Tooltip("Enable Dynamic Resolution. This will allocate render buffers to maxDynamicResolutionScale size and " +
                 "will change the viewport to adapt performance.")]
        public bool enableDynamicResolution = false;

        [SerializeField]
        [Tooltip("Minimum scaling factor used when dynamic resolution is enabled.")]
        [RangeAttribute(0.7f, 1.3f)]
        public float minDynamicResolutionScale = 1.0f;

        [SerializeField]
        [Tooltip("Maximum scaling factor used when dynamic resolution is enabled.")]
        [RangeAttribute(0.7f, 1.3f)]
        public float maxDynamicResolutionScale = 1.0f;

        private const int _pixelStepPerFrame = 32;

        /// <summary>
        /// Adaptive Resolution is based on Unity engine's renderViewportScale/eyeTextureResolutionScale feature
        /// But renderViewportScale was broken in an array of Unity engines, this function help to filter out those broken engines
        /// </summary>
        ///
        [System.Obsolete("Deprecated. Use Dynamic Render Scaling instead.", false)]
        public static bool IsAdaptiveResSupportedByEngine()
        {
            return true;
        }

        /// <summary>
        /// Min RenderScale the app can reach under adaptive resolution mode ( enableAdaptiveResolution = true );
        /// </summary>
        [RangeAttribute(0.5f, 2.0f)]
        [HideInInspector]
        [Tooltip("Min RenderScale the app can reach under adaptive resolution mode")]
        [System.Obsolete("Deprecated. Use minDynamicRenderScale instead.", false)]
        public float minRenderScale = 0.7f;

        /// <summary>
        /// Max RenderScale the app can reach under adaptive resolution mode ( enableAdaptiveResolution = true );
        /// </summary>
        [RangeAttribute(0.5f, 2.0f)]
        [HideInInspector]
        [Tooltip("Max RenderScale the app can reach under adaptive resolution mode")]
        [System.Obsolete("Deprecated. Use maxDynamicRenderScale instead.", false)]
        public float maxRenderScale = 1.0f;

        /// <summary>
        /// Set the relative offset rotation of head poses
        /// </summary>
        [SerializeField] [Tooltip("Set the relative offset rotation of head poses")]
        private Vector3 _headPoseRelativeOffsetRotation;

        public Vector3 headPoseRelativeOffsetRotation
        {
            get { return _headPoseRelativeOffsetRotation; }
            set
            {
                OVRPlugin.Quatf rotation;
                OVRPlugin.Vector3f translation;
                if (OVRPlugin.GetHeadPoseModifier(out rotation, out translation))
                {
                    Quaternion finalRotation = Quaternion.Euler(value);
                    rotation = finalRotation.ToQuatf();
                    OVRPlugin.SetHeadPoseModifier(ref rotation, ref translation);
                }

                _headPoseRelativeOffsetRotation = value;
            }
        }

        /// <summary>
        /// Set the relative offset translation of head poses
        /// </summary>
        [SerializeField] [Tooltip("Set the relative offset translation of head poses")]
        private Vector3 _headPoseRelativeOffsetTranslation;

        public Vector3 headPoseRelativeOffsetTranslation
        {
            get { return _headPoseRelativeOffsetTranslation; }
            set
            {
                OVRPlugin.Quatf rotation;
                OVRPlugin.Vector3f translation;
                if (OVRPlugin.GetHeadPoseModifier(out rotation, out translation))
                {
                    if (translation.FromFlippedZVector3f() != value)
                    {
                        translation = value.ToFlippedZVector3f();
                        OVRPlugin.SetHeadPoseModifier(ref rotation, ref translation);
                    }
                }

                _headPoseRelativeOffsetTranslation = value;
            }
        }

        /// <summary>
        /// The TCP listening port of Oculus Profiler Service, which will be activated in Debug/Developerment builds
        /// When the app is running on editor or device, open "Tools/Oculus/Oculus Profiler Panel" to view the realtime system metrics
        /// </summary>
        public int profilerTcpPort = OVRSystemPerfMetrics.TcpListeningPort;
    }
}