Imports System.IO
Public Enum StateArmoredArmadillo
    StandArmored
    WalkDebug
    IntroAnimation
    Guard
    ShootArmored
    JumpStartArmored
    JumpArmored
    JumpEndArmored
    Rolling
    RollingRecoveryArmored
    RollingRecoveryEndArmored
    FreeFalling
End Enum

Public Enum StateArmoredArmadilloProjectile
    Create
    Horizontal
End Enum


Public Enum FaceDir
    Left
    Right
End Enum

Public Class CImage
    Public Width As Integer
    Public Height As Integer
    Public Elmt(,) As System.Drawing.Color
    Public ColorMode As Integer 'not used

    Sub OpenImage(ByVal FName As String)
        'open file .bmp, read file
        Dim s As String
        Dim L As Long
        Dim BR As BinaryReader
        Dim h, w, pos As Integer
        Dim r, g, b As Integer
        Dim pad As Integer

        BR = New BinaryReader(File.Open(FName, FileMode.Open))

        Try
            BlockRead(BR, 2, s)

            If s <> "BM" Then
                MsgBox("Not a BMP file")
            Else 'BMP file
                BlockReadInt(BR, 4, L) 'size
                'MsgBox("Size = " + CStr(L))
                BlankRead(BR, 4) 'reserved
                BlockReadInt(BR, 4, pos) 'start of data
                BlankRead(BR, 4) 'size of header
                BlockReadInt(BR, 4, Width) 'width
                'MsgBox("Width = " + CStr(I.Width))
                BlockReadInt(BR, 4, Height) 'height
                'MsgBox("Height = " + CStr(I.Height))
                BlankRead(BR, 2) 'color panels
                BlockReadInt(BR, 2, ColorMode) 'colormode
                If ColorMode <> 24 Then
                    MsgBox("Not a 24-bit color BMP")
                Else

                    BlankRead(BR, pos - 30)

                    ReDim Elmt(Width - 1, Height - 1)
                    pad = (4 - (Width * 3 Mod 4)) Mod 4

                    For h = Height - 1 To 0 Step -1
                        For w = 0 To Width - 1
                            BlockReadInt(BR, 1, b)
                            BlockReadInt(BR, 1, g)
                            BlockReadInt(BR, 1, r)
                            Elmt(w, h) = Color.FromArgb(r, g, b)

                        Next
                        BlankRead(BR, pad)

                    Next

                End If

            End If

        Catch ex As Exception
            MsgBox("Error")

        End Try

        BR.Close()


    End Sub


    Sub CreateMask(ByRef Mask As CImage)
        'create mask from *this*
        Dim i, j As Integer

        Mask = New CImage
        Mask.Width = Width
        Mask.Height = Height

        ReDim Mask.Elmt(Mask.Width - 1, Mask.Height - 1)

        For i = 0 To Width - 1
            For j = 0 To Height - 1
                If Elmt(i, j).R = 0 And Elmt(i, j).G = 0 And Elmt(i, j).B = 0 Then
                    Mask.Elmt(i, j) = Color.FromArgb(255, 255, 255)
                Else
                    Mask.Elmt(i, j) = Color.FromArgb(0, 0, 0)
                End If
            Next
        Next

    End Sub


    Sub CopyImg(ByRef Img As CImage)
        'copies image to Img
        Img = New CImage
        Img.Width = Width
        Img.Height = Height
        ReDim Img.Elmt(Width - 1, Height - 1)

        For i = 0 To Width - 1
            For j = 0 To Height - 1
                Img.Elmt(i, j) = Elmt(i, j)
            Next
        Next

    End Sub

End Class

Public Class CCharacter
    Public PosX, PosY As Double
    Public Vx, Vy As Double
    Public FrameIdx As Integer
    Public CurrFrame As Integer
    Public ArrSprites() As CArrFrame
    Public IdxArrSprites As Integer
    Public FDir As FaceDir
    Public Destroy As Boolean = False

    Public Const gravity = 1

    'Public CurrState as ?

    Public Sub GetNextFrame()
        CurrFrame = CurrFrame + 1
        If CurrFrame = ArrSprites(IdxArrSprites).Elmt(FrameIdx).MaxFrameTime Then
            FrameIdx = FrameIdx + 1
            If FrameIdx = ArrSprites(IdxArrSprites).N Then
                FrameIdx = 0
            End If
            CurrFrame = 0

        End If

    End Sub

    Public Overridable Sub Update()

    End Sub


End Class

Public Class CCharArmoredArmadillo
    Inherits CCharacter

    Public isWalking As Boolean = False
    Public isGuarding As Boolean = False
    Public isShooting As Boolean = False
    Public isInRollingAnimation As Boolean = False
    Public isIntro As Boolean = True
    Public isFreeFalling As Boolean = False
    Public wallbangCounter As Integer
    Public CurrState As StateArmoredArmadillo

    Public Sub State(state As StateArmoredArmadillo, idxspr As Integer)
        CurrState = state
        IdxArrSprites = idxspr
        CurrFrame = 0
        FrameIdx = 0

    End Sub


    Public Overrides Sub Update()
        Select Case CurrState
            Case StateArmoredArmadillo.StandArmored
                GetNextFrame()
                If isWalking Then
                    State(StateArmoredArmadillo.WalkDebug, 1)
                ElseIf isGuarding Then
                    State(StateArmoredArmadillo.Guard, 3)
                ElseIf isShooting Then
                    State(StateArmoredArmadillo.ShootArmored, 4)
                ElseIf isInRollingAnimation Then
                    State(StateArmoredArmadillo.JumpStartArmored, 6)
                End If
            Case StateArmoredArmadillo.WalkDebug
                PosX = PosX + Vx
                If FDir = FaceDir.Left And isWalking = True Then
                    Vx = -5
                    PosX = PosX + Vx
                    If PosX <= 45 Then
                        Vx *= -1
                        FDir = FaceDir.Right
                    End If

                ElseIf FDir = FaceDir.Right And isWalking = True Then
                    Vx = 5
                    PosX = PosX + Vx
                    If PosX >= 300 Then
                        Vx *= -1
                        FDir = FaceDir.Left
                    End If
                End If
                ' stopping walk
                'stopping left
                If FrameIdx = 0 And PosX >= PosX + Vx Then
                    isWalking = False
                    State(StateArmoredArmadillo.StandArmored, 0)
                    'stopping right
                ElseIf CurrFrame = 0 And PosX <= PosX + Vx Then
                    isWalking = False
                    State(StateArmoredArmadillo.StandArmored, 0)
                End If
                GetNextFrame()
            Case StateArmoredArmadillo.IntroAnimation
                GetNextFrame()
                If CurrFrame = 4 And FrameIdx = 5 Then
                    State(StateArmoredArmadillo.StandArmored, 0)
                End If

            Case StateArmoredArmadillo.ShootArmored
                If FrameIdx = 0 And CurrFrame = 2 Then
                    'TODO: shoot projectile

                ElseIf FrameIdx = 1 And CurrFrame = 1 Then
                    isShooting = False
                    State(StateArmoredArmadillo.StandArmored, 0)
                End If
                GetNextFrame()

            Case StateArmoredArmadillo.Guard
                'TODO: create a hit event and deflect state
                If isGuarding = False Then
                    State(StateArmoredArmadillo.StandArmored, 0)
                End If
                GetNextFrame()

            Case StateArmoredArmadillo.JumpStartArmored
                Vy = -10
                Vx = 0
                GetNextFrame()
                State(StateArmoredArmadillo.JumpArmored, 7)
            Case StateArmoredArmadillo.JumpArmored
                PosX = PosX + Vx
                PosY = PosY + Vy
                Vy = Vy + gravity
                GetNextFrame()
                If FrameIdx = 4 Then
                    State(StateArmoredArmadillo.JumpEndArmored, 8)
                End If

            Case StateArmoredArmadillo.JumpEndArmored
                PosX = PosX + Vx
                PosY = PosY + Vy
                Vy = Vy + gravity
                GetNextFrame()
                If PosY >= 238 And Vy > 0 Then
                    Vy = 0
                    State(StateArmoredArmadillo.Rolling, 9)
                    PosY = 238
                    If FDir = FaceDir.Left Then
                        Vx = -10
                    Else
                        Vx = 10
                    End If
                    wallbangCounter = 0
                End If

            Case StateArmoredArmadillo.Rolling
                Dim rnd As New Random()
                GetNextFrame()
                If isIntro Then
                    PosY = PosY + Vy
                    Vy = Vy + gravity
                    If PosY >= 235 Then
                        Vy = -10
                        State(StateArmoredArmadillo.RollingRecoveryArmored, 10)
                    End If

                ElseIf FDir = FaceDir.Left Then
                    PosX = PosX + Vx
                    PosY = PosY + Vy
                    If PosX <= 30 Then
                        PosX = 31

                        Dim lowerBoundAngle, upperBoundAngle As Integer
                        If Vy <= 0 Then
                            upperBoundAngle = 60
                            lowerBoundAngle = 30
                        Else
                            upperBoundAngle = 330
                            lowerBoundAngle = 300
                        End If
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(lowerBoundAngle, upperBoundAngle + 1)
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 10)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                        FDir = FaceDir.Right
                        If rnd.NextDouble() < 0.2 Then
                            State(StateArmoredArmadillo.FreeFalling, 9)
                        End If

                    ElseIf PosY <= 30 Then
                        PosY = 31

                        Dim lowerBoundAngle, upperBoundAngle As Integer
                        upperBoundAngle = 240
                        lowerBoundAngle = 210
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(lowerBoundAngle, upperBoundAngle + 1)
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 10)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                        If rnd.NextDouble() < 0.2 Then
                            State(StateArmoredArmadillo.FreeFalling, 9)
                        End If
                    ElseIf PosY >= 250 Then
                        PosY = 249

                        Dim lowerBoundAngle, upperBoundAngle As Integer
                        upperBoundAngle = 150
                        lowerBoundAngle = 120
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(lowerBoundAngle, upperBoundAngle + 1)
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 10)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                        If rnd.NextDouble() < 0.2 Then
                            State(StateArmoredArmadillo.FreeFalling, 9)
                        End If
                    End If

                ElseIf FDir = FaceDir.Right Then
                    PosX = PosX + Vx
                    PosY = PosY + Vy
                    If PosX >= 310 Then
                        PosX = 309

                        Dim lowerBoundAngle, upperBoundAngle As Integer
                        If Vy <= 0 Then
                            upperBoundAngle = 150
                            lowerBoundAngle = 120
                        Else
                            upperBoundAngle = 240
                            lowerBoundAngle = 210
                        End If
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(lowerBoundAngle, upperBoundAngle + 1)
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 10)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                        FDir = FaceDir.Left
                        If rnd.NextDouble() < 0.2 Then
                            State(StateArmoredArmadillo.FreeFalling, 9)
                        End If

                    ElseIf PosY <= 30 Then
                        PosY = 31

                        Dim lowerBoundAngle, upperBoundAngle As Integer
                        upperBoundAngle = 330
                        lowerBoundAngle = 300
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(lowerBoundAngle, upperBoundAngle + 1)
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 10)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                        If rnd.NextDouble() < 0.2 Then
                            State(StateArmoredArmadillo.FreeFalling, 9)
                        End If
                    ElseIf PosY >= 250 Then
                        PosY = 249

                        Dim lowerBoundAngle, upperBoundAngle As Integer
                        upperBoundAngle = 60
                        lowerBoundAngle = 30
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(lowerBoundAngle, upperBoundAngle + 1)
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 10)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                        If rnd.NextDouble() < 0.2 Then
                            State(StateArmoredArmadillo.FreeFalling, 9)
                        End If
                    End If
                End If

            Case StateArmoredArmadillo.FreeFalling
                Dim rnd As New Random()
                GetNextFrame()
                If PosY >= 250 Then
                    PosY = 238
                    Vy = -10
                    Vx = 0
                    State(StateArmoredArmadillo.RollingRecoveryArmored, 10)
                End If

                If FDir = FaceDir.Left Then
                    PosX = PosX + Vx
                    PosY = PosY + Vy
                    Vy = Vy + gravity
                    If PosX <= 30 Then
                        PosX = 31

                        Dim lowerBoundAngle, upperBoundAngle As Integer
                        If Vy <= 0 Then
                            upperBoundAngle = 60
                            lowerBoundAngle = 30
                        Else
                            upperBoundAngle = 330
                            lowerBoundAngle = 300
                        End If
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(lowerBoundAngle, upperBoundAngle + 1)
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 10)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                        FDir = FaceDir.Right

                    ElseIf PosY <= 30 Then
                        PosY = 31

                        Dim lowerBoundAngle, upperBoundAngle As Integer
                        upperBoundAngle = 240
                        lowerBoundAngle = 210
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(lowerBoundAngle, upperBoundAngle + 1)
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 10)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                    End If

                ElseIf FDir = FaceDir.Right Then
                    PosX = PosX + Vx
                    PosY = PosY + Vy
                    Vy = Vy + gravity
                    If PosX >= 310 Then
                        PosX = 309

                        Dim lowerBoundAngle, upperBoundAngle As Integer
                        If Vy <= 0 Then
                            upperBoundAngle = 150
                            lowerBoundAngle = 120
                        Else
                            upperBoundAngle = 240
                            lowerBoundAngle = 210
                        End If
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(lowerBoundAngle, upperBoundAngle + 1)
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 10)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                        FDir = FaceDir.Left

                    ElseIf PosY <= 30 Then
                        PosY = 31

                        Dim lowerBoundAngle, upperBoundAngle As Integer
                        upperBoundAngle = 330
                        lowerBoundAngle = 300
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(lowerBoundAngle, upperBoundAngle + 1)
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 10)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                    End If
                End If

            Case StateArmoredArmadillo.RollingRecoveryArmored
                GetNextFrame()
                If FrameIdx = 6 Or PosY >= 238 Then
                    State(StateArmoredArmadillo.RollingRecoveryEndArmored, 11)
                End If
                PosX = PosX + Vx
                PosY = PosY + Vy
                Vy = Vy + gravity

            Case StateArmoredArmadillo.RollingRecoveryEndArmored
                If PosY >= 238 And Vy >= 0 Then
                    State(StateArmoredArmadillo.StandArmored, 0)
                    PosY = 238
                    Vx = 0
                    Vy = 0
                    If isIntro Then
                        isIntro = False
                        State(StateArmoredArmadillo.IntroAnimation, 2)
                    End If
                    isInRollingAnimation = False
                End If
                GetNextFrame()
                PosX = PosX + Vx
                PosY = PosY + Vy
                Vy = Vy + gravity

        End Select

    End Sub
End Class

Public Class CCharArmoredArmadilloProjectile
    Inherits CCharacter

    Public CurrState As StateArmoredArmadilloProjectile

    Public Sub State(state As StateArmoredArmadilloProjectile, idxspr As Integer)
        CurrState = state
        IdxArrSprites = idxspr
        CurrFrame = 0
        FrameIdx = 0

    End Sub

    Public Overrides Sub Update()
        Select Case CurrState
            Case StateArmoredArmadilloProjectile.Create
                GetNextFrame()
                If FrameIdx = 0 And CurrFrame = 0 Then
                    State(StateArmoredArmadilloProjectile.Horizontal, 1)
                End If

            Case StateArmoredArmadilloProjectile.Horizontal
                GetNextFrame()
                PosX = PosX + Vx
                If PosX >= 320 Or PosX <= 20 Then
                    Destroy = True
                End If
                If FDir = FaceDir.Left Then
                    Vx = -6
                Else
                    Vx = 6
                End If

        End Select
    End Sub

End Class


Public Class CElmtFrame
  Public CtrPoint As TPoint
  Public Top, Bottom, Left, Right As Integer

  Public MaxFrameTime As Integer

  Public Sub New(ctrx As Integer, ctry As Integer, l As Integer, t As Integer, r As Integer, b As Integer, mft As Integer)
    CtrPoint.x = ctrx
    CtrPoint.y = ctry
    Top = t
    Bottom = b
    Left = l
    Right = r
    MaxFrameTime = mft

  End Sub
End Class

Public Class CArrFrame
  Public N As Integer
  Public Elmt As CElmtFrame()

  Public Sub New()
    N = 0
    ReDim Elmt(-1)
  End Sub

  Public Overloads Sub Insert(E As CElmtFrame)
    ReDim Preserve Elmt(N)
    Elmt(N) = E
    N = N + 1
  End Sub

  Public Overloads Sub Insert(ctrx As Integer, ctry As Integer, l As Integer, t As Integer, r As Integer, b As Integer, mft As Integer)
    Dim E As CElmtFrame
    E = New CElmtFrame(ctrx, ctry, l, t, r, b, mft)
    ReDim Preserve Elmt(N)
    Elmt(N) = E
    N = N + 1

  End Sub

End Class

Public Structure TPoint
  Dim x As Integer
  Dim y As Integer

End Structure

