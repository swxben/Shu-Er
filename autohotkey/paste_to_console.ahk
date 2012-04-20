; Paste to console

#NoEnv
SendMode Input
SetWorkingDir %A_ScriptDir%

#IfWinActive ahk_class ConsoleWindowClass
	; Remap ^V to alt-space e p (paste via the window menu)
	^V::		
		Send !{Space}ep
	return
#IfWinActive
