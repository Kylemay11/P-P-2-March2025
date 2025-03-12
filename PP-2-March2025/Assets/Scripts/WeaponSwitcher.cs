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

        if (Input.GetButton("Fire1"))
        {
            raycastWeapon currentWeaponScript = weapons[currentWeaponIndex].GetComponent<raycastWeapon>();
            if (currentWeaponScript != null)
                currentWeaponScript.TryShoot();
        }
    }

    void SwitchWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == index);
        }

        currentWeaponIndex = index;

        WeaponNotificationUI weaponUI = gameManager.instance.weaponNotification;
        if (weaponUI != null)
        {
            weaponUI.ShowWeaponName(weapons[index].name);
        }
    }

    public GameObject GetCurrentWeapon()
    {
        return weapons[currentWeaponIndex];
    }
}