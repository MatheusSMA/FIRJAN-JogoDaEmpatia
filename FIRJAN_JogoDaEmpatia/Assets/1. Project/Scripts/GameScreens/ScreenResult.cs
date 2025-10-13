using UnityEngine;
using TMPro;

/// <summary>
/// Script para gerenciar a tela de resultados do Jogo da Empatia.
/// Exibe a pontuação final e as habilidades desenvolvidas.
/// </summary>
public class ScreenResult : CanvasScreen
{
    [Header("Result UI References")]
    [SerializeField] private TextMeshProUGUI empathyScoreText;
    [SerializeField] private TextMeshProUGUI activeListeningScoreText;
    [SerializeField] private TextMeshProUGUI selfAwarenessScoreText;
    [SerializeField] private TextMeshProUGUI finalMessageText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI topWordsText;

    [Header("Score Display Settings")]
    [SerializeField] private string empathyFormat = "Empatia: {0} pontos";
    [SerializeField] private string activeListeningFormat = "Escuta Ativa: {0} pontos";
    [SerializeField] private string selfAwarenessFormat = "Autoconsciência: {0} pontos";
    [SerializeField] private string totalScoreFormat = "Pontuação Total: {0}/12";


    #region Public Methods

    /// <summary>
    /// Método público para exibir os resultados finais.
    /// Chamado pelo ScreenGame.cs ao finalizar o jogo.
    /// </summary>
    /// <param name="finalScore">Pontuação final obtida no jogo (0-12)</param>
    public void ShowFinalResults(int finalScore)
    {
        // Calcula os pontos de cada habilidade baseado na pontuação final
        SkillScores scores = CalculateSkillScores(finalScore);

        // Exibe os pontos de cada habilidade
        DisplaySkillScores(scores);

        // Exibe a pontuação total
        DisplayTotalScore(finalScore);

        // Exibe a mensagem final da Aya
        DisplayFinalMessage();

        // Atualiza a nuvem de palavras com as pontuações das habilidades
        UpdateWordCloudWithSkillScores(scores);

        // Exibe as palavras mais pontuadas
        DisplayTopWords();

        // Envia dados para o sistema NFC
        SubmitToNFCSystem(scores);

        // Log para debug
        Debug.Log($"Resultados finais - Pontuação: {finalScore}, " +
                 $"Empatia: {scores.empathy}, " +
                 $"Escuta Ativa: {scores.activeListening}, " +
                 $"Autoconsciência: {scores.selfAwareness}");
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Estrutura para armazenar as pontuações das habilidades.
    /// </summary>
    private struct SkillScores
    {
        public int empathy;
        public int activeListening;
        public int selfAwareness;

        public SkillScores(int empathy, int activeListening, int selfAwareness)
        {
            this.empathy = empathy;
            this.activeListening = activeListening;
            this.selfAwareness = selfAwareness;
        }
    }

    /// <summary>
    /// Calcula os pontos de cada habilidade baseado na pontuação final.
    /// </summary>
    /// <param name="finalScore">Pontuação final (0-12)</param>
    /// <returns>Estrutura com os pontos de cada habilidade</returns>
    private SkillScores CalculateSkillScores(int finalScore)
    {
        if (finalScore >= 9 && finalScore <= 12)
        {
            // Pontuação alta: 9-12 pontos
            return new SkillScores(
                empathy: 9,
                activeListening: 7,
                selfAwareness: 5
            );
        }
        else if (finalScore >= 6 && finalScore <= 8)
        {
            // Pontuação média: 6-8 pontos
            return new SkillScores(
                empathy: 8,
                activeListening: 6,
                selfAwareness: 4
            );
        }
        else
        {
            // Pontuação baixa: 5 ou menos pontos
            return new SkillScores(
                empathy: 7,
                activeListening: 5,
                selfAwareness: 3
            );
        }
    }

    /// <summary>
    /// Exibe os pontos de cada habilidade na UI.
    /// </summary>
    /// <param name="scores">Pontuações das habilidades</param>
    private void DisplaySkillScores(SkillScores scores)
    {
        // Exibe pontuação de Empatia
        if (empathyScoreText != null)
        {
            empathyScoreText.text = string.Format(empathyFormat, scores.empathy);
        }

        // Exibe pontuação de Escuta Ativa
        if (activeListeningScoreText != null)
        {
            activeListeningScoreText.text = string.Format(activeListeningFormat, scores.activeListening);
        }

        // Exibe pontuação de Autoconsciência
        if (selfAwarenessScoreText != null)
        {
            selfAwarenessScoreText.text = string.Format(selfAwarenessFormat, scores.selfAwareness);
        }
    }

    /// <summary>
    /// Exibe a pontuação total na UI.
    /// </summary>
    /// <param name="finalScore">Pontuação final</param>
    private void DisplayTotalScore(int finalScore)
    {
        if (totalScoreText != null)
        {
            totalScoreText.text = string.Format(totalScoreFormat, finalScore);
        }
    }

    /// <summary>
    /// Exibe a mensagem final da Aya.
    /// </summary>
    private void DisplayFinalMessage()
    {
        if (finalMessageText != null)
        {
            finalMessageText.text = "Candidato, Empatia não resolve todos os problemas, mas ela muda a forma como você os enfrenta. " +
                                   "Cada decisão contribuiu para fortalecer suas habilidades de Empatia, Escuta Ativa e Autoconsciência, " +
                                   "que foram pontuadas ao longo desta experiência.";
        }
    }

    /// <summary>
    /// Exibe as palavras mais pontuadas da nuvem de palavras.
    /// </summary>
    private void DisplayTopWords()
    {
        if (topWordsText != null && WordCloudDisplay.Instance != null)
        {
            var topWords = WordCloudDisplay.Instance.GetTopWords(5);

            if (topWords.Count > 0)
            {
                string topWordsMessage = "Palavras mais selecionadas:\n";

                for (int i = 0; i < topWords.Count; i++)
                {
                    var word = topWords[i];
                    topWordsMessage += $"{i + 1}. {word.Key}: {word.Value} pontos\n";
                }

                topWordsText.text = topWordsMessage.TrimEnd('\n');
            }
            else
            {
                topWordsText.text = "Nenhuma palavra pontuada ainda.";
            }
        }
    }

    #endregion

    #region Public Utility Methods

    /// <summary>
    /// Método para reiniciar o jogo (pode ser chamado por um botão "Jogar Novamente").
    /// </summary>
    public void RestartGame()
    {
        ScreenManager.SetCallScreen("cta");
    }

    /// <summary>
    /// Método para voltar ao menu principal (pode ser usado por um botão "Menu").
    /// </summary>
    public void GoToMainMenu()
    {
        ScreenManager.SetCallScreen("cta");
    }

    #endregion

    #region Word Cloud Integration

    /// <summary>
    /// Atualiza a nuvem de palavras com as pontuações das habilidades.
    /// </summary>
    /// <param name="scores">Pontuações das habilidades</param>
    private void UpdateWordCloudWithSkillScores(SkillScores scores)
    {
        if (WordCloudDisplay.Instance != null)
        {
            // Adiciona pontos às palavras-chave de cada habilidade na nuvem de palavras

            // Empatia - palavras relacionadas
            WordCloudDisplay.Instance.AddWordPoints("Empatia", scores.empathy);
            WordCloudDisplay.Instance.AddWordPoints("Respeito ao cliente", scores.empathy);
            WordCloudDisplay.Instance.AddWordPoints("Compromisso", scores.empathy);

            // Escuta Ativa - palavras relacionadas  
            WordCloudDisplay.Instance.AddWordPoints("Colaboração", scores.activeListening);
            WordCloudDisplay.Instance.AddWordPoints("Parceria", scores.activeListening);
            WordCloudDisplay.Instance.AddWordPoints("Resolução de problemas", scores.activeListening);

            // Autoconsciência - palavras relacionadas
            WordCloudDisplay.Instance.AddWordPoints("Adaptação", scores.selfAwareness);
            WordCloudDisplay.Instance.AddWordPoints("Resiliência", scores.selfAwareness);
            WordCloudDisplay.Instance.AddWordPoints("Estratégia", scores.selfAwareness);

            Debug.Log("[ScreenResult] Pontuações das habilidades adicionadas à nuvem de palavras");
        }
        else
        {
            Debug.LogWarning("[ScreenResult] WordCloudDisplay.Instance não encontrado para atualizar pontuações");
        }
    }

    #endregion

    #region NFC Integration

    /// <summary>
    /// Envia dados das habilidades para o sistema NFC.
    /// </summary>
    /// <param name="scores">Pontuações das habilidades</param>
    private void SubmitToNFCSystem(SkillScores scores)
    {
        if (NFCGameService.Instance != null)
        {
            NFCGameService.Instance.SubmitGameResult(scores.empathy, scores.activeListening, scores.selfAwareness);
            Debug.Log("[ScreenResult] Dados enviados para sistema NFC");
        }
        else
        {
            Debug.LogWarning("[ScreenResult] NFCGameService não encontrado");
        }
    }

    #endregion

    #region Debug Methods

    [ContextMenu("Test Show Results")]
    public void TestShowResults()
    {
        ShowFinalResults(10); // Simula uma pontuação alta para teste
    }

    [ContextMenu("Test Show Results - Low Score")]
    public void TestShowResultsLowScore()
    {
        ShowFinalResults(3); // Simula uma pontuação baixa para teste
    }

    [ContextMenu("Test NFC Submit")]
    public void TestNFCSubmit()
    {
        SkillScores testScores = new SkillScores(9, 7, 5);
        SubmitToNFCSystem(testScores);
    }

    #endregion
}