using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start() {
        UnityEngine.Debug.Log("Starting...");
        
        
        // var r = await Chompar.Json.Update();
        // var teams = await Chompar.FileSystem.Load<List<Chompar.Json.Team>>(Chompar.FileSystem.TEAMS_FILE);
        // var projects = await Chompar.FileSystem.Load<List<Chompar.Json.Project>>(Chompar.FileSystem.PROJECTS_FILE);
        // var assets = await Chompar.FileSystem.Load<List<Chompar.Json.Asset>>(Chompar.FileSystem.ASSETS_FILE);

        // UnityEngine.Debug.Log($"{teams.Count} {projects.Count} {assets.Count}");

        var ass = new Chompar.Json.Asset();
        ass.Name = "ICSM.obj";
        ass.Sha256 = "0b17574a544ec43f1b7f2b6fbb346195b1516847261f18122f4d049f686a9164";
        ass.Size = 3818320;

        
        await Chompar.FileSystem.Download(ass);
        Chompar.FileSystem.Load(ass);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
