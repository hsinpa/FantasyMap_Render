using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LLama.Common;
using LLama;
using System.IO;
using System;
using System.Threading.Tasks;


namespace Hsinpa.Inference
{
    public class LllmInference
    {
        const string MODEL_NAME = "phi-2.Q4_K_M.gguf";

        public async void Load()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "models", MODEL_NAME);
            Debug.Log(path);

            var parameters = new ModelParams(path)
            {
                ContextSize = 1024, // The longest length of chat as memory.
                GpuLayerCount = 0 // How many layers to offload to GPU. Please adjust it according to your GPU memory.
            };

            string result = await Task.Run(async () =>
            {
                using var model = LLamaWeights.LoadFromFile(parameters);
                var ex = new StatelessExecutor(model, parameters);

                var inferenceParams = new InferenceParams() { Temperature = 1.0f, MaxTokens = 1024 };

                string prompt = "Instruct: list out what is must have for a country. Output:";
                string result = "";
                await foreach (var n in ex.InferAsync(prompt, inferenceParams))
                {
                    result += n;
                }

                return result;
            });

            Debug.Log(result);

        }
    }
}
