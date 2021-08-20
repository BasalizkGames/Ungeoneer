using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyScript : MonoBehaviour
{
    public GameObject Player;
    Rigidbody2D rb;
    float step;

    public float speed = 2.5f;
    public int damage;
    public int health;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AIDestinationSetter>().target = Player.transform;
        Player = WorldScript.Instance.PlayerCharacter;
        rb = GetComponent<Rigidbody2D>();
        step = speed * Time.fixedDeltaTime;
        speed = 2.5f;
        damage = 2;
        health = 10;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (WorldScript.Instance.gameRunning)
        {
            //rb.transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, step);
        }
    }
}
