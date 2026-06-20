using UnityEngine;

namespace MercuryStudioAssignment
{
    [CreateAssetMenu(fileName = "MonsterCatalog", menuName = "MonsterWheel/MonsterCatalog")]
    public class MonsterCatalog : ScriptableObject
    {
        [SerializeField] private Sprite[] _monsters;

        public int Count => _monsters.Length;

        public Sprite GetSprite(int id) => _monsters[id];

        public int RandomId() => Random.Range(0, Count);
    }
}
