using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Unity.FPS.Roguelike
{
    public class UpgradeUI : MonoBehaviour
    {
        public GameObject UIContainer;
        public GameObject UpgradeCardPrefab;
        public Transform CardsParent;

        public void ShowOptions(List<UpgradeData> upgrades)
        {
            UIContainer.SetActive(true);
            foreach (Transform child in CardsParent)
            {
                Destroy(child.gameObject);
            }

            foreach (var upgrade in upgrades)
            {
                GameObject card = Instantiate(UpgradeCardPrefab, CardsParent);
                // Setup card UI
                Transform textT = card.transform.Find("Text");
                if (textT != null)
                {
                    TextMeshProUGUI tmp = textT.GetComponent<TextMeshProUGUI>();
                    if (tmp != null)
                    {
                        tmp.text = "<b>" + upgrade.Title + "</b>\n\n" + upgrade.Description;
                    }
                }
                
                Transform iconT = card.transform.Find("Icon");
                if (iconT != null)
                {
                    UnityEngine.UI.Image iconImg = iconT.GetComponent<UnityEngine.UI.Image>();
                    if (iconImg != null)
                    {
                        iconImg.sprite = upgrade.Icon;
                        iconImg.enabled = upgrade.Icon != null;
                    }
                }

                Button btn = card.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    Debug.Log("[Roguelike] Button clicked for: " + upgrade.Title);
                    UpgradeManager.Instance.SelectUpgrade(upgrade);
                    UIContainer.SetActive(false);
                });
}
}
    }
}
