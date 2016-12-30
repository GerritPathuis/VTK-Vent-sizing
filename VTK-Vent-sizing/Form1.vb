Imports System.Text
Imports System.IO
Imports System.Configuration
Imports System.Math
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Threading


Public Class Form1
    'Naam; stof nr; Maxdruk[bar];Kst [bar.m.s-1];Glimmtemp [c];size[mu]; ...
    Public Shared dust() As String =
  {"Gluten, Weizen    ; 2204 ; 8.7; 105; 540; 48",
   "Holz,             ; 5154 ; 6.1;  70; 550; <10",
   "Kartoffelstärke   ; 180  ; 8.2; 116; 450; 28",
   "Maisstärke        ; 191  ;10.6; 143; 420; 22",
   "Quellstärkeu      ; 197  ; 8.5;  50; 390; 14",
   "Kartoffelstärke   ; 3041 ; 7.8;  43; 420; --",
   "Test data         ; test ; 8.0; 150; 999; --"}
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click, TabPage1.Enter, NumericUpDown5.ValueChanged, NumericUpDown3.ValueChanged, NumericUpDown2.ValueChanged, NumericUpDown4.ValueChanged, NumericUpDown1.ValueChanged, NumericUpDown6.ValueChanged, ComboBox1.SelectedIndexChanged, NumericUpDown7.ValueChanged, NumericUpDown8.ValueChanged
        Dim slender, vol, Length, duct_L, dia As Double
        Dim kst, Vent_area, Pstat, B, C, Temp As Double
        Dim Pmax As Double          'Max Pressure without Venting
        Dim Pred_max As Double      'Max Pressure with Venting after explosion
        Dim Pred_max_duct As Double 'Max Pressure after with Venting explosion due to duct length 
        Dim words() As String

        If ComboBox1.SelectedIndex > -1 Then
            words = dust(ComboBox1.SelectedIndex).Split(";")
            TextBox3.Text = words(1)        'Stoff-nr
            Double.TryParse(words(2), Pmax) 'Pmax
            Double.TryParse(words(3), kst)  'Kst
            TextBox7.Text = words(4)        'Glimmtemperatuur
            TextBox11.Text = words(5)       'Mediaan Particle size [mu]

        End If

        dia = NumericUpDown1.Value          'vessel diameter
        Length = NumericUpDown2.Value       'Vessel length
        duct_L = NumericUpDown8.Value       'Duct length to vent
        If Pmax >= NumericUpDown3.Minimum And Pmax <= NumericUpDown3.Maximum Then NumericUpDown3.Value = Pmax.ToString
        If kst >= NumericUpDown4.Minimum And kst <= NumericUpDown4.Maximum Then NumericUpDown4.Value = kst.ToString
        Pred_max = NumericUpDown5.Value     'gezochte maximale optredender druk = reduced explosion pressure
        Pstat = NumericUpDown6.Value        'Venting activation pressure
        Temp = NumericUpDown7.Value         'Operating temp

        slender = Length / dia              '[-]
        vol = PI / 4 * dia ^ 2 * Length     '[m3]

        B = (3.264 * 10 ^ -5 * Pmax * kst * Pred_max ^ -0.569 + 0.27 * (Pstat - 0.1) * Pred_max ^ -0.5) * vol ^ 0.753
        C = -4.305 * Log10(Pred_max) + 0.758
        Vent_area = B * (1 + C * Log10(slender))    'Required Vent area[m2]

        '----------Vent duct---- loss
        Pred_max_duct = Pred_max * (1 + 1.73 * (Vent_area * vol ^ -0.753) ^ 1.6 * duct_L)

        '----------Vent are not always 100 effective---- 
        Vent_area /= 0.9                            '90% vent efficiency

        TextBox1.Text = Round(slender, 2).ToString
        TextBox2.Text = Round(vol, 2).ToString

        TextBox8.Text = Round(Sqrt(Vent_area * 4 / PI), 2).ToString 'round vent door
        TextBox10.Text = Round(Sqrt(Vent_area), 2).ToString         'square vent door
        TextBox4.Text = Round(Vent_area, 2).ToString                'Vent Area
        TextBox15.Text = Round(Pred_max_duct, 2).ToString           'Vent Area

        '-------------- checks---------------------
        NumericUpDown6.BackColor = IIf(Pstat < 0.1 Or Pstat > 1.0, Color.Red, Color.Yellow) 'activation (pop open) pressure
        TextBox2.BackColor = IIf(vol < 0.1 Or vol > 10000, Color.Red, Color.LightGreen)     'Volume limits
        NumericUpDown5.BackColor = IIf(Pred_max < 0.1 Or Pred_max > 2.0, Color.Red, Color.Yellow) 'Reduced explosion pressure
        NumericUpDown7.BackColor = IIf(Temp < -20 Or Temp > 60, Color.Red, Color.Yellow) 'Reduced explosion pressure

        If (Pred_max < Pstat) Then
            TextBox12.Text = "Reduced expl. pres. must be bigger then the activ. pres."
            TextBox12.BackColor = Color.Red
        Else
            TextBox12.Text = ""
            TextBox12.BackColor = SystemColors.Window
        End If

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

        TextBox5.Text = "Based on: SHAPA TECHNICAL PAPER 10 (Revised)"

        TextBox6.Text = "Over the years various modifications To the nomographs were made mainly by extending the range Of applicability And eventually formula where produced In VDI 3673."
        TextBox6.Text &= "It Is the same basic formula that was used In VDI 3673, NFPA 68 And EN14491 up until 2007 When NFPA 68 decided To use a total different method."
        TextBox6.Text &= "However the formula used the EN14491:  2012 Is the same basic formula that VDI 3673 uses."
        TextBox6.Text &= "It should also be noted that in circa 1989 the Institute of Chemical Engineers (IChemE)"
        TextBox6.Text &= "published nomographs And these are still available to be used in the UK where it can be proven it provides a higher level of safety Or there Is a case that 'state Of the Art'"
        TextBox6.Text &= "can be demonstrated in preference to using EN14491:  2012. "

        TextBox9.Text = "German dust database" & vbCrLf & "http://staubex.ifa.dguv.de/"

        TextBox13.Text = "Cyclones require some special attention due to the difficulty of locating the explosion vent."
        TextBox13.Text &= "This Is due to internal wall surfaces being critical to the efficient operation of the cyclone." & vbCrLf
        TextBox13.Text &= "If a panel was simply installed on the wall the cyclone may fail to create the required cyclonic air flow."
        TextBox13.Text &= "generates its own issues As this pipe Is effectively an internal vent duct, which has a negative effect On the explosion venting." & vbCrLf & vbCrLf
        TextBox13.Text &= "Therefore the dimensions of the immersion pipe (shown in the diagram) need to be accounted for "
        TextBox13.Text &= "And treated As a vent duct - details Of this calculation are In section 4.0. Any taper in the bottom of the pipe must also be accounted for."

        TextBox14.Text = "Influence Of explosion vent ducts." & vbCrLf
        TextBox14.Text &= "Explosion vent ducts influence the Pred, Max In two ways. Firstly they offer a restriction To the flow Of hot gases And unburnt fuel released through the vent –"
        TextBox14.Text &= "In the same way a pipe will restrict flow Of water If the bore Is reduced. However the second reason they influence the Pred, Max prevents the use Of oversizing the duct "
        TextBox14.Text &= "to compensate for the former effect. The unburnt fuel which Is forced into the duct when the explosion vent opens ignites when the flame catches up with "
        TextBox14.Text &= "this fuel And creates a secondary explosion (high pressure/back pressure) inside the duct, which in turns slows the explosion venting process down in the main,"
        TextBox14.Text &="protected vessel – increasing the duct diameter will enhance this effect And thus Is Not advised.  The increase in pressure created by the acceptable duct "
        TextBox14.Text &= "Design given in the standards can be calculated from the following equation." & vbCrLf & vbCrLf
        TextBox14.Text &= "P 'red,max = Pred,max x (1+ 1.73 x (A x V-0.753) 1.6 x l )" & vbCrLf
        TextBox14.Text &= "Where" & vbCrLf
        TextBox14.Text &= "l = the vent duct length In meters (m)" & vbCrLf
        TextBox14.Text &= "V = Vessel volume In cubic meters (m3)" & vbCrLf
        TextBox14.Text &= "A = Required vent area without the vent duct In square meters (m2)" & vbCrLf
        TextBox14.Text &= "P 'red,max = the maximum reduced pressure in the vessel with the vent duct in bar.g" & vbCrLf & vbCrLf
        TextBox14.Text &= "The vent duct should be designed so that it Is Not larger than the explosion vent And it must be straight Or have a minimum curvature to"
        TextBox14.Text &= "radius >2 And angle no more than 20o from the horizontal. "

    End Sub
End Class
