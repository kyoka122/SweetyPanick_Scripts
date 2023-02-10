using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoCandyMove : MonoBehaviour
{
    // set from inspecter
    // move speeds
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float jumpSpeed;

    // particle prefabs
    [SerializeField]
    private GameObject jumpParticle;
    [SerializeField]
    private GameObject landingParticle;
    [SerializeField]
    private GameObject dashParticle;
    [SerializeField]
    private GameObject atkParticle;
    [SerializeField]
    private GameObject skillParticle;

    private ParticleSystem dashpar;
 
    // component
    private Animator animator;
    private Rigidbody2D rb;

    // horizontal move
    private float directionInput; // horizontal key input
    private Vector3 scale; // object scale for inversion
    private Vector3 modelScale; // charactor model scale for inversion
    
    // jump
    private bool isLanding;
  
    // yawn
    private float time;
    private float duration = 20f; // yawn duration

    void Start()
    {
        Application.targetFrameRate = 60;

        animator = this.gameObject.GetComponentInChildren<Animator>();
        rb = this.gameObject.GetComponent<Rigidbody2D>();

        scale = this.transform.localScale;
        modelScale = animator.gameObject.transform.localScale;

        GameObject dashparticleObj = Instantiate(dashParticle, this.transform);
        dashparticleObj.transform.parent = this.transform;
        dashpar = dashparticleObj.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        directionInput = Input.GetAxis("Horizontal");
        
        time += Time.deltaTime;

        // WALK or RUN
        if(directionInput != 0)
        {
            InvertRigidbody();
            InvertModel();

            if(Input.GetKey(KeyCode.LeftShift))
            {
                dashpar.Play();
                animator.SetBool("IsRunning", true);
                rb.velocity = new Vector2(runSpeed * directionInput, rb.velocity.y);
            } else {
                dashpar.Stop();
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsWalking", true);
                rb.velocity = new Vector2(walkSpeed * directionInput, rb.velocity.y);
            }

            time = 0;
        } else {
            dashpar.Stop();
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }

        // JUMP
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(jumpParticle, this.transform);
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        // YAWN
        if(duration < time)
        {
            animator.SetTrigger("HadWaited");
            time = 0;
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            Instantiate(atkParticle, this.transform);
            rb.velocity = Vector3.zero;
            animator.SetTrigger("HadATKButtonPressed");
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            Instantiate(skillParticle, this.transform);
            rb.velocity = Vector3.zero;
            animator.SetTrigger("HadSkillButtonPressed");
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            rb.velocity = Vector3.zero;
            animator.SetTrigger("HadCureButtonPressed");
        }
    }

    void FixedUpdate()
    {
        // landing detection
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, -1f * Vector2.up, 2f);
        if(hit.collider == null)
        {
            isLanding = false;
            dashpar.Stop();
            
            animator.SetFloat("FallSpeed", rb.velocity.y);
        }

        if(rb.velocity.y == 0 && !isLanding)
        {
            Instantiate(landingParticle, this.transform);
            
            animator.SetFloat("FallSpeed", 0f);
            isLanding = true;
        }
    }


    /* Invert Object */
    private void InvertRigidbody()
    {
        if(0 < directionInput)
        {
            scale.x = Mathf.Abs(scale.x) * -1f;
        } else {
            scale.x = Mathf.Abs(scale.x);
        }

        this.transform.localScale = scale;
    }

    /* Invert Charactor Sprites(Prevent UnityChanSpringBone from collapsing) */
    private void InvertModel()
    {
        if(0 < directionInput)
        {
            modelScale.x = Mathf.Abs(modelScale.x) * -1f;
            animator.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        } else {
            modelScale.x = Mathf.Abs(modelScale.x);
            animator.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        animator.gameObject.transform.localScale = modelScale; 
    }
}
