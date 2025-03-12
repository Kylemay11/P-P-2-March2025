using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;
    private int currentWeaponIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SwitchWeapon(currentWeaponIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchWeapon(2);

        if (Input.GetButtonDown("Fire1"))
        {
            raycastWeapon weapon = weapons[currentWeaponIndex].GetComponent<raycastWeapon>();
            if (weapon != null)
            {
                weapon.Shoot();
            }
        }

    }

    void SwitchWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == index);
        }
        currentWeaponIndex = index;
    }

    public GameObject GetCurrentWeapon()
    {
        return weapons[currentWeaponIndex];
    }

}
