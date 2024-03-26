using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using SimpleJSON;
using Hsinpa.Map;
using log4net.ObjectRenderer;
using UnityEngine.UI;
using System.Linq;
using System.Net;
using System;

namespace Hsinpa {
    public class FantasyMapRender : MonoBehaviour
    {
        [SerializeField]
        private RawImage debug_map_image;

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
        }


        async void RenderMap()
        {
            await PrepareModel();

            var vertices_dict = map_model.vertices_dict;
            var vertices_list = vertices_dict.Values.ToList();
            int vertices_lens = vertices_list.Count;

            //for (int i = 0; i < vertices_lens; i++)
            //{
            //    map_texture.SetPixel((int)vertices_list[i].p[0], HEIGHT - (int)vertices_list[i].p[1], Color.white);
            //}

            var cell_dict = map_model.cells_dict;
            var cell_list = cell_dict.Values.ToList();
            int cell_lens = cell_list.Count;

            for (int i = 0; i < cell_lens; i++)
            {
                FM_Cells_Type cell = cell_list[i];
                int c_vertice_lens = cell.v.Length;
                for (int v = 0; v < c_vertice_lens; v++)
                {
                    FM_Vertices_Type vertice_a = vertices_dict[cell.v[v]];
                    FM_Vertices_Type vertice_b;

                    if (v == 0) {
                        vertice_b = vertices_dict[cell.v[c_vertice_lens - 1]];
                    } else
                    {
                        vertice_b = vertices_dict[cell.v[v - 1]];
                    }

                    DrawLine(new Vector2(vertice_a.p[0], vertice_a.p[1]),
                            new Vector2(vertice_b.p[0], vertice_b.p[1]),
                            Color.green, map_texture
                        );
                }
            }



            map_texture.Apply();

            if (debug_map_image != null)
                debug_map_image.texture = map_texture;
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

    }
}
