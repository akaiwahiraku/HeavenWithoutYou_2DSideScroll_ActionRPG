using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerFX : EntityFX
{
    [Header("Screen shake FX")]
    private CinemachineImpulseSource screenShake;
    [SerializeField] private float shakeMultiplier;
    public Vector3 shakeNormalDamage;
    public Vector3 shakeHighDamage;
    public Vector3 shakeNormalAttack;

    [Header("After image fx")]
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float colorLooseRate;
    [SerializeField] private float afterImageCooldown;
    private float afterImageCooldownTimer;

    [Space]
    [SerializeField] private ParticleSystem dustFx;
    [SerializeField] private ParticleSystem guardFx;
    [SerializeField] private ParticleSystem healFx;
    [SerializeField] private ParticleSystem overDriveFx;
    [SerializeField] private ParticleSystem startOverDriveFx;
    [SerializeField] private ParticleSystem startForceFx;

    protected override void Start()
    {
        base.Start();
        screenShake = GetComponent<CinemachineImpulseSource>();

    }

    private void Update()
    {
        afterImageCooldownTimer -= Time.deltaTime;

    }
    public void ScreenShake(Vector3 _shakePower)
    {
        screenShake.m_DefaultVelocity = new Vector3(_shakePower.x * player.facingDir, _shakePower.y) * shakeMultiplier;
        screenShake.GenerateImpulse();
    }

    public void CreateAfterImage(Vector3 playerVelocity)
    {
        if (playerVelocity.sqrMagnitude < 0.01f)
            return;

        if (afterImageCooldownTimer < 0)
        {
            afterImageCooldownTimer = afterImageCooldown;
            GameObject newAfterImage = Instantiate(afterImagePrefab, transform.position, transform.rotation);
            newAfterImage.GetComponent<AfterImageFX>().SetupAfterImage(colorLooseRate, sr.sprite, playerVelocity);
        }
    }


    public void PlayDustFX()
    {
        if (dustFx != null)
            dustFx.Play();
    }

    public void PlayGuardFX()
    {
        if (guardFx != null)
            guardFx.Play();
    }

    public void PlayHealFX()
    {
        if (healFx != null)
            healFx.Play();
    }

    public void PlayOverDriveFX()
    {
        if (overDriveFx != null)
        {
            overDriveFx.gameObject.SetActive(true); // 再表示
            overDriveFx.Play();
        }
    }

    public void StopOverDriveFX()
    {
        if (overDriveFx != null)
        {
            overDriveFx.Stop();
            overDriveFx.Clear(); // 既存パーティクルを削除
        }
    }


    public void PlayStartOverDriveFX()
    {
        if (startOverDriveFx != null)
            startOverDriveFx.Play();
    }

    public void PlayStartForceFX()
    {
        if (startForceFx != null)
            startForceFx.Play();
    }

    public void StopStartOverDriveFX()
    {
        if (startOverDriveFx != null)
            startOverDriveFx.Stop();
    }

    public override void CreateHitFx(Transform _target, bool _critical)
    {
        float xOffset = 1.5f * player.facingDir;  // プレイヤー向きに応じてオフセット
        float xPosition = xOffset + Random.Range(-.1f, .1f);
        float yPosition = Random.Range(-.1f, .1f);
        float zRotation = Random.Range(-30, 30);

        GameObject hitPrefab = hitFx;
        float yRotation = 0;
        if (player.facingDir == -1)
            yRotation = -180;

        Vector3 hitFxRotation = new Vector3(0, yRotation, zRotation);

        if (_critical)
        {
            hitPrefab = criticalHitFx;
        }

        // X オフセットを適用してエフェクトを生成
        GameObject newHitFx = Instantiate(hitPrefab, _target.position + new Vector3(xPosition, yPosition), Quaternion.identity, _target);
        newHitFx.transform.Rotate(hitFxRotation);
        Destroy(newHitFx, .5f);
    }

}
