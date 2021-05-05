Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks


Public Class Form1
    Dim bmp As Bitmap
    Dim Bg, Img As CImage
    Dim SpriteMap As CImage
    Dim SpriteMask As CImage
    Dim AA_StandArmored,
        AA_WalkDebug, 'only for debuggin purposes, armored armadillo can't actually walk in the canonical lore of megaman extended universe
        AA_IntroAnimation,
        AA_Guard,
        AA_ShootArmored,
        AA_JumpStartArmored,
        AA_JumpArmored,
        AA_JumpEndArmored,
        AA_Rolling,
        AA_RollingRecoveryArmored,
        AA_RollingRecoveryEndArmored As CArrFrame
    Dim AA_ProjCreate1, AA_ProjHorizontal, AA_ProjHit As CArrFrame
    Dim ListChar As New List(Of CCharacter)
    Dim AA As CCharArmoredArmadillo




    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'open image for background, assign to bg

        ' Initialize the random-number generator.
        Randomize()

        Bg = New CImage
        Bg.OpenImage("C:\Dev\visual-studio-vb\2ndAttempt\scenery.bmp")

        Bg.CopyImg(Img)

        SpriteMap = New CImage
        SpriteMap.OpenImage("C:\Dev\visual-studio-vb\2ndAttempt\chara.bmp")

        SpriteMap.CreateMask(SpriteMask)

        'initialize sprites for Armored Armadillo
        AA_StandArmored = New CArrFrame
        AA_StandArmored.Insert(107, 71, 87, 50, 133, 93, 1)

        AA_WalkDebug = New CArrFrame
        AA_WalkDebug.Insert(174, 71, 151, 50, 207, 92, 1)
        AA_WalkDebug.Insert(41, 72, 17, 44, 76, 94, 1)


        AA_IntroAnimation = New CArrFrame
        AA_IntroAnimation.Insert(107, 71, 87, 50, 133, 93, 2)
        AA_IntroAnimation.Insert(174, 71, 151, 50, 207, 92, 2)
        AA_IntroAnimation.Insert(107, 71, 87, 50, 133, 93, 8)
        AA_IntroAnimation.Insert(174, 71, 151, 50, 207, 92, 8)
        AA_IntroAnimation.Insert(109, 131, 89, 107, 142, 150, 4)
        AA_IntroAnimation.Insert(176, 131, 157, 106, 210, 148, 4)
        AA_IntroAnimation.Insert(41, 72, 17, 44, 76, 94, 5)

        AA_Guard = New CArrFrame
        AA_Guard.Insert(109, 131, 89, 107, 142, 150, 2)
        'AA_Guard.Insert(176, 131, 157, 106, 210, 148, 2)

        AA_ShootArmored = New CArrFrame
        AA_ShootArmored.Insert(237, 72, 216, 48, 271, 93, 3)
        AA_ShootArmored.Insert(42, 129, 22, 105, 77, 149, 2)

        AA_JumpStartArmored = New CArrFrame
        AA_JumpStartArmored.Insert(124, 236, 99, 214, 148, 272, 1)

        AA_JumpArmored = New CArrFrame
        AA_JumpArmored.Insert(124, 236, 99, 214, 148, 272, 8)
        AA_JumpArmored.Insert(252, 177, 226, 154, 284, 204, 3)
        AA_JumpArmored.Insert(124, 236, 99, 214, 148, 272, 3)
        AA_JumpArmored.Insert(191, 234, 169, 214, 212, 272, 2)
        AA_JumpArmored.Insert(249, 241, 231, 221, 269, 263, 2)

        AA_JumpEndArmored = New CArrFrame
        AA_JumpEndArmored.Insert(249, 241, 231, 221, 269, 263, 1)

        AA_Rolling = New CArrFrame
        AA_Rolling.Insert(276, 487, 260, 474, 293, 501, 1)
        AA_Rolling.Insert(229, 489, 213, 472, 246, 505, 1)
        'AA_Rolling.Insert(82, 487, 66, 470, 99, 502, 1)
        'AA_Rolling.Insert(132, 486, 116, 470, 149, 502, 1)

        AA_RollingRecoveryArmored = New CArrFrame
        AA_RollingRecoveryArmored.Insert(276, 487, 260, 474, 293, 501, 1)
        AA_RollingRecoveryArmored.Insert(229, 489, 213, 472, 246, 505, 1)
        AA_RollingRecoveryArmored.Insert(276, 487, 260, 474, 293, 501, 1)
        AA_RollingRecoveryArmored.Insert(229, 489, 213, 472, 246, 505, 1)
        AA_RollingRecoveryArmored.Insert(249, 241, 231, 221, 269, 263, 2)
        AA_RollingRecoveryArmored.Insert(191, 234, 169, 214, 212, 272, 2)
        AA_RollingRecoveryArmored.Insert(54, 234, 28, 214, 77, 272, 2)
        AA_RollingRecoveryArmored.Insert(124, 236, 99, 214, 148, 272, 2)

        AA_RollingRecoveryEndArmored = New CArrFrame
        AA_RollingRecoveryEndArmored.Insert(124, 236, 99, 214, 148, 272, 2)

        AA = New CCharArmoredArmadillo
        ReDim AA.ArrSprites(11)
        AA.ArrSprites(0) = AA_StandArmored
        AA.ArrSprites(1) = AA_WalkDebug
        AA.ArrSprites(2) = AA_IntroAnimation
        AA.ArrSprites(3) = AA_Guard
        AA.ArrSprites(4) = AA_ShootArmored
        '5 is missing
        AA.ArrSprites(6) = AA_JumpStartArmored
        AA.ArrSprites(7) = AA_JumpArmored
        AA.ArrSprites(8) = AA_JumpEndArmored
        AA.ArrSprites(9) = AA_Rolling
        AA.ArrSprites(10) = AA_RollingRecoveryArmored
        AA.ArrSprites(11) = AA_RollingRecoveryEndArmored

        AA.PosX = 280
        AA.PosY = 70
        AA.Vx = 0
        AA.Vy = 10
        AA.State(StateArmoredArmadillo.Rolling, 9)
        AA.FDir = FaceDir.Left

        ListChar.Add(AA)

        'initialize sprites for Sprite Projectiles
        AA_ProjCreate1 = New CArrFrame
        AA_ProjCreate1.Insert(191, 22, 184, 14, 202, 29, 1)
        AA_ProjCreate1.Insert(252, 23, 243, 13, 264, 33, 1)

        AA_ProjHorizontal = New CArrFrame
        AA_ProjHorizontal.Insert(162, 22, 155, 13, 172, 30, 1)
        AA_ProjHorizontal.Insert(191, 22, 184, 14, 202, 29, 1)
        AA_ProjHorizontal.Insert(217, 23, 211, 15, 232, 30, 1)

        AA_ProjHit = New CArrFrame
        AA_ProjHit.Insert(104, 20, 97, 13, 112, 28, 2)
        AA_ProjHit.Insert(134, 21, 128, 15, 141, 28, 1)
        AA_ProjHit.Insert(76, 21, 68, 12, 85, 29, 1)
        AA_ProjHit.Insert(134, 21, 128, 15, 141, 28, 1)

        'TODO: add on hit sprite


        bmp = New Bitmap(Img.Width, Img.Height)


        DisplayImg()
        ResizeImg()




        Timer1.Enabled = True
    End Sub

    Sub PutSprites()
        Dim cc As CCharacter

        'set background
        'Dim w, h As Integer
        'w = Img.Width - 1
        'h = Img.Height - 1

        'Parallel.For(0, w - 1, _
        '   Sub(i)
        '     Parallel.For(0, h - 1, _
        '       Sub(j)
        '         For j = 0 To h - 1
        '           Img.Elmt(i, j) = Bg.Elmt(i, j)
        '         Next

        '       End Sub)
        '   End Sub)


        For i = 0 To Img.Width - 1
            For j = 0 To Img.Height - 1
                Img.Elmt(i, j) = Bg.Elmt(i, j)
            Next
        Next


        For Each cc In ListChar
            Dim EF As CElmtFrame = cc.ArrSprites(cc.IdxArrSprites).Elmt(cc.FrameIdx)
            Dim spritewidth = EF.Right - EF.Left
            Dim spriteheight = EF.Bottom - EF.Top
            If cc.FDir = FaceDir.Left Then
                Dim spriteleft As Integer = cc.PosX - EF.CtrPoint.x + EF.Left
                Dim spritetop As Integer = cc.PosY - EF.CtrPoint.y + EF.Top
                'set mask
                For i = 0 To spritewidth
                    For j = 0 To spriteheight
                        Img.Elmt(spriteleft + i, spritetop + j) = OpAnd(Img.Elmt(spriteleft + i, spritetop + j), SpriteMask.Elmt(EF.Left + i, EF.Top + j))
                    Next
                Next

                'set sprite
                For i = 0 To spritewidth
                    For j = 0 To spriteheight
                        Img.Elmt(spriteleft + i, spritetop + j) = OpOr(Img.Elmt(spriteleft + i, spritetop + j), SpriteMap.Elmt(EF.Left + i, EF.Top + j))
                    Next
                Next
            Else 'facing right
                Dim spriteleft = cc.PosX + EF.CtrPoint.x - EF.Right
                Dim spritetop = cc.PosY - EF.CtrPoint.y + EF.Top
                'set mask
                For i = 0 To spritewidth
                    For j = 0 To spriteheight
                        Img.Elmt(spriteleft + i, spritetop + j) = OpAnd(Img.Elmt(spriteleft + i, spritetop + j), SpriteMask.Elmt(EF.Right - i, EF.Top + j))
                    Next
                Next

                'set sprite
                For i = 0 To spritewidth
                    For j = 0 To spriteheight
                        Img.Elmt(spriteleft + i, spritetop + j) = OpOr(Img.Elmt(spriteleft + i, spritetop + j), SpriteMap.Elmt(EF.Right - i, EF.Top + j))
                    Next
                Next

            End If

        Next


    End Sub

    Sub DisplayImg()
        'display bg and sprite on picturebox
        Dim i, j As Integer
        'Dim sw As New System.Diagnostics.Stopwatch

        'sw.Start()

        PutSprites()



        Dim rect As New Rectangle(0, 0, bmp.Width, bmp.Height)
        Dim bmpdata As System.Drawing.Imaging.BitmapData = bmp.LockBits(rect,
     System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat)

        'Get the address of the first line.
        Dim ptr As IntPtr = bmpdata.Scan0


        'Declare an array to hold the bytes of the bitmap.
        Dim bytes As Integer = Math.Abs(bmpdata.Stride) * bmp.Height
        Dim rgbvalues(bytes) As Byte


        'Copy the RGB values into the array.
        System.Runtime.InteropServices.Marshal.Copy(ptr, rgbvalues, 0, bytes)

        Dim n As Integer = 0
        Dim col As System.Drawing.Color

        'Set every third value to 255. A 24bpp bitmap will look red.  
        For j = 0 To Img.Height - 1
            For i = 0 To Img.Width - 1
                col = Img.Elmt(i, j)
                rgbvalues(n) = col.B
                rgbvalues(n + 1) = col.G
                rgbvalues(n + 2) = col.R
                rgbvalues(n + 3) = col.A

                n = n + 4
            Next
        Next

        'Copy the RGB values back to the bitmap
        System.Runtime.InteropServices.Marshal.Copy(rgbvalues, 0, ptr, bytes)


        'Unlock the bits.
        bmp.UnlockBits(bmpdata)

        'Timer1.Enabled = False

        'MsgBox(CStr(bmp.GetPixel(0, 0).A) + " " + CStr(bmp.GetPixel(0, 0).R) + " " + CStr(bmp.GetPixel(0, 0).G) + " " + CStr(bmp.GetPixel(0, 0).B))
        'MsgBox(CStr(bmp.GetPixel(1, 0).A) + " " + CStr(bmp.GetPixel(1, 0).R) + " " + CStr(bmp.GetPixel(1, 0).G) + " " + CStr(bmp.GetPixel(1, 0).B))



        PictureBox1.Refresh()

        PictureBox1.Image = bmp
        PictureBox1.Width = bmp.Width
        PictureBox1.Height = bmp.Height
        PictureBox1.Top = 0
        PictureBox1.Left = 0





    End Sub



    Sub ResizeImg()
        Dim w, h As Integer

        w = PictureBox1.Width
        h = PictureBox1.Height

        Me.ClientSize = New Size(w, h)

    End Sub




    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Dim CC As CCharacter


        PictureBox1.Refresh()

        For Each CC In ListChar
            CC.Update()

        Next

        If AA.CurrState = StateArmoredArmadillo.ShootArmored And AA.CurrFrame = 2 Then
            CreateArmoredArmadilloProjectile(1)
        End If

        Dim Listchar1 As New List(Of CCharacter)

        For Each CC In ListChar
            If Not CC.Destroy Then
                Listchar1.Add(CC)
            End If
        Next

        ListChar = Listchar1

        DisplayImg()

    End Sub

    Sub CreateArmoredArmadilloProjectile(n As Integer)
        Dim SP As CCharArmoredArmadilloProjectile

        SP = New CCharArmoredArmadilloProjectile
        If AA.FDir = FaceDir.Left Then
            SP.PosX = AA.PosX - 20
            SP.FDir = FaceDir.Left
        Else
            SP.PosX = AA.PosX + 20
            SP.FDir = FaceDir.Right
        End If

        SP.PosY = AA.PosY - 10

        SP.Vx = 0
        SP.Vy = 0
        SP.CurrState = StateArmoredArmadilloProjectile.Create
        ReDim SP.ArrSprites(2)
        If n = 1 Then
            SP.ArrSprites(0) = AA_ProjCreate1
        End If
        SP.ArrSprites(1) = AA_ProjHorizontal
        SP.ArrSprites(2) = AA_ProjHit

        ListChar.Add(SP)
    End Sub

    Private Sub ArmoredArmadillo_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Left And AA.CurrState = StateArmoredArmadillo.StandArmored Then
            AA.isWalking = True
            AA.FDir = FaceDir.Left
        ElseIf e.KeyCode = Keys.Right And AA.CurrState = StateArmoredArmadillo.StandArmored Then
            AA.isWalking = True
            AA.FDir = FaceDir.Right
        ElseIf e.KeyCode = Keys.Down And AA.CurrState = StateArmoredArmadillo.StandArmored Then
            AA.isGuarding = True
        ElseIf e.KeyCode = Keys.S And AA.CurrState = StateArmoredArmadillo.StandArmored Then
            AA.isShooting = True
        ElseIf e.KeyCode = Keys.Space And AA.CurrState = StateArmoredArmadillo.StandArmored Then
            AA.isInRollingAnimation = True
        End If
    End Sub

    Private Sub ArmoredArmadillo_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp

        If e.KeyCode = Keys.Down Then
            AA.isGuarding = False
        End If
    End Sub

End Class
