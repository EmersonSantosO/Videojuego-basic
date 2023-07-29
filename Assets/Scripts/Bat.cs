using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Bat : MonoBehaviour
{
    private CinemachineVirtualCamera cm;
    private SpriteRenderer sp;
    private PlayerController player;
    private Rigidbody2D rb;
    private bool recoil;

    public float movementVelocity = 3;
    public float radioDetection = 15;
    public LayerMask playerLayer;

    public int hp = 1;
    public string nombre;

    private void Awake()
    {
        cm = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineVirtualCamera>();
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }


    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = nombre;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = player.transform.position - transform.position;
        float distance = Vector2.Distance(transform.position, player.transform.position);

        if(distance <= radioDetection)
        {
            rb.velocity = direction.normalized * movementVelocity;
            changeView(direction.normalized.x);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }


    }

    private void changeView(float directionX)
    {
        if(directionX < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if(directionX > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.recibirDmg((transform.position - player.transform.position).normalized);
            recoil = true;
        }
    }

    private void FixedUpdate()
    {
        if (recoil)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * 100, ForceMode2D.Impulse);
            recoil = false;
        }
    }

    public void takeDamage()
    {
        //StartCoroutine(cameraShake(0.1f));
        if (hp > 0)
        {
            StartCoroutine(damageEffect());
            recoil = true;
            hp--;
        }
        else
        {
            recoil = true;
            Destroy(gameObject, 0.2f);
        }
    }

    private IEnumerator cameraShake(float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
        yield return new WaitForSeconds(time);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
    }

    private IEnumerator damageEffect()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.4f);
        sp.color = Color.white;
    }
}
