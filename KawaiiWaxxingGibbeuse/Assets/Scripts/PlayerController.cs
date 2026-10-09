using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;

public class PlayerController : MonoBehaviourPun
{
    [Header("Stats")]
    public float moveSpeed;
    public float jumpForce;

    [Header("Components")]
    public Rigidbody _rb;

    [Header("Weapons")]
    public GameObject _pistol;
    public GameObject _shotgun;
    public GameObject _rifle;

    public int id;
    public Player photonPlayer;
    private int curAttackerId;
    public int curHP;
    public int maxHP;
    public int kills;
    public bool dead;
    private bool flashingDamage;
    public MeshRenderer mr;
    public PlayerWeapon weapon;

    private string activeGun;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pistol.SetActive(true);
        activeGun = "Pistol";
        _shotgun.SetActive(false);
        _rifle.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine || dead)
            return;

        Move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryJump();
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            _pistol.SetActive(true);
            activeGun = "Pistol";

            _shotgun.SetActive(false);
            _rifle.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            
            _shotgun.SetActive(true);
            activeGun = "Shotgun";

            _rifle.SetActive(false);
            _pistol.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            _rifle.SetActive(true);
            activeGun = "Rifle";

            _pistol.SetActive(false);
            _shotgun.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
            weapon.TryShoot(activeGun);
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 dir = (transform.forward * z + transform.right * x) * moveSpeed;
        dir.y = _rb.linearVelocity.y;

        _rb.linearVelocity = dir;
    }

    void TryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, 1.5f))
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        GameManager.instance.players[id - 1] = this;

        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            _rb.isKinematic = true;
        }

        else
        {
            GameUI.instance.Initialize(this);
        }
    }

    [PunRPC]
    public void TakeDamage(int attackerId, int damage)
    {
        if (dead)
            return;

        curHP -= damage;
        curAttackerId = attackerId;

        GameUI.instance.UpdateHealthBar();

        photonView.RPC("DamageFlash", RpcTarget.Others);

        if (curHP <= 0)
            photonView.RPC("Die", RpcTarget.All);
    }

    [PunRPC]
    void DamageFlash()
    {
        if (flashingDamage)
            return;

        StartCoroutine(DamageFlashCoroutine());

        IEnumerator DamageFlashCoroutine()
        {
            flashingDamage = true;
            Color defaultColor = mr.material.color;
            mr.material.color = Color.red;

            yield return new WaitForSeconds(0.05f);

            mr.material.color = defaultColor;
            flashingDamage = false;
        }
    }

    [PunRPC]
    void Die()
    {
        curHP = 0;
        dead = true;

        GameManager.instance.alivePlayers--;

        if (PhotonNetwork.IsMasterClient)
            GameManager.instance.CheckWinCondition();

        if(photonView.IsMine)
        {
            if (curAttackerId != 0)
                GameManager.instance.GetPlayer(curAttackerId).photonView.RPC("AddKill", RpcTarget.All);

            GetComponentInChildren<CameraController>().SetAsSpectator();

            _rb.isKinematic = true;
            transform.position = new Vector3(0, -50, 0);
        }
    }

    [PunRPC]
    public void AddKill()
    {
        kills++;
        GameUI.instance.UpdatePlayerInfoText();
    }

    [PunRPC]
    public void Heal(int amountToHeal)
    {
        curHP = Mathf.Clamp(curHP + amountToHeal, 0, maxHP);
        GameUI.instance.UpdateHealthBar();
    }

}
