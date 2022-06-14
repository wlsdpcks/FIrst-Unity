using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    private Vector3 Velocity;
    private Vector3 PlayerMovementInput;
    private Vector2 PlayerMouseInput;
    private float xRot;

    public int ammo;
    public int health;
    public int hasGrenades;

    public int maxAmmo;
    public int maxHealth;
    public int maxHasGrenades;

    public GameObject[] weapons;
    public bool[] hasWeapons;

    public Animator anim;

    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool sDown4;


    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private CharacterController Controller;
    [Space]
    [SerializeField] private float Speed;
    [SerializeField] private float Jumpforce;
    [SerializeField] private float Sensitivity;
    [SerializeField] private float Gravity = -9.81f;
    [SerializeField] bool lockCursor = true;

    GameObject nearObject;
    GameObject equipWeapon;
    int equipWeaponIndex = -1;

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        PlayerMovementInput = new Vector3(x, 0f, z);
        PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        anim.SetFloat("speed", Mathf.Abs(x) + Mathf.Abs(z));

        MovePlayer();
        MovePlayerCamera();
        GetInput();
        Swap();
        Interaction();
    }

    void GetInput()
    {
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        sDown4 = Input.GetButtonDown("Swap4");
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;
        if (sDown4 && (!hasWeapons[3] || equipWeaponIndex == 3))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;
        if (sDown4) weaponIndex = 3;

        if (sDown1 || sDown2 || sDown3 || sDown4)
        {
            if(equipWeapon != null)
                equipWeapon.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true);
        }
    }

    void Interaction()
    {
        if(iDown && nearObject != null)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput);

        if (Controller.isGrounded)
        {
            Velocity.y = -1f;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Velocity.y = Jumpforce;
            }
        }
        else
        {
            Velocity.y -= Gravity * -2f * Time.deltaTime;
        }

        Controller.Move(MoveVector * Speed * Time.deltaTime);
        Controller.Move(Velocity * Time.deltaTime);
    }

    private void MovePlayerCamera()
    {
        xRot -= PlayerMouseInput.y * Sensitivity;

        transform.Rotate(0f, PlayerMouseInput.x * Sensitivity, 0f);
        PlayerCamera.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Grenade:
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}