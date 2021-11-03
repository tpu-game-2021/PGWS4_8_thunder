using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalkThunderGenerator : MonoBehaviour
{
    private Mesh mesh;
    float resetTime;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        resetTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        resetTime -= Time.deltaTime;
        if (resetTime < 0.0f)
        {
            resetTime = Random.Range(1.0f, 3.0f);
            generateThunder();
        }
    }
       
         #region
    void generateThunder()
    {
        Vector3 pos = new Vector3(0.0f, 1000.0f, 0.0f);

        Vector3 dir = new Vector3(0.0f, -1.0f, 0.0f);
        dir.Normalize();

        List<Vector3> posList = new List<Vector3>();

        posList.Add(pos);

       for(int i = 0; i < 1000; i++)
        {
            float d = Random.Range(-15.0f, +15.0f);
            Quaternion q = Quaternion.AngleAxis(d, new Vector3(0.0f, 0.0f, 1.0f));
            dir = q * dir;
            dir.Normalize();

            float distance = Random.Range(30.0f, 50.0f);
            pos += dir * distance;

            posList.Add(pos);

            if (pos.y < 0.0f) break;
        }

           generateMesh(posList);

           gameObject.transform.position = new Vector3(
            Random.Range(-300.0f, +300.0f), 0.0f, Random.Range(-300.0f, +300.0f));
    }
       
        #endregion

    void generateMesh(List<Vector3> a_pos)
    {
        if (a_pos.Count < 2) return;

        mesh.Clear();

        int vertex_count = 2 * a_pos.Count;

        Vector3[] vertices = new Vector3[vertex_count];
        int vtx = 0;
        for(int i = 0; i < a_pos.Count; i++)
        {
            Vector3 dir;
            if (i == 0)
            {
                dir = a_pos[1] - a_pos[0];
            }
            else if (i == a_pos.Count - 1) 
            {
                dir = a_pos[a_pos.Count - 1] - a_pos[a_pos.Count - 2];
            }
            else
            {
                dir = a_pos[i + 1] - a_pos[i - 1];
            }
            dir.Normalize();

            const float THUNDER_WIDTH = 5.0f;
            Vector3 t = new Vector3(-dir.y, dir.x, 0.0f) * THUNDER_WIDTH;

            vertices[vtx + 0] = a_pos[i] - t;
            vertices[vtx + 1] = a_pos[i] + t;

            vtx += 2;
        }


        Color[] colors = new Color[vertex_count];
        for (int i = 0; i < vertex_count; i++)
        {
            colors[i] = Color.white;
        }

        int triangle_count = 6 * (a_pos.Count - 1);
        int[] triangles = new int[triangle_count];
        int idx = 0;
        vtx = 0;

        while (idx < triangle_count)
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
