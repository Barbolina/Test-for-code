using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int Movment_speed; // - Скорость передвижение персонажа по горизонтали.
    [SerializeField] private int Max_dis_jump; // - Максимальная высота прыжка персонажа.

    [SerializeField] private int Jump_speed; // - Скорость перемещение персонажа в прыжке.
    [SerializeField] private int Fall_speed; // - Скорость падения персонажа.

    private Rigidbody2D rigidbodyPlayer;

    private int Look_right = 1;

    private bool In_Ground;
    [SerializeField] private Transform Transform_Feet;
    [SerializeField] private float Check_Radius;
    [SerializeField] private LayerMask Is_Ground;

    private Animator animatorPlayer;
    private Transform transformPlayer;
    
    [Header("Атака")]

    [SerializeField] private float Attack_speed; //- Время(анимации) удара. (ПУ 0.2 сек.)
    [SerializeField] private float Attack_movement_after; //- Дистанция на которую переместится персонаж после удара.
    [SerializeField] private float Attack_cooldown; //- Перезарядка атаки. (ПУ 1 сек.)
    [SerializeField] private Transform Attack_position;
    [SerializeField] private float Attack_radius;
    [SerializeField] private LayerMask Attack_layers;

    private Collider2D[] Attack_players;
    private bool Can_attack = true;
    private bool Can_Move = true;
    void Start()
    {
        Attack_position = gameObject.transform.GetChild(0);
        animatorPlayer = gameObject.GetComponent<Animator>();

        rigidbodyPlayer = gameObject.GetComponent<Rigidbody2D>();
        transformPlayer = gameObject.GetComponent<Transform>();
    }

    
    void Update()
    {
        In_Ground = Physics2D.OverlapCircle(Transform_Feet.position, Check_Radius, Is_Ground);
        animatorPlayer.SetBool("OnGround", In_Ground);
        
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
        {
            animatorPlayer.SetBool("IsRun", false);
            
        }

        if (Can_Move)
        {
            Jump();
            GoLeft();
            GoRight();
        }
        Attack();
        No_fall_acceleration();
    }

    private void No_fall_acceleration()
    {
        if (rigidbodyPlayer.velocity.magnitude > Fall_speed)   
        {
            rigidbodyPlayer.velocity = rigidbodyPlayer.velocity.normalized * Fall_speed;
        }
    }

    private void GoRight()
    {
        if (Input.GetKey(KeyCode.D))
        {
            Look_right = 1;
            animatorPlayer.SetBool("IsRun", true);
            transformPlayer.rotation = Quaternion.Euler(0, 0, 0);
            if (In_Ground)
            {
                rigidbodyPlayer.velocity = new Vector2(1 * Movment_speed, rigidbodyPlayer.velocity.y);
            }
            else
            {
                rigidbodyPlayer.velocity = new Vector2(1 * Jump_speed, rigidbodyPlayer.velocity.y);
            }
        } 
    }
    private void GoLeft()
    {

        if (Input.GetKey(KeyCode.A))
        {
            Look_right = -1;
            animatorPlayer.SetBool("IsRun", true);
            transformPlayer.rotation = Quaternion.Euler(0, 180, 0);
            if (In_Ground)
            {
                rigidbodyPlayer.velocity = new Vector2(-1 * Movment_speed, rigidbodyPlayer.velocity.y);
            }
            else
            {
                rigidbodyPlayer.velocity = new Vector2(-1 * Jump_speed, rigidbodyPlayer.velocity.y);
            }
        } 
    }

    private void Jump()
    {
        if (In_Ground && Input.GetKeyDown(KeyCode.Space))
        {
            rigidbodyPlayer.velocity = new Vector2(rigidbodyPlayer.velocity.x, Max_dis_jump );
        }
        if (Input.GetKeyUp(KeyCode.Space) && rigidbodyPlayer.velocity.y > 0)
        {
            rigidbodyPlayer.velocity = new Vector2(rigidbodyPlayer.velocity.x, 0);
        }
        if (rigidbodyPlayer.velocity.y > 0)
        {
            animatorPlayer.SetBool("IsJump", true);
        }
        else { animatorPlayer.SetBool("IsJump", false); }

    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0) && Can_attack)
        {
            Can_attack = false;
            Can_Move = false;
            animatorPlayer.SetTrigger("IsAttack");
            Attack_players = Physics2D.OverlapCircleAll(Attack_position.position, Attack_radius, Attack_layers);
            rigidbodyPlayer.velocity = new Vector2(Attack_movement_after * Look_right, rigidbodyPlayer.velocity.y);

            foreach (Collider2D enemy in Attack_players)
            {
                enemy.GetComponent<OtherPlayer>().Kill();
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(Attack_position.position, Attack_radius);
    }
    private void OnAttack()
    {
        //animatorPlayer.ResetTrigger("IsAttack");
        Can_Move = true;
        StartCoroutine("Timer_cooldown");
    }

    private IEnumerator Timer_cooldown()
    {
        yield return new WaitForSeconds(Attack_cooldown);
        Can_attack = true;
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
