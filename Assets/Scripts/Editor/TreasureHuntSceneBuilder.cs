using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TreasureHuntSceneBuilder
{
    private const string CharacterPrefabPath = "Assets/Models/RPG Tiny Hero Duo/Prefab/MaleCharacterPBR.prefab";

    [MenuItem("Treasure Hunt/Build All 3 Scenes")]
    public static void BuildAllScenes()
    {
        Debug.Log("<color=yellow>[Treasure Hunt]</color> Starting Automated 3-Scene Generation...");

        BuildScene1();
        BuildScene2();
        BuildScene3();

        UpdateBuildSettings();

        // Also update MainScene to mirror Scene 1 for immediate play
        BuildMainScene();

        Debug.Log("<color=green>[Treasure Hunt] All 3 Scenes & MainScene Built Successfully!</color>");
        EditorUtility.DisplayDialog("Treasure Hunt Complete!", "All 3 scenes (Island Cove, Ancient Temple, Treasure Vault) have been built and added to Build Settings with 0 errors!", "Awesome!");
    }

    public static void BuildAllScenesBatchMode()
    {
        Debug.Log("[Treasure Hunt Batch] Generating scenes...");
        BuildScene1();
        BuildScene2();
        BuildScene3();
        UpdateBuildSettings();
        BuildMainScene();
        Debug.Log("[Treasure Hunt Batch] Finished successfully.");
    }

    // ==========================================
    // SCENE 1: THE TROPICAL COVE
    // ==========================================
    private static void BuildScene1()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "Scene1_IslandCove";

        // 1. Lighting & Environment
        SetupLighting(new Color(1f, 0.95f, 0.85f), 1.2f, new Vector3(50f, -30f, 0f), new Color(0.4f, 0.5f, 0.65f), false);

        // 2. Core Managers
        SetupManagers();

        // 3. Player & Camera
        Vector3 playerSpawn = new Vector3(0f, 0.5f, -8f);
        GameObject player = CreatePlayer(playerSpawn);
        CreateCamera(player.transform, playerSpawn);

        // 4. Island Environment
        // Central Sand Island
        GameObject island = GameObject.CreatePrimitive(PrimitiveType.Cube);
        island.name = "IslandGround";
        island.transform.position = new Vector3(0f, -0.5f, 5f);
        island.transform.localScale = new Vector3(32f, 1f, 36f);
        island.GetComponent<Renderer>().material = ProceduralProps.GetSandMaterial();

        // Solid Cliff Wall Obstacles around Island Perimeter
        ProceduralProps.CreateCliffWall(new Vector3(0f, 4f, 23f), new Vector3(34f, 8f, 3f), Quaternion.identity);
        ProceduralProps.CreateCliffWall(new Vector3(-17f, 3f, 12f), new Vector3(3f, 6f, 24f), Quaternion.identity);
        ProceduralProps.CreateCliffWall(new Vector3(17f, 3f, 12f), new Vector3(3f, 6f, 24f), Quaternion.identity);

        // Solid Coastal Boulders
        ProceduralProps.CreateRockObstacle(new Vector3(-6f, 1f, -9f), new Vector3(2.5f, 2f, 2.5f), Quaternion.Euler(10f, 45f, 5f));
        ProceduralProps.CreateRockObstacle(new Vector3(5f, 1.2f, -8f), new Vector3(3f, 2.4f, 2f), Quaternion.Euler(0f, 30f, 0f));
        ProceduralProps.CreateRockObstacle(new Vector3(-13f, 1.5f, 3f), new Vector3(3.5f, 3f, 3f), Quaternion.Euler(5f, 60f, 10f));
        ProceduralProps.CreateRockObstacle(new Vector3(14f, 1.5f, 4f), new Vector3(3f, 2.8f, 3.2f), Quaternion.Euler(0f, 75f, 0f));

        // Pirate Campfire Site at Beach Landing
        ProceduralProps.CreateCampfire(new Vector3(0f, 0f, -6f));

        // Water Plane around island
        ProceduralProps.CreateWaterPlane(new Vector3(0f, -0.4f, 5f), new Vector2(80f, 80f));

        // Ruined Watchtower (stepping stone platforms to Map Fragment #1)
        GameObject towerBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
        towerBase.name = "TowerBase";
        towerBase.transform.position = new Vector3(-8f, 0.5f, 10f);
        towerBase.transform.localScale = new Vector3(6f, 1f, 6f);
        towerBase.GetComponent<Renderer>().material = ProceduralProps.GetStoneMaterial();

        GameObject towerStep1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        towerStep1.name = "TowerStep1";
        towerStep1.transform.position = new Vector3(-8f, 1.5f, 10f);
        towerStep1.transform.localScale = new Vector3(4.5f, 1f, 4.5f);
        towerStep1.GetComponent<Renderer>().material = ProceduralProps.GetDarkStoneMaterial();

        GameObject towerStep2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        towerStep2.name = "TowerTop";
        towerStep2.transform.position = new Vector3(-8f, 2.5f, 10f);
        towerStep2.transform.localScale = new Vector3(3f, 1f, 3f);
        towerStep2.GetComponent<Renderer>().material = ProceduralProps.GetStoneMaterial();

        // Ruin Pillars around watchtower
        ProceduralProps.CreateRuinPillar(new Vector3(-10.5f, 0f, 7.5f), 4f);
        ProceduralProps.CreateRuinPillar(new Vector3(-5.5f, 0f, 7.5f), 3.5f);
        ProceduralProps.CreateRuinPillar(new Vector3(-10.5f, 0f, 12.5f), 4.5f);
        ProceduralProps.CreateRuinPillar(new Vector3(-5.5f, 0f, 12.5f), 3f);

        // Palm Trees
        ProceduralProps.CreatePalmTree(new Vector3(10f, 0f, -5f), 5.5f);
        ProceduralProps.CreatePalmTree(new Vector3(-11f, 0f, -6f), 5f);
        ProceduralProps.CreatePalmTree(new Vector3(12f, 0f, 8f), 6f);
        ProceduralProps.CreatePalmTree(new Vector3(8f, 0f, 18f), 5.2f);
        ProceduralProps.CreatePalmTree(new Vector3(-12f, 0f, 18f), 4.8f);

        // Tidal Stepping Stones across water to Offshore Islet
        for (int i = 0; i < 4; i++)
        {
            GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stone.name = $"TidalStone_{i}";
            stone.transform.position = new Vector3(12f + i * 2.8f, -0.1f, -2f + Mathf.Sin(i) * 1.5f);
            stone.transform.localScale = new Vector3(1.8f, 0.4f, 1.8f);
            stone.GetComponent<Renderer>().material = ProceduralProps.GetStoneMaterial();
        }

        // Offshore Islet
        GameObject islet = GameObject.CreatePrimitive(PrimitiveType.Cube);
        islet.name = "OffshoreIslet";
        islet.transform.position = new Vector3(26f, -0.2f, -2f);
        islet.transform.localScale = new Vector3(7f, 0.8f, 7f);
        islet.GetComponent<Renderer>().material = ProceduralProps.GetSandMaterial();
        ProceduralProps.CreatePalmTree(new Vector3(27f, 0f, -1f), 4.5f);

        // Moving Tidal Platform to offshore cache
        ProceduralProps.CreateMovingPlatform(new Vector3(-4f, 0.3f, 21f), new Vector3(0f, 0f, 6f), 2.2f);

        GameObject northIslet = GameObject.CreatePrimitive(PrimitiveType.Cube);
        northIslet.name = "NorthCacheIslet";
        northIslet.transform.position = new Vector3(-4f, 0.2f, 29f);
        northIslet.transform.localScale = new Vector3(5f, 0.8f, 5f);
        northIslet.GetComponent<Renderer>().material = ProceduralProps.GetStoneMaterial();

        // 5. Quests & Interactables
        // Clue #1: Captain's Logbook
        ProceduralProps.CreateClueScroll(
            new Vector3(0f, 0f, -4.5f),
            "Captain's Logbook",
            "Ahoy brave explorer!\n\nCaptain Redbeard hid the key to the Forbidden Temple across this cove.\n\nOBJECTIVE:\n1. Collect 10 Gold Coins scattered across the cove.\n2. Find the 3 hidden Map Fragments (Watchtower, Offshore Islet, North Cache).\n\nWhen 10 coins are gathered, your ship will set sail automatically to the Forbidden Temple!\n\nBeware the deep ocean water — falling into the sea will cost you a life!",
            "Read"
        );

        // Map Fragment #1 (Top of Watchtower)
        ProceduralProps.CreateCollectibleCoin(new Vector3(-8f, 3.8f, 10f), CollectibleCoin.CoinType.MapFragment);

        // Map Fragment #2 (Offshore Islet)
        ProceduralProps.CreateCollectibleCoin(new Vector3(26f, 0.8f, -2f), CollectibleCoin.CoinType.MapFragment);

        // Map Fragment #3 (North Cache Islet across moving platform)
        ProceduralProps.CreateCollectibleCoin(new Vector3(-4f, 1.2f, 29f), CollectibleCoin.CoinType.MapFragment);

        // Sea Shrine with Bronze Skeleton Key (Appears after map is united)
        GameObject shrineBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        shrineBase.name = "SeaShrineBase";
        shrineBase.transform.position = new Vector3(8f, 0.4f, 6f);
        shrineBase.transform.localScale = new Vector3(2.5f, 0.4f, 2.5f);
        shrineBase.GetComponent<Renderer>().material = ProceduralProps.GetDarkStoneMaterial();

        ProceduralProps.CreateKeyPickup(
            new Vector3(8f, 1.4f, 6f),
            KeyPickup.KeyType.Bronze,
            "Bronze Skeleton Key",
            true,
            true
        );

        // Bonus Chest in Palm Grove
        ProceduralProps.CreateTreasureChest(new Vector3(12f, 0f, 12f), Quaternion.Euler(0f, -45f, 0f), false, KeyPickup.KeyType.Bronze, 15, false);

        // Ancient Sea Gate (Portal to Scene 2)
        ProceduralProps.CreatePortalArchway(
            new Vector3(0f, 0f, 18f),
            Quaternion.Euler(0f, 0f, 0f),
            "Scene2_AncientTemple",
            "Ancient Sea Gate",
            reqKey: true,
            keyType: KeyPickup.KeyType.Bronze,
            portalColor: new Color(0.2f, 0.8f, 1f)
        );

        // Exactly 15 Gold Coins across the 5 cove zones!
        Vector3[] coinPositions = new Vector3[]
        {
            // Zone 1: Campfire Beach Landing (3 coins)
            new Vector3(0f, 0.6f, -2.5f),
            new Vector3(-2.5f, 0.6f, -3.5f),
            new Vector3(2.5f, 0.6f, -3.5f),
            // Zone 2: Ancient Watchtower Steps & Summit (3 coins)
            new Vector3(-8f, 1.6f, 10f),
            new Vector3(-8f, 2.6f, 10f),
            new Vector3(-8f, 3.6f, 10f),
            // Zone 3: Tidal Stepping Stones path (3 coins)
            new Vector3(12f, 0.6f, -2f),
            new Vector3(15f, 0.6f, -1f),
            new Vector3(18f, 0.6f, -2f),
            // Zone 4: Offshore Islet & Palm Grove (3 coins)
            new Vector3(24f, 0.6f, -2f),
            new Vector3(12f, 0.6f, 10f),
            new Vector3(10f, 0.6f, 14f),
            // Zone 5: North Causeway leading to Sea Gate (3 coins)
            new Vector3(0f, 0.6f, 6f),
            new Vector3(0f, 0.6f, 11f),
            new Vector3(0f, 0.6f, 15f)
        };

        foreach (var pos in coinPositions)
        {
            ProceduralProps.CreateCollectibleCoin(pos, CollectibleCoin.CoinType.StandardCoin, 1);
        }

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Scene1_IslandCove.unity");
        Debug.Log("[Treasure Hunt] Scene 1 (Island Cove) built!");
    }

    // ==========================================
    // SCENE 2: THE ANCIENT TEMPLE
    // ==========================================
    private static void BuildScene2()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "Scene2_AncientTemple";

        // 1. Lighting & Moody Atmosphere
        SetupLighting(new Color(0.9f, 0.7f, 0.5f), 0.6f, new Vector3(45f, 60f, 0f), new Color(0.12f, 0.12f, 0.18f), true);

        // 2. Core Managers
        SetupManagers();

        // 3. Player & Camera
        Vector3 playerSpawn = new Vector3(0f, 0.5f, -14f);
        GameObject player = CreatePlayer(playerSpawn);
        CreateCamera(player.transform, playerSpawn);

        // 4. Temple Architecture
        // Entrance Hall
        GameObject enterHall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        enterHall.name = "EntranceHallGround";
        enterHall.transform.position = new Vector3(0f, -0.5f, -9f);
        enterHall.transform.localScale = new Vector3(18f, 1f, 14f);
        enterHall.GetComponent<Renderer>().material = ProceduralProps.GetDarkStoneMaterial();

        // Chasm Void
        GameObject chasmBottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
        chasmBottom.name = "ChasmMist";
        chasmBottom.transform.position = new Vector3(0f, -8f, 5f);
        chasmBottom.transform.localScale = new Vector3(30f, 0.5f, 16f);
        var cr = chasmBottom.GetComponent<Renderer>();
        cr.material = ProceduralProps.GetGlowingMaterial(new Color(0.2f, 0.05f, 0.3f), "MistMat");

        // Inner Sanctum Ground (across chasm)
        GameObject innerSanctum = GameObject.CreatePrimitive(PrimitiveType.Cube);
        innerSanctum.name = "InnerSanctumGround";
        innerSanctum.transform.position = new Vector3(0f, -0.5f, 19f);
        innerSanctum.transform.localScale = new Vector3(22f, 1f, 14f);
        innerSanctum.GetComponent<Renderer>().material = ProceduralProps.GetDarkStoneMaterial();

        // Solid Temple Perimeter Stone Walls
        ProceduralProps.CreateStoneWall(new Vector3(-9.5f, 3f, -9f), new Vector3(1f, 6f, 14f), Quaternion.identity);
        ProceduralProps.CreateStoneWall(new Vector3(9.5f, 3f, -9f), new Vector3(1f, 6f, 14f), Quaternion.identity);
        ProceduralProps.CreateStoneWall(new Vector3(0f, 3f, -16.5f), new Vector3(20f, 6f, 1f), Quaternion.identity);

        ProceduralProps.CreateStoneWall(new Vector3(-11.5f, 3f, 19f), new Vector3(1f, 6f, 14f), Quaternion.identity);
        ProceduralProps.CreateStoneWall(new Vector3(11.5f, 3f, 19f), new Vector3(1f, 6f, 14f), Quaternion.identity);
        ProceduralProps.CreateStoneWall(new Vector3(0f, 3f, 26.5f), new Vector3(24f, 6f, 1f), Quaternion.identity);

        ProceduralProps.CreateStoneWall(new Vector3(-13.5f, 2f, 5f), new Vector3(1f, 4f, 16f), Quaternion.identity);
        ProceduralProps.CreateStoneWall(new Vector3(13.5f, 2f, 5f), new Vector3(1f, 4f, 16f), Quaternion.identity);

        // Two Moving Stone Platforms bridging the chasm
        ProceduralProps.CreateMovingPlatform(new Vector3(-4f, 0.2f, 0f), new Vector3(0f, 0f, 10f), 2.8f);
        ProceduralProps.CreateMovingPlatform(new Vector3(4f, 0.2f, 10f), new Vector3(0f, 0f, -10f), 2.8f);

        // Pillars lining the halls
        float[] pillarZ = new float[] { -14f, -9f, -4f, 14f, 19f, 24f };
        foreach (float pz in pillarZ)
        {
            ProceduralProps.CreateRuinPillar(new Vector3(-8f, 0f, pz), 5.5f, 0.5f);
            ProceduralProps.CreateRuinPillar(new Vector3(8f, 0f, pz), 5.5f, 0.5f);
            ProceduralProps.CreateWallTorch(new Vector3(-7.8f, 2.5f, pz), Quaternion.Euler(0f, 90f, 0f));
            ProceduralProps.CreateWallTorch(new Vector3(7.8f, 2.5f, pz), Quaternion.Euler(0f, -90f, 0f));
        }

        // Spike Traps in entrance corridors
        ProceduralProps.CreateSpikeTrap(new Vector3(-2.5f, 0f, -8f), 0f);
        ProceduralProps.CreateSpikeTrap(new Vector3(2.5f, 0f, -6f), 0.8f);
        ProceduralProps.CreateSpikeTrap(new Vector3(0f, 0f, -3f), 1.6f);
        ProceduralProps.CreateSpikeTrap(new Vector3(0f, 0f, 15f), 0.4f);

        // 5. The Three Elemental Altars (Puzzle)
        // Clue #2: Tablet of the Ancients
        ProceduralProps.CreateClueScroll(
            new Vector3(0f, 0f, -12f),
            "Tablet of the Ancients",
            "Greetings Traveler.\n\nThe gates to the Pirate King's Treasury are locked by the Ancient Ones. To forge the Silver Key of Passage, you must awaken the three Primordial Altars:\n\n1. Ruby Fire Altar (West Chamber)\n2. Emerald Earth Altar (East Chamber)\n3. Sapphire Water Altar (North Sanctum across the chasm)\n\nTread lightly over the spike traps and traverse the floating stones!",
            "Inspect"
        );

        GameObject fireAltar = ProceduralProps.CreateElementalAltar(new Vector3(-6f, 0f, -4f), ElementalAltar.ElementType.Fire, "Ruby Fire Altar", Color.red);
        GameObject earthAltar = ProceduralProps.CreateElementalAltar(new Vector3(6f, 0f, -4f), ElementalAltar.ElementType.Earth, "Emerald Earth Altar", Color.green);
        GameObject waterAltar = ProceduralProps.CreateElementalAltar(new Vector3(0f, 0f, 22f), ElementalAltar.ElementType.Water, "Sapphire Water Altar", Color.cyan);

        // Central Altar / Dais where Silver Key manifests
        GameObject centralDais = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        centralDais.name = "CentralPuzzleDais";
        centralDais.transform.position = new Vector3(0f, 0.35f, 15f);
        centralDais.transform.localScale = new Vector3(3f, 0.35f, 3f);
        centralDais.GetComponent<Renderer>().material = ProceduralProps.GetStoneMaterial();

        // Silver Key Object (starts inactive until altars lit)
        GameObject silverKey = ProceduralProps.CreateKeyPickup(
            new Vector3(0f, 1.4f, 15f),
            KeyPickup.KeyType.Silver,
            "Silver Key of Passage",
            false,
            false
        );

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

        // Grand Temple Portal (leads to Scene 3)
        ProceduralProps.CreatePortalArchway(
            new Vector3(0f, 0f, 25f),
            Quaternion.Euler(0f, 0f, 0f),
            "Scene3_TreasureVault",
            "Grand Temple Portal",
            reqKey: true,
            keyType: KeyPickup.KeyType.Silver,
            portalColor: new Color(0.8f, 0.2f, 1f)
        );

        // Secret Chest in East Wing
        ProceduralProps.CreateTreasureChest(new Vector3(8f, 0f, 18f), Quaternion.Euler(0f, -90f, 0f), false, KeyPickup.KeyType.Bronze, 20, false);

        // Coins & Gems
        Vector3[] templeCoins = new Vector3[]
        {
            new Vector3(-4f, 0.6f, -11f),
            new Vector3(4f, 0.6f, -11f),
            new Vector3(-6f, 0.6f, -7f),
            new Vector3(6f, 0.6f, -7f),
            new Vector3(-1.5f, 0.6f, -1f),
            new Vector3(1.5f, 0.6f, -1f),
            new Vector3(-4f, 0.6f, 17f),
            new Vector3(4f, 0.6f, 17f),
            new Vector3(-6f, 0.6f, 21f),
            new Vector3(6f, 0.6f, 21f),
            new Vector3(-2f, 0.6f, 24f),
            new Vector3(2f, 0.6f, 24f)
        };

        foreach (var pos in templeCoins)
        {
            ProceduralProps.CreateCollectibleCoin(pos, CollectibleCoin.CoinType.StandardCoin, 1);
        }

        // Rare Gems
        ProceduralProps.CreateCollectibleCoin(new Vector3(-6f, 1.2f, -1f), CollectibleCoin.CoinType.RareGem, 5);
        ProceduralProps.CreateCollectibleCoin(new Vector3(6f, 1.2f, -1f), CollectibleCoin.CoinType.RareGem, 5);
        ProceduralProps.CreateCollectibleCoin(new Vector3(0f, 1.2f, 18f), CollectibleCoin.CoinType.RareGem, 5);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Scene2_AncientTemple.unity");
        Debug.Log("[Treasure Hunt] Scene 2 (Ancient Temple) built!");
    }

    // ==========================================
    // SCENE 3: THE PIRATE KING'S VAULT
    // ==========================================
    private static void BuildScene3()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "Scene3_TreasureVault";

        // 1. Radiant Royal Lighting
        SetupLighting(new Color(1f, 0.85f, 0.4f), 1.4f, new Vector3(55f, -20f, 0f), new Color(0.2f, 0.15f, 0.1f), false);

        // 2. Core Managers
        SetupManagers();

        // 3. Player & Camera
        Vector3 playerSpawn = new Vector3(0f, 0.5f, -12f);
        GameObject player = CreatePlayer(playerSpawn);
        CreateCamera(player.transform, playerSpawn);

        // 4. Grand Octagonal Chamber
        GameObject vaultFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        vaultFloor.name = "VaultGround";
        vaultFloor.transform.position = new Vector3(0f, -0.5f, 0f);
        vaultFloor.transform.localScale = new Vector3(28f, 1f, 28f);
        vaultFloor.GetComponent<Renderer>().material = ProceduralProps.GetDarkStoneMaterial();

        // Solid Vault Perimeter Stone Walls
        ProceduralProps.CreateStoneWall(new Vector3(0f, 3f, 14.5f), new Vector3(29f, 6f, 1f), Quaternion.identity);
        ProceduralProps.CreateStoneWall(new Vector3(0f, 3f, -14.5f), new Vector3(29f, 6f, 1f), Quaternion.identity);
        ProceduralProps.CreateStoneWall(new Vector3(-14.5f, 3f, 0f), new Vector3(1f, 6f, 29f), Quaternion.identity);
        ProceduralProps.CreateStoneWall(new Vector3(14.5f, 3f, 0f), new Vector3(1f, 6f, 29f), Quaternion.identity);

        // Gold Trim Border
        for (int i = 0; i < 4; i++)
        {
            GameObject border = GameObject.CreatePrimitive(PrimitiveType.Cube);
            border.name = $"GoldTrim_{i}";
            border.transform.SetParent(vaultFloor.transform, false);
            border.transform.localPosition = new Vector3(i % 2 == 0 ? (i == 0 ? 0.49f : -0.49f) : 0f, 0.52f, i % 2 != 0 ? (i == 1 ? 0.49f : -0.49f) : 0f);
            border.transform.localScale = new Vector3(i % 2 == 0 ? 0.02f : 1f, 0.05f, i % 2 != 0 ? 0.02f : 1f);
            border.GetComponent<Renderer>().material = ProceduralProps.GetGoldMaterial();
            Object.DestroyImmediate(border.GetComponent<Collider>());
        }

        // 8 Gilded Columns
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * 11f, 0f, Mathf.Sin(angle) * 11f);
            GameObject pillar = ProceduralProps.CreateRuinPillar(pos, 6f, 0.6f);
            pillar.GetComponent<Renderer>().material = ProceduralProps.GetGoldMaterial();

            // Wall torch
            ProceduralProps.CreateWallTorch(pos + new Vector3(0f, 3.5f, 0f), Quaternion.Euler(0f, -i * 45f, 0f));
        }

        // Stepped Central Treasure Dais
        GameObject step1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        step1.name = "DaisStep1";
        step1.transform.position = new Vector3(0f, 0.25f, 0f);
        step1.transform.localScale = new Vector3(8f, 0.25f, 8f);
        step1.GetComponent<Renderer>().material = ProceduralProps.GetStoneMaterial();

        GameObject step2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        step2.name = "DaisStep2";
        step2.transform.position = new Vector3(0f, 0.65f, 0f);
        step2.transform.localScale = new Vector3(5.5f, 0.25f, 5.5f);
        step2.GetComponent<Renderer>().material = ProceduralProps.GetGoldMaterial();

        GameObject daisTop = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        daisTop.name = "DaisTop";
        daisTop.transform.position = new Vector3(0f, 1.0f, 0f);
        daisTop.transform.localScale = new Vector3(3.5f, 0.25f, 3.5f);
        daisTop.GetComponent<Renderer>().material = ProceduralProps.GetDarkStoneMaterial();

        // 5. The Grand Legendary Treasure Chest (CLIMAX!)
        ProceduralProps.CreateTreasureChest(
            new Vector3(0f, 1.25f, 0f),
            Quaternion.Euler(0f, 180f, 0f),
            requiresKey: false,
            keyType: KeyPickup.KeyType.Gold,
            coins: 50,
            isGrand: true
        );

        // Vault Monument / Lore Inscription
        ProceduralProps.CreateClueScroll(
            new Vector3(0f, 0f, -8f),
            "Vault Monument of the Pirate King",
            "\"Let it be known to all who gaze upon this bounty:\n\nYou have braved the tides of the Whispering Cove, unraveled the secrets of the ancient map, and mastered the trials of the Elemental Temple.\n\nThe Pirate King's eternal treasure is rightfully yours!\n\nApproach the grand altar, open the chest of legends, and claim your victory!\"",
            "Read Inscription"
        );

        // Giant Crystal Clusters around room
        Vector3[] crystalPositions = new Vector3[]
        {
            new Vector3(-8f, 0f, -6f),
            new Vector3(8f, 0f, -6f),
            new Vector3(-8f, 0f, 6f),
            new Vector3(8f, 0f, 6f)
        };

        foreach (var cpos in crystalPositions)
        {
            GameObject crystal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            crystal.name = "AmethystCluster";
            crystal.transform.position = cpos + Vector3.up * 1f;
            crystal.transform.localScale = new Vector3(0.9f, 2f, 0.9f);
            crystal.transform.rotation = Quaternion.Euler(Random.Range(-15f, 15f), Random.Range(0f, 360f), Random.Range(-15f, 15f));
            crystal.GetComponent<Renderer>().material = ProceduralProps.GetGlowingMaterial(new Color(0.7f, 0.2f, 1f), "CrystalAmethyst");

            GameObject cl = new GameObject("CrystalLight");
            cl.transform.SetParent(crystal.transform, false);
            var l = cl.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = new Color(0.7f, 0.2f, 1f);
            l.range = 5f;
            l.intensity = 2f;
        }

        // Piles of Bonus Treasure Coins & Gems
        for (int i = 0; i < 20; i++)
        {
            float ang = i * (360f / 20f) * Mathf.Deg2Rad;
            float r = Random.Range(2.5f, 7.5f);
            Vector3 pos = new Vector3(Mathf.Cos(ang) * r, 0.6f, Mathf.Sin(ang) * r);
            ProceduralProps.CreateCollectibleCoin(pos, CollectibleCoin.CoinType.StandardCoin, 1);
        }

        for (int i = 0; i < 4; i++)
        {
            float ang = (i * 90f + 45f) * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(ang) * 4.5f, 1f, Mathf.Sin(ang) * 4.5f);
            ProceduralProps.CreateCollectibleCoin(pos, CollectibleCoin.CoinType.RareGem, 5);
        }

        // Exit / Replay Portal in the corner
        ProceduralProps.CreatePortalArchway(
            new Vector3(0f, 0f, 12f),
            Quaternion.Euler(0f, 180f, 0f),
            "Scene1_IslandCove",
            "Return Portal (Restart Journey)",
            reqKey: false,
            keyType: KeyPickup.KeyType.Bronze,
            portalColor: new Color(1f, 0.84f, 0f)
        );

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Scene3_TreasureVault.unity");
        Debug.Log("[Treasure Hunt] Scene 3 (Treasure Vault) built!");
    }

    // ==========================================
    // UPDATE MAIN SCENE
    // ==========================================
    private static void BuildMainScene()
    {
        // Re-open Scene1_IslandCove and save as MainScene so user can click Play immediately in MainScene too!
        Scene scene1 = EditorSceneManager.OpenScene("Assets/Scenes/Scene1_IslandCove.unity", OpenSceneMode.Single);
        EditorSceneManager.SaveScene(scene1, "Assets/Scenes/MainScene.unity");
        Debug.Log("[Treasure Hunt] MainScene.unity updated to launch directly into Scene 1!");
    }

    // ==========================================
    // HELPERS
    // ==========================================
    private static void SetupLighting(Color lightColor, float intensity, Vector3 rotation, Color ambientColor, bool fog)
    {
        GameObject sun = new GameObject("DirectionalLight");
        Light l = sun.AddComponent<Light>();
        l.type = LightType.Directional;
        l.color = lightColor;
        l.intensity = intensity;
        l.shadows = LightShadows.Soft;
        sun.transform.rotation = Quaternion.Euler(rotation);

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = ambientColor;
        RenderSettings.fog = fog;
        if (fog)
        {
            RenderSettings.fogColor = new Color(0.12f, 0.12f, 0.18f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.02f;
        }
    }

    private static void SetupManagers()
    {
        // GameManager
        GameObject gmObj = new GameObject("GameManager");
        gmObj.AddComponent<GameManager>();

        // SoundManager
        GameObject smObj = new GameObject("SoundManager");
        smObj.AddComponent<SoundManager>();

        // GameHUD
        GameObject hudObj = new GameObject("GameHUD");
        hudObj.AddComponent<GameHUD>();
    }

    private static GameObject CreatePlayer(Vector3 position)
    {
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = position;

        CharacterController cc = player.AddComponent<CharacterController>();
        cc.center = new Vector3(0f, 0.9f, 0f);
        cc.height = 1.8f;
        cc.radius = 0.45f;
        cc.stepOffset = 0.35f;

        PlayerController pc = player.AddComponent<PlayerController>();

        // Instantiate RPG Hero Character Prefab if available
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CharacterPrefabPath);
        if (prefab != null)
        {
            GameObject charModel = (GameObject)PrefabUtility.InstantiatePrefab(prefab, player.transform);
            charModel.transform.localPosition = Vector3.zero;
            charModel.transform.localRotation = Quaternion.identity;
        }
        else
        {
            // Stylized procedural avatar fallback
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "FallbackBody";
            body.transform.SetParent(player.transform, false);
            body.transform.localPosition = new Vector3(0f, 0.9f, 0f);
            body.transform.localScale = new Vector3(0.8f, 0.9f, 0.8f);
            body.GetComponent<Renderer>().material = ProceduralProps.GetGlowingMaterial(Color.blue, "AvatarMat");
            Object.DestroyImmediate(body.GetComponent<Collider>());
        }

        return player;
    }

    private static void CreateCamera(Transform playerTarget, Vector3 playerPos)
    {
        GameObject camObj = new GameObject("MainCamera");
        camObj.tag = "MainCamera";
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        camObj.AddComponent<AudioListener>();

        ThirdPersonCamera tpc = camObj.AddComponent<ThirdPersonCamera>();
        tpc.target = playerTarget;
        tpc.defaultDistance = 5.0f;
        tpc.targetOffset = new Vector3(0f, 1.4f, 0f);

        camObj.transform.position = playerPos + new Vector3(0f, 2.5f, -5f);
        camObj.transform.LookAt(playerPos + Vector3.up * 1.4f);
    }

    private static void UpdateBuildSettings()
    {
        List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>
        {
            new EditorBuildSettingsScene("Assets/Scenes/Scene1_IslandCove.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Scene2_AncientTemple.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Scene3_TreasureVault.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/MainScene.unity", true)
        };

        EditorBuildSettings.scenes = buildScenes.ToArray();
        AssetDatabase.SaveAssets();
        Debug.Log("[Treasure Hunt] EditorBuildSettings updated with all 4 scene entries!");
    }
}
