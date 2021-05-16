Imports System.IO
Public Enum StateArmoredArmadillo
    StandArmored
    WalkDebug
    IntroAnimation
    Guard
    ShootArmored
    StaggeredArmored
    JumpStartArmored
    JumpArmored
    JumpEndArmored
    Rolling
    RollingRecoveryArmored
    RollingRecoveryEndArmored
    FreeFalling
    DeflectSpecial
End Enum

Public Enum StateMegaman
    Spawn
    Run
    Shoot
    Die
    JumpStart
    Jump
    JumpEnd
    Stand
    JumpDown
    Staggered
End Enum

Public Enum StateArmoredArmadilloProjectile
    Create
    Horizontal
    Hit
    EightWay
End Enum

Public Enum StateMegamanProjectile
    Create
    Horizontal
    Hit
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

    Public Overridable Sub Update(ByRef AllCharHitboxes As Hitboxes, ByRef Events() As Boolean)

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
    Public CurrState As StateArmoredArmadillo

    Public Sub State(state As StateArmoredArmadillo, idxspr As Integer)
        CurrState = state
        IdxArrSprites = idxspr
        CurrFrame = 0
        FrameIdx = 0

    End Sub


    Public Overrides Sub Update(ByRef AllCharHitboxes As Hitboxes, ByRef Events() As Boolean)
        Dim MMHitbox As HitboxClass = AllCharHitboxes.MegamanHitbox
        Dim AAHitbox As HitboxClass = AllCharHitboxes.ArmoredArmadilloHitbox
        If Events(0) And Not Events(5) Then
            If AAHitbox.CtrPoint.x < MMHitbox.CtrPoint.x Then
                'shot from the right
                Vx = -1
                FDir = FaceDir.Right
            Else
                'shot from the left
                Vx = 1
                FDir = FaceDir.Left
            End If
            Vy = 0
            Events(5) = True
            State(StateArmoredArmadillo.StaggeredArmored, 5)
        End If
        If PosX <= MMHitbox.Right And PosX >= MMHitbox.Left And PosY >= MMHitbox.Top And PosY <= MMHitbox.Bottom And Not Events(3) Then
            Events(6) = True
        End If
        Select Case CurrState
            Case StateArmoredArmadillo.StandArmored
                GetNextFrame()
                'If isWalking Then
                '    State(StateArmoredArmadillo.WalkDebug, 1)
                If isGuarding Then
                    State(StateArmoredArmadillo.Guard, 3)
                ElseIf isShooting Then
                    State(StateArmoredArmadillo.ShootArmored, 4)
                ElseIf isInRollingAnimation Then
                    State(StateArmoredArmadillo.JumpStartArmored, 6)
                End If
            'Case StateArmoredArmadillo.WalkDebug
            '    PosX = PosX + Vx
            '    If FDir = FaceDir.Left And isWalking = True Then
            '        Vx = -5
            '        PosX = PosX + Vx
            '        If AAHitbox.Left <= 22 Then
            '            Vx *= -1
            '            FDir = FaceDir.Right
            '        End If

            '    ElseIf FDir = FaceDir.Right And isWalking = True Then
            '        Vx = 5
            '        PosX = PosX + Vx
            '        If AAHitbox.Right >= 323 Then
            '            Vx *= -1
            '            FDir = FaceDir.Left
            '        End If
            '    End If
            '    ' stopping walk
            '    'stopping left
            '    If FrameIdx = 0 And PosX >= PosX + Vx Then
            '        isWalking = False
            '        State(StateArmoredArmadillo.StandArmored, 0)
            '        'stopping right
            '    ElseIf CurrFrame = 0 And PosX <= PosX + Vx Then
            '        isWalking = False
            '        State(StateArmoredArmadillo.StandArmored, 0)
            '    End If
            '    GetNextFrame()
            Case StateArmoredArmadillo.IntroAnimation
                GetNextFrame()
                If CurrFrame = 4 And FrameIdx = 6 Then
                    Events(5) = False
                    State(StateArmoredArmadillo.StandArmored, 0)
                End If

            Case StateArmoredArmadillo.ShootArmored
                If FrameIdx = 1 And CurrFrame = 1 Then
                    isShooting = False
                    State(StateArmoredArmadillo.StandArmored, 0)
                End If
                GetNextFrame()

            Case StateArmoredArmadillo.Guard
                If isGuarding = False Then
                    State(StateArmoredArmadillo.StandArmored, 0)
                End If
                If Events(7) Then
                    State(StateArmoredArmadillo.DeflectSpecial, 12)
                End If
                GetNextFrame()

            Case StateArmoredArmadillo.DeflectSpecial
                If FrameIdx = 9 And CurrFrame = 4 Then
                    State(StateArmoredArmadillo.StandArmored, 0)
                    Events(4) = False
                    isGuarding = False
                    Events(7) = False
                End If
                GetNextFrame()

            Case StateArmoredArmadillo.StaggeredArmored
                isInRollingAnimation = False 'in case aa was hit when jumping or when recovering from rolling
                If AAHitbox.Right >= 323 Then
                    PosX = 323 - AAHitbox.relativeDistance(2)
                    Vx = 0
                ElseIf AAHitbox.Left <= 22 Then
                    PosX = 22 + AAHitbox.relativeDistance(0)
                    Vx = 0
                End If
                If AAHitbox.Bottom >= 255 Then
                    Vy = 0
                Else
                    Vy = Vy + gravity
                End If
                PosX = PosX + Vx
                PosY = PosY + Vy
                If FrameIdx = 5 And CurrFrame = 1 And AAHitbox.Bottom >= 255 Then
                    Events(0) = False
                    Events(5) = False
                    State(StateArmoredArmadillo.StandArmored, 0)
                End If
                GetNextFrame()

            Case StateArmoredArmadillo.JumpStartArmored
                Vy = -12
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
                If AAHitbox.Bottom >= 255 And Vy > 0 Then
                    Events(4) = True
                    Vy = 0
                    State(StateArmoredArmadillo.Rolling, 9)
                    PosY = 238
                    If FDir = FaceDir.Left Then
                        Vx = -20
                    Else
                        Vx = 20
                    End If
                End If

            Case StateArmoredArmadillo.Rolling
                Dim rnd As New Random()
                GetNextFrame()
                If PosX >= MMHitbox.Left And PosX <= MMHitbox.Right And PosY >= MMHitbox.Top And PosY <= MMHitbox.Bottom And Events(3) = False Then
                    Events(1) = True 'MM is hit
                End If
                If isIntro Then
                    PosY = PosY + Vy
                    Vy = Vy + gravity
                    If AAHitbox.Bottom >= 255 Then
                        PosY = (255 - AAHitbox.relativeDistance(3))
                        Vy = -12
                        State(StateArmoredArmadillo.RollingRecoveryArmored, 10)
                    End If

                ElseIf FDir = FaceDir.Left Then
                    PosX = PosX + Vx
                    PosY = PosY + Vy
                    If AAHitbox.Left <= 22 Then
                        PosX = 22 + AAHitbox.relativeDistance(0)

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
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 20)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                        FDir = FaceDir.Right
                        If rnd.NextDouble() < 0.2 Then
                            State(StateArmoredArmadillo.FreeFalling, 9)
                        End If

                    ElseIf AAHitbox.Top <= 18 Then
                        PosY = 18 + AAHitbox.relativeDistance(1)

                        Dim lowerBoundAngle, upperBoundAngle As Integer
                        upperBoundAngle = 240
                        lowerBoundAngle = 210
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(lowerBoundAngle, upperBoundAngle + 1)
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 20)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                        If rnd.NextDouble() < 0.2 Then
                            State(StateArmoredArmadillo.FreeFalling, 9)
                        End If
                    ElseIf AAHitbox.Bottom >= 255 Then
                        PosY = 255 - AAHitbox.relativeDistance(3)

                        Dim lowerBoundAngle, upperBoundAngle As Integer
                        upperBoundAngle = 150
                        lowerBoundAngle = 120
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(lowerBoundAngle, upperBoundAngle + 1)
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 20)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                        If rnd.NextDouble() < 0.2 Then
                            State(StateArmoredArmadillo.FreeFalling, 9)
                        End If
                    End If

                ElseIf FDir = FaceDir.Right Then
                    PosX = PosX + Vx
                    PosY = PosY + Vy
                    If AAHitbox.Right >= 323 Then
                        PosX = 323 - AAHitbox.relativeDistance(2)

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
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 20)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                        FDir = FaceDir.Left
                        If rnd.NextDouble() < 0.2 Then
                            State(StateArmoredArmadillo.FreeFalling, 9)
                        End If

                    ElseIf AAHitbox.Top <= 18 Then
                        PosY = 18 + AAHitbox.relativeDistance(1)

                        Dim lowerBoundAngle, upperBoundAngle As Integer
                        upperBoundAngle = 330
                        lowerBoundAngle = 300
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(lowerBoundAngle, upperBoundAngle + 1)
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 20)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                        If rnd.NextDouble() < 0.2 Then
                            State(StateArmoredArmadillo.FreeFalling, 9)
                        End If
                    ElseIf AAHitbox.Bottom >= 255 Then
                        PosY = 255 - AAHitbox.relativeDistance(3)

                        Dim lowerBoundAngle, upperBoundAngle As Integer
                        upperBoundAngle = 60
                        lowerBoundAngle = 30
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(lowerBoundAngle, upperBoundAngle + 1)
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 20)
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
                If PosX >= MMHitbox.Left And PosX <= MMHitbox.Right And PosY >= MMHitbox.Top And PosY <= MMHitbox.Bottom And Events(3) = False Then
                    Events(1) = True 'MM is hit
                End If
                If AAHitbox.Bottom >= 255 Then
                    PosY = 249
                    Vy = -12
                    Vx = 0
                    Events(4) = False
                    State(StateArmoredArmadillo.RollingRecoveryArmored, 10)
                End If

                If FDir = FaceDir.Left Then
                    PosX = PosX + Vx
                    PosY = PosY + Vy
                    Vy = Vy + gravity
                    If AAHitbox.Left <= 22 Then
                        PosX = 22 + AAHitbox.relativeDistance(0)

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
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 5)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                        FDir = FaceDir.Right

                    ElseIf AAHitbox.Top <= 18 Then
                        PosY = 18 + AAHitbox.relativeDistance(1)

                        Dim lowerBoundAngle, upperBoundAngle As Integer
                        upperBoundAngle = 240
                        lowerBoundAngle = 210
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(lowerBoundAngle, upperBoundAngle + 1)
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 5)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                    End If

                ElseIf FDir = FaceDir.Right Then
                    PosX = PosX + Vx
                    PosY = PosY + Vy
                    Vy = Vy + gravity
                    If AAHitbox.Right >= 323 Then
                        PosX = 323 - AAHitbox.relativeDistance(2)

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
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 5)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                        FDir = FaceDir.Left

                    ElseIf AAHitbox.Top <= 18 Then
                        PosY = 18 + AAHitbox.relativeDistance(1)

                        Dim lowerBoundAngle, upperBoundAngle As Integer
                        upperBoundAngle = 330
                        lowerBoundAngle = 300
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(lowerBoundAngle, upperBoundAngle + 1)
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, 5)
                        Vx = Velocity(0)
                        Vy = Velocity(1) * -1
                    End If
                End If

            Case StateArmoredArmadillo.RollingRecoveryArmored
                GetNextFrame()
                If FrameIdx = 6 Then
                    State(StateArmoredArmadillo.RollingRecoveryEndArmored, 11)
                End If
                PosX = PosX + Vx
                PosY = PosY + Vy
                Vy = Vy + gravity

            Case StateArmoredArmadillo.RollingRecoveryEndArmored
                If AAHitbox.Bottom >= 255 And Vy >= 0 Then
                    State(StateArmoredArmadillo.StandArmored, 0)
                    PosY = 234
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

    Public Overrides Sub Update(ByRef AllCharHitboxes As Hitboxes, ByRef Events() As Boolean)
        Dim MMHitbox As HitboxClass = AllCharHitboxes.MegamanHitbox
        Dim AAHitbox As HitboxClass = AllCharHitboxes.ArmoredArmadilloHitbox
        Select Case CurrState
            Case StateArmoredArmadilloProjectile.Create
                GetNextFrame()
                If FrameIdx = 0 And CurrFrame = 0 Then
                    State(StateArmoredArmadilloProjectile.Horizontal, 1)
                End If

            Case StateArmoredArmadilloProjectile.Horizontal
                GetNextFrame()
                PosX = PosX + Vx
                If PosX >= MMHitbox.Left And PosX <= MMHitbox.Right And PosY >= MMHitbox.Top And PosY <= MMHitbox.Bottom And Events(3) = False Then
                    State(StateArmoredArmadilloProjectile.Hit, 2)
                    Events(1) = True 'MM is hit
                End If
                If PosX >= 320 Or PosX <= 20 Then
                    State(StateArmoredArmadilloProjectile.Hit, 2)
                End If
                If FDir = FaceDir.Left Then
                    Vx = -6
                Else
                    Vx = 6
                End If

            Case StateArmoredArmadilloProjectile.EightWay
                GetNextFrame()
                PosX = PosX + Vx
                PosY = PosY + Vy
                If PosX >= MMHitbox.Left And PosX <= MMHitbox.Right And PosY >= MMHitbox.Top And PosY <= MMHitbox.Bottom And Events(3) = False Then
                    State(StateArmoredArmadilloProjectile.Hit, 2)
                    Events(1) = True 'MM is hit
                End If
                If PosX >= 320 Or PosX <= 20 Or PosY <= 18 Or PosY >= 255 Then
                    State(StateArmoredArmadilloProjectile.Hit, 2)
                End If

            Case StateArmoredArmadilloProjectile.Hit
                If FrameIdx = 3 And CurrFrame = 0 Then
                    Destroy = True
                End If
                GetNextFrame()

        End Select
    End Sub

End Class

Public Class CCharMegaman
    Inherits CCharacter

    Public CurrState As StateMegaman

    Public Sub State(state As StateMegaman, idxspr As Integer)
        CurrState = state
        IdxArrSprites = idxspr
        CurrFrame = 0
        FrameIdx = 0

    End Sub


    Public Overrides Sub Update(ByRef AllCharHitboxes As Hitboxes, ByRef Events() As Boolean)
        Dim MMHitbox As HitboxClass = AllCharHitboxes.MegamanHitbox
        Dim AAHitbox As HitboxClass = AllCharHitboxes.ArmoredArmadilloHitbox
        If Events(1) And Not Events(3) Then
            Vy = -10
            If AAHitbox.CtrPoint.x > MMHitbox.CtrPoint.x Then
                'shot from the right
                Vx = -5
                FDir = FaceDir.Right
            Else
                'shot from the left
                Vx = 5
                FDir = FaceDir.Left
            End If
            State(StateMegaman.Die, 3)
            Events(6) = False 'rolling towards mm technically triggers this event as well
        ElseIf Events(6) And Not Events(3) Then
            If AAHitbox.CtrPoint.x > MMHitbox.CtrPoint.x Then
                'bodied from the right
                Vx = -5
                FDir = FaceDir.Right
            Else
                'bodied from the left
                Vx = 5
                FDir = FaceDir.Left
            End If
            Vy = 0
            State(StateMegaman.Staggered, 9)
        End If
        Select Case CurrState
            Case StateMegaman.Spawn

                If FrameIdx = 7 And CurrFrame = 1 Then
                    Events(3) = False
                    State(StateMegaman.Stand, 7)
                End If
                GetNextFrame()

            Case StateMegaman.Stand
                If AAHitbox.CtrPoint.x >= MMHitbox.CtrPoint.x Then
                    'AA is to the right of MM
                    Vx = 5
                    FDir = FaceDir.Right
                Else
                    'AA is to the left of MM
                    Vx = -5
                    FDir = FaceDir.Left
                End If
                If Math.Abs(AAHitbox.CtrPoint.x - MMHitbox.CtrPoint.x) >= 150 Then
                    State(StateMegaman.Run, 1)
                Else
                    Dim rnd As New Random()
                    If rnd.NextDouble() < 0.5 Then
                        State(StateMegaman.Shoot, 2)
                    Else
                        State(StateMegaman.JumpStart, 4)
                    End If
                End If
                GetNextFrame()

            Case StateMegaman.Run
                If Math.Abs(AAHitbox.CtrPoint.x - MMHitbox.CtrPoint.x) <= 150 Then
                    State(StateMegaman.Shoot, 2)
                End If
                PosX = PosX + Vx
                GetNextFrame()

            Case StateMegaman.Die
                GetNextFrame()
                Events(3) = True 'no collision in this state
                PosY = PosY + Vy
                PosX = PosX + Vx
                Vy = Vy + gravity
                If FrameIdx = 7 And CurrFrame = 7 Then
                    Events(1) = False ' MM is hit
                    Events(2) = False 'MM is dead
                    Destroy = True
                End If

            Case StateMegaman.Staggered
                GetNextFrame()
                Events(3) = True 'no collision in this state
                If MMHitbox.Right >= 323 Then
                    PosX = 323 - MMHitbox.relativeDistance(2)
                    Vx = 0
                ElseIf MMHitbox.Left <= 22 Then
                    PosX = 22 + MMHitbox.relativeDistance(0)
                    Vx = 0
                End If
                If MMHitbox.Bottom >= 255 Then
                    Vy = 0
                    PosY = 255 - MMHitbox.relativeDistance(3)
                Else
                    Vy = Vy + gravity
                End If
                PosX = PosX + Vx
                PosY = PosY + Vy
                If FrameIdx = 0 And CurrFrame = 7 And MMHitbox.Bottom >= 255 Then
                    Events(6) = False
                    Events(3) = False
                    Vx = 0
                    PosY = 238
                    State(StateMegaman.Stand, 7)
                End If

            Case StateMegaman.Shoot
                If FrameIdx = 1 And CurrFrame = 1 Then
                    State(StateMegaman.Stand, 7)
                End If
                GetNextFrame()

            Case StateMegaman.JumpStart
                PosY = 237
                Vy = -12
                Dim Rnd As New Random
                Dim RandomizedVx As Integer = Rnd.Next(0, 7)
                If FDir = FaceDir.Right Then
                    Vx = RandomizedVx
                Else
                    Vx = RandomizedVx * -1
                End If
                GetNextFrame()
                If CurrFrame = 2 Then
                    State(StateMegaman.Jump, 5)
                End If
            Case StateMegaman.Jump
                PosX = PosX + Vx
                PosY = PosY + Vy
                Vy = Vy + gravity
                GetNextFrame()
                If MMHitbox.Right >= 323 Then
                    Vx = 0
                    PosX = 323 - MMHitbox.relativeDistance(2)
                ElseIf MMHitbox.Left <= 22 Then
                    Vx = 0
                    PosX = 22 + MMHitbox.relativeDistance(0)
                End If
                If Vy >= 0 Then
                    State(StateMegaman.JumpDown, 8)
                End If
            Case StateMegaman.JumpDown
                PosX = PosX + Vx
                PosY = PosY + Vy
                Vy = Vy + gravity
                If MMHitbox.Right >= 323 Then
                    Vx = 0
                    PosX = 323 - MMHitbox.relativeDistance(2)
                ElseIf MMHitbox.Left <= 22 Then
                    Vx = 0
                    PosX = 22 + MMHitbox.relativeDistance(0)
                End If
                GetNextFrame()
                If MMHitbox.Bottom >= 255 Then
                    State(StateMegaman.JumpEnd, 6)
                End If

            Case StateMegaman.JumpEnd
                PosY = 238
                GetNextFrame()
                If FrameIdx = 2 And CurrFrame = 2 Then
                    State(StateMegaman.Stand, 7)
                End If
        End Select

    End Sub
End Class

Public Class CCharMegamanProjectile
    Inherits CCharacter

    Public CurrState As StateMegamanProjectile

    Public Sub State(state As StateMegamanProjectile, idxspr As Integer)
        CurrState = state
        IdxArrSprites = idxspr
        CurrFrame = 0
        FrameIdx = 0

    End Sub

    Public Overrides Sub Update(ByRef AllCharHitboxes As Hitboxes, ByRef Events() As Boolean)
        Dim MMHitbox As HitboxClass = AllCharHitboxes.MegamanHitbox
        Dim AAHitbox As HitboxClass = AllCharHitboxes.ArmoredArmadilloHitbox
        Select Case CurrState
            Case StateMegamanProjectile.Create
                GetNextFrame()
                If FrameIdx = 0 And CurrFrame = 0 Then
                    If FDir = FaceDir.Left Then
                        Vx = -6
                    Else
                        Vx = 6
                    End If
                    State(StateMegamanProjectile.Horizontal, 1)
                End If

            Case StateMegamanProjectile.Horizontal
                GetNextFrame()
                PosX = PosX + Vx
                PosY = PosY + Vy
                If PosX >= AAHitbox.Left And PosX <= AAHitbox.Right And PosY >= AAHitbox.Top And PosY <= AAHitbox.Bottom And Not Events(5) Then
                    If Events(4) = True Then
                        Dim rnd As New Random
                        ' Generate random value between the two angle
                        Dim RandomizedAngle As Integer = rnd.Next(30, 60 + 1)
                        Dim RandomizedDir As Double = rnd.NextDouble()
                        Dim Velocity As Double() = FindComponentVector(RandomizedAngle, Vx)
                        Vy = Velocity(1)
                        Vx = Vx * -1
                        If RandomizedDir >= 0.5 Then
                            Vy = Vy * -1
                        End If
                        If FDir = FaceDir.Left Then
                            FDir = FaceDir.Right
                        Else
                            FDir = FaceDir.Left
                        End If
                        Events(7) = True
                    Else
                        State(StateMegamanProjectile.Hit, 2)
                        Events(0) = True 'AA is hit
                    End If
                ElseIf PosX >= MMHitbox.Left And PosX <= MMHitbox.Right And PosY >= MMHitbox.Top And PosY <= MMHitbox.Bottom And Not Events(3) Then
                    State(StateMegamanProjectile.Hit, 2)
                    Events(1) = True 'MM is hit by the deflected projectile
                End If
                If PosX >= 320 Or PosX <= 20 Or PosY >= 250 Or PosY <= 30 Then
                    State(StateMegamanProjectile.Hit, 2)
                End If

            Case StateMegamanProjectile.Hit
                If FrameIdx = 3 And CurrFrame = 0 Then
                    Destroy = True
                End If
                GetNextFrame()

        End Select
    End Sub

End Class


Public Class CElmtFrame
    Public CtrPoint As TPoint
    Public Top, Bottom, Left, Right As Integer
    Public RelativeDistance(4) As Integer 'for the current sprite, give the relative distance of l,t,r,b from ctrpoint

    Public MaxFrameTime As Integer

    Public Sub New(ctrx As Integer, ctry As Integer, l As Integer, t As Integer, r As Integer, b As Integer, mft As Integer)
        CtrPoint.x = ctrx
        CtrPoint.y = ctry
        Top = t
        Bottom = b
        Left = l
        Right = r
        MaxFrameTime = mft
        RelativeDistance(0) = CtrPoint.x - Left
        RelativeDistance(1) = CtrPoint.y - Top
        RelativeDistance(2) = Right - CtrPoint.x
        RelativeDistance(3) = Bottom - CtrPoint.y
    End Sub
End Class

Public Class HitboxClass
    'derived from CElmtFrame class, but fixes the left and right boundaries accordingly (e.g. takes into account whether the 
    'character object was facing to the left (uses default attributes of CElmtFrame since the spritesheet is left-facing) or otherwise)
    Public CtrPoint As TPoint
    Public Top, Bottom, Left, Right As Integer
    Public relativeDistance(4) As Integer

    Public Sub UpdateHitbox(ByRef character As CCharacter)
        Dim CurrSprite As CElmtFrame = character.ArrSprites(character.IdxArrSprites).Elmt(character.FrameIdx)
        relativeDistance = CurrSprite.RelativeDistance

        CtrPoint.x = character.PosX
        CtrPoint.y = character.PosY

        Top = character.PosY - relativeDistance(1)
        Bottom = character.PosY + relativeDistance(3)
        If character.FDir = FaceDir.Left Then
            Left = character.PosX - relativeDistance(0)
            Right = character.PosX + relativeDistance(2)
        Else
            Left = character.PosX - relativeDistance(2)
            Right = character.PosX + relativeDistance(0)
        End If
    End Sub
End Class

Public Class Hitboxes
    'could've just use array, but this helps differentiating the two char as opposed to just use 0 for AA and 1 for MM, for instance
    Public ArmoredArmadilloHitbox As HitboxClass
    Public MegamanHitbox As HitboxClass

    'just think of this as c struct, idk if it exists here but im too lazy to find out
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

