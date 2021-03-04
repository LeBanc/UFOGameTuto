using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour {

    public float score;

    private void LateUpdate()
    {
        if(m_isDead) Destroy(gameObject);
    }

    public float GetScore()
    {
        if (!m_isDead)
        {
            return score;
        }
        else
        {
            return 0f;
        }
    }

    public void Die()
    {
        if (!m_isDead)
        {
            m_isDying = true;
            StartCoroutine(Dying());
            FindObjectOfType<GameManager>().PickUpDestroyed();
        }
    }

    IEnumerator Dying()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if(sr) sr.enabled = false;
        CircleCollider2D cc2D = GetComponent<CircleCollider2D>();
        if (cc2D) cc2D.enabled = false;
        AudioSource audio = GetComponent<AudioSource>();
        if (audio)
        {
            audio.Play();
            while (audio.isPlaying) yield return new WaitForSeconds(Time.deltaTime);
        }
        m_isDead = true;
    }

    public bool IsDying()
    {
        return m_isDying;
    }

    private bool m_isDead = false;
    private bool m_isDying = false;

}
