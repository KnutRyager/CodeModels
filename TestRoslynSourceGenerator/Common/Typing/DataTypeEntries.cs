using System.Diagnostics.CodeAnalysis;

namespace Common.Typing;

public class D_bool<T>
{
    protected bool value;
    public static implicit operator bool(D_bool<T> v) => v.value;
    public static implicit operator D_bool<T>(bool v) => new() { value = v };
}

public class D_byte<T>
{
    protected byte value;
    public static implicit operator byte(D_byte<T> v) => v.value;
    public static implicit operator D_byte<T>(byte v) => new() { value = v };
}

public class D_short<T>
{
    protected short value;
    public static implicit operator short(D_short<T> v) => v.value;
    public static implicit operator D_short<T>(short v) => new() { value = v };
}

public class D_int<T> : IComparable<D_int<T>>
{
    protected int value;
    public static implicit operator int(D_int<T> v) => v.value;
    public static implicit operator D_int<T>(int v) => new() { value = v };
    public int CompareTo([AllowNull] D_int<T> other) => other == null ? 0 : value - other.value;
}

public class D_long<T>
{
    protected long value;
    public static implicit operator long(D_long<T> v) => v.value;
    public static implicit operator D_long<T>(long v) => new() { value = v };
}

public class D_float<T>
{
    protected float value;
    public static implicit operator float(D_float<T> v) => v.value;
    public static implicit operator D_float<T>(float v) => new() { value = v };
}

public class D_double<T>
{
    protected double value;
    public static implicit operator double(D_double<T> v) => v.value;
    public static implicit operator D_double<T>(double v) => new() { value = v };
}

public class D_decimal<T>
{
    protected decimal value;
    public static implicit operator decimal(D_decimal<T> v) => v.value;
    public static implicit operator D_decimal<T>(decimal v) => new() { value = v };
}

public class D_DateTime<T>
{
    protected DateTime value;
    public static implicit operator DateTime(D_DateTime<T> v) => v.value;
    public static implicit operator D_DateTime<T>(DateTime v) => new() { value = v };
}

public class D_char<T>
{
    protected char value;
    public static implicit operator char(D_char<T> v) => v.value;
    public static implicit operator D_char<T>(char v) => new() { value = v };
}

public class D_string<T>
{
    protected string? value;
    public static implicit operator string?(D_string<T> d) => d.value;
    public static implicit operator D_string<T>(string b) => new() { value = b };
    public override string ToString() => value ?? "";
}

public class D_json<T> : D_string<T>
{
    public static implicit operator D_json<T>(string v) => new() { value = v };
}

public class D_xml<T> : D_string<T>
{
    public static implicit operator D_xml<T>(string v) => new() { value = v };
}
