namespace QuantityMeasurement.WinUI;
using QuantityMeasurement.Core;
using QuantityMeasurement.Units;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    
    }

    private void btnCompare_Click(object sender, EventArgs e)
    {
        string input1 = txtValue1.Text;
        string input2 = txtValue2.Text;

        // 1. Unified Validation
        if (!double.TryParse(input1, out double v1) || !double.TryParse(input2, out double v2))
        {
            MessageBox.Show("Please enter valid numeric values.", "Validation Error");
            return;
        }

        bool areEqual = false;
        string selectedUnit = cmbUnitSelector.SelectedItem.ToString()?? "Feet";

        // 2. Logic Selection based on UI choice
        if (selectedUnit == "Feet")
        {
            Feet f1 = new Feet(v1);
            Feet f2 = new Feet(v2);
            areEqual = f1.Equals(f2);
        }
        else if (selectedUnit == "Inches")
        {
            Inches i1 = new Inches(v1);
            Inches i2 = new Inches(v2);
            areEqual = i1.Equals(i2);
        }

        // 3. Update UI once
        lblResult.Text = $"{selectedUnit} Result: {areEqual.ToString().ToLower()}";
        lblResult.ForeColor = areEqual ? Color.Green : Color.Red;
    }


}
