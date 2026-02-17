using UnityEngine;

namespace InterdimensionalGroceries.BuildSystem
{
    public class BuildModeDebug : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (BuildModeController.Instance != null)
                {
                    Debug.Log("=== BUILD MODE DEBUG ===");
                    Debug.Log($"Current State: {BuildModeController.Instance.CurrentState}");
                    Debug.Log($"Is Active: {BuildModeController.Instance.IsActive}");
                }
            }
        }
    }
}
