using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Hsinpa.Map
{
    public class FantasyMapModel
    {
        public Dictionary<uint, FM_Vertices_Type> vertices_dict = new Dictionary<uint, FM_Vertices_Type> ();
        public Dictionary<uint, FM_Cells_Type> cells_dict = new Dictionary<uint, FM_Cells_Type>();
        public Dictionary<uint, FM_Province_Type> provinces_dict = new Dictionary<uint, FM_Province_Type>();
        public Dictionary<uint, FM_State_Type> states_dict = new Dictionary<uint, FM_State_Type>();

        public async Task Load(string file_path)
        {
            string full_text = await File.ReadAllTextAsync(file_path);

            await Task.Run(() => {
                JSONNode simple_json = JSON.Parse(full_text);

                var cells = simple_json["cells"]["cells"].AsArray;
                var vertices = simple_json["cells"]["vertices"].AsArray;
                var provinces = simple_json["cells"]["provinces"].AsArray;
                var states = simple_json["cells"]["states"].AsArray;

                cells_dict = Load_Map_Cells(cells);
                vertices_dict = Load_Map_Vertices(vertices);
                provinces_dict = Load_Map_Province(provinces);
                states_dict = Load_Map_State(states);


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

    }
}