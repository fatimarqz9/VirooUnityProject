#nullable enable
using System;
using System.Collections;
using System.IO;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.Networking;
using Viroo.SceneLoader.SceneContext;

namespace VirooLab
{
    public class LoadTexture : MonoBehaviour
    {
        [SerializeField]
        private string folderName = default!;

        [SerializeField]
        private string textureFileName = string.Empty;

        [SerializeField]
        private Renderer pictureRenderer = default!;

        private ILogger<LoadTexture>? logger;

        protected void Inject(ISceneContextProvider sceneContextProvider, ILogger<LoadTexture> logger)
        {
            this.logger = logger;

            string path = Path.Combine(sceneContextProvider.ResourcesFolderAbsolutePath, folderName, textureFileName);
            StartCoroutine(GetTexture(path));
        }

        private IEnumerator GetTexture(string path)
        {
            Uri uri = new(path);
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                request.downloadHandler = new DownloadHandlerTexture();
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    logger!.LogError("Could not load texture from {Path}", path);
                    yield break;
                }

                Texture texture = DownloadHandlerTexture.GetContent(request);

                if (!texture)
                {
                    logger!.LogError("Error getting content for loaded image from {Path}", path);
                    yield break;
                }

                pictureRenderer.material.mainTexture = texture;

                float scaleFactor = (float)texture.height / texture.width;

                pictureRenderer.transform.localScale = new Vector3(1, scaleFactor, 1);

                pictureRenderer.gameObject.SetActive(value: true);
            }
        }

        protected void Awake()
        {
            pictureRenderer.gameObject.SetActive(value: false);

            this.QueueForInject();
        }
    }
}
