using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using SimpleJSON;
using Hsinpa.Map;
using UnityEngine.UI;
using System.Linq;
using System.Net;
using System;
using Hsinpa.Inference;

namespace Hsinpa {
    public class FantasyMapRender : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter meshFilter;

        FantasyMapModel map_model;
        Texture2D map_texture;

        const int WIDTH = 512;
        const int HEIGHT = 256;
        Color32[] empty_color_set = new Color32[WIDTH * HEIGHT];

        // Start is called before the first frame update
        void Start()
        {
            map_model = new FantasyMapModel();
            map_texture = new Texture2D(WIDTH, HEIGHT, TextureFormat.ARGB32, false);

            RenderMap();

            //var llm_inference = new LllmInference();
            //llm_inference.Load();
        }


        async void RenderMap()
        {
            await PrepareModel();

            Mesh mesh = new Mesh();
            MeshDataType meshDataType =  await Task.Run( () => RenderMapMesh(WIDTH, HEIGHT, map_model.vertices_dict, map_model.cells_dict));

            mesh.SetVertices(meshDataType.vertices);
            mesh.SetUVs(channel: 0, uvs: meshDataType.uvs);
            mesh.SetTriangles(meshDataType.triangles, 0);

            meshFilter.mesh = mesh;
        }

        private void DrawLine(Vector2 point1, Vector2 point2, Color color, Texture2D targetTexture)
        {
            Vector2 t = point1;
            float frac = 1 / Mathf.Sqrt(Mathf.Pow(point2.x - point1.x, 2) + Mathf.Pow(point2.y - point1.y, 2));
            float ctr = 0;

            while ((int)t.x != (int)point2.x || (int)t.y != (int)point2.y)
            {
                t = Vector2.Lerp(point1, point2, ctr);
                ctr += frac;
                targetTexture.SetPixel((int)t.x, (int)t.y, color);
            }
        }

        async Task PrepareModel()
        {
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    empty_color_set[x + (y * WIDTH)] = new Color(0, 0, 0, 0);
                }
            }

            string path = Application.streamingAssetsPath + "/" + "Cheres PackCells.json";

            await map_model.Load(path);
        }


        private MeshDataType RenderMapMesh(int width, int height, Dictionary<uint, FM_Vertices_Type> vertices_dict, Dictionary<uint, FM_Cells_Type> cells_dict)
        {
            MeshDataType mesh = new MeshDataType();
            var vertices_list = vertices_dict.Values.ToList();
            int vertices_lens = vertices_list.Count;

            var cell_list = cells_dict.Values.ToList();
            int cell_lens = cell_list.Count;

            Vector3[] vertices = new Vector3[vertices_lens + cell_lens];
            Vector2[] uvs = new Vector2[vertices_lens + cell_lens];
            List<int> triangles = new List<int>();

            // Create Vertices and UV
            for (int vertices_index = 0; vertices_index < vertices_lens; vertices_index++)
            {
                FM_Vertices_Type vertices_type = vertices_list[vertices_index];
                vertices[vertices_index] = new Vector3(vertices_type.p[0], 0, vertices_type.p[1]);
                uvs[vertices_index] = new Vector2(vertices_type.p[0] / width, vertices_type.p[1] / height);
            }

            // Create Triangle
            for (int cell_index = 0; cell_index < cell_lens; cell_index++) {
                int connect_vertices_lens = cell_list[cell_index].v.Length;
                Vector3 cell_center = new Vector3(cell_list[cell_index].p[0], 0, cell_list[cell_index].p[1]);

                int cell_vertices_index = vertices_lens + cell_index;
                vertices[cell_vertices_index] = cell_center;
                uvs[cell_vertices_index] = new Vector2(cell_center.x / width, cell_center.y / height);


                for (int cv_index = 0; cv_index < connect_vertices_lens; cv_index++)
                {
                    int child_a = (int)cell_list[cell_index].v[cv_index];

                    int child_b_index = (cv_index == 0) ? connect_vertices_lens - 1 : cv_index - 1;
                    int child_b = (int)cell_list[cell_index].v[child_b_index];

                    triangles.Add(cell_vertices_index);
                    triangles.Add(child_a);
                    triangles.Add(child_b);
                }
            }

            mesh.vertices = (vertices);
            mesh.uvs = uvs;
            mesh.triangles = triangles;

            return mesh;
        }
    }
}
