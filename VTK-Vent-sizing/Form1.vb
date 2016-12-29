Imports System.Text
Imports System.IO
Imports System.Configuration
Imports System.Math
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Threading


Public Class Form1
    'Naam; stof nr; Maxdruk[bar];Kst [bar.m.s-1];Glimmtemp [c]; ...
    Public Shared dust() As String =
  {"Gluten, Weizen;nr2204; 8.7; 105; 540",
   "Kartoffelstärke;nr180; 8.2; 116; 450",
   "Maisstärke     ;nr191;10.6; 143; 420",
   "Quellstärke    ;nr197; 8.5;  50; 390",
   "Kartoffelstärke;nr3041;7.8;  43; 420",
   "Test data;nr test;8.0;  150; 999"}


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click, TabPage1.Enter, NumericUpDown5.ValueChanged, NumericUpDown3.ValueChanged, NumericUpDown2.ValueChanged, NumericUpDown4.ValueChanged, NumericUpDown1.ValueChanged, NumericUpDown6.ValueChanged, ComboBox1.SelectedIndexChanged
        Dim slender, vol, Length, dia As Double
        Dim kst, Vent_area, Pmax, Pred_max, Pstat, B, C As Double
        Dim words() As String

        If ComboBox1.SelectedIndex > -1 Then
            words = dust(ComboBox1.SelectedIndex).Split(";")
            TextBox3.Text = words(1)        'Stoff-nr
            Double.TryParse(words(2), Pmax) 'Pmax
            Double.TryParse(words(3), kst)  'Kst
            TextBox7.Text = words(4)        'Glimmtemperatuur
        End If

        dia = NumericUpDown1.Value
        Length = NumericUpDown2.Value
        If Pmax >= NumericUpDown3.Minimum And Pmax <= NumericUpDown3.Maximum Then NumericUpDown3.Value = Pmax.ToString
        If kst >= NumericUpDown4.Minimum And kst <= NumericUpDown4.Maximum Then NumericUpDown4.Value = kst.ToString
        Pred_max = NumericUpDown5.Value     'gezochte maximale optredender druk
        Pstat = NumericUpDown6.Value        'Venting activation pressure

        slender = Length / dia              '[-]
        vol = PI / 4 * dia ^ 2 * Length     '[m3]

        B = (3.264 * 10 ^ -5 * Pmax * kst * Pred_max ^ -0.569 + 0.27 * (Pstat - 0.1) * Pred_max ^ -0.5) * vol ^ 0.753
        C = -4.305 * Log10(Pred_max) + 0.758
        Vent_area = B * (1 + C * Log10(slender))    '[m2]

        TextBox1.Text = Round(slender, 2).ToString
        TextBox2.Text = Round(vol, 2).ToString

        TextBox8.Text = Round(B, 4).ToString
        TextBox10.Text = Round(C, 4).ToString
        TextBox4.Text = Round(Vent_area, 2).ToString
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim words() As String
        Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")
        Thread.CurrentThread.CurrentUICulture = New CultureInfo("en-US")

        For hh = 0 To UBound(dust)              'Fill combobox1
            words = dust(hh).Split(";")
            ComboBox1.Items.Add(words(0))
        Next hh
        ComboBox1.SelectedIndex = 2
    End Sub
End Class
