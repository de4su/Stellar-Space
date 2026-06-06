using UnityEngine;
using Unity.FPS.Gameplay;
using Unity.FPS.Game;

namespace Unity.FPS.Roguelike
{
    public class PlayerStats : MonoBehaviour
    {
        public static PlayerStats Instance { get; private set; }

        public float SpeedAdd = 0f;
        public float JumpAdd = 0f;
        public float FireRateAdd = 0f;
        public float DamageAdd = 0f;
        public float MaxHealthAdd = 0f;
        public float AmmoAdd = 0f;

        PlayerCharacterController m_Controller;
        PlayerWeaponsManager m_WeaponsManager;
        Health m_Health;

        float m_BaseJumpForce;
        float m_BaseMaxSpeedOnGround;
        float m_BaseMaxSpeedInAir;
        float m_BaseMaxHealth;

        void Awake()
        {
            Instance = this;
            m_Controller = GetComponent<PlayerCharacterController>();
            m_WeaponsManager = GetComponent<PlayerWeaponsManager>();
            m_Health = GetComponent<Health>();

            if (m_Controller)
            {
                m_BaseJumpForce = m_Controller.JumpForce;
                m_BaseMaxSpeedOnGround = m_Controller.MaxSpeedOnGround;
                m_BaseMaxSpeedInAir = m_Controller.MaxSpeedInAir;
            }

            if (m_Health)
            {
                m_BaseMaxHealth = m_Health.MaxHealth;
            }
        }

        public void ApplyUpgrade(UpgradeData upgrade)
        {
            switch (upgrade.Type)
            {
                case UpgradeType.PlayerSpeed:
                    SpeedAdd += upgrade.Value;
                    ApplySpeed();
                    break;
                case UpgradeType.JumpHeight:
                    JumpAdd += upgrade.Value;
                    ApplyJump();
                    break;
                case UpgradeType.FireRate:
                    FireRateAdd += upgrade.Value;
                    ApplyFireRate(upgrade.Value);
                    break;
                case UpgradeType.Damage:
                    DamageAdd += upgrade.Value;
                    break;
                case UpgradeType.MaxHealth:
                    float oldMax = m_Health.MaxHealth;
                    MaxHealthAdd += upgrade.Value;
                    m_Health.MaxHealth = m_BaseMaxHealth + MaxHealthAdd;
                    if (m_Health.MaxHealth > oldMax)
                        m_Health.Heal(m_Health.MaxHealth - oldMax);
                    break;
                case UpgradeType.AmmoClip:
                    AmmoAdd += upgrade.Value;
                    ApplyAmmo(upgrade.Value);
                    break;
            }
        }

        void ApplyFireRate(float addValue)
        {
            foreach (var weapon in m_WeaponsManager.GetInventoryWeapons())
            {
                // Simple additive approach: subtract from delay, clamped to a minimum
                weapon.DelayBetweenShots = Mathf.Max(0.05f, weapon.DelayBetweenShots - addValue * 0.05f);
            }
        }

        void ApplyAmmo(float addValue)
        {
            foreach (var weapon in m_WeaponsManager.GetInventoryWeapons())
            {
                weapon.MaxAmmo += Mathf.RoundToInt(addValue);
            }
        }

        void ApplySpeed()
        {
            if (m_Controller)
            {
                m_Controller.MaxSpeedOnGround = m_BaseMaxSpeedOnGround + SpeedAdd;
                m_Controller.MaxSpeedInAir = m_BaseMaxSpeedInAir + SpeedAdd;
            }
        }

        void ApplyJump()
        {
            if (m_Controller)
            {
                m_Controller.JumpForce = m_BaseJumpForce + JumpAdd;
            }
        }
    }
}
