using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Hsinpa.Map;

using Hsinpa.Algorithm;
using Unity.Mathematics;

namespace Hsinpa {
    public class FantasyMapRender : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter meshFilter;

        [SerializeField]
        private MeshRenderer meshRenderer;

        FantasyMapModel map_model;

        [SerializeField]
        Texture2D map_texture;

        const int WIDTH = 512;
        const int HEIGHT = 256;

        void Start()
        {
            map_model = new FantasyMapModel(WIDTH, HEIGHT);
            map_texture = new Texture2D(WIDTH, HEIGHT, TextureFormat.ARGB32, false);

            RenderMap();

            //var llm_inference = new LllmInference();
            //llm_inference.Load();
        }


        async void RenderMap()
        {
            await PrepareModel();

            Mesh mesh = new Mesh();
            MeshDataType meshDataType =  await Task.Run( () => map_model.RenderMapMesh());

            mesh.SetVertices(meshDataType.vertices);
            mesh.SetUVs(channel: 0, uvs: meshDataType.uvs);
            mesh.SetTriangles(meshDataType.triangles, 0);

            meshFilter.mesh = mesh;
            meshRenderer.material.SetTexture("_MainTex", map_texture);

            byte[] color_buffer = await RenderTexture(WIDTH, HEIGHT);
            map_texture.SetPixelData(color_buffer, 0);
            map_texture.Apply();
        }

        async Task PrepareModel()
        {
            string path = Application.streamingAssetsPath + "/" + "Cheres PackCells.json";

            await map_model.Load(path);
        }

        private async Task<byte[]> RenderTexture(int width, int height)
        {
            byte[] color_bytes = new byte[width * height * 4];
            List<QuadTreeUti.Point> points = new List<QuadTreeUti.Point>();
            float2 extend = new float2(4, 4);

            await Task.Run(() =>
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        points.Clear();

                        map_model.quadTree.RecursiveQueryRect(query_area: 4, x:x, y:y, current_expansion: 0, max_expansion: 4, ref points);

                        int index = (x + (width * y)) * 4;

                        color_bytes[index] = 255; // Alpha

                        color_bytes[index + 1] = 0; // Red

                        color_bytes[index + 2] = 255; // Green

                        color_bytes[index + 3] = 0; // Blue
                    }
                }
            });

            return color_bytes;
        }
    }
}
