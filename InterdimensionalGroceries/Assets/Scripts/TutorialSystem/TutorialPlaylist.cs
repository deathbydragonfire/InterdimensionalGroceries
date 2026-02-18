using UnityEngine;

namespace TutorialSystem
{
    [CreateAssetMenu(fileName = "TutorialPlaylist", menuName = "Tutorial/Tutorial Playlist")]
    public class TutorialPlaylist : ScriptableObject
    {
        public TutorialEvent[] events;
    }
}
