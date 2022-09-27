using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBuilder : MonoBehaviour
{
    public int randomSeed;
    private static int birdCount;
    public static float scale;
    // public enum legType {FourToe, TwoToe, Flipper};
    // public enum wingType {FeatherTipped, FeathersBack, FeathersOut};
    // public enum beakType {General, Pelican, Parrot};
    
    void Start() {
        birdCount = 0;
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
            Vector3 bodyBase = new Vector3(birdCount * scale, scale * 0.5f, birdCount * scale);
            birdCount += 1;
            body = new Body(bodyBase);
            Vector3 leftLegBase = bodyBase + new Vector3(-0.1f * body.getWidth(), 0.2f * body.getHeight(), 0.0f);
            Vector3 rightLegBase = bodyBase + new Vector3(0.1f * body.getWidth(), 0.2f * body.getHeight(), 0.0f);

            leftLeg = new Leg(leftLegBase);
            rightLeg = new Leg(rightLegBase, leftLeg);

            Color bodyColor = body.getColor();
            Vector3 bodyCenter = body.getPos() + new Vector3(0.0f, body.getHeight() * 0.5f, 0.0f);

            Vector3 leftWingBase = bodyCenter + new Vector3(-0.4f * body.getWidth(), 0.0f, 0.0f);
            Vector3 rightWingBase = bodyCenter + new Vector3(0.4f * body.getWidth(), 0.0f, 0.0f);
            leftWing = new Wing(leftWingBase, bodyColor);
            rightWing = new Wing(rightWingBase, leftWing);

            Vector3 beakBase = bodyCenter + new Vector3(0.0f, 0.0f, 0.4f * body.getLength());
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
            length = Random.Range(scale, scale * 1.2f);
            height = Random.Range(0.4f * scale, 0.6f * scale);
            width = Random.Range(0.4f * scale, 0.6f * scale);
            col = new Color (Random.Range(0.1f, 0.80f), Random.Range(0.1f, 0.80f), Random.Range(0.1f, 0.80f), 1.0f);
        }

        public Vector3 getPos() {
            return pos;
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
            width = Random.Range(0.1f * scale, 0.2f * scale);
            footHeight = Random.Range(0.4f * scale, 0.6f * scale);
            toeLength = Random.Range(0.2f * scale, 0.3f * scale);
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
            length = Random.Range(0.7f * scale, 0.9f * scale);
            width = Random.Range(0.4f * scale, 0.6f * scale);
            thickness = Random.Range(0.1f * scale, 0.2f * scale);
            col = bodyColor + new Color(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 1.0f);
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

        public Beak(Vector3 beakBase) {
            pos = beakBase;
            length = Random.Range(0.3f * scale, 0.5f * scale);
            width = Random.Range(0.2f * scale, 0.2f * scale);
            thickness = Random.Range(0.2f * scale, 0.2f * scale);
            type = Random.Range(1, 4);
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

    }

}