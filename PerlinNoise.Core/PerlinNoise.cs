// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace PerlinNoise.Core;

/// <summary>
/// Gera valores de ruído Perlin 2D com uma semente.
/// Suporta controle de amplitude e frequência para composição de ruído fractal.
/// </summary>
public class PerlinNoise
{
    private int[]? _permutation;

    public void InitializePermutation(int seed)
    {
        Random random = new Random(seed);

        _permutation = new int[512];
        var basePermutation = new int[256];

        for (int i = 0; i < 256; i++)
        {
            basePermutation[i] = i;
        }

        for (int i = 0; i < 256; i++)
        {
            int j = random.Next(256);
            (basePermutation[i], basePermutation[j]) = (basePermutation[j], basePermutation[i]);
        }

        for (int i = 0; i < 512; i++)
        {
            _permutation[i] = basePermutation[i % 256];
        }
    }

    /// <summary>
    /// Gera ruído Perlin com freqüência e amplitude configuráveis.
    /// </summary>
    public float Noise(float x, float y, float frequency = 1f, float amplitude = 1f)
    {
        ArgumentNullException.ThrowIfNull(_permutation);

        x *= frequency;
        y *= frequency;

        int gridX = (int)MathF.Floor(x) & 255;
        int gridY = (int)MathF.Floor(y) & 255;

        float localX = x - MathF.Floor(x);
        float localY = y - MathF.Floor(y);

        float fadedX = Fade(localX);
        float fadedY = Fade(localY);

        int hashTopLeft = _permutation[_permutation[gridX] + gridY];
        int hashBottomLeft = _permutation[_permutation[gridX] + gridY + 1];
        int hashTopRight = _permutation[_permutation[gridX + 1] + gridY];
        int hashBottomRight = _permutation[_permutation[gridX + 1] + gridY + 1];

        float gradTopLeft = Grad(hashTopLeft, localX, localY);
        float gradTopRight = Grad(hashTopRight, localX - 1, localY);
        float gradBottomLeft = Grad(hashBottomLeft, localX, localY - 1);
        float gradBottomRight = Grad(hashBottomRight, localX - 1, localY - 1);

        float interpTop = Lerp(gradTopLeft, gradTopRight, fadedX);
        float interpBottom = Lerp(gradBottomLeft, gradBottomRight, fadedX);

        float result = Lerp(interpTop, interpBottom, fadedY);

        return ((result + 1) / 2) * amplitude;
    }

    private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);
    private static float Lerp(float a, float b, float t) => a + t * (b - a);
    private static float Grad(int hash, float x, float y)
    {
        int h = hash & 0x3F;
        float u = h < 4 ? x : y;
        float v = h < 4 ? y : x;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }
}
