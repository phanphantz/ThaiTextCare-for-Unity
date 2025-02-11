using UnityEngine;
using UnityEngine.UI;

namespace PhEngine.ThaiTextCare.Utility
{
    public class ImageHighlight : Highlight
    {
        [Header("Components")]
        [SerializeField] Image image;

        [Header("Settings")]
        [SerializeField] float positionOffsetY;
        [SerializeField] float sizeOffsetY;
        
        public override Highlight Clone(WordHit word)
        {
            return Instantiate(this, word.text.rectTransform, false);
        }

        public override void Dispose()
        {
            Destroy(gameObject);
        }

        public override void PlaceAt(WordHit word)
        {
            image.rectTransform.anchoredPosition = word.startPosition + new Vector3(0, positionOffsetY, 0);
            image.fillAmount = 0;
            
            var height = word.fontSize + sizeOffsetY;
            image.rectTransform.sizeDelta = new Vector2(word.Width, height);
            image.gameObject.SetActive(true);
        }
    }
}