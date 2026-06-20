using UnityEngine;
using UnityEngine.UI;
using VegetaSystem;

namespace MercuryStudioAssignment
{
    public class MonsterCell : SinglePoolable
    {
        [SerializeField] private Image _image;

        public int MonsterId { get; private set; } = -1;

        public RectTransform Rect => (RectTransform)transform;

        public void Bind(int monsterId, Sprite sprite)
        {
            MonsterId = monsterId;
            _image.sprite = sprite;
        }

        public override void Get()
        {
            gameObject.SetActive(true);
        }

        public override void Release()
        {
            MonsterId = -1;
            gameObject.SetActive(false);
        }
    }
}
