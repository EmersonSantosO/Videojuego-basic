using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction;
    private new Animator animation;
    private Vector2 movementDirection;
    private Vector2 direccionDmg;
    private SpriteRenderer sp;

    [Header("Stats")]
    public float movementVelocity = 5;
    public float jumpForce = 7;
    public int vidas = 3;
    public float tiempoInmortal;

    [Header("Collitions")]
    public LayerMask layerGround;
    public Vector2 groundPosition;
    public float collitionRange;

    [Header("Booleans")]
    public bool moveState = true;
    public bool jumping = true;
    public bool isAttacking = false;
    public bool inmortal;
    public bool aplicarFuerza;

	private void Awake()
	{
        rb = GetComponent<Rigidbody2D>();
        animation = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();
	}

    public void Morir()
    {
        if (vidas > 0)
        {
            return;
        }

        animation.SetBool("muerto", true);
        StartCoroutine(diediedie());
        
    }

    private IEnumerator diediedie() {
        this.enabled = false;
        Destroy(rb);
        yield return new WaitForSeconds(3);
        GameManager.instance.GameOver();
        
    }

    public void recibirDmg()
    {
        StartCoroutine(impactoDmg(Vector2.zero));
    }

    public void recibirDmg(Vector2 direccion)
    {
        StartCoroutine(impactoDmg(direccion));
    }



    private IEnumerator impactoDmg(Vector2 direccion)
    {
        if (!inmortal)
        {
            StartCoroutine(Inmortalidad());
            vidas--;
            float velocidadAuxiliar = movementVelocity;
            this.direccionDmg = direccion;
            
            Time.timeScale = 0.4f;
            yield return new WaitForSeconds(0.2f);
            Time.timeScale = 1;

            for (int i = GameManager.instance.vidasUI.transform.childCount - 1; i >= 0; i--)
            {
                if (GameManager.instance.vidasUI.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    GameManager.instance.vidasUI.transform.GetChild(i).gameObject.SetActive(false);
                    break;
                }
            }
            
            movementVelocity = velocidadAuxiliar;
            Morir();
        }
    }

    public void DarInmortalidad()
    {
        StartCoroutine(Inmortalidad());
    }

    public IEnumerator Inmortalidad()
    {
        inmortal = true;
        float tiempoTranscurrido = 0;
        
        while(tiempoTranscurrido < this.tiempoInmortal)
        {
            sp.color = new Color(1, 1, 1, .5f);
            yield return new WaitForSeconds(this.tiempoInmortal /20);
            sp.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(this.tiempoInmortal / 20);
            tiempoTranscurrido += this.tiempoInmortal / 10;
        }

        inmortal = false;
    }

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        isOnGround();
    }

    private void attack(Vector2 direction)
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!isAttacking)
            {
                isAttacking = true;

                animation.SetBool("attack", true);
            }
        }
    }

    public void finishAttack()
    {
        animation.SetBool("attack", false);
        isAttacking = false; 
    }

    private Vector2 attackDirection(Vector2 movementDirection, Vector2 directionRaw)
    {
        if(rb.velocity.x == 0 && directionRaw.y != 0)
        {
            return new Vector2(0, directionRaw.y);
        }

        return new Vector2(movementDirection.x, directionRaw.y);
    }


    private void Movement() 
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        direction = new Vector2(x, y);
        Vector2 directionRaw = new Vector2(xRaw, yRaw);

        Walk();
        attack(attackDirection(movementDirection, directionRaw));

        upgratedJump();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (jumping)
            {
                animation.SetBool("jumping", true);
                jump();
            }
        }

        float velocity;
        if (rb.velocity.y > 0)
        {
            velocity = 1;
        }
        else
        {
            velocity = -1;
        }

        if (!jumping)
        {
            animation.SetFloat("verticalVelocity", velocity);
        }
        else
        {
            if (velocity == -1)
            {
                animation.SetBool("jumping", false);
            }
        }
 
    }

    private void Walk()
    {
        if (moveState == true)
        {
            rb.velocity = new Vector2(direction.x * movementVelocity, rb.velocity.y);

            if(direction != Vector2.zero)
            {
                if (jumping)
                {
                    animation.SetBool("walking", true);
                }
                else
                {
                    animation.SetBool("jumping", true);
                }

                if(direction.x < 0 && transform.localScale.x > 0)
                {
                    movementDirection = attackDirection(Vector2.left, direction);
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                }else if(direction.x > 0 && transform.localScale.x < 0)
                {
                    movementDirection = attackDirection(Vector2.right, direction);
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                }
            }
            else
            {
                if(direction.y > 0 && direction.x == 0)
                {

                }
                animation.SetBool("walking", false);
            }
        }
    }

    private void isOnGround()
    {
        jumping = Physics2D.OverlapCircle((Vector2)transform.position + groundPosition, collitionRange, layerGround);
    }

    public void upgratedJump()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.5f - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.0f - 1) * Time.deltaTime;
        }
    }

    private void jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * jumpForce;
    }
}
