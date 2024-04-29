#pragma warning disable CA1861
#pragma warning disable CS8618
// 2-22
// n = 8 бит
// F(x) = 1 + x + x3 + x7 + x8
using System.Collections;
using System.Numerics;
using System.Text;

internal static class Program
{
    internal static void Main()
    {
        LFSR lfsr = new();
     
        BitArray polynomial = new(new bool[] { true, true, false, false, false, false, false, true, true });

        // Начальное состояние регистра
        BitArray primary = new(new bool[] { true, true, true, false, false, true, false, true, true });

        lfsr.Init(primary, polynomial);

        (int steps, int zero, int one, int even, int uneven) = PeriodDate(lfsr, primary);

        Console.WriteLine($"{BitArrayToString(primary)} - начальное состояние");
        Console.WriteLine($"Период работы генератора: {steps}");
        Console.WriteLine($"Количество нулей в битах: {zero}");
        Console.WriteLine($"Количество единиц в битах: {one}");
        Console.WriteLine($"Количество четных чисел: {even}");
        Console.WriteLine($"Количество нечетных чисел: {uneven}");

    }

    // Метод для определения длины периода
    private static (int steps, int zero, int one, int even, int uneven) PeriodDate(LFSR lfsr, BitArray primary)
    {
        BitArray currentState = primary;
        int steps = 0;
        int even = 0;
        int zero = 0;
        Console.WriteLine(BitArrayToString(primary));
        do
        {
            currentState = lfsr.NextState(currentState);
            Console.WriteLine(BitArrayToString(currentState));
            steps++;

            // Четные и нечетные чисела
            BigInteger currentStateBigInt = BitArrayToBigInt(currentState);
            if (currentStateBigInt % 2 == 0)
            {
                even++;
            }

            // Нули и единицы 
            foreach (bool bit in currentState)
            {
                if (bit == false)
                {
                    zero++;
                }
            }
        } while (!primary.Cast<bool>().SequenceEqual(currentState.Cast<bool>()));

        return (steps += 1, zero, (primary.Length * steps) - zero, even, steps - even);
    }
    internal static string BitArrayToString(BitArray array)
    {
        StringBuilder sb = new();
        for (int i = 0; i < array.Length; i++)
        {
            _ = sb.Append(array[i] ? "1" : "0");
        }

        return sb.ToString();
    }
    internal static BigInteger BitArrayToBigInt(BitArray bitArray)
    {
        BigInteger result = 0;
        for (int i = 0; i < bitArray.Length; i++)
        {
            if (bitArray[i])
            {
                result |= BigInteger.One << i;
            }
        }

        return result;
    }
}
internal sealed class LFSR
{
    internal BitArray Polynomial { get; set; }

    internal BitArray Primary { get; set; }

    internal void Init(BitArray _primary, BitArray _polynomial)
    {
        Polynomial = _polynomial;
        Primary = _primary;
    }

    internal BitArray NextState(BitArray currentState)
    {
        BitArray nextState = new(currentState.Length);

        // Используем XOR для вычисления нового значения 
        BitArray xoredBits = XOR(currentState, Polynomial);
        nextState[0] = xoredBits[0];

        // Копируем биты из предыдущего состояния
        for (int i = 1; i < currentState.Length; i++)
        {
            nextState[i] = currentState[i - 1];
        }

        return nextState;
    }

    // Метод XOR 
    private static BitArray XOR(BitArray baseBits, BitArray polynomial)
    {
        BitArray result = new(polynomial.Length);
        bool? bit = null;

        for (int j = 0; j < polynomial.Length; j++)
        {
            if (polynomial[j])
            {
                if (bit == null)
                {
                    bit = baseBits[j];
                }
                else
                {
                    bool currentBitValue = baseBits[j];
                    bit ^= currentBitValue;
                }
            }
        }

        result[0] = (bit & true) == true;
        return result;
    }
}
