using System.Collections.Generic;
using UnityEngine;

namespace DB.Statuses
{
    [RequireComponent(typeof(DB.Core.Entity))]
    public class StatusManager : MonoBehaviour
    {
        private DB.Core.Entity entity;

        // active statuses: name → stack count
        private Dictionary<StatusData, int> activeStatuses = new();

        private void Awake()
        {
            entity = GetComponent<DB.Core.Entity>();
        }

        public void AddStatus(StatusData status, int stacks)
        {
            if (activeStatuses.ContainsKey(status))
                activeStatuses[status] += stacks;
            else
                activeStatuses.Add(status, stacks);

            Debug.Log($"{entity.entityName} gained {stacks} × {status.statusName}");
        }

        public void TickStatuses(DB.Statuses.TickTiming timing)
        {
            foreach (var kvp in activeStatuses)
            {
                StatusData status = kvp.Key;
                int stacks = kvp.Value;

                if (status.tickTiming == timing)
                {
                    // For now, simple placeholder effects
                    if (!status.isBuff)
                        entity.TakeDamage(stacks); // debuff deals dmg
                    else
                        entity.Heal(stacks);       // buff heals 
                }
            }
        }
    }
}
