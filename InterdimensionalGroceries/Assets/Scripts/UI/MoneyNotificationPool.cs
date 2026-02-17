using UnityEngine;
using System.Collections.Generic;

public class MoneyNotificationPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private MoneyNotification notificationPrefab;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private int initialPoolSize = 5;
    
    private Queue<MoneyNotification> availableNotifications;
    private List<MoneyNotification> allNotifications;

    private void Awake()
    {
        availableNotifications = new Queue<MoneyNotification>();
        allNotifications = new List<MoneyNotification>();
        
        if (spawnParent == null)
        {
            spawnParent = transform;
        }
        
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewNotification();
        }
    }

    private MoneyNotification CreateNewNotification()
    {
        MoneyNotification notification = Instantiate(notificationPrefab, spawnParent);
        notification.gameObject.SetActive(false);
        availableNotifications.Enqueue(notification);
        allNotifications.Add(notification);
        return notification;
    }

    public void SpawnNotification(float amount)
    {
        MoneyNotification notification;
        
        if (availableNotifications.Count > 0)
        {
            notification = availableNotifications.Dequeue();
        }
        else
        {
            notification = CreateNewNotification();
            availableNotifications.Dequeue();
        }
        
        notification.Show(amount, () => ReturnToPool(notification));
    }

    private void ReturnToPool(MoneyNotification notification)
    {
        if (notification != null && !availableNotifications.Contains(notification))
        {
            availableNotifications.Enqueue(notification);
        }
    }

    private void OnDestroy()
    {
        foreach (var notification in allNotifications)
        {
            if (notification != null)
            {
                notification.ForceHide();
            }
        }
    }
}
