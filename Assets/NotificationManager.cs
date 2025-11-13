using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform notificationParent;
    [SerializeField] private Notification[] notificationPrefabs;

    [SerializeField] private Color[] notificationColors;

    [Header("Settings")]
    [SerializeField] private int maxNotifications = 1;

    public static NotificationManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RemoveOldNotification(int sceneId)
    {

        for (int i = 0; i < notificationParent.childCount; i++)
        {
            Notification newNotification = notificationParent.GetChild(i).GetComponent<Notification>();

            if (newNotification.sceneIndex == sceneId)
            {
                Destroy(newNotification.gameObject);
            }
        }

        }

    public void ShowNotification(int sceneId ,int prefabId, int colorId, string notificationText, string secondText, Transform Target)
    {
        if (sceneId == SceneLoader.Instance.activeScene)
        {
            if (notificationParent.childCount >= maxNotifications)
                Destroy(notificationParent.GetChild(0).gameObject);

            Notification newNotification = Instantiate(notificationPrefabs[prefabId], notificationParent);

            newNotification.notificationText.text = "" + notificationText;
            newNotification.secondNotificationText.text = "" + secondText;
            newNotification.sceneIndex = sceneId;

            newNotification.backgroundImage.color = notificationColors[colorId];
            newNotification.notificationButton.onClick.AddListener(() => TapOnNotification(newNotification, sceneId, Target));

        }

        
    }

    public void TapOnNotification(Notification notification,int sceneId, Transform Target)
    {
        if (sceneId == SceneLoader.Instance.activeScene)

        {
            /// Debug.Log("TapOnNotification");
            ///
            if (Target != null)
            {
                SceneLoader.Instance.activeBattleScenes[sceneId].cameraController.CamToPoint(Target.transform.position);
               
            }
            Destroy(notification.gameObject);
        }


    }

    private IEnumerator FadeInAndOut(CanvasGroup cg, float duration)
    {
        float fadeIn = 0.3f;
        float fadeOut = 0.5f;

        for (float t = 0; t < fadeIn; t += Time.deltaTime)
        {
            cg.alpha = t / fadeIn;
            yield return null;
        }
        cg.alpha = 1f;

        yield return new WaitForSeconds(duration);

        for (float t = 0; t < fadeOut; t += Time.deltaTime)
        {
            cg.alpha = 1 - (t / fadeOut);
            yield return null;
        }
        Destroy(cg.gameObject);
    }
}
