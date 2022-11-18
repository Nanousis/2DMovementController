using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    up,
    down,
    left,
    right,
    noDirection
};

public class PlayerMovement : MonoBehaviour
{
    public SpriteRenderer spRend;
    public Direction currentDirection;
    private Direction calculatedDirection;
    private Vector2 nextPosition;
    private Vector2 prevPosition;
    public float stepDistance;
    private float distanceToPoint = 0;
    public float movesPerSecond=1;

    public AnimationCycle upCycle;
    public AnimationCycle downCycle;
    public AnimationCycle rightCycle;
    private AnimationCycle currentCycle;
    private AnimationCycle prevCycle;
    private float animationTime=0;
    public float framePerCycle;
    public bool isWalking=false;
    private collisionDir dirCollision;
    // Start is called before the first frame update
    void Start()
    {
        prevPosition = transform.position;
        nextPosition = transform.position;
        StartCoroutine(ManageAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        dirCollision = CheckCollision();
        transform.position = Vector2.Lerp(prevPosition, nextPosition, distanceToPoint);
        distanceToPoint += Time.deltaTime * movesPerSecond;

        float Vertical = Input.GetAxis("Vertical");
        float Horizontal = Input.GetAxis("Horizontal");
        if (Mathf.Abs(Horizontal) > 0)
        {
            calculatedDirection = (Horizontal > 0) ? Direction.right:Direction.left;
        }
        else if (Mathf.Abs(Vertical) > 0)
        {
            calculatedDirection = (Vertical > 0) ? Direction.up : Direction.down;
        }
        else
        {
            calculatedDirection = Direction.noDirection;
        }
        bool switchDir = false;
        switch(calculatedDirection)
        {
            case Direction.up:
                switchDir = (currentDirection == Direction.down);
                break;

            case Direction.down:
                switchDir = (currentDirection == Direction.up);
                break;

            case Direction.left:
                switchDir = (currentDirection == Direction.right);
                break;

            case Direction.right:
                switchDir = (currentDirection == Direction.left);
                break;

        }
        if (switchDir)
        {
            distanceToPoint = 1f - distanceToPoint;
        }
        if((Vector2)transform.position == nextPosition)
        {
            distanceToPoint = 0;
            switchDir = true;
        }

        if (switchDir)
        {
            currentDirection = calculatedDirection;
            prevPosition = nextPosition;
            isWalking = true;
            switch (currentDirection)
            {
                case Direction.up:
                    if(!dirCollision.up)
                    nextPosition = (Vector2)nextPosition + (stepDistance * Vector2.up);
                    break;
                case Direction.down:
                    if (!dirCollision.down)
                        nextPosition = (Vector2)nextPosition + (stepDistance * Vector2.down);
                    break;
                case Direction.left:
                    if (!dirCollision.left)
                        nextPosition = (Vector2)nextPosition + (stepDistance * Vector2.left);
                    break;
                case Direction.right:
                    if (!dirCollision.right)
                        nextPosition = (Vector2)nextPosition + (stepDistance * Vector2.right);
                    break;
                case Direction.noDirection:
                    isWalking = false;
                    nextPosition = prevPosition;
                    break;
            }

        }
    }
    IEnumerator ManageAnimation()
    {
        switch (currentDirection)
        {
            case Direction.up:
                currentCycle = upCycle;
                break;

            case Direction.down:
                currentCycle = downCycle;
                break;
            case Direction.left:
                spRend.flipX = true;
                currentCycle = rightCycle;
                break;
            case Direction.right:

                spRend.flipX = false;
                currentCycle = rightCycle;
                break;
        }
        if (isWalking)
        {
            if (animationTime >= (1f / framePerCycle)||prevCycle!=currentCycle)
            {
                spRend.sprite = currentCycle.sprites[currentCycle.currentSprite];
                if ((++currentCycle.currentSprite) == currentCycle.sprites.Length)
                {
                    currentCycle.currentSprite = 0;
                }
                animationTime = 0;
            }
        }
        else
        {
            spRend.sprite = currentCycle.sprites[0];
            currentCycle.currentSprite = 0;
            animationTime = 1;
        }
        prevCycle = currentCycle;
        animationTime += Time.deltaTime;
        yield return new WaitForEndOfFrame();
        StartCoroutine(ManageAnimation());
    }
    collisionDir CheckCollision()
    {
        collisionDir col = new collisionDir();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, stepDistance);
        col.up = (hit.collider != null);
         hit = Physics2D.Raycast(transform.position, Vector2.down, stepDistance);
        col.down = (hit.collider != null);
         hit = Physics2D.Raycast(transform.position, Vector2.left, stepDistance);
        col.left = (hit.collider != null);
         hit = Physics2D.Raycast(transform.position, Vector2.right, stepDistance);
        col.right = (hit.collider != null);
        return col;
    }
}
[System.Serializable]
public class AnimationCycle
{
    public Sprite[] sprites;
    public int currentSprite;
}
public class collisionDir
{
    public bool up;
    public bool down;
    public bool left;
    public bool right;
}