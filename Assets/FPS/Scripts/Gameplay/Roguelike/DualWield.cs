using UnityEngine;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;

namespace Unity.FPS.Roguelike
{
    public class DualWield : MonoBehaviour
    {
        // Horizontal offset of the off-hand weapon from WeaponParentSocket — tune in editor
        const float k_OffHandXOffset = -0.50f;

        PlayerWeaponsManager m_WeaponsManager;
        WeaponController m_MainWeapon;
        GameObject m_OffHandSocket;
        WeaponController m_OffHandWeapon;
        Transform m_OffHandMuzzle;
        Animator m_OffHandAnimator;

        void Start()
        {
            m_WeaponsManager = GetComponent<PlayerWeaponsManager>();
            if (m_WeaponsManager == null)
            {
                Debug.LogError("[DualWield] PlayerWeaponsManager not found on player!");
                return;
            }

            m_MainWeapon = m_WeaponsManager.GetActiveWeapon();
            if (m_MainWeapon == null)
            {
                Debug.LogError("[DualWield] No active weapon found — make sure DualWield is applied after weapon init.");
                return;
            }

            // Disable ADS for the duration this perk is active
            m_WeaponsManager.ForceNoAim = true;

            SetupOffHand();

            // Hook into the main weapon's fire event to fire the off-hand shot
            m_MainWeapon.OnShoot += FireOffHand;

            Debug.Log("[Roguelike] Tank Special: Dual Wield activated.");
        }

        void SetupOffHand()
        {
            // Parent to WeaponParentSocket so the off-hand inherits all bob, recoil,
            // and weapon-switch animation automatically
            m_OffHandSocket = new GameObject("OffHandSocket");
            m_OffHandSocket.transform.SetParent(m_WeaponsManager.WeaponParentSocket, false);
            m_OffHandSocket.transform.localPosition = new Vector3(k_OffHandXOffset, 0f, 0f);
            m_OffHandSocket.transform.localRotation = Quaternion.identity;
            m_OffHandSocket.transform.localScale = Vector3.one;

            // Instantiate a visual copy of the current weapon
            GameObject offHandGo = Instantiate(m_MainWeapon.SourcePrefab, m_OffHandSocket.transform);
            offHandGo.transform.localPosition = Vector3.zero;
            offHandGo.transform.localRotation = Quaternion.identity;

            // Set up the off-hand WeaponController so it knows who owns it,
            // but we will never call HandleShootInputs on it — it is visual only
            m_OffHandWeapon = offHandGo.GetComponent<WeaponController>();
            if (m_OffHandWeapon != null)
            {
                m_OffHandWeapon.Owner = gameObject;
                m_OffHandWeapon.SourcePrefab = m_MainWeapon.SourcePrefab;
                m_OffHandMuzzle = m_OffHandWeapon.WeaponMuzzle;
                m_OffHandAnimator = m_OffHandWeapon.WeaponAnimator;
                m_OffHandWeapon.ShowWeapon(true);
            }
            else
            {
                Debug.LogWarning("[DualWield] WeaponController not found on off-hand instance. Muzzle will be missing.");
                m_OffHandMuzzle = m_OffHandSocket.transform; // fallback
            }

            // Apply the FPS weapon layer to all children — same pattern as PlayerWeaponsManager.AddWeapon
            if (m_WeaponsManager.FpsWeaponLayer.value > 0)
            {
                int layerIndex = Mathf.RoundToInt(Mathf.Log(m_WeaponsManager.FpsWeaponLayer.value, 2));
                foreach (Transform t in offHandGo.GetComponentsInChildren<Transform>(true))
                {
                    t.gameObject.layer = layerIndex;
                }
            }
        }

        void FireOffHand()
        {
            if (m_MainWeapon == null || m_OffHandMuzzle == null) return;

            // Use the main weapon's spread calculation from the off-hand muzzle position
            Vector3 shotDirection = m_MainWeapon.GetShotDirectionWithinSpread(m_OffHandMuzzle);

            // Spawn the same projectile as the main weapon, from the off-hand muzzle
            ProjectileBase newProjectile = Instantiate(
                m_MainWeapon.ProjectilePrefab,
                m_OffHandMuzzle.position,
                Quaternion.LookRotation(shotDirection));

            // Shoot() sets owner, inherited muzzle velocity, and charge from the main weapon
            newProjectile.Shoot(m_MainWeapon);

            // Drain one unit of heat from the shared (main weapon) meter
            m_MainWeapon.UseAmmo(1f);

            // Trigger the firing animation on the off-hand weapon
            if (m_OffHandAnimator != null)
                m_OffHandAnimator.SetTrigger("Attack");

            // Spawn muzzle flash on the off-hand if the weapon has one
            if (m_MainWeapon.MuzzleFlashPrefab != null)
            {
                GameObject flash = Instantiate(
                    m_MainWeapon.MuzzleFlashPrefab,
                    m_OffHandMuzzle.position,
                    m_OffHandMuzzle.rotation,
                    m_OffHandMuzzle);

                Destroy(flash, 2f);
            }
        }

        void OnDestroy()
        {
            if (m_MainWeapon != null)
                m_MainWeapon.OnShoot -= FireOffHand;

            if (m_WeaponsManager != null)
                m_WeaponsManager.ForceNoAim = false;

            if (m_OffHandSocket != null)
                Destroy(m_OffHandSocket);

            Debug.Log("[Roguelike] Dual Wield deactivated.");
        }
    }
}
