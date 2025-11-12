using UnityEngine;
using DB.Core;
using DB.Statuses;

namespace DB.Effects
{
    public static class EffectProcessor
    {
        public static void ExecuteEffect(EffectData effect, Entity caster, Entity target)
        {
            switch (effect.type)
            {
                case EffectType.Damage:
                    target.TakeDamage(effect.value);
                    break;

                case EffectType.Heal:
                    caster.Heal(effect.value);
                    break;

                case EffectType.GainBlock:
                    caster.GainBlock(effect.value);
                    break;

                case EffectType.Draw:
                    Debug.Log("Draw card placeholder");
                    break;

                case EffectType.GainMana:
                    caster.GainMana(effect.value);
                    break;

                case EffectType.ApplyStatus:
                    if (effect.status != null)
                        target.statusManager.AddStatus(effect.status, effect.value);
                    break;
            }
        }
    }
}
