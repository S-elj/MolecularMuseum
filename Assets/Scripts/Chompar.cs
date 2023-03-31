using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
public abstract class Chompar {
    public const string URL = "https://raw.githubusercontent.com/Shyrogan/dummy_chompar/main/";

    public abstract class FileSystem {
        public static string TEAMS_FILE = Path.Combine(Application.persistentDataPath, "teams.json");
        public static string PROJECTS_FILE = Path.Combine(Application.persistentDataPath, "projects.json");
        public static string ASSETS_FILE = Path.Combine(Application.persistentDataPath, "assets.json");

        public static string ASSET_FOLDER = Path.Combine(Application.persistentDataPath, "assets");
        
        public static async Task Write(string fileName, string content) {
            if (!Directory.Exists(Application.persistentDataPath))
                Directory.CreateDirectory(Application.persistentDataPath);

            var file = Path.Combine(Application.persistentDataPath, fileName);
            await File.WriteAllTextAsync(file, content, Encoding.UTF8);
        }

        public static async Task<T> Load<T>(string fileName) {
            return JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync(Path.Combine(Application.persistentDataPath, fileName)));
        }

        public static async Task Download(Json.Asset asset) {
            var path = Path.Combine(ASSET_FOLDER, asset.Name);
            if (File.Exists(path)) {
                return;
            }
            
            using var assetRequest = UnityWebRequest.Get($"{URL}/asset/download/{asset.Name}");
            var assetResult = await assetRequest.SendWebRequest();
            if (assetResult == UnityWebRequest.Result.Success) {
                if (!Directory.Exists(ASSET_FOLDER))
                    Directory.CreateDirectory(ASSET_FOLDER);
                
                await File.WriteAllBytesAsync(path, assetRequest.downloadHandler.data);
            }
        }

        public static void Load(Json.Asset asset) {
            var path = Path.Combine(ASSET_FOLDER, asset.Name);
        }
    }

    
    /**
     * Une classe abstraite qui contient tout ce qui faut pour travailler autour du JSON.
     * Ici on parle uniquement du JSON, pour télécharger les assets il faut passer par
     * la méthode DownloadAsset dans FileSystem.
     */
    public abstract class Json {
        public static async Task<UnityWebRequest.Result> Update() {
            // Teams
            using var teamsRequest = UnityWebRequest.Get($"{URL}/team");
            var teamsResult = await teamsRequest.SendWebRequest();
            await FileSystem.Write(FileSystem.TEAMS_FILE, teamsRequest.downloadHandler.text);
            
            // Projects
            using var projectsRequest = UnityWebRequest.Get($"{URL}/project");
            var projectsResult = await projectsRequest.SendWebRequest();
            await FileSystem.Write(FileSystem.PROJECTS_FILE, projectsRequest.downloadHandler.text);
            
            // Assets
            using var assetsRequest = UnityWebRequest.Get($"{URL}/asset");
            var assetsResult = await assetsRequest.SendWebRequest();
            await FileSystem.Write(FileSystem.ASSETS_FILE, assetsRequest.downloadHandler.text);
            
            // Si une des requêtes échoue, on renvoie pourquoi
            return teamsResult != UnityWebRequest.Result.Success
                ? teamsResult
                : projectsResult != UnityWebRequest.Result.Success
                    ? projectsResult
                    : assetsResult != UnityWebRequest.Result.Success
                        ? assetsResult
                        : UnityWebRequest.Result.Success;
        }
        
        [Serializable]
        public class Team {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public List<string> Members { get; set; }
    
            public List<int> Projects { get; set; }
            public List<string> Asset { get; set; }
        }

        [Serializable]
        public class Project {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int Team { get; set; }
            public List<string> Assets { get; set; }
        }

        [Serializable]
        public class Asset {
            public string Name { get; set; }
            public long Size { get; set; }
            public string Sha256 { get; set; }
        }
    }
}

public static class UnityWebRequestExtension {
    public static TaskAwaiter<UnityWebRequest.Result> GetAwaiter(this UnityWebRequestAsyncOperation reqOp) {
        TaskCompletionSource<UnityWebRequest.Result> tsc = new();
        reqOp.completed += _ => tsc.TrySetResult(reqOp.webRequest.result);
 
        if (reqOp.isDone) tsc.TrySetResult(reqOp.webRequest.result);
 
        return tsc.Task.GetAwaiter();
    }
}