Option Explicit
Dim fso, winShell, MyTarget, MySource, file, zipFile
Set fso = CreateObject("Scripting.FileSystemObject")
Set winShell = createObject("Shell.Application")

MyTarget = Wscript.Arguments.Item(0)
MySource = Wscript.Arguments.Item(1)

Wscript.Echo "Adding " & MySource & " to " & MyTarget

'create a new clean zip archive
Set zipFile = winShell.NameSpace(MyTarget)
if zipFile is Nothing Then
    Set file = fso.CreateTextFile(MyTarget, True)
    file.write("PK" & chr(5) & chr(6) & string(18,chr(0)))
    file.close
    Set file = Nothing
End If

Set zipFile = winShell.NameSpace(MyTarget)
zipFile.CopyHere MySource, &H10& 'winShell.NameSpace(MySource).Items if it's a folder
wscript.sleep 500

'do until zipFile.items.count = 1 'winShell.namespace(MySource).items.count
'    wscript.sleep 500
'loop

Set winShell = Nothing
Set fso = Nothing