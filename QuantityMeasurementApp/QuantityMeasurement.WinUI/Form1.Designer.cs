namespace QuantityMeasurement.WinUI;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    // Defining the controls
    private System.Windows.Forms.TextBox txtValue1;
    private System.Windows.Forms.TextBox txtValue2;
    private System.Windows.Forms.Button btnCompare;
    private System.Windows.Forms.Label lblResult;
    private System.Windows.Forms.Label lblTitle;
    // private System.Windows.Forms.ComboBox cmbUnitSelector;
    private System.Windows.Forms.ComboBox cmbUnit1;
    private System.Windows.Forms.ComboBox cmbUnit2;


    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Text = "Quantity Measurement App - UC3" ;

        this.txtValue1 = new System.Windows.Forms.TextBox();
        this.txtValue2 = new System.Windows.Forms.TextBox();
        this.btnCompare = new System.Windows.Forms.Button();
        this.lblResult = new System.Windows.Forms.Label();
        this.lblTitle = new System.Windows.Forms.Label();
        // this.cmbUnitSelector = new System.Windows.Forms.ComboBox();
        // cmbUnit1
        this.cmbUnit1 = new System.Windows.Forms.ComboBox();
        this.cmbUnit1.Location = new System.Drawing.Point(200, 50);
        this.cmbUnit1.Size = new System.Drawing.Size(100, 23);
        this.cmbUnit1.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbUnit1.Items.AddRange(new object[] { "Feet", "Inches", "Yard", "Meter" });
        this.cmbUnit1.SelectedIndex = 0;

        // cmbUnit2
        this.cmbUnit2 = new System.Windows.Forms.ComboBox();
        this.cmbUnit2.Location = new System.Drawing.Point(200, 90);
        this.cmbUnit2.Size = new System.Drawing.Size(100, 23);
        this.cmbUnit2.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbUnit2.Items.AddRange(new object[] { "Feet", "Inches", "Yard", "Meter" });
        this.cmbUnit2.SelectedIndex = 0;


        this.Controls.Add(this.cmbUnit1);
        this.Controls.Add(this.cmbUnit2);

        this.SuspendLayout() ;

        // lblTitle
        this.lblTitle.Text = "Select Unit and Enter Values : ";
        this.lblTitle.Location = new System.Drawing.Point(30, 20);
        this.lblTitle.AutoSize = true;

        // txtValue1
        this.txtValue1.Location = new System.Drawing.Point(30, 50);
        this.txtValue1.Size = new System.Drawing.Size(150, 23);

        // txtValue2
        this.txtValue2.Location = new System.Drawing.Point(30, 90);
        this.txtValue2.Size = new System.Drawing.Size(150, 23);

        // btnCompare
        this.btnCompare.Location = new System.Drawing.Point(30, 130);
        this.btnCompare.Size = new System.Drawing.Size(150, 30);
        this.btnCompare.Text = "Check Equality";
        this.btnCompare.UseVisualStyleBackColor = true;
        this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);

        // lblResult
        this.lblResult.Location = new System.Drawing.Point(30, 180);
        this.lblResult.Size = new System.Drawing.Size(300, 23);
        this.lblResult.Text = "Result: ";

        // Form1
        this.ClientSize = new System.Drawing.Size(350, 250);
        this.Controls.Add(this.lblTitle);
        this.Controls.Add(this.txtValue1);
        this.Controls.Add(this.txtValue2);
        this.Controls.Add(this.btnCompare);
        this.Controls.Add(this.lblResult);
        this.Text = "Quantity Measurement App - UC1 and UC2";
        this.ResumeLayout(false);
        this.PerformLayout();
        
        

        // // This line prevents the user from typing. It makes the box "Select Only."
        // this.cmbUnitSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;


        
    }

    #endregion
}
