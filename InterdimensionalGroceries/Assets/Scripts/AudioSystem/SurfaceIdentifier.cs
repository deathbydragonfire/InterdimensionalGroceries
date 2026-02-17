using UnityEngine;

namespace InterdimensionalGroceries.AudioSystem
{
    public class SurfaceIdentifier : MonoBehaviour
    {
        [SerializeField] private SurfaceType surfaceType = SurfaceType.Default;

        public SurfaceType SurfaceType => surfaceType;
    }
}
