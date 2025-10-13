using UnityEngine;

/// <summary>
/// Script para gerenciar a tela inicial (CTA - Call to Action) do Jogo da Empatia.
/// Herda de CanvasScreen e fornece funcionalidade para iniciar o jogo.
/// </summary>
public class ScreenCta : CanvasScreen
{
    [Header("CTA Screen Settings")]
    [SerializeField] private string gameScreenName = "gameplay";

    /// <summary>
    /// Método público chamado pelo botão "Começar" na interface.
    /// Desativa a tela CTA atual e ativa a tela do jogo.
    /// </summary>
    public void StartGame()
    {
        // Chama o método do ScreenManager para ativar a tela do jogo
        ScreenManager.SetCallScreen(gameScreenName);
    }
}