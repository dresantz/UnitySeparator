using UnityEditor;
using UnityEngine;

// Componente para configurar o separador
[ExecuteInEditMode]
public class Separator : MonoBehaviour
{
    [Header("Separator Settings")]
    public Color backgroundColor = new Color(0.72f, 0.72f, 0.72f, 1f);
    public float cornerRadius = 4f;
    public bool showIcon = true;

    private void OnValidate()
    {
        // Ajusta o nome automaticamente
        // gameObject.name.ToUpper, caso queira que fique maiúsculo automático
        gameObject.name = $"{gameObject.name}";
    }
}

// Gerenciador para customizar a Hierarchy
[InitializeOnLoad]
public static class HierarchySeparatorManager
{
    private static Texture2D icon;

    static HierarchySeparatorManager()
    {
        // Carrega o ícone para os separadores
        icon = EditorGUIUtility.IconContent("d_FilterByLabel").image as Texture2D;

        // Registra o evento para customizar a Hierarchy
        EditorApplication.hierarchyWindowItemOnGUI += CustomizeHierarchy;
    }

    private static void CustomizeHierarchy(int instanceID, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        // Verifica se o objeto possui o componente Separator
        if (obj == null) return;
        Separator config = obj.GetComponent<Separator>();
        if (config == null) return;

        // Desenha o fundo arredondado com a cor configurada
        DrawRoundedRect(selectionRect, config.backgroundColor, config.cornerRadius);

        // Adiciona texto estilizado
        EditorGUI.LabelField(selectionRect, obj.name, new GUIStyle
        {
            fontStyle = FontStyle.Bold,
            normal = new GUIStyleState { textColor = Color.black },
            // Muda o alinhamento do texto
            alignment = TextAnchor.MiddleCenter
        });

        // Se a opção showIcon estiver ativada, desenha o ícone
        if (config.showIcon)
        {
            Rect iconRect = new Rect(selectionRect.x + selectionRect.width - 25, selectionRect.y, 20, 18);
            GUI.Label(iconRect, new GUIContent(icon));
        }
    }

    // Desenha um retângulo com bordas arredondadas
    private static void DrawRoundedRect(Rect rect, Color color, float radius)
    {
        // Define a cor para o desenho
        GUI.color = color;

        // Partes centrais (horizontal e vertical)
        EditorGUI.DrawRect(new Rect(rect.x + radius, rect.y, rect.width - 2 * radius, rect.height), color); // Central Horizontal
        EditorGUI.DrawRect(new Rect(rect.x, rect.y + radius, rect.width, rect.height - 2 * radius), color); // Central Vertical

        // Círculos nos cantos
        DrawCircle(new Rect(rect.x, rect.y, radius * 2, radius * 2), color); // Top Left
        DrawCircle(new Rect(rect.x + rect.width - radius * 2, rect.y, radius * 2, radius * 2), color); // Top Right
        DrawCircle(new Rect(rect.x, rect.y + rect.height - radius * 2, radius * 2, radius * 2), color); // Bottom Left
        DrawCircle(new Rect(rect.x + rect.width - radius * 2, rect.y + rect.height - radius * 2, radius * 2, radius * 2), color); // Bottom Right

        // Restaura a cor padrão
        GUI.color = Color.white;
    }

    // Desenha um círculo em uma área específica
    private static void DrawCircle(Rect rect, Color color)
    {
        Texture2D circle = MakeCircleTexture(color);
        GUI.DrawTexture(rect, circle);
    }

    // Cria uma textura circular
    private static Texture2D MakeCircleTexture(Color color)
    {
        int size = 32; // Tamanho da textura
        Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);

        // Preenche a textura com um círculo
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(size / 2, size / 2));
                texture.SetPixel(x, y, distance <= size / 2 ? color : Color.clear);
            }
        }
        texture.Apply();
        return texture;
    }
}
