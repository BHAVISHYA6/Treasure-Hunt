using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class SceneDirector : MonoBehaviour
{
    public enum SceneType { AutoDetect, IslandCove, AncientTemple, TreasureVault }

    [Header("Scene Configuration")]
    public SceneType sceneType = SceneType.AutoDetect;
    public bool autoBuildOnAwake = true;

    private const string CharacterPrefabPath = "Assets/Models/RPG Tiny Hero Duo/Prefab/MaleCharacterPBR.prefab";

    private void Awake()
    {
        if (Application.isPlaying)
        {
            EnsureManagersExist();
            if (autoBuildOnAwake)
            {
                BuildSceneIfNeeded();
            }
        }
    }

    private void Start()
    {
        if (Application.isPlaying)
        {
            EnsurePlayerAndCamera();
        }
    }

    public void BuildSceneIfNeeded()
    {
        SceneType current = ResolveSceneType();

        // Check if level environment already exists in the hierarchy
        if (GameObject.Find("IslandGround") != null ||
            GameObject.Find("EntranceHallGround") != null ||
            GameObject.Find("VaultGround") != null)
        {
            return; // Already populated
        }

        switch (current)
        {
            case SceneType.IslandCove:
                BuildIslandCove();
                break;
            case SceneType.AncientTemple:
                BuildAncientTemple();
                break;
            case SceneType.TreasureVault:
                BuildTreasureVault();
                break;
        }
    }

    private SceneType ResolveSceneType()
    {
        if (sceneType != SceneType.AutoDetect) return sceneType;

        string name = SceneManager.GetActiveScene().name.ToLower();
        if (name.Contains("2") || name.Contains("temple") || name.Contains("dungeon"))
            return SceneType.AncientTemple;
        if (name.Contains("3") || name.Contains("vault"))
            return SceneType.TreasureVault;
        return SceneType.IslandCove;
    }

    public static void EnsureManagersExist()
    {
        if (GameManager.Instance == null && FindObjectOfType<GameManager>() == null)
        {
            GameObject gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
        }

        if (SoundManager.Instance == null && FindObjectOfType<SoundManager>() == null)
        {
            GameObject sm = new GameObject("SoundManager");
            sm.AddComponent<SoundManager>();
        }

        if (GameHUD.Instance == null && FindObjectOfType<GameHUD>() == null)
        {
            GameObject hud = new GameObject("GameHUD");
            hud.AddComponent<GameHUD>();
        }
    }

    private void EnsurePlayerAndCamera()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        Vector3 spawnPos = Vector3.zero;

        SceneType current = ResolveSceneType();
        switch (current)
        {
            case SceneType.IslandCove: spawnPos = new Vector3(0f, 0.5f, -8f); break;
            case SceneType.AncientTemple: spawnPos = new Vector3(0f, 0.5f, -14f); break;
            case SceneType.TreasureVault: spawnPos = new Vector3(0f, 0.5f, -12f); break;
        }

        if (player == null)
        {
            player = CreatePlayerInstance(spawnPos);
        }
        else
        {
            // Position existing player at spawn
            var cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            player.transform.position = spawnPos;
            if (cc != null) cc.enabled = true;
        }

        ThirdPersonCamera cam = FindObjectOfType<ThirdPersonCamera>();
        if (cam == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                cam = mainCam.gameObject.AddComponent<ThirdPersonCamera>();
                cam.target = player.transform;
            }
            else
            {
                GameObject camObj = new GameObject("MainCamera");
                camObj.tag = "MainCamera";
                mainCam = camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
                cam = camObj.AddComponent<ThirdPersonCamera>();
                cam.target = player.transform;
            }
        }
        else
        {
            cam.target = player.transform;
        }
    }

    public static PlayerController CreatePlayerInstance(Vector3 spawnPosition)
    {
        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        playerObj.transform.position = spawnPosition;

        CharacterController cc = playerObj.AddComponent<CharacterController>();
        cc.center = new Vector3(0f, 0.9f, 0f);
        cc.height = 1.8f;
        cc.radius = 0.45f;
        cc.stepOffset = 0.35f;

        PlayerController pc = playerObj.AddComponent<PlayerController>();

#if UNITY_EDITOR
        GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(CharacterPrefabPath);
        if (prefab != null)
        {
            GameObject charModel = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, playerObj.transform);
            charModel.transform.localPosition = Vector3.zero;
            charModel.transform.localRotation = Quaternion.identity;
            return pc;
        }
#endif

        // Stylized visual mesh fallback
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "HeroBody";
        body.transform.SetParent(playerObj.transform, false);
        body.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        body.transform.localScale = new Vector3(0.7f, 0.9f, 0.7f);
        body.GetComponent<Renderer>().material = ProceduralProps.GetGlowingMaterial(new Color(0.2f, 0.5f, 0.9f), "PlayerHeroMat");
        Object.DestroyImmediate(body.GetComponent<Collider>());

        return pc;
    }

    // ==========================================
    // BUILD LEVEL 1: THE TROPICAL COVE
    // ==========================================
    [ContextMenu("Build Scene In Editor")]
    public void BuildFromContextMenu()
    {
        ClearBuiltScene();
        BuildSceneIfNeeded();
        EnsurePlayerAndCamera();
        Debug.Log("[SceneDirector] Scene generated successfully in Editor!");
    }

    [ContextMenu("Clear Built Scene")]
    public void ClearBuiltScene()
    {
        string[] rootNames = new string[] {
            "IslandGround", "TowerBase", "TowerStep1", "TowerTop", "OffshoreIslet", "NorthCacheIslet",
            "EntranceHallGround", "ChasmMist", "InnerSanctumGround", "CentralPuzzleDais",
            "VaultGround", "DaisStep1", "DaisStep2", "DaisTop"
        };

        foreach (var name in rootNames)
        {
            var obj = GameObject.Find(name);
            if (obj != null) DestroyImmediate(obj);
        }
    }

    private void BuildIslandCove()
    {
        GameObject envParent = new GameObject("IslandCoveEnvironment");

        // 1. Sand Island Ground
        GameObject island = GameObject.CreatePrimitive(PrimitiveType.Cube);
        island.name = "IslandGround";
        island.transform.position = new Vector3(0f, -0.5f, 5f);
        island.transform.localScale = new Vector3(32f, 1f, 36f);
        island.GetComponent<Renderer>().material = ProceduralProps.GetSandMaterial();
        island.transform.SetParent(envParent.transform);

        // 2. Solid Coastline Cliff Barriers (Enclose the cove so player cannot walk into empty void)
        // South beach cliff rocks
        ProceduralProps.CreateCliffWall(new Vector3(-12f, 2f, -12f), new Vector3(10f, 4f, 4f), Quaternion.Euler(0f, 15f, 0f)).transform.SetParent(envParent.transform);
        ProceduralProps.CreateCliffWall(new Vector3(12f, 2f, -12f), new Vector3(10f, 4f, 4f), Quaternion.Euler(0f, -15f, 0f)).transform.SetParent(envParent.transform);
        // West cliff wall
        ProceduralProps.CreateCliffWall(new Vector3(-17f, 3f, 5f), new Vector3(3f, 6f, 38f), Quaternion.identity).transform.SetParent(envParent.transform);
        // East cliff wall
        ProceduralProps.CreateCliffWall(new Vector3(17f, 3f, 12f), new Vector3(3f, 6f, 24f), Quaternion.identity).transform.SetParent(envParent.transform);

        // Large solid coastal boulders
        ProceduralProps.CreateRockObstacle(new Vector3(-6f, 1f, -9f), new Vector3(2.5f, 2f, 2.5f), Quaternion.Euler(10f, 45f, 5f)).transform.SetParent(envParent.transform);
        ProceduralProps.CreateRockObstacle(new Vector3(5f, 1.2f, -8f), new Vector3(3f, 2.4f, 2f), Quaternion.Euler(0f, 30f, 0f)).transform.SetParent(envParent.transform);
        ProceduralProps.CreateRockObstacle(new Vector3(-13f, 1.5f, 3f), new Vector3(3.5f, 3f, 3f), Quaternion.Euler(5f, 60f, 10f)).transform.SetParent(envParent.transform);
        ProceduralProps.CreateRockObstacle(new Vector3(14f, 1.5f, 4f), new Vector3(3f, 2.8f, 3.2f), Quaternion.Euler(0f, 75f, 0f)).transform.SetParent(envParent.transform);

        // 3. Pirate Campfire Site at Beach Landing
        ProceduralProps.CreateCampfire(new Vector3(0f, 0f, -6f)).transform.SetParent(envParent.transform);

        // 4. Water Hazard Plane (Water now has WaterHazard component - splash, damage & camp respawn!)
        GameObject water = ProceduralProps.CreateWaterPlane(new Vector3(0f, -0.4f, 5f), new Vector2(80f, 80f));
        water.transform.SetParent(envParent.transform);

        // 5. Ruined Stone Watchtower (Solid stepped platforms to Map Fragment #1)
        GameObject towerBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
        towerBase.name = "TowerBase";
        towerBase.transform.position = new Vector3(-8f, 0.5f, 10f);
        towerBase.transform.localScale = new Vector3(6f, 1f, 6f);
        towerBase.GetComponent<Renderer>().material = ProceduralProps.GetStoneMaterial();
        towerBase.transform.SetParent(envParent.transform);

        GameObject towerStep1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        towerStep1.name = "TowerStep1";
        towerStep1.transform.position = new Vector3(-8f, 1.5f, 10f);
        towerStep1.transform.localScale = new Vector3(4.5f, 1f, 4.5f);
        towerStep1.GetComponent<Renderer>().material = ProceduralProps.GetDarkStoneMaterial();
        towerStep1.transform.SetParent(envParent.transform);

        GameObject towerStep2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        towerStep2.name = "TowerTop";
        towerStep2.transform.position = new Vector3(-8f, 2.5f, 10f);
        towerStep2.transform.localScale = new Vector3(3f, 1f, 3f);
        towerStep2.GetComponent<Renderer>().material = ProceduralProps.GetStoneMaterial();
        towerStep2.transform.SetParent(envParent.transform);

        // Watchtower Pillars (Solid CapsuleColliders)
        ProceduralProps.CreateRuinPillar(new Vector3(-10.5f, 0f, 7.5f), 4f).transform.SetParent(envParent.transform);
        ProceduralProps.CreateRuinPillar(new Vector3(-5.5f, 0f, 7.5f), 3.5f).transform.SetParent(envParent.transform);
        ProceduralProps.CreateRuinPillar(new Vector3(-10.5f, 0f, 12.5f), 4.5f).transform.SetParent(envParent.transform);
        ProceduralProps.CreateRuinPillar(new Vector3(-5.5f, 0f, 12.5f), 3f).transform.SetParent(envParent.transform);

        // 6. Solid Palm Trees (Now with impassable root CapsuleColliders)
        ProceduralProps.CreatePalmTree(new Vector3(9f, 0f, -4f), 5.5f).transform.SetParent(envParent.transform);
        ProceduralProps.CreatePalmTree(new Vector3(-10f, 0f, -5f), 5f).transform.SetParent(envParent.transform);
        ProceduralProps.CreatePalmTree(new Vector3(12f, 0f, 8f), 6f).transform.SetParent(envParent.transform);
        ProceduralProps.CreatePalmTree(new Vector3(7f, 0f, 16f), 5.2f).transform.SetParent(envParent.transform);
        ProceduralProps.CreatePalmTree(new Vector3(-12f, 0f, 17f), 4.8f).transform.SetParent(envParent.transform);

        // 7. Stepping Stones across water to Offshore Islet
        for (int i = 0; i < 4; i++)
        {
            GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stone.name = $"TidalStone_{i}";
            stone.transform.position = new Vector3(12f + i * 2.8f, -0.1f, -2f + Mathf.Sin(i) * 1.5f);
            stone.transform.localScale = new Vector3(1.8f, 0.4f, 1.8f);
            stone.GetComponent<Renderer>().material = ProceduralProps.GetStoneMaterial();
            stone.transform.SetParent(envParent.transform);
        }

        // Offshore Islet
        GameObject islet = GameObject.CreatePrimitive(PrimitiveType.Cube);
        islet.name = "OffshoreIslet";
        islet.transform.position = new Vector3(26f, -0.2f, -2f);
        islet.transform.localScale = new Vector3(7f, 0.8f, 7f);
        islet.GetComponent<Renderer>().material = ProceduralProps.GetSandMaterial();
        islet.transform.SetParent(envParent.transform);
        ProceduralProps.CreatePalmTree(new Vector3(27f, 0f, -1f), 4.5f).transform.SetParent(envParent.transform);
        ProceduralProps.CreateRockObstacle(new Vector3(28f, 0.8f, -4f), new Vector3(2.5f, 1.8f, 2f), Quaternion.identity).transform.SetParent(envParent.transform);

        // 8. Moving Tidal Platform to Northern Cache
        GameObject mp = ProceduralProps.CreateMovingPlatform(new Vector3(-4f, 0.3f, 21f), new Vector3(0f, 0f, 6f), 2.2f);
        mp.transform.SetParent(envParent.transform);

        GameObject northIslet = GameObject.CreatePrimitive(PrimitiveType.Cube);
        northIslet.name = "NorthCacheIslet";
        northIslet.transform.position = new Vector3(-4f, 0.2f, 29f);
        northIslet.transform.localScale = new Vector3(5f, 0.8f, 5f);
        northIslet.GetComponent<Renderer>().material = ProceduralProps.GetStoneMaterial();
        northIslet.transform.SetParent(envParent.transform);

        // 9. Clue #1: Captain's Logbook
        ProceduralProps.CreateClueScroll(
            new Vector3(0f, 0f, -4.5f),
            "Captain's Logbook",
            "Ahoy brave explorer!\n\nCaptain Redbeard hid the key to the Forbidden Temple across this cove.\n\nOBJECTIVE:\n1. Collect 10 Gold Coins scattered across the cove.\n2. Find the 3 hidden Map Fragments (Watchtower, Offshore Islet, North Cache).\n\nWhen 10 coins are gathered, your ship will set sail automatically to the Forbidden Temple!\n\nBeware the deep ocean water — falling into the sea will cost you a life!",
            "Read"
        ).transform.SetParent(envParent.transform);

        // 10. Map Fragments (Coins of type MapFragment)
        ProceduralProps.CreateCollectibleCoin(new Vector3(-8f, 3.8f, 10f), CollectibleCoin.CoinType.MapFragment).transform.SetParent(envParent.transform);
        ProceduralProps.CreateCollectibleCoin(new Vector3(26f, 0.8f, -2f), CollectibleCoin.CoinType.MapFragment).transform.SetParent(envParent.transform);
        ProceduralProps.CreateCollectibleCoin(new Vector3(-4f, 1.2f, 29f), CollectibleCoin.CoinType.MapFragment).transform.SetParent(envParent.transform);

        // 11. Sea Shrine with Bronze Key (unlocked when 3 fragments collected)
        GameObject shrineBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        shrineBase.name = "SeaShrineBase";
        shrineBase.transform.position = new Vector3(8f, 0.4f, 6f);
        shrineBase.transform.localScale = new Vector3(2.5f, 0.4f, 2.5f);
        shrineBase.GetComponent<Renderer>().material = ProceduralProps.GetDarkStoneMaterial();
        shrineBase.transform.SetParent(envParent.transform);

        ProceduralProps.CreateKeyPickup(
            new Vector3(8f, 1.4f, 6f),
            KeyPickup.KeyType.Bronze,
            "Bronze Skeleton Key",
            true,
            true
        ).transform.SetParent(envParent.transform);

        // 12. Bonus Chest in Palm Grove
        ProceduralProps.CreateTreasureChest(new Vector3(12f, 0f, 12f), Quaternion.Euler(0f, -45f, 0f), false, KeyPickup.KeyType.Bronze, 5, false).transform.SetParent(envParent.transform);

        // 13. Ancient Sea Gate (Portal to Scene 2)
        ProceduralProps.CreatePortalArchway(
            new Vector3(0f, 0f, 18f),
            Quaternion.Euler(0f, 0f, 0f),
            "Scene2_AncientTemple",
            "Ancient Sea Gate",
            reqKey: true,
            keyType: KeyPickup.KeyType.Bronze,
            portalColor: new Color(0.2f, 0.8f, 1f)
        ).transform.SetParent(envParent.transform);

        // 14. Exactly 15 Gold Coins across the 5 cove zones!
        Vector3[] coins = new Vector3[] {
            // Zone 1: Campfire Beach Landing (3 coins)
            new Vector3(0f, 0.6f, -2.5f),
            new Vector3(-2.5f, 0.6f, -3.5f),
            new Vector3(2.5f, 0.6f, -3.5f),

            // Zone 2: Palm Grove Path (3 coins)
            new Vector3(-2f, 0.6f, 1.5f),
            new Vector3(-4f, 0.6f, 4.5f),
            new Vector3(2f, 0.6f, 2.5f),

            // Zone 3: Ruined Watchtower Steps (3 coins)
            new Vector3(-8f, 1.2f, 7.5f),
            new Vector3(-8f, 1.8f, 10f),
            new Vector3(-8f, 2.8f, 10f),

            // Zone 4: Tidal Stepping Stones & Shore (3 coins)
            new Vector3(7f, 0.6f, 0.5f),
            new Vector3(12f, 0.6f, -2f),
            new Vector3(17.5f, 0.6f, -2f),

            // Zone 5: North Path & Moving Platform Cache (3 coins)
            new Vector3(0f, 0.6f, 8f),
            new Vector3(0f, 0.6f, 13f),
            new Vector3(-4f, 1.2f, 29f)
        };

        foreach (var c in coins)
        {
            ProceduralProps.CreateCollectibleCoin(c, CollectibleCoin.CoinType.StandardCoin, 1).transform.SetParent(envParent.transform);
        }
    }

    // ==========================================
    // BUILD LEVEL 2: THE ANCIENT TEMPLE
    // ==========================================
    private void BuildAncientTemple()
    {
        GameObject envParent = new GameObject("AncientTempleEnvironment");

        // 1. Solid Stone Dungeon Floors
        // Entrance Hall Ground
        GameObject enterHall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        enterHall.name = "EntranceHallGround";
        enterHall.transform.position = new Vector3(0f, -0.5f, -9f);
        enterHall.transform.localScale = new Vector3(18f, 1f, 14f);
        enterHall.GetComponent<Renderer>().material = ProceduralProps.GetDarkStoneMaterial();
        enterHall.transform.SetParent(envParent.transform);

        // Chasm Mist (Hazard Void below)
        GameObject chasmBottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
        chasmBottom.name = "ChasmMist";
        chasmBottom.transform.position = new Vector3(0f, -8f, 5f);
        chasmBottom.transform.localScale = new Vector3(30f, 0.5f, 16f);
        chasmBottom.GetComponent<Renderer>().material = ProceduralProps.GetGlowingMaterial(new Color(0.2f, 0.05f, 0.3f), "MistMat");
        chasmBottom.transform.SetParent(envParent.transform);

        // Inner Sanctum Ground (across chasm)
        GameObject innerSanctum = GameObject.CreatePrimitive(PrimitiveType.Cube);
        innerSanctum.name = "InnerSanctumGround";
        innerSanctum.transform.position = new Vector3(0f, -0.5f, 19f);
        innerSanctum.transform.localScale = new Vector3(22f, 1f, 14f);
        innerSanctum.GetComponent<Renderer>().material = ProceduralProps.GetDarkStoneMaterial();
        innerSanctum.transform.SetParent(envParent.transform);

        // 2. Solid Impassable Dungeon Walls (Enclose all rooms so player cannot glitch through)
        // Entrance South Wall
        ProceduralProps.CreateStoneWall(new Vector3(0f, 2.5f, -16.5f), new Vector3(19f, 5f, 1f), Quaternion.identity).transform.SetParent(envParent.transform);
        // Entrance West & East Walls
        ProceduralProps.CreateStoneWall(new Vector3(-9.5f, 2.5f, -9f), new Vector3(1f, 5f, 14f), Quaternion.identity).transform.SetParent(envParent.transform);
        ProceduralProps.CreateStoneWall(new Vector3(9.5f, 2.5f, -9f), new Vector3(1f, 5f, 14f), Quaternion.identity).transform.SetParent(envParent.transform);
        // Chasm Perimeter Walls
        ProceduralProps.CreateStoneWall(new Vector3(-12f, 2.5f, 5f), new Vector3(1f, 6f, 16f), Quaternion.identity).transform.SetParent(envParent.transform);
        ProceduralProps.CreateStoneWall(new Vector3(12f, 2.5f, 5f), new Vector3(1f, 6f, 16f), Quaternion.identity).transform.SetParent(envParent.transform);
        // Inner Sanctum North Wall
        ProceduralProps.CreateStoneWall(new Vector3(0f, 2.5f, 26.5f), new Vector3(23f, 5f, 1f), Quaternion.identity).transform.SetParent(envParent.transform);
        // Inner Sanctum West & East Walls
        ProceduralProps.CreateStoneWall(new Vector3(-11.5f, 2.5f, 19f), new Vector3(1f, 5f, 14f), Quaternion.identity).transform.SetParent(envParent.transform);
        ProceduralProps.CreateStoneWall(new Vector3(11.5f, 2.5f, 19f), new Vector3(1f, 5f, 14f), Quaternion.identity).transform.SetParent(envParent.transform);

        // 3. Moving Stone Platforms Bridging the Chasm
        ProceduralProps.CreateMovingPlatform(new Vector3(-4f, 0.2f, 0f), new Vector3(0f, 0f, 10f), 2.8f).transform.SetParent(envParent.transform);
        ProceduralProps.CreateMovingPlatform(new Vector3(4f, 0.2f, 10f), new Vector3(0f, 0f, -10f), 2.8f).transform.SetParent(envParent.transform);

        // 4. Solid Ruin Pillars & Flaming Wall Torches
        float[] pillarZ = new float[] { -14f, -9f, -4f, 14f, 19f, 24f };
        foreach (float pz in pillarZ)
        {
            ProceduralProps.CreateRuinPillar(new Vector3(-8f, 0f, pz), 5.5f, 0.5f).transform.SetParent(envParent.transform);
            ProceduralProps.CreateRuinPillar(new Vector3(8f, 0f, pz), 5.5f, 0.5f).transform.SetParent(envParent.transform);
            ProceduralProps.CreateWallTorch(new Vector3(-8.8f, 2.5f, pz), Quaternion.Euler(0f, 90f, 0f)).transform.SetParent(envParent.transform);
            ProceduralProps.CreateWallTorch(new Vector3(8.8f, 2.5f, pz), Quaternion.Euler(0f, -90f, 0f)).transform.SetParent(envParent.transform);
        }

        // 5. Active Spike Traps (Solid when extended + damage + knockback!)
        ProceduralProps.CreateSpikeTrap(new Vector3(-2.5f, 0f, -8f), 0f).transform.SetParent(envParent.transform);
        ProceduralProps.CreateSpikeTrap(new Vector3(2.5f, 0f, -6f), 0.8f).transform.SetParent(envParent.transform);
        ProceduralProps.CreateSpikeTrap(new Vector3(0f, 0f, -3f), 1.6f).transform.SetParent(envParent.transform);
        ProceduralProps.CreateSpikeTrap(new Vector3(0f, 0f, 15f), 0.4f).transform.SetParent(envParent.transform);

        // 6. Clue #2: Tablet of the Ancients
        ProceduralProps.CreateClueScroll(
            new Vector3(0f, 0f, -12f),
            "Tablet of the Ancients",
            "Greetings Traveler.\n\nThe gates to the Pirate King's Treasury are locked by the Ancient Ones. To forge the Silver Key of Passage, you must awaken the three Primordial Altars:\n\n1. Ruby Fire Altar (West Chamber)\n2. Emerald Earth Altar (East Chamber)\n3. Sapphire Water Altar (North Sanctum across the chasm)\n\nOBJECTIVE:\nCollect 10 Temple Treasures & Gems, and awaken the 3 Altars.\nOnce complete, the temple gate will unlock automatically to the Vault!\n\nBeware the spike traps and avoid falling into the misty void!",
            "Inspect"
        ).transform.SetParent(envParent.transform);

        // 7. The Three Elemental Altars
        GameObject fireAltar = ProceduralProps.CreateElementalAltar(new Vector3(-6f, 0f, -4f), ElementalAltar.ElementType.Fire, "Ruby Fire Altar", Color.red);
        GameObject earthAltar = ProceduralProps.CreateElementalAltar(new Vector3(6f, 0f, -4f), ElementalAltar.ElementType.Earth, "Emerald Earth Altar", Color.green);
        GameObject waterAltar = ProceduralProps.CreateElementalAltar(new Vector3(0f, 0f, 22f), ElementalAltar.ElementType.Water, "Sapphire Water Altar", Color.cyan);
        fireAltar.transform.SetParent(envParent.transform);
        earthAltar.transform.SetParent(envParent.transform);
        waterAltar.transform.SetParent(envParent.transform);

        // 8. Central Dais with Silver Key
        GameObject centralDais = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        centralDais.name = "CentralPuzzleDais";
        centralDais.transform.position = new Vector3(0f, 0.35f, 15f);
        centralDais.transform.localScale = new Vector3(3f, 0.35f, 3f);
        centralDais.GetComponent<Renderer>().material = ProceduralProps.GetStoneMaterial();
        centralDais.transform.SetParent(envParent.transform);

        GameObject silverKey = ProceduralProps.CreateKeyPickup(
            new Vector3(0f, 1.4f, 15f),
            KeyPickup.KeyType.Silver,
            "Silver Key of Passage",
            false,
            false
        );
        silverKey.transform.SetParent(envParent.transform);

        // Puzzle Controller Link
        GameObject puzzleObj = new GameObject("TemplePuzzleController");
        TemplePuzzleController tpc = puzzleObj.AddComponent<TemplePuzzleController>();
        tpc.altars = new List<ElementalAltar>
        {
            fireAltar.GetComponent<ElementalAltar>(),
            earthAltar.GetComponent<ElementalAltar>(),
            waterAltar.GetComponent<ElementalAltar>()
        };
        tpc.silverKeyObject = silverKey;
        puzzleObj.transform.SetParent(envParent.transform);

        // 9. Grand Temple Portal (leads to Scene 3)
        ProceduralProps.CreatePortalArchway(
            new Vector3(0f, 0f, 25f),
            Quaternion.Euler(0f, 0f, 0f),
            "Scene3_TreasureVault",
            "Grand Temple Portal",
            reqKey: true,
            keyType: KeyPickup.KeyType.Silver,
            portalColor: new Color(0.8f, 0.2f, 1f)
        ).transform.SetParent(envParent.transform);

        // 10. Secret Chest in East Chamber
        ProceduralProps.CreateTreasureChest(new Vector3(8f, 0f, 18f), Quaternion.Euler(0f, -90f, 0f), false, KeyPickup.KeyType.Bronze, 5, false).transform.SetParent(envParent.transform);

        // 11. Exactly 15 Temple Treasures & Gems:
        // 12 Standard Coins
        Vector3[] templeCoins = new Vector3[] {
            new Vector3(-4f, 0.6f, -11f),
            new Vector3(4f, 0.6f, -11f),
            new Vector3(-6f, 0.6f, -7f),
            new Vector3(6f, 0.6f, -7f),
            new Vector3(-2f, 0.6f, -4f),
            new Vector3(2f, 0.6f, -4f),
            new Vector3(-4f, 0.6f, 17f),
            new Vector3(4f, 0.6f, 17f),
            new Vector3(-6f, 0.6f, 21f),
            new Vector3(6f, 0.6f, 21f),
            new Vector3(-2f, 0.6f, 24f),
            new Vector3(2f, 0.6f, 24f)
        };
        foreach (var c in templeCoins)
        {
            ProceduralProps.CreateCollectibleCoin(c, CollectibleCoin.CoinType.StandardCoin, 1).transform.SetParent(envParent.transform);
        }

        // 3 Elemental Gems (Ruby, Emerald, Sapphire) = 15 total treasures!
        ProceduralProps.CreateCollectibleCoin(new Vector3(-6f, 1.2f, -1f), CollectibleCoin.CoinType.RareGem, 1).transform.SetParent(envParent.transform);
        ProceduralProps.CreateCollectibleCoin(new Vector3(6f, 1.2f, -1f), CollectibleCoin.CoinType.RareGem, 1).transform.SetParent(envParent.transform);
        ProceduralProps.CreateCollectibleCoin(new Vector3(0f, 1.2f, 18f), CollectibleCoin.CoinType.RareGem, 1).transform.SetParent(envParent.transform);
    }

    // ==========================================
    // BUILD LEVEL 3: THE PIRATE KING'S VAULT
    // ==========================================
    private void BuildTreasureVault()
    {
        GameObject envParent = new GameObject("TreasureVaultEnvironment");

        // Vault Floor
        GameObject vaultFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        vaultFloor.name = "VaultGround";
        vaultFloor.transform.position = new Vector3(0f, -0.5f, 0f);
        vaultFloor.transform.localScale = new Vector3(28f, 1f, 28f);
        vaultFloor.GetComponent<Renderer>().material = ProceduralProps.GetDarkStoneMaterial();
        vaultFloor.transform.SetParent(envParent.transform);

        // Solid Vault Perimeter Stone Walls
        ProceduralProps.CreateStoneWall(new Vector3(0f, 3f, 14.5f), new Vector3(29f, 6f, 1f), Quaternion.identity).transform.SetParent(envParent.transform);
        ProceduralProps.CreateStoneWall(new Vector3(0f, 3f, -14.5f), new Vector3(29f, 6f, 1f), Quaternion.identity).transform.SetParent(envParent.transform);
        ProceduralProps.CreateStoneWall(new Vector3(-14.5f, 3f, 0f), new Vector3(1f, 6f, 29f), Quaternion.identity).transform.SetParent(envParent.transform);
        ProceduralProps.CreateStoneWall(new Vector3(14.5f, 3f, 0f), new Vector3(1f, 6f, 29f), Quaternion.identity).transform.SetParent(envParent.transform);

        // 8 Gilded Columns
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * 11f, 0f, Mathf.Sin(angle) * 11f);
            GameObject pillar = ProceduralProps.CreateRuinPillar(pos, 6f, 0.6f);
            pillar.GetComponent<Renderer>().material = ProceduralProps.GetGoldMaterial();
            pillar.transform.SetParent(envParent.transform);

            ProceduralProps.CreateWallTorch(pos + new Vector3(0f, 3.5f, 0f), Quaternion.Euler(0f, -i * 45f, 0f)).transform.SetParent(envParent.transform);
        }

        // Stepped Central Treasure Dais
        GameObject step1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        step1.name = "DaisStep1";
        step1.transform.position = new Vector3(0f, 0.25f, 0f);
        step1.transform.localScale = new Vector3(8f, 0.25f, 8f);
        step1.GetComponent<Renderer>().material = ProceduralProps.GetStoneMaterial();
        step1.transform.SetParent(envParent.transform);

        GameObject step2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        step2.name = "DaisStep2";
        step2.transform.position = new Vector3(0f, 0.65f, 0f);
        step2.transform.localScale = new Vector3(5.5f, 0.25f, 5.5f);
        step2.GetComponent<Renderer>().material = ProceduralProps.GetGoldMaterial();
        step2.transform.SetParent(envParent.transform);

        GameObject daisTop = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        daisTop.name = "DaisTop";
        daisTop.transform.position = new Vector3(0f, 1.0f, 0f);
        daisTop.transform.localScale = new Vector3(3.5f, 0.25f, 3.5f);
        daisTop.GetComponent<Renderer>().material = ProceduralProps.GetDarkStoneMaterial();
        daisTop.transform.SetParent(envParent.transform);

        // The Grand Legendary Treasure Chest
        ProceduralProps.CreateTreasureChest(
            new Vector3(0f, 1.25f, 0f),
            Quaternion.Euler(0f, 180f, 0f),
            requiresKey: false,
            keyType: KeyPickup.KeyType.Gold,
            coins: 50,
            isGrand: true
        ).transform.SetParent(envParent.transform);

        // Vault Monument Inscription
        ProceduralProps.CreateClueScroll(
            new Vector3(0f, 0f, -8f),
            "Vault Monument of the Pirate King",
            "\"Let it be known to all who gaze upon this bounty:\n\nYou have braved the tides of the Whispering Cove, unraveled the secrets of the ancient map, and mastered the trials of the Elemental Temple.\n\nThe Pirate King's eternal treasure is rightfully yours!\n\nApproach the grand altar, open the chest of legends, and claim your victory!\"",
            "Read Inscription"
        ).transform.SetParent(envParent.transform);

        // Giant Crystal Clusters
        Vector3[] crystalPositions = new Vector3[] {
            new Vector3(-8f, 0f, -6f), new Vector3(8f, 0f, -6f),
            new Vector3(-8f, 0f, 6f), new Vector3(8f, 0f, 6f)
        };
        foreach (var cpos in crystalPositions)
        {
            GameObject crystal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            crystal.name = "AmethystCluster";
            crystal.transform.position = cpos + Vector3.up * 1f;
            crystal.transform.localScale = new Vector3(0.9f, 2f, 0.9f);
            crystal.GetComponent<Renderer>().material = ProceduralProps.GetGlowingMaterial(new Color(0.7f, 0.2f, 1f), "CrystalAmethyst");
            crystal.transform.SetParent(envParent.transform);
        }

        // Piles of Coins & Gems
        for (int i = 0; i < 20; i++)
        {
            float ang = i * (360f / 20f) * Mathf.Deg2Rad;
            float r = Random.Range(3f, 7f);
            Vector3 pos = new Vector3(Mathf.Cos(ang) * r, 0.6f, Mathf.Sin(ang) * r);
            ProceduralProps.CreateCollectibleCoin(pos, CollectibleCoin.CoinType.StandardCoin, 1).transform.SetParent(envParent.transform);
        }

        for (int i = 0; i < 4; i++)
        {
            float ang = (i * 90f + 45f) * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(ang) * 4.5f, 1f, Mathf.Sin(ang) * 4.5f);
            ProceduralProps.CreateCollectibleCoin(pos, CollectibleCoin.CoinType.RareGem, 5).transform.SetParent(envParent.transform);
        }

        // Return Portal
        ProceduralProps.CreatePortalArchway(
            new Vector3(0f, 0f, 12f),
            Quaternion.Euler(0f, 180f, 0f),
            "Scene1_IslandCove",
            "Return Portal (Restart Journey)",
            reqKey: false,
            keyType: KeyPickup.KeyType.Bronze,
            portalColor: new Color(1f, 0.84f, 0f)
        ).transform.SetParent(envParent.transform);
    }
}
