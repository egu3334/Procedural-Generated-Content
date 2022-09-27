using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int randomSeed;
    public int gridLength;
    public int timesteps;
    public bool[,] grid;
    public int chestCount;
    public int tileLength;
    public int wallHeight;
    public TextAsset floorText;
    public TextAsset wallText;
    public TextAsset riverText;
    public TextAsset lilypadText;
    public GameObject treasureChest;
    public bool riverOn;

    private int maxArea = 0;
    private float threshhold = 0.45f;
    private int maxLabel;
    private int[,] visited;
    private Texture2D floor;
    private Texture2D wall;
    private Texture2D river;
    private Texture2D lilypad;
    private Vector3[] verts;
	private int[] tris;
    
    // Start is called before the first frame update
    void Start()
    {
        Random.seed = randomSeed;
        gridLength = Mathf.Max(gridLength, 5);

        while ((float)(maxArea) / (float)(gridLength * gridLength) < threshhold) {
            grid = new bool[gridLength, gridLength];
            for (int i = 0; i < gridLength; i++) {
                for (int j = 0; j < gridLength; j++) {
                    grid[i, j] = Random.Range(0.00f, 1.00f) > 0.40f;
                }
            }

            int step = 0;
            while (step < timesteps) {
                bool[,] newGrid = new bool[gridLength, gridLength];
                for (int i = 0; i < gridLength; i++) {
                    for (int j = 0; j < gridLength; j++) {
                        int neighbors = neighborCount(i, j);
                        if (grid[i, j]) {
                            newGrid[i, j] = neighbors > 4 ? false: true;
                        } else {
                            newGrid[i, j] = neighbors < 3 ? true: false;
                        }
                    }
                }
                grid = newGrid;
                step += 1;
            }

            visited = new int[gridLength, gridLength];
            int label = 1;
            maxLabel = -1;
            for (int i = 0; i < gridLength; i++) {
                for (int j = 0; j < gridLength; j++) {
                    if (visited[i, j] == 0) {
                        int area = flood(i, j, label);
                        if (area > 0) {
                            if (area > maxArea) {
                                maxLabel = label;
                                maxArea = area;
                            }
                            label += 1;
                        }
                    }
                }
            }
        }
        
        for (int i = 0; i < gridLength; i++) {
            for (int j = 0; j < gridLength; j++) {
                if (visited[i, j] != maxLabel) {
                    visited[i, j] = -1;
                }
            }
        }

        if (riverOn) {
            createRiver();
        }
        placeChests();

        floor = new Texture2D(tileLength, tileLength);
        floor.LoadImage(floorText.bytes);
        wall = new Texture2D(tileLength, wallHeight);
        wall.LoadImage(wallText.bytes);
        river = new Texture2D(tileLength, tileLength);
        river.LoadImage(riverText.bytes);
        lilypad = new Texture2D(tileLength, tileLength);
        lilypad.LoadImage(lilypadText.bytes);

        treasureChest.transform.localScale = new Vector3(0.25f * tileLength, 1, 0.25f * tileLength);
        draw();
        /*string test = "";
        for (int i = 0; i < gridLength; i++) {
            for (int j = 0; j < gridLength; j++) {
                if (grid[i, j]) {
                    test += " ";
                } else {
                    test += "i";
                }
            }
            test += "\n";
        }
        Debug.Log(test);

        for (int i = 0; i < gridLength; i++) {
            for (int j = 0; j < gridLength; j++) {
                Debug.Log(neighborCount(i,j));
            }
            test += "\n";
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    int neighborCount(int x, int y) {
        int neighbors = 0;
        for (int i = x - 1; i <= x + 1; i++) {
            for (int j = y - 1; j <= y + 1; j++) {
                if (i == -1 || j == -1 || i == gridLength || j == gridLength || !grid[i, j]) {
                    neighbors += 1;
                }
            }
        }
        return grid[x, y] ? neighbors: neighbors - 1;
    }

    int flood(int x, int y, int label) {
        if (x < 0 || y < 0 || x >= gridLength || y >= gridLength || visited[x, y] != 0) {
            return 0;
        }
        if (!grid[x, y]) {
            visited[x, y] = -1;
            return 0;
        }
        visited[x, y] = label;
        int count = 1;
        count += flood(x - 1, y, label);
        count += flood(x + 1, y, label);
        count += flood(x, y - 1, label);
        count += flood(x, y + 1, label);
        return count;
    }

    void createRiver() {
        int level = 0;
        int center = Random.Range(1, gridLength - 1);
        int width = Random.Range(1, gridLength / 2 - (Mathf.Abs(gridLength / 2 - center)));
        while (level < gridLength) {
            for (int i = center - width; i < center + width; i++) {
                visited[level, i] = visited[level, i] == -1 ? -3 : -4;
            }

            int newCenter = center + Random.Range(-1, 2);
            while (newCenter == 0 || newCenter == gridLength - 1) {
                newCenter = center + Random.Range(-1, 2);
            }
            center = newCenter;

            int newWidth = width + Random.Range(-1, 2);
            while (newWidth == 0 || center - newWidth < 0 || center + newWidth >= gridLength) {
                newWidth = width + Random.Range(-1, 2);
            }
            width = newWidth;
            level+= 1;
        }
    }

    void placeChests() {
        int a = 0;
        while (a < chestCount) {
            int x = Random.Range(0, gridLength);
            int y = Random.Range(0, gridLength);
            if (visited[x, y] == maxLabel) {
                visited[x, y] = -2;
                a += 1;
            } 
        }
    }

    void draw() {
        for (int i = 0; i < gridLength; i++) {
            for (int j = 0; j < gridLength; j++) {

                GameObject floorObject = new GameObject("Floor");
                floorObject.transform.position = new Vector3 (i * tileLength, 0, j * tileLength);
                floorObject.transform.localScale = new Vector3 (tileLength, 1, tileLength);

                Mesh floorMesh = CreateMyMesh();
                floorObject.AddComponent<MeshFilter>();
                floorObject.AddComponent<MeshRenderer>();

                floorObject.GetComponent<MeshFilter>().mesh = floorMesh;

                Renderer rend = floorObject.GetComponent<Renderer>();
                rend.material.color = new Color (0.0f, 0.0f, 0.0f, 0.0f);

                if (visited[i, j] == -1) {
                    GameObject ceilingObject = Instantiate(floorObject);
                    floorObject.transform.position = floorObject.transform.position + new Vector3(0, wallHeight, 0);
                } else {
                    rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
                    Renderer renderer = floorObject.GetComponent<Renderer>();

                    if (visited[i, j] == -3) {
                        renderer.material.mainTexture = river;
                    } else if (visited[i, j] == -4) {
                        renderer.material.mainTexture = lilypad;
                    } else {
                        renderer.material.mainTexture = floor;
                        if (visited[i, j] == -2) {
                            Instantiate(treasureChest, new Vector3(i * tileLength + (float)tileLength / 2, 0.5f, j * tileLength + (float)tileLength / 2), Quaternion.identity);
                        }
                    }
                    
                    if (i == 0 || visited[i - 1, j] == -1) {
                        GameObject northWall = new GameObject("North Wall");
                        northWall.transform.position = new Vector3 (i * tileLength, wallHeight, j * tileLength + tileLength);
                        northWall.transform.rotation = Quaternion.Euler(90, 0, -90);
                        drawWall(northWall);
                    }
                    
                    if (i == gridLength - 1 || visited[i + 1, j] == -1) {
                        GameObject southWall = new GameObject("South Wall");
                        southWall.transform.position = new Vector3 (i * tileLength + tileLength, 0, j * tileLength + tileLength);
                        southWall.transform.rotation = Quaternion.Euler(-90, 180, -90);
                        drawWall(southWall);
                    }
                    
                    if (j == 0 || visited[i, j - 1] == -1) {
                        GameObject westWall = new GameObject("West Wall");
                        westWall.transform.position = new Vector3 (i * tileLength, wallHeight, j * tileLength);
                        westWall.transform.rotation = Quaternion.Euler(90, 0, 0);
                        drawWall(westWall);
                    }
                    
                    if (j == gridLength - 1 || visited[i, j + 1] == -1) {
                        GameObject eastWall = new GameObject("East Wall");
                        eastWall.transform.position = new Vector3 (i * tileLength + tileLength, wallHeight, j * tileLength + tileLength);
                        eastWall.transform.rotation = Quaternion.Euler(90, 180, 0);
                        drawWall(eastWall);
                    }
                    
                }
            }
        }

        drawBorder();
    }

    Mesh CreateMyMesh() {
		
		Mesh mesh = new Mesh();

		Vector3[] verts = new Vector3[4];

		verts[0] = new Vector3 (1, 0, 0);
		verts[1] = new Vector3 (1, 0, 1);
		verts[2] = new Vector3 (0, 0, 1);
		verts[3] = new Vector3 (0, 0, 0);

		Vector2[] uv = new Vector2[4];

		uv[0] = new Vector2(1, 0);
		uv[1] = new Vector2(1, 1);
		uv[2] = new Vector2(0, 1);
		uv[3] = new Vector2(0, 0);

		int[] tris = new int[6];

		tris[0] = 0;
		tris[1] = 2;
		tris[2] = 1;
		tris[3] = 0;
		tris[4] = 3;
		tris[5] = 2;

		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.uv = uv;

		mesh.RecalculateNormals();

		return (mesh);
	}

    void drawWall(GameObject wallObject) {

        wallObject.transform.localScale = new Vector3(tileLength, 1, wallHeight);

        Mesh wallMesh = CreateMyMesh();
        wallObject.AddComponent<MeshFilter>();
        wallObject.AddComponent<MeshRenderer>();

        wallObject.GetComponent<MeshFilter>().mesh = wallMesh;

        Renderer rend = wallObject.GetComponent<Renderer>();
        rend.material.mainTexture = wall;
    }

    void drawBorder() {
        GameObject northBorder = new GameObject("North Border");
        northBorder.transform.position = new Vector3 (0, 0, gridLength * tileLength);
        northBorder.transform.rotation = Quaternion.Euler(-90, 180, -90);
        drawWall(northBorder);
        northBorder.transform.localScale = new Vector3(tileLength * gridLength, 1, wallHeight);

        GameObject southBorder = new GameObject("South Border");
        southBorder.transform.position = new Vector3 (gridLength * tileLength, wallHeight, gridLength * tileLength);
        southBorder.transform.rotation = Quaternion.Euler(90, 0, -90);
        drawWall(southBorder);
        southBorder.transform.localScale = new Vector3(tileLength * gridLength, 1, wallHeight);

        GameObject westBorder = new GameObject("West Border");
        westBorder.transform.position = new Vector3 (gridLength * tileLength, wallHeight, 0);
        westBorder.transform.rotation = Quaternion.Euler(90, 180, 0);
        drawWall(westBorder);
        westBorder.transform.localScale = new Vector3(tileLength * gridLength, 1, wallHeight);

        GameObject eastBorder = new GameObject("East Border");
        eastBorder.transform.position = new Vector3 (0, wallHeight, gridLength * tileLength);
        eastBorder.transform.rotation = Quaternion.Euler(90, 0, 0);
        drawWall(eastBorder);
        eastBorder.transform.localScale = new Vector3(tileLength * gridLength, 1, wallHeight);
    }
/*
	void MakeTri(int i1, int i2, int i3) {
		int index = ntris * 3;
		ntris++;

		tris[index]     = i1;
		tris[index + 1] = i2;
		tris[index + 2] = i3;
	}

	void MakeQuad(int i1, int i2, int i3, int i4) {
		MakeTri (i1, i2, i3);
		MakeTri (i1, i3, i4);
	}
    */
}
