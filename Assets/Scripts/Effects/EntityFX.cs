using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    // �C���X�y�N�^�ŕK���A�T�C������i���ݒ�Ȃ� Awake �Ŏ����擾�j
    [SerializeField] protected SpriteRenderer sr;
    protected Player player;

    [Header("Pop Up Text")]
    [SerializeField] private GameObject popUpTextPrefab;

    [Header("Flash FX")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Material hitMat;
    private Material originalMat;

    //[Header("OverDrive FX")]
    //[SerializeField] private float overDriveDuration;
    //[SerializeField] private Material overDriveMat;
    //private Material odMat;

    [Header("Ailment colors")]
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] shockColor;

    [Header("Ailment particles")]
    [SerializeField] private ParticleSystem igniteFx;
    [SerializeField] private ParticleSystem chillFx;
    [SerializeField] private ParticleSystem shockFx;

    [Header("Hit FX")]
    [SerializeField] protected GameObject hitFx;
    [SerializeField] protected GameObject criticalHitFx;
    [SerializeField] private GameObject cloneHitFx;

    private GameObject myHealthBar;

    [Header("Default Color")]
    // Inspector �Ŋ�{�F��ݒ�i���̐F�� FlashFX �I����߂�܂��j
    [SerializeField] private Color defaultColor;

    protected virtual void Awake()
    {
        if (sr == null)
        {
            sr = GetComponentInChildren<SpriteRenderer>();
        }
    }

    protected virtual void Start()
    {
        player = CurrencyManager.instance.player;
        originalMat = sr.material;
        myHealthBar = GetComponentInChildren<UI_HealthBar>().gameObject;
        // defaultColor �����ݒ�Ȃ�A������ sr.color ���̗p����
        if (defaultColor == default(Color))
        {
            defaultColor = sr.color;
        }
    }

    public void CreatePopUpText(string _text)
    {
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(2, 3);
        Vector3 positionOffset = new Vector3(randomX, randomY, 0);
        GameObject newText = Instantiate(popUpTextPrefab, transform.position + positionOffset, Quaternion.identity);
        newText.GetComponent<TextMeshPro>().text = _text;
    }

    public void MakeTransparent(bool _transparent)
    {
        if (_transparent)
        {
            myHealthBar.SetActive(false);
            sr.color = Color.clear;
        }
        else
        {
            myHealthBar.SetActive(true);
            sr.color = defaultColor;
        }
    }

    /// <summary>
    /// FlashFX �����FhitMat �ɐ؂�ւ��A��u���ɂ��A���̌� defaultColor �ɖ߂�
    /// </summary>
    public IEnumerator FlashFX()
    {
        //Debug.Log("[FlashFX] Starting flash. Default color = " + defaultColor);
        sr.material = hitMat;
        sr.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        sr.material = originalMat;
        sr.color = defaultColor;
        //Debug.Log("[FlashFX] Flash ended. Restored color = " + sr.color + ", material = " + sr.material.name);
    }

    /// <summary>
    /// FlashFX �̃e�X�g�p���b�p�[
    /// </summary>
    public IEnumerator FlashFXTestWrapper()
    {
        yield return FlashFX();
    }

    /// <summary>
    /// Ailment �n�G�t�F�N�g�p�FInvokeRepeating �ł̐F�ύX���~���邾��
    /// </summary>
    private void CancelColorChange()
    {
        //Debug.Log("[CancelColorChange] Called. Current color: " + sr.color);
        CancelInvoke();
        igniteFx.Stop();
        chillFx.Stop();
        shockFx.Stop();
    }

    public void IgniteFxFor(float _seconds)
    {
        //Debug.Log("[IgniteFxFor] Starting Ignite FX. Current color: " + sr.color);
        igniteFx.Play();
        InvokeRepeating("IgniteColorFx", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void ChillFxFor(float _seconds)
    {
        //Debug.Log("[ChillFxFor] Starting Chill FX. Current color: " + sr.color);
        chillFx.Play();
        InvokeRepeating("ChillColorFx", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void ShockFxFor(float _seconds)
    {
        //Debug.Log("[ShockFxFor] Starting Shock FX. Current color: " + sr.color);
        shockFx.Play();
        InvokeRepeating("ShockColorFx", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    private void IgniteColorFx()
    {
        if (sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
        //Debug.Log("[IgniteColorFx] sr.color = " + sr.color);
    }

    private void ChillColorFx()
    {
        if (sr.color != chillColor[0])
            sr.color = chillColor[0];
        else
            sr.color = chillColor[1];
        //Debug.Log("[ChillColorFx] sr.color = " + sr.color);
    }

    private void ShockColorFx()
    {
        if (sr.color != shockColor[0])
            sr.color = shockColor[0];
        else
            sr.color = shockColor[1];
        //Debug.Log("[ShockColorFx] sr.color = " + sr.color);
    }

    public virtual void CreateHitFx(Transform _target, bool _critical)
    {
        float xPosition = Random.Range(-0.1f, 0.1f);
        float yPosition = Random.Range(-0.1f, 0.1f);
        float zRotation = Random.Range(-30, 30);
        GameObject hitPrefab = hitFx;
        float yRotation = 0;
        if (player.facingDir == -1)
            yRotation = -180;
        Vector3 hitFxRotation = new Vector3(0, yRotation, zRotation);
        if (_critical)
            hitPrefab = criticalHitFx;
        GameObject newHitFx = Instantiate(hitPrefab, _target.position + new Vector3(xPosition, yPosition), Quaternion.identity, _target);
        newHitFx.transform.Rotate(hitFxRotation);
        Destroy(newHitFx, 0.5f);
    }

    public void CreateCloneHitFx(Transform _target, int facingDir)
    {
        float xOffset = 2f * facingDir;
        float xPosition = xOffset + Random.Range(-0.1f, 0.1f);
        float yPosition = Random.Range(-0.1f, 0.1f);
        float zRotation = Random.Range(-30, 30);
        Vector3 hitFxRotation = new Vector3(0, 0, zRotation);
        GameObject cloneHitPrefab = cloneHitFx;
        GameObject newHitFx = Instantiate(cloneHitPrefab, _target.position + new Vector3(xPosition, yPosition), Quaternion.identity, _target);
        if (facingDir == -1)
            newHitFx.transform.Rotate(0, 180, 0);
        newHitFx.transform.Rotate(hitFxRotation);
        Destroy(newHitFx, 0.5f);
    }
}
