using UnityEngine;

namespace DB.Effects
{
    public enum EffectType { Damage, Heal, GainBlock, Draw, GainMana, ApplyStatus }

    [CreateAssetMenu(menuName = "DB/Effect")]
    public class EffectData : ScriptableObject
    {
        public EffectType type;
        public int value = 0;
        public DB.Statuses.StatusData status; // optional
        public bool affectSelf = false;       // self vs target
    }
}
