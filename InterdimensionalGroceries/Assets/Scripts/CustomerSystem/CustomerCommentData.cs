using UnityEngine;
using System.Collections.Generic;
using InterdimensionalGroceries.ItemSystem;

namespace InterdimensionalGroceries.CustomerSystem
{
    [System.Serializable]
    public class ItemCommentEntry
    {
        [SerializeField] private ItemType itemType;
        [SerializeField] private List<string> comments = new List<string>();

        public ItemType ItemType => itemType;
        public List<string> Comments => comments;
    }

    [CreateAssetMenu(fileName = "SO_CustomerComments", menuName = "InterdimensionalGroceries/Customer Comment Data")]
    public class CustomerCommentData : ScriptableObject
    {
        [SerializeField] private List<ItemCommentEntry> commentEntries = new List<ItemCommentEntry>();

        public List<ItemCommentEntry> CommentEntries => commentEntries;

        public List<string> GetCommentsForItem(ItemType itemType)
        {
            foreach (var entry in commentEntries)
            {
                if (entry.ItemType == itemType)
                {
                    return entry.Comments;
                }
            }
            return null;
        }
    }
}
