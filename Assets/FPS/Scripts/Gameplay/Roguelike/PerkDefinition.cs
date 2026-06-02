using UnityEngine;

namespace Unity.FPS.Roguelike
{
    public class PerkDefinition : MonoBehaviour
    {
        [Header("Identity")]
        public string PerkName;
        [TextArea]
        public string Description;

        [Header("Movement & Stats")]
        public float SpeedMultiplier = 1f;
        public float JumpMultiplier = 1f;
        public float HealthMultiplier = 1f;
        public float DamageMultiplier = 1f;

        [Header("Ability Timing")]
        public float Duration = 0f;
        public float Cooldown = 0f;
        public float EnergyMax = 100f;
        public float EnergyDrainRate = 0f;
        public float EnergyRegenRate = 0f;

        [Header("Combat")]
        public float Damage = 0f;
        public float Force = 0f;
        public float Range = 0f;

        [Header("Special")]
        public float MomentumMultiplier = 1f;
    }
}
