using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using SimpleJSON;

namespace Hsinpa {
    public class FantasyMapRender : MonoBehaviour
    {
        [SerializeField]
        private RenderTexture map_texture;

        List<FM_Vertices_Type> _vertices_list = new List<FM_Vertices_Type>();

        // Start is called before the first frame update
        void Start()
        {
            string path = Application.streamingAssetsPath + "/" + "Cheres PackCells.json";

            _ = Preload_JSONFile(path);
        }

        private async Task Preload_JSONFile(string file_path) {
            string full_text = await File.ReadAllTextAsync(file_path);

            await Task.Run(() => {
                JSONNode simple_json = JSON.Parse(full_text);

                var cells = simple_json["cells"]["cells"].AsArray;
                var vertices = simple_json["cells"]["vertices"].AsArray;

                var cell_dict = Load_Map_Cells(cells);
                // _vertices_list = Load_Map_Vertices(vertices);

                Debug.Log($"Vertices count {vertices.Count}");
                Debug.Log($"Cells count {cell_dict.Count}");

                Debug.Log("Simple json readed");
            });
        }

        private List<FM_Vertices_Type> Load_Map_Vertices(JSONArray vertices_array) {
            List<FM_Vertices_Type> vertices_list = new List<FM_Vertices_Type>();

            for (int i  = 0; i < vertices_array.Count; i++) {
                vertices_list.Add(
                    JsonUtility.FromJson<FM_Vertices_Type>(vertices_array[i].ToString())
                );
            }

            return vertices_list;
        }

        private Dictionary<uint, FM_Cells_Type> Load_Map_Cells(JSONArray cells_array) {
            Dictionary<uint, FM_Cells_Type> cell_dict = new Dictionary<uint, FM_Cells_Type>();

            for (int i  = 0; i < cells_array.Count; i++) {
                FM_Cells_Type cells = JsonUtility.FromJson<FM_Cells_Type>(cells_array[i].ToString());

                if (!cell_dict.ContainsKey(cells.i))
                    cell_dict.Add(cells.i, cells);
            }

            return cell_dict;
        }

    }
}
