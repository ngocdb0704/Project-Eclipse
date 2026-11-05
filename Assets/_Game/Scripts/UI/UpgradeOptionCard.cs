
using System;
using Eclipse.Progression;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class UpgradeOptionCard : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Text levelText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Text rarityText;
        [SerializeField] private Image iconImage;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void Show(
            UpgradeData upgrade,
            int nextUpgradeLevel,
            Action<UpgradeData> onSelected)
        {
            if (upgrade == null)
                throw new ArgumentNullException(nameof(upgrade));

            if (onSelected == null)
                throw new ArgumentNullException(nameof(onSelected));

            if (button == null)
                button = GetComponent<Button>();

            if (nameText != null)
                nameText.text = upgrade.DisplayName;

            if (levelText != null)
            {
                levelText.text =
                    $"Level {nextUpgradeLevel}/{upgrade.MaxLevel}";
            }

            if (descriptionText != null)
                descriptionText.text = upgrade.Description;

            if (rarityText != null)
                rarityText.text = upgrade.Rarity.ToString();

            if (iconImage != null)
            {
                iconImage.sprite = upgrade.Icon;
                iconImage.enabled = upgrade.Icon != null;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(
                () => onSelected(upgrade));

            button.interactable = true;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (button == null)
                button = GetComponent<Button>();

            button.onClick.RemoveAllListeners();
            gameObject.SetActive(false);
        }

        public void Focus()
        {
            if (button == null)
                button = GetComponent<Button>();

            if (button.IsActive() && button.IsInteractable())
                button.Select();
        }
    }
}
