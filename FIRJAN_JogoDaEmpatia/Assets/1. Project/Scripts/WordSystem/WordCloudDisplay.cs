using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

[System.Serializable]
public class WordDisplaySlot
{
    [Header("TextMeshPro Reference")]
    public TextMeshProUGUI textComponent;

    [Header("Position Info")]
    public string positionName = "";

    [Header("Runtime Data")]
    public string currentWord = "";
    public int currentPoints = 0;
}

public class WordCloudDisplay : MonoBehaviour
{
    [Header("TextMeshPro Slots")]
    public List<WordDisplaySlot> wordSlots = new List<WordDisplaySlot>();

    [Header("Size Settings")]
    public float minFontSize = 20f;
    public float maxFontSize = 80f;

    [Header("Animation Settings")]
    public float animationDuration = 0.5f;

    [Header("Color Settings")]
    public Gradient colorGradient;

    [Header("Auto-Population Settings")]
    [Tooltip("Auto-preencher slots no Start()?")]
    public bool autoPopulateOnStart = true;

    [Tooltip("Mostrar logs detalhados de debug?")]
    public bool showDebugLogs = true;

    private Dictionary<string, int> wordScores = new Dictionary<string, int>();
    public static WordCloudDisplay Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Auto-preencher slots com palavras disponíveis se habilitado
        if (autoPopulateOnStart)
        {
            AutoPopulateWordSlots();
        }

        ClearAllSlots();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void AddWordPoints(string wordText, int points = 1)
    {
        if (string.IsNullOrEmpty(wordText)) return;

        if (wordScores.ContainsKey(wordText))
        {
            wordScores[wordText] += points;
            // Garantir que não fique negativo
            wordScores[wordText] = Mathf.Max(0, wordScores[wordText]);
        }
        else
        {
            // Garantir que valores iniciais negativos virem 0
            wordScores[wordText] = Mathf.Max(0, points);
        }

        if (showDebugLogs)
        {
            Debug.Log($"[WordCloudDisplay] '{wordText}' now has {wordScores[wordText]} points");
        }
        UpdateWordCloudDisplay();
    }

    private void UpdateWordCloudDisplay()
    {
        var sortedWords = wordScores.OrderByDescending(w => w.Value)
                                   .ThenBy(w => w.Key)
                                   .ToList();

        Debug.Log($"[WordCloudDisplay] Updating display with {sortedWords.Count} words");

        for (int i = 0; i < sortedWords.Count && i < wordSlots.Count; i++)
        {
            var word = sortedWords[i];
            var slot = wordSlots[i];
            UpdateSlot(slot, word.Key, word.Value, sortedWords);
        }

        for (int i = sortedWords.Count; i < wordSlots.Count; i++)
        {
            ClearSlot(wordSlots[i]);
        }
    }

    private void UpdateSlot(WordDisplaySlot slot, string wordText, int points, List<KeyValuePair<string, int>> allWords)
    {
        if (slot.textComponent == null) return;

        bool textChanged = slot.currentWord != wordText;
        bool scoreChanged = slot.currentPoints != points;

        if (textChanged || scoreChanged)
        {
            if (textChanged && !string.IsNullOrEmpty(slot.currentWord))
            {
                AnimateSlotChange(slot, wordText, points, allWords);
            }
            else
            {
                SetSlotImmediate(slot, wordText, points, allWords);
            }
        }
    }

    private void AnimateSlotChange(WordDisplaySlot slot, string newWord, int newPoints, List<KeyValuePair<string, int>> allWords)
    {
        if (slot.textComponent == null) return;

        Debug.Log($"[WordCloudDisplay] Animating change in '{slot.positionName}': '{slot.currentWord}' -> '{newWord}'");

        slot.textComponent.DOFade(0f, animationDuration * 0.3f)
            .OnComplete(() =>
            {
                SetSlotImmediate(slot, newWord, newPoints, allWords);
                slot.textComponent.DOFade(1f, animationDuration * 0.7f);
                // Removido DOPunchScale para manter escala sempre em (1,1,1)
            });
    }

    private void SetSlotImmediate(WordDisplaySlot slot, string wordText, int points, List<KeyValuePair<string, int>> allWords)
    {
        if (slot.textComponent == null) return;

        slot.currentWord = wordText;
        slot.currentPoints = points;
        slot.textComponent.text = wordText;
        ApplyWordStyle(slot, points, allWords);
    }

    private void ApplyWordStyle(WordDisplaySlot slot, int points, List<KeyValuePair<string, int>> allWords)
    {
        if (slot.textComponent == null || allWords.Count == 0) return;

        int minPoints = allWords.Min(w => w.Value);
        int maxPoints = allWords.Max(w => w.Value);

        float normalizedScore;
        if (maxPoints > minPoints)
        {
            normalizedScore = (float)(points - minPoints) / (maxPoints - minPoints);
        }
        else
        {
            normalizedScore = 0.5f;
        }

        normalizedScore = Mathf.Max(normalizedScore, 0.2f);

        float fontSize = Mathf.Lerp(minFontSize, maxFontSize, normalizedScore);
        slot.textComponent.fontSize = fontSize;

        if (colorGradient != null && colorGradient.colorKeys.Length > 0)
        {
            Color color = colorGradient.Evaluate(normalizedScore);
            slot.textComponent.color = color;
        }

        Debug.Log($"[WordCloudDisplay] '{slot.currentWord}' -> {points} pts, normalized: {normalizedScore:F2}, fontSize: {fontSize:F1}");
    }

    private void ClearSlot(WordDisplaySlot slot)
    {
        if (slot.textComponent == null) return;

        if (!string.IsNullOrEmpty(slot.currentWord))
        {
            slot.currentWord = "";
            slot.currentPoints = 0;
            slot.textComponent.text = "";
        }
    }

    private void ClearAllSlots()
    {
        foreach (var slot in wordSlots)
        {
            ClearSlot(slot);
        }

        wordScores.Clear();
        Debug.Log("[WordCloudDisplay] All slots cleared");
    }

    public void ResetAllScores()
    {
        wordScores.Clear();
        ClearAllSlots();
        Debug.Log("[WordCloudDisplay] All scores reset");
    }

    public int GetWordScore(string wordText)
    {
        return wordScores.ContainsKey(wordText) ? wordScores[wordText] : 0;
    }

    public Dictionary<string, int> GetAllWordScores()
    {
        return new Dictionary<string, int>(wordScores);
    }

    /// <summary>
    /// Auto-preenche os wordSlots com todas as palavras disponíveis no jogo.
    /// </summary>
    private void AutoPopulateWordSlots()
    {
        // Coletar todas as palavras únicas baseadas no jogo
        HashSet<string> allWords = new HashSet<string>();

        // Lista completa de todas as palavras do jogo (baseado no ScreenGame)
        string[] allGameWords = {
            // Rodada 1
            "Adaptação", "Desengajado", "Resolução de problema", "Falta de profissionalismo",
            "Compromisso", "Falta de comunicação", "Respeito ao cliente", "Negligente",
            // Rodada 2
            "Desleixo", "Resiliência", "Falta de respeito", "Amadorismo",
            "Prioridade", "Falta de atenção",
            // Rodada 3
            "Improdutividade", "Distração", "Resolução de problemas", "Colaboração",
            "Desorganização", "Descomprometimento", "Estratégia", "Parceria"
        };

        // Adicionar todas as palavras (removendo duplicatas automaticamente pelo HashSet)
        foreach (string word in allGameWords)
        {
            if (!string.IsNullOrEmpty(word))
            {
                allWords.Add(word);
            }
        }

        // Limpar slots existentes
        wordSlots.Clear();

        // Criar slots para cada palavra única
        foreach (string word in allWords.OrderBy(w => w))
        {
            WordDisplaySlot newSlot = new WordDisplaySlot();
            newSlot.positionName = $"Slot_{word}";
            newSlot.currentWord = "";
            newSlot.currentPoints = 0;
            // textComponent será configurado manualmente no Inspector

            wordSlots.Add(newSlot);
        }

        if (showDebugLogs)
        {
            Debug.Log($"[WordCloudDisplay] Auto-populado {wordSlots.Count} slots com palavras disponíveis");

            // Log das palavras encontradas
            foreach (var slot in wordSlots)
            {
                Debug.Log($"  - Slot criado: {slot.positionName}");
            }
        }
    }

    [ContextMenu("Auto Populate Word Slots")]
    public void ManualAutoPopulate()
    {
        AutoPopulateWordSlots();
        Debug.Log("[WordCloudDisplay] Slots auto-populados manualmente via Context Menu");

#if UNITY_EDITOR
        // Marcar como "dirty" para salvar no Editor
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    /// <summary>
    /// Método público para forçar a auto-população (útil para chamar do Inspector ou outros scripts).
    /// </summary>
    public void ForceAutoPopulate()
    {
        AutoPopulateWordSlots();
        Debug.Log("[WordCloudDisplay] Slots auto-populados via método público");
    }

    [ContextMenu("Debug Word Slots")]
    public void DebugWordSlots()
    {
        Debug.Log("=== WORD CLOUD DEBUG ===");
        Debug.Log($"Total Slots: {wordSlots.Count}");

        for (int i = 0; i < wordSlots.Count; i++)
        {
            var slot = wordSlots[i];
            var hasComponent = slot.textComponent != null;
            Debug.Log($"Slot {i}: '{slot.positionName}' | Component: {hasComponent} | Word: '{slot.currentWord}' | Points: {slot.currentPoints}");
        }

        Debug.Log($"Total Words Tracked: {wordScores.Count}");
        foreach (var word in wordScores.OrderByDescending(w => w.Value))
        {
            Debug.Log($"  '{word.Key}': {word.Value} points");
        }
        Debug.Log("========================");
    }

    [ContextMenu("Test Add Word Points")]
    public void TestAddWordPoints()
    {
        string[] testWords = { "Empatia", "Colaboração", "Respeito", "Comunicação" };

        foreach (string word in testWords)
        {
            AddWordPoints(word, Random.Range(1, 5));
        }
    }

    [ContextMenu("Reset All Scores")]
    public void DebugResetScores()
    {
        ResetAllScores();
    }

    #region Public Utility Methods

    /// <summary>
    /// Obtém as palavras mais pontuadas em ordem decrescente.
    /// </summary>
    /// <param name="topCount">Número de palavras para retornar (padrão: 5)</param>
    /// <returns>Lista das palavras mais pontuadas</returns>
    public List<KeyValuePair<string, int>> GetTopWords(int topCount = 5)
    {
        return wordScores.OrderByDescending(w => w.Value)
                        .Take(topCount)
                        .ToList();
    }

    /// <summary>
    /// Obtém a palavra com maior pontuação.
    /// </summary>
    /// <returns>Palavra com maior pontuação ou null se não há palavras</returns>
    public string GetTopWord()
    {
        if (wordScores.Count == 0) return null;

        return wordScores.OrderByDescending(w => w.Value)
                        .First()
                        .Key;
    }

    /// <summary>
    /// Força a atualização visual da nuvem de palavras.
    /// </summary>
    public void ForceUpdateDisplay()
    {
        UpdateWordCloudDisplay();
    }

    /// <summary>
    /// Obtém o número total de palavras com pontuação.
    /// </summary>
    public int GetTrackedWordsCount()
    {
        return wordScores.Count;
    }

    /// <summary>
    /// Verifica se uma palavra específica está sendo rastreada.
    /// </summary>
    /// <param name="wordText">Texto da palavra</param>
    /// <returns>True se a palavra está sendo rastreada</returns>
    public bool IsWordTracked(string wordText)
    {
        return wordScores.ContainsKey(wordText);
    }

    #endregion
}