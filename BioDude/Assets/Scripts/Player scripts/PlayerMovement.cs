using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public GameObject pistolBullet;
    public AudioSource pistolFire;



    public List<Explosive> GrenadeList;
    Explosive selectedGrenade;
    public float throwForce = 5000f;

    private Rigidbody2D rb2D;
    private float directionAngle;

    // Use this for initialization
    void Start () {
//unused         rb2D = GetComponent<Rigidbody2D>();
        if (GrenadeList.Count > 0)
            selectedGrenade = GrenadeList[0];
    }

    // Update is called once per frame
    void FixedUpdate () {
        Movement();
        Looking();
    }
    private void UseGrenade()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        Debug.Log("throw");
        if (selectedGrenade != null)
        {
            Explosive nade = Instantiate(selectedGrenade, new Vector3(x, y, z), transform.rotation);
            nade.Throw(throwForce);
        }
    }
    
    private void Movement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveX * speed, moveY * speed);
        //rb2D.AddForce(move * speed);
    }

    private void Looking()
    {
        Vector2 playerPos = Camera.main.WorldToViewportPoint(transform.position);
        Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        directionAngle = Mathf.Atan2(mousePos.y - playerPos.y, mousePos.x - playerPos.x);
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, directionAngle * Mathf.Rad2Deg - 90));
        Debug.DrawLine(transform.position, transform.position + 10 * new Vector3(Mathf.Cos(directionAngle) , Mathf.Sin(directionAngle), 0), Color.blue);
        if (Input.GetMouseButtonDown(0))
            Shooting();
        if (Input.GetKeyDown(KeyCode.E))
            UseGrenade();
        if (Input.GetKeyDown(KeyCode.R) && GameObject.FindGameObjectWithTag("PlayerWeaponSlot").GetComponent<WeaponManager>().currentAmmo > 0 && GameObject.FindGameObjectWithTag("PlayerWeaponSlot").GetComponent<WeaponManager>().isReloading == false && GameObject.FindGameObjectWithTag("PlayerWeaponSlot").GetComponent<WeaponManager>().currentClipAmmo != GameObject.FindGameObjectWithTag("PlayerWeaponSlot").GetComponent<WeaponManager>().clipSize)
            StartCoroutine(GameObject.FindGameObjectWithTag("PlayerWeaponSlot").GetComponent<WeaponManager>().Reload());
    }

    public float GetDirectionAngle()
    {
        //return AngleBetweenToPoints(transform.position, Input.mousePosition) + 90;
        return directionAngle;
    }

    private void Shooting()
    {
        //sometimes bullet spawns behind the player :D
        if (GameObject.FindGameObjectWithTag("PlayerWeaponSlot").GetComponent<WeaponManager>().cooldownEnded && GameObject.FindGameObjectWithTag("PlayerWeaponSlot").GetComponent<WeaponManager>().activeWeapon != null && GameObject.FindGameObjectWithTag("PlayerWeaponSlot").GetComponent<WeaponManager>().isReloading == false)
        {
            if (GameObject.FindGameObjectWithTag("PlayerWeaponSlot").GetComponent<WeaponManager>().currentClipAmmo > 0)
            {
                pistolFire.Play();
                float x = GameObject.FindGameObjectWithTag("PlayerWeaponSlot").transform.position.x;
                float y = GameObject.FindGameObjectWithTag("PlayerWeaponSlot").transform.position.y;
                float z = GameObject.FindGameObjectWithTag("PlayerWeaponSlot").transform.position.z;
                GameObject.FindGameObjectWithTag("PlayerWeaponSlot").GetComponent<WeaponManager>().cooldownEnded = false;
                GameObject.FindGameObjectWithTag("PlayerWeaponSlot").GetComponent<WeaponManager>().currentClipAmmo--;
                StartCoroutine("Cooldown");
                Instantiate(pistolBullet, new Vector3(x, y, z), transform.rotation);
                if (GameObject.FindGameObjectWithTag("PlayerWeaponSlot").GetComponent<WeaponManager>().currentClipAmmo == 0 && GameObject.FindGameObjectWithTag("PlayerWeaponSlot").GetComponent<WeaponManager>().currentAmmo > 0)
                    StartCoroutine(GameObject.FindGameObjectWithTag("PlayerWeaponSlot").GetComponent<WeaponManager>().Reload());
            }
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(GameObject.FindGameObjectWithTag("PlayerWeaponSlot").GetComponent<WeaponManager>().activeWeapon.GetComponent<Weapon>().cooldown);
        GameObject.FindGameObjectWithTag("PlayerWeaponSlot").GetComponent<WeaponManager>().cooldownEnded = true;
    }
}
