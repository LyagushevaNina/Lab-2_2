#pragma warning disable CA1861
#pragma warning disable CS8618

using System.Collections;
using System.Numerics;
using System.Text;

internal static class Program
{
    internal static void Main()
    {
        Console.Clear();

        // Создаем объект класса LFSR
        LFSR lfsr = new();

        // Полином определяет, какие биты из регистра будут использоваться для вычисления следующего бита в последовательности выходных значений регистра
        BitArray polynomial = new(new bool[] { true, false, true, false, false, false, false, true, true });

        // Начальное состояние регистра
        BitArray Primary = new(new bool[] { true, false, true, false, false, false, false, true, true });

        // Инициализируем LFSR
        lfsr.Init(Primary, polynomial);

        // Определяем интересующие нас данные
        (int steps, int zero, int one, int even, int uneven) = GetPeriodInfo(lfsr, Primary);

        Console.WriteLine($"{BitArrayToString(Primary)} - initial state");
        Console.WriteLine($"Generator period length: {steps}");
        Console.WriteLine($"Number of zeros in one period in bits: {zero}");
        Console.WriteLine($"Number of ones in one period in bits: {one}");
        Console.WriteLine($"Number of even numbers in one period: {even}");
        Console.WriteLine($"Number of odd numbers in one period: {uneven}");

    }

    // Метод для определения длины периода
    private static (int steps, int zero, int one, int even, int uneven) GetPeriodInfo(LFSR lfsr, BitArray Primary)
    {
        BitArray currentState = Primary;
        int steps = 0;
        int even = 0;
        int zero = 0;
        Console.WriteLine(BitArrayToString(Primary));
        do
        {
            currentState = lfsr.NextState(currentState);
            Console.WriteLine(BitArrayToString(currentState));
            steps++;

            // Подсчет количества четных и нечетных чисел при однобайтовом представлении
            BigInteger currentStateBigInt = BitArrayToBigInt(currentState);
            if (currentStateBigInt % 2 == 0)
            {
                even++;
            }

            // Подсчет количества нулей и единиц при битовом представлении
            foreach (bool bit in currentState)
            {
                if (bit == false)
                {
                    zero++;
                }
            }
        } while (!Primary.Cast<bool>().SequenceEqual(currentState.Cast<bool>()));

        return (steps += 1, zero, (Primary.Length * steps) - zero, even, steps - even);
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
                result |= BigInteger.One << i; // Установка i-го бита в результате
            }
        }

        return result;
    }
}
internal sealed class LFSR
{
    // Полином, определяющий характеристики генератора
    internal BitArray Polynomial { get; set; }

    // Начальное состояние регистра
    internal BitArray Primary { get; set; }

    // Метод для инициализации LFSR с указанными начальным состоянием и полиномом
    internal void Init(BitArray _primary, BitArray _polynomial)
    {
        Polynomial = _polynomial;
        Primary = _primary;
    }

    // Метод для генерации следующего состояния на основе текущего состояния
    internal BitArray NextState(BitArray currentState)
    {
        BitArray nextState = new(currentState.Length);

        // Вычисляем новое значение для первого бита с использованием XOR
        BitArray xoredBits = XOR(currentState, Polynomial);
        nextState[0] = xoredBits[0];

        // Копируем все остальные биты из предыдущего состояния
        for (int i = 1; i < currentState.Length; i++)
        {
            nextState[i] = currentState[i - 1];
        }

        return nextState;
    }

    // Метод для вычисления XOR битов
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
