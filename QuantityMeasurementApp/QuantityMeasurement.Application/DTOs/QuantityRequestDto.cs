namespace QuantityMeasurement.Application.DTOs;
public class QuantityRequestDto
{
    public double Value1 { get; set; }
    public string Unit1 { get; set; } = string.Empty;

    public double? Value2 { get; set; }
    public string? Unit2 { get; set; }

    public string? TargetUnit { get; set; }

    public double? Scalar { get; set; }

}
