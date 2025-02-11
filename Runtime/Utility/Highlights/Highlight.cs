using UnityEngine;

namespace PhEngine.ThaiTextCare.Utility
{
    [DisallowMultipleComponent]
    public abstract class Highlight : MonoBehaviour
    {
        public WordHit Word => word;
        [SerializeField] WordHit word;
        public void SetWord(WordHit value)
        {
            word = value;
        }
        
        public abstract Highlight Clone(WordHit word);
        public abstract void Dispose();
        public abstract void PlaceAt(WordHit word);
    }
}