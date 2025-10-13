using UnityEngine;

/// <summary>
/// Script de exemplo para demonstrar como configurar as telas do Jogo da Empatia.
/// Este script pode ser usado como referência para configuração no Unity Editor.
/// </summary>
public class GameSetupExample : MonoBehaviour
{
  [Header("Exemplo de Configuração das Telas")]
  [TextArea(10, 20)]
  public string setupInstructions = @"
CONFIGURAÇÃO DAS TELAS NO UNITY - VERSÃO ATUALIZADA:

1. SCREEN CTA (ScreenCta.cs):
   - Screen Name: 'cta'
   - Next Screen Name: 'gameplay'
   - Conectar o botão 'Começar' ao método StartGame()

2. SCREEN GAME (ScreenGame.cs) - NOVO COMPORTAMENTO:
   - Screen Name: 'gameplay'
   - Previous Screen Name: 'cta'
   - Next Screen Name: 'results'
   
   CONFIGURAÇÃO DAS 6 SPRITES (IMPORTANTE!):
   * Round1Image1: Primeira imagem da rodada 1
   * Round1Image2: Segunda imagem da rodada 1  
   * Round2Image1: Primeira imagem da rodada 2
   * Round2Image2: Segunda imagem da rodada 2
   * Round3Image1: Primeira imagem da rodada 3
   * Round3Image2: Segunda imagem da rodada 3
   
   CONFIGURAÇÃO DA UI - DUAS IMAGENS COM FADE:
   * ImageDisplay1: Primeira Image com CanvasGroup para a primeira imagem
   * ImageDisplay1CanvasGroup: CanvasGroup da primeira imagem (para fade effect)
   * ImageDisplay2: Segunda Image com CanvasGroup para a segunda imagem (ao lado)
   * ImageDisplay2CanvasGroup: CanvasGroup da segunda imagem (para fade effect)
   * DescriptionText: TextMeshPro para mostrar descrições ('Observe a situação' -> 'Qual sua opinião?')
   * WordsContainer: Transform onde os botões de palavras serão criados (inicialmente desativado)
   * WordButtonPrefab: Prefab do botão de palavra (deve ter TextMeshPro)
   * ConfirmButton: Botão para confirmar escolhas (inicialmente desativado)
   * FeedbackText: TextMeshPro para mensagem da Aya
   
   COMPORTAMENTO DE CADA RODADA COM FADE:
   1. Primeira imagem aparece com fade in + descrição específica da situação da rodada
   2. Após 10 segundos: segunda imagem aparece ao lado com fade in + 'Qual sua opinião sobre isso?'
   3. Aparece as opções de palavras e botão confirmar
   4. Após confirmação: feedback da Aya e próxima rodada
   
   SITUAÇÕES DAS RODADAS (configuráveis no Inspector):
   - Rodada 1: Em uma reunião online importante com um cliente, Marcos não liga a câmera.
   - Rodada 2: Funcionário chega atrasado e de moletom numa reunião super importante com clientes.
   - Rodada 3: Funcionária toma café com outro colaborador enquanto o resto da equipe trabalha sem parar.

3. SCREEN RESULT (ScreenResult.cs):
   - Screen Name: 'results'
   - Previous Screen Name: 'gameplay'
   - Configurar as referências de UI:
     * EmpathyScoreText: TextMeshPro para pontos de Empatia
     * ActiveListeningScoreText: TextMeshPro para pontos de Escuta Ativa
     * SelfAwarenessScoreText: TextMeshPro para pontos de Autoconsciência
     * FinalMessageText: TextMeshPro para mensagem final da Aya
     * TotalScoreText: TextMeshPro para pontuação total

CONFIGURAÇÕES IMPORTANTES:
- Image Transition Delay: 10 segundos (configurável) - tempo entre primeira e segunda imagem
- Fade Duration: 1 segundo (configurável) - duração do efeito de fade in das imagens
- Initial Description: 'Observe a situação apresentada' (configurável)
- Question Description: 'Qual sua opinião sobre isso?' (configurável)

ESTRUTURA RECOMENDADA NO UNITY:
- Crie dois GameObjects com Image + CanvasGroup para as imagens
- Posicione side-by-side (lado a lado) para aparecer simultaneamente
- CanvasGroups começam com Alpha = 0 (invisíveis)

SISTEMA DE PONTUAÇÃO (inalterado):
- 9-12 pontos: Empatia=9, Escuta Ativa=7, Autoconsciência=5
- 6-8 pontos:  Empatia=8, Escuta Ativa=6, Autoconsciência=4
- 0-5 pontos:  Empatia=7, Escuta Ativa=5, Autoconsciência=3
"; [Tooltip("ScreenManager é uma classe estática - não precisa de referência")]
  // ScreenManager é uma classe estática, então não precisamos de uma instância
  // public ScreenManager screenManager;

  /// <summary>
  /// Método para debug - mostra informações sobre as telas configuradas.
  /// </summary>
  [ContextMenu("Debug Screen Info")]
  public void DebugScreenInfo()
  {
    CanvasScreen[] screens = FindObjectsByType<CanvasScreen>(FindObjectsSortMode.None);

    Debug.Log($"Encontradas {screens.Length} telas no projeto:");

    foreach (CanvasScreen screen in screens)
    {
      Debug.Log($"Tela: {screen.GetType().Name} - Nome: {screen.screenData.screenName} - Ativa: {screen.IsOn()}");
    }
  }
}