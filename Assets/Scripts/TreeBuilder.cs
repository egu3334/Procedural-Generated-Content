using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBuilder : MonoBehaviour
{
    
    public int randomSeed;
    public int treeCount;
    public float maxRadius;
    public float maxLength;
    public float deathChance;
    public float budChance;
    public int quadCount;
    public int timeSteps;

    private Queue<TreeNode> growingBuds;

    private Vector3 xVector = new Vector3(1.0f, 0.0f, 0.0f);
    private Vector3 yVector = new Vector3(0.0f, 1.0f, 0.0f);
    private Vector3 zVector = new Vector3(0.0f, 0.0f, 1.0f);

    public class TreeNode {

        private Vector3 position;
        private Vector3 direction;
        private Vector3 normal;
        private Vector3 binormal;

        private float length;
        private float radius;
        private int order;
        private bool orthographic;
        private Color color;
        
        public TreeNode(Vector3 pos, Vector3 dir, Vector3 norm, Vector3 bin, float len, float rad, int ord, bool ortho, Color col) {
            position = pos;
            direction = dir;
            normal = norm;
            binormal = bin;
            length = len;
            radius = rad;
            order = ord;
            orthographic = ortho;
            color = col;
        }

        public Vector3 getPosition() {
            return position;
        }

        public Vector3 getDirection() {
            return direction;
        }

        public Vector3 getNormal() {
            return normal;
        }

        public Vector3 getBinormal() {
            return binormal;
        }

        public float getLength() {
            return length;
        }

        public float getRadius() {
            return radius;
        }

        public int getOrder() {
            return order;
        }

        public bool isOrthographic() {
            return orthographic;
        }

        public Color getColor() {
            return color;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Random.seed = randomSeed;
        growingBuds = new Queue<TreeNode>();

        for (int i = 0; i < treeCount; i++) {
            generateTree(i);
        }
        for (int i = 0; i < timeSteps; i++) {
            int count = growingBuds.Count;
            for (int j = 0; j < count; j++) {
                TreeNode curr = growingBuds.Dequeue();
                TreeNode next = generateNextNode(curr, i);
                drawBranch(curr, next);
                if (next.getRadius() > 0.0f && Random.Range(0.01f, 1.00f) <= budChance) {
                    TreeNode budStart = generateBudNode(curr, next.getRadius());
                    TreeNode budNext = generateNextNode(budStart, i);
                    drawBranch(budStart, budNext);
                }
            }
        }
    }

    void generateTree(int i) {
        Vector3 position = new Vector3(i * 50.0f, 0.0f, i * 50.0f);
        float length = Random.Range(0.9f * maxLength, maxLength);
        float radius = Random.Range(0.8f * maxRadius, maxRadius);
        bool orthographic  = (Random.value > 0.5f);
        Color randomColor = new Color (Random.Range(0.72f, 0.80f), Random.Range(0.57f, 0.65f), Random.Range(0.49f, 0.45f), 1.0f);
        growingBuds.Enqueue(new TreeNode(position, yVector, zVector, xVector, length, radius, 0, orthographic, randomColor));
    }

    TreeNode generateNextNode(TreeNode curr, int timestep) {
        Vector3 nextPosition = curr.getPosition() + curr.getLength() * curr.getDirection();
        if (timestep == timeSteps - 1 || Random.Range(0.01f, 1.00f) <= (float)timestep / ((float)timeSteps * 0.8f) * deathChance) {
            return new TreeNode(nextPosition, nextPosition, nextPosition, nextPosition, 0.0f, 0.0f, 0, true, curr.getColor());
        } else {
            Vector3 directionShift = curr.getOrder() == 0 || curr.isOrthographic() ? yVector : new Vector3(curr.getDirection().x, 0.0f, curr.getDirection().z).normalized;

            Vector3 nextDirection = randomVector((curr.getDirection() + 0.1f * directionShift).normalized, 0.2f).normalized;

            Vector3 nextBinormal = Vector3.Cross(nextDirection, curr.getNormal()).normalized;

            Vector3 nextNormal = Vector3.Cross(nextBinormal, nextDirection).normalized;

            float nextLength = Random.Range(0.8f * curr.getLength(), curr.getLength());
            float nextRadius = Random.Range(0.8f * curr.getRadius(), curr.getRadius());

            TreeNode nextNode = new TreeNode(nextPosition, nextDirection, nextNormal, nextBinormal, nextLength, nextRadius, curr.getOrder(), curr.isOrthographic(), curr.getColor());
            growingBuds.Enqueue(nextNode);

            return nextNode;
        }
    }

    TreeNode generateBudNode(TreeNode curr, float childRadius) {

        float relativeLocation = Random.Range(0.5f, 0.75f);
        Vector3 nextPosition = curr.getPosition() + relativeLocation * curr.getLength() * curr.getDirection();

        Vector3 nextDirection = randomVector(curr.getDirection(), 0.8f).normalized;

        Vector3 nextBinormal = Vector3.Cross(nextDirection, curr.getNormal()).normalized;

        Vector3 nextNormal = Vector3.Cross(nextBinormal, nextDirection).normalized;

        float nextLength = Random.Range(0.7f * curr.getLength(), 0.9f * curr.getLength());
        float nextRadius = Random.Range(0.55f * curr.getRadius(), curr.getRadius() - relativeLocation * (curr.getRadius() - childRadius));

        return new TreeNode(nextPosition, nextDirection, nextNormal, nextBinormal, nextLength, nextRadius, curr.getOrder() + 1, curr.isOrthographic(), curr.getColor());
    }

    void drawBranch(TreeNode start, TreeNode end) {

        GameObject treeSegment = new GameObject("Tree Segment " + start.getOrder().ToString());
		treeSegment.AddComponent<MeshFilter>();
		treeSegment.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        Vector3[] verts = new Vector3[2 * quadCount];
        int[] tris = new int[6 * quadCount];

        for (int i = 0; i < quadCount; i++) {
            float theta = (float) i / (float) quadCount * 2.0f * Mathf.PI;
            verts[i] = start.getPosition() + start.getRadius() * Mathf.Cos(theta) * start.getNormal() + start.getRadius() * Mathf.Sin(theta) * start.getBinormal();
            verts[i + quadCount] = end.getPosition() + end.getRadius() * Mathf.Cos(theta) * end.getNormal() + end.getRadius() * Mathf.Sin(theta) * end.getBinormal();
        }

        tris[0] = quadCount - 1;
        tris[1] = 0;
        tris[2] = quadCount;

        tris[3] = quadCount;
        tris[4] = 2 * quadCount - 1;
        tris[5] = quadCount - 1;

        for (int i = 1; i < quadCount; i++) {
            tris[i * 6] = i - 1;
            tris[i * 6 + 1] = i;
            tris[i * 6 + 2] = i + quadCount;

            tris[i * 6 + 3] = i + quadCount;
            tris[i * 6 + 4] = i + quadCount - 1;
            tris[i * 6 + 5] = i - 1;
        }

        mesh.vertices = verts;
		mesh.triangles = tris;

		mesh.RecalculateNormals();
        treeSegment.GetComponent<MeshFilter>().mesh = mesh;

		Renderer rend = treeSegment.GetComponent<Renderer>();
		rend.material.color = start.getColor();

    }

    Vector3 randomVector(Vector3 baseVector, float i) {
        Vector3 offset = -1.0f * baseVector;
        while (Vector3.Dot(offset, baseVector) <= 0.0f) {
            offset = new Vector3(Random.Range(-1.0f * i, i), Random.Range(-1.0f * i, i), Random.Range(-1.0f * i, i));
        }
        return offset + baseVector;
    }
}
