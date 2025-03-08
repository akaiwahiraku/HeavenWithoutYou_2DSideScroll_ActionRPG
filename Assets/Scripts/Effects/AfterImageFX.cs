using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageFX : MonoBehaviour
{
    private SpriteRenderer sr;
    private float colorLooseRate;
    private Vector3 velocity;

    public void SetupAfterImage(float _loosingSpeed, Sprite _spriteImage, Vector3 _velocity)
    {
        sr = GetComponent<SpriteRenderer>();

        sr.sprite = _spriteImage;
        colorLooseRate = _loosingSpeed;
        velocity = _velocity * 0.2f; // �v���C���[�̑��x���󂯎��AAfterImage�ɓK�p�B���x�͒x�߂ɂ���


    }

    private void Update()
    {
        // AfterImage���v���C���[�̑��x�ɉ����Ĉړ�������
        transform.position += velocity * Time.deltaTime;

        float alpha = sr.color.a - colorLooseRate * Time.deltaTime;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

        if(sr.color.a <= 0)
            Destroy(gameObject);
    }
}
