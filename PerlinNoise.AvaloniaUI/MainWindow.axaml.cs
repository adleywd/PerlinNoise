using System;
using System.Buffers;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace PerlinNoise.AvaloniaUI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnGenerateNoise(object? sender, RoutedEventArgs e)
    {
        const int imageWidth = 128;
        const int imageHeight = 128;

        // 1. Define seed (aleatória ou do input do usuário)
        int seed = new Random().Next(1, 999999);
        var isGenerateRandomSeedChecked = this.FindControl<CheckBox>("GenerateRandomSeed")?.IsChecked ?? false;

        if (!isGenerateRandomSeedChecked &&
            this.FindControl<TextBox>("SeedInput") is { } seedInput &&
            int.TryParse(seedInput.Text, out int userSeed))
        {
            seed = userSeed;
        }

        // Mostra semente usada
        this.FindControl<TextBlock>("UsedSeed")!.Text = $"Seed used: {seed}";

        // 2. Obtém os valores dos sliders
        float scale = (float)(this.FindControl<Slider>("ScaleSlider")?.Value ?? 0.05);
        float frequency = (float)(this.FindControl<Slider>("FrequencySlider")?.Value ?? 1.0);
        float amplitude = (float)(this.FindControl<Slider>("AmplitudeSlider")?.Value ?? 1.0);

        // 3. Inicializa o Perlin Noise com a semente
        var perlin = new PerlinNoise.Core.PerlinNoise();
        perlin.InitializePermutation(seed);

        // 4. Prepara buffer de pixels no formato BGRA
        int stride = imageWidth * 4;
        byte[] pixels = ArrayPool<byte>.Shared.Rent(stride * imageHeight);

        for (int y = 0; y < imageHeight; y++)
        {
            for (int x = 0; x < imageWidth; x++)
            {
                // Aplica frequência e amplitude ao ruído
                float noiseValue = perlin.Noise(x * scale * frequency, y * scale * frequency) * amplitude;

                // Garante que o valor fique entre 0 e 1 após aplicar amplitude
                noiseValue = Math.Clamp(noiseValue, 0f, 1f);

                byte gray = (byte)(noiseValue * 255);

                int index = (y * stride) + (x * 4);
                pixels[index + 0] = gray; // Blue
                pixels[index + 1] = gray; // Green
                pixels[index + 2] = gray; // Red
                pixels[index + 3] = 255;  // Alpha (opaco)
            }
        }

        // 5. Cria WriteableBitmap com o buffer preenchido
        var writeableBitmap = new WriteableBitmap(
            new PixelSize(imageWidth, imageHeight),
            new Vector(96, 96), // resolução DPI
            PixelFormat.Bgra8888,
            AlphaFormat.Premul);

        using (var fb = writeableBitmap.Lock())
        {
            Marshal.Copy(pixels, 0, fb.Address, stride * imageHeight);
        }

        ArrayPool<byte>.Shared.Return(pixels);

        // 6. Exibe a imagem na interface
        this.FindControl<Image>("NoiseImage")!.Source = writeableBitmap;
    }
}
