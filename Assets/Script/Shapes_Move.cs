using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shapes_Move : MonoBehaviour
{

    private Vector3[,] positionArray = new[,] {
        { new Vector3(2.25f, -0.546f, 5.859375f), new Vector3(1.125f, -1.193f, 5.859375f), new Vector3(0f,-1.84f,5.859375f), new Vector3(-1.125f,-2.487f,5.859375f), new Vector3(-2.25f,-3.1340000000000003f,5.859375f) },
        { new Vector3(2.25f,-3.1340000000000003f,5.859375f), new Vector3(1.125f,-2.487f,5.859375f), new Vector3(0f,-1.84f,5.859375f), new Vector3(-1.125f,-1.193f,5.859375f), new Vector3(-2.25f,-0.546f,5.859375f) } };

    private GameObject[,] shapes = new GameObject[2, 3];

    public Vector3[] nextposition;

    public int[,] currentindex = new int[,] { { 0, 0 }, { 0, 0 }, { 0, 0 } };
    public int[,] nextindex = new int[,] { { 0, 0 }, { 0, 0 }, { 0, 0 } };

    public int[] currentmove = new int[] { 0, 0 };
    public int[] nextmove = new int[] { 0, 0 };

    public bool is_moving = false;
    public bool[,] is_limit = new bool[2, 2];

    private Queue<int[]> moves = new Queue<int[]>();

    private int k;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            shapes[this.gameObject.transform.GetChild(i).gameObject.GetComponent<Shape_Properties>().position[0], this.gameObject.transform.GetChild(i).gameObject.GetComponent<Shape_Properties>().position[1] - 1] = this.gameObject.transform.GetChild(i).gameObject;
            if (this.gameObject.transform.GetChild(i).gameObject.GetComponent<Shape_Properties>().position[1] == 2)
            {
                shapes[this.gameObject.transform.GetChild(i).gameObject.GetComponent<Shape_Properties>().position[0] + 1, this.gameObject.transform.GetChild(i).gameObject.GetComponent<Shape_Properties>().position[1] - 1] = this.gameObject.transform.GetChild(i).gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float H_middle = Screen.height / 3;
            float W_middle = Screen.width / 2;
            if (touch.phase == TouchPhase.Began)
            {
                if (touch.position.x > W_middle && touch.position.y > H_middle)
                {
                    moves.Enqueue(new int[] { 0, -1 });
                }
                if (touch.position.x > W_middle && touch.position.y < H_middle)
                {
                    moves.Enqueue(new int[] { 1, -1 });
                }
                if (touch.position.x < W_middle && touch.position.y > H_middle)
                {
                    moves.Enqueue(new int[] { 1, 1 });
                }
                if (touch.position.x < W_middle && touch.position.y < H_middle)
                {
                    moves.Enqueue(new int[] { 0, 1 });
                }
            }
        }

        if (moves.Count != 0)
        {
            if (nextmove[0] == currentmove[0])
            {
                nextmove = moves.Dequeue();
                if (nextmove[0] == currentmove[0] || is_moving == false)
                {
                    currentmove = nextmove;
                }
            }
            else
            {
                if (is_moving == false)
                {
                    currentmove = nextmove;
                }
            }
        }

        if (currentmove[1] != 0)
        {
            Debug.Log(currentmove[1]);
            is_moving = true;
            for (int i = 0; i < 3; i++)
            {
                currentindex[i, 0] = currentmove[0];
                currentindex[i, 1] = shapes[currentmove[0], i].GetComponent<Shape_Properties>().position[1];

                nextindex[i, 0] = currentindex[i, 0];
                nextindex[i, 1] = currentindex[i, 1] + currentmove[1];

                /* if (nextindex[1] == 0 || nextindex[1] == 4)
                {
                    is_limit = true;
                } */

                nextposition[i] = positionArray[nextindex[i, 0], nextindex[i, 1]];

                shapes[nextindex[i, 0], i].GetComponent<Shape_Properties>().position[0] = nextindex[i, 0];
                shapes[nextindex[i, 0], i].GetComponent<Shape_Properties>().position[1] = nextindex[i, 1];

                if (nextindex[i, 1] == 2)
                {
                    shapes[Mathf.Abs(nextindex[i, 0] - 1), 1] = shapes[nextindex[i, 0], i];
                }
            }
            currentmove = new int[] { 0, 0 };

        }

        if (is_moving == true)
        {
            k = 0;
            for (int i = 0; i < 3; i++)
            {
                if (shapes[nextindex[i, 0], i].transform.position != nextposition[i])
                {
                    shapes[nextindex[i, 0], i].transform.position = Vector3.MoveTowards(shapes[nextindex[i, 0], i].transform.position, nextposition[i], Time.deltaTime);
                    k += 1;
                }
            }
            if (k == 0)
            {
                is_moving = false;
                currentmove = new int[] { 0, 0 };
            }
        }

    }
}
