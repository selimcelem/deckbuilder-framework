using System.Collections.Generic;
using UnityEngine;
using DB.Effects;

namespace DB.Cards
{
    public enum CardType { Attack, Skill, Power, Utility }
    public enum TargetType { None, Self, SingleEnemy, AllEnemies }
    public enum Rarity { Common, Uncommon, Rare, Epic }

    [CreateAssetMenu(menuName = "DB/Card")]
    public class CardData : ScriptableObject
    {
        public string cardName;
        public Sprite art;
        public int cost = 1;
        public CardType cardType = CardType.Skill;
        public TargetType target = TargetType.SingleEnemy;
        public List<EffectData> effects;
        [TextArea] public string description;
        public Rarity rarity = Rarity.Common;
    }
}
