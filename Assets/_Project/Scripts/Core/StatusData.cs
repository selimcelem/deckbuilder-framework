using UnityEngine;

namespace DB.Statuses
{
    public enum TickTiming { StartOfTurn, EndOfTurn, OnAttack, Passive }

    [CreateAssetMenu(menuName = "DB/Status")]
    public class StatusData : ScriptableObject
    {
        public string statusName;
        public Sprite icon;
        public bool isBuff;
        public bool stacks = true;
        public TickTiming tickTiming = TickTiming.EndOfTurn;
    }
}
