using System.Collections.Generic;
using UnityEngine;

namespace Beautify.Universal {

	[DisallowMultipleComponent]
    [ExecuteAlways]
	public sealed class BeautifyCutOutDoFRenderer : MonoBehaviour {

		readonly List<Renderer> cachedRenderers = new List<Renderer>(32);

		void OnEnable () {
			gameObject.GetComponentsInChildren(true, cachedRenderers);
			BeautifyRendererFeature.BeautifyDoFTransparentMaskPass.RegisterCutOutRenderers(cachedRenderers);
		}

		void OnDisable () {
			BeautifyRendererFeature.BeautifyDoFTransparentMaskPass.UnregisterCutOutRenderers(cachedRenderers);
			cachedRenderers.Clear();
		}

	}

}


