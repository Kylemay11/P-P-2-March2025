using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;

    private int currentWeaponIndex;

    void Start()
    {
        SwitchWeapon(currentWeaponIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchWeapon(3);

        if (!gameManager.instance.isPaused && Input.GetButton("Fire1"))
        {
            raycastWeapon currentWeaponScript = weapons[currentWeaponIndex].GetComponent<raycastWeapon>();
            if (currentWeaponScript != null)
                currentWeaponScript.TryShoot();
        }
    }
    
    public void SwitchWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == index);
        }

        currentWeaponIndex = index;

        raycastWeapon weapon = weapons[index].GetComponent<raycastWeapon>();


        if (weapon != null)
        {
            AmmoUI.instance?.UpdateAmmo(weapon.CurrentAmmo, weapon.MaxAmmo);
            AmmoUI.instance?.Show(true);
            gameManager.instance.weaponNotification?.ShowWeaponName(weapon.name);
        }
        else
        {
            AmmoUI.instance?.Show(false);
        }
    }
}

