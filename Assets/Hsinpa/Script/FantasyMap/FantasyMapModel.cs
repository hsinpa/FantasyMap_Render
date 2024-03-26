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

        public async Task Load(string file_path)
        {
            string full_text = await File.ReadAllTextAsync(file_path);

            await Task.Run(() => {
                JSONNode simple_json = JSON.Parse(full_text);

                var cells = simple_json["cells"]["cells"].AsArray;
                var vertices = simple_json["cells"]["vertices"].AsArray;

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

                if (!cell_dict.ContainsKey(cells.i))
                    cell_dict.Add(cells.i, cells);
            }

            return cell_dict;
        }

    }
}