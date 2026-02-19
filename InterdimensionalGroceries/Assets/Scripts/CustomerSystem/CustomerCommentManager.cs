using UnityEngine;
using System.Collections.Generic;
using InterdimensionalGroceries.ItemSystem;

namespace InterdimensionalGroceries.CustomerSystem
{
    public class CustomerCommentManager : MonoBehaviour
    {
        public static CustomerCommentManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private CustomerCommentData commentData;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public string GetRandomComment(ItemType itemType)
        {
            if (commentData == null)
            {
                return $"Bring: {itemType}";
            }

            List<string> comments = commentData.GetCommentsForItem(itemType);

            if (comments == null || comments.Count == 0)
            {
                return $"Bring: {itemType}";
            }

            int randomIndex = Random.Range(0, comments.Count);
            return comments[randomIndex];
        }
    }
}
