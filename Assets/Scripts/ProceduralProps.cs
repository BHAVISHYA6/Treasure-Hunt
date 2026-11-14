using UnityEngine;

public static class ProceduralProps
{
    private static Material goldMat;
    private static Material bronzeMat;
    private static Material silverMat;
    private static Material woodMat;
    private static Material stoneMat;
    private static Material darkStoneMat;
    private static Material sandMat;
    private static Material fireRuneMat;
    private static Material waterRuneMat;
    private static Material earthRuneMat;
    private static Material waterSurfaceMat;
    private static Material leafMat;

    public static Material GetGoldMaterial()
    {
        if (goldMat == null)
        {
            goldMat = new Material(Shader.Find("Standard"));
            goldMat.color = new Color(1.0f, 0.82f, 0.2f);
            goldMat.SetFloat("_Metallic", 0.9f);
            goldMat.SetFloat("_Glossiness", 0.85f);
        }
        return goldMat;
    }

    public static Material GetSilverMaterial()
    {
        if (silverMat == null)
        {
            silverMat = new Material(Shader.Find("Standard"));
            silverMat.color = new Color(0.9f, 0.92f, 0.95f);
            silverMat.SetFloat("_Metallic", 0.92f);
            silverMat.SetFloat("_Glossiness", 0.9f);
        }
        return silverMat;
    }

    public static Material GetBronzeMaterial()
    {
        if (bronzeMat == null)
        {
            bronzeMat = new Material(Shader.Find("Standard"));
            bronzeMat.color = new Color(0.8f, 0.5f, 0.25f);
            bronzeMat.SetFloat("_Metallic", 0.75f);
            bronzeMat.SetFloat("_Glossiness", 0.65f);
        }
        return bronzeMat;
    }

    public static Material GetWoodMaterial()
    {
        if (woodMat == null)
        {
            woodMat = new Material(Shader.Find("Standard"));
            woodMat.color = new Color(0.38f, 0.24f, 0.14f);
            woodMat.SetFloat("_Glossiness", 0.25f);
        }
        return woodMat;
    }

    public static Material GetStoneMaterial()
    {
        if (stoneMat == null)
        {
            stoneMat = new Material(Shader.Find("Standard"));
            stoneMat.color = new Color(0.55f, 0.55f, 0.55f);
            stoneMat.SetFloat("_Glossiness", 0.15f);
        }
        return stoneMat;
    }

    public static Material GetDarkStoneMaterial()
    {
        if (darkStoneMat == null)
        {
            darkStoneMat = new Material(Shader.Find("Standard"));
            darkStoneMat.color = new Color(0.25f, 0.25f, 0.28f);
            darkStoneMat.SetFloat("_Glossiness", 0.2f);
        }
        return darkStoneMat;
    }

    public static Material GetSandMaterial()
    {
        if (sandMat == null)
        {
            sandMat = new Material(Shader.Find("Standard"));
            sandMat.color = new Color(0.86f, 0.78f, 0.55f);
            sandMat.SetFloat("_Glossiness", 0.05f);
        }
        return sandMat;
    }

    public static Material GetLeafMaterial()
    {
        if (leafMat == null)
        {
            leafMat = new Material(Shader.Find("Standard"));
            leafMat.color = new Color(0.18f, 0.55f, 0.2f);
            leafMat.SetFloat("_Glossiness", 0.2f);
        }
        return leafMat;
    }

    public static Material GetGlowingMaterial(Color color, string name = "RuneGlow")
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.name = name;
        mat.color = color;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", color * 1.5f);
        mat.SetFloat("_Glossiness", 0.8f);
        return mat;
    }

    // 1. Treasure Chest
    public static GameObject CreateTreasureChest(Vector3 position, Quaternion rotation, bool requiresKey, KeyPickup.KeyType keyType, int coins, bool isGrand)
    {
        GameObject chestRoot = new GameObject(isGrand ? "GrandLegendaryChest" : "TreasureChest");
        chestRoot.transform.position = position;
        chestRoot.transform.rotation = rotation;

        // Base
        GameObject chestBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
        chestBase.name = "ChestBase";
        chestBase.transform.SetParent(chestRoot.transform, false);
        chestBase.transform.localPosition = new Vector3(0f, 0.35f, 0f);
        chestBase.transform.localScale = isGrand ? new Vector3(1.8f, 0.7f, 1.2f) : new Vector3(1.2f, 0.6f, 0.8f);
        chestBase.GetComponent<Renderer>().material = GetWoodMaterial();

        // Base Rim Gold Trim
        GameObject rim = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rim.name = "GoldRim";
        rim.transform.SetParent(chestBase.transform, false);
        rim.transform.localPosition = new Vector3(0f, 0.45f, 0f);
        rim.transform.localScale = new Vector3(1.05f, 0.15f, 1.05f);
        rim.GetComponent<Renderer>().material = isGrand ? GetGoldMaterial() : GetBronzeMaterial();
        Object.DestroyImmediate(rim.GetComponent<Collider>());

        // Lid Pivot & Lid
        GameObject lidPivot = new GameObject("LidPivot");
        lidPivot.transform.SetParent(chestRoot.transform, false);
        float lidY = isGrand ? 0.7f : 0.65f;
        float lidZ = isGrand ? -0.6f : -0.4f;
        lidPivot.transform.localPosition = new Vector3(0f, lidY, lidZ);

        GameObject lidMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lidMesh.name = "LidMesh";
        lidMesh.transform.SetParent(lidPivot.transform, false);
        float halfDepth = isGrand ? 0.6f : 0.4f;
        lidMesh.transform.localPosition = new Vector3(0f, isGrand ? 0.2f : 0.18f, halfDepth);
        lidMesh.transform.localScale = isGrand ? new Vector3(1.85f, 0.35f, 1.25f) : new Vector3(1.25f, 0.3f, 0.85f);
        lidMesh.GetComponent<Renderer>().material = GetWoodMaterial();

        // Gold Lock on Lid
        GameObject lockMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lockMesh.name = "Lock";
        lockMesh.transform.SetParent(lidMesh.transform, false);
        lockMesh.transform.localPosition = new Vector3(0f, -0.4f, 0.52f);
        lockMesh.transform.localScale = new Vector3(0.25f, 0.4f, 0.15f);
        lockMesh.GetComponent<Renderer>().material = isGrand ? GetGoldMaterial() : (keyType == KeyPickup.KeyType.Silver ? GetSilverMaterial() : GetBronzeMaterial());
        Object.DestroyImmediate(lockMesh.GetComponent<Collider>());

        // Interior Treasure Glow Light
        GameObject glowObj = new GameObject("InteriorGlow");
        glowObj.transform.SetParent(chestRoot.transform, false);
        glowObj.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        Light glowLight = glowObj.AddComponent<Light>();
        glowLight.type = LightType.Point;
        glowLight.color = isGrand ? Color.yellow : new Color(1f, 0.8f, 0.4f);
        glowLight.range = isGrand ? 6f : 4f;
        glowLight.intensity = 0f;

        // Treasure Burst Particles
        GameObject partObj = new GameObject("SparkleParticles");
        partObj.transform.SetParent(chestRoot.transform, false);
        partObj.transform.localPosition = new Vector3(0f, 0.6f, 0f);
        ParticleSystem ps = partObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = 2.5f;
        main.startLifetime = 1.5f;
        main.startSpeed = isGrand ? 4f : 2.5f;
        main.startSize = 0.25f;
        main.startColor = Color.yellow;
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, isGrand ? 60 : 30) });
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 25f;

        // Interaction Box Collider on Root
        BoxCollider boxCol = chestRoot.AddComponent<BoxCollider>();
        boxCol.center = new Vector3(0f, 0.6f, 0f);
        boxCol.size = isGrand ? new Vector3(2.5f, 1.8f, 2.5f) : new Vector3(2f, 1.5f, 2f);
        boxCol.isTrigger = true;

        TreasureChest tc = chestRoot.AddComponent<TreasureChest>();
        tc.chestName = isGrand ? "Pirate King's Grand Vault Chest" : (requiresKey ? $"{keyType} Locked Chest" : "Wooden Cache");
        tc.requiresKey = requiresKey;
        tc.requiredKeyType = keyType;
        tc.coinsReward = coins;
        tc.isGrandVaultChest = isGrand;
        tc.chestLid = lidPivot.transform;
        tc.interiorGlow = glowLight;
        tc.treasureParticles = ps;

        return chestRoot;
    }

    // 2. Collectible Coin / Gem
    public static GameObject CreateCollectibleCoin(Vector3 position, CollectibleCoin.CoinType type, int value = 1)
    {
        GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        coin.name = type == CollectibleCoin.CoinType.MapFragment ? "MapFragment" : (type == CollectibleCoin.CoinType.RareGem ? "RareGem" : "GoldCoin");
        coin.transform.position = position;

        if (type == CollectibleCoin.CoinType.MapFragment)
        {
            coin.transform.localScale = new Vector3(0.5f, 0.05f, 0.7f);
            coin.GetComponent<Renderer>().material = GetGlowingMaterial(new Color(0.3f, 0.9f, 1f), "ParchmentGlow");
        }
        else if (type == CollectibleCoin.CoinType.RareGem)
        {
            coin.transform.localScale = new Vector3(0.4f, 0.5f, 0.4f);
            coin.GetComponent<Renderer>().material = GetGlowingMaterial(Color.magenta, "GemGlow");
        }
        else
        {
            coin.transform.localScale = new Vector3(0.55f, 0.07f, 0.55f);
            coin.transform.rotation = Quaternion.Euler(75f, 0f, 0f);
            coin.GetComponent<Renderer>().material = GetGoldMaterial();
        }

        var col = coin.GetComponent<Collider>();
        col.isTrigger = true;

        var cc = coin.AddComponent<CollectibleCoin>();
        cc.type = type;
        cc.coinValue = value;

        return coin;
    }

    // 3. Key Pickup
    public static GameObject CreateKeyPickup(Vector3 position, KeyPickup.KeyType type, string name, bool requiresAllMapPieces = false, bool hiddenUntilReady = false)
    {
        GameObject keyRoot = new GameObject(name);
        keyRoot.transform.position = position;

        Material keyMat = type == KeyPickup.KeyType.Gold ? GetGoldMaterial() : (type == KeyPickup.KeyType.Silver ? GetSilverMaterial() : GetBronzeMaterial());

        // Ring Head
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "KeyRing";
        ring.transform.SetParent(keyRoot.transform, false);
        ring.transform.localPosition = new Vector3(0f, 0.4f, 0f);
        ring.transform.localScale = new Vector3(0.35f, 0.05f, 0.35f);
        ring.GetComponent<Renderer>().material = keyMat;
        Object.DestroyImmediate(ring.GetComponent<Collider>());

        // Shaft
        GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        shaft.name = "KeyShaft";
        shaft.transform.SetParent(keyRoot.transform, false);
        shaft.transform.localPosition = new Vector3(0f, 0.1f, 0f);
        shaft.transform.localScale = new Vector3(0.1f, 0.3f, 0.1f);
        shaft.GetComponent<Renderer>().material = keyMat;
        Object.DestroyImmediate(shaft.GetComponent<Collider>());

        // Teeth
        GameObject teeth = GameObject.CreatePrimitive(PrimitiveType.Cube);
        teeth.name = "KeyTeeth";
        teeth.transform.SetParent(keyRoot.transform, false);
        teeth.transform.localPosition = new Vector3(0.12f, -0.1f, 0f);
        teeth.transform.localScale = new Vector3(0.15f, 0.15f, 0.08f);
        teeth.GetComponent<Renderer>().material = keyMat;
        Object.DestroyImmediate(teeth.GetComponent<Collider>());

        // Glow Light
        GameObject glow = new GameObject("KeyGlow");
        glow.transform.SetParent(keyRoot.transform, false);
        var l = glow.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = type == KeyPickup.KeyType.Gold ? Color.yellow : (type == KeyPickup.KeyType.Silver ? Color.cyan : new Color(1f, 0.6f, 0.2f));
        l.range = 3f;
        l.intensity = 1.5f;

        SphereCollider sc = keyRoot.AddComponent<SphereCollider>();
        sc.radius = 1.2f;
        sc.isTrigger = true;

        KeyPickup kp = keyRoot.AddComponent<KeyPickup>();
        kp.keyType = type;
        kp.keyName = name;
        kp.requiresAllMapPieces = requiresAllMapPieces;
        kp.hiddenUntilReady = hiddenUntilReady;

        return keyRoot;
    }

    // 4. Clue Scroll / Tablet
    public static GameObject CreateClueScroll(Vector3 position, string title, string text, string promptAction = "Read")
    {
        GameObject clueRoot = new GameObject($"Clue_{title}");
        clueRoot.transform.position = position;

        // Pedestal base
        GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pedestal.transform.SetParent(clueRoot.transform, false);
        pedestal.transform.localPosition = new Vector3(0f, 0.4f, 0f);
        pedestal.transform.localScale = new Vector3(0.8f, 0.4f, 0.8f);
        pedestal.GetComponent<Renderer>().material = GetStoneMaterial();

        // Scroll / Tablet Visual
        GameObject scroll = GameObject.CreatePrimitive(PrimitiveType.Cube);
        scroll.name = "ParchmentVisual";
        scroll.transform.SetParent(clueRoot.transform, false);
        scroll.transform.localPosition = new Vector3(0f, 1.1f, 0f);
        scroll.transform.localScale = new Vector3(0.6f, 0.08f, 0.45f);
        scroll.transform.rotation = Quaternion.Euler(20f, 0f, 0f);
        scroll.GetComponent<Renderer>().material = GetGlowingMaterial(new Color(0.95f, 0.88f, 0.7f), "ParchmentMat");
        Object.DestroyImmediate(scroll.GetComponent<Collider>());

        // Candle / Light
        GameObject glow = new GameObject("CandleGlow");
        glow.transform.SetParent(clueRoot.transform, false);
        glow.transform.localPosition = new Vector3(0f, 1.3f, 0f);
        var l = glow.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = new Color(1f, 0.75f, 0.4f);
        l.range = 4f;
        l.intensity = 1.2f;

        SphereCollider sc = clueRoot.AddComponent<SphereCollider>();
        sc.radius = 2.0f;
        sc.isTrigger = true;

        ClueScroll cs = clueRoot.AddComponent<ClueScroll>();
        cs.clueTitle = title;
        cs.clueContent = text;
        cs.promptAction = promptAction;
        cs.rotatingVisual = scroll.transform;

        return clueRoot;
    }

    // 5. Elemental Altar
    public static GameObject CreateElementalAltar(Vector3 position, ElementalAltar.ElementType type, string name, Color color)
    {
        GameObject altarRoot = new GameObject(name);
        altarRoot.transform.position = position;

        // Stepped Stone Base
        GameObject base1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        base1.transform.SetParent(altarRoot.transform, false);
        base1.transform.localPosition = new Vector3(0f, 0.25f, 0f);
        base1.transform.localScale = new Vector3(2.2f, 0.25f, 2.2f);
        base1.GetComponent<Renderer>().material = GetDarkStoneMaterial();

        GameObject base2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        base2.transform.SetParent(altarRoot.transform, false);
        base2.transform.localPosition = new Vector3(0f, 0.65f, 0f);
        base2.transform.localScale = new Vector3(1.6f, 0.3f, 1.6f);
        base2.GetComponent<Renderer>().material = GetStoneMaterial();

        // Hovering Elemental Crystal
        GameObject crystal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        crystal.name = "RuneCrystal";
        crystal.transform.SetParent(altarRoot.transform, false);
        crystal.transform.localPosition = new Vector3(0f, 1.4f, 0f);
        crystal.transform.localScale = new Vector3(0.45f, 0.5f, 0.45f);
        var r = crystal.GetComponent<Renderer>();
        r.material = GetGlowingMaterial(color * 0.4f, $"{name}_Crystal");

        // Light
        GameObject lightObj = new GameObject("BeaconLight");
        lightObj.transform.SetParent(altarRoot.transform, false);
        lightObj.transform.localPosition = new Vector3(0f, 2.0f, 0f);
        Light beacon = lightObj.AddComponent<Light>();
        beacon.type = LightType.Point;
        beacon.color = color;
        beacon.range = 7f;
        beacon.intensity = 3f;
        beacon.enabled = false;

        // Particle System
        GameObject psObj = new GameObject("BeaconParticles");
        psObj.transform.SetParent(altarRoot.transform, false);
        psObj.transform.localPosition = new Vector3(0f, 1.4f, 0f);
        ParticleSystem ps = psObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = color;
        main.startLifetime = 1.2f;
        main.startSpeed = 1.5f;
        main.startSize = 0.2f;
        main.playOnAwake = false;
        var em = ps.emission;
        em.rateOverTime = 25;

        SphereCollider sc = altarRoot.AddComponent<SphereCollider>();
        sc.radius = 2.5f;
        sc.isTrigger = true;

        ElementalAltar ea = altarRoot.AddComponent<ElementalAltar>();
        ea.elementType = type;
        ea.altarName = name;
        ea.elementColor = color;
        ea.beaconLight = beacon;
        ea.beaconParticles = ps;
        ea.crystalRenderer = r;

        return altarRoot;
    }

    // 6. Scene Portal Archway
    public static GameObject CreatePortalArchway(Vector3 position, Quaternion rotation, string targetScene, string name, bool reqKey, KeyPickup.KeyType keyType, Color portalColor)
    {
        GameObject portalRoot = new GameObject(name);
        portalRoot.transform.position = position;
        portalRoot.transform.rotation = rotation;

        // Left Pillar
        GameObject leftPillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leftPillar.name = "PillarLeft";
        leftPillar.transform.SetParent(portalRoot.transform, false);
        leftPillar.transform.localPosition = new Vector3(-1.8f, 2.2f, 0f);
        leftPillar.transform.localScale = new Vector3(0.6f, 2.2f, 0.6f);
        leftPillar.GetComponent<Renderer>().material = GetDarkStoneMaterial();

        // Right Pillar
        GameObject rightPillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rightPillar.name = "PillarRight";
        rightPillar.transform.SetParent(portalRoot.transform, false);
        rightPillar.transform.localPosition = new Vector3(1.8f, 2.2f, 0f);
        rightPillar.transform.localScale = new Vector3(0.6f, 2.2f, 0.6f);
        rightPillar.GetComponent<Renderer>().material = GetDarkStoneMaterial();

        // Top Arch Beam
        GameObject topArch = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topArch.name = "ArchBeam";
        topArch.transform.SetParent(portalRoot.transform, false);
        topArch.transform.localPosition = new Vector3(0f, 4.4f, 0f);
        topArch.transform.localScale = new Vector3(4.4f, 0.6f, 0.8f);
        topArch.GetComponent<Renderer>().material = GetStoneMaterial();

        // Swirling Vortex Disc
        GameObject vortex = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        vortex.name = "VortexDisc";
        vortex.transform.SetParent(portalRoot.transform, false);
        vortex.transform.localPosition = new Vector3(0f, 2.2f, 0f);
        vortex.transform.localScale = new Vector3(2.6f, 0.05f, 3.8f);
        vortex.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        vortex.GetComponent<Renderer>().material = GetGlowingMaterial(portalColor, "PortalVortexMat");
        Object.DestroyImmediate(vortex.GetComponent<Collider>());

        // Portal Light
        GameObject glow = new GameObject("PortalGlow");
        glow.transform.SetParent(portalRoot.transform, false);
        glow.transform.localPosition = new Vector3(0f, 2.2f, 0f);
        Light pl = glow.AddComponent<Light>();
        pl.type = LightType.Point;
        pl.color = portalColor;
        pl.range = 8f;
        pl.intensity = 3.5f;

        // Particle System
        GameObject psObj = new GameObject("PortalParticles");
        psObj.transform.SetParent(portalRoot.transform, false);
        psObj.transform.localPosition = new Vector3(0f, 2.2f, 0f);
        ParticleSystem ps = psObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = portalColor;
        main.startLifetime = 1.0f;
        main.startSpeed = 1.2f;
        main.startSize = 0.3f;
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 1.2f;

        BoxCollider bc = portalRoot.AddComponent<BoxCollider>();
        bc.center = new Vector3(0f, 2.2f, 0f);
        bc.size = new Vector3(3.2f, 4.0f, 1.5f);
        bc.isTrigger = true;

        ScenePortal sp = portalRoot.AddComponent<ScenePortal>();
        sp.portalName = name;
        sp.targetSceneName = targetScene;
        sp.requiresKey = reqKey;
        sp.requiredKey = keyType;
        sp.vortexVisual = vortex.transform;
        sp.portalGlow = pl;
        sp.portalParticles = ps;

        return portalRoot;
    }

    // 7. Spike Trap
    public static GameObject CreateSpikeTrap(Vector3 position, float startDelay = 0f)
    {
        GameObject trapRoot = new GameObject("SpikeTrap");
        trapRoot.transform.position = position;

        // Grate base
        GameObject grate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        grate.name = "GrateBase";
        grate.transform.SetParent(trapRoot.transform, false);
        grate.transform.localPosition = new Vector3(0f, 0.05f, 0f);
        grate.transform.localScale = new Vector3(2.0f, 0.1f, 2.0f);
        grate.GetComponent<Renderer>().material = GetDarkStoneMaterial();

        // Spikes group
        GameObject spikes = new GameObject("SpikeGroup");
        spikes.transform.SetParent(trapRoot.transform, false);

        for (int x = -1; x <= 1; x += 2)
        {
            for (int z = -1; z <= 1; z += 2)
            {
                GameObject spike = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                spike.name = "SpikeCone";
                spike.transform.SetParent(spikes.transform, false);
                spike.transform.localPosition = new Vector3(x * 0.5f, 0.25f, z * 0.5f);
                spike.transform.localScale = new Vector3(0.12f, 0.35f, 0.12f);
                spike.GetComponent<Renderer>().material = GetSilverMaterial();
                Object.DestroyImmediate(spike.GetComponent<Collider>());
            }
        }

        // Warning Light
        GameObject wl = new GameObject("WarningLight");
        wl.transform.SetParent(trapRoot.transform, false);
        wl.transform.localPosition = new Vector3(0f, 0.2f, 0f);
        Light light = wl.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = Color.red;
        light.range = 3f;
        light.intensity = 2f;
        light.enabled = false;

        BoxCollider bc = trapRoot.AddComponent<BoxCollider>();
        bc.center = new Vector3(0f, 0.4f, 0f);
        bc.size = new Vector3(1.8f, 0.8f, 1.8f);
        bc.isTrigger = true;

        // Solid collider that is enabled only when spikes are extended
        GameObject solidBoxObj = new GameObject("SolidSpikeCollider");
        solidBoxObj.transform.SetParent(spikes.transform, false);
        solidBoxObj.transform.localPosition = new Vector3(0f, 0.25f, 0f);
        BoxCollider solidBox = solidBoxObj.AddComponent<BoxCollider>();
        solidBox.size = new Vector3(1.6f, 0.6f, 1.6f);
        solidBox.enabled = false;

        SpikeTrap st = trapRoot.AddComponent<SpikeTrap>();
        st.spikeGroup = spikes.transform;
        st.warningLight = light;
        st.solidSpikeCollider = solidBox;
        st.startDelay = startDelay;

        return trapRoot;
    }

    // 8. Moving Platform
    public static GameObject CreateMovingPlatform(Vector3 position, Vector3 localDelta, float speed = 2.5f)
    {
        GameObject plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plat.name = "MovingPlatform";
        plat.transform.position = position;
        plat.transform.localScale = new Vector3(3.0f, 0.4f, 3.0f);
        plat.GetComponent<Renderer>().material = GetStoneMaterial();

        // Rider trigger on top
        GameObject trigObj = new GameObject("RiderTrigger");
        trigObj.transform.SetParent(plat.transform, false);
        trigObj.transform.localPosition = new Vector3(0f, 0.6f, 0f);
        BoxCollider bc = trigObj.AddComponent<BoxCollider>();
        bc.size = new Vector3(1f, 0.4f, 1f);
        bc.isTrigger = true;

        MovingPlatform mp = plat.AddComponent<MovingPlatform>();
        mp.localEndPoint = localDelta;
        mp.moveSpeed = speed;

        return plat;
    }

    // 9. Ruin Pillar
    public static GameObject CreateRuinPillar(Vector3 position, float height = 4f, float radius = 0.5f)
    {
        GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pillar.name = "RuinPillar";
        pillar.transform.position = position + Vector3.up * (height * 0.5f);
        pillar.transform.localScale = new Vector3(radius * 2f, height * 0.5f, radius * 2f);
        pillar.GetComponent<Renderer>().material = GetStoneMaterial();

        // Capital head
        GameObject cap = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cap.name = "PillarCapital";
        cap.transform.SetParent(pillar.transform, false);
        cap.transform.localPosition = new Vector3(0f, 1f, 0f);
        cap.transform.localScale = new Vector3(1.3f, 0.2f, 1.3f);
        cap.GetComponent<Renderer>().material = GetDarkStoneMaterial();

        return pillar;
    }

    // 10. Palm Tree
    public static GameObject CreatePalmTree(Vector3 position, float height = 5f)
    {
        GameObject tree = new GameObject("PalmTree");
        tree.transform.position = position;

        // Solid impassable trunk collider on the root
        CapsuleCollider col = tree.AddComponent<CapsuleCollider>();
        col.radius = 0.55f;
        col.height = height;
        col.center = new Vector3(0f, height * 0.5f, 0f);

        // Trunk
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Trunk";
        trunk.transform.SetParent(tree.transform, false);
        trunk.transform.localPosition = new Vector3(0.2f, height * 0.5f, 0f);
        trunk.transform.localScale = new Vector3(0.35f, height * 0.5f, 0.35f);
        trunk.transform.localRotation = Quaternion.Euler(4f, 0f, 2f);
        trunk.GetComponent<Renderer>().material = GetWoodMaterial();

        // Fronds
        for (int i = 0; i < 6; i++)
        {
            GameObject frond = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frond.name = $"Frond_{i}";
            frond.transform.SetParent(tree.transform, false);
            frond.transform.localPosition = new Vector3(0f, height, 0f);
            frond.transform.localRotation = Quaternion.Euler(25f, i * 60f, 0f);
            frond.transform.localScale = new Vector3(0.6f, 0.08f, 2.5f);
            frond.transform.position += frond.transform.forward * 1.0f;
            frond.GetComponent<Renderer>().material = GetLeafMaterial();
            Object.DestroyImmediate(frond.GetComponent<Collider>());
        }

        return tree;
    }

    // 11. Wall Torch
    public static GameObject CreateWallTorch(Vector3 position, Quaternion rotation)
    {
        GameObject torch = new GameObject("WallTorch");
        torch.transform.position = position;
        torch.transform.rotation = rotation;

        // Stick
        GameObject stick = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stick.name = "TorchStick";
        stick.transform.SetParent(torch.transform, false);
        stick.transform.localPosition = new Vector3(0f, 0.25f, 0f);
        stick.transform.localScale = new Vector3(0.08f, 0.25f, 0.08f);
        stick.GetComponent<Renderer>().material = GetWoodMaterial();
        Object.DestroyImmediate(stick.GetComponent<Collider>());

        // Flame Light
        GameObject flObj = new GameObject("FlameLight");
        flObj.transform.SetParent(torch.transform, false);
        flObj.transform.localPosition = new Vector3(0f, 0.55f, 0f);
        Light l = flObj.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = new Color(1f, 0.6f, 0.2f);
        l.range = 6f;
        l.intensity = 2.2f;

        return torch;
    }

    // 12. Water Plane
    public static GameObject CreateWaterPlane(Vector3 position, Vector2 size)
    {
        GameObject water = GameObject.CreatePrimitive(PrimitiveType.Cube);
        water.name = "WaterSurface";
        water.transform.position = position - Vector3.up * 0.2f;
        water.transform.localScale = new Vector3(size.x, 0.4f, size.y);
        var r = water.GetComponent<Renderer>();
        r.material = GetGlowingMaterial(new Color(0.1f, 0.45f, 0.75f, 0.8f), "WaterMat");
        
        // Water is a hazard: trigger collider with WaterHazard script
        var col = water.GetComponent<Collider>();
        col.isTrigger = true;
        water.AddComponent<WaterHazard>();

        return water;
    }

    // 13. Solid Rock Obstacle
    public static GameObject CreateRockObstacle(Vector3 position, Vector3 scale, Quaternion rotation)
    {
        GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rock.name = "RockObstacle";
        rock.transform.position = position;
        rock.transform.rotation = rotation;
        rock.transform.localScale = scale;
        rock.GetComponent<Renderer>().material = GetDarkStoneMaterial();
        return rock;
    }

    // 14. Solid Cliff Wall
    public static GameObject CreateCliffWall(Vector3 position, Vector3 size, Quaternion rotation)
    {
        GameObject cliff = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cliff.name = "CliffWall";
        cliff.transform.position = position;
        cliff.transform.rotation = rotation;
        cliff.transform.localScale = size;
        cliff.GetComponent<Renderer>().material = GetDarkStoneMaterial();
        return cliff;
    }

    // 15. Solid Stone Dungeon Wall
    public static GameObject CreateStoneWall(Vector3 position, Vector3 size, Quaternion rotation)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "StoneWall";
        wall.transform.position = position;
        wall.transform.rotation = rotation;
        wall.transform.localScale = size;
        wall.GetComponent<Renderer>().material = GetStoneMaterial();
        return wall;
    }

    // 16. Solid Wooden Crate
    public static GameObject CreateWoodenCrate(Vector3 position, Vector3 size)
    {
        GameObject crate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        crate.name = "WoodenCrate";
        crate.transform.position = position;
        crate.transform.localScale = size;
        crate.GetComponent<Renderer>().material = GetWoodMaterial();
        return crate;
    }

    // 17. Pirate Campfire Site
    public static GameObject CreateCampfire(Vector3 position)
    {
        GameObject camp = new GameObject("CampfireSite");
        camp.transform.position = position;

        // Stone fire ring
        for (int i = 0; i < 6; i++)
        {
            float ang = i * 60f * Mathf.Deg2Rad;
            GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stone.transform.SetParent(camp.transform, false);
            stone.transform.localPosition = new Vector3(Mathf.Cos(ang) * 0.7f, 0.15f, Mathf.Sin(ang) * 0.7f);
            stone.transform.localScale = new Vector3(0.35f, 0.3f, 0.35f);
            stone.GetComponent<Renderer>().material = GetDarkStoneMaterial();
        }

        // Fire glow light
        GameObject fireLight = new GameObject("FireLight");
        fireLight.transform.SetParent(camp.transform, false);
        fireLight.transform.localPosition = new Vector3(0f, 0.45f, 0f);
        Light fl = fireLight.AddComponent<Light>();
        fl.type = LightType.Point;
        fl.color = new Color(1f, 0.5f, 0.1f);
        fl.range = 8f;
        fl.intensity = 2.8f;

        // Crates and wood around fire
        CreateWoodenCrate(position + new Vector3(1.3f, 0.4f, 0.4f), new Vector3(0.85f, 0.8f, 0.85f)).transform.SetParent(camp.transform);
        CreateWoodenCrate(position + new Vector3(-1.2f, 0.35f, -0.5f), new Vector3(0.75f, 0.7f, 0.75f)).transform.SetParent(camp.transform);

        return camp;
    }
}
