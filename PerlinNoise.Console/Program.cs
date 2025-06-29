using System.Drawing;

var perlin = new PerlinNoise.Core.PerlinNoise();
perlin.InitializePermutation(seed: 42);

int width = 256;
int height = 256;
float scale = 0.05f;

float[,] values = new float[height, width];

// Gera valores de ruído e armazena na matriz
for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        float value = perlin.Noise(x * scale, y * scale);
        values[y, x] = value;
    }
}

// Cria imagem em tons de cinza com base nos valores
using Bitmap bitmap = new(width, height);
for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        int gray = (int)(values[y, x] * 255);
        bitmap.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
    }
}

// Salva a imagem
var imageFileName = "perlin_noise.png";
bitmap.Save(imageFileName);
Console.WriteLine("Imagem 'perlin_noise.png' criada com sucesso.");

// Mostra o caminho completo da imagem no console
string fullPath = Path.GetFullPath(imageFileName);
Console.WriteLine($"\nImagem gerada com sucesso!");
Console.WriteLine($"📂 Caminho completo: {fullPath}");
Console.WriteLine($"🔗 Abre no navegador/arquivo: file:///{fullPath.Replace("\\", "/")}");