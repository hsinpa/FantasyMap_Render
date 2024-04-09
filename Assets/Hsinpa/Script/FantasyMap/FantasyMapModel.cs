using Hsinpa.Algorithm;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace Hsinpa.Map
{
    public class FantasyMapModel
    {
        public Dictionary<uint, FM_Vertices_Type> vertices_dict = new Dictionary<uint, FM_Vertices_Type> ();
        public Dictionary<uint, FM_Cells_Type> cells_dict = new Dictionary<uint, FM_Cells_Type>();
        public Dictionary<uint, FM_Province_Type> provinces_dict = new Dictionary<uint, FM_Province_Type>();
        public Dictionary<uint, FM_State_Type> states_dict = new Dictionary<uint, FM_State_Type>();

        public QuadTree quadTree;
        private int _width;
        private int _height;

        public FantasyMapModel(int map_width, int map_height)
        {
            this._width = map_width;
            this._height = map_height;

            float2 extend = new float2(map_width * 0.5f, map_height * 0.5f);
            quadTree = new QuadTree(new QuadTreeUti.QuadRect(extend.x, extend.y, extend));
        }

        #region Mesh
        public FM_Cells_Type GetCellByPosition(int x, int y, List<int> vertices_ids) {
                

            return default(FM_Cells_Type);
        }


        public MeshDataType RenderMapMesh()
        {
            MeshDataType mesh = new MeshDataType();
            var vertices_list = this.vertices_dict.Values.ToList();
            int vertices_lens = vertices_list.Count;

            var cell_list = this.cells_dict.Values.ToList();
            int cell_lens = cell_list.Count;

            Vector3[] vertices = new Vector3[vertices_lens + cell_lens];
            Vector2[] uvs = new Vector2[vertices_lens + cell_lens];
            List<int> triangles = new List<int>();

            // Create Vertices and UV
            for (int vertices_index = 0; vertices_index < vertices_lens; vertices_index++)
            {
                FM_Vertices_Type vertices_type = vertices_list[vertices_index];
                Vector3 vertice_position = new Vector3(vertices_type.p[0], 0, vertices_type.p[1]);
                vertices[vertices_index] = vertice_position;
                uvs[vertices_index] = new Vector2(vertices_type.p[0] / (float)this._width, vertices_type.p[1] / (float)this._height);
            
                quadTree.Insert(new QuadTreeUti.Point() { x = vertice_position.x, y = vertice_position.z, id = (int)vertices_type.i});
            }

            // Create Triangle
            for (int cell_index = 0; cell_index < cell_lens; cell_index++)
            {
                int connect_vertices_lens = cell_list[cell_index].v.Length;
                Vector3 cell_center = new Vector3(cell_list[cell_index].p[0], 0, cell_list[cell_index].p[1]);

                int cell_vertices_index = vertices_lens + cell_index;
                vertices[cell_vertices_index] = cell_center;
                uvs[cell_vertices_index] = new Vector2(cell_center.x / (float)this._width, cell_center.z / (float)this._height);

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
        #endregion

        #region Load Data
        public async Task Load(string file_path)
        {
            string full_text = await File.ReadAllTextAsync(file_path);

            await Task.Run(() => {
                JSONNode simple_json = JSON.Parse(full_text);

                var cells = simple_json["cells"]["cells"].AsArray;
                var vertices = simple_json["cells"]["vertices"].AsArray;
                var provinces = simple_json["cells"]["provinces"].AsArray;
                var states = simple_json["cells"]["states"].AsArray;

                provinces_dict = Load_Map_Province(provinces);
                states_dict = Load_Map_State(states);

                cells_dict = Load_Map_Cells(cells);
                vertices_dict = Load_Map_Vertices(vertices);

                Debug.Log("Simple json readed");
            });
        }

        private Dictionary<uint, FM_Vertices_Type> Load_Map_Vertices(JSONArray vertices_array)
        {
            Dictionary<uint, FM_Vertices_Type> vertices_dict = new Dictionary<uint, FM_Vertices_Type>();

            for (int i = 0; i < vertices_array.Count; i++)
            {
                FM_Vertices_Type vertices = JsonUtility.FromJson<FM_Vertices_Type>(vertices_array[i].ToString());

                vertices_dict.Add(vertices.i, vertices);
            }

            return vertices_dict;
        }

        private Dictionary<uint, FM_Cells_Type> Load_Map_Cells(JSONArray cells_array)
        {
            Dictionary<uint, FM_Cells_Type> cell_dict = new Dictionary<uint, FM_Cells_Type>();

            for (int i = 0; i < cells_array.Count; i++)
            {
                FM_Cells_Type cells = JsonUtility.FromJson<FM_Cells_Type>(cells_array[i].ToString());

                //Save to Province
                if (provinces_dict.TryGetValue(cells.province, out var province_type))
                {
                    province_type.cells.Add(cells);
                }

                if (!cell_dict.ContainsKey(cells.i))
                    cell_dict.Add(cells.i, cells);
            }

            return cell_dict;
        }

        private Dictionary<uint, FM_Province_Type> Load_Map_Province(JSONArray provinces_array)
        {
            Dictionary<uint, FM_Province_Type> province_dict = new Dictionary<uint, FM_Province_Type>();

            for (int i = 0; i < provinces_array.Count; i++)
            {
                string raw_json = provinces_array[i].ToString();

                if (!raw_json.StartsWith("{")) continue;

                FM_Province_Type province = JsonUtility.FromJson<FM_Province_Type>(raw_json);
                province.cells = new List<FM_Cells_Type>();

                province_dict.Add(province.i, province);
            }

            return province_dict;
        }

        private Dictionary<uint, FM_State_Type> Load_Map_State(JSONArray states_array)
        {
            Dictionary<uint, FM_State_Type> states_dict = new Dictionary<uint, FM_State_Type>();

            for (int i = 0; i < states_array.Count; i++)
            {
                string raw_json = states_array[i].ToString();
                Debug.Log(raw_json);
            }

            return states_dict;
        }
        #endregion
    }
}