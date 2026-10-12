using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;


public abstract class Gun
{
    public float shootRate;
    public int curAmmo;
    public int maxAmmo;
    public int mag;
    public int maxMag;

    public Transform bulletSpawnPos;
}

[System.Serializable]
public class Pistol : Gun
{
}

[System.Serializable]
public class Shotgun : Gun
{
    public Transform bulletSpawnPos2;
}

[System.Serializable]
public class Rifle : Gun
{
}

public class PlayerWeapon : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public int damage;
    //public int curAmmo;
    //public int maxAmmo;
    public float bulletSpeed;
    //public float shootRate;
    private float lastShootTime;
    public GameObject bulletPrefab;
    //public Transform bulletSpawnPos;
    private PlayerController player;

    public Pistol ps;
    public Shotgun sg;
    public Rifle rf;
    
    void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    public void TryShoot(string activeGun)
    {

        if(activeGun == "Pistol")
        {
            if (ps.mag <= 0 || Time.time - lastShootTime < ps.shootRate)
                return;

            ps.mag--;
            lastShootTime = Time.time;

            player.photonView.RPC("SpawnBullet", RpcTarget.All, ps.bulletSpawnPos.transform.position,
                Camera.main.transform.forward);
        }

        if (activeGun == "Shotgun")
        {
            if (sg.mag < 2 || Time.time - lastShootTime < sg.shootRate)
                return;

            sg.mag -= 2;
            lastShootTime = Time.time;

            player.photonView.RPC("SpawnBullet", RpcTarget.All, sg.bulletSpawnPos.transform.position,
                 Camera.main.transform.forward);
            player.photonView.RPC("SpawnBullet", RpcTarget.All, sg.bulletSpawnPos2.transform.position,
                Camera.main.transform.forward);
            player.photonView.RPC("SpawnBullet", RpcTarget.All, sg.bulletSpawnPos.transform.position,
                 Camera.main.transform.forward);
            player.photonView.RPC("SpawnBullet", RpcTarget.All, sg.bulletSpawnPos2.transform.position,
                Camera.main.transform.forward);
            player.photonView.RPC("SpawnBullet", RpcTarget.All, sg.bulletSpawnPos.transform.position,
                 Camera.main.transform.forward);
            player.photonView.RPC("SpawnBullet", RpcTarget.All, sg.bulletSpawnPos2.transform.position,
                Camera.main.transform.forward);

        }

        if (activeGun == "Rifle")
        {
            if (rf.mag <= 0 || Time.time - lastShootTime < rf.shootRate)
                return;

            rf.mag--;
            lastShootTime = Time.time;

            player.photonView.RPC("SpawnBullet", RpcTarget.All, rf.bulletSpawnPos.transform.position,
                Camera.main.transform.forward);
        }
        
        GameUI.instance.UpdateAmmoText(activeGun);
    }

    [PunRPC]
    void SpawnBullet(Vector3 pos, Vector3 dir)
    {

        GameObject bulletObj = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bulletObj.transform.forward = dir;

        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.Initialize(damage, player.id, player.photonView.IsMine);
        bulletScript._rb.linearVelocity = dir * bulletSpeed;
    }
    
    [PunRPC]
    public void GiveAmmo(int ammoToGive)
    {
        ps.curAmmo = Mathf.Clamp(ps.curAmmo + ammoToGive, 0, ps.maxAmmo);
        sg.curAmmo = Mathf.Clamp(sg.curAmmo + ammoToGive, 0, sg.maxAmmo);
        rf.curAmmo = Mathf.Clamp(rf.curAmmo + ammoToGive, 0, rf.maxAmmo);
        
        GameUI.instance.UpdateAmmoText(player.activeGun);
    }

    public void reloadGun(string activeGun)
    {
        if (activeGun == "Pistol" && ps.curAmmo >= ps.maxMag)
        {
            ps.curAmmo -= ps.maxMag - ps.mag;
            ps.mag = ps.maxMag;
        }
        if (activeGun == "Shotgun" && sg.curAmmo >= sg.maxMag)
        {
            sg.curAmmo -= sg.maxMag - sg.mag;
            sg.mag = sg.maxMag;
        }
        if (activeGun == "Rifle" && rf.curAmmo >= rf.maxMag)
        {
            rf.curAmmo -= rf.maxMag - rf.mag;
            rf.mag = rf.maxMag;
        }
    }
    
}



