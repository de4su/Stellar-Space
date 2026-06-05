using UnityEngine;
using UnityEngine.UI;
using Unity.FPS.Roguelike;

namespace Unity.FPS.UI
{
    public class SkillDisplayHUD : MonoBehaviour
    {
        [SerializeField] private GameObject iconPrefab;
        [SerializeField] private Transform container;

        private void Start()
        {
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.OnUpgradesUpdated += RefreshUI;
            }
            RefreshUI();
        }

        private void OnDestroy()
        {
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.OnUpgradesUpdated -= RefreshUI;
            }
        }

        private void RefreshUI()
        {
            if (UpgradeManager.Instance == null || container == null || iconPrefab == null) return;

            // Clear container
            foreach (Transform child in container)
            {
                // In Editor, we use DestroyImmediate if we want it to be immediate, 
                // but this runs in Play mode usually.
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }

            // Rebuild icons
            foreach (var upgrade in UpgradeManager.Instance.ActiveUpgrades)
            {
                if (upgrade.Icon != null)
                {
                    GameObject iconGO = Instantiate(iconPrefab, container);
                    Transform iconTransform = iconGO.transform.Find("Icon");
                    UnityEngine.UI.Image img = (iconTransform != null) ? iconTransform.GetComponent<UnityEngine.UI.Image>() : iconGO.GetComponent<UnityEngine.UI.Image>();
                    if (img != null)
                    {
                        img.sprite = upgrade.Icon;
                        img.enabled = true;
                    }
                }
            }
        }
    }
}
