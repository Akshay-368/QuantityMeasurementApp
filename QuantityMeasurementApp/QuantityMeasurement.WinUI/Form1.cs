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

        // Validation - Main Flow UC1
        if (double.TryParse(input1, out double val1) && double.TryParse(input2, out double val2))
        {
            // Instantiate your Business Logic classes
            Feet feet1 = new Feet(val1);
            Feet feet2 = new Feet(val2);

            // Comparison
            bool areEqual = feet1.Equals(feet2);

            // Output
            lblResult.Text = $"Output: Equal ({areEqual.ToString().ToLower()})";
            lblResult.ForeColor = areEqual ? Color.Green : Color.Red;
        }
        else
        {
            MessageBox.Show("Input: Please enter valid numeric values in Feet.", "Validation Error");
        }

        
    }


}
