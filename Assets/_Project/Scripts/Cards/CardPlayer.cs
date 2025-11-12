using UnityEngine;
using DB.Core;
using DB.Effects;

namespace DB.Cards
{
    public class CardPlayer : MonoBehaviour
    {
        public Entity player;
        public Entity enemy;

        public void PlayCard(CardData card)
        {
            Debug.Log($"Playing card: {card.cardName}");

            foreach (EffectData effect in card.effects)
            {
                // Decide who the target is
                Entity target = card.target switch
                {
                    TargetType.Self => player,
                    TargetType.SingleEnemy => enemy,
                    _ => enemy
                };

                EffectProcessor.ExecuteEffect(effect, player, target);
            }
        }
    }
}
