using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : UFOController {

    public float speedFactor;
    public float startDelay;

    // Use this for initialization
    public override void Start () {
        base.Start();
        rb2D = GetComponent<Rigidbody2D>();
        StartCoroutine(StartDelay());
        mask = LayerMask.GetMask("Blocks");

        pathGraph = FindObjectOfType<LevelBuilder>().GetGraph();
        tileSize = FindObjectOfType<DataManager>().GetSelectedImageSet().transform.Find("Tiles").GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x;
        pathToTarget = new List<Position>();
        go = new GameObject();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!GetReset())
        {
            PickupMovement pickUp = FindObjectOfType<PickupMovement>();
            if (pickUp)
            {
                // If the pickUp has changed, reset the boolean saying that the path is leading to the pickup
                if(pickUp != lastPickUp) pickUpFound = false;
                // If the path is already leading to the pickup, don't build it again
                if (!pickUpFound) FindPath(pickUp);
                // At each call, find the current target in the path to get closer to the pickUp
                FindTarget();

                // Add force to move the UFO to the target
                Vector3 direction = target.position - transform.position;
                direction = direction.normalized;
                rb2D.AddForce(new Vector2(direction.x * speedFactor, direction.y * speedFactor));
            }

            rb2D.MoveRotation(rb2D.rotation + rotation);
        }
        else
        {
            rb2D.velocity = Vector3.zero;
        }
    }

    IEnumerator StartDelay()
    {
        SetReset(true);
        yield return new WaitForSeconds(startDelay);
        SetReset(false);
    }

    private void FindTarget()
    {
    // As the path is limited to 20 elements, we can go through the whole path at each call
    // From the farest position to the closest : if the position is visible in straight line by UFO, define it as target else, try with the position closer to UFO in the list
        for (int i = pathToTarget.Count-1; i > 0 ; i--)
        {
            Vector3 direction = new Vector3(pathToTarget[i].xPos * tileSize, pathToTarget[i].yPos * tileSize, 0) - transform.position;
            hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), new Vector2(direction.x, direction.y), direction.magnitude, mask, 0f, 0f);
            if (!hit)
            {
                go.transform.position = new Vector3(pathToTarget[i].xPos * tileSize, pathToTarget[i].yPos * tileSize, 0);
                target = go.transform;
                return;
            }
        }
    }

    private void FindPath(PickupMovement pickup)
    {
        lastPickUp = pickup;
        pickUpFound = false;
        pathToTarget.Clear();
        // Get PickupPosition in grid reference
        Position targetPosition = new Position(Mathf.RoundToInt(pickup.transform.position.x / tileSize), Mathf.RoundToInt(pickup.transform.position.y / tileSize));
        // Get UFO Position in grid reference
        Position ufoPosition = new Position(Mathf.RoundToInt(transform.position.x / tileSize), Mathf.RoundToInt(transform.position.y / tileSize));

        // Add UFO Position to Path list
        pathToTarget.Add(ufoPosition);

        Position nearestPos = new Position();
        // For a path range of 20 positions (to limit the cost of this function)
        for(int i =0;i<20;i++)
        {
            // Search which neighbor of last position in Path list is closest to both pickup and UFO
            Node node = new Node(new Position());
            float minDistUFO = Mathf.Infinity;
            float minDistTarget = Mathf.Infinity;
            foreach (Node n in pathGraph)
            {
                if (n.pos.xPos == pathToTarget[pathToTarget.Count - 1].xPos && n.pos.yPos == pathToTarget[pathToTarget.Count - 1].yPos)
                {
                    node = n;
                }
            }
            foreach(Position p in node.neighbors)
            {
                // Test if the neighbor is already in path
                bool isInPath = false;
                foreach(Position pInPath in pathToTarget)
                {
                    if (pInPath.xPos == p.xPos && pInPath.yPos == p.yPos) isInPath = true;
                }

                // If the neighbor is not already in the path, test its distances to the target and the UFO
                // The neighbor closer to target is selected, if equality, the one closer to UFO is selected
                if (!isInPath)
                {
                    float distUFO = Mathf.Pow(p.xPos - ufoPosition.xPos,2) + Mathf.Pow(p.yPos - ufoPosition.yPos,2);
                    float distTarget = Mathf.Pow(p.xPos - targetPosition.xPos,2) + Mathf.Pow(p.yPos - targetPosition.yPos,2);
                    if (distTarget < minDistTarget)
                    {
                        minDistUFO = distUFO;
                        minDistTarget = distTarget;
                        nearestPos = new Position(p.xPos, p.yPos);
                    }
                    if(distTarget == minDistTarget && distUFO < minDistUFO)
                    {
                        minDistUFO = distUFO;
                        minDistTarget = distTarget;
                        nearestPos = new Position(p.xPos, p.yPos);
                    }
                }
                // If there isn't any neighbor available, (nearestPos is the same as before), add previous position as next position
                if (nearestPos.xPos == node.pos.xPos && nearestPos.yPos == node.pos.yPos) nearestPos = new Position(pathToTarget[pathToTarget.Count-2].xPos, pathToTarget[pathToTarget.Count - 2].yPos);
            }
            // Add nearest neighbor to Path list and end research if the target is found
            if (nearestPos.xPos==targetPosition.xPos && nearestPos.yPos == targetPosition.yPos)
            {
                pathToTarget.Add(targetPosition);
                pickUpFound = true;
                break;
            }
            else
            {
                pathToTarget.Add(nearestPos);
            }
        }
        // The position closest to UFO in the path is defined as the target
        go.transform.position = new Vector3(pathToTarget[1].xPos * tileSize, pathToTarget[1].yPos * tileSize, 0);
        target = go.transform;
    }

    private Rigidbody2D rb2D;
    private PickupMovement lastPickUp;
    private RaycastHit2D hit;
    private Transform target;
    private LayerMask mask;
    private List<Node> pathGraph;
    private List<Position> pathToTarget;
    private float tileSize;
    private GameObject go;
    private bool pickUpFound;
}
