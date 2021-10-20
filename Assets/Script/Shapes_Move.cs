using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shapes_Move : MonoBehaviour
{

    private Vector3[,] positionArray = new[,] {
        { new Vector3(2.25f, -0.546f, 5.859375f), new Vector3(1.125f, -1.193f, 5.859375f), new Vector3(0f,-1.84f,5.859375f), new Vector3(-1.125f,-2.487f,5.859375f), new Vector3(-2.25f,-3.1340000000000003f,5.859375f) },
        { new Vector3(2.25f,-3.1340000000000003f,5.859375f), new Vector3(1.125f,-2.487f,5.859375f), new Vector3(0f,-1.84f,5.859375f), new Vector3(-1.125f,-1.193f,5.859375f), new Vector3(-2.25f,-0.546f,5.859375f) } };

    private GameObject[,] shapes = new GameObject[2, 3];

    private GameObject shape_aux;

    private int[,] shape_index_aux = new int[3, 2];
    private int index_save;

    public Vector3[] nextposition;

    private Vector3 position_aux;

    public int[,] currentindex = new int[,] { { 0, 0 }, { 0, 0 }, { 0, 0 } };
    public int[,] nextindex = new int[,] { { 0, 0 }, { 0, 0 }, { 0, 0 } };

    public int[,] center = new int[,] { };

    public int[] currentmove = new int[] { 0, 0 };
    public int[] nextmove = new int[] { 0, 0 };
    public int[] trashmove = new int[] { 0, 0 };

    public bool is_moving = false;

    public bool[] is_limit = new bool[] { false, false };
    public int[] is_limit_index = new int[] { 0, 0 };

    private Queue<int[]> moves = new Queue<int[]>();

    private int k;

    private int l = 3;

    private float smooth = 15;

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

        if (moves.Count != 0 || nextmove[1] != 0)
        {

            if (nextmove[0] == currentmove[0])
            {
                nextmove = moves.Dequeue();
                if (is_limit[nextmove[0]] == true && (is_limit_index[nextmove[0]] + nextmove[1] > 4 || is_limit_index[nextmove[0]] + nextmove[1] < 0))
                {
                    nextmove = new int[] { 0, 0 };
                }
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

            if (is_limit[nextmove[0]] == true)
            {
                nextmove = new int[] { 0, 0 };
            }
        }

        if (currentmove[1] != 0)
        {
            is_moving = true;
            l = 0;
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
                    index_save = i;
                    shapes[Mathf.Abs(nextindex[i, 0] - 1), 1] = shapes[nextindex[i, 0], i];
                    l++;
                }
                else if (nextindex[i, 1] == 0 || nextindex[i, 1] == 4)
                {
                    is_limit[nextindex[i, 0]] = true;
                    is_limit_index[nextindex[i, 0]] = nextindex[i, 1];
                }
                else
                {
                    l++;
                }
            }

            if (l == 3)
            {
                is_limit[currentmove[0]] = false;
            }


            //switch the two gameobjects with their positions

            shape_aux = shapes[nextindex[index_save, 0], 1];
            shapes[nextindex[index_save, 0], 1] = shapes[nextindex[index_save, 0], index_save];
            shapes[nextindex[index_save, 0], index_save] = shape_aux;

            position_aux = nextposition[1];
            nextposition[1] = nextposition[index_save];
            nextposition[index_save] = position_aux;

            currentmove[1] = 0;

            Debug.Log(shapes[0, 0].name + "," + shapes[0, 1].name + "," + shapes[0, 2].name);
            Debug.Log(shapes[1, 0].name + "," + shapes[1, 1].name + "," + shapes[1, 2].name);


        }

        if (is_moving == true)
        {
            k = 0;
            for (int i = 0; i < 3; i++)
            {
                if (shapes[nextindex[i, 0], i].transform.position != nextposition[i])
                {
                    shapes[nextindex[i, 0], i].transform.position = Vector3.MoveTowards(shapes[nextindex[i, 0], i].transform.position, nextposition[i], Time.deltaTime * smooth);
                    k++;
                }
            }
            if (k == 0)
            {
                is_moving = false;
                currentmove[1] = 0;
            }
        }

    }
}
