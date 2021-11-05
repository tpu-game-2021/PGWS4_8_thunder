using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningScript : MonoBehaviour
{
    List<Vector3> posList = new List<Vector3>();
    private Mesh mesh;
    float resetTime;
    float timerMod;
    float modChecker;
    float speed;
    int portion;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        resetTime = 0.0f; 
    }

    // Update is called once per frame
    void Update()
    {
        modChecker = resetTime % speed;
        resetTime -= Time.deltaTime;

        if (resetTime < 0f)
        {
            resetTime = Random.Range(1f, 3f);
            timerMod = resetTime;
            posList.Clear();
            generateThunder();
            speed = timerMod / posList.Count;
            portion = 0;
        }

        if (modChecker < resetTime % speed)
        {
            portion++;
            generateMesh();
        }
    }

    void generateThunder()
    {
        Vector3 pos = new Vector3(0f, 1000f, 0f);

        Vector3 dir = new Vector3(0f, -1f, 0f);
        dir.Normalize();

        posList.Add(pos);

        for (int i = 0; i < 1000; i++)
        {
            float d = Random.Range(-15f, +15f);
            Quaternion q = Quaternion.AngleAxis(d, new Vector3(0f, 0f, 1f));
            dir = q * dir;
            dir.Normalize();

            float distance = Random.Range(30f, 50f);
            pos += dir * distance;

            posList.Add(pos);
            if (pos.y < 0f)
            {
                break;
            }
        }

        gameObject.transform.position = new Vector3(
            Random.Range(-300f, +300f),
            0f,
            Random.Range(-300f, +300f)
            );
    }
    
    void generateMesh()
    {
        if (posList.Count < 2)
        {
            return;
        }

        mesh.Clear();

        int vertex_Count = 2 * portion;

        Vector3[] vertices = new Vector3[vertex_Count];
        int vtx = 0;
        for (int i = 0; i < portion; i++)
        {
            Vector3 dir;
            if (i == 0)
            {
                dir = posList[1] - posList[0];
            }
            else if (i == portion - 1)
            {
                dir = posList[portion - 1] - posList[portion - 2];
            }
            else
            {
                dir = posList[i + 1] - posList[i - 1];
            }
            dir.Normalize();

            const float THUNDER_WIDTH = 5f;
            Vector3 t = new Vector3(-dir.y, dir.x, 0f) * THUNDER_WIDTH;

            vertices[vtx + 0] = posList[i] - t;
            vertices[vtx + 1] = posList[i] + t;
            vtx += 2;
        }

        Color baseColor = Color.red;
        Color[] colors = new Color[vertex_Count];
        float colorMod = 0.75f / posList.Count;
        for (int i = 0; i < vertex_Count; i++)
        {
            if (i % 2 == 0)
            {
                baseColor += new Color(0, colorMod, 0);
            }
            colors[i] = baseColor;      
        }

        int triangle_Count = 6 * (portion - 1);
        int[] triangles = new int[triangle_Count];
        int idx = 0;
        vtx = 0;
        while (idx < triangle_Count)
        {
            triangles[idx + 0] = vtx + 0;
            triangles[idx + 1] = vtx + 1;
            triangles[idx + 2] = vtx + 2;

            triangles[idx + 3] = vtx + 2;
            triangles[idx + 4] = vtx + 1;
            triangles[idx + 5] = vtx + 3;

            idx += 6;
            vtx += 2;
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
