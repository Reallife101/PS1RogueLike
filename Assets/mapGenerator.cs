using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class mapGenerator : MonoBehaviour
{
    /*  0 = Nothing
     *  1 = N
     *  2 = E
     *  3 = S
     *  4 = W
     *  5 = NE
     *  6 = NS
     *  7 = NW
     *  8 = ES
     *  9 = EW
     *  10 = SW
     *  11 = NES
     *  12 = NEW
     *  13 = NSW
     *  14 = ESW
     *  15 = NESW
     * 
     */

    public List<GameObject> Nothing;
    public List<GameObject> N;
    public List<GameObject> E;
    public List<GameObject> S;
    public List<GameObject> W;
    public List<GameObject> NE;
    public List<GameObject> NS;
    public List<GameObject> NW;
    public List<GameObject> ES;
    public List<GameObject> EW;
    public List<GameObject> SW;
    public List<GameObject> NES;
    public List<GameObject> NEW;
    public List<GameObject> NSW;
    public List<GameObject> ESW;
    public List<GameObject> NESW;
    public List<List<GameObject>> rooms = new List<List<GameObject>>();

    public GameObject player;


    public static int WIDTH = 5;
    public static int HEIGHT = 5;
    public static int ROOM_LENGTH = 20;

    //Declare 2d array of booleans for dirs
    private static bool[,] dirs = new bool[,] { { false, false, false, false }, { true, false, false, false }, { false, true, false, false },
        { false, false, true, false }, { false, false, false, true }, { true, true, false, false }, { true, false, true, false },
        { true, false, false, true }, { false, true, true, false }, { false, true, false, true }, { false, false, true, true }, 
        { true, true, true, false }, { true, true, false, true }, { true, false, true, true }, { false, true, true, true }, 
        { true, true, true, true } };

    private static bool[,,] bans = new bool[WIDTH, HEIGHT, 4];
    private static bool[,,] req = new bool[WIDTH, HEIGHT, 4];
    private static int[,] gen = new int[WIDTH, HEIGHT];
    private static List<Vector2> remaining_rooms = new List<Vector2>();

    //------------ Setup rooms list --------------
    public void setup_rooms()
    {
        rooms.Add(Nothing);
        rooms.Add(N);
        rooms.Add(E);
        rooms.Add(S);
        rooms.Add(W);
        rooms.Add(NE);
        rooms.Add(NS);
        rooms.Add(NW);
        rooms.Add(ES);
        rooms.Add(EW);
        rooms.Add(SW);
        rooms.Add(NES);
        rooms.Add(NEW);
        rooms.Add(NSW);
        rooms.Add(ESW);
        rooms.Add(NESW);
    }


    //------------Instantiate everything--------------
    public static void setup()
    {
        //Set up all maps
        for (int i = 0; i<WIDTH; i++)
        {
            for (int j =0; j<HEIGHT; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    bans[i, j, k] = false;
                    req[i, j, k] = false;
                }
                gen[i, j] = 0;
            }
        }

        // Set Horizontal bans
        for (int i = 0; i < WIDTH; i++)
        {
            bans[0, i, 0] = true;
            bans[HEIGHT-1, i, 2] = true;
        }

        // Set Vertical bans
        for (int i = 0; i < HEIGHT; i++)
        {
            bans[i, 0, 3] = true;
            bans[i, WIDTH-1, 1] = true;
        }

    }

    //--------------- Update bans and req and remaining -------------
    public static void updateRemaining(int cell, int row, int col)
    {
        // Set Up ban
        for (int k = 0; k < 4; k++)
        {
            if (dirs[cell, k])
            {
                if (k==0)
                {
                    if (row-1 >=0)
                    {
                        req[row - 1, col, 2] = true;
                        if (gen[row-1, col] == 0 && !remaining_rooms.Contains(new Vector2(row-1, col)))
                        {
                            remaining_rooms.Add(new Vector2(row - 1, col));
                        }
                    }
                }
                else if (k==2)
                {
                    if (row + 1 < HEIGHT)
                    {
                        req[row + 1, col, 0] = true;
                        if (gen[row + 1, col] == 0 && !remaining_rooms.Contains(new Vector2(row + 1, col)))
                        {
                            remaining_rooms.Add(new Vector2(row + 1, col));
                        }
                    }

                }
                else if (k==1)
                {
                    if (col + 1 < WIDTH)
                    {
                        req[row, col+1, 3] = true;
                        if (gen[row, col + 1] == 0 && !remaining_rooms.Contains(new Vector2(row, col + 1)))
                        {
                            remaining_rooms.Add(new Vector2(row, col + 1));
                        }
                    }
                }
                else if(k == 3)
                {
                    if (col - 1 >= 0)
                    {
                        req[row, col - 1, 1] = true;
                        if (gen[row, col - 1] == 0 && !remaining_rooms.Contains(new Vector2(row, col - 1)))
                        {
                            remaining_rooms.Add(new Vector2(row, col - 1));
                        }
                    }
                }
            }

            if (!dirs[cell, k])
            {
                if (k == 0)
                {
                    if (row - 1 >= 0)
                    {
                        bans[row - 1, col, 2] = true;
                    }
                }
                else if (k == 2)
                {
                    if (row + 1 < HEIGHT)
                    {
                        bans[row + 1, col, 0] = true;
                    }

                }
                else if (k == 1)
                {
                    if (col + 1 < WIDTH)
                    {
                        bans[row, col + 1, 3] = true;
                    }
                }
                else if (k == 3)
                {
                    if (col - 1 >= 0)
                    {
                        bans[row, col - 1, 1] = true;
                    }
                }
            }
        }


    }

    //--------------- Generate the Map -----------------
    public static void genMap()
    {
        int[] startRooms = new int[] { 11, 12, 13, 14, 15 };

        int start_row = (WIDTH - 1) / 2;
        int start_col = (HEIGHT - 1) / 2;
        int random_cell = startRooms[Random.Range(0, startRooms.Length)];

        gen[start_row, start_col] = random_cell;

        updateRemaining(random_cell, start_row, start_col);

        //Branch off
        while (remaining_rooms.Count > 0)
        {
            List<int> possible_rooms = new List<int>();

            Vector2 point = remaining_rooms[0];
            for (int i = 0; i < 16; i++)
            {
                bool flag = true;

                for (int j = 0; j < 4; j++)
                {
                    if ((req[(int)point.x, (int)point.y, j] && !dirs[i, j]) || (bans[(int)point.x, (int)point.y, j] && dirs[i, j]))
                    {
                        flag = false;
                    }
                }

                if (flag)
                {
                    possible_rooms.Add(i);
                }
            }

            //set cells
            random_cell = possible_rooms[Random.Range(0, possible_rooms.Count)];
            gen[(int)point.x, (int)point.y] = random_cell;

            updateRemaining(random_cell, (int)point.x, (int)point.y);
            remaining_rooms.RemoveAt(0);

        }

    }

    
    //---------------------- Print 2D array ----------------------------
    public static void Print2DArray<T>(T[,] matrix)
    {

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                sb.Append(matrix[i, j]);
                sb.Append(' ');
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }

    //--------------- Build Map ---------------
    public void buildmap(int[,] matrix)
    {
        player.transform.position = new Vector3((WIDTH - 1) / 2*ROOM_LENGTH, 0f, -((HEIGHT - 1) / 2 * ROOM_LENGTH));

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                int rand = Random.Range(0, rooms[matrix[i, j]].Count);
                Instantiate(rooms[matrix[i, j]][rand], new Vector3(j*ROOM_LENGTH, (float)-0.5, -i*ROOM_LENGTH), Quaternion.identity);
                
            }
        }
    }


    //private static bool[,,] bans;
    // Start is called before the first frame update
    void Start()
    {
        setup_rooms();
        setup();
        genMap();
        //Print2DArray<int>(gen);
        buildmap(gen);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
