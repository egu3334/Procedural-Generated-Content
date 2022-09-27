// Sample code that creates a mesh and makes multiple copies of it
//
// Greg Turk

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMesh : MonoBehaviour {

	private Vector3[] verts;  // the vertices of the mesh
	private int[] tris;       // the triangles of the mesh (triplets of integer references to vertices)
	private int ntris = 0;    // the number of triangles that have been created so far
	public int randomSeed;

	// Create a mesh and put multiple copies of it into the scene.

	void Start() {

		Random.seed = randomSeed;
		// call the routine that makes a mesh (a cube) from scratch
		Mesh my_mesh = CreateMyMesh();

		// make multiple copies of this mesh and place these copies in the scene

		float k = 10.0f;
		int num_objects = 40;

		for (int i = 0; i < num_objects; i++) {

			// create a new GameObject and give it a MeshFilter and a MeshRenderer
			GameObject s = new GameObject(i.ToString("Object 0"));
			s.AddComponent<MeshFilter>();
			s.AddComponent<MeshRenderer>();

			s.transform.position = new Vector3 (Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));  // move this object to a new location
			s.transform.localScale = new Vector3 (0.25f, 0.25f, 0.25f);  // shrink the object

			// associate my mesh with this object
			s.GetComponent<MeshFilter>().mesh = my_mesh;

			// change the color of the object
			Renderer rend = s.GetComponent<Renderer>();
			rend.material.color = new Color (Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);  // light green color
		}
	
	}

	// Create a cube that is centered on the origin, with sides of length = 2.
	//
	// Note that although the faces of a cube share corners, we cannot share these vertices
	// because that would mess up the surface normals at the vertices.

	Mesh CreateMyMesh() {
		
		// create a mesh object
		Mesh mesh = new Mesh();

		// vertices of a octahedron
		int num_verts = 24;
		verts = new Vector3[num_verts];

		// vertices for faces of the octahedron

		verts[0] = new Vector3 (1, 0, 1);
		verts[1] = new Vector3 (1, 0, -1);
		verts[2] = new Vector3 (0, 1, 0);

		verts[3] = new Vector3 (1, 0, -1);
		verts[4] = new Vector3 (-1, 0, -1);
		verts[5] = new Vector3 (0, 1, 0);

		verts[6] = new Vector3 (-1, 0, -1);
		verts[7] = new Vector3 (-1, 0, 1);
		verts[8] = new Vector3 (0, 1, 0);

		verts[9] = new Vector3 (-1, 0, 1);
		verts[10] = new Vector3 (1, 0, 1);
		verts[11] = new Vector3 (0, 1, 0);

		verts[12] = new Vector3 (1, 0, -1);
		verts[13] = new Vector3 (1, 0, 1);
		verts[14] = new Vector3 (0, -1, 0);

		verts[15] = new Vector3 (-1, 0, -1);
		verts[16] = new Vector3 (1, 0, -1);
		verts[17] = new Vector3 (0, -1, 0);

		verts[18] = new Vector3 (-1, 0, 1);
		verts[19] = new Vector3 (-1, 0, -1);
		verts[20] = new Vector3 (0, -1, 0);

		verts[21] = new Vector3 (1, 0, 1);
		verts[22] = new Vector3 (-1, 0, 1);
		verts[23] = new Vector3 (0, -1, 0);

		// squares that make up the cube faces

		int num_tris = 8;  // need 2 triangles per face
		tris = new int[num_tris * 3];  // need 3 vertices per triangle

		for (int i = 0; i < 24; i += 3) {
			MakeTri(i, i + 1, i + 2);
		}

		// save the vertices and triangles in the mesh object
		mesh.vertices = verts;
		mesh.triangles = tris;

		mesh.RecalculateNormals();  // automatically calculate the vertex normals

		return (mesh);
	}

	// make a triangle from three vertex indices (clockwise order)
	void MakeTri(int i1, int i2, int i3) {
		int index = ntris * 3;  // figure out the base index for storing triangle indices
		ntris++;

		tris[index]     = i1;
		tris[index + 1] = i2;
		tris[index + 2] = i3;
	}

	// make a quadrilateral from four vertex indices (clockwise order)
	void MakeQuad(int i1, int i2, int i3, int i4) {
		MakeTri (i1, i2, i3);
		MakeTri (i1, i3, i4);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
