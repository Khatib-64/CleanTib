namespace CleanTib.Domain.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ClassSupportDeepSearchAttribute : Attribute
{
    public uint Depth { get; set; }

    public ClassSupportDeepSearchAttribute()
        => Depth = 0;

    public ClassSupportDeepSearchAttribute(uint depth)
        => Depth = depth;
}

[AttributeUsage(AttributeTargets.Property)]
public class ColumnSupportDeepSearchAttribute : Attribute
{

}
