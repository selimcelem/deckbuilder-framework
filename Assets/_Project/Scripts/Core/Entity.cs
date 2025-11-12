using UnityEngine;
using DB.Statuses; 

namespace DB.Core
{
    public class Entity : MonoBehaviour
    {
        [Header("Stats")]
        public string entityName = "Unnamed";
        public int maxHP = 50;
        public int currentHP = 50;
        public int block = 0;
        public int mana = 3;

        // Each entity has a StatusManager attached
        public StatusManager statusManager;

        protected virtual void Awake()
        {
            if (!statusManager)
                statusManager = GetComponent<StatusManager>();
        }

        public virtual void TakeDamage(int amount)
        {
            int damageAfterBlock = Mathf.Max(amount - block, 0);
            block = Mathf.Max(block - amount, 0);
            currentHP -= damageAfterBlock;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);

            Debug.Log($"{entityName} took {damageAfterBlock} damage (HP {currentHP}/{maxHP})");
        }

        public virtual void Heal(int amount)
        {
            currentHP = Mathf.Min(currentHP + amount, maxHP);
            Debug.Log($"{entityName} healed {amount} → HP {currentHP}/{maxHP}");
        }

        public void GainBlock(int amount)
        {
            block += amount;
            Debug.Log($"{entityName} gained {amount} Block (total {block})");
        }

        public void SpendMana(int amount)
        {
            mana = Mathf.Max(mana - amount, 0);
        }

        public void GainMana(int amount)
        {
            mana += amount;
        }
    }
}
