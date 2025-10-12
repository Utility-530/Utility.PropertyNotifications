using System;
using System.Security;


namespace Utility.Structs;

public sealed class SecurePin : IDisposable
{
    private readonly SecureString _secure;

    public int Length => _secure.Length;

    public SecurePin(ReadOnlySpan<char> value)
    {
        if (value.Length < 4 || value.Length > 6)
            throw new ArgumentException("PIN must be 4–6 digits long.");

        _secure = new SecureString();

        foreach (char c in value)
        {
            if (!char.IsDigit(c))
                throw new ArgumentException("PIN must contain only digits.");

            _secure.AppendChar(c);
        }

        _secure.MakeReadOnly();
    }
    public SecurePin(ReadOnlySpan<int> digits)
    {
        if (digits.Length < 4 || digits.Length > 6)
            throw new ArgumentException("PIN must be 4–6 digits long.");

        _secure = new SecureString();

        foreach (int c in digits)
        {
            if (c < 0 || c > 9)
                throw new ArgumentException("PIN digits must be 0–9.");

            _secure.AppendChar((char)('0' + c));
        }

        _secure.MakeReadOnly();
    }

    // Overloaded convenience constructors
    public SecurePin(int d1, int d2, int d3, int d4)
        : this(new[] { d1, d2, d3, d4 }) { }

    public SecurePin(int d1, int d2, int d3, int d4, int d5)
        : this(new[] { d1, d2, d3, d4, d5 }) { }

    public SecurePin(int d1, int d2, int d3, int d4, int d5, int d6)
        : this(new[] { d1, d2, d3, d4, d5, d6 }) { }

    public bool Verify(ReadOnlySpan<char> input)
    {
        if (input.Length != _secure.Length)
            return false;

        IntPtr ptr = IntPtr.Zero;
        try
        {
            // Decrypt SecureString to unmanaged memory
            ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(_secure);
            string real = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr)!;

            // Constant-time comparison
            int diff = 0;
            for (int i = 0; i < input.Length; i++)
            {
                diff |= real[i] ^ input[i];
            }
            return diff == 0;
        }
        finally
        {
            if (ptr != IntPtr.Zero)
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
        }
    }

    public void Dispose()
    {
        _secure?.Dispose();
    }

    public override string ToString() => new ('*', _secure.Length);
}
