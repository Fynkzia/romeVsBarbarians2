using UnityEngine;
using UnityEditor;

public class TerrainGeneratorWindow : EditorWindow
{
    // ---- Base Terrain ----
    public Terrain baseTerrain;  // берем настройки отсюда
    private TerrainData baseData;

    // ---- Terrain settings ----
    public int terrainSize = 400;
    public int heightmapResolution = 513;
    public int alphamapResolution = 512;
    public int basemapResolution = 1024;
    public int detailResolution = 1024;
    public Material terrainMaterial;

    // ---- Small noise ----
    public float smallNoiseScale = 30f;
    public float smallNoiseHeight = 1f;

    // ---- big noise ----
    public float bigNoiseScale = 30f;
    public float bigNoiseHeight = 1f;

    // ---- Medium hills ----
    public int mediumHillsCount = 8;
    public float mediumHillRadius = 30f;
    public float mediumHillHeight = 4f;

    // ---- Flat hills ----
    public int flatHillsCount = 8;
    public float flatHillRadius = 30f;
    public float flatHillHeight = 4f;
    public float flatHillTerrainHeight = 4f;

    // ---- Mountains ----
    public int mountainsCount = 4;
    public float mountainRadius = 80f;
    public float mountainHeight = 15f;

    // ---- Textures ----
    public Texture2D mainTex;
    public Texture2D grassTex;
    public Texture2D dirtTex;
    public Texture2D rockTex;

    // ---- Prefabs ----
    public GameObject grassPrefab;
    public int grassAmount = 2000;

    public GameObject treePrefab;
    public int treeAmount = 300;

    private Vector2 scrollPos;
    private TerrainData dataToSave;

    [MenuItem("Tools/Procedural Terrain Generator")]
    public static void OpenWindow()
    {
        GetWindow<TerrainGeneratorWindow>("Terrain Generator");
    }

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.Space(20);

        if (GUILayout.Button("GENERATE TERRAIN", GUILayout.Height(40)))
        {
            GenerateTerrain();
        }

//        if (GUILayout.Button("Save TERRAIN", GUILayout.Height(40)))
//        {
//#if UNITY_EDITOR
//            string folder = "Assets/GeneratedTerrainData";
//            if (!AssetDatabase.IsValidFolder(folder))
//            {
//                AssetDatabase.CreateFolder("Assets", "GeneratedTerrainData");
//            }

//            // создаём уникальное имя, чтобы не перезаписывать старые
//            string path = folder + "/TerrainData_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".asset";
//            AssetDatabase.CreateAsset(dataToSave, path);
//            AssetDatabase.SaveAssets();
//            Debug.Log("Saved TerrainData to: " + path);
//#endif
//        }



        GUILayout.Label("Base Terrain Source", EditorStyles.boldLabel);
        baseTerrain = (Terrain)EditorGUILayout.ObjectField("Copy settings from:", baseTerrain, typeof(Terrain), true);

        if (baseTerrain != null)
        {
            baseData = baseTerrain.terrainData;
            EditorGUILayout.HelpBox("Using settings from provided Terrain", MessageType.Info);

            terrainSize = (int)baseData.size.x;
            heightmapResolution = baseData.heightmapResolution;
            alphamapResolution = baseData.alphamapResolution;
            basemapResolution = baseData.baseMapResolution;
            detailResolution = baseData.detailResolution;
            terrainMaterial = baseTerrain.materialTemplate;
        }

        GUILayout.Space(10);
        GUILayout.Label("Terrain Settings Override", EditorStyles.boldLabel);

        terrainSize = EditorGUILayout.IntField("Terrain Size", terrainSize);
        heightmapResolution = EditorGUILayout.IntField("Heightmap Resolution", heightmapResolution);
        alphamapResolution = EditorGUILayout.IntField("Alphamap Resolution", alphamapResolution);
        basemapResolution = EditorGUILayout.IntField("Basemap Resolution", basemapResolution);
        detailResolution = EditorGUILayout.IntField("Detail Resolution", detailResolution);

        terrainMaterial = (Material)EditorGUILayout.ObjectField("Custom Material", terrainMaterial, typeof(Material), false);

        GUILayout.Space(20);
        GUILayout.Label("Noise Terrain Generation", EditorStyles.boldLabel);

        smallNoiseScale = EditorGUILayout.FloatField("Small Noise Scale", smallNoiseScale);
        smallNoiseHeight = EditorGUILayout.FloatField("Small Noise Height", smallNoiseHeight);

        GUILayout.Space(10);
        GUILayout.Label("BigNoise Terrain Generation", EditorStyles.boldLabel);

        bigNoiseScale = EditorGUILayout.FloatField("Big Noise Scale", bigNoiseScale);
        bigNoiseHeight = EditorGUILayout.FloatField("Big Noise Height", bigNoiseHeight);

        GUILayout.Space(10);
        GUILayout.Label("Medium Hills", EditorStyles.boldLabel);

        mediumHillsCount = EditorGUILayout.IntField("Count", mediumHillsCount);
        mediumHillRadius = EditorGUILayout.FloatField("Radius", mediumHillRadius);
        mediumHillHeight = EditorGUILayout.FloatField("Height", mediumHillHeight);

        GUILayout.Space(10);
        GUILayout.Label("Flat Hills", EditorStyles.boldLabel);

        flatHillsCount = EditorGUILayout.IntField("Count", flatHillsCount);
        flatHillRadius = EditorGUILayout.FloatField("Radius", flatHillRadius);
        flatHillHeight = EditorGUILayout.FloatField("Height", flatHillHeight);
        flatHillTerrainHeight = EditorGUILayout.FloatField("Terrain Height", flatHillTerrainHeight);

        GUILayout.Space(10);
        GUILayout.Label("Mountains", EditorStyles.boldLabel);

        mountainsCount = EditorGUILayout.IntField("Count", mountainsCount);
        mountainRadius = EditorGUILayout.FloatField("Radius", mountainRadius);
        mountainHeight = EditorGUILayout.FloatField("Height", mountainHeight);

        GUILayout.Space(10);
        GUILayout.Label("Terrain Textures", EditorStyles.boldLabel);

        mainTex = (Texture2D)EditorGUILayout.ObjectField("Main", mainTex, typeof(Texture2D), false);
        grassTex = (Texture2D)EditorGUILayout.ObjectField("Grass", grassTex, typeof(Texture2D), false);
        dirtTex = (Texture2D)EditorGUILayout.ObjectField("Dirt", dirtTex, typeof(Texture2D), false);
        rockTex = (Texture2D)EditorGUILayout.ObjectField("Rock", rockTex, typeof(Texture2D), false);

        GUILayout.Space(10);
        GUILayout.Label("Prefabs", EditorStyles.boldLabel);

        grassPrefab = (GameObject)EditorGUILayout.ObjectField("Grass Prefab", grassPrefab, typeof(GameObject), false);
        grassAmount = EditorGUILayout.IntField("Grass Amount", grassAmount);

        treePrefab = (GameObject)EditorGUILayout.ObjectField("Tree Prefab", treePrefab, typeof(GameObject), false);
        treeAmount = EditorGUILayout.IntField("Trees Amount", treeAmount);

        EditorGUILayout.EndScrollView();
    }

    void GenerateTerrain()
    {
        TerrainData tData = new TerrainData();

#if UNITY_EDITOR
        string folder = "Assets/GeneratedTerrainData";
        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder("Assets", "GeneratedTerrainData");
        }

        // создаём уникальное имя, чтобы не перезаписывать старые
        string path = folder + "/TerrainData_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".asset";
        AssetDatabase.CreateAsset(tData, path);
        AssetDatabase.SaveAssets();
        Debug.Log("Saved TerrainData to: " + path);
#endif

        tData.heightmapResolution = heightmapResolution;
        tData.alphamapResolution = alphamapResolution;
        tData.baseMapResolution = basemapResolution;
        tData.SetDetailResolution(detailResolution, 8);

        tData.size = new Vector3(terrainSize, 30, terrainSize);

        float offsetX = Random.Range(0f, 99999f);
        float offsetZ = Random.Range(0f, 99999f);

        int res = heightmapResolution;

        float[,] bigNoiseMap = new float[res, res];

        for (int x = 0; x < res; x++)
        {
            for (int z = 0; z < res; z++)
            {
                float nx = (x + offsetX) / res * bigNoiseScale;
                float nz = (z + offsetZ) / res * bigNoiseScale;

                bigNoiseMap[z, x] = Mathf.PerlinNoise(nx, nz) * (bigNoiseHeight / 20f);
            }
        }
        //AddFractures(bigNoiseMap);

        // --- 1. Большая карта --- (горы + холмы)
        float[,] largeMap = new float[res, res];

        for (int i = 0; i < flatHillsCount; i++)
            AddPlateuHill(largeMap,flatHillTerrainHeight, flatHillRadius, flatHillRadius * 1.2f, flatHillHeight, flatHillHeight * 1.2f);
        //AddHill(largeMap, mediumHillRadius, mediumHillHeight);

        for (int i = 0; i < mediumHillsCount; i++) 
        AddHill(largeMap, mediumHillRadius, mediumHillHeight);

        for (int i = 0; i < mountainsCount; i++)
            AddHill(largeMap, mountainRadius, mountainHeight);



        // --- 2. ПИКСЕЛИЗАЦИЯ больших форм ---
        int steps = 20; // количество ступеней
        for (int x = 0; x < res; x++)
        {
            for (int z = 0; z < res; z++)
            {
                largeMap[z, x] = Mathf.Round(largeMap[z, x] * steps) / steps;
                bigNoiseMap[z, x] = Mathf.Round(bigNoiseMap[z, x] * steps) / steps;
            }
        }

        // --- 3. Мелкий шум ---
        float[,] noiseMap = new float[res, res];

        for (int x = 0; x < res; x++)
        {
            for (int z = 0; z < res; z++)
            {
                float nx = x / (float)res * smallNoiseScale;
                float nz = z / (float)res * smallNoiseScale;

                noiseMap[z, x] = Mathf.PerlinNoise(nx, nz) * (smallNoiseHeight / 20f);
            }
        }

        // --- 4. Сложение ---
        float[,] finalMap = new float[res, res];
        for (int x = 0; x < res; x++)
        {
            for (int z = 0; z < res; z++)
            {
                // largeMap доминирует там, где он выше
                float combined = Mathf.Max(largeMap[z, x], bigNoiseMap[z, x]) + noiseMap[z, x];
                finalMap[z, x] = combined;
            }
        }

        

        // --- 5. Установка ---
        tData.SetHeights(0, 0, finalMap);

        // Create terrain object
        GameObject terrain = Terrain.CreateTerrainGameObject(tData);
        terrain.name = "GeneratedTerrain";

        if (terrainMaterial != null)
            terrain.GetComponent<Terrain>().materialTemplate = terrainMaterial;

        // Textures
        ApplyTextures(tData);
       GenerateDirtPatches(tData,50,0,10);


        // Prefabs
        //ScatterPrefabs(grassPrefab, grassAmount, terrain);
        //ScatterPrefabs(treePrefab, treeAmount, terrain);

        GenerateGrassPatches(tData,grassAmount,0,1,terrain);
        GenerateGrassPatches(tData, grassAmount/2, 1, 2, terrain);

        terrain.GetComponent<Terrain>().heightmapPixelError = 20;
        terrain.GetComponent<Terrain>().heightmapMaximumLOD = 1;
        terrain.layer = 3;
        terrain.tag = "Terrain";

        EditorUtility.SetDirty(tData);

        // Сохранить
        AssetDatabase.SaveAssets();
        // --- SAVE TERRAIN DATA ---

        Debug.Log("Terrain Generated");
    }




    void AddHill(float[,] map, float baseRadius, float height)
    {
        int res = map.GetLength(0);
        int cx = Random.Range(0, res);
        int cz = Random.Range(0, res);

        // Основной круглый холм
        for (int x = 0; x < res; x++)
        {
            for (int z = 0; z < res; z++)
            {
                float dist = Vector2.Distance(new Vector2(x, z), new Vector2(cx, cz));
                if (dist < baseRadius)
                {
                    float t = 1f - dist / baseRadius;
                    map[z, x] += t * t * height / 30f;
                }
            }
        }

        // Добавляем выступы
        int spikesCount = Random.Range(3, 7); // число выступов
        for (int i = 0; i < spikesCount; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float spikeLength = baseRadius * Random.Range(0.5f, 2f);
            float spikeWidth = baseRadius * Random.Range(0.5f, 2f);
            float spikeHeight = 1f;

            if(height >= 5)
            {
                spikeHeight = height * Random.Range(0.5f, 1f);
            }
            else {
               // spikeLength *= 1.5f;
                //spikeWidth *= 1.5f;
            }


            // координаты конца выступа
            int spikeCx = cx + Mathf.RoundToInt(Mathf.Cos(angle) * spikeLength);
            int spikeCz = cz + Mathf.RoundToInt(Mathf.Sin(angle) * spikeLength);

            // проходим по квадрату вокруг выступа
            int minX = Mathf.Clamp(Mathf.Min(cx, spikeCx) - Mathf.RoundToInt(spikeWidth), 0, res - 1);
            int maxX = Mathf.Clamp(Mathf.Max(cx, spikeCx) + Mathf.RoundToInt(spikeWidth), 0, res - 1);
            int minZ = Mathf.Clamp(Mathf.Min(cz, spikeCz) - Mathf.RoundToInt(spikeWidth), 0, res - 1);
            int maxZ = Mathf.Clamp(Mathf.Max(cz, spikeCz) + Mathf.RoundToInt(spikeWidth), 0, res - 1);

            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    // расстояние до линии центра→конец выступа
                    float dx = spikeCx - cx;
                    float dz = spikeCz - cz;
                    float px = x - cx;
                    float pz = z - cz;

                    float proj = (px * dx + pz * dz) / (dx * dx + dz * dz);
                    proj = Mathf.Clamp01(proj);

                    float closestX = cx + proj * dx;
                    float closestZ = cz + proj * dz;
                    float dist = Vector2.Distance(new Vector2(x, z), new Vector2(closestX, closestZ));

                    if (dist < spikeWidth)
                    {
                        float t = 1f - dist / spikeWidth;
                        map[z, x] += t * t * spikeHeight / 30f;
                    }
                }
            }
        }
    }




        void AddPlateuHill(
    float[,] map,
    float terrainHeight,
    float radiusMin,
    float radiusMax,
    float heightMin,
    float heightMax,
    float chainProbability = 0.25f,
    Vector2? forcedCenter = null
)
    {
        int size = map.GetLength(0);

        // --- 1) Центр холма ---
        Vector2 center;
        if (forcedCenter.HasValue)
            center = forcedCenter.Value;
        else
            center = new Vector2(Random.Range(0, size), Random.Range(0, size));

        // --- 2) Эллиптическое растяжение ---
        float radiusX = Random.Range(radiusMin, radiusMax);
        float radiusZ = Random.Range(radiusMin, radiusMax);

        // --- 3) Высота ---
        float hillHeight = Random.Range(heightMin, heightMax);

        // --- 4) Случайная шумовая форма ---
        float noiseScale = Random.Range(0.05f, 0.2f);
        float shapeChaos = Random.Range(0.3f, 0.7f); // насколько "ломаная" форма

        float flatTop = Random.Range(0.2f, 0.3f); // доля плоской вершины

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float nx = (x - center.x) / radiusX;
                float nz = (z - center.y) / radiusZ;

                // Эллиптическое расстояние
                float dist = Mathf.Sqrt(nx * nx + nz * nz);

                if (dist < 1f)
                {
                    // --- 5) Форма-клякса через шум ---
                    float noiseMask = Mathf.PerlinNoise(x * noiseScale, z * noiseScale);
                    noiseMask = Mathf.Lerp(1f - shapeChaos, 1f + shapeChaos, noiseMask);

                    dist /= noiseMask;

                    if (dist < 1f)
                    {
                        float h;

                        // --- 6) Плоская вершина ---
                        if (dist < flatTop)
                        {
                            h = hillHeight;
                        }
                        else
                        {
                            float t = Mathf.InverseLerp(flatTop, 1f, dist);
                            float smooth = 1f - (t * t * (20 - 2 * t)); // smoothstep
                            h = hillHeight * smooth;
                        }

                        // --- 7) Записываем высоту в реальных единицах террейна ---
                        map[x, z] = Mathf.Clamp(map[x, z] + h, 0, terrainHeight);
                    }
                }
            }
        }

        // --- 8) Создание цепочки холмов ---
        if (Random.value < chainProbability)
        {
            // Новый центр смещаем от текущего
            Vector2 newCenter = center + new Vector2(
                Random.Range(radiusMin, radiusMax) ,
                Random.Range(radiusMin, radiusMax) 
            );

            // Рекурсивный вызов (ограниченно)
            AddPlateuHill(
                map, terrainHeight,
                radiusMin, radiusMax,
                heightMin * 0.8f, heightMax * 0.8f,
                chainProbability * 0.8f,
                newCenter
            );
        }
    }




    void ApplyTextures(TerrainData tData)
    {
        int numLayers = 4;

        SplatPrototype[] splats = new SplatPrototype[numLayers];
        splats[0] = new SplatPrototype() { texture = mainTex, tileSize = new Vector2(20, 20) };   // основная трава
        splats[1] = new SplatPrototype() { texture = grassTex, tileSize = new Vector2(15, 15) }; // густая трава
        splats[2] = new SplatPrototype() { texture = dirtTex, tileSize = new Vector2(15, 15) };  // грязь
        splats[3] = new SplatPrototype() { texture = rockTex, tileSize = new Vector2(12, 12) };  // камень

        tData.splatPrototypes = splats;

        int size = tData.alphamapResolution;
        float[,,] map = new float[size, size, numLayers];

        int steps = 3; // пикселизация переходов

        // параметры островков
        float patchScale = 0.07f;
        float grassThreshold = 0.55f;
        float dirtThreshold = 0.68f;

        // новые параметры
        float rockAngle = 30f;        // угол наклона для камней
        float thresholdStep = 0.25f;  // скачок высот
        float lowHillThreshold = 4f;  // минимальная высота, чтобы ступеньку считать грязью

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float normX = (float)x / size;
                float normZ = (float)z / size;

                float h = tData.GetHeight(x, z);
                float steep = tData.GetSteepness(normX, normZ);

                // -----------------------------------------------------
                // вычисление ступеньки (скачок высот)
                // -----------------------------------------------------
                float hL = GetHeightSafe(tData, x - 1, z);
                float hR = GetHeightSafe(tData, x + 1, z);
                float hU = GetHeightSafe(tData, x, z + 1);
                float hD = GetHeightSafe(tData, x, z - 1);

                float stepDelta =
                    (Mathf.Abs(h - hL) +
                     Mathf.Abs(h - hR) +
                     Mathf.Abs(h - hU) +
                     Mathf.Abs(h - hD)) * 0.25f;

                bool isStep = stepDelta > thresholdStep;

                // -----------------------------------------------------
                // островки
                // -----------------------------------------------------
                float noise = Mathf.PerlinNoise(x * patchScale, z * patchScale);
                bool grassPatch = noise > grassThreshold;
                bool dirtPatch = noise > dirtThreshold;

                float[] mix = new float[numLayers];

                // базовая трава
                mix[0] = 1f;

                // -----------------------------------------------------
                // приоритеты
                // -----------------------------------------------------

                // 1) Камень — крутой уклон
                if (steep > rockAngle)
                {
                    mix[3] = 1f;
                }
                // 2) Грязь — резкие ступеньки, но только на больших холмах
                else if (isStep && h > lowHillThreshold)
                {
                    mix[0] = 1f;
                }
                else
                {
                    //// 3) Декоративные островки
                    //if (grassPatch)
                    //    mix[1] = 1f;
                    //if (dirtPatch)
                    //    mix[2] = 1f;
                }

                // -----------------------------------------------------
                // Нормализация + пикселизация
                // -----------------------------------------------------
                float total = mix[0] + mix[1] + mix[2] + mix[3];
                for (int i = 0; i < numLayers; i++)
                {
                    float val = mix[i] / total;
                    val = Mathf.Round(val * steps) / steps;
                    map[z, x, i] = val;
                }
            }
        }

        tData.SetAlphamaps(0, 0, map);
    }


    float GetHeightSafe(TerrainData tData, int x, int z)
    {
        x = Mathf.Clamp(x, 0, tData.heightmapResolution - 1);
        z = Mathf.Clamp(z, 0, tData.heightmapResolution - 1);
        return tData.GetHeight(x, z);
    }


    // Генерация flow map и углубление русел




    void ScatterPrefabs(GameObject prefab, int count, GameObject terrain)
    {
        if (prefab == null) return;

        Terrain t = terrain.GetComponent<Terrain>();

        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(0, terrainSize);
            float z = Random.Range(0, terrainSize);
            float y = t.SampleHeight(new Vector3(x, 0, z));

            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            obj.transform.position = new Vector3(x, y, z);
            obj.transform.SetParent(terrain.transform);
        }
    }

    void GenerateDirtPatches(TerrainData tData, int numPatches, float minHeight, float maxHeight, int dirtLayer = 2)
    {
        int size = tData.alphamapResolution;
        float[,,] alpha = tData.GetAlphamaps(0, 0, size, size);

        for (int p = 0; p < numPatches; p++)
        {
            // ищем центр
            int cx = Random.Range(0, size);
            int cz = Random.Range(0, size);

            // случайный размер пятна
            float radius = Random.Range(10f, 50f);

            // случайные оффсеты шума
            float ox = Random.Range(0f, 999f);
            float oz = Random.Range(0f, 999f);

            for (int x = 0; x < size; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    float dx = x - cx;
                    float dz = z - cz;
                    float dist = Mathf.Sqrt(dx * dx + dz * dz);

                    // базовое затухание по кругу
                    float circleMask = 1f - Mathf.Clamp01(dist / radius);

                    if (circleMask <= 0f)
                        continue;

                    // fbm noise для органичной формы
                    float nx = (x + ox) * 0.05f;
                    float nz = (z + oz) * 0.05f;

                    float noise =
                        Mathf.PerlinNoise(nx, nz) * 0.6f +
                        Mathf.PerlinNoise(nx * 2f, nz * 2f) * 0.3f +
                        Mathf.PerlinNoise(nx * 4f, nz * 4f) * 0.1f;

                    // соединяем круг и шум
                    float mask = circleMask * noise;

                    // плавная граница
                    mask = Mathf.SmoothStep(0.0f, 0.7f, mask);

                    if (mask > 0.05f)
                    {
                        float h = tData.GetHeight(x, z);
                        if (h >= minHeight && h <= maxHeight)
                        {
                            // ставим грязь
                            alpha[z, x, dirtLayer] = Mathf.Clamp01(mask);

                            // убираем траву пропорционально
                            alpha[z, x, 0] *= (1f - mask);
                            alpha[z, x, 1] *= (1f - mask);
                        }
                    }
                }
            }
        }

        // нормализация альфы
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float sum = 0;
                for (int i = 0; i < alpha.GetLength(2); i++)
                    sum += alpha[z, x, i];

                if (sum < 0.001f) continue;

                for (int i = 0; i < alpha.GetLength(2); i++)
                    alpha[z, x, i] /= sum;
            }
        }

        tData.SetAlphamaps(0, 0, alpha);
    }


    void GenerateGrassPatches(TerrainData tData, int numPatches, float minHeight, float maxHeight, GameObject terrain, int textureLayer = 1)
    {
        int size = tData.alphamapResolution;
        float[,,] alpha = tData.GetAlphamaps(0, 0, size, size); // получаем текущие значения всех слоев

        for (int p = 0; p < numPatches; p++)
        {
            // ищем центр пятна в низине
            int cx, cz;
            float h;
            int attempts = 0;
            do
            {
                cx = Random.Range(0, size);
                cz = Random.Range(0, size);
                h = tData.GetHeight(cx, cz);
                attempts++;
                if (attempts > 50) break;
            } while (h < minHeight || h > maxHeight);

            float radius = Random.Range(5f, 20f);

            for (int x = 0; x < size; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    float dx = x - cx;
                    float dz = z - cz;
                    float dist = Mathf.Sqrt(dx * dx + dz * dz);
                    float distortion = Mathf.PerlinNoise(x * 0.1f, z * 0.1f) * radius * 0.3f;

                    if (dist < radius + distortion)
                    {
                        float heightHere = tData.GetHeight(x, z);
                        if (heightHere > minHeight && heightHere <= maxHeight)
                        {
                            float random = Random.value;

                            // устанавливаем значение конкретного слоя текстуры
                            alpha[z, x, textureLayer] = Mathf.Clamp01(random / 3f);

                            // уменьшаем остальные слои, чтобы суммарно 1
                            float sum = 0f;
                            for (int i = 0; i < alpha.GetLength(2); i++)
                                sum += alpha[z, x, i];

                            for (int i = 0; i < alpha.GetLength(2); i++)
                                alpha[z, x, i] /= sum;

                            // спавн префаба травы
                            if (random > 0.9f)
                            {
                                Vector3 pos = new Vector3(x / (float)size * tData.size.x, heightHere, z / (float)size * tData.size.z);
                                GameObject g = (GameObject)PrefabUtility.InstantiatePrefab(grassPrefab, terrain.transform);
                                g.transform.position = pos;
                            }
                        }
                    }
                }
            }
        }

        tData.SetAlphamaps(0, 0, alpha);
    }


}
