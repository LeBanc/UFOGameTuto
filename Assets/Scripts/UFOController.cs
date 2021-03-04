using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOController : MonoBehaviour {
    
    public float rotation = 2.0f;
    public float maxDamage = 5;
    public Sprite[] ufoSprites;

    public virtual void Start()
    {
        origin = transform.position;
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            if (!other.gameObject.GetComponent<ObjectController>().IsDying())
            {
                AddScore(other.gameObject.GetComponent<ObjectController>().GetScore());
                other.gameObject.GetComponent<ObjectController>().Die();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {
            if (invicible) return;
            damage += 1;
            if (damage > maxDamage)
            {
                StartCoroutine(Respawn());
                return;
            }
            StartCoroutine(IsDamaged());
        }
    }

    IEnumerator IsDamaged()
    {
        invicible = true;
        anim.SetBool("isDamaged", true);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("isDamaged", false);
        Color cracks = new Color(1, 1, 1, 0.8f*(damage / maxDamage));
        transform.Find("Cracks").GetComponent<SpriteRenderer>().color = cracks;
        invicible = false;
    }

    IEnumerator Respawn()
    {
        reset = true;
        Color cracks = new Color(1, 1, 1, 0);
        transform.Find("Cracks").GetComponent<SpriteRenderer>().color = cracks;
        anim.SetBool("isDead", true);
        GetComponent<CircleCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        transform.position = origin;
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(1f);
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
        damage = 0;
        reset = false;
        anim.SetBool("isDead", false);
    }

    private void AddScore(float newScore)
    {
        score += newScore;
        FindObjectOfType<GameManager>().DisplayScore(score, ufoNumber);
    }

    public void SetUFONumber(int num)
    {
        ufoNumber = num;
        GetComponent<SpriteRenderer>().sprite = ufoSprites[num - 1];
    }

    public bool GetReset()
    {
        return reset;
    }

    public void SetReset(bool var)
    {
        reset = var;
    }

    private Vector3 origin;
    private float damage = 0;
    private bool invicible = false;
    private int ufoNumber = 1;
    private float score = 0f;
    private bool reset = false;
    private Animator anim;
}
