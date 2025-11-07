using System;

namespace Utility.Structs;

public readonly struct Pin
{
    private readonly byte[] _digits;

    public int Length => _digits?.Length ?? 0;

    public Pin(ReadOnlySpan<int> digits)
    {
        if (digits.Length < 4 || digits.Length > 6)
            throw new ArgumentException("PIN must be 4–6 digits long.");

        _digits = new byte[digits.Length];
        for (int i = 0; i < digits.Length; i++)
        {
            if (digits[i] < 0 || digits[i] > 9)
                throw new ArgumentException("PIN digits must be 0–9.");
            _digits[i] = (byte)digits[i];
        }
    }

    // Overloaded convenience constructors
    public Pin(int d1, int d2, int d3, int d4)
        : this(new[] { d1, d2, d3, d4 }) { }

    public Pin(int d1, int d2, int d3, int d4, int d5)
        : this(new[] { d1, d2, d3, d4, d5 }) { }

    public Pin(int d1, int d2, int d3, int d4, int d5, int d6)
        : this(new[] { d1, d2, d3, d4, d5, d6 }) { }

    public Pin(ReadOnlySpan<char> value)
    {
        if (value.Length < 4 || value.Length > 6)
            throw new ArgumentException("PIN must be 4–6 digits long.");

        _digits = new byte[value.Length];

        for (int i = 0; i < value.Length; i++)
        {
            if (!char.IsDigit(value[i]))
                throw new ArgumentException("PIN must contain only digits.");

            _digits[i] = (byte)(value[i] - '0');
        }
    }

    public bool Verify(ReadOnlySpan<char> input)
    {
        if (input.Length != _digits.Length)
            return false;

        // Constant-time comparison
        int diff = 0;
        for (int i = 0; i < _digits.Length; i++)
        {
            diff |= _digits[i] ^ (byte)(input[i] - '0');
        }
        return diff == 0;
    }

    public void Clear()
    {
        if (_digits != null)
            Array.Clear(_digits, 0, _digits.Length);
    }

    public override string ToString() => new('*', _digits?.Length ?? 0);
}