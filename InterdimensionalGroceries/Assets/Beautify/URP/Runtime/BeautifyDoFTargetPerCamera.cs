using UnityEngine;

namespace Beautify.Universal {

    [ExecuteAlways, RequireComponent(typeof(Camera))]
    public class BeautifyDoFTargetPerCamera : MonoBehaviour {

        public Transform target;
        Camera _cam;

        void OnEnable () {
            _cam = GetComponent<Camera>();
            var s = BeautifySettings.instance;
            if (s != null) s.OnCameraBeforeAutofocus += Handle;
        }

        void OnDisable () {
            var s = BeautifySettings.instance;
            if (s != null) s.OnCameraBeforeAutofocus -= Handle;
        }

        void Handle (Camera cam, ref Transform current) {
            if (cam != _cam) return;
            if (target != null) current = target;
        }
    }

}