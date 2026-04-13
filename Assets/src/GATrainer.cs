using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Data;

[System.Serializable]
public class BotDNA {
    public float[] weights = new float[15];
    public float fitness = 0;

    public BotDNA() {}

    public void InitializeRandom() {
        // Cấp cho thế hệ đầu tiên tỷ lệ vàng gần giống cờ vua tiêu chuẩn
        weights[0] = Random.Range(80f, 150f);   // Tốt thường
        weights[1] = Random.Range(280f, 350f);  // Tượng/Mã
        weights[2] = Random.Range(480f, 550f);  // Xe
        weights[3] = Random.Range(850f, 1000f); // 🚨 Hậu (Bắt buộc khởi điểm cao)
        weights[4] = Random.Range(300f, 400f);  // Tốt tiến hóa
        weights[5] = Random.Range(800f, 1000f); // Tốt tiến hóa xịn

        // Các trọng số chiến thuật (từ index 6 trở đi) vẫn để ngẫu nhiên
        for (int i = 6; i < weights.Length; i++) {
            weights[i] = Random.Range(10f, 50f); 
        }
    }

    // Đột biến (Mutate)
    public void Mutate(float mutationRate) {
        for (int i = 0; i < weights.Length; i++) {
            if (Random.value < mutationRate) {
                // Đột biến theo tỷ lệ phần trăm (thay đổi nhẹ +/- 15% để tránh phá hỏng gen tốt)
                float change = weights[i] * Random.Range(-0.15f, 0.15f); 
                weights[i] += change;

                // Khóa chặn: Riêng Gen số 3 (Hậu) và 5 (Tốt xịn) không bao giờ cho rớt dưới 800
                if (i == 3 || i == 5) {
                    weights[i] = Mathf.Clamp(weights[i], 800f, 2000f);
                } else {
                    weights[i] = Mathf.Clamp(weights[i], 1f, 2000f); 
                }
            }
        }
    }

    // Lai ghép (Uniform Crossover)
    public BotDNA Crossover(BotDNA partner) {
        BotDNA child = new BotDNA();
        for (int i = 0; i < weights.Length; i++) {
            child.weights[i] = Random.value > 0.5f ? this.weights[i] : partner.weights[i];
        }
        return child;
    }
}


public class GATrainer : MonoBehaviour {
    public static GATrainer instance;

    [Header("Cấu hình GA")]
    public bool isTraining = false;
    public int populationSize = 10;
    public float mutationRate = 0.1f;
    public int eliteCount = 2; // Số lượng siêu elit được giữ nguyên sang đời sau
    
    [Header("Thông tin đang chạy")]
    public int currentGeneration = 1;
    public int currentMatchIndex = 0;
    
    [HideInInspector]
    public List<BotDNA> population = new List<BotDNA>();
    
    [Header("Training Mode (3 hoặc 4)")]
    public int playersPerMatch = 4;
    public BotDNA[] currentDNAs;

    string savePath;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.dataPath + "/BestBotBrain.json";
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        if (!isTraining)
        {
            LoadPvEBrain();
            return;
        }

        QualitySettings.vSyncCount = 0;  
        Application.targetFrameRate = -1;
        
        LoadPopulation(); // Thử load não cũ nếu có
        if (population.Count < populationSize) {
            population.Clear();
            for (int i = 0; i < populationSize; i++)
            {
                BotDNA newBot = new BotDNA();
                newBot.InitializeRandom();
                population.Add(newBot);
            } 
        }
        
        Time.timeScale = 20f; 
        StartNextMatch();
    }

    public void StartNextMatch() {
        playersPerMatch = Mathf.Clamp(playersPerMatch, 2, 4);

        if (currentMatchIndex >= populationSize / playersPerMatch) {
            EvolveNextGeneration();
            currentMatchIndex = 0;
        }

        currentDNAs = new BotDNA[playersPerMatch];
        for(int i = 0; i < playersPerMatch; i++) {
            currentDNAs[i] = population[currentMatchIndex * playersPerMatch + i];
        }

        SceneManager.LoadScene("main_entry");
    }

    public void ReportMatchResult(int loserColor, bool isDraw, int totalTurns, float[] materialScores) {
        
        // 1. Phân bổ điểm sinh tồn (Rank)
        // Nếu hòa, mọi người đều nhận điểm hòa (5).
        if (isDraw) {
            for (int i = 0; i < playersPerMatch; i++) {
                currentDNAs[i].fitness += 5f;
            }
        } 
        // Nếu có người thua, người sống sót cuối cùng ăn trọn điểm (20), người chết được điểm an ủi (2).
        else {
            for (int i = 0; i < playersPerMatch; i++) {
                if (i == loserColor) {
                    currentDNAs[i].fitness += 2f; // Kẻ thua cuộc (Chết)
                } else {
                    currentDNAs[i].fitness += 20f; // Kẻ còn sống
                    currentDNAs[i].fitness += 70f;
                }
            }
        }

        // 2. Điểm phần thưởng chênh lệch lực lượng (Càng cắn được nhiều máu địch càng tốt)
        float totalEnemyMaterial = 0;
        for (int i = 0; i < playersPerMatch; i++) totalEnemyMaterial += materialScores[i];

        for (int i = 0; i < playersPerMatch; i++) {
            // Điểm của người hiện tại so với TỔNG lực lượng của các kẻ địch
            float myMaterial = materialScores[i];
            float enemyMaterial = totalEnemyMaterial - myMaterial;
            currentDNAs[i].fitness += (myMaterial - (enemyMaterial / (playersPerMatch - 1))) * 0.05f;
        }

        // 3. Phạt câu giờ
        float turnPenalty = totalTurns * 0.05f;
        for (int i = 0; i < playersPerMatch; i++) {
             currentDNAs[i].fitness -= turnPenalty;
        }

        currentMatchIndex++;
        StartNextMatch();
    }

    void EvolveNextGeneration() {
        // Sắp xếp theo Fitness giảm dần
        population.Sort((a, b) => b.fitness.CompareTo(a.fitness));
        Debug.Log($"<color=cyan>Xong Thế hệ {currentGeneration}! Hậu Bot xịn nhất giá: {population[0].weights[3]} | Fitness: {population[0].fitness}</color>");

        SavePopulation(); // Lưu lại lứa tốt nhất

        List<BotDNA> newPop = new List<BotDNA>();
        
        // 1. Giữ lại tinh hoa (Elitism) - Chuyển thẳng sang lứa sau không lai tạp
        for (int i = 0; i < eliteCount; i++) {
            BotDNA elite = new BotDNA();
            System.Array.Copy(population[i].weights, elite.weights, 15); // Copy value, not reference
            newPop.Add(elite);
        }

        // 2. Lai ghép phần còn lại bằng Tournament Selection
        for (int i = eliteCount; i < populationSize; i++) {
            BotDNA parentA = TournamentSelection();
            BotDNA parentB = TournamentSelection();
            BotDNA child = parentA.Crossover(parentB);
            child.Mutate(mutationRate);
            newPop.Add(child);
        }

        population = newPop;
        foreach (var dna in population) dna.fitness = 0; 
        currentGeneration++;
    }

    // Chọn ngẫu nhiên 3 con, lấy con giỏi nhất trong 3 con đó (Giữ đa dạng gen)
    BotDNA TournamentSelection() {
        int tournamentSize = 3;
        BotDNA best = null;
        for (int i = 0; i < tournamentSize; i++) {
            BotDNA randomBot = population[Random.Range(0, populationSize)];
            if (best == null || randomBot.fitness > best.fitness) {
                best = randomBot;
            }
        }
        return best;
    }

    // Lưu/Tải File JSON
    void SavePopulation() {
        GAPopulationData data = new GAPopulationData { generation = currentGeneration, dnaList = population };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    void LoadPopulation() {
        if (File.Exists(savePath)) {
            string json = File.ReadAllText(savePath);
            GAPopulationData data = JsonUtility.FromJson<GAPopulationData>(json);
            population = data.dnaList;
            currentGeneration = data.generation;
            if (GATrainer.instance == null || !GATrainer.instance.isTraining)
                Debug.Log($"<color=green>Đã load não bộ từ Thế hệ {currentGeneration}</color>");
        }
    }

    void LoadPvEBrain() {
        if (File.Exists(savePath)) {
            string json = File.ReadAllText(savePath);
            
            GAPopulationData fileData = JsonUtility.FromJson<GAPopulationData>(json);
            
            if (fileData != null && fileData.dnaList.Count > 0) {
                data.mem.pveBrains = fileData.dnaList; 
                
                if (GATrainer.instance == null || !GATrainer.instance.isTraining)
                    Debug.Log($"<color=green>Đã nạp thành công {data.mem.pveBrains.Count} bộ não Siêu Trí Tuệ vào Bot!</color>");
            }
        } else {
            if (GATrainer.instance == null || !GATrainer.instance.isTraining)
                Debug.LogWarning("Chưa có file BestBotBrain.json! Hãy bật Is Training để train trước.");
        }
    }
}