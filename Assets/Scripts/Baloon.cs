using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baloon : MonoBehaviour
{
    [SerializeField] private float MOVE_SPEED = 6.0f;

    private SpriteRenderer spriteRenderer;
    private RectTransform rectTransform;
    private BoxCollider2D boxCollider2D;

    [HideInInspector]
    public int color;
    
    [HideInInspector]
    public bool moving = false;

    [HideInInspector]
    public bool destroy = false;

    [HideInInspector]
    public Vector3 target;
    
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rectTransform = GetComponent<RectTransform>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, MOVE_SPEED * Time.deltaTime);
            if (transform.position == target)
            {
                moving = false;
            }
        }
        else if (destroy)
        {
            animator.SetBool("IsDestroying", true);
        }
    }
    public void OnDestroyEvent()
    {
        destroy = false;
    }

    public bool IsThis(Vector3 coord)
    {
        coord.z = boxCollider2D.bounds.center.z;
        return boxCollider2D.bounds.Contains(coord);
    }
    
    public void SetSwap(Vector3 target)
    {
        this.target = target;
        this.moving = true;
    }
}
