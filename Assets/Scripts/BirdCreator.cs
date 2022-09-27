using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdCreator : MonoBehaviour {

    public int numBirds;
    public int cylQuadCount;
    public float birdScale;

    public int randomSeed;
    private static int birdCount;
    public static float scale;
    // public enum legType {FourToeClaw, TwoToe, Flipper};
    // public enum wingType {FeatherTipped, FeathersBack, FeathersOut};
    // public enum beakType {General, Pelican, Parrot};

    private Vector3 xVector = new Vector3(1.0f, 0.0f, 0.0f);
    private Vector3 yVector = new Vector3(0.0f, 1.0f, 0.0f);
    private Vector3 zVector = new Vector3(0.0f, 0.0f, 1.0f);

    void Start() {
        Random.seed = randomSeed;
        birdCount = 0;
        scale = birdScale;
        for (int i = 0; i < numBirds; i++) {
            Bird currBird = new Bird();
            drawBird(currBird);
        }
    }
    
    void drawBird(Bird bird) {
        drawBody(bird.getBody());
        drawLeg(bird.getLeftLeg());
        drawLeg(bird.getRightLeg());
        drawWing(bird.getLeftWing());
        drawWing(bird.getRightWing());
        drawBeak(bird.getBeak());
    }

    void drawBody(Body body) {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        sphere.transform.localScale = new Vector3(body.getWidth(), body.getHeight(), body.getLength());
        sphere.transform.position = body.getCenter();
        // sphere.AddComponent<MeshRenderer>();
        Renderer rend = sphere.GetComponent<Renderer>();
		rend.material.color = body.getColor();
    }

    void drawLeg(Leg leg) {
        
        CylinderNode legBase = new CylinderNode(leg.getPos(), yVector, zVector, xVector, leg.getFootHeight(), 0.5f * leg.getWidth());
        CylinderNode legBottom = new CylinderNode(leg.getPos() - yVector * leg.getFootHeight(), yVector, zVector, xVector, 0.0f, 0.5f * leg.getWidth());
        CylinderNode footDummy = new CylinderNode(leg.getPos() - yVector * leg.getFootHeight(), yVector, xVector, zVector, 0.0f, 0.0f);

        drawCylinder(legBottom, legBase, leg.getColor(), false, cylQuadCount);
        drawCylinder(legBase, legBottom, leg.getColor(), false, cylQuadCount);
        drawCylinder(legBottom, footDummy, leg.getColor(), true, cylQuadCount);
        
        Queue<CylinderNode> toes = new Queue<CylinderNode>();
        int timeSteps = 10;
        Vector3 directionChange = new Vector3(0.0f, 0.0f, 0.0f);
        float toeWidth = 0.6f * leg.getWidth();
        float radiusChange = toeWidth / (float)timeSteps;
        Vector3 toeStart = leg.getPos() - yVector * leg.getFootHeight() + new Vector3(0.0f, 0.5f * toeWidth, 0.0f);

        if (leg.getType() == 1) {
            //normal two toed
            Vector3 rightDir = new Vector3(0.5f, 0.0f, Mathf.Sqrt(3.0f) / 2.0f);
            Vector3 leftDir = new Vector3(-0.5f, 0.0f, Mathf.Sqrt(3.0f) / 2.0f);

            toes.Enqueue(new CylinderNode(toeStart, rightDir, yVector, Vector3.Cross(rightDir, yVector).normalized, leg.getToeLength() / (float) timeSteps, toeWidth));
            toes.Enqueue(new CylinderNode(toeStart, leftDir, yVector, Vector3.Cross(leftDir, yVector).normalized, leg.getToeLength() / (float) timeSteps, toeWidth));
    
        } else if (leg.getType() == 2) {
            //clawed four toe
            directionChange = -1.0f * yVector;

            toes.Enqueue(new CylinderNode(toeStart, zVector, yVector, xVector, leg.getToeLength() / (float) timeSteps, toeWidth));
            toes.Enqueue(new CylinderNode(toeStart, -1.0f * zVector, yVector, xVector, leg.getToeLength() / (float) timeSteps, toeWidth));

            Vector3 rightDir = new Vector3(Mathf.Sqrt(3.0f) / 2.0f, 0.0f, 0.5f);
            Vector3 leftDir = new Vector3(Mathf.Sqrt(3.0f) / -2.0f, 0.0f, 0.5f);

            toes.Enqueue(new CylinderNode(toeStart, rightDir, yVector, Vector3.Cross(rightDir, yVector).normalized, leg.getToeLength() / (float) timeSteps, toeWidth));
            toes.Enqueue(new CylinderNode(toeStart, leftDir, yVector, Vector3.Cross(leftDir, yVector).normalized, leg.getToeLength() / (float) timeSteps, toeWidth));

        } else {
            //three toe flipper
            toes.Enqueue(new CylinderNode(toeStart, zVector, yVector, xVector, leg.getToeLength() / (float) timeSteps, toeWidth));

            Vector3 rightDir = new Vector3(1.0f / Mathf.Sqrt(2.0f), 0.0f, 1.0f / Mathf.Sqrt(2.0f));
            Vector3 leftDir = new Vector3(-1.0f / Mathf.Sqrt(2.0f), 0.0f, 1.0f / Mathf.Sqrt(2.0f));

            toes.Enqueue(new CylinderNode(toeStart, rightDir, yVector, Vector3.Cross(rightDir, yVector).normalized, leg.getToeLength() / (float) timeSteps, toeWidth));
            toes.Enqueue(new CylinderNode(toeStart, leftDir, yVector, Vector3.Cross(leftDir, yVector).normalized, leg.getToeLength() / (float) timeSteps, toeWidth));

            Vector3 leftEnd = toeStart + leg.getToeLength() * leftDir;
            Vector3 midEnd = toeStart + leg.getToeLength() * zVector;
            Vector3 rightEnd = toeStart + leg.getToeLength() * rightDir;

            drawTriangle(leftEnd, midEnd, toeStart, leg.getColor());
            drawTriangle(toeStart, midEnd, leftEnd, leg.getColor());
            drawTriangle(rightEnd, midEnd, toeStart, leg.getColor());
            drawTriangle(toeStart, midEnd, rightEnd, leg.getColor());
        }

        for (int i = 0; i < timeSteps; i++) {
            int count = toes.Count;
            for (int j = 0; j < count; j++) {
                CylinderNode curr = toes.Dequeue();
                
                Vector3 nextPosition = curr.getPosition() + curr.getLength() * curr.getDirection();
                Vector3 nextDirection = (curr.getDirection() + 0.1f * directionChange).normalized;
                Vector3 nextBinormal = Vector3.Cross(nextDirection, curr.getNormal()).normalized;
                Vector3 nextNormal = Vector3.Cross(nextBinormal, nextDirection).normalized;
                float nextRadius = curr.getRadius() - radiusChange;
                CylinderNode nextNode = new CylinderNode(nextPosition, nextDirection, nextNormal, nextBinormal, curr.getLength(), nextRadius);

                drawCylinder(curr, nextNode, leg.getColor(), false, cylQuadCount);
                toes.Enqueue(nextNode);
            }
        }

    }

    void drawWing(Wing wing) {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        sphere.transform.localScale = new Vector3(wing.getLength(), wing.getThickness(), wing.getWidth());
        sphere.transform.position = wing.getPos();
        // sphere.AddComponent<MeshRenderer>();
        Renderer rend = sphere.GetComponent<Renderer>();
		rend.material.color = wing.getColor();
    }

    void drawBeak(Beak beak) {
        if (beak.getType() == 1) {
            //normal (triangular)
            int timeSteps = 10;
            float widthChange = beak.getWidth()/ 2.0f / (float) timeSteps;
            Queue<CylinderNode> beakQ = new Queue<CylinderNode>();
            beakQ.Enqueue(new CylinderNode(beak.getPos(), zVector, yVector, xVector, beak.getLength() / (float) timeSteps, beak.getWidth() / 2.0f));
            for (int i = 0; i < timeSteps; i++) {
                CylinderNode curr = beakQ.Dequeue();
                
                Vector3 nextPosition = curr.getPosition() + curr.getLength() * curr.getDirection();
                float nextRadius = curr.getRadius() - widthChange;
                CylinderNode nextNode = new CylinderNode(nextPosition, curr.getDirection(), curr.getNormal(), curr.getBinormal(), curr.getLength(), nextRadius);

                drawCylinder(curr, nextNode, beak.getColor(), false, 4);
                drawCylinder(curr, nextNode, beak.getColor(), true, 4);
                beakQ.Enqueue(nextNode);
            }
    
        } else if (beak.getType() == 2) {
            //parrot
            Vector3[,] controlPoints = new Vector3[,] {
                {
                    beak.getPos() + new Vector3(-0.5f * beak.getWidth(), 0.0f, 0.0f),
                    beak.getPos() + new Vector3(-0.5f * beak.getWidth(), 0.0f, beak.getLength() / 2.0f),
                    beak.getPos() + new Vector3(-0.25f * beak.getWidth(), -1.0f * beak.getThickness(), beak.getLength()),
                    beak.getPos() + new Vector3(0.0f, -2.0f * beak.getThickness(), beak.getLength())
                },
                {
                    beak.getPos() + new Vector3(beak.getWidth() / -6.0f, 0.0f, 0.0f), 
                    beak.getPos() + new Vector3(beak.getWidth() / -6.0f, 0.0f, beak.getLength()),
                    beak.getPos() + new Vector3(beak.getWidth() / -12.0f, -1.0f * beak.getThickness(), beak.getLength()),
                    beak.getPos() + new Vector3(0.0f, -2.0f * beak.getThickness(), beak.getLength())
                },
                {
                    beak.getPos() + new Vector3(beak.getWidth() / 6.0f, 0.0f, 0.0f), 
                    beak.getPos() + new Vector3(beak.getWidth() / 6.0f, 0.0f, beak.getLength()),
                    beak.getPos() + new Vector3(beak.getWidth() / 12.0f, -1.0f * beak.getThickness(), beak.getLength()),
                    beak.getPos() + new Vector3(0.0f, -2.0f * beak.getThickness(), beak.getLength())
                },
                {
                    beak.getPos() + new Vector3(0.5f * beak.getWidth(), 0.0f, 0.0f),
                    beak.getPos() + new Vector3(0.5f * beak.getWidth(), 0.0f, beak.getLength() / 2.0f),
                    beak.getPos() + new Vector3(0.25f * beak.getWidth(), -1.0f * beak.getThickness(), beak.getLength()),
                    beak.getPos() + new Vector3(0.0f, -2.0f * beak.getThickness(), beak.getLength())
                }
            };

            int timeSteps = 20;
            float increment = 1.0f / timeSteps;
            float t = increment;
            Vector3 a = controlPoints[0,0];
            Vector3 b = controlPoints[1,0];
            Vector3 c = controlPoints[2,0];
            Vector3 d = controlPoints[3,0];
            Vector3[] prevPoints = allBezierPoints(a, b, c, d, timeSteps);

            Vector3 botLeft = controlPoints[0,0];
            Vector3 topLeft = controlPoints[0,3];
            Vector3 botRight = controlPoints[3,0];
            Vector3 topRight = controlPoints[3,3];

            for (int i = 0; i < timeSteps; i ++) {
                Vector3 w = calcBezierPoint(controlPoints[0,0], controlPoints[0,1], controlPoints[0,2], controlPoints[0,3], t);
                Vector3 x = calcBezierPoint(controlPoints[1,0], controlPoints[1,1], controlPoints[1,2], controlPoints[1,3], t);
                Vector3 y = calcBezierPoint(controlPoints[2,0], controlPoints[2,1], controlPoints[2,2], controlPoints[2,3], t);
                Vector3 z = calcBezierPoint(controlPoints[3,0], controlPoints[3,1], controlPoints[3,2], controlPoints[3,3], t);
                Vector3[] nextPoints = allBezierPoints(w, x, y, z, timeSteps);
                drawPatchQuads(prevPoints, nextPoints, beak.getColor());
                //drawQuad(botLeft, prevPoints[0], nextPoints[0], botLeft + (topLeft - botLeft) / (float) timeSteps, beak.getColor());
                //drawQuad(nextPoints[timeSteps], prevPoints[timeSteps], botRight, botRight + (topRight - botRight) / (float) timeSteps, beak.getColor());
                //botLeft += (topLeft - botLeft) / (float) timeSteps;
                //botRight += (topRight - botRight) / (float) timeSteps;
                prevPoints = nextPoints;
                t += increment;
            }

            controlPoints = new Vector3[,] {
                {
                    beak.getPos() + new Vector3(-0.5f * beak.getWidth(), 0.0f, 0.0f),
                    beak.getPos() + new Vector3(-0.5f * beak.getWidth(), 0.0f, beak.getLength() / 2.0f),
                    beak.getPos() + new Vector3(-0.25f * beak.getWidth(), -1.0f * beak.getThickness(), beak.getLength()),
                    beak.getPos() + new Vector3(0.0f, -2.0f * beak.getThickness(), beak.getLength())
                },
                {
                    beak.getPos() + new Vector3(beak.getWidth() / -6.0f, beak.getThickness(), 0.0f), 
                    beak.getPos() + new Vector3(beak.getWidth() / -6.0f, beak.getThickness(), beak.getLength()),
                    beak.getPos() + new Vector3(beak.getWidth() / -12.0f, -1.0f * beak.getThickness(), beak.getLength()),
                    beak.getPos() + new Vector3(0.0f, -2.0f * beak.getThickness(), beak.getLength())
                },
                {
                    beak.getPos() + new Vector3(beak.getWidth() / 6.0f, beak.getThickness(), 0.0f), 
                    beak.getPos() + new Vector3(beak.getWidth() / 6.0f, beak.getThickness(), beak.getLength()),
                    beak.getPos() + new Vector3(beak.getWidth() / 12.0f, -1.0f * beak.getThickness(), beak.getLength()),
                    beak.getPos() + new Vector3(0.0f, -2.0f * beak.getThickness(), beak.getLength())
                },
                {
                    beak.getPos() + new Vector3(0.5f * beak.getWidth(), 0.0f, 0.0f),
                    beak.getPos() + new Vector3(0.5f * beak.getWidth(), 0.0f, beak.getLength() / 2.0f),
                    beak.getPos() + new Vector3(0.25f * beak.getWidth(), -1.0f * beak.getThickness(), beak.getLength()),
                    beak.getPos() + new Vector3(0.0f, -2.0f * beak.getThickness(), beak.getLength())
                }
            };

            timeSteps = 20;
            increment = 1.0f / timeSteps;
            t = increment;
            a = controlPoints[0,0];
            b = controlPoints[1,0];
            c = controlPoints[2,0];
            d = controlPoints[3,0];
            prevPoints = allBezierPoints(a, b, c, d, timeSteps);

            //Vector3 botLeft = controlPoints[0,0];
            //Vector3 topLeft = controlPoints[0,3];
            //Vector3 botRight = controlPoints[3,0];
            //Vector3 topRight = controlPoints[3,3];

            for (int i = 0; i < timeSteps; i ++) {
                Vector3 w = calcBezierPoint(controlPoints[0,0], controlPoints[0,1], controlPoints[0,2], controlPoints[0,3], t);
                Vector3 x = calcBezierPoint(controlPoints[1,0], controlPoints[1,1], controlPoints[1,2], controlPoints[1,3], t);
                Vector3 y = calcBezierPoint(controlPoints[2,0], controlPoints[2,1], controlPoints[2,2], controlPoints[2,3], t);
                Vector3 z = calcBezierPoint(controlPoints[3,0], controlPoints[3,1], controlPoints[3,2], controlPoints[3,3], t);
                Vector3[] nextPoints = allBezierPoints(w, x, y, z, timeSteps);
                drawPatchQuads(nextPoints, prevPoints, beak.getColor());
                //drawQuad(botLeft, prevPoints[0], nextPoints[0], botLeft + (topLeft - botLeft) / (float) timeSteps, beak.getColor());
                //drawQuad(nextPoints[timeSteps], prevPoints[timeSteps], botRight, botRight + (topRight - botRight) / (float) timeSteps, beak.getColor());
                //botLeft += (topLeft - botLeft) / (float) timeSteps;
                //botRight += (topRight - botRight) / (float) timeSteps;
                prevPoints = nextPoints;
                t += increment;
            }


        } else {
            //pelican
            Vector3[,] controlPoints = new Vector3[,] {
                {
                    beak.getPos() + new Vector3(-0.5f * beak.getWidth(), 0.0f, 0.0f),
                    beak.getPos() + new Vector3(-0.5f * beak.getWidth(), -1.5f * beak.getThickness(), beak.getLength() / 3.0f),
                    beak.getPos() + new Vector3(-0.5f * beak.getWidth(), -0.5f, 2.0f * beak.getLength() / 3.0f),
                    beak.getPos() + new Vector3(-0.5f * beak.getWidth(), 0.0f, beak.getLength())
                },
                {
                    beak.getPos() + new Vector3(beak.getWidth() / -6.0f, 0.0f, 0.0f), 
                    beak.getPos() + new Vector3(beak.getWidth() / -6.0f, -2.0f * beak.getThickness(), beak.getLength() / 2.0f),
                    beak.getPos() + new Vector3(beak.getWidth() / -6.0f, -1.5f * beak.getThickness(), beak.getLength()),
                    beak.getPos() + new Vector3(beak.getWidth() / -6.0f, 0.0f, beak.getLength())
                },
                {
                    beak.getPos() + new Vector3(beak.getWidth() / 6.0f, 0.0f, 0.0f), 
                    beak.getPos() + new Vector3(beak.getWidth() / 6.0f, -2.0f * beak.getThickness(), beak.getLength() / 2.0f),
                    beak.getPos() + new Vector3(beak.getWidth() / 6.0f, -1.5f * beak.getThickness(), beak.getLength()),
                    beak.getPos() + new Vector3(beak.getWidth() / 6.0f, 0.0f, beak.getLength())
                },
                {
                    beak.getPos() + new Vector3(0.5f * beak.getWidth(), 0.0f, 0.0f),
                    beak.getPos() + new Vector3(0.5f * beak.getWidth(), -1.5f * beak.getThickness(), beak.getLength() / 3.0f),
                    beak.getPos() + new Vector3(0.5f * beak.getWidth(), -0.5f, 2.0f * beak.getLength() / 3.0f),
                    beak.getPos() + new Vector3(0.5f * beak.getWidth(), 0.0f, beak.getLength())
                }
            };

            int timeSteps = 20;
            float increment = 1.0f / timeSteps;
            float t = increment;
            Vector3 a = controlPoints[0,0];
            Vector3 b = controlPoints[1,0];
            Vector3 c = controlPoints[2,0];
            Vector3 d = controlPoints[3,0];
            Vector3[] prevPoints = allBezierPoints(a, b, c, d, timeSteps);

            Vector3 botLeft = controlPoints[0,0];
            Vector3 topLeft = controlPoints[0,3];
            Vector3 botRight = controlPoints[3,0];
            Vector3 topRight = controlPoints[3,3];

            for (int i = 0; i < timeSteps; i ++) {
                Vector3 w = calcBezierPoint(controlPoints[0,0], controlPoints[0,1], controlPoints[0,2], controlPoints[0,3], t);
                Vector3 x = calcBezierPoint(controlPoints[1,0], controlPoints[1,1], controlPoints[1,2], controlPoints[1,3], t);
                Vector3 y = calcBezierPoint(controlPoints[2,0], controlPoints[2,1], controlPoints[2,2], controlPoints[2,3], t);
                Vector3 z = calcBezierPoint(controlPoints[3,0], controlPoints[3,1], controlPoints[3,2], controlPoints[3,3], t);
                Vector3[] nextPoints = allBezierPoints(w, x, y, z, timeSteps);
                drawPatchQuads(prevPoints, nextPoints, beak.getColor());
                drawQuad(botLeft, prevPoints[0], nextPoints[0], botLeft + (topLeft - botLeft) / (float) timeSteps, beak.getColor());
                drawQuad(nextPoints[timeSteps], prevPoints[timeSteps], botRight, botRight + (topRight - botRight) / (float) timeSteps, beak.getColor());
                botLeft += (topLeft - botLeft) / (float) timeSteps;
                botRight += (topRight - botRight) / (float) timeSteps;
                prevPoints = nextPoints;
                t += increment;
            }


            //drawQuad(controlPoints[0,0], controlPoints[3,0], controlPoints[3,3], controlPoints[0, 3], beak.getColor());
            drawQuad(controlPoints[0,0], controlPoints[0, 3], controlPoints[3,3], controlPoints[3, 0], beak.getColor());

        }
    }

    Vector3 calcBezierPoint(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t) {
        Vector3 e = a * Mathf.Pow(1.0f - t, 3);
        Vector3 f = 3.0f * b * Mathf.Pow(1.0f - t, 2) * t;
        Vector3 g = 3.0f * c * (1.0f - t) * Mathf.Pow(t, 2);
        Vector3 h = d * Mathf.Pow(t, 3);
        return e + f + g + h;
    }

    Vector3[] allBezierPoints(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int timeSteps) {
        Vector3[] ret = new Vector3[timeSteps + 1];
        float t = 0.0f;
        float increment = 1.0f / (float) timeSteps;
        for (int i = 0; i <= timeSteps; i++) {
            ret[i] = calcBezierPoint(a, b, c, d, t);
            t += increment;
        }
        return ret;
    }

    void drawPatchQuads(Vector3[] past, Vector3[] next, Color col) {
        for (int i = 0; i < past.Length - 1; i++) {
            drawQuad(past[i], past[i + 1], next[i + 1], next[i], col);
        }
    }

    void drawQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color col) {
        drawTriangle(a, b, d, col);
        drawTriangle(c, d, b, col);
    }

    void drawTriangle(Vector3 a, Vector3 b, Vector3 c, Color col) {
        Vector3[] verts = new Vector3[] {a, b, c};
        int[] tris = new int[] {0, 1, 2};
        drawObject(verts, tris, col, "tri");
    }

    void drawCylinder(CylinderNode start, CylinderNode end, Color col, bool reverse, int quadCount) {

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

        if (reverse) {
            tris[0] = quadCount;
            tris[1] = 0;
            tris[2] = quadCount - 1;

            tris[3] = quadCount - 1;
            tris[4] = 2 * quadCount - 1;
            tris[5] = quadCount;

            for (int i = 1; i < quadCount; i++) {
                tris[i * 6] = i + quadCount;
                tris[i * 6 + 1] = i;
                tris[i * 6 + 2] = i - 1;

                tris[i * 6 + 3] = i - 1;
                tris[i * 6 + 4] = i + quadCount - 1;
                tris[i * 6 + 5] = i + quadCount;
            }
        }

        drawObject(verts, tris, col, "cyl");

    }

    void drawObject(Vector3[] verts, int[] tris, Color col, string name) {
        
        GameObject drawnObject = new GameObject(name);
		drawnObject.AddComponent<MeshFilter>();
		drawnObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        mesh.vertices = verts;
		mesh.triangles = tris;

		mesh.RecalculateNormals();
        drawnObject.GetComponent<MeshFilter>().mesh = mesh;

		Renderer rend = drawnObject.GetComponent<Renderer>();
		rend.material.color = col;
    }

    public class Bird {
    
    private Body body;
    private Leg leftLeg;
    private Leg rightLeg;
    private Wing leftWing;
    private Wing rightWing;
    private Beak beak;
    // private float scale;

        public Bird() {
            Vector3 bodyBase = new Vector3(2.0f * birdCount * scale, scale * 0.5f, 2.0f * birdCount * scale);
            birdCount += 1;
            body = new Body(bodyBase);
            Vector3 leftLegBase = bodyBase + new Vector3(-0.3f * body.getWidth(), 0.2f * body.getHeight(), 0.0f);
            Vector3 rightLegBase = bodyBase + new Vector3(0.3f * body.getWidth(), 0.2f * body.getHeight(), 0.0f);

            leftLeg = new Leg(leftLegBase);
            rightLeg = new Leg(rightLegBase, leftLeg);

            Color bodyColor = body.getColor();
            Vector3 bodyCenter = body.getPos() + new Vector3(0.0f, body.getHeight() * 0.5f, 0.0f);

            Vector3 leftWingBase = bodyCenter + new Vector3(-0.4f * body.getWidth(), 0.0f, 0.0f);
            Vector3 rightWingBase = bodyCenter + new Vector3(0.4f * body.getWidth(), 0.0f, 0.0f);
            leftWing = new Wing(leftWingBase, bodyColor);
            rightWing = new Wing(rightWingBase, leftWing);

            Vector3 beakBase = bodyCenter + new Vector3(0.0f, 0.0f, 0.35f * body.getLength());
            beak = new Beak(beakBase);
        }

        public Body getBody() {
            return body;
        }

        public Leg getLeftLeg() {
            return leftLeg;
        }

        public Leg getRightLeg() {
            return rightLeg;
        }

        public Wing getLeftWing() {
            return leftWing;
        }

        public Wing getRightWing() {
            return rightWing;
        }

        public Beak getBeak() {
            return beak;
        }

    }

    public class Body {

        private Vector3 pos;
        private float length;
        private float height;
        private float width;
        private Color col;

        public Body(Vector3 bodyBase) {
            pos = bodyBase;
            length = Random.Range(scale, scale * 1.3f);
            height = Random.Range(0.7f * scale, 0.9f * scale);
            width = Random.Range(0.7f * scale, 0.9f * scale);
            col = new Color (Random.Range(0.1f, 0.80f), Random.Range(0.1f, 0.80f), Random.Range(0.1f, 0.80f), 1.0f);
        }

        public Vector3 getPos() {
            return pos;
        }

        public Vector3 getCenter() {
            return pos + new Vector3(0.0f, 0.5f * height, 0.0f);
        }

        public float getLength() {
            return length;
        }

        public float getHeight() {
            return height;
        }

        public float getWidth() {
            return width;
        }

        public Color getColor() {
            return col;
        }
    }

    public class Leg {
        
        private Vector3 pos;
        private float width;
        private float footHeight;
        private float toeLength;
        private int type;
        private Color col;

        public Leg(Vector3 legBase) {
            pos = legBase;
            width = Random.Range(0.05f * scale, 0.1f * scale);
            footHeight = Random.Range(0.4f * scale, 0.6f * scale);
            toeLength = Random.Range(0.15f * scale, 0.25f * scale);
            type = Random.Range(1, 4);
            col = new Color (1.0f, Random.Range(0.4f, 0.6f), Random.Range(0.2f, 0.4f), 1.0f);
        }

        public Leg(Vector3 legBase, Leg otherLeg) {
            pos = legBase;
            width = otherLeg.getWidth();
            footHeight = otherLeg.getFootHeight();
            toeLength = otherLeg.getToeLength();
            type = otherLeg.getType();
            col = otherLeg.getColor();
        }

        public Vector3 getPos() {
            return pos;
        }

        public float getToeLength() {
            return toeLength;
        }

        public float getFootHeight() {
            return footHeight;
        }

        public float getWidth() {
            return width;
        }

        public int getType() {
            return type;
        }

        public Color getColor() {
            return col;
        }
    }

    public class Wing {
        
        private Vector3 pos;
        private float length;
        private float width;
        private float thickness;
        private Color col;
        // private int type;

        public Wing(Vector3 wingBase, Color bodyColor) {
            pos = wingBase;
            length = Random.Range(1.0f * scale, 1.2f * scale);
            width = Random.Range(0.4f * scale, 0.6f * scale);
            thickness = Random.Range(0.1f * scale, 0.2f * scale);
            col = new Color(Random.Range(0.9f * bodyColor.r, 1.1f * bodyColor.r), Random.Range(0.9f * bodyColor.g, 1.1f * bodyColor.g), Random.Range(0.9f * bodyColor.b, 1.1f * bodyColor.b));
        }

        public Wing(Vector3 wingBase, Wing otherWing) {
            pos = wingBase;
            length = otherWing.getLength();
            width = otherWing.getWidth();
            thickness = otherWing.getThickness();
            col = otherWing.getColor();
        }

        public Vector3 getPos() {
            return pos;
        }

        public float getLength() {
            return length;
        }

        public float getThickness() {
            return thickness;
        }

        public float getWidth() {
            return width;
        }

        public Color getColor() {
            return col;
        }
        /*
        public int getType() {
            return type;
        }
        */
    }

    public class Beak {
        
        private Vector3 pos;
        private float length;
        private float width;
        private float thickness;
        private int type;
        private Color col;

        public Beak(Vector3 beakBase) {
            pos = beakBase;
            length = Random.Range(0.5f * scale, 0.7f * scale);
            width = Random.Range(0.2f * scale, 0.3f * scale);
            thickness = Random.Range(0.2f * scale, 0.3f * scale);
            type = Random.Range(1, 4);
            col = new Color(1.0f, Random.Range(0.5f, 1.0f), Random.Range(0.2f, 0.4f), 1.0f);
        }

        public Vector3 getPos() {
            return pos;
        }

        public float getLength() {
            return length;
        }

        public float getThickness() {
            return thickness;
        }

        public float getWidth() {
            return width;
        }

        public int getType() {
            return type;
        }

        public Color getColor() {
            return col;
        }

    }

    public class CylinderNode {

        private Vector3 position;
        private Vector3 direction;
        private Vector3 normal;
        private Vector3 binormal;

        private float length;
        private float radius;
        
        public CylinderNode(Vector3 pos, Vector3 dir, Vector3 norm, Vector3 bin, float len, float rad) {
            position = pos;
            direction = dir;
            normal = norm;
            binormal = bin;
            length = len;
            radius = rad;
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

    }
}