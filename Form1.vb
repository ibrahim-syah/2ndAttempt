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
        AA_StaggeredArmored,
        AA_JumpStartArmored,
        AA_JumpArmored,
        AA_JumpEndArmored,
        AA_Rolling,
        AA_RollingRecoveryArmored,
        AA_RollingRecoveryEndArmored,
        AA_DeflectSpecial As CArrFrame
    Dim AA_ProjCreate1, AA_ProjHorizontal, AA_ProjHit, AA_ProjEightWay As CArrFrame
    Dim MM_Spawn, MM_Run, MM_Shoot, MM_JumpStart, MM_Jump, MM_JumpEnd, MM_Die, MM_Stand, MM_JumpDown, MM_Staggered As CArrFrame
    Dim MM_ProjCreate1, MM_ProjHorizontal, MM_ProjHit As CArrFrame
    Dim ListChar As New List(Of CCharacter)
    Dim AA As CCharArmoredArmadillo
    Dim MM As CCharMegaman
    Dim AllCharHitboxes As New Hitboxes
    Dim Events(7) As Boolean 'Hold certain flags and triggers e.g. 0 is AA getting hit, 1 is MM getting hit




    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'open image for background, assign to bg

        ' Initialize the random-number generator.
        'Randomize()

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

        AA_StaggeredArmored = New CArrFrame
        AA_StaggeredArmored.Insert(48, 309, 19, 285, 79, 333, 2)
        AA_StaggeredArmored.Insert(34, 588, 5, 564, 65, 612, 2)
        AA_StaggeredArmored.Insert(48, 309, 19, 285, 79, 333, 2)
        AA_StaggeredArmored.Insert(34, 588, 5, 564, 65, 612, 2)
        AA_StaggeredArmored.Insert(48, 309, 19, 285, 79, 333, 2)
        AA_StaggeredArmored.Insert(34, 588, 5, 564, 65, 612, 2)

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

        AA_DeflectSpecial = New CArrFrame
        AA_DeflectSpecial.Insert(247, 126, 225, 103, 278, 145, 2)
        AA_DeflectSpecial.Insert(47, 185, 27, 161, 80, 203, 2)
        AA_DeflectSpecial.Insert(117, 183, 97, 160, 150, 203, 2)
        AA_DeflectSpecial.Insert(247, 126, 225, 103, 278, 145, 2)
        AA_DeflectSpecial.Insert(47, 185, 27, 161, 80, 203, 2)
        AA_DeflectSpecial.Insert(117, 183, 97, 160, 150, 203, 2)
        AA_DeflectSpecial.Insert(247, 126, 225, 103, 278, 145, 2)
        AA_DeflectSpecial.Insert(47, 185, 27, 161, 80, 203, 2)
        AA_DeflectSpecial.Insert(117, 183, 97, 160, 150, 203, 2)
        AA_DeflectSpecial.Insert(185, 180, 165, 155, 217, 203, 5)


        AA = New CCharArmoredArmadillo
        ReDim AA.ArrSprites(12)
        AA.ArrSprites(0) = AA_StandArmored
        AA.ArrSprites(1) = AA_WalkDebug
        AA.ArrSprites(2) = AA_IntroAnimation
        AA.ArrSprites(3) = AA_Guard
        AA.ArrSprites(4) = AA_ShootArmored
        AA.ArrSprites(5) = AA_StaggeredArmored
        AA.ArrSprites(6) = AA_JumpStartArmored
        AA.ArrSprites(7) = AA_JumpArmored
        AA.ArrSprites(8) = AA_JumpEndArmored
        AA.ArrSprites(9) = AA_Rolling
        AA.ArrSprites(10) = AA_RollingRecoveryArmored
        AA.ArrSprites(11) = AA_RollingRecoveryEndArmored
        AA.ArrSprites(12) = AA_DeflectSpecial

        AA.PosX = 280
        AA.PosY = 70
        AA.Vx = 0
        AA.Vy = 10
        AA.State(StateArmoredArmadillo.Rolling, 9)
        AA.FDir = FaceDir.Left

        ListChar.Add(AA)

        'clean this up
        Events(0) = False 'is AA getting hit?
        Events(1) = False 'is MM getting hit by projectile or roll?
        Events(2) = False 'is MM alive?
        Events(3) = False 'is MM invincible (collision will not be registered)?
        Events(4) = False 'is AA guarding/rolling (projectile will be deflected)?
        Events(5) = True 'is AA invincible (collision will not be registered)?
        Events(6) = False 'is MM staggered by AA's body?
        Events(7) = False 'is AA deflecting projectile?

        'initialize sprites for Sprite Projectiles
        AA_ProjCreate1 = New CArrFrame
        AA_ProjCreate1.Insert(191, 22, 184, 14, 202, 29, 1)
        AA_ProjCreate1.Insert(252, 23, 243, 13, 264, 33, 1)

        AA_ProjHorizontal = New CArrFrame
        AA_ProjHorizontal.Insert(162, 22, 155, 13, 172, 30, 1)
        AA_ProjHorizontal.Insert(191, 22, 184, 14, 202, 29, 1)
        AA_ProjHorizontal.Insert(217, 23, 211, 15, 232, 30, 1)

        AA_ProjEightWay = New CArrFrame
        AA_ProjEightWay.Insert(105, 20, 97, 13, 112, 28, 1)
        AA_ProjEightWay.Insert(76, 21, 68, 12, 85, 29, 1)
        AA_ProjEightWay.Insert(134, 21, 128, 15, 141, 28, 1)

        AA_ProjHit = New CArrFrame
        AA_ProjHit.Insert(104, 20, 97, 13, 112, 28, 2)
        AA_ProjHit.Insert(134, 21, 128, 15, 141, 28, 1)
        AA_ProjHit.Insert(76, 21, 68, 12, 85, 29, 1)
        AA_ProjHit.Insert(134, 21, 128, 15, 141, 28, 1)


        'Initialize sprite for megaman
        MM_Spawn = New CArrFrame
        MM_Spawn.Insert(657, 803, 651, 794, 674, 812, 20)
        MM_Spawn.Insert(538, 661, 523, 643, 554, 678, 2)
        MM_Spawn.Insert(657, 803, 651, 794, 674, 812, 2)
        MM_Spawn.Insert(538, 661, 523, 643, 554, 678, 2)
        MM_Spawn.Insert(657, 803, 651, 794, 674, 812, 2)
        MM_Spawn.Insert(538, 661, 523, 643, 554, 678, 2)
        MM_Spawn.Insert(657, 803, 651, 794, 674, 812, 2)
        MM_Spawn.Insert(538, 661, 523, 643, 554, 678, 2)
        MM_Spawn.Insert(657, 803, 651, 794, 674, 812, 2)

        MM_Run = New CArrFrame
        MM_Run.Insert(538, 720, 523, 703, 554, 738, 1)
        MM_Run.Insert(487, 721, 477, 702, 498, 737, 1)
        MM_Run.Insert(447, 722, 437, 703, 461, 739, 1)
        MM_Run.Insert(394, 721, 379, 705, 412, 740, 1)
        MM_Run.Insert(340, 720, 325, 705, 360, 739, 1)
        MM_Run.Insert(291, 723, 279, 705, 306, 739, 1)
        MM_Run.Insert(242, 719, 231, 701, 254, 736, 1)
        MM_Run.Insert(194, 720, 182, 701, 208, 737, 1)
        MM_Run.Insert(149, 718, 133, 702, 164, 737, 1)
        MM_Run.Insert(93, 720, 75, 704, 110, 738, 1)
        MM_Run.Insert(43, 721, 28, 704, 58, 738, 1)

        MM_Shoot = New CArrFrame
        MM_Shoot.Insert(492, 660, 477, 644, 508, 679, 3)
        MM_Shoot.Insert(446, 659, 431, 643, 461, 678, 2)

        MM_Die = New CArrFrame
        MM_Die.Insert(399, 659, 385, 643, 412, 678, 2)
        MM_Die.Insert(358, 658, 342, 643, 372, 678, 2)
        MM_Die.Insert(657, 803, 651, 794, 674, 812, 2)
        MM_Die.Insert(358, 658, 342, 643, 372, 678, 2)
        MM_Die.Insert(657, 803, 651, 794, 674, 812, 2)
        MM_Die.Insert(358, 658, 342, 643, 372, 678, 2)
        MM_Die.Insert(657, 803, 651, 794, 674, 812, 2)
        MM_Die.Insert(358, 658, 342, 643, 372, 678, 8)

        MM_Staggered = New CArrFrame
        MM_Staggered.Insert(399, 659, 385, 643, 412, 678, 8)

        MM_JumpStart = New CArrFrame
        MM_JumpStart.Insert(269, 775, 255, 759, 285, 792, 3)


        MM_Jump = New CArrFrame
        MM_Jump.Insert(538, 777, 525, 759, 550, 797, 2)
        MM_Jump.Insert(498, 777, 490, 758, 506, 800, 2)
        MM_Jump.Insert(458, 777, 449, 757, 469, 801, 20)

        MM_JumpDown = New CArrFrame
        MM_JumpDown.Insert(416, 779, 405, 760, 429, 800, 1)

        MM_JumpEnd = New CArrFrame
        MM_JumpEnd.Insert(370, 779, 358, 761, 386, 801, 2)
        MM_JumpEnd.Insert(321, 777, 308, 760, 333, 799, 2)
        MM_JumpEnd.Insert(269, 775, 255, 759, 285, 792, 3)

        MM_Stand = New CArrFrame
        MM_Stand.Insert(538, 661, 523, 643, 554, 678, 1)

        'initialize sprites for Sprite Projectiles
        MM_ProjCreate1 = New CArrFrame
        MM_ProjCreate1.Insert(191, 22, 184, 14, 202, 29, 1)
        MM_ProjCreate1.Insert(252, 23, 243, 13, 264, 33, 1)

        MM_ProjHorizontal = New CArrFrame
        MM_ProjHorizontal.Insert(162, 22, 155, 13, 172, 30, 1)
        MM_ProjHorizontal.Insert(191, 22, 184, 14, 202, 29, 1)
        MM_ProjHorizontal.Insert(217, 23, 211, 15, 232, 30, 1)

        MM_ProjHit = New CArrFrame
        MM_ProjHit.Insert(104, 20, 97, 13, 112, 28, 2)
        MM_ProjHit.Insert(134, 21, 128, 15, 141, 28, 1)
        MM_ProjHit.Insert(76, 21, 68, 12, 85, 29, 1)
        MM_ProjHit.Insert(134, 21, 128, 15, 141, 28, 1)


        bmp = New Bitmap(Img.Width, Img.Height)

        Dim MMHitbox = New HitboxClass
        Dim AAHitbox = New HitboxClass
        AllCharHitboxes.MegamanHitbox = MMHitbox
        AllCharHitboxes.ArmoredArmadilloHitbox = AAHitbox



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
            Dim imgx, imgy As Integer
            If cc.FDir = FaceDir.Left Then
                Dim spriteleft As Integer = cc.PosX - EF.CtrPoint.x + EF.Left
                Dim spritetop As Integer = cc.PosY - EF.CtrPoint.y + EF.Top
                'set mask
                For i = 0 To spritewidth
                    For j = 0 To spriteheight
                        imgx = spriteleft + i
                        imgy = spritetop + j
                        If imgx >= 0 And imgx <= Img.Width - 1 And imgy >= 0 And imgy <= Img.Height - 1 Then
                            Img.Elmt(spriteleft + i, spritetop + j) = OpAnd(Img.Elmt(spriteleft + i, spritetop + j), SpriteMask.Elmt(EF.Left + i, EF.Top + j))
                        End If
                    Next
                Next

                'set sprite
                For i = 0 To spritewidth
                    For j = 0 To spriteheight
                        imgx = spriteleft + i
                        imgy = spritetop + j
                        If imgx >= 0 And imgx <= Img.Width - 1 And imgy >= 0 And imgy <= Img.Height - 1 Then
                            Img.Elmt(spriteleft + i, spritetop + j) = OpOr(Img.Elmt(spriteleft + i, spritetop + j), SpriteMap.Elmt(EF.Left + i, EF.Top + j))
                        End If
                    Next
                Next
            Else 'facing right
                Dim spriteleft = cc.PosX + EF.CtrPoint.x - EF.Right
                Dim spritetop = cc.PosY - EF.CtrPoint.y + EF.Top
                'set mask
                For i = 0 To spritewidth
                    For j = 0 To spriteheight
                        imgx = spriteleft + i
                        imgy = spritetop + j
                        If imgx >= 0 And imgx <= Img.Width - 1 And imgy >= 0 And imgy <= Img.Height - 1 Then
                            Img.Elmt(spriteleft + i, spritetop + j) = OpAnd(Img.Elmt(spriteleft + i, spritetop + j), SpriteMask.Elmt(EF.Right - i, EF.Top + j))
                        End If
                    Next
                Next

                'set sprite
                For i = 0 To spritewidth
                    For j = 0 To spriteheight
                        imgx = spriteleft + i
                        imgy = spritetop + j
                        If imgx >= 0 And imgx <= Img.Width - 1 And imgy >= 0 And imgy <= Img.Height - 1 Then
                            Img.Elmt(spriteleft + i, spritetop + j) = OpOr(Img.Elmt(spriteleft + i, spritetop + j), SpriteMap.Elmt(EF.Right - i, EF.Top + j))
                        End If
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
            CC.Update(AllCharHitboxes, Events)
        Next

        AllCharHitboxes.ArmoredArmadilloHitbox.UpdateHitbox(AA)

        If AA.CurrState = StateArmoredArmadillo.ShootArmored And AA.CurrFrame = 2 Then
            CreateArmoredArmadilloProjectile()
        End If

        If AA.CurrState = StateArmoredArmadillo.DeflectSpecial And AA.FrameIdx = 9 And AA.CurrFrame = 0 Then
            CreateArmoredArmadilloProjectileSpecial()
        End If

        If Events(2) = False Then
            SpawnMegaman(MM)
        Else
            AllCharHitboxes.MegamanHitbox.UpdateHitbox(MM)
            If MM.CurrState = StateMegaman.Shoot And MM.CurrFrame = 2 Then
                CreateMegamanProjectile()
            End If
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

    Sub CreateArmoredArmadilloProjectile()
        Dim AAP As CCharArmoredArmadilloProjectile

        AAP = New CCharArmoredArmadilloProjectile
        If AA.FDir = FaceDir.Left Then
            AAP.PosX = AA.PosX - 20
            AAP.FDir = FaceDir.Left
        Else
            AAP.PosX = AA.PosX + 20
            AAP.FDir = FaceDir.Right
        End If

        AAP.PosY = AA.PosY - 10

        AAP.Vx = 0
        AAP.Vy = 0
        AAP.CurrState = StateArmoredArmadilloProjectile.Create
        ReDim AAP.ArrSprites(2)
        AAP.ArrSprites(0) = AA_ProjCreate1
        AAP.ArrSprites(1) = AA_ProjHorizontal
        AAP.ArrSprites(2) = AA_ProjHit

        ListChar.Add(AAP)
    End Sub

    Sub CreateArmoredArmadilloProjectileSpecial()
        Dim EightWayList(8) As CCharArmoredArmadilloProjectile
        Dim velocity(,) As Double = {
            {6, 0},
            {4.24, 4.24},
            {0, 6},
            {-4.24, 4.24},
            {-6, 0},
            {-4.24, -4.24},
            {0, -6},
            {4.24, -4.24}
        }

        Dim i As Integer = 0
        While i < 8
            Dim AAP As New CCharArmoredArmadilloProjectile
            AAP.PosX = AA.PosX
            AAP.PosY = AA.PosY - 10
            AAP.Vx = velocity(i, 0)
            AAP.Vy = velocity(i, 1)
            AAP.CurrState = StateArmoredArmadilloProjectile.EightWay
            AAP.IdxArrSprites = 3
            ReDim AAP.ArrSprites(3)
            AAP.ArrSprites(0) = AA_ProjCreate1
            AAP.ArrSprites(1) = AA_ProjHorizontal
            AAP.ArrSprites(2) = AA_ProjHit
            AAP.ArrSprites(3) = AA_ProjEightWay

            ListChar.Add(AAP)
            i = i + 1
        End While
    End Sub

    Sub SpawnMegaman(ByRef MM As CCharMegaman)
        Dim rnd As New Random()
        Events(2) = True 'MM is alive
        Events(3) = True 'MM is invinclible in this state

        MM = New CCharMegaman

        Dim RandomizedSpawnXPos As Integer = rnd.Next(60, 269 + 1)
        MM.PosX = RandomizedSpawnXPos

        If MM.PosX >= 164 Then
            MM.FDir = FaceDir.Left
            MM.Vx = -5
        Else
            MM.FDir = FaceDir.Right
            MM.Vx = 5
        End If
        MM.PosY = 238

        MM.Vy = 0
        MM.State(StateMegaman.Spawn, 0)
        ReDim MM.ArrSprites(10)
        MM.ArrSprites(0) = MM_Spawn
        MM.ArrSprites(1) = MM_Run
        MM.ArrSprites(2) = MM_Shoot
        MM.ArrSprites(3) = MM_Die
        MM.ArrSprites(4) = MM_JumpStart
        MM.ArrSprites(5) = MM_Jump
        MM.ArrSprites(6) = MM_JumpEnd
        MM.ArrSprites(7) = MM_Stand
        MM.ArrSprites(8) = MM_JumpDown
        MM.ArrSprites(9) = MM_Staggered
        ListChar.Add(MM)
    End Sub

    Sub CreateMegamanProjectile()
        Dim MMP As CCharMegamanProjectile

        MMP = New CCharMegamanProjectile
        If MM.FDir = FaceDir.Left Then
            MMP.PosX = MM.PosX - 20
            MMP.FDir = FaceDir.Left
        Else
            MMP.PosX = MM.PosX + 20
            MMP.FDir = FaceDir.Right
        End If

        MMP.PosY = MM.PosY

        MMP.Vx = 0
        MMP.Vy = 0
        MMP.CurrState = StateArmoredArmadilloProjectile.Create
        ReDim MMP.ArrSprites(2)
        MMP.ArrSprites(0) = MM_ProjCreate1
        MMP.ArrSprites(1) = MM_ProjHorizontal
        MMP.ArrSprites(2) = MM_ProjHit

        ListChar.Add(MMP)
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
            Events(4) = True
        ElseIf e.KeyCode = Keys.Down And AA.CurrState = StateArmoredArmadillo.Guard Then
            AA.isGuarding = False
        ElseIf e.KeyCode = Keys.S And AA.CurrState = StateArmoredArmadillo.StandArmored Then
            AA.isShooting = True
        ElseIf e.KeyCode = Keys.Space And AA.CurrState = StateArmoredArmadillo.StandArmored Then
            AA.isInRollingAnimation = True
        End If
    End Sub

End Class
