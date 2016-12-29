Imports System.Text
Imports System.IO
Imports System.Configuration
Imports System.Math
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Threading


Public Class Form1

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click, TabPage1.Enter, NumericUpDown6.ValueChanged, NumericUpDown5.ValueChanged, NumericUpDown3.ValueChanged, NumericUpDown2.ValueChanged, NumericUpDown4.ValueChanged, NumericUpDown1.ValueChanged
        Dim slender, vol, height, dia As Double
        Dim kst, Vent_area, Pmax, Pred_max, Pstat, B, C As Double

        dia = NumericUpDown1.Value
        height = NumericUpDown2.Value
        Pmax = NumericUpDown3.Value
        kst = NumericUpDown4.Value
        Pred_max = NumericUpDown5.Value
        Pstat = NumericUpDown6.Value

        slender = height / dia              '[-]
        vol = PI / 4 * dia ^ 2 * height     '[m3]

        B = (3.264 ^ 10 - 5 * Pmax * kst * Pred_max ^ -0.569 + 0.27 * (Pstat - 0.1) * Pred_max ^ -0.5) * vol ^ 0.753

        C = -4.305 * Log10(Pred_max) + 0.758

        Vent_area = B * (1 + C * Log10(slender))    '[m2]

        TextBox1.Text = Round(slender, 2).ToString
        TextBox2.Text = Round(vol, 2).ToString
        TextBox4.Text = Round(C, 2).ToString
    End Sub

    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter

    End Sub
End Class
